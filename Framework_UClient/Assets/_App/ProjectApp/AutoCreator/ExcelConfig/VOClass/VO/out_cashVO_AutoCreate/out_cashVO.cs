/****************************************************
    文件：out_cashVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：out_cash 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class out_cashVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.out_cash;
        
        
        /// <summary>
        /// 累计获得钞票数量 
        /// </summary>
        public int[] totalCash;

        /// <summary>
        /// 玩法cash产出区间 
        /// </summary>
        public int[] cashRange;

        /// <summary>
        /// cash*3产出区间 
        /// </summary>
        public int[] cashRange2;

        /// <summary>
        /// 是否视频 
        /// </summary>
        public bool isVideo;

        /// <summary>
        /// 增减浮动值 
        /// </summary>
        public float range;

    }
}
        