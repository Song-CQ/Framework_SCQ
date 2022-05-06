/****************************************************
    文件：CtrlMsg_Base.cs
	作者：Clear
    日期：2022/5/4 16:52:2
    类型: 逻辑脚本
	功能：Nothing
*****************************************************/
using UnityEngine;

namespace ProjectApp
{
    public static partial class CtrlMsg
    {
        public const string NAME = "CtrlMsg";
        public const uint BASE = 0;
        private static uint Cursor_BASE = BASE;

        /// 通用消息
        public static readonly uint CommonMsg = ++Cursor_BASE;

        // 游戏 开始准备
        public static readonly uint Game_StartReady = ++Cursor_BASE;
        // 游戏 开始之前
        public static readonly uint Game_StartBefore = ++Cursor_BASE;
        // 游戏 开始
        public static readonly uint Game_Start = ++Cursor_BASE;
        // 游戏 开始之后
        public static readonly uint Game_StartLater = ++Cursor_BASE;

    }
}