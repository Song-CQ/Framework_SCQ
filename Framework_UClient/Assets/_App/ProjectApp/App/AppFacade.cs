namespace ProjectApp
{
    /// <summary>
    /// 项目配属属性
    /// </summary>
    public static class AppFacade
    {
        #region 应用配置
        /// <summary>
        /// 应用代号
        /// </summary>
        public const string AppName = "Framework";

        /// <summary>
        /// 项目描述
        /// </summary>
        public const string AppDesc = "基础框架";

        /// <summary>
        /// 包名
        /// </summary>
        public const string PackageName = "com.Framework_SCQ.Test";


        /// <summary>
        /// 密钥Key
        /// </summary>
        public const string AESKey = "1543065415321000";

        /// <summary>
        /// 密钥IVector
        /// </summary>
        public const string AESIVector = "1543065415321000";

        /// <summary>
        /// 服务器标签
        /// </summary>
       // public const string ServerTag = "test_zef";

        /// <summary>
        /// 游戏服连接组
        /// </summary>
        //public static string[] WebSocketUrls =
        //{
        //    "wss://centhadst.pub/sctricard/", // 官服
        //    "wss://www.centhadst.pub/sctricard/", // AWS CDN
        //    "wss://cloudflare.centhadst.pub/sctricard/", // Cloudflare CDN
        //};

        /// <summary>
        /// 正服端口
        /// </summary>
        //public const string WebSocketPort = "/8006/";

        /// <summary>
        /// 测服端口
        /// </summary>
        //public const string WebSocketTestPort = "/8056/"; //"/8066/";

        /// <summary>
        /// 域名
        /// </summary>
       // public const string Domain = "centhadst.pub";

        /// <summary>
        /// SDK接口前缀
        /// 根据产品发布的账号来填写: solitaire / slidey
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
        /// 是否弱联网
        /// </summary>
        //public const bool IsWeakNetwork = true;

        /// <summary>
        /// 是否离线游戏
        /// </summary>
        //public const bool IsOfflineGame = true;

        /// <summary>
        /// 自定义SDK
        /// </summary>

        //public static ISDK[] CustomSDK = null;
        //new ISDK[]{ new BasePlatformOS(), new BaseChannel(), new BaseCoreSDK() };



        /// <summary>
        /// 是否使用UGameAndroid编译(只影响Android编译，不影响iOS编译)
        /// </summary>
        public const bool IsUseUGameAndroid = true;
        #endregion

        #region 方法
        /// <summary>
        /// 项目入口
        /// </summary>
        public static void MainFunc()
        {
            // 设置分辨率
            //AppConst.StandardResolution = new Vector2Int(720, 1280);
        }

        /// <summary>
        /// 初始化前 自定义项目设置
        /// </summary>
        public static void InitFunc()
        {
            // 不使用内置设置
            //AppConst.UseInternalSetting = false;

            // 设置多语言
            //AppConst.IsMultiLanguage = true;
            //AppConst.DefaultLanguage = "en";

            // 不需要游戏载入界面
            //CommonConfig.LoadingUI = false;
            // 不需要App更新提醒
            //AppConst.IsNeedPromptAppUpdate = false;
        }

        /// <summary>
        /// 启动游戏 自定义项目设置
        /// </summary>
        public static void StartUpFunc()
        {
            // 比如不需要Loading延迟
            //AppConst.ShowLoadingSplashPageTime = 0;
            //AppConst.IsLoadingDelay = false;
            //AppConst.LoadingDelayTime = 0;
            //AppConst.LoadingCompleteDelayTime = 0;
            //AppConst.GameStartReadyDelayTime = 0;
        }

        /// <summary>
        /// 开始游戏前 自定义项目设置
        /// </summary>
        public static void GameStartFunc()
        {
            // 比如不需要主框架的mainCamera fguiCamera audioListener
            //CameraMgr.Instance.mainCamera.enabled = false;
            //CameraMgr.Instance.fguiCamera.enabled = false;
            //AudioMgr.Instance.audioListener.enabled = false;
        }
        #endregion
    }
}