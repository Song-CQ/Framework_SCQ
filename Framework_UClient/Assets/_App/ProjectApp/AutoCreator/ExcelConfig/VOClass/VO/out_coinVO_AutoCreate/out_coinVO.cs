/****************************************************
    文件：out_coinVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：out_coin 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class out_coinVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.out_coin;
        
        
        /// <summary>
        /// 累计获得金币数量 
        /// </summary>
        public int[] totalCoin;

        /// <summary>
        /// bingo金币与*3金币产出 
        /// </summary>
        public int[] coinRange;

        /// <summary>
        /// 是否视频 
        /// </summary>
        public bool isVideo;

    }
}
        