using System;
using System.Data;
using System.IO;
using ExcelDataReader;
using ExcelTool.Tool;

namespace ExcelTool.Data
{
    public class ExcelData
    {
        private DataSet mData;

        public string Name { get;private set; }
      

        // TODO: add Sheet Struct Define

        public ExcelData(string filePath) {
            
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    
                    string[] names = filePath.Split(@"\"[0]);
                    Name = names[names.Length - 1];
                    using (var reader = ExcelReaderFactory.CreateReader(stream)) {
                        // Use the AsDataSet extension method
                        // The result of each spreadsheet is in result.Tables
                        var result = reader.AsDataSet();
                        this.mData = result;
                      
                        Console.WriteLine("读取表"+filePath+"成功");
                    }
                }
            }
            catch (Exception)
            {
                StringColor.WriteLine("读取表"+filePath+"失败");
                throw ;
            }
           
            if (this.Sheets.Count < 1) {
                StringColor.WriteLine(filePath+"表为空",ConsoleColor.Blue);
            }
        }

        public DataTableCollection Sheets {
            get {
                return this.mData.Tables;
            }
        }
        
        
    }
}