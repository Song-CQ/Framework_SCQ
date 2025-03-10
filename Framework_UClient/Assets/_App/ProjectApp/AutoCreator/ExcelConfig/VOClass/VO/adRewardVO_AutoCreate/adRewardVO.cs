/****************************************************
    文件：adRewardVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：adReward 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class adRewardVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.adReward;
        
        
        /// <summary>
        /// 中文描述 
        /// </summary>
        public string desc;

        /// <summary>
        /// 奖励项 
        /// </summary>
        public int reward;

        /// <summary>
        /// 数量 
        /// </summary>
        public int rewardAmount;

        /// <summary>
        /// 倍率 
        /// </summary>
        public int[] ratio;

        /// <summary>
        /// x1至x5的概率 
        /// </summary>
        public int[] probability;

        /// <summary>
        /// 冷却时间秒 
        /// </summary>
        public int cdTime;

    }
}
        