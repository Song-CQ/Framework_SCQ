using System;

namespace FutureCore
{
    /// <summary>
    /// Loading��������״̬
    /// </summary>
    public enum ProgressState : int
    {
        // δ���
        Unstart = -10,
        //资源准备
         AssetsPrepare = 0,
        // 检测版本更新
        VersionUpdate = 10,
        // 资源初始化
        AssetsInit = 70,
        // 加载热更代码
        LoadHotFix = 80,
        // 远程存储初始化
        PreferencesInit = 85,
        // 数据初始化
        ConfigInit = 90,
        // 加载完成
        ShowScene = 100,
    }

    public class UserInfo
    {
        public string userId;
    }

    /// <summary>
    /// ȫ��Ӧ��
    /// Ӧ�ò��߼�ע�뵽��ܲ�
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

        /// <summary>
        /// ��Ŀ���Init
        /// </summary>
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

        #region UIMsg

        public static void DisplayLoadingUI()
        {
            AppDispatcher.Instance.Dispatch(AppMsg.UI_DisplayLoadingUI);
        }



        public static void SetLoadingSchedule(ProgressState state,Action onComplete = null)
        {
            GenericDispatcher.Instance.Dispatch<int, Action>(AppMsg.UI_SetLoadingValueUI,(int)state,onComplete);

        }
        public static void HideLoadingUI(bool isDelay = false)
        {
            
             AppDispatcher.Instance.Dispatch(AppMsg.UI_HideLoadingUI);
       
            
        }

        //public static void SetLoadingUI(ProgressState state, bool isDelay = false)
        //{
        //    currProgressState = state;
        //    SetLoadingUI(state.ToString(), (int)state, isDelay);
        //}

        //public static void SetLoadingUI(string info, int progress, bool isDelay = false)
        //{
        //    SetLoadingUIInfo(info);
        //    SetLoadingUIProgress(progress, isDelay);
        //}

        //public static void SetLoadingUIInfo(string info)
        //{
        //    AppDispatcher.Instance.Dispatch(AppMsg.UI_SetLoadingUIInfo, info);
        //}

        //public static void SetLoadingUIProgress(int progress, bool isDelay = false)
        //{
        //    if (!isDelay)
        //    {
        //        AppDispatcher.Instance.Dispatch(AppMsg.UI_SetLoadingUIProgress, progress);
        //        return;
        //    }

        //    LoadingProgressDelayTime += AppConst.LoadingDelayTime;
        //    TimerUtil.Simple.AddTimer(LoadingProgressDelayTime, () =>
        //    {
        //        AppDispatcher.Instance.Dispatch(AppMsg.UI_SetLoadingUIProgress, progress);
        //    });
        //}

        //public static void SetLoadingUIProgressComplete(bool isDelay = false)
        //{
        //    if (!isDelay)
        //    {
        //        AppDispatcher.Instance.Dispatch(AppMsg.UI_LoadingUIProgressComplete, 100);
        //        return;
        //    }

        //    LoadingProgressDelayTime += AppConst.LoadingCompleteDelayTime + (AppConst.LoadingDelayTime * 2);
        //    TimerUtil.Simple.AddTimer(LoadingProgressDelayTime, () =>
        //    {
        //        LoadingProgressDelayTime = 0;
        //        AppDispatcher.Instance.Dispatch(AppMsg.UI_LoadingUIProgressComplete, 100);
        //    });
        //}

        //public static void DisplayWaitUI()
        //{
        //    AppDispatcher.Instance.Dispatch(AppMsg.UI_DisplayWaitUI);
        //}

        //public static void DisplayWaitTimeUI(float funcTime, Action func)
        //{
        //    WaitTimeActionClass waitObj = new WaitTimeActionClass(funcTime, func);
        //    AppDispatcher.Instance.Dispatch(AppMsg.UI_DisplayWaitTimeUI, waitObj);
        //}

        //public static void HideWaitUI()
        //{
        //    AppDispatcher.Instance.Dispatch(AppMsg.UI_HideWaitUI);
        //}

        //public static void ShowTipsUI(string text)
        //{
        //    AppDispatcher.Instance.Dispatch(AppMsg.UI_ShowTipsUI, text);
        //}

        //public static void ShowAffirmUI(string contentInfo, string affirmInfo, string cancelInfo, Action affirmFunc, Action cancelFunc)
        //{
        //    AppDispatcher.Instance.Dispatch(AppMsg.UI_ShowAffirmUI);
        //}

        //public static void ShowPlatformTipsUI(string text)
        //{
        //    AppDispatcher.Instance.Dispatch(AppMsg.UI_ShowPlatformTipsUI, text);
        //}
        #endregion UIMsg


    }


}