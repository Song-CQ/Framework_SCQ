using ExcelTool.Data;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using ExcelTool.Tool;
using System.Threading;

namespace ExcelTool
{
    static class CreateAssemblyHelp
    {
        public static bool IsDefDll = false;
        
        private static StringBuilder svBuilder;
        
        private static CSharpCodeProvider provider;

        private static CompilerParameters cp;
        
        private static List<string> defFieldLst = new List<string>
        {
            "id","key"
        };
        
        static CreateAssemblyHelp()
        {
            
            svBuilder=new StringBuilder();
            
            //创建编译器实例。 

            provider = new CSharpCodeProvider();

            //设置编译参数。 

            cp = new CompilerParameters();

            
            cp.GenerateExecutable = false;
            
            // Generate an executable instead of

            // a class library.

            //cp.GenerateExecutable = true;

            // Set the assembly file name to generate.

            cp.OutputAssembly = System.IO.Directory.GetCurrentDirectory();

            // Generate debug information.

            cp.IncludeDebugInformation = true;

            // Save the assembly as a physical file.

            cp.GenerateInMemory = true;

            cp.OutputAssembly = Directory.GetCurrentDirectory()+ @"\VoClassLib.dll";
            
            // Set the level at which the compiler

            // should start displaying warnings.

            cp.WarningLevel = 3;

            // Set whether to treat all warnings as errors.

            cp.TreatWarningsAsErrors = false;

            // Set compiler argument to optimize output.

            cp.CompilerOptions = "/optimize";
           

            cp.ReferencedAssemblies.Add("System.dll");

            //cp.ReferencedAssemblies.Add("System.Core.dll");

            cp.ReferencedAssemblies.Add("System.Data.dll");

            //cp.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");

            cp.ReferencedAssemblies.Add("System.Deployment.dll");

            cp.ReferencedAssemblies.Add("System.Design.dll");

            cp.ReferencedAssemblies.Add("System.Drawing.dll");

            cp.ReferencedAssemblies.Add("BaseVoClassLib.dll");
        }

        public static Assembly ExcelDataToAssembly(List<ExcelData> dataList)
        {
            List<string> allClassval = new List<string>();
            List<string> allClassname = new List<string>();
            List<string> allModel = new List<string>();
            foreach (var data in dataList)
            {
                DataTable item = data.Sheet;
                if (data.IsStart)
                {
                    
                }
                else
                {
                    if (item.Rows.Count < 3)
                    {
                        continue;
                    }
                    Console.WriteLine("解析表: " + data.Name);
                    DataRow field_Names = item.Rows[0];
                    DataRow field_description = item.Rows[1];
                    DataRow field_Types = item.Rows[2];

                    string classStr = ParsingHeaders(data, field_Names, field_description, field_Types);
                    allClassval.Add(classStr);
                    allClassname.Add(item.TableName + "VO");

                    string VoClassStr = ParsingCreateVoModel(data, field_Names, field_description, field_Types);
                    allClassval.Add(VoClassStr);
                    allClassname.Add(item.TableName + "VOModel");
                    allModel.Add(item.TableName);

                    string staticKeyStr = ParsingstaticKey(data);
                    if (staticKeyStr != string.Empty)
                    {
                        allClassval.Add(staticKeyStr);
                        allClassname.Add(item.TableName + "StaticKey");
                    }
                    
                }
                StringColor.WriteLine("解析" + item.TableName + "表成功", ConsoleColor.Green);
                
            }
            //创建表数据管理器类
            CreateDataModeMgrToClass(allModel);
            //将所有类写入程序集
            Assembly assembly = WriteInAssembly(allClassname,allClassval);
            
            return assembly; 
        }

        private static string ParsingstaticKey(ExcelData excelData)
        {

            DataColumn keyColums = null;
            DataTable item = excelData.Sheet;
            DataRow field_Names =item.Rows[0];
            foreach (DataColumn VARIABLE in item.Columns)
            {
                string key = field_Names[VARIABLE].ToString();
                if (key.ToLower()=="key")
                {
                    keyColums = VARIABLE;
                }
            }
            if (keyColums==null)
            {
                return String.Empty;
            }
            
            string classVal = GetTemplateClass("ExcelTool.Data.VoStaticKeyTemplate.cs");
            svBuilder.Clear();
            for (var index = 3; index < item.Rows.Count; index++)
            {
                DataRow row = item.Rows[index];
                string key = row[keyColums].ToString();
                if (key == null || key == string.Empty)
                {
                    continue;
                }

                string val = $"\n        public const string {key} = " + '"' + key + '"'+";";
                svBuilder.Append(val);
            }
            classVal =classVal.Replace("#Name",excelData.Name);
            classVal = classVal.Replace("#Val",svBuilder.ToString());
            classVal =classVal.Replace("#Class",item.TableName+"StaticKey");
            return classVal;

        }


        private static string ParsingCreateVoModel(ExcelData excelData, DataRow fieldNames, DataRow fieldDescription, DataRow fieldTypes)
        {
            DataTable item = excelData.Sheet;
            //获取模板
            string classVal = GetTemplateClass("ExcelTool.Data.VoModelTemplate.cs");
            classVal = classVal.Replace("#Name",excelData.Name);
            classVal = classVal.Replace("#Class", item.TableName+"VOModel");
            classVal = classVal.Replace("#DataVo", item.TableName+"VO");
            
            classVal = classVal.Replace("#SheetName", '"'+item.TableName+'"');
            bool isHasKey = false;
            bool isHasId = false;
            svBuilder.Clear();
            foreach (DataColumn itemColumn in item.Columns)
            {
                string _fieldName = fieldNames[itemColumn].ToString().Trim();
                if (_fieldName.ToLower() == "id")
                {
                    isHasId = true;
                }
                if (_fieldName.ToLower() == "key")
                {
                    isHasKey = true;
                }
                
                svBuilder.Append('"');
                svBuilder.Append(_fieldName.ToString()+'"'+',');
            }
            svBuilder.Remove(svBuilder.Length-1,1);
            
            classVal = classVal.Replace("#HasStringKey", isHasKey.ToString().ToLower());
            classVal = classVal.Replace("#HasStringId", isHasId.ToString().ToLower());
            classVal = classVal.Replace("#HasStaticField", "false");
            
            classVal = classVal.Replace("#HeadFields", svBuilder.ToString());

            return classVal;
            
        }

        private static string ParsingHeaders(ExcelData excelData, DataRow field_Names, DataRow field_description, DataRow field_Types)
        {
            DataTable item = excelData.Sheet;
            //获取模板
            string classVal = GetTemplateClass("ExcelTool.Data.VoClassTemplate.cs");
            classVal = classVal.Replace("#Name",excelData.Name);
            classVal = classVal.Replace("#Class", item.TableName+"VO");
            svBuilder.Clear();
            foreach (DataColumn itemColumn in item.Columns)
            {
                string _fieldName = field_Names[itemColumn].ToString().Trim();
                if (defFieldLst.Contains(_fieldName.ToLower()))
                {
                    //忽略默认字段
                    continue;
                }
                string _fieldDescription = field_description[itemColumn].ToString().Trim();
                string _fieldType = field_Types[itemColumn].ToString().Trim();
                
                svBuilder.Append(WrittenField(_fieldType,_fieldName,_fieldDescription));
            }

            classVal = classVal.Replace("#Val", svBuilder.ToString());
            return classVal;
        }

        private static string WrittenField(string field_Type, string field_Name,string _fieldDescription)
        {
            string val =  "\n        /// <summary>\n        /// {0} \n        /// </summary>\n        public {1} {2};\n";
            val = String.Format(val,_fieldDescription,field_Type,field_Name);
            return val;
        }
        
        /// <summary>
        /// 读取模板文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetTemplateClass(string path)
        {
            System.Reflection.Assembly app = System.Reflection.Assembly.GetExecutingAssembly();

            string[] xx = app.GetManifestResourceNames();

            using (System.IO.Stream ms = app.GetManifestResourceStream(path))
            {
                byte[] bs = new byte[ms.Length];
                ms.Read(bs, 0, bs.Length);
                
                string txt = Encoding.UTF8.GetString(bs);
                //Console.WriteLine("读取模板成功");
                return txt;
            }
        }


        public static Assembly WriteInAssembly (List<string> allClassName,List<string> allClassVal)
        {

            if (!IsDefDll)
            {
                Console.WriteLine("生成Dll还是CS文件(空格 CS !空格 Dll)");
                var v =Console.ReadKey();
                Console.WriteLine(v.Key);
                if (v.Key == ConsoleKey.Spacebar)
                {
                    cp.GenerateInMemory = false;
                }
                else
                {
                    cp.GenerateInMemory = true;
                }
            }

            Assembly assembly = null;
            
            Console.WriteLine("开始编译程序集");
            CompilerResults result = provider.CompileAssemblyFromSource(cp, allClassVal.ToArray());

            if (result.Errors.Count > 0)
            {
                for (int i = 0; i < result.Errors.Count; i++)
                {               
                    StringColor.WriteLine(result.Errors[i]);                   
                }
                for (int i = 0; i < allClassName.Count; i++)
                {
                    string dir = GetClassNameDir(allClassName[i]);
                    WriteIn2Cs(MainMgr.Instance.OutClassPath+@"\"+dir+"VO_AutoCreate",allClassName[i], allClassVal[i]);
                }
                CopyFileToOutClass(Directory.GetCurrentDirectory() + @"\BaseVoClassLib.dll");
                StringColor.WriteLine("编译程序集失败");
                
                Thread.CurrentThread.Abort();
            }
            else
            { 
                assembly = result.CompiledAssembly;
                StringColor.WriteLine("编译程序集成功",ConsoleColor.Green);
                
                if (cp.GenerateInMemory)
                {
                    StringColor.WriteLine("生成Dll成功",ConsoleColor.Green);
                    CopyFileToOutClass(Directory.GetCurrentDirectory() + @"\BaseVoClassLib.dll");
                    CopyFileToOutClass(Directory.GetCurrentDirectory()+@"\VoClassLib.dll");
                }
                else
                {
                    CopyFileToOutClass(Directory.GetCurrentDirectory() + @"\BaseVoClassLib.dll");
                    for (int i = 0; i < allClassName.Count; i++)
                    {
                        string dir = GetClassNameDir(allClassName[i]);
                      
                        WriteIn2Cs(MainMgr.Instance.OutClassPath+@"\"+dir+"VO_AutoCreate",allClassName[i], allClassVal[i]);
                    }
                }
            }
            return assembly;
        }

        private static string GetClassNameDir(string name)
        {
            string dir = name.Replace("VO", String.Empty);
            dir = dir.Replace("Model", String.Empty);
            dir = dir.Replace("StaticKey", String.Empty);
            return dir;
        }

        private static void CreateDataModeMgrToClass(List<string> allModel)
        {

            string setDataToDic = string.Empty;
            string init = string.Empty;
            string setDataModel = string.Empty;
            foreach (var tableName in allModel)
            {
                string className = tableName + "VOModel";
                string setDataVal = "\n            SetExcalData<"+tableName+"VO"+">("+'"'+tableName+'"'+");";
                setDataToDic += setDataVal;
                init += "\n            " + className + ".Instance.Init();";
                setDataModel += "\n            "+ className + ".Instance.SetData(excelDataStrDic[typeof("+tableName+"VO"+")]"+" as "+tableName+"VO"+"[]);";
            }
            string mgrTempLate = GetTemplateClass("ExcelTool.Data.ExcelDataMgr.cs");
            mgrTempLate = mgrTempLate.Replace("#SetDataToDic",setDataToDic);
            mgrTempLate = mgrTempLate.Replace("#Init",init);
            mgrTempLate = mgrTempLate.Replace("#SetDataModel",setDataModel);
            mgrTempLate = mgrTempLate.Replace("#OutPath",MainMgr.Instance.OutDataPath);
            WriteIn2Cs(MainMgr.Instance.OutClassPath,"ExcelMgr",mgrTempLate);
        }
        
        private static void CopyFileToOutClass(string path)
        {
            if (true)
            {
                try
                {
                    FileInfo fieldInfo = new FileInfo(path);
                    fieldInfo.CopyTo(MainMgr.Instance.OutClassPath + @"\"+ fieldInfo.Name);
                }
                catch (Exception e)
                {
                    StringColor.WriteLine(e);
                    Thread.CurrentThread.Abort();
                }      
               
            }
        }
        private static void WriteIn2Cs(string dirInfo,string name,string val)
        {
            DirectoryInfo infor  = Directory.CreateDirectory(dirInfo);
            using( System.IO.StreamWriter file = new System.IO.StreamWriter( infor.FullName+@"\"+name+".cs"))
            {
                file.Write(val);
                Console.WriteLine("生成类文件成功:"+name+".cs");
            }
        }    
        


    }
}
