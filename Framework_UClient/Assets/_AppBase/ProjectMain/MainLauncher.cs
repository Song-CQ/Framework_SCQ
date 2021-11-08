using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FutureCore;
using FuturePlugin;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectApp.App
{
    public static class MainLauncher
    {
        public static bool IsAutoLauncher = true;
        private const string MainScene = "0_MainScene";

        private static bool IsInMain = false;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SceneMain()
        {               
            if (!IsAutoLauncher) return;
            if (SceneManager.GetActiveScene().name != MainScene) return;

            LogUtil.Log("[MainLauncher]SceneMain".AddColor(StringExtend.ColorType.Green));
            Main();
        }
        
        private static void Main()
        {
            if (IsInMain) return;

            LogUtil.Log("[MainLauncher]Main".AddColor(StringExtend.ColorType.Green));

            // 版本检测
            if (!Application.unityVersion.StartsWith("2019.4.32f"))
            {
                LogUtil.Log("[MainLauncher]UnityVersion mismatching".AddColor(StringExtend.ColorType.亮黄色));
            }

            // 进入框架程序
            IsInMain = true;
            // 应用启动时间
            AppConst.LaunchDateTime = DateTime.Now;

            // 外观入口
            AppFacade.MainFunc();
            // 初始化平台
            //SDKGlobal.InitPlatform();
            // 启动引擎层
            AppLauncher();
        }

        private static void AppLauncher()
        {
            AppObjConst.FutureFrameGo = new GameObject(AppObjConst.FutureFrameGoName);
            AppObjConst.FutureFrameGo.AddComponent<FutureFrame>();
            Unity3dUtil.SetDontDestroyOnLoad(AppObjConst.FutureFrameGo);
            
            AppObjConst.LauncherGo = new GameObject(AppObjConst.LauncherGoName);
            AppObjConst.LauncherGo.SetParent(AppObjConst.FutureFrameGo);
            AppObjConst.LauncherGo.AddComponent<EngineLauncher>().Init(AppMain);
        }
        private static void AppMain()
        {
            return;
            Assembly appAssembly = Assembly.GetExecutingAssembly();
            Type appMainClass = appAssembly.GetType("ProjectApp.Main.AppMain");
            MethodInfo mainFunc = appMainClass.GetMethod("Main", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
            mainFunc.Invoke(null, null);
         
        }
    }
}

