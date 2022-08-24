/****************************************************
    文件：adRewardVO.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：G_广告奖励表_A 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class adRewardVO:BaseVO
    {
        
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
        