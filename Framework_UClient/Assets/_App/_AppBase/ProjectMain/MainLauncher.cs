using FutureCore;
using FuturePlugin;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ProjectApp
{
    public static class MainLauncher
    {
        public static bool IsAutoLauncher = true;
        private const string MainScene = "0_MainScene";

        private static bool IsInMain = false;

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //在加载场景后调用 保证ProjectAppSet对象被顺利读取
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void SceneMain()
        {
            if (!IsAutoLauncher) return;
            if (SceneManager.GetActiveScene().name != MainScene) return;

            LogInit.InitLog();
            
            LogUtil.Log("[MainLauncher]SceneMain".AddColor(ColorType.Green));

            Main();
        }

        

        private static void Main()
        {
            if (IsInMain) return;

            LogUtil.Log("[MainLauncher]Main".AddColor(ColorType.Green));

            // 版本检测
            if (!Application.unityVersion.StartsWith("2022.3.34f"))
            {
                LogUtil.Log("[MainLauncher]UnityVersion mismatching".AddColor(ColorType.亮黄色));
            }

            // 进入框架程序
            IsInMain = true;
            // 应用启动时间
            AppConst.LaunchDateTime = DateTime.Now;

            // 外观入口
            AppFacade_Frame.MainFunc();
            // 初始化平台
            //SDKGlobal.InitPlatform();
            // 启动引擎层
            AppLauncher();
        }

        private static void AppLauncher()
        {
            //创建框架Go
            AppObjConst.FutureFrameGo = new GameObject(AppObjConst.FutureFrameGoName);
            AppObjConst.FutureFrameGo.AddComponent<FutureFrame>();
            Unity3dUtil.SetDontDestroyOnLoad(AppObjConst.FutureFrameGo);
            //创建引擎go
            AppObjConst.LauncherGo = new GameObject(AppObjConst.LauncherGoName);
            AppObjConst.LauncherGo.SetParent(AppObjConst.FutureFrameGo);
            AppObjConst.LauncherGo.AddComponent<EngineLauncher>().Init(AppMain);
        }
        private static void AppMain()
        {
            //启动项目层
            Assembly appAssembly = Assembly.GetExecutingAssembly();
            Type appMainClass = appAssembly.GetType("ProjectApp.Main.AppMain");
            MethodInfo mainFunc = appMainClass.GetMethod("Main", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
            mainFunc.Invoke(null, null);

        }
    }
}

