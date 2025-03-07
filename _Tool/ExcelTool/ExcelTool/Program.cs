/*******************************************
       く__,.ヘヽ.        /  ,ー､ 〉
           ＼ ', !-─‐-i  /  /´
           ／｀ｰ'       L/／｀ヽ､
         /   ／,   /|   ,   ,       ',
       ｲ   / /-‐/  ｉ L_ ﾊ ヽ!   i
       ﾚ ﾍ 7ｲ｀ﾄ ﾚ'ｧ-ﾄ､!ハ|   |
          !,/7 '0'     ´0iソ|    |
          |.从*    _     ,,,, / |./    |
          ﾚ'| i＞.､,,__  _,.イ /   .i   |
            ﾚ'| | / k_７_/ﾚ'ヽ,  ﾊ.  |
              | |/i 〈|/   i  ,.ﾍ |  i  |
             .|/ /  ｉ：    ﾍ!    ＼  |
              kヽ&gt;､ﾊ _,.ﾍ､    /､!
              !'〈//｀Ｔ´', ＼ ｀'7'ｰr'
              ﾚ'ヽL__|___i,___,ンﾚ|ノ
                  ﾄ-,/  |___./
                  'ｰ'    !_,.:
 ******************************************/

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

            LogUtil.SetLogCallBack_Log((e) => {
                Console.WriteLine(e);
            },null);
            LogUtil.LogGirl();
            if (args==null||args.Length==0)
            {
                CreateAssemblyHelp.IsCreateDll = true;
                ExcelToAssemblyDataHelp.IsEnciphermentData = false;
                ExcelToAssemblyDataHelp.IsOutMultipleDatas = false;
               
            }
            else 
            {
               
                CreateAssemblyHelp.IsCreateDll = args[0] == "Dll";
                ExcelToAssemblyDataHelp.IsEnciphermentData = args[1] == "True";
                ExcelToAssemblyDataHelp.IsOutMultipleDatas = args[2] == "True";
            }


           
            StringColor.WriteLine("输出类型:"+ CreateAssemblyHelp.IsCreateDll.ToString(), ConsoleColor.Yellow);
            StringColor.WriteLine("是否加密:"+ ExcelToAssemblyDataHelp.IsEnciphermentData, ConsoleColor.Yellow);
            StringColor.WriteLine("是否单独为表生成数据文件:"+ ExcelToAssemblyDataHelp.IsOutMultipleDatas, ConsoleColor.Yellow);
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();


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
            StringColor.WriteLine("Init_MainMgr\n",ConsoleColor.Yellow);
            MainMgr.Instance.Init(task);
        }

        

    }
}
