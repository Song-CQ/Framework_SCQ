/****************************************************
    文件: UICtrlMsg_OpenClose.cs
	作者: Clear
    日期: 2023/11/24 15:47:44
    类型: 框架自动创建(请勿修改)
	功能: UI打开关闭消息
*****************************************************/
namespace ProjectApp
{
    public static partial class UICtrlMsg
    {
        private static uint cursor_UIOpenClose = 110000;

        public static uint dfasdasUI_Open = ++cursor_UIOpenClose;
        public static uint dfasdasUI_Close = ++cursor_UIOpenClose;
        public static uint FXEffectUI_Open = ++cursor_UIOpenClose;
        public static uint FXEffectUI_Close = ++cursor_UIOpenClose;
        public static uint GameUI_Open = ++cursor_UIOpenClose;
        public static uint GameUI_Close = ++cursor_UIOpenClose;
        public static uint MainUI_Open = ++cursor_UIOpenClose;
        public static uint MainUI_Close = ++cursor_UIOpenClose;
        public static uint TipsUI_Open = ++cursor_UIOpenClose;
        public static uint TipsUI_Close = ++cursor_UIOpenClose;


    }
}