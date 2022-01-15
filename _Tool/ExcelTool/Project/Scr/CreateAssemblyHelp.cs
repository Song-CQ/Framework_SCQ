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
        public static bool IsCreateDll = true;
        
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

            cp.OutputAssembly = MainMgr.Instance.CurrentDirectory;

            // Generate debug information.

            cp.IncludeDebugInformation = true;

            // Save the assembly as a physical file.

            cp.GenerateInMemory = true;

            cp.OutputAssembly = MainMgr.Instance.CurrentDirectory+ @"\VoClassLib.dll";
            
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
            List<string> allStaticVO = new List<string>();
            foreach (var data in dataList)
            {
                DataTable item = data.Sheet;
                string tableName = item.TableName.RemoveTableNameAnnotation();
                if (data.IsStart)
                {
                    if (item.Rows.Count < 4)
                    {
                        continue;
                    }
                    Console.WriteLine("解析表: " + data.Name);
                    string classStr = ParsingStaticHeaders(data);
                    if (classStr!=null||classStr!=string.Empty)
                    {
                        allClassval.Add(classStr);
                        allClassname.Add(tableName + "StaticVO");
                        allStaticVO.Add(tableName);
                    }
                   
                }
                else
                {
                    if (item.Rows.Count < 4)
                    {
                        continue;
                    }
                    Console.WriteLine("解析表: " + data.Name);
                    DataRow field_Names = item.Rows[0];
                    DataRow field_Types = item.Rows[1];
                    DataRow field_description = item.Rows[2];
           

                    string classStr = ParsingHeaders(data, field_Names, field_description, field_Types);
                    allClassval.Add(classStr);
                    allClassname.Add(tableName + "VO");

                    string VoClassStr = ParsingCreateVoModel(data, field_Names, field_description, field_Types);
                    allClassval.Add(VoClassStr);
                    allClassname.Add(tableName + "VOModel");
                    allModel.Add(tableName);

                    string staticKeyStr = ParsingstaticKey(data);
                    if (staticKeyStr != string.Empty)
                    {
                        allClassval.Add(staticKeyStr);
                        allClassname.Add(tableName + "StaticKey");
                    }
                    
                }
                StringColor.WriteLine("解析" + tableName + "表成功", ConsoleColor.Green);
                
            }
            //创建表数据管理器类
            CreateDataModeMgrToClass(allStaticVO,allModel);
            //创建打表Version
            CreateVOVersionToClass();
            //将所有类写入程序集
            Assembly assembly = WriteInAssembly(allClassname,allClassval);
            
            return assembly; 
        }
        

        private static string ParsingStaticHeaders(ExcelData data)
        {
            DataTable item = data.Sheet;
            DataRow field_Names = item.Rows[0];
            string id = "id";
            string staticKey = "statickey";
            string staticDesc = "staticdesc";
            string staticType = "statictype";
            string staticValue = "staticvalue";

            DataColumn idColumn = null;
            DataColumn staticKeyColumn = null;
            DataColumn staticDescColumn = null;
            DataColumn staticTypeColumn = null;
            DataColumn staticValueColumn = null;
            
            foreach (DataColumn row in item.Columns)
            {
                if (field_Names[row].ToString().Trim().ToLower()==id)
                {
                    idColumn = row;
                }
                if (field_Names[row].ToString().Trim().ToLower()==staticKey)
                {
                    staticKeyColumn = row;
                }
                if (field_Names[row].ToString().Trim().ToLower()==staticDesc)
                {
                    staticDescColumn = row;
                }
                if (field_Names[row].ToString().Trim().ToLower()==staticType)
                {
                    staticTypeColumn = row;
                }
                if (field_Names[row].ToString().Trim().ToLower()==staticValue)
                {
                    staticValueColumn = row;
                }
            }
            if (idColumn==null||staticKeyColumn==null||staticDescColumn==null||staticTypeColumn==null||staticValueColumn==null)
            {
                return null;
            }
            /*
                /// <summary>
                /// id = #id
                /// #描述
                /// #Val
                /// </summary>
                public #Type #Key = #ValData;
            */
            string val = "\n"+
                         "\n        /// <summary>" +
                         "\n        /// id = #id" +
                         "\n        /// #描述" +
                         "\n        /// #Val" +
                         "\n        /// </summary>" +
                         "\n        public #Type #Key#FieldData;";
            svBuilder.Clear();
            for (int i = 4; i < item.Rows.Count; i++)
            {
                DataRow row = item.Rows[i];
                string line = val;
                line = line.Replace("#id",row[idColumn].ToString());
                line = line.Replace("#描述",row[staticDescColumn].ToString());
                line = line.Replace("#Val",row[staticValueColumn].ToString());
                line = line.Replace("#Type",row[staticTypeColumn].ToString());
                line = line.Replace("#Key",row[staticKeyColumn].ToString());

                string temp = GetValStr(row[staticTypeColumn].ToString(), row[staticValueColumn].ToString());
                if (temp!=string.Empty)
                {
                    temp = " = " + temp;
                }
                line = line.Replace("#FieldData",temp);
                
                svBuilder.Append(line);
            }
            string classVal = GetTemplateClass("ExcelTool.Data.VoStaticExcelTemplate.cs");
            classVal =classVal.Replace("#Name",data.Name);
            classVal = classVal.Replace("#Val",svBuilder.ToString());
            classVal =classVal.Replace("#Class",item.TableName.RemoveTableNameAnnotation()+"StaticVO");
            
            return classVal;
        }

        private static string GetValStr(string type, string val)
        {
            string valStr = String.Empty;
            if (val==String.Empty)
            {
                return valStr;
            }
            switch (type.ToLower())
            {
                case "int":
                case "bool":
                    valStr = val;
                    break;
                case "float":
                    valStr = val+"f";
                    break;
                case "string":
                    valStr = '"'+ val +'"';
                    break;
                case "string[]":
                case "int[]":
                case "float[]":
                case "bool[]":
                    val = val.Replace("[",string.Empty);
                    val = val.Replace("]",string.Empty);
                    
                    type = type.Replace("[",string.Empty);
                    type = type.Replace("]",string.Empty);
                    string[] vals = val.Split(',');
                    val = string.Empty;
                    for (var index = 0; index < vals.Length; index++)
                    {
                        var item = vals[index];
                        val += GetValStr(type, item)+',';
                    }
                    val = val.Remove(val.Length-1);
                    valStr = $"new {type}[]"+'{'+val+'}';
                    break;
            }
            return valStr;
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
            for (var index = 4; index < item.Rows.Count; index++)
            {
                DataRow row = item.Rows[index];
                string key = row[keyColums].ToString();
                if (key == null || key == string.Empty)
                {
                    continue;
                }
                string fieldName = key.Replace(" ","_");
                if (char.IsNumber(fieldName[0]))
                {
                    fieldName = "_" + fieldName;
                }

                string val = $"\n        public const string {fieldName} = " + '"' + key + '"'+";";
                svBuilder.Append(val);
            }
            classVal =classVal.Replace("#Name",excelData.Name);
            classVal = classVal.Replace("#Val",svBuilder.ToString());
            classVal =classVal.Replace("#Class",item.TableName.RemoveTableNameAnnotation()+"StaticKey");
            return classVal;

        }


        private static string ParsingCreateVoModel(ExcelData excelData, DataRow fieldNames, DataRow fieldDescription, DataRow fieldTypes)
        {
            DataTable item = excelData.Sheet;
            string tableName = item.TableName.RemoveTableNameAnnotation();
            //获取模板
            string classVal = GetTemplateClass("ExcelTool.Data.VoModelTemplate.cs");
            classVal = classVal.Replace("#Name",excelData.Name);
            classVal = classVal.Replace("#Class", tableName+"VOModel");
            classVal = classVal.Replace("#DataVo", tableName+"VO");
            
            classVal = classVal.Replace("#SheetName", '"'+tableName+'"');
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
            classVal = classVal.Replace("#Name",excelData.Name.Replace(".xlsx",""));
            classVal = classVal.Replace("#Class", item.TableName.RemoveTableNameAnnotation()+"VO");
            svBuilder.Clear();

            foreach (DataColumn itemColumn in item.Columns)
            {
                string _fieldName = field_Names[itemColumn].ToString().Trim();
                if (_fieldName == "" || defFieldLst.Contains(_fieldName.ToLower()))
                {
                    //忽略默认和空字段
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

            if (IsCreateDll)
            {
                // Console.WriteLine("生成Dll还是CS文件(空格 CS !空格 Dll)");
                // var v =Console.ReadKey();
                // Console.WriteLine(v.Key);
                cp.GenerateInMemory = true;
            }
            else
            {
                cp.GenerateInMemory = false;
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
                    WriteIn2Cs(MainMgr.Instance.OutClassPath+@"\VOClass\"+dir+"VO_AutoCreate",allClassName[i], allClassVal[i]);
                }
                CopyFileToOutClass(MainMgr.Instance.CurrentDirectory + @"\BaseVoClassLib.dll", MainMgr.Instance.OutClassPath+@"\VODll");
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
                    CopyFileToOutClass(MainMgr.Instance.CurrentDirectory + @"\BaseVoClassLib.dll", MainMgr.Instance.OutClassPath + @"\VODll");
                    CopyFileToOutClass(MainMgr.Instance.CurrentDirectory+@"\VoClassLib.dll",MainMgr.Instance.OutClassPath + @"\VODll", true);
                    FileInfo fieldInfo = new FileInfo(MainMgr.Instance.CurrentDirectory+@"\VoClassLib.pdb");
                    fieldInfo.Delete();
                }
                else
                {
                    CopyFileToOutClass(MainMgr.Instance.CurrentDirectory + @"\BaseVoClassLib.dll",MainMgr.Instance.OutClassPath + @"\VODll");
                    for (int i = 0; i < allClassName.Count; i++)
                    {
                        string dir = GetClassNameDir(allClassName[i]);
                      
                        WriteIn2Cs(MainMgr.Instance.OutClassPath+ @"\VOClass\" + dir+"VO_AutoCreate",allClassName[i], allClassVal[i]);
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
            if (name.Contains("StaticVO"))
            {
                dir = @"StaticVO\" + dir;
            }
            else 
            {
                dir = @"VO\" + dir;
            }
            return dir;
        }

        /// <summary>
        /// 创建打表Version
        /// </summary>
        private static void CreateVOVersionToClass()
        {
            string val = GetTemplateClass("ExcelTool.Data.VersionTemplate.cs");
            val = val.Replace("#Time",DateTime.Now.ToString("yyyyMMddHHmmss"));
            WriteIn2Cs(MainMgr.Instance.OutClassPath+ @"\VOVersion", "ConfigVOVersion_AutoCreator", val);
        }
        /// <summary>
        /// 创建表格管理器
        /// </summary>
        /// <param name="allStaticVo"></param>
        /// <param name="allModel"></param>
        private static void CreateDataModeMgrToClass(List<string> allStaticVo, List<string> allModel)
        {
            string setStaticDataToDic = string.Empty;
            string setDataToDic = string.Empty;
            string init = string.Empty;
            string setDataModel = string.Empty;
            foreach (var tableName in allStaticVo)
            {
                string setDataVal = "\n            "+tableName+"StaticVO.SetData(GetStaticExcalData<" + tableName+"StaticVO"+">("+'"'+tableName+'"'+"));";
                setStaticDataToDic += setDataVal;
            }
            foreach (var tableName in allModel)
            {
                string className = tableName + "VOModel";
                string setDataVal = "\n            SetExcalData<"+tableName+"VO"+">("+'"'+tableName+'"'+");";
                setDataToDic += setDataVal;
                init += "\n            " + className + ".Instance.Init();";
                setDataModel += "\n            "+ className + ".Instance.SetData(excelDataStrDic[typeof("+tableName+"VO"+")]"+" as "+tableName+"VO"+"[]);";
            }
            string mgrTempLate = GetTemplateClass("ExcelTool.Data.ExcelDataMgr.cs");
            mgrTempLate = mgrTempLate.Replace("#IsEnciphermentData",ExcelToAssemblyDataHelp.IsEnciphermentData.ToString().ToLower());
            mgrTempLate = mgrTempLate.Replace("#SetStaticDataToDic",setStaticDataToDic);
            mgrTempLate = mgrTempLate.Replace("#SetDataToDic",setDataToDic);
            mgrTempLate = mgrTempLate.Replace("#Init",init);
            mgrTempLate = mgrTempLate.Replace("#SetDataModel",setDataModel);
            WriteIn2Cs(MainMgr.Instance.OutClassPath+@"\VOMgr", "ExcelMgr_AudioCreator", mgrTempLate);
        }
        
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="CopyPath"></param>
        /// <param name="isDel">是否删除原本文件</param>
        private static void CopyFileToOutClass(string CopyPath,string toPath,bool isDel=false)
        {
            try
            {
                FileInfo fieldInfo = new FileInfo(CopyPath);
                DirectoryInfo infor = Directory.CreateDirectory(toPath);
                fieldInfo.CopyTo(infor.FullName + @"\"+ fieldInfo.Name);
                if (isDel)
                {
                    fieldInfo.Delete();
                }
            }
            catch (Exception e)
            {
                StringColor.WriteLine(e);
                Thread.CurrentThread.Abort();
            }
            
        }
        private static void WriteIn2Cs(string dirInfo,string name,string val)
        {
            DirectoryInfo infor = Directory.CreateDirectory(dirInfo);
            using( System.IO.StreamWriter file = new System.IO.StreamWriter(infor.FullName+@"\"+name+".cs"))
            {
                file.Write(val);
                Console.WriteLine("生成类文件成功:"+name+".cs");
            }
        }    
        
        

    }
}
