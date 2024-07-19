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

        

        private static string tempStr;


        public static async void CreateAdapter()
        {
            
            Task task  = Task.Run(CreateAdapter_1);
            
            await task;

            Debug.Log("完成");
        }

        public static void CreateAdapter_1()
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
            CreateTypeAdapter(typeof(BaseMgr<>));


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


        public static void CreateVersionData()
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
                    //dll没有更改不用更新新版本
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
        }

        private static void RegisterAutoAssetInAllDirectory_HotFix()
        {

            ILRuntimeMgr_AutoCreator.CreateVersionData();



        }

        private static string HotFix_Class_Temp_Path = UnityEditorPathConst.HotFix_Out + "/HotFixClass_Cache";
        public static string HotFix_Class_Path = UnityEditorPathConst.HotFix_Out + "/HotFixClass";

        public static string HotFix_ModuleMgrPath = UnityEditorPathConst.AutoRegisterPath + "/ModuleMgr";
        public static string HotFix_LogicPath = Application.dataPath + "/_App/ProjectApp/ProjectApp/Logic";

        private static string replaceHotFix_Class_Path;
        private static string replaceCompile_Include = @"    <Compile Include= ""[&&]"" />";

        private static StringBuilder svBuilder = new StringBuilder();
        private static List<string> allClassVal = new List<string>();

        public static async void AutoRegister_HotFix_ILRuntimeMgr(bool isDelLocalCS, Action cb)
        {
            if (EditorUtility.DisplayDialog("启动ILRuntime热更流程", "是否启动ILRuntime热更流程", "确认", "取消"))
            {
                cb?.Invoke();
                Debug.Log("[启动ILRuntime热更]");

                allClassVal.Clear();
                Debug.Log("------------------------------------------ILRuntime热更流程----开始执行---------------------------------------------------------");

                ClientToHotFixClass(isDelLocalCS);//将代码提取至缓存目录

                RegisterCode();//注册代码

                CompileCodeToDll();//编译代码 to Dll


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
            replaceHotFix_Class_Path = Path.GetFullPath(UnityEditorPathConst.HotFix_Out + "/");

            svBuilder.Clear();
            string startStr = "<!-- Hotfix Class Start -->";
            string endStr = "<!-- Hotfix Class End -->";
            svBuilder.AppendLine(startStr);

            FutureCore.FileUtil.CopyFolder(HotFix_Class_Temp_Path, UnityEditorPathConst.HotFix_Out, "HotFixClass", "*.cs", CopyFileCallback);

            svBuilder.AppendLine(endStr);

            string csprojPath = UnityEditorPathConst.HotFix_Out + "/ProjectApp_HotFix.csproj";

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
        /// 编译Dll
        /// </summary>
        public static async void CompileCodeToDll()
        {



            if (allClassVal.Count==0)
            {

                string path = UnityEditorPathConst.HotFix_Out+"/HotFixClass";

                DirectoryInfo directory = new DirectoryInfo(path);

                var fieldInfos = directory.GetFiles("*.cs", SearchOption.AllDirectories);

                foreach (var fieldInfo in fieldInfos)
                {
                    string classVal = fieldInfo.OpenText().ReadToEnd();
                    allClassVal.Add(classVal);
                }

                
            }


            Debug.Log("--------------------------- 编译 HotFix Dll ---------------------------");
            CreateAssembly();
            LogUtil.Log("------ 创建编译器实例成功 ------");
            LogUtil.Log("------ 开始编译Dll ------");
            CompileAssembly();
            //await Task.Run(CompileAssembly);

        }


        private static void CompileAssembly()
        {
             

            CompilerResults result = provider.CompileAssemblyFromSource(cp, allClassVal.ToArray());

            Assembly assembly = null;
            if (result.Errors.Count > 0)
            {

                string val = AppDomain.CurrentDomain.BaseDirectory;

                LogUtil.Log("------ 编译程序集失败 ------");


                foreach (var item in result.Errors)
                {
                    LogUtil.LogError(item.ToString());
                }
                
            }
            else
            {
                assembly = result.CompiledAssembly;
                LogUtil.Log("------ 编译程序集成功 ------".AddColor(ColorType.Green));
            
                if (cp.GenerateInMemory)
                {
                   
         
                }
                else
                {
                    
                }
            }


            
        }



        private static void CopyFileCallback(string sourceFileName, string destFileName)
        {
            string path = Path.GetFullPath(destFileName);
            path = path.Replace(replaceHotFix_Class_Path, "");

            string val = replaceCompile_Include.Replace("[&&]", path);

            svBuilder.AppendLine(val);

            string classVal = File.ReadAllText(destFileName);
            allClassVal.Add(classVal);
        }

        private static CSharpCodeProvider provider;
        private static CompilerParameters cp;

        private static void CreateAssembly()
        {

            //创建编译器实例。 

            provider = new CSharpCodeProvider();

            //设置编译参数。 

            cp = new CompilerParameters();

            // 获取或设置一个值，该值指示是否生成可执行文件。
            cp.GenerateExecutable = false;
            //获取或设置一个值，该值指示是否在已编译的可执行文件中包含调试信息
            cp.IncludeDebugInformation = true;

            //获取或设置一个值，该值指示是否在内存中生成输出
            cp.GenerateInMemory = true;
            //获取或设置输出程序集的名称。
            cp.OutputAssembly = Application.streamingAssetsPath + @"HotFix\HotFix.dll";

            cp.WarningLevel = 3;

            // Set whether to treat all warnings as errors.获取或设置一个值，该值指示是否将警告视为错误。

            cp.TreatWarningsAsErrors = false;

            // Set compiler argument to optimize output.获取或设置调用编译器时使用的可选命令行参数。

            cp.CompilerOptions = "/optimize";

            // Set a temporary files collection.
            // The TempFileCollection stores the temporary files
            // generated during a build in the current directory,
            // and does not delete them after compilation.
            if (true)
            {
                string path = Application.dataPath + "/../_HotFix_Temp";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                cp.TempFiles = new TempFileCollection(path, true);
            }
           


            //获取当前项目所引用的程序集

            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");

            

            string unityDllPath = EditorApplication.applicationPath + @"/../Data/Managed/UnityEngine/";
   
            string scriptAssembliesPath = Application.dataPath + @"/../Library/ScriptAssemblies/";
            string futureLibPath = Application.dataPath + @"/_FutureFrame/FutureLib/";

            //unity dll文件
            cp.ReferencedAssemblies.Add(unityDllPath + "UnityEngine.dll");
            cp.ReferencedAssemblies.Add(unityDllPath + "UnityEngine.CoreModule.dll");

            //项目 dll文件
            cp.ReferencedAssemblies.Add(scriptAssembliesPath + "UnityEngine.UI.dll");
            cp.ReferencedAssemblies.Add(scriptAssembliesPath + "FutureFrame.dll");
            cp.ReferencedAssemblies.Add(scriptAssembliesPath + "Plugins3rdLibrary.dll");
            cp.ReferencedAssemblies.Add(scriptAssembliesPath + "ProjectApp.dll");

            //第三方dll文件
            cp.ReferencedAssemblies.Add(futureLibPath + "LogUtil/LogUtil.dll");

            //如果数据表使用的dll 则要引入表dll 
            if (AppConst.ExcelConfig_UseDll)
            {
                cp.ReferencedAssemblies.Add(Application.dataPath + "/_App/ProjectApp/AutoCreator/ExcelConfig/VODll/BaseVoClassLib.dll");
                cp.ReferencedAssemblies.Add(Application.dataPath + "/_App/ProjectApp/AutoCreator/ExcelConfig/VODll/VoClassLib.dll");
            }
        




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
                LogUtil.LogWarning( "存在文件夹的热更占位文件！    \n"+ ModuleMgrPath);
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
                    FutureCore.FileUtil.WriteAllText(path, contents,true);


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
                    FutureCore.FileUtil.WriteAllText(path, contents,true);
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