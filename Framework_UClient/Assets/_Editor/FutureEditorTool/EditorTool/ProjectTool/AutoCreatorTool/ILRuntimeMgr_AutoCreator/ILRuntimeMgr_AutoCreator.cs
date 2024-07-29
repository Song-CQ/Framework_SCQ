/****************************************************
    文件: ILRuntimeMgr_AutoCreate.cs
    作者: Clear
    日期: #CreateTime#
    类型: 框架核心脚本(请勿修改)
    功能: ILRuntimeMgr_AutoCreator
*****************************************************/
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FutureCore;
using FutureCore.Data;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class ILRuntimeMgr_AutoCreator
    {
        private static string autoRegisterPath = UnityEditorPathConst.AutoRegisterPath + "/ILRuntimeMgr";

        private static string ILRuntimeMgr_RegisterClass = @"/****************************************************
    文件: ILRuntimeMgr_Register.cs
    作者: Clear
    类型: 框架自动创建(请勿修改)
    功能: ILRuntime自动注册
* ****************************************************/
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;
using FutureCore;

namespace ProjectApp
{
    public static partial class ILRuntimeMgr_Register
    {
        public static void RegisterProject(AppDomain appDomain)
        {

//ReplaceBinder
        }

    }
}";

        [MenuItem("[test Tool]/测试")]
        public static void Set()
        {
            CreateTypeAdapter(typeof(BaseMgr<>));

        }

        private static string tempStr;


        public static async void CreateAdapter()
        {

            Task task = Task.Run(CreateAdapter_Task);

            await task;

            Debug.Log("完成");
        }

        public static void CreateAdapter_Task()
        {

            Debug.Log("[ILRuntimeMgr_AutoCreator]开始注册跨域继承适配器");


            if (!Directory.Exists(autoRegisterPath + "/Adapter_AutoCreator"))
            {
                Directory.CreateDirectory(autoRegisterPath + "/Adapter_AutoCreator");
            }
            tempStr = string.Empty;
            CreateTypeAdapter(typeof(BaseCtrl));
            CreateTypeAdapter(typeof(BaseModel));
            CreateTypeAdapter(typeof(BaseUICtrl));
            CreateTypeAdapter(typeof(BaseUI));

            CreateTypeAdapter(typeof(BaseSystem));



            if (!Directory.Exists(autoRegisterPath + "/Register_AutoCreator"))
            {
                Directory.CreateDirectory(autoRegisterPath + "/Register_AutoCreator");
            }
            CreateRegister();


            Debug.Log("[ILRuntimeMgr_AutoCreator]注册热更跨域继承适配器完成");
        }

        private static void CreateRegister()
        {
            string path = autoRegisterPath + "/Register_AutoCreator/ILRuntimeMgr_Register.cs";
            string classVal = ILRuntimeMgr_RegisterClass.Replace("//ReplaceBinder", tempStr);
            File.WriteAllText(path, classVal, System.Text.Encoding.UTF8);
            Debug.Log($"[ILRuntimeMgr_AutoCreator]生成注册器完成");
        }

        private static void CreateTypeAdapter(Type type)
        {
            string path = autoRegisterPath + "/Adapter_AutoCreator/" + type.Name + "Adapter.cs";

            string classVal = ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(type, "ProjectApp");
            tempStr += $"            appDomain.RegisterCrossBindingAdaptor(new {type.Name}Adapter());\r\n";
            File.WriteAllText(path, classVal, System.Text.Encoding.UTF8);
            Debug.Log($"[ILRuntimeMgr_AutoCreator]注册{type.Name}适配器完成");
        }

        public static void GenerateCLRBindingByAnalysis()
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (System.IO.FileStream fs = new System.IO.FileStream("Assets/StreamingAssets/HotFix/HotFix.dll", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                domain.LoadAssembly(fs);

                //Crossbind Adapter is needed to generate the correct binding code

                ProjectApp.ILRuntimeMgr_Register.RegisterAll(domain);

                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/Samples/ILRuntime/Generated");
            }
        }


        public static void UpdataVersionData()
        {
            string filePath = Path.Combine(PathConst.HotFixPath_StreamingAssets, "version.json");
            string dllPath = Path.Combine(PathConst.HotFixPath_StreamingAssets, "HotFix.dll");

            HotFixVerify hotFixVerify;
            if (!File.Exists(filePath))
            {
                hotFixVerify = new HotFixVerify();
            }
            else
            {
                string data = File.ReadAllText(filePath);
                hotFixVerify = JsonUtility.FromJson<HotFixVerify>(data);
            }
            string md5Str = string.Empty;
            if (File.Exists(dllPath))
            {
                md5Str = VerifyUtil.GetFileMD5(dllPath);
                if (md5Str == hotFixVerify.MD5)
                {
                    //dll没有更改不用更新新版本 、
                    LogUtil.Log("[ILRuntimeMgr_AutoCreator] dll没有更改不用更新新版本");
                    return;
                }
            }
            else
            {
                LogUtil.LogError("[ILRuntimeMgr_AutoCreator]HotFix.dll文件不存在，请生成!");
                return;
            }
            hotFixVerify.version++;
            hotFixVerify.buildDate = DateTime.Now.ToString();
            hotFixVerify.MD5 = md5Str;
            hotFixVerify.size = VerifyUtil.GetFileSize(dllPath);

            string val = JsonUtility.ToJson(hotFixVerify);

            FutureCore.FileUtil.WriteAllText(filePath, val);
            LogUtil.Log("[ILRuntimeMgr_AutoCreator] hotFixVerify! 更新成功!");

        }


        public enum CompileCodePlan
        {
            MsBuild = 0,//使用vs的Msbuild编译 建议使用这个方法
            CompileAssembly_HotfixTool = 1,
            CompileAssembly_UnityEditor = 2,//这种编译方式会导致编译出来的pdb调试文件被unity的(pdb2mdb.exe)转换成mbd文件
           
        }


        private static string HotFix_Class_Temp_Path = UnityEditorPathConst.TemporaryPath + "/HotFixClass";

        public static string HotFix_Class_Path = Application.dataPath + "/../../Framework_Project/HotFix/HotFixClass";

        public static string HotFix_ModuleMgrPath = UnityEditorPathConst.AutoRegisterPath + "/ModuleMgr";
        public static string HotFix_LogicPath = Application.dataPath + "/_App/ProjectApp/ProjectApp/Logic";
        public static string HotFix_OutputAssembly = Application.dataPath + @"\..\..\Framework_Project\HotFix\bin";

        private static string replaceCompile_Include = @"    <Compile Include= ""[&&]"" />";

        private static StringBuilder svBuilder = new StringBuilder();

        public static async void AutoRegister_HotFix_ILRuntimeMgr(bool isDelLocalCS, bool isCread_BatFile, CompileCodePlan compileCodePlan, Action cb)
        {
            if (EditorUtility.DisplayDialog("启动ILRuntime热更流程", "是否启动ILRuntime热更流程", "确认", "取消"))
            {
                cb?.Invoke();
                Debug.Log("[启动ILRuntime热更]");

                Debug.Log("------------------------------------------ILRuntime热更流程----开始执行---------------------------------------------------------");

                ClientToHotFixClass(isDelLocalCS);//将代码提取至缓存目录

                RegisterCode();//注册代码

                await CompileCodeToDll(isCread_BatFile,compileCodePlan);//编译代码 to Dll           


                Debug.Log("------------------------------------------ILRuntime热更流程----执行完毕---------------------------------------------------------");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();








            }
        }

        /// <summary>
        /// 注册代码
        /// </summary>
        public static void RegisterCode()
        {


            Debug.Log("------------------------------------------ 注册 HotFix 代码 ----------------------------------------------------------------");
            FutureCore.FileUtil.DeleteFileOrDirectory(HotFix_Class_Path);

            FutureCore.FileUtil.CopyFolder(HotFix_Class_Temp_Path, Path.GetDirectoryName(HotFix_Class_Path), Path.GetFileName(HotFix_Class_Path), "*.cs",CopyFileCode);
            
            


            svBuilder.Clear();
            string startStr = "<!-- Hotfix Class Start -->";
            string endStr = "<!-- Hotfix Class End -->";
            svBuilder.AppendLine(startStr);

            string ProjectApp_HotFix_Path = Path.GetDirectoryName(HotFix_Class_Path);

            List<string> files = new List<string>();

            files.AddRange(Directory.GetFiles(HotFix_Class_Path, "*.cs", SearchOption.AllDirectories));

            files.AddRange(Directory.GetFiles(HotFix_Class_Path + "/../Main", "*.cs", SearchOption.AllDirectories));

            string headPath = Path.GetFullPath(ProjectApp_HotFix_Path + "/");
            foreach (string file in files)
            {
                string path = Path.GetFullPath(file).Replace(headPath, string.Empty);

                svBuilder.AppendLine(replaceCompile_Include.Replace("[&&]", path));
            }

            svBuilder.AppendLine(endStr);

            string csprojPath = ProjectApp_HotFix_Path + "/ProjectApp_HotFix.csproj";

            if (File.Exists(csprojPath))
            {
                FileInfo fileInfo = new FileInfo(csprojPath);

                StreamReader streamReader = fileInfo.OpenText();

                string content = streamReader.ReadToEnd();
                streamReader.Close();

                string[] startVals = content.Split(startStr);
                string[] endVals = content.Split(endStr);

                content = startVals[0] + svBuilder + endVals[endVals.Length - 1];

                FileStream streamWriter = fileInfo.OpenWrite();
                StreamWriter writer = new StreamWriter(streamWriter);
                writer.Write(content);
                writer.Close();
            }

            Debug.Log("------------------------------------------ 注册 完成 ----------------------------------------------------------------");

        }

        /// <summary>
        /// 拷贝文件 
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        private static void CopyFileCode(string sourceFileName, string destFileName)
        {

            string allText = File.ReadAllText(sourceFileName);
            allText = allText.Replace("namespace ProjectApp", "namespace ProjectApp.HotFix");

            FutureCore.FileUtil.WriteAllText(destFileName, allText, true);
            
        }



        /// <summary>
        /// 编译Dll
        /// </summary>
        public static async Task CompileCodeToDll(bool isCread_BatFile, CompileCodePlan compileCodePlan)
        {
            Debug.Log("--------------------------- 编译 HotFix Dll ---------------------------");

            LogUtil.Log("------ 开始编译Dll ------");

            await Task.Run(() => CompileAssembly(isCread_BatFile, compileCodePlan));


            LogUtil.Log("------ 复制 HotFix Dll 至 StreamingAssets ------");
            //移动dll to StreamingAssets 
            string dllPath = Path.Combine(PathConst.HotFixPath_StreamingAssets, "HotFix.dll");
            string pdbPath = Path.Combine(PathConst.HotFixPath_StreamingAssets, "HotFix.pdb");
            //HotFix_OutputAssembly

            FutureCore.FileUtil.CopyFile(HotFix_OutputAssembly + "/HotFix.dll", dllPath, true);
            FutureCore.FileUtil.CopyFile(HotFix_OutputAssembly + "/HotFix.pdb", pdbPath, true);
            LogUtil.Log("------ 复制结束 ------");

            //更新信息
            UpdataVersionData();

        }



        private static bool CompileAssembly(bool isCread_BatFile, CompileCodePlan compileCodePlan)
        {
            // 动态编译程序集 因为不知道怎么在unity线程 动态编译 引用框架 netstandard 导致编译失败 Consider adding a reference to assembly `netstandard,

            // 解决方案1 : 建一个新的C#工程 使用netstandard框架 用新程序编译；
            // 解决方案2 : 用vs 自带的msbuild 编译   调用bat文件编译 ProjectApp_HotFix解决方案

            /*  已解决？ 获取当前项目所引用的程序集 netstandard.dll 
            string frameworkInstallDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            cp.ReferencedAssemblies.Add(frameworkInstallDir + @"\Facades\netstandard.dll");
             */
 

            bool IsCompileAssembly_Sub =true;

            if (compileCodePlan == CompileCodePlan.MsBuild)//使用msbuild 编译Dll
            {
                string cmdFile = Path.GetFullPath(Application.dataPath + @"\..\..\_Tool\HotFixTool\编译HotFix_Dll_MsBuild.bat");

                if (!File.Exists(cmdFile)|| isCread_BatFile)
                {
                    string allstr =
@"@echo off
cd %~dp0
@call {0} ../../Framework_Project/HotFix/ProjectApp_HotFix.csproj
pause";

                    string path = @"E:\SetUp\VS2022\Msbuild\Current\Bin\msbuild.exe";
                    if (!File.Exists(path))
                    {
                        UnityEngine.Debug.LogError("[MsBuild]MsBuild.exe 不存在，路径:"+ path);
                        UnityEngine.Debug.LogError(@"[MsBuild]请重新设置MsBuild.exe,MsBuild位于vs安装目录的VS2022\Msbuild\Current\Bin\msbuild.exe");

                        return false;
                    }
                    //EditorPrefs.HasKey


                    string paths = EditorPrefs.GetString("ExternalScriptEditor");
                    LogUtil.LogError(paths);


                    allstr = allstr.Replace("{0}",path);
                    FutureCore.FileUtil.WriteAllText(cmdFile, allstr, true);
                }

                System.Diagnostics.Process process = System.Diagnostics.Process.Start(cmdFile);
                process.WaitForExit();
                process.Close();
                process.Dispose();

                UnityEngine.Debug.Log("[MsBuild]编译程序集Dll完成".AddColor(ColorType.Green));
            }
            else
            {
                CreateAssembly();//创建程序集参数
                
                if (compileCodePlan == CompileCodePlan.CompileAssembly_UnityEditor) // 动态编译程序集 Dll  //但是生成的pdb调试文件会被自动转换成.mbd文件  unity的pdb2mdb.exe 转换
                {
                    IsCompileAssembly_Sub = CompileAssembly();
                    
                }
                else if (compileCodePlan == CompileCodePlan.CompileAssembly_HotfixTool) // 使用外部HotFixTool 动态编译程序集 Dll 
                {

                    //使用外部工程编译
                    string json = JsonUtility.ToJson(GetCompilerParametersData(cp, HotFix_Class_Path));

                    //写入参数
                    FutureCore.FileUtil.WriteAllText(Application.dataPath + @"\..\..\_Tool\HotFixTool\CompilerParameters.json", json, true);

                    string cmdFile = Path.GetFullPath(Application.dataPath + @"\..\..\_Tool\HotFixTool\编译HotFix_Dll_CompileAssembly.bat");

                    if (!File.Exists(cmdFile)|| isCread_BatFile)
                    {
                        string allstr =
    @"
@echo on

echo HotFix_Dll_CompileAssembly 
echo\

cd %~dp0
@call Release\HotFix_Tool.exe %1
";
                        FutureCore.FileUtil.WriteAllText(cmdFile, allstr, true);
                    }

                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(cmdFile, "EndCloseCmd");
                    process.WaitForExit();
                    process.Close();
                    process.Dispose();

                }
            }

            LogUtil.Log(IsCompileAssembly_Sub?"------ 编译结束 : 成功 ------ ".AddColor(ColorType.Green): "------ 编译结束 : 失败 ------ ".AddColor(ColorType.Red));

            return IsCompileAssembly_Sub;
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
            public string OutputAssembly = Application.streamingAssetsPath + @"\HotFix\HotFix.dll";

            public int WarningLevel = -1;

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
            public string HotFixClassPath ;


        }

        private static CSharpCodeProvider provider;
        private static CompilerParameters cp;
        
        private static CompilerParameters CreateAssembly()
        {

            //创建编译器实例。 

            provider = new CSharpCodeProvider();

            //CompilerParametersData cp = new CompilerParametersData();

            //设置编译参数。 

            cp = new CompilerParameters();



            // 获取或设置一个值，该值指示是否生成可执行文件。
            cp.GenerateExecutable = false;
            //获取或设置一个值，该值指示是否在已编译的可执行文件中包含调试信息
            cp.IncludeDebugInformation = true;

            //获取或设置一个值，该值指示是否在内存中生成输出
            cp.GenerateInMemory = true;
            //获取或设置输出程序集的名称。
            cp.OutputAssembly = HotFix_OutputAssembly + @"\HotFix.dll";

            cp.WarningLevel = 2;

            // Set whether to treat all warnings as errors.获取或设置一个值，该值指示是否将警告视为错误。

            cp.TreatWarningsAsErrors = false;

            // Set compiler argument to optimize output.获取或设置调用编译器时使用的可选命令行参数。

            cp.CompilerOptions = "/optimize";

            // Set a temporary files collection.
            // The TempFileCollection stores the temporary files
            // generated during a build in the current directory,
            // and does not delete them after compilation.
            //if (creadTempFiles)
            //{
            //    string path = Application.dataPath + "/../_HotFix_Temp";
            //    if (!Directory.Exists(path))
            //    {
            //        Directory.CreateDirectory(path);
            //    }
            //    cp.TempFiles = new TempFileCollection(path, true);
            //}



            //获取当前项目所引用的程序集

            List<string> allReferencedAssemblies = new List<string>();

            allReferencedAssemblies.Add("System.dll");
            allReferencedAssemblies.Add("System.Core.dll");
            allReferencedAssemblies.Add("System.Data.dll");



            string unityDllPath = UnityEditorPathConst.PluginsPath + "/Unity_DLL/";


            string scriptAssembliesPath = Application.dataPath + @"/../Library/ScriptAssemblies/";
           
            //unity dll文件
            allReferencedAssemblies.Add(unityDllPath + "UnityEngine.dll");
            allReferencedAssemblies.Add(unityDllPath + "UnityEngine.CoreModule.dll");

            //项目 dll文件
            allReferencedAssemblies.Add(scriptAssembliesPath + "UnityEngine.UI.dll");
            allReferencedAssemblies.Add(scriptAssembliesPath + "FutureFrame.dll");
            allReferencedAssemblies.Add(scriptAssembliesPath + "Plugins3rdLibrary.dll");
            allReferencedAssemblies.Add(scriptAssembliesPath + "ProjectApp.dll");

            //第三方dll文件
            allReferencedAssemblies.Add(UnityEditorPathConst.PluginsPath + @"/LogUtil/LogUtil.dll");

            //如果数据表使用的dll 则要引入表dll 
            if (AppConst.ExcelConfig_UseDll)
            {
                allReferencedAssemblies.Add(Application.dataPath + "/_App/ProjectApp/AutoCreator/ExcelConfig/VODll/BaseVoClassLib.dll");
                allReferencedAssemblies.Add(Application.dataPath + "/_App/ProjectApp/AutoCreator/ExcelConfig/VODll/VoClassLib.dll");
            }


            ////获取当前项目所引用的程序集 netstandard.dll
            string frameworkInstallDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            cp.ReferencedAssemblies.Add(frameworkInstallDir + @"\Facades\netstandard.dll");


            cp.ReferencedAssemblies.AddRange(allReferencedAssemblies.ToArray());


            //for (int i = 0; i < allReferencedAssemblies.Count; i++)
            //{
            //    LogUtil.LogWarning(allReferencedAssemblies[i]);
            //}


            ////要编译的cs文件路径
            //cp.HotFixClassPath = UnityEditorPathConst.TemporaryPath + "/HotFixClass";

            return cp;
        }

        public static CompilerParametersData GetCompilerParametersData(CompilerParameters parameters, string _HotFixClassPath)
        {
            CompilerParametersData cp = new CompilerParametersData();

            cp.HotFixClassPath = _HotFixClassPath;

            // 获取或设置一个值，该值指示是否生成可执行文件。
            cp.GenerateExecutable = parameters.GenerateExecutable;
            //获取或设置一个值，该值指示是否在已编译的可执行文件中包含调试信息
            cp.IncludeDebugInformation = parameters.IncludeDebugInformation;

            //获取或设置一个值，该值指示是否在内存中生成输出
            cp.GenerateInMemory = parameters.GenerateInMemory;
            //获取或设置输出程序集的名称。
            cp.OutputAssembly = parameters.OutputAssembly;

            cp.WarningLevel = parameters.WarningLevel;

            // Set whether to treat all warnings as errors.获取或设置一个值，该值指示是否将警告视为错误。

            cp.TreatWarningsAsErrors = parameters.TreatWarningsAsErrors;

            // Set compiler argument to optimize output.获取或设置调用编译器时使用的可选命令行参数。

            cp.CompilerOptions = parameters.CompilerOptions;


            cp.ReferencedAssemblies = new string[parameters.ReferencedAssemblies.Count];

            for (int i = 0; i < cp.ReferencedAssemblies.Length; i++)
            {
                cp.ReferencedAssemblies[i] = parameters.ReferencedAssemblies[i];
            }

            return cp;
        }
        private static bool CompileAssembly()
        {
            LogUtil.Log(("------ 创建编译器实例成功 ------"));


            string frameworkInstallDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

            LogUtil.Log("工作目录："+(frameworkInstallDir));

            string mainStartPath = HotFix_Class_Path+"/../Main";
            string hotFixFilesPath = HotFix_Class_Path;

            string[] mainStartFiles = Directory.GetFiles(mainStartPath, "*.cs", SearchOption.AllDirectories);
            string[] hotFixFiles = Directory.GetFiles(hotFixFilesPath, "*.cs", SearchOption.AllDirectories);

            List<string> allClass = new List<string>();
            allClass.AddRange(mainStartFiles);
            allClass.AddRange(hotFixFiles);
           

            CompilerResults result = provider.CompileAssemblyFromFile(cp,allClass.ToArray());

            bool isFace = false;
            Assembly assembly = null;
            if (result.Errors.Count > 0)
            {

                string val = AppDomain.CurrentDomain.BaseDirectory;

                LogUtil.Log("------ 编译程序集失败 ------");


                foreach (var item in result.Errors)
                {
                    LogUtil.LogError(item.ToString());
                }
                isFace = false;
            }
            else
            {
                assembly = result.CompiledAssembly;

                foreach (var item in result.Output)
                {

                    LogUtil.Log(item);
                }

            ;
                foreach (var item in assembly.GetTypes())
                {

                    LogUtil.Log(item.FullName);
                }

                LogUtil.Log("-------------------------------");
                Assembly assembly_fix = Assembly.LoadFile(@"H:\UnityPro\Framework_SCQ\Framework_UClient\Assets\StreamingAssets\HotFix\HotFix2.dll");
                foreach (var item in assembly_fix.GetTypes())
                {

                    LogUtil.Log(item.FullName);
                }


                LogUtil.Log("------ 编译程序集成功 ------");
                if (cp.GenerateInMemory)
                {


                }
                else
                {

                }

                isFace = true;

            }




            LogUtil.Log("------ 编译程序集结束 ------");

            return isFace;
        }

        /// <summary>
        /// 热更代码载入Unity客户端
        /// </summary>
        public static void HotFixClassToClient()
        {
            Debug.Log("------------------------------------------热更代码载入Unity客户端---------------------------------------------------------");
            Debug.Log("HotFixClass_Cache 文件夹热更代码载入Unity客户端");
            string ModuleMgrPath = UnityEditorPathConst.AutoRegisterPath + "/ModuleMgr";
            string LogicPath = Application.dataPath + "/_App/ProjectApp/ProjectApp/Logic";

            if (File.Exists(ModuleMgrPath + "/ModuleMgr_文件夹的热更占位文件.txt"))
            {
                File.Delete(ModuleMgrPath + "/ModuleMgr_文件夹的热更占位文件.txt");
            }

            FutureCore.FileUtil.CopyFolder(HotFix_Class_Temp_Path + "/ModuleMgr", UnityEditorPathConst.AutoRegisterPath);

            if (File.Exists(LogicPath + "/Logic_文件夹的热更占位文件.txt"))
            {
                File.Delete(LogicPath + "/Logic_文件夹的热更占位文件.txt");
            }

            FutureCore.FileUtil.CopyFolder(HotFix_Class_Temp_Path + "/Logic", Application.dataPath + "/_App/ProjectApp/ProjectApp");
            Debug.Log("------------------------------------------热更代码载入完成---------------------------------------------------------");
        }

        /// <summary>
        /// Unity客户端热更代码 提取至 热更缓存文件夹 
        /// </summary>
        public static void ClientToHotFixClass(bool isDelLocalCS)
        {
            Debug.Log("------------------------------------------ 开始提取模块代码 ----------------------------------------------------------------");

            Debug.Log("------------------------------------------ 提取 Module 模块代码 ----------------------------------------------------------------");

            string ModuleMgrPath = HotFix_ModuleMgrPath;
            string LogicPath = HotFix_LogicPath;


            if (File.Exists(ModuleMgrPath + "/ModuleMgr_文件夹的热更占位文件.txt"))
            {
                LogUtil.LogWarning("存在文件夹的热更占位文件！    \n" + ModuleMgrPath);
                LogUtil.LogWarning("跳过提取 Module 模块代码");

            }
            else
            {
                FutureCore.FileUtil.CopyFolder(ModuleMgrPath, HotFix_Class_Temp_Path);

                if (isDelLocalCS)
                {
                    FutureCore.FileUtil.DeleteFileOrDirectory(ModuleMgrPath);

                    string path = ModuleMgrPath + "/ModuleMgr_文件夹的热更占位文件.txt";
                    string contents = "ModuleMgr-的热更占位文件";
                    FutureCore.FileUtil.WriteAllText(path, contents, true);


                }

            }


            Debug.Log("------------------------------------------ 提取 Logic 逻辑代码 ----------------------------------------------------------------");


            if (File.Exists(LogicPath + "/Logic_文件夹的热更占位文件.txt"))
            {

                LogUtil.LogWarning("存在文件夹的热更占位文件！    \n" + LogicPath);
                LogUtil.LogWarning("跳过提取 Logic 逻辑代码");
            }
            else
            {
                FutureCore.FileUtil.CopyFolder(LogicPath, HotFix_Class_Temp_Path);

                if (isDelLocalCS)
                {
                    FutureCore.FileUtil.DeleteFileOrDirectory(LogicPath);

                    string path = LogicPath + "/Logic_文件夹的热更占位文件.txt";
                    string contents = "Logic-的热更占位文件";
                    FutureCore.FileUtil.WriteAllText(path, contents, true);
                }
            }

            Debug.Log("------------------------------------------ 提取代码完成 ----------------------------------------------------------------");
        }

        public static void Delete_HotFixClass_Cache()
        {
            if (EditorUtility.DisplayDialog("删除热更代码缓存", "是否删除 ILRuntime热更文件夹缓存?(删除前请确保代码已经载入到客户端，避免照成代码丢失)", "确认", "取消"))
            {
                Debug.Log("------------------------------------------删除热更代码缓存---------------------------------------------------------");
                FutureCore.FileUtil.DeleteFileOrDirectory(HotFix_Class_Temp_Path);


                Debug.Log("------------------------------------------删除热更缓成功---------------------------------------------------------");
            }
        }
    }
}