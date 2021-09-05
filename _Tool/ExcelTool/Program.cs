using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelTool.Tool;
using System.Threading;
using System.Threading.Tasks;
using ExcelTool.Data;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text;
using System.Reflection.Emit;

namespace ExcelTool
{
    class Program
    {

        private static string readExcelPath = String.Empty;
        private static string outDataPath = String.Empty;
        private static string outClassPath = String.Empty;
        private static List<ExcelData> excelDataLst = new List<ExcelData>();


        static void Main(string[] args)
        {
          
            Task task = new Task(Init);
            task.Start();
            Task.WaitAll(task);
            Console.WriteLine("                                ");
            StringColor.WriteLine("*****************************", ConsoleColor.Yellow);
            Console.WriteLine("按任意键关闭");
            Console.ReadKey();
        }

        private static void Init()
        {
            Console.WriteLine("Init");
            //读取路径
            SetPath();
            //获取表路劲
            List<string> path = FindAllExcelPath();
            //创建数据组
            foreach (var VARIABLE in path)
            {
                ExcelData item = new ExcelData(VARIABLE);
                excelDataLst.Add(item);
            }
            StringColor.WriteLine("读取表完成,读取数量:"+excelDataLst.Count, ConsoleColor.Green);
            //检测输出目录
            CheckOut();


        }

        private static void CheckOut()
        {
            try
            {
                DirectoryInfo theFolder = new DirectoryInfo(outDataPath);
                if (theFolder.Exists)
                {
                    DelectDir(theFolder);
                    CreateClass();

                }
                else
                {
                    StringColor.WriteLine("打表输出目录不存在");
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private static void CreateClass()
        {
            foreach (var data in excelDataLst)
            {
                CreateClassHelp.ExcelDataToAssembly(data);
            }
           


        }




        public static void DelectDir(DirectoryInfo dir)
        {
            try
            {
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            Console.WriteLine("删除输出文件夹文件完成");
        }

        private static void SetPath()
        {
            string _read = "ReadExcelPath:";
            string _write_Class = "OutClassPath:";
            string _write_Data = "OutDataPath:";
            string path = System.IO.Directory.GetCurrentDirectory();
            path += @"\Path.txt";
            try
            {
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains(_read))
                        {
                            readExcelPath = line.Replace(_read, String.Empty);
                            continue;
                        }
                        if (line.Contains(_write_Class))
                        {
                            outDataPath = line.Replace(_write_Class, String.Empty);
                            continue;
                        }
                        if (line.Contains(_write_Data))
                        {
                            outClassPath = line.Replace(_write_Data, String.Empty);
                            continue;
                        }
                    }
                }

                if (readExcelPath == String.Empty)
                {
                    StringColor.WriteLine("ReadExcelPath:" + "无路径");
                    Console.ReadKey(true);
                }
                if (outDataPath == String.Empty)
                {
                    StringColor.WriteLine("OutClassPath:" + "无路径");
                    Console.ReadKey(true);
                }
                if (outDataPath == String.Empty)
                {
                    StringColor.WriteLine("OutDataPath:" + "无路径");
                    Console.ReadKey(true);
                }
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                StringColor.WriteLine("读取路劲失败");
                StringColor.WriteLine(e.Message);
                Console.ReadKey(true);
            }
            Console.WriteLine("读取路劲成功");
        }

        private static List<string> FindAllExcelPath()
        {
            List<string> pathStr = new List<string>();
            try
            {
                DirectoryInfo theFolder = new DirectoryInfo(readExcelPath);
                foreach (FileInfo nextFile in theFolder.GetFiles())
                {

                    if (nextFile.Extension == ".xlsx"|| nextFile.Extension == ".xls")
                    {
                        pathStr.Add(nextFile.FullName);
                        Console.WriteLine("获取文件路劲:" + nextFile.FullName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("读取读取路劲下的配置表失败");
                throw;

            }

            return pathStr;
        }

    }
}
