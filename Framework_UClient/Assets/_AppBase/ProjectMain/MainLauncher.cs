using System;
using System.Collections;
using System.Collections.Generic;
using FutureCore;
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

            LogUtil.Log("[MainLauncher]SceneMain".AddColor(StringTool.ColorType.Green));
            Main();
        }
        
        private static void Main()
        {
            if (IsInMain) return;

            LogUtil.Log("[MainLauncher]Main".AddColor(StringTool.ColorType.Green));

            // 版本检测
            if (!Application.unityVersion.StartsWith("2019.2"))
            {
                LogUtil.Log("[MainLauncher]UnityVersion mismatching".AddColor(StringTool.ColorType.亮黄色));
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
            
        }

    }
}

