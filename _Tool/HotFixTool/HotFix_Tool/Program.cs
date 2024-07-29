using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HotFix_Tool
{

    public static class Log
    {
        public static void LogError(string val)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(val);
            Console.ForegroundColor = ConsoleColor.White;

        }
        public static void LogInfo(string val, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(val);
            Console.ForegroundColor = ConsoleColor.White;

        }


    }
      

    internal class Program
    {
        private static CSharpCodeProvider provider;
        private static CompilerParameters cp;

        private static CompilerParametersData data;


        static void Main(string[] args)
        {
            bool isEndCloseCmd = false;
            if (args.Length>=1)
            {
                string cmd = args[0];

                Log.LogError("是否自动关闭:" + cmd);
                if ( cmd == "EndCloseCmd")
                {
                    isEndCloseCmd = true;
                }
                
            }
            
            string jsonPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "../CompilerParameters.json";

            if (!File.Exists(jsonPath))
            {

                Log.LogError("参数文件不存在!:"+ jsonPath);
                Console.ReadKey();//让窗体保存接受外部参数的状态来达到不退出的效果
                return;
            }
            string contents = File.ReadAllText(jsonPath);

            data = Newtonsoft.Json.JsonConvert.DeserializeObject<CompilerParametersData>(contents);

            CreateAssembly();

            CompileAssembly();
            
            if (!isEndCloseCmd)
            {
                Console.ReadKey();//让窗体保存接受外部参数的状态来达到不退出的效果
            }
           
        }


        //        private static CompilerResults BuildSharedAssembly(string sAssemblyName, string sFrameworkVersion, string[] hCode)
        //        {
        //            CompilerParameters hBuildParams = new CompilerParameters();
        //            hBuildParams.OutputAssembly = sAssemblyName;
        //            hBuildParams.GenerateExecutable = false;
        //            hBuildParams.GenerateInMemory = false;
        //#if DEBUG
        //            hBuildParams.IncludeDebugInformation = true;
        //            hBuildParams.TempFiles = new TempFileCollection(Environment.GetEnvironmentVariable("TEMP"), true);
        //#else
        //                        hBuildParams.IncludeDebugInformation    = false;
        //                        hBuildParams.CompilerOptions            = "/optimize";
        //#endif
        //            Assembly hRootAssembly = sFrameworkVersion == "v3.5" ? Assembly.Load(Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(hA => hA.Name == "Netbase.Shared").Single()) : Assembly.GetExecutingAssembly();

        //            foreach (AssemblyName hAssemblyName in hRootAssembly.GetReferencedAssemblies())
        //            {
        //                Assembly hLoaded = Assembly.Load(hAssemblyName);
        //                hBuildParams.ReferencedAssemblies.Add(hLoaded.Location);
        //            }

        //            Dictionary<string, string> hOptions = new Dictionary<string, string>();
        //            hOptions.Add("CompilerVersion", sFrameworkVersion);

        //            using (CodeDomProvider hCodeDom = CodeDomProvider.CreateProvider("CSharp", hOptions))
        //            {
        //                return hCodeDom.CompileAssemblyFromSource(hBuildParams, hCode);
        //            }
        //        }


        static void DisplayCSharpCompilerInfo()
        {
            Dictionary<string, string> provOptions =
            new Dictionary<string, string>();

            provOptions.Add("CompilerVersion", "v3.5");
            // Get the provider for Microsoft.CSharp
            CSharpCodeProvider csProvider = new CSharpCodeProvider(provOptions);

            // Display the C# language provider information.
            Console.WriteLine("CSharp provider is {0}",
                csProvider.ToString());
            Console.WriteLine("  Provider hash code:     {0}",
                csProvider.GetHashCode().ToString());
            Console.WriteLine("  Default file extension: {0}",
                csProvider.FileExtension);

            Console.WriteLine();
        }

        private static void CreateAssembly()
        {

            //创建编译器实例。 
            Console.WriteLine("创建编译器实例");
            CodeDomProvider.CreateProvider("CSharp");


            provider = new CSharpCodeProvider();

            Dictionary<string, string> provOptions =
           new Dictionary<string, string>();

            //provOptions.Add("CompilerVersion", "v4.0");
            //// Get the provider for Microsoft.CSharp
            //provider = new CSharpCodeProvider(provOptions);

            // Display the C# language provider information.
            Console.WriteLine("CSharp provider is {0}",
                provider.ToString());
            Console.WriteLine("  Provider hash code:     {0}",
                provider.GetHashCode().ToString());
            Console.WriteLine("  Default file extension: {0}",
                provider.FileExtension);

            ////设置编译参数。 

            cp = new CompilerParameters();



            //// 获取或设置一个值，该值指示是否生成可执行文件。
            cp.GenerateExecutable = data.GenerateExecutable;
            ////获取或设置一个值，该值指示是否在已编译的可执行文件中包含调试信息
            cp.IncludeDebugInformation = data.IncludeDebugInformation;

            ////获取或设置一个值，该值指示是否在内存中生成输出
            cp.GenerateInMemory = data.GenerateInMemory;
            ////获取或设置输出程序集的名称。
            cp.OutputAssembly = data.OutputAssembly;

            cp.WarningLevel = data.WarningLevel;

            //// Set whether to treat all warnings as errors.获取或设置一个值，该值指示是否将警告视为错误。

            cp.TreatWarningsAsErrors = data.TreatWarningsAsErrors;

            //// Set compiler argument to optimize output.获取或设置调用编译器时使用的可选命令行参数。

            cp.CompilerOptions = data.CompilerOptions;

            //// Set a temporary files collection.
            //// The TempFileCollection stores the temporary files
            //// generated during a build in the current directory,
            //// and does not delete them after compilation.
            ////if (creadTempFiles)
            ////{
            ////    string path = Application.dataPath + "/../_HotFix_Temp";
            ////    if (!Directory.Exists(path))
            ////    {
            ////        Directory.CreateDirectory(path);
            ////    }
            ////    cp.TempFiles = new TempFileCollection(path, true);
            ////}


            //cp.CoreAssemblyFileName = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\mscorlib.dll";



            //cp.ReferencedAssemblies.Add(typeof(CSharpCodeProvider).Assembly.Location);

            //cp.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\mscorlib.dll");

            foreach (var item in data.ReferencedAssemblies)
            {
                //Console.WriteLine("剔除DLL：" + Path.GetFileName(item));

                //if (Path.GetFileName(item) == "netstandard.dll" || Path.GetFileName(item) == "VoClassLib.dll")
                //{
                //    continue;
                //}
                
                cp.ReferencedAssemblies.Add(item);

            }
            //cp.ReferencedAssemblies.Add("Microsoft.CSharp.dll");

            //cp.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\Facades\netstandard.dll");



            //获取当前项目所引用的程序集 netstandard.dll
            //string frameworkInstallDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            //cp.ReferencedAssemblies.Add(frameworkInstallDir + @"netstandard.dll");
            //cp.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\Facades\netstandard.dll");



            foreach (var item in cp.ReferencedAssemblies)
            {
                Console.WriteLine("引用DLL：" + item);
            }

            

        }

        private static void CompileAssembly()
        {
            Log.LogInfo("------ 创建编译器实例成功 ------", ConsoleColor.Green);


            string frameworkInstallDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

            Log.LogInfo(frameworkInstallDir, ConsoleColor.Green);

    

            string csprojPath = data.HotFixClassPath + "/../ProjectApp_HotFix.csproj";

            string allstr = File.ReadAllText(csprojPath);

            int startIndex  = allstr.IndexOf("<!-- Hotfix Class Start -->") + 28;
            int endIndex = allstr.IndexOf("<!-- Hotfix Class End -->");

            allstr = allstr.Substring(startIndex,endIndex-startIndex);

            allstr = allstr.Replace(@"<Compile Include= ""","@").Replace(@""" />", string.Empty);

            

            List<string> allClass = new List<string>();

            foreach (var item in allstr.Split("@".ToCharArray()))
            {
                string path = item.Trim();
                if (path==string.Empty)
                {
                    continue;
                }
                path = Path.GetFullPath(data.HotFixClassPath + "/../" + item);
                allClass.Add(path);
                Log.LogInfo("编译文件："+ path);
            }




            CompilerResults result = provider.CompileAssemblyFromFile(cp, allClass.ToArray());

           


            Assembly assembly = null;
            if (result.Errors.Count > 0)
            {

                string val = AppDomain.CurrentDomain.BaseDirectory;

                Log.LogError("------ 编译程序集失败 ------");


                foreach (var item in result.Errors)
                {
                    Console.WriteLine(item.ToString());
                }

            }
            else
            {
                assembly = result.CompiledAssembly;

                //assembly.GetCustomAttribute();
                for (int i = 1; i < result.Output.Count; i++)
                {

                    Log.LogInfo(result.Output[i], ConsoleColor.Green);
                }

                //Assembly assembly_fix = Assembly.LoadFile(@"H:\UnityPro\Framework_SCQ\Framework_UClient\Assets\StreamingAssets\HotFix\HotFix.dll");
                //foreach (var item in assembly_fix.GetTypes())
                //{
                    
                //    Log.LogInfo(item.FullName);
                //}

                Log.LogInfo("------ 编译程序集成功 ------", ConsoleColor.Green);
                if (cp.GenerateInMemory)
                {


                }
                else
                {

                }
            }




            Log.LogInfo("------ 编译程序集结束 ------", ConsoleColor.Green);

           

     
        }
    }


    public class CompilerParametersData
    {

        // 获取或设置一个值，该值指示是否生成可执行文件。
        public bool GenerateExecutable = false;
        //获取或设置一个值，该值指示是否在已编译的可执行文件中包含调试信息
        public bool IncludeDebugInformation = true;

        //获取或设置一个值，该值指示是否在内存中生成输出
        public bool GenerateInMemory = true;
        //获取或设置输出程序集的名称。
        public string OutputAssembly =  @"\HotFix\HotFix.dll";

        public int WarningLevel = 2;

        // Set whether to treat all warnings as errors.获取或设置一个值，该值指示是否将警告视为错误。

        public bool TreatWarningsAsErrors = false;

        // Set compiler argument to optimize output.获取或设置调用编译器时使用的可选命令行参数。

        public string CompilerOptions = "/optimize";

        // Set a temporary files collection.
        // The TempFileCollection stores the temporary files
        // generated during a build in the current directory,
        // and does not delete them after compilation.
        public string TempFiles = "";

        //引用
        public string[] ReferencedAssemblies;


        //要编译的cs文件路径
        public string HotFixClassPath =  "/HotFixClass";
    }


    
    


}
