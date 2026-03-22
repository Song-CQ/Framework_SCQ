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
using System.Xml.Linq;

namespace ExcelTool
{
    class Program
    {
        static Task task;
        static void Main(string[] args)
        {

            LogUtil.SetLogCallBack_Log((e) => {
                StringColor.WriteLine(e, ConsoleColor.White);
            },null);
            LogUtil.SetLogCallBack_LogError((e) => {
                StringColor.WriteLine(e, ConsoleColor.Red);
            },null);
            LogUtil.SetLogCallBack_LogWarning((e) => {
                StringColor.WriteLine(e, ConsoleColor.Yellow);
            },null);
            LogUtil.LogGirl();
            if (args==null||args.Length==0)
            {
                ExcelToAssemblyDataHelp.Version = 1;
                CreateAssemblyHelp.IsCreateDll = true;
                ExcelToAssemblyDataHelp.IsOutMultipleDatas = true;
                //加密
                ExcelToAssemblyDataHelp.IsEnciphermentData = true;
                EncryptConst.AES_IVector = "7C9E1B3D5A6F8C2E4F6A8D0B2F4E6C8A";
                EncryptConst.AES_Key = "4F6B8E1A3CD5F9B2D4BA6C8E0F1A3B5C";

            }
            else 
            {
               
                ExcelToAssemblyDataHelp.Version = uint.Parse(args[0]);
                ExcelToAssemblyDataHelp.Version += 1;//版本加一

                CreateAssemblyHelp.IsCreateDll = args[1] == "Dll";
                ExcelToAssemblyDataHelp.IsOutMultipleDatas = args[2] == "True";

                ExcelToAssemblyDataHelp.IsEnciphermentData = args[3] == "True";
                EncryptConst.AES_IVector = args[4];
                EncryptConst.AES_Key = args[5];
            }


           
            StringColor.WriteLine("Version :" + ExcelToAssemblyDataHelp.Version, ConsoleColor.Yellow);
            StringColor.WriteLine("输出类型:"+ (CreateAssemblyHelp.IsCreateDll ?"Dll":"CS"), ConsoleColor.Yellow);
            StringColor.WriteLine("是否单独为表生成数据文件:"+ ExcelToAssemblyDataHelp.IsOutMultipleDatas, ConsoleColor.Yellow);
            StringColor.WriteLine("是否加密:"+ ExcelToAssemblyDataHelp.IsEnciphermentData, ConsoleColor.Yellow);
            StringColor.WriteLine("AES_IVector: ****************" + EncryptConst.AES_IVector[EncryptConst.AES_IVector.Length-2] + EncryptConst.AES_IVector[EncryptConst.AES_IVector.Length - 1], ConsoleColor.Yellow);
            StringColor.WriteLine("AES_Key: ****************" + EncryptConst.AES_Key[EncryptConst.AES_Key.Length - 2] + EncryptConst.AES_Key[EncryptConst.AES_Key.Length - 1], ConsoleColor.Yellow);
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
