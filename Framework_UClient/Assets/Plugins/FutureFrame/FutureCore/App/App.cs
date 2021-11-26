using System;

namespace FutureCore
{
    /// <summary>
    /// Loading��������״̬
    /// </summary>
    public enum ProgressState : int
    {
        // δ����
        Unstart = -10,
        // ��Դ�汾�ȸ���
        //[InspectorName("��Դ�汾�ȸ���")]
        VersionUpdate = 0,
        // ��Դ��ʼ��
        AssetsInit = 20,
        // ������Դ��ʼ��
        PermanentAssetsInit = 25,
        // ���ӵ�¼
        ConnectLogin = 30,
        // Զ�̴洢��ʼ��
        PreferencesInit = 60,
        // ���ñ���ʼ��
        ConfigInit = 70,
        // Ԥ���ؿ�ʼ
        PreloadStart = 80,
        // ��ʾ����
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

    }


}