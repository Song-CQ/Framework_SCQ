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
using UnityEngine;

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
        }

        private static void Init()
        {
            StringColor.WriteLine("Init_MainMgr",ConsoleColor.Yellow);
            MainMgr.Instance.Init(task);
        }
        
    }
}
