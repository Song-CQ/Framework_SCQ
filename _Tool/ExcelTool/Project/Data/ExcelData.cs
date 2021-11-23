using System;
using System.Data;
using System.IO;
using ExcelDataReader;
using ExcelTool.Tool;
using Newtonsoft.Json;

namespace ExcelTool.Data
{
    public class ExcelData
    {
        private DataSet mData;

        public string Name { get; private set; }

        public bool IsStart { get; private set; }

        public ExcelData(string filePath)
        {

            try
            {
                if (filePath.Contains("静态配置表"))
                {
                    IsStart = true;
                }
                else if (filePath.Contains("游戏配置表"))
                {
                    IsStart = false;
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

            if (mData.Tables.Count < 1)
            {
                StringColor.WriteLine(filePath + "表为空", ConsoleColor.Blue);
            }
        }

        public DataTable Sheet
        {
            get
            {
                if (mData.Tables.Count < 1)
                {
                    return null;
                }
                return mData.Tables[0];
            }
        }

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