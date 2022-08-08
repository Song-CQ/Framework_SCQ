/****************************************************
    文件：LayerMaskConst.cs
	作者：Clear
    日期：2022/1/15 17:52:12
    类型: 框架核心脚本(请勿修改)
	功能：层级常量
*****************************************************/
using UnityEngine;

namespace FutureCore
{
    public static class LayerMaskConst
    {
        public const string Everything_Name = "Everything";
        public const string Default_Name = "Default";
        public const string UI_Name = "UI";

        public readonly static int Everything = LayerMask.NameToLayer(Everything_Name);
        public readonly static int Default = LayerMask.NameToLayer(Default_Name);
        public readonly static int UI = LayerMask.NameToLayer(UI_Name);
    }
}