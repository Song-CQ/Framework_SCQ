/****************************************************
    文件: UICtrlMsg_OpenClose_Common.cs
	作者: Clear
    日期: 2022/5/2 23:26:38
    类型: 框架自动创建(请勿修改)
	功能: UI打开关闭消息(通用)
*****************************************************/
namespace ProjectApp
{
    public static partial class UICtrlMsg
    {
        private static uint cursor_UIOpenClose_Common = 110000;

        public static uint LoadingUI_Open = ++cursor_UIOpenClose_Common;
        public static uint LoadingUI_Close = ++cursor_UIOpenClose_Common;


    }
}