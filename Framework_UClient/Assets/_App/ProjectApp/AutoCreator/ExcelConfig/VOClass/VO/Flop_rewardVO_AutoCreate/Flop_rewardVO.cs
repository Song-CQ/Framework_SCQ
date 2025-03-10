/****************************************************
    文件：Flop_rewardVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：Flop_reward 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class Flop_rewardVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.Flop_reward;
        
        
        /// <summary>
        /// 奖励ID 
        /// </summary>
        public int itemID;

        /// <summary>
        /// 累计资产对应的奖励 
        /// </summary>
        public int[] quantity;

        /// <summary>
        /// 奖励权重 
        /// </summary>
        public int weight;

        /// <summary>
        /// 是否翻倍 
        /// </summary>
        public bool isMulti;

        /// <summary>
        /// 控制器 
        /// </summary>
        public int cont_index;

    }
}
        