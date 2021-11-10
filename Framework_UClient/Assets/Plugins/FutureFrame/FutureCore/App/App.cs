using System;

namespace FutureCore
{
    /// <summary>
    /// Loading进度流程状态
    /// </summary>
    public enum ProgressState : int
    {
        // 未启动
        Unstart = -10,
        // 资源版本热更新
        //[InspectorName("资源版本热更新")]
        VersionUpdate = 0,
        // 资源初始化
        AssetsInit = 20,
        // 基础资源初始化
        PermanentAssetsInit = 25,
        // 连接登录
        ConnectLogin = 30,
        // 远程存储初始化
        PreferencesInit = 60,
        // 配置表初始化
        ConfigInit = 70,
        // 预加载开始
        PreloadStart = 80,
        // 显示场景
        ShowScene = 100,
    }

    public class UserInfo
    {
        public string userId;
    }

    /// <summary>
    /// 全局应用
    /// 应用层逻辑注入到框架层
    /// </summary>
    public static class App
    {
        #region User
        public static UserInfo UserInfo;
        #endregion 

        #region Application

        private static FCApplication currApplication = null;
        private static ProgressState currProgressState = ProgressState.Unstart;

        public static void InitApplication(FCApplication application)
        {
            currApplication = application;
            currApplication.Init();
            currApplication.Enable();
        }

        public static ProgressState GetCurrProgressState()
        {
            return currProgressState;
        }

        public static int GetCurrProgressStateValue()
        {
            return (int)currProgressState;
        }

        public static void Restart()
        {
            currApplication.Restart();
        }

        public static void Quit()
        {
            currApplication.Quit();
        }

        #endregion



        #region AppFacade
        public static string GetAppName()
        {
            return AppFacade_Frame.AppName;
        }


        public static void AppFacadeInit()
        {
            if (AppFacade_Frame.InitFunc != null)
            {
                AppFacade_Frame.InitFunc();
            }
        }

        public static void AppFacadeStartUp()
        {
            if (AppFacade_Frame.StartUpFunc != null)
            {
                AppFacade_Frame.StartUpFunc();
            }
        }

        public static void AppFacadeGameStart()
        {
            if (AppFacade_Frame.GameStartFunc != null)
            {
                AppFacade_Frame.GameStartFunc();
            }
        }

        #endregion

    }


}