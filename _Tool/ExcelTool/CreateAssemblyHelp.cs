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

namespace ExcelTool
{
    static class CreateAssemblyHelp
    {
        public static bool IsDefDll = true;
        
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

            cp.OutputAssembly = MainMgr.Instance.OutClassPath+@"\ExcelClass.dll";
            
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

            cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
        }

       

        public static Assembly ExcelDataToAssembly(List<ExcelData> dataList)
        {
            List<string> allClassval = new List<string>();
            List<string> allClassname = new List<string>();
            
            foreach (var data in dataList)
            {
                Console.WriteLine("解析ExcelData: " + data.Name);
                foreach (DataTable item in data.Sheets)
                {
                    if (item.Rows.Count<3)
                    {
                        continue;
                    }
                    Console.WriteLine("解析表："+item.TableName);
                    DataRow field_Names =item.Rows[0];
                    DataRow field_description = item.Rows[1];
                    DataRow field_Types = item.Rows[2];
                    string classStr = ParsingHeaders(item,field_Names,field_description, field_Types);
                    allClassval.Add(classStr);
                    allClassname.Add(item.TableName);
                    Console.WriteLine("解析"+item.TableName+"类文件成功");
                }
            }
            //将所有类写入程序集
            Assembly assembly = WriteInAssembly(allClassname,allClassval);
            return assembly; 
        }

        private static string ParsingHeaders(DataTable item, DataRow field_Names, DataRow field_description, DataRow field_Types)
        {
            //获取模板
            string classVal = GetTemplateClass();
            classVal = classVal.Replace("#Name",item.TableName);
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

        private static string GetTemplateClass()
        {
            System.Reflection.Assembly app = System.Reflection.Assembly.GetExecutingAssembly();

            string[] xx = app.GetManifestResourceNames();

            using (System.IO.Stream ms = app.GetManifestResourceStream("ExcelTool.VoClass.ExcelData.cs"))
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
                if (v.Key== ConsoleKey.Spacebar)
                {
                    cp.GenerateInMemory = false;
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
                StringColor.WriteLine("编译程序集失败");
                for (int i = 0; i < allClassName.Count; i++)
                {
                    WriteIn2Cs(allClassName[i],allClassVal[i]);
                }
            }
            else
            { 
                assembly = result.CompiledAssembly;
                StringColor.WriteLine("编译程序集成功",ConsoleColor.Green);
                if (cp.GenerateInMemory)
                {
                    StringColor.WriteLine("生成Dll成功",ConsoleColor.Green);
                }
                else
                {
                    for (int i = 0; i < allClassName.Count; i++)
                    {
                        WriteIn2Cs(allClassName[i],allClassVal[i]);
                    }
                }
            }
            return assembly;
        }

        private static void WriteIn2Cs(string name,string val)
        {
            DirectoryInfo infor  = Directory.CreateDirectory(MainMgr.Instance.OutClassPath+@"\"+name+"VO_AutoCreate");
            using( System.IO.StreamWriter file = new System.IO.StreamWriter( infor.FullName+@"\"+name+"VO.cs"))
            {
                file.Write(val);
                Console.WriteLine("生成类文件成功:"+name+"VO.cs");
            }
        }    
        


    }
}
