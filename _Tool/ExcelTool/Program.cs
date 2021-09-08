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
        
        static void Main(string[] args)
        {
            
            // List<int>  asa=new List<int>();
            // Console.WriteLine(asa.GetType());
            // return;
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
            Console.WriteLine("Init_MainMgr");
            MainMgr.Instance.Init();
        }
        
    }
}
