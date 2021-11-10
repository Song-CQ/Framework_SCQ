using System;
using ExcelTool.Tool;
using System.Threading.Tasks;


namespace ExcelTool
{
    class Program
    {
        static Task task;
        static void Main(string[] args)
        {
            
            if (args==null||args.Length==0||args[0]=="Dll")
            {
                CreateAssemblyHelp.IsCreateDll = true;
               
            }
            else if (args[0]=="CS")
            {
                CreateAssemblyHelp.IsCreateDll = false;
            }
            if (args==null||args.Length==0||args[1]=="True")
            {
                ExcelToAssemblyDataHelp.IsEnciphermentData = true;
               
            }
            else if (args[1]=="False")
            {
                ExcelToAssemblyDataHelp.IsEnciphermentData = false;
            }

            
            try
            {
                task = new Task(Init);
                task.Start();
                Task.WaitAll(task);
            
                Console.WriteLine("                                ");
                StringColor.WriteLine("打表完成",ConsoleColor.Green);
                
            }
            catch (Exception e)
            {
                StringColor.WriteLine(e);
                Console.WriteLine("                                ");
                StringColor.WriteLine("打表失败");
            }
            
            StringColor.WriteLine("*****************************", ConsoleColor.Yellow);
            Console.WriteLine("按任意键关闭");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void Init()
        {
            StringColor.WriteLine("Init_MainMgr",ConsoleColor.Yellow);
            MainMgr.Instance.Init(task);
        }
        
    }
}
