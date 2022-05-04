/****************************************************
    文件: UICtrlMsg_OpenClose.cs
	作者: Clear
    日期: 2022/5/4 12:53:10
    类型: 框架自动创建(请勿修改)
	功能: UI打开关闭消息
*****************************************************/
namespace ProjectApp
{
    public static partial class UICtrlMsg
    {
        private static uint cursor_UIOpenClose = 110000;

        public static uint GameUI_Open = ++cursor_UIOpenClose;
        public static uint GameUI_Close = ++cursor_UIOpenClose;
        public static uint MainUI_Open = ++cursor_UIOpenClose;
        public static uint MainUI_Close = ++cursor_UIOpenClose;


    }
}