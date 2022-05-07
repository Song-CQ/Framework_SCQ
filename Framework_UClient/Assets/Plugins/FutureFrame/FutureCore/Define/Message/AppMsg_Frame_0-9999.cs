namespace FutureCore
{
    /// <summary>
    /// 应用消息
    /// Frame_0-9999
    /// </summary>
    public class AppMsg
    {
        public const string NAME = "AppMsg";
        public const uint BASE = 0;
        private static uint Cursor_BASE = BASE;

        /// 应用消息
        // 应用拉起
        public static readonly uint App_StartUp = ++Cursor_BASE;
        // 应用退出
        public static readonly uint App_Quit = ++Cursor_BASE;
        // 应用失去焦点
        public static readonly uint App_Focus_False = ++Cursor_BASE;
        // 应用开始暂停
        public static readonly uint App_Pause_True = ++Cursor_BASE;
        // 应用游戏暂停
        public static readonly uint App_GamePause = ++Cursor_BASE;
        // 应用游戏恢复
        public static readonly uint App_GameResume = ++Cursor_BASE;

        //按下Home键
        public static readonly uint KeyCode_Home = ++Cursor_BASE;
        //按下退出键
        public static readonly uint KeyCode_Escape = ++Cursor_BASE;


        /// 系统消息
        // 系统管理器启动完成
        public static readonly uint System_ManagerStartUpComplete = ++Cursor_BASE;
        // 系统资源初始化完成
        public static readonly uint System_AssetsInitComplete = ++Cursor_BASE;
        /// <summary>
        /// 加载热更代码完成
        /// </summary>
        public static readonly uint System_LoadHotFixComplete = ++Cursor_BASE;
        /// <summary>
        /// Config 初始化完成
        /// </summary>
        public static readonly uint System_ConfigInitComplete = ++Cursor_BASE;



        /// <summary>
        /// 显示加载界面
        /// </summary>
        public static readonly uint UI_DisplayLoadingUI = ++Cursor_BASE;
        /// <summary>
        /// 设置加载界面进度
        /// </summary>
        public static readonly uint UI_SetLoadingValueUI = ++Cursor_BASE;
        /// <summary>
        /// 隐藏加载界面
        /// </summary>
        public static readonly uint UI_HideLoadingUI = ++Cursor_BASE;


        /// 场景消息
        // 场景切换
        public static readonly uint Scene_Switch = ++Cursor_BASE;

    }
}
