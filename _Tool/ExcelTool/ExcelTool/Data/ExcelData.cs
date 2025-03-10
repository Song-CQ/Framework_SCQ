using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using ExcelTool.Tool;
using Newtonsoft.Json;

namespace ExcelTool.Data
{

    public class DataTableItem
    {
        public string TableName => Sheet.TableName;
        public bool IsStart ;
        public DataTable Sheet;

    }
    
    public class ExcelData
    {
        private DataSet mData;

        public string Name { get; private set; }

        public int DataTableCount;

        public ExcelData(string filePath)
        {

            try
            {
                if (filePath.Contains("静态配置表"))
                {
                    //IsStart = true;
                }
                else if (filePath.Contains("游戏配置表"))
                {
                    //IsStart = false;
                }
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)

                    string[] names = filePath.Split(@"\"[0]);
                    Name = names[names.Length - 1];
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Use the AsDataSet extension method
                        // The result of each spreadsheet is in result.Tables
                        var result = reader.AsDataSet();
                        this.mData = result;
                        StringColor.WriteLine("路劲:" + filePath + "\n读取表:" + Name + "成功", ConsoleColor.Green);
                    }
                    
                }
            }
            catch (Exception e)
            {
                StringColor.WriteLine("读取表" + filePath + "失败");
                throw e;
            }

            DataTableCount = mData.Tables.Count;
            Sheets = new List<DataTableItem>();

            if (mData.Tables.Count < 1)
            {
                StringColor.WriteLine(filePath + "表为空", ConsoleColor.Blue);
            }
            else
            {
                for (int i = 0; i < mData.Tables.Count; i++)               
                {
                    DataTable Sheet = mData.Tables[i];
                    bool IsStart = false;

                    bool IsHaveStaticKeyRow = false;
                    bool IsHaveStaticTypeRow = false;

                    if (Sheet.Rows.Count == 0)
                    {
                        continue;
                    }

                    DataRow dataRow = Sheet.Rows[0];
                    foreach (DataColumn itemColumn in Sheet.Columns)
                    {
                        if (dataRow[itemColumn].ToString().ToLower().Trim() == "statickey")
                        {
                            IsHaveStaticKeyRow = true;
                        }
                        if (dataRow[itemColumn].ToString().ToLower().Trim() == "statictype")
                        {
                            IsHaveStaticTypeRow = true;
                        }
                    }
                    if (IsHaveStaticKeyRow && IsHaveStaticTypeRow)
                    {
                        IsStart = true;
                    }
                    else
                    {
                        IsStart = false;

                        if (Sheet.Rows.Count < 4)
                        {
                            continue;
                        }
           
                        string _fieldName = Sheet.Rows[0][0].ToString().Trim();
                        string _fieldName2 = Sheet.Rows[1][0].ToString().Trim();
                        if (_fieldName != "id" || _fieldName2!="int")
                        {
                            StringColor.WriteLine( Sheet.TableName + "表结构不符合约定，已跳过", ConsoleColor.Red);
                            if(_fieldName != "id")
                                StringColor.WriteLine("表头第一位字段 "+_fieldName  + " != id ", ConsoleColor.Red);
                            if (_fieldName2 != "int")                        
                                    StringColor.WriteLine("第二位id字段 的数据结构" + _fieldName + " != int ", ConsoleColor.Red);

                            continue;
                        }

                    }




                    DataTableItem dataTableEX = new DataTableItem();
                    dataTableEX.IsStart = IsStart;
                    dataTableEX.Sheet = Sheet;

                    Sheets.Add(dataTableEX);
                }

                

            }
        }

        public List<DataTableItem> Sheets { private set; get;}
        

     

    }

    public static class ExcelTool
    {
        /// <summary>
        /// 移除表名的注释
        /// </summary>
        public static string RemoveTableNameAnnotation(this string name)
        {
            return name.Split('-')[0];
        }

    }
}