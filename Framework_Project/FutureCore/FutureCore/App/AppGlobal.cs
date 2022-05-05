/****************************************************
    文件：AppGlobal.cs
	作者：Clear
    日期：2022/1/13 15:35:9
    类型: 框架核心脚本(请勿修改)
	功能：Nothing
*****************************************************/
namespace FutureCore
{
    public static class AppGlobal 
    {
        private static bool m_isLoginSucceed = false;
        /// <summary>
        /// 是否用户登录成功
        /// </summary>
        public static bool IsLoginSucceed
        {
            get
            {
                return /*WSNetMgr.Instance.isConnected && NetConst.IsNetAvailable &&*/ m_isLoginSucceed;
            }
            set
            {
                m_isLoginSucceed = value;
            }
        }
        /// <summary>
        /// 游戏是否开始
        /// </summary>
        public static bool IsGameStart = false;

        /// <summary>
        /// 游戏是否暂停
        /// </summary>
        public static bool IsGamePause = false;

        /// <summary>
        /// 是否显示断线提示
        /// </summary>
        public static bool IsShowDisconnectionTips = true;


    }
}