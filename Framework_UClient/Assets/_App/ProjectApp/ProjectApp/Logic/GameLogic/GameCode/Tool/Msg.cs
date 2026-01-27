using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    /// <summary>
    /// 游戏消息定义
    /// </summary>
    public static class GameMsg
    {
        private static uint _counter = 1000;
        /// <summary>
        /// 点击一个元素
        /// </summary>
        public static readonly uint ClickElement = ++_counter;
        /// <summary>
        /// 拖动一个元素到另一个元素
        /// </summary>
        public static readonly uint SwipeElement = ++_counter;

        
        

        /// <summary>
        /// 选中一个元素
        /// </summary>
        public static readonly uint SelectElement = ++_counter;

        /// <summary>
        /// 取消选中元素
        /// </summary>
        public static readonly uint DeselectElement = ++_counter; // 1002

        /// <summary>
        /// 交换两个元素
        /// </summary>
        public static readonly uint SwapElements = ++_counter;

        /// <summary>
        /// 元素匹配成功
        /// </summary>
        public static readonly uint MatchElements = ++_counter;

        /// <summary>
        /// 元素消除
        /// </summary>
        public static readonly uint ClearElements = ++_counter;

        /// <summary>
        /// 生成新元素
        /// </summary>
        public static readonly uint GenerateElements = ++_counter;

        /// <summary>
        /// 元素下落
        /// </summary>
        public static readonly uint ElementsFall = ++_counter;

        /// <summary>
        ///  改变元素的类型
        /// </summary>
        public static readonly uint RestElements = ++_counter;
        /// <summary>
        ///  改变全部元素的类型
        /// </summary>
        public static readonly uint RestAllElements = ++_counter;

        /// <summary>
        /// 游戏分数更新
        /// </summary>
        public static readonly uint ScoreUpdated = ++_counter;

        /// <summary>
        /// 游戏结束
        /// </summary>
        public static readonly uint GameOver = ++_counter;

        /// <summary>
        /// 游戏胜利
        /// </summary>
        public static readonly uint GameWin = ++_counter;

    }

}
