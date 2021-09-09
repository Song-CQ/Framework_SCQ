using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ExcelTool.Data;
using ExcelTool.Tool;
using Newtonsoft.Json;

namespace ExcelTool
{
    public static class ExcelToAssemblyDataHelp
    {
 
        private static Assembly _assembly;
        public static void Start(Assembly assembly, List<ExcelData> excelDataLst)
        {
            _assembly = assembly;
            foreach (var  excelData in excelDataLst)
            {
                foreach (DataTable sheet in excelData.Sheets)
                {
                    CreateObj(sheet);
                }
            }
        }


        private static void CreateObj(DataTable sheet)
        {
            if (sheet.Rows.Count<3)
            {
                return;
            }
            Console.WriteLine("开始生成表："+sheet.TableName+"数据");
            string className = "ProjectApp.Data."+sheet.TableName + "VO";
            DataRow field_Names =sheet.Rows[0];
            DataRow field_Types = sheet.Rows[2];
            List<object> myDataLst=new List<object>();
            try
            {
                for (int i = 3; i < sheet.Rows.Count; i++)
                {
                
                    object myObject = _assembly.CreateInstance(className);
                    Type myType = myObject.GetType();
                    DataRow dataRow = sheet.Rows[i];
                    foreach (DataColumn itemColumn in sheet.Columns)
                    {
                        string _fieldName = field_Names[itemColumn].ToString().Trim();
                
                        string _fieldType = field_Types[itemColumn].ToString().Trim();
                    
                        FieldInfo fieldInfo = myType.GetField(_fieldName);
                        if (fieldInfo==null)
                        {
                            StringColor.WriteLine(sheet.TableName + "VO:"+_fieldName+"字段不存在");
                        }
                        object val = ValToObj(fieldInfo.FieldType,dataRow[itemColumn].ToString());
                        
                        fieldInfo.SetValue(myObject, val);
                    }
                    myDataLst.Add(myObject);
                }
            }
            catch (Exception e)
            {
                StringColor.WriteLine(e);
                StringColor.WriteLine("生成表："+sheet.TableName+"数据失败");
                Thread.CurrentThread.Abort();
            }
            string jsonData = JsonConvert.SerializeObject(myDataLst);
            File.WriteAllText(MainMgr.Instance.OutDataPath+@"\"+sheet.TableName+"_Data.txt",jsonData);
            StringColor.WriteLine("生成表："+sheet.TableName+"数据成功",ConsoleColor.Green);
        }
      
        public static object ValToObj(Type thisType,string valString)
        {
            object obj = null; 
           // thisType = GetTypeByString(type);
            if (thisType!=null)
            {
                if (valString==null||valString==string.Empty)
                {
                    obj = thisType.IsValueType ? Activator.CreateInstance(thisType) : null;
                    return obj;
                }
                else
                {
                    if (thisType.IsValueType&&!thisType.IsEnum||thisType == typeof(string))
                    {
                        obj = Convert.ChangeType(valString,thisType);
                        return obj;
                    }
                    else if (thisType.IsArray)
                    {
                        Type Cl_Type = thisType.GetElementType();
                        if (Cl_Type.IsValueType||Cl_Type == typeof(string))
                        {
                           string[] vals = StringToStringArr(valString);
                           Array array = Array.CreateInstance(Cl_Type,vals.Length);
                           for (int i = 0; i < vals.Length; i++)
                           {
                               object o = Convert.ChangeType(vals[i],Cl_Type);
                               array.SetValue(o,i);
                           }
                           obj = array;
                           return obj;
                        }
                    }
                    else if (thisType.HasImplementedRawGeneric(typeof(List<>)))
                    {
                        Type Cl_Type = thisType.GetGenericArguments()[0];
                        if (Cl_Type.IsValueType || Cl_Type == typeof(string))
                        {
                            string[] vals = StringToStringArr(valString);
                            //创建一个list返回
                            obj = Activator.CreateInstance(thisType, new object[]{});
                            MethodInfo methodInfo = thisType.GetMethod("Add");
                            object[] tempObjs = new object[1];
                            for (int i = 0; i < vals.Length; i++)
                            {
                                object o = Convert.ChangeType(vals[i], Cl_Type);
                                tempObjs[0] = o;
                                methodInfo.Invoke(obj, tempObjs);
                            }
                            return obj;
                        }
                    }
                    else if (thisType.HasImplementedRawGeneric(typeof(Dictionary<,>)))
                    {
                       
                        Type Cl_TypeKey = thisType.GetGenericArguments()[0];
                        Type Cl_TypeVal = thisType.GetGenericArguments()[1];
                        if ((Cl_TypeKey.IsValueType || Cl_TypeKey == typeof(string))&&(Cl_TypeVal.IsValueType || Cl_TypeVal == typeof(string)))
                        {
                            StringToStringDic(valString,out  List<string> keys, out  List<string> vals);
                            //创建一个list返回
                            obj = Activator.CreateInstance(thisType,new object[]{});
                            MethodInfo methodInfo = thisType.GetMethod("Add");
                            object[] tempObjs = new object[2];
                            for (int i = 0; i < keys.Count; i++)
                            {
                                 object key_o = Convert.ChangeType(keys[i], Cl_TypeKey);
                                 object val_o = Convert.ChangeType(vals[i], Cl_TypeVal);
                                 tempObjs[0] = key_o;
                                 tempObjs[1] = val_o;
                                 methodInfo.Invoke(obj, tempObjs);
                            }
                            return obj;
                        }
                        
                    }
                }
            }
            StringColor.WriteLine("字段类型"+thisType.ToString()+"不支持");
           
            return obj;
        }
        private static List<string> tempStrLst=new List<string>();
        private static void StringToStringDic(string valStr,out List<string> key, out List<string> val)
        {
            valStr = valStr.Replace("[","{");
            valStr = valStr.Replace("]","}");
            string[] str = valStr.Split('{','}');
            key = new List<string>();
            val = new List<string>();
            foreach (string temp in str)
            {
                if (!string.IsNullOrEmpty(temp))
                {
                    string[] item = temp.Split(',');
                    key.Add(item[0]);
                    val.Add(item[1]);
                }
            }
        }

        /// <summary>
        /// 判断指定的类型 <paramref name="type"/> 是否是指定泛型类型的子类型，或实现了指定泛型接口。
        /// </summary>
        /// <param name="type">需要测试的类型。</param>
        /// <param name="generic">泛型接口类型，传入 typeof(IXxx<>)</param>
        /// <returns>如果是泛型接口的子类型，则返回 true，否则返回 false。</returns>
        public static bool HasImplementedRawGeneric(this Type type, Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));
 
            // 测试接口。
            var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
            if (isTheRawGenericType) return true;
 
            // 测试类型。
            while (type != null && type != typeof(object))
            {
                isTheRawGenericType = IsTheRawGenericType(type);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }
 
            // 没有找到任何匹配的接口或类型。
            return false;
 
            // 测试某个类型是否是指定的原始接口。
            bool IsTheRawGenericType(Type test)
            {
               return generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
            }
            
        }
        public static string[] StringToStringArr(string val)
        {
             val = val.Replace("[",string.Empty);
             val = val.Replace("]",string.Empty);
             val = val.Replace("{",string.Empty);
             val = val.Replace("}",string.Empty);
             string[] arr = val.Split(',');
             return arr;
        }

    }
}