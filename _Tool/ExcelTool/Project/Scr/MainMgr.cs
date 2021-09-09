using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using ExcelTool.Data;
using ExcelTool.Tool;

namespace ExcelTool
{
    public class MainMgr
    {
        private static MainMgr instance;
        public static MainMgr Instance
        {
            get
            {
                if (instance==null)
                {
                    instance=new MainMgr();
                }
                return instance;
            }
        }

        public System.Threading.Tasks.Task CurrTask { get; private set; }
        public string ReadExcelPath { get; private set; }
        public string OutDataPath { get; private set; }
        public string OutClassPath { get; private set; }
        private List<ExcelData> excelDataLst = new List<ExcelData>();

        public void Init(System.Threading.Tasks.Task task)
        {

            CurrTask = task;
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
            StringColor.WriteLine("读取表完成,读取数量:"+excelDataLst.Count, ConsoleColor.Yellow);
            //检测输出目录
            CheckAndDelect(OutDataPath);
            CheckAndDelect(OutClassPath);
            //创建程序集生成data
            CreateAssembly();
          
            
        }
        private void CreateAssembly()
        {
            Assembly assembly = CreateAssemblyHelp.ExcelDataToAssembly(excelDataLst);
            if (assembly!=null)
            {
                //生成data
                ExcelToAssemblyDataHelp.Start(assembly,excelDataLst);
            }
        }
        
        private void CheckAndDelect(string path)
        {
  
            try
            {
                DirectoryInfo theFolder =  Directory.CreateDirectory(path);
                if (theFolder.Exists)
                {
                    DelectDir(theFolder);
                }
                else
                {
                    StringColor.WriteLine(path+"目录不存在");
                  
                }
            }
            catch (Exception e)
            {
                StringColor.WriteLine(e);
                Thread.CurrentThread.Abort();
            }
        }
        public  void DelectDir(DirectoryInfo dir)
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
                StringColor.WriteLine(e);
                Thread.CurrentThread.Abort();
            }
            StringColor.WriteLine("删除"+dir.Name+"文件夹文件完成",ConsoleColor.Yellow);
        }

        private  void SetPath()
        {
            string _read = "ReadExcelPath:";
            string _write_Class = "OutClassPath:";
            string _write_Data = "OutDataPath:";
            string path = System.IO.Directory.GetCurrentDirectory();
            ReadExcelPath = String.Empty;
            OutDataPath = String.Empty;
            OutClassPath = String.Empty;
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
                            ReadExcelPath = line.Replace(_read, String.Empty);
                            continue;
                        }
                        if (line.Contains(_write_Class))
                        {
                            OutClassPath = line.Replace(_write_Class, String.Empty);
                            continue;
                        }
                        if (line.Contains(_write_Data))
                        {
                            OutDataPath = line.Replace(_write_Data, String.Empty);
                            continue;
                        }
                    }
                }

                if (ReadExcelPath == String.Empty)
                {
                    StringColor.WriteLine("ReadExcelPath:" + "无路径");
                    Console.ReadKey(true);
                }
                if (OutDataPath == String.Empty)
                {
                    StringColor.WriteLine("OutClassPath:" + "无路径");
                    Console.ReadKey(true);
                }
                if (OutDataPath == String.Empty)
                {
                    StringColor.WriteLine("OutDataPath:" + "无路径");
                    Console.ReadKey(true);
                }
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                StringColor.WriteLine("读取路劲失败");
                StringColor.WriteLine(e);
                Thread.CurrentThread.Abort();
            }
            Console.WriteLine("读取路劲成功");
        }

        private List<string> FindAllExcelPath()
        {
            List<string> pathStr = new List<string>();
            try
            {
                DirectoryInfo theFolder = new DirectoryInfo(ReadExcelPath);
                foreach (FileInfo nextFile in theFolder.GetFiles())
                {
                    if (nextFile.Name.Contains("~$"))
                    {
                        continue;
                    }
                    if (nextFile.Extension == ".xlsx"|| nextFile.Extension == ".xls")
                    {
                        pathStr.Add(nextFile.FullName);
                        Console.WriteLine("获取文件路劲:" + nextFile.FullName);
                    }
                }
            }
            catch (Exception e)
            {
                StringColor.WriteLine("读取读取路劲下的配置表失败");
                StringColor.WriteLine(e);
                Thread.CurrentThread.Abort();
            }

            return pathStr;
        }
        
    }
}