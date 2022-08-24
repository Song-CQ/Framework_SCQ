/****************************************************************************
* ScriptType: 主框架
* 请勿修改!!!
****************************************************************************/

namespace FutureCore
{
    /// <summary>
    /// 主线程消息
    /// Frame_0-9999
    /// </summary>
    public static class MainThreadMsg
    {
        public const string NAME = "MainThreadMsg";
        public const uint BASE = 0;
        private static uint Cursor_BASE = BASE;

        /// 应用消息
        // 应用得到焦点
        public static readonly uint App_Focus_True = ++Cursor_BASE;
        // 应用结束暂停
        public static readonly uint App_Pause_False = ++Cursor_BASE;
    }
}