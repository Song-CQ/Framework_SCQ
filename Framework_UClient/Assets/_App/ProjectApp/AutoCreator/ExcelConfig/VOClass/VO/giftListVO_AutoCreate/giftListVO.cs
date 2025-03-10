/****************************************************
    文件：giftListVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：giftList 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class giftListVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.giftList;
        
        
        /// <summary>
        /// 首次增加数量 
        /// </summary>
        public int increaseFirst;

        /// <summary>
        /// 单次增加 
        /// </summary>
        public int increase;

        /// <summary>
        /// 实物泡泡列表隐藏 
        /// </summary>
        public int hide_1;

        /// <summary>
        /// 实物转盘列表隐藏 
        /// </summary>
        public int hide_2;

        /// <summary>
        /// 展示数量 
        /// </summary>
        public int quantity;

    }
}
        