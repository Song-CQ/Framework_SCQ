using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using ExcelTool.Data;
using ExcelTool.Tool;
using Newtonsoft.Json.Linq;

namespace ExcelTool
{
    public class MainMgr
    {
        private static MainMgr instance;
        public static MainMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainMgr();
                }
                return instance;
            }
        }
        private UrlData urlData;
        public System.Threading.Tasks.Task CurrTask { get; private set; }

        public string ReadExcelPath => FrameworkDirectory + "/" + urlData.ReadExcelPath;
        public string OutDataPath => FrameworkDirectory + "/" + urlData.OutDataPath;
        public string OutClassPath => FrameworkDirectory + "/" + urlData.OutClassPath;

        private List<ExcelData> excelDataLst = new List<ExcelData>();

        public string CurrentDirectory
        {
            get
            {
                if (currentDirectory == null)
                {
                    currentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Remove(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Length - 1);
                }
                return currentDirectory;
            }
        }
        private string currentDirectory;

        public string FrameworkDirectory
        {
            get
            {
                string temp = Path.GetFullPath(CurrentDirectory + "/../../..");
                return temp;
            }
        }


        public void Init(System.Threading.Tasks.Task task)
        {
            ///设置运行目录
            Environment.CurrentDirectory = CurrentDirectory;

            CurrTask = task;
            //加载路劲Josn
            LoadUrlData();
            //获取表路劲
            List<string> path = FindAllExcelPath();
            //创建数据组
            foreach (var VARIABLE in path)
            {
                ExcelData item = new ExcelData(VARIABLE);
                if (item.DataTableCount != 0)
                {
                    excelDataLst.Add(item);
                }
            }
            StringColor.WriteLine("读取表完成,读取数量:" + excelDataLst.Count, ConsoleColor.Yellow);
            //检测输出目录
            CheckAndDelect(OutDataPath);
            CheckAndDelect(OutClassPath);
            if (excelDataLst.Count != 0)
            {
                //创建程序集生成data
                CreateAssembly();
            }
            else
            {
                StringColor.WriteLine("表数量为0");
            }

        }
        private void CreateAssembly()
        {
            Assembly assembly = CreateAssemblyHelp.ExcelDataToAssembly(excelDataLst);
    
            if (assembly != null)
            {
                //生成data
                ExcelToAssemblyDataHelp.Start(assembly, excelDataLst);
            }
        }

        private void CheckAndDelect(string path)
        {
            try
            {
                DirectoryInfo theFolder = Directory.CreateDirectory(path);
                if (theFolder.Exists)
                {
                    DelectDir(theFolder);
                }
                else
                {
                    StringColor.WriteLine(path + "目录不存在");

                }
            }
            catch (Exception e)
            {
                StringColor.WriteLine(e);
                Thread.CurrentThread.Abort();
            }
        }
        public void DelectDir(DirectoryInfo dir)
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
            StringColor.WriteLine("删除" + dir.FullName + "文件夹文件完成", ConsoleColor.Yellow);
        }

        private void LoadUrlData()
        {
            string path = CurrentDirectory.Substring(0, CurrentDirectory.LastIndexOf(@"\"));
            path += @"\Setting\Url.json";
            try
            {
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                using (StreamReader file = new StreamReader(path))
                {
                    using (Newtonsoft.Json.JsonTextReader jsonText = new Newtonsoft.Json.JsonTextReader(file))
                    {
                        urlData = JToken.ReadFrom(jsonText).ToObject<UrlData>();
                    }
                }
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                StringColor.WriteLine("读取路劲失败");
                StringColor.WriteLine(e);
                Thread.CurrentThread.Abort();
            }

            if (urlData.ReadExcelPath == String.Empty)
            {
                StringColor.WriteLine("ReadExcelPath:" + "无路径");
                Console.ReadKey(true);
            }
            else
            {
                StringColor.WriteLine("ReadExcelPath:" + urlData.ReadExcelPath, ConsoleColor.Green);
            }
            if (urlData.OutDataPath == String.Empty)
            {
                StringColor.WriteLine("OutClassPath:" + "无路径");
                Console.ReadKey(true);
            }
            else
            {
                StringColor.WriteLine("OutClassPath:" + urlData.OutDataPath , ConsoleColor.Green);
            }
            if (urlData.OutDataPath == String.Empty)
            {
                StringColor.WriteLine("OutDataPath:" + "无路径");
                Console.ReadKey(true);
            }
            else
            {
                StringColor.WriteLine("OutDataPath:" + urlData.OutDataPath, ConsoleColor.Green);
            }
            Console.WriteLine("读取路劲成功\n");
        }

        private List<string> FindAllExcelPath()
        {
            List<string> pathStr = new List<string>();
            try
            {
                DirectoryInfo theFolder = Directory.CreateDirectory(ReadExcelPath);
                foreach (FileInfo nextFile in theFolder.GetFiles("*",SearchOption.AllDirectories))
                {
                    if (nextFile.Name.Contains("~$"))
                    {
                        continue;
                    }
                    if (nextFile.Extension == ".xlsx" || nextFile.Extension == ".xls")
                    {
                        pathStr.Add(nextFile.FullName);
                        //Console.WriteLine("获取文件路劲:" + nextFile.FullName);
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