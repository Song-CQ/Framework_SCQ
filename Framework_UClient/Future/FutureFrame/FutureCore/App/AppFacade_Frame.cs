/****************************************************************************
* ScriptType: 主框架
* 请勿修改!!!
****************************************************************************/

using System;

namespace FutureCore
{
    /// <summary>
    /// 应用配置_框架
    /// </summary>
    public static class AppFacade_Frame
    {
        /// <summary>
        /// 应用代号
        /// </summary>
        public static string AppName;

        /// <summary>
        /// 项目描述
        /// </summary>
        public static string AppDesc;

        /// <summary>
        /// 包名
        /// </summary>
        public static string PackageName;

        /// <summary>
        /// 密钥Key
        /// </summary>
        public static string AESKey;

        /// <summary>
        /// 密钥IVector
        /// </summary>
        public static string AESIVector;

        /// <summary>
        /// 游戏服连接组
        /// </summary>
        public static string[] WebSocketUrls;

        /// <summary>
        /// 正服端口
        /// </summary>
        public static string WebSocketPort;

        /// <summary>
        /// 测服端口
        /// </summary>
        public static string WebSocketTestPort;

        /// <summary>
        /// 域名
        /// </summary>
        public static string Domain;

        /// <summary>
        /// SDK接口前缀
        /// 根据产品发布的账号来填写: solitaire / slidey
        /// </summary>
        public static string SDKApiPrefix;

        /// <summary>
        /// BuglyAppIDForAndroid
        /// </summary>
        public static string BuglyAppIDForAndroid;

        /// <summary>
        /// BuglyAppIDForiOS
        /// </summary>
        public static string BuglyAppIDForiOS;

        /// <summary>
        /// 是否弱联网
        /// </summary>
        public static bool IsWeakNetwork;

        /// <summary>
        /// 是否离线游戏
        /// </summary>
        public static bool IsOfflineGame;

        /// <summary>
        /// 自定义SDK
        /// </summary>
        //public static ISDK[] CustomSDK;

        /// <summary>
        /// 是否使用UGameAndroid编译(只影响android编译，不影响ios编译)
        /// </summary>
        public static bool IsUseUGameAndroid;

        /// <summary>
        /// 初始化前 自定义项目设置
        /// </summary>
        public static Action InitFunc;

        /// <summary>
        /// 启动游戏 自定义项目设置
        /// </summary>
        public static Action StartUpFunc;

        /// <summary>
        /// 开始游戏 自定义项目设置
        /// </summary>
        public static Action GameStartFunc;

        /// <summary>
        /// 服务器资源路劲
        /// </summary>
        public static string ServerAssestUrl;
    }
}