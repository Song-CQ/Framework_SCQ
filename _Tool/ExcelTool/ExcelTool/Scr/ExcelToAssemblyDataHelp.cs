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
using System.Xml.Linq;
using ExcelTool.Data;
using ExcelTool.Tool;
using Newtonsoft.Json;
using ProjectApp.Data;

namespace ExcelTool
{
    public static class ExcelToAssemblyDataHelp
    {

        private static Assembly _assembly;
        private static List<string> tempm_key = new List<string>();
        private static List<string> tempm_Id = new List<string>();

        public static bool IsEnciphermentData = true;
        public static bool IsOutMultipleDatas = false;


        private static Dictionary<string, BaseStaticVO> allConfigData_configStaticVODic = new Dictionary<string, BaseStaticVO>();
        private static Dictionary<string, List<BaseVO>> allConfigData_configVODic = new Dictionary<string, List<BaseVO>>();

        public static void Start(Assembly assembly, List<ExcelData> excelDataLst)
        {

            _assembly = assembly;
            StringColor.WriteLine("表数据是否加密：" + ExcelToAssemblyDataHelp.IsEnciphermentData, ConsoleColor.Yellow);
            StringColor.WriteLine("是否每个表都创建单独的数据文件：" + ExcelToAssemblyDataHelp.IsOutMultipleDatas, ConsoleColor.Yellow);


            foreach (var excelData in excelDataLst)
            {
                if (excelData.IsStart)
                {
                    CreateStartObj(excelData.Name, excelData.Sheet);
                }
                else
                {
                    CreateObj(excelData.Name, excelData.Sheet);
                }
            }

            if (!IsOutMultipleDatas)
            {

                CreateConfigData();
            }
        }

        private static void CreateConfigData()
        {
            try
            {
                object allConfigData = _assembly.CreateInstance("ProjectApp.Data.ConfigData");
                Type configDataType = allConfigData.GetType();

                var files = configDataType.GetFields(BindingFlags.Public | BindingFlags.Instance);

                foreach (var item in allConfigData_configStaticVODic)
                {
                    FieldInfo fieldInfo = configDataType.GetField(item.Key , BindingFlags.Public | BindingFlags.Instance);
                    fieldInfo.SetValue(allConfigData, item.Value);
                }

                foreach (var item in allConfigData_configVODic)
                {
                    FieldInfo fieldInfo = configDataType.GetField(item.Key+"_List", BindingFlags.Public | BindingFlags.Instance);

                    // 获取字段的泛型类型（即 List<T> 中的 T）
                    Type listType = fieldInfo.FieldType;
                    Type itemType = listType.GetGenericArguments()[0]; // 获取 T 的类型


                    //  创建 List<T> 实例
                    object listValue = Activator.CreateInstance(listType);

                    // 获取 List<int> 的 Add 方法
                    MethodInfo addMethod = listValue.GetType().GetMethod("Add");

                    foreach (var i in item.Value)
                    {
                        // 调用 Add 方法
                        addMethod.Invoke(listValue, new[] { i });
                    }
                    
                    fieldInfo.SetValue(allConfigData, listValue);
                }

                string jsonData = JsonConvert.SerializeObject(allConfigData);

                if (IsEnciphermentData)
                {
                    byte[] bytes = AESEncryptUtil.Encrypt(jsonData);
                    File.WriteAllBytes(MainMgr.Instance.OutDataPath + @"\ConfigData.bytes", bytes);
                }
                else
                {
                    File.WriteAllText(MainMgr.Instance.OutDataPath + @"\ConfigData.txt", jsonData);
                }


            }
            catch (Exception e)
            {
                StringColor.WriteLine(e);
                StringColor.WriteLine("生成ConfigData数据失败");
                Thread.CurrentThread.Abort();
            }
        }

        private static void CreateStartObj(string name, DataTable sheet)
        {
            if (sheet.Rows.Count < 3)
            {
                return;
            }
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

            DataRow field_Names = sheet.Rows[0];
            foreach (DataColumn row in sheet.Columns)
            {
                if (field_Names[row].ToString().Trim().ToLower() == id)
                {
                    idColumn = row;
                }
                if (field_Names[row].ToString().Trim().ToLower() == staticKey)
                {
                    staticKeyColumn = row;
                }
                if (field_Names[row].ToString().Trim().ToLower() == staticDesc)
                {
                    staticDescColumn = row;
                }
                if (field_Names[row].ToString().Trim().ToLower() == staticType)
                {
                    staticTypeColumn = row;
                }
                if (field_Names[row].ToString().Trim().ToLower() == staticValue)
                {
                    staticValueColumn = row;
                }
            }
            if (idColumn == null || staticKeyColumn == null || staticDescColumn == null || staticTypeColumn == null || staticValueColumn == null)
            {
                return;
            }
            string tableName = sheet.TableName.RemoveTableNameAnnotation();
            string className = "ProjectApp.Data." + tableName + "StaticVO";
            Console.WriteLine("开始生成静态表：" + name + "数据");
            Console.WriteLine("类名：" + className);
            bool _SetFaceNo = true;

            try
            {

                object myObject = _assembly.CreateInstance(className);
                Type myType = myObject.GetType();
                for (int i = 4; i < sheet.Rows.Count; i++)
                {
                    DataRow dataRow = sheet.Rows[i];
                    string idStr = dataRow[idColumn].ToString().Trim();
                    string keyStr = dataRow[staticKeyColumn].ToString().Trim();
                    string typeStr = dataRow[staticTypeColumn].ToString().Trim();
                    if (keyStr == "" || id == "" || typeStr == "")
                    {
                        ///忽略空
                        continue;
                    }
                    string valStr = dataRow[staticValueColumn].ToString().Trim();
                    FieldInfo fieldInfo = myType.GetField(keyStr);
                    object val = null;

                    try
                    {
                        val = ValToObj(fieldInfo.FieldType, valStr);

                    }
                    catch (Exception e)
                    {

                        StringColor.WriteLine("生成静态表：" + name + "数据失败");
                        LogUtil.LogError("字段值写入数据失败！");
                        LogUtil.LogError("字段名:" + keyStr);
                        LogUtil.LogError("目标类型:" + fieldInfo.FieldType.ToString());
                        LogUtil.LogError("写入值:" + valStr);
                        StringColor.WriteLine(e);
                        Thread.CurrentThread.Abort();



                    }

                    fieldInfo.SetValue(myObject, val);

                }


                if (IsOutMultipleDatas)
                {
                    string jsonData = JsonConvert.SerializeObject(myObject);
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(MainMgr.Instance.OutDataPath + @"\StaticExcelData");
                    if (IsEnciphermentData)
                    {
                        byte[] bytes = AESEncryptUtil.Encrypt(jsonData);
                        File.WriteAllBytes(directoryInfo.FullName + @"\" + tableName + "_StaticData.bytes", bytes);
                    }
                    else
                    {
                        File.WriteAllText(directoryInfo.FullName + @"\" + tableName + "_StaticData.txt", jsonData);
                    }
                }
                else
                {

                    FieldInfo constField = myType.GetField("VOType", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    
                    allConfigData_configStaticVODic.Add(tableName, myObject as BaseStaticVO);

                }



                StringColor.WriteLine("生成静态表：" + name + "数据成功", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                StringColor.WriteLine(e);
                StringColor.WriteLine("生成静态表：" + name + "数据失败");
                Thread.CurrentThread.Abort();
            }


        }


        private static void CreateObj(string Name, DataTable sheet)
        {
            if (sheet.Rows.Count < 3)
            {
                return;
            }
            string tableName = sheet.TableName.RemoveTableNameAnnotation();

            string className = "ProjectApp.Data." + tableName + "VO";
            Console.WriteLine("开始生成表：" + Name + "数据");
            Console.WriteLine("类名：" + className);
            DataRow field_Names = sheet.Rows[0];
            DataRow field_Types = sheet.Rows[2];
            List<object> myDataLst = new List<object>();
            tempm_key.Clear();
            tempm_Id.Clear();

            try
            {
                for (int i = 4; i < sheet.Rows.Count; i++)
                {
                    object myObject = _assembly.CreateInstance(className);
                    Type myType = myObject.GetType();
                    DataRow dataRow = sheet.Rows[i];
                    foreach (DataColumn itemColumn in sheet.Columns)
                    {
                        string _fieldName = field_Names[itemColumn].ToString().Trim();

                        string _fieldType = field_Types[itemColumn].ToString().Trim();
                        if (_fieldName == "" || _fieldType == "")
                        {
                            ///忽略空
                            continue;
                        }

                        FieldInfo fieldInfo = myType.GetField(_fieldName);
                        if (fieldInfo == null)
                        {
                            fieldInfo = myType.GetField(_fieldName.ToLower());
                            if (fieldInfo == null)
                            {
                                StringColor.WriteLine(tableName + "VO:" + _fieldName + "字段不存在");
                            }
                        }

                        string valStr = dataRow[itemColumn].ToString().Trim();
                        object val = null;
                        try
                        {
                            val = ValToObj(fieldInfo.FieldType, valStr);
                        }
                        catch (Exception)
                        {

                            LogUtil.LogError("字段值写入数据失败！");
                            LogUtil.LogError("字段名:" + _fieldName);
                            LogUtil.LogError("目标类型:" + fieldInfo.FieldType.ToString());
                            LogUtil.LogError("写入值:" + valStr);
                            StringColor.WriteLine("生成表：" + Name + "数据失败");
                            Thread.CurrentThread.Abort();
                        }

                        if (_fieldName.ToLower() == "id")
                        {
                            if (valStr == string.Empty || valStr == null)
                            {
                                myObject = null;
                                break;
                            }

                            if (tempm_Id.Contains(valStr))
                            {
                                StringColor.WriteLine(tableName + "表id重复:" + valStr);
                            }
                            tempm_Id.Add(valStr);
                        }
                        if (_fieldName.ToLower() == "key")
                        {
                            if (tempm_key.Contains(valStr))
                            {
                                StringColor.WriteLine(tableName + "表Key重复:" + valStr);
                            }
                            tempm_key.Add(valStr);
                        }
                        fieldInfo.SetValue(myObject, val);
                    }

                    if (myObject != null)
                    {
                        myDataLst.Add(myObject);
                    }
                }
            }
            catch (Exception e)
            {
                StringColor.WriteLine(e);
                StringColor.WriteLine("生成表：" + Name + "数据失败");
                Thread.CurrentThread.Abort();

            }


            if (IsOutMultipleDatas)
            {
                string jsonData = JsonConvert.SerializeObject(myDataLst.ToArray());
                DirectoryInfo directoryInfo = Directory.CreateDirectory(MainMgr.Instance.OutDataPath + @"\ExcelData");
                if (IsEnciphermentData)
                {
                    byte[] bytes = AESEncryptUtil.Encrypt(jsonData);
                    File.WriteAllBytes(directoryInfo.FullName + @"\" + tableName + "_Data.bytes", bytes);
                }
                else
                {
                    File.WriteAllText(directoryInfo.FullName + @"\" + tableName + "_Data.txt", jsonData);
                }
            }
            else
            {
                Type myType = myDataLst[0].GetType();
                FieldInfo constField = myType.GetField("VOType", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                uint key = Convert.ToUInt32(constField.GetValue(myDataLst[0]));
                List<BaseVO> allBaseStatics = new List<BaseVO>();
                foreach (var item in myDataLst)
                {
                    allBaseStatics.Add(item as BaseVO);
                }

                allConfigData_configVODic.Add(tableName, allBaseStatics);

            }



            StringColor.WriteLine("生成表：" + Name + "数据成功", ConsoleColor.Green);
        }


        public static object ValToObj(Type thisType, string valString)
        {
            object obj = null;
            // thisType = GetTypeByString(type);

            if (thisType != null)
            {
                if (valString == null || valString == string.Empty)
                {
                    obj = thisType.IsValueType ? Activator.CreateInstance(thisType) : null;
                    return obj;
                }
                else
                {
                    if (thisType.IsValueType && !thisType.IsEnum || thisType == typeof(string))
                    {
                        obj = Convert.ChangeType(valString, thisType);
                        return obj;
                    }
                    else if (thisType.IsArray)
                    {
                        Type Cl_Type = thisType.GetElementType();
                        if (Cl_Type.IsValueType || Cl_Type == typeof(string))
                        {
                            string[] vals = StringToStringArr(valString);
                            Array array = Array.CreateInstance(Cl_Type, vals.Length);
                            for (int i = 0; i < vals.Length; i++)
                            {
                                object o = Convert.ChangeType(vals[i], Cl_Type);
                                array.SetValue(o, i);
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
                            obj = Activator.CreateInstance(thisType, new object[] { });
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
                        if ((Cl_TypeKey.IsValueType || Cl_TypeKey == typeof(string)) && (Cl_TypeVal.IsValueType || Cl_TypeVal == typeof(string)))
                        {
                            StringToStringDic(valString, out List<string> keys, out List<string> vals);
                            //创建一个list返回
                            obj = Activator.CreateInstance(thisType, new object[] { });
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
            StringColor.WriteLine("字段类型" + thisType.ToString() + "不支持");

            return obj;
        }
        private static List<string> tempStrLst = new List<string>();
        private static void StringToStringDic(string valStr, out List<string> key, out List<string> val)
        {
            valStr = valStr.Replace("[", "{");
            valStr = valStr.Replace("]", "}");
            string[] str = valStr.Split('{', '}');
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
            val = val.Replace("[", string.Empty);
            val = val.Replace("]", string.Empty);
            val = val.Replace("{", string.Empty);
            val = val.Replace("}", string.Empty);
            string[] arr = val.Split(',');
            return arr;
        }

    }
}