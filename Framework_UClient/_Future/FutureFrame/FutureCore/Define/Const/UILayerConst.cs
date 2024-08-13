/****************************************************
    文件：UILayerConst.cs
	作者：Clear
    日期：2022/1/16 18:52:37
    类型: 框架核心脚本(请勿修改)
	功能：UI层级常量
*****************************************************/
namespace FutureCore
{
    public static class UILayerConst 
    {
        public const string Background = "Background";
        public const string Bottom = "Bottom";

        public const string Normal = "Normal";
        public const string Top = "Top";

        public const string FullScreen = "FullScreen";
        public const string Popup = "Popup";

        public const string Highest = "Highest";
        public const string Animation = "Animation";
        public const string Tips = "Tips";

        public const string Loading = "Loading";
        public const string System = "System";

        public static readonly string[] AllUILayer = new[]
        {
            Background,
            Bottom,

            Normal,
            Top,

            FullScreen,
            Popup,

            Highest,
            Animation,
            Tips,

            Loading,
            System
        };




    }
}