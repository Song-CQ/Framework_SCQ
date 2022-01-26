/****************************************************
    文件：UICtrlMsg_OpenClose.cs
	作者：Clear
    日期：2022/1/26 14:39:26
    类型: 框架核心脚本(请勿修改)
	功能：UI打开关闭消息
*****************************************************/
namespace FutureCore
{
    public static partial class UICtrlMsg
    {
        private static uint cursor_Logic = 110000;

        public static uint MainUI_Open = ++cursor_Logic;
        public static uint MainUI_Close = ++cursor_Logic;

    }
}