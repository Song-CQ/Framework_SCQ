namespace ProjectApp
{
    /// <summary>
    /// ��Ŀ��������
    /// </summary>
    public static class AppFacade
    {
        #region Ӧ������
        /// <summary>
        /// Ӧ�ô���
        /// </summary>
        public const string AppName = "Framework";

        /// <summary>
        /// ��Ŀ����
        /// </summary>
        public const string AppDesc = "�������";

        /// <summary>
        /// ����
        /// </summary>
        public const string PackageName = "com.Framework_SCQ.Test";


        /// <summary>
        /// ��ԿKey
        /// </summary>
        public const string AESKey = "1543065415321000";

        /// <summary>
        /// ��ԿIVector
        /// </summary>
        public const string AESIVector = "1543065415321000";

        /// <summary>
        /// ��������ǩ
        /// </summary>
       // public const string ServerTag = "test_zef";

        /// <summary>
        /// ��Ϸ��������
        /// </summary>
        //public static string[] WebSocketUrls =
        //{
        //    "wss://centhadst.pub/sctricard/", // �ٷ�
        //    "wss://www.centhadst.pub/sctricard/", // AWS CDN
        //    "wss://cloudflare.centhadst.pub/sctricard/", // Cloudflare CDN
        //};

        /// <summary>
        /// �����˿�
        /// </summary>
        //public const string WebSocketPort = "/8006/";

        /// <summary>
        /// ����˿�
        /// </summary>
        //public const string WebSocketTestPort = "/8056/"; //"/8066/";

        /// <summary>
        /// ����
        /// </summary>
       // public const string Domain = "centhadst.pub";

        /// <summary>
        /// SDK�ӿ�ǰ׺
        /// ���ݲ�Ʒ�������˺�����д: solitaire / slidey
        /// </summary>
        //public const string SDKApiPrefix = AppName;

        /// <summary>
        /// BuglyAppIDForAndroid
        /// </summary>
       // public const string BuglyAppIDForAndroid = "8d30676fa7";

        /// <summary>
        /// BuglyAppIDForiOS
        /// </summary>
       // public const string BuglyAppIDForiOS = "7c5289d3ae";

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        //public const bool IsWeakNetwork = true;

        /// <summary>
        /// �Ƿ�������Ϸ
        /// </summary>
        //public const bool IsOfflineGame = true;

        /// <summary>
        /// �Զ���SDK
        /// </summary>

        //public static ISDK[] CustomSDK = null;
        //new ISDK[]{ new BasePlatformOS(), new BaseChannel(), new BaseCoreSDK() };



        /// <summary>
        /// �Ƿ�ʹ��UGameAndroid����(ֻӰ��Android���룬��Ӱ��iOS����)
        /// </summary>
        public const bool IsUseUGameAndroid = true;

        /// <summary>
        /// ��������Դ·��
        /// </summary>
        public const string ServerAssestUrl = "";
        #endregion

        #region ����
        /// <summary>
        /// ��Ŀ���
        /// </summary>
        public static void MainFunc()
        {
            // ���÷ֱ���
            //AppConst.StandardResolution = new Vector2Int(720, 1280);
        }

        /// <summary>
        /// ��ʼ��ǰ �Զ�����Ŀ����
        /// </summary>
        public static void InitFunc()
        {
            // ��ʹ����������
            //AppConst.UseInternalSetting = false;

            // ���ö�����
            //AppConst.IsMultiLanguage = true;
            //AppConst.DefaultLanguage = "en";

            // ����Ҫ��Ϸ�������
            //CommonConfig.LoadingUI = false;
            // ����ҪApp��������
            //AppConst.IsNeedPromptAppUpdate = false;
        }

        /// <summary>
        /// ������Ϸ �Զ�����Ŀ����
        /// </summary>
        public static void StartUpFunc()
        {
            // ���粻��ҪLoading�ӳ�
            //AppConst.ShowLoadingSplashPageTime = 0;
            //AppConst.IsLoadingDelay = false;
            //AppConst.LoadingDelayTime = 0;
            //AppConst.LoadingCompleteDelayTime = 0;
            //AppConst.GameStartReadyDelayTime = 0;
        }

        /// <summary>
        /// ��ʼ��Ϸǰ �Զ�����Ŀ����
        /// </summary>
        public static void GameStartFunc()
        {
            // ���粻��Ҫ����ܵ�mainCamera fguiCamera audioListener
            //CameraMgr.Instance.mainCamera.enabled = false;
            //CameraMgr.Instance.fguiCamera.enabled = false;
            //AudioMgr.Instance.audioListener.enabled = false;
        }
        #endregion
    }
}