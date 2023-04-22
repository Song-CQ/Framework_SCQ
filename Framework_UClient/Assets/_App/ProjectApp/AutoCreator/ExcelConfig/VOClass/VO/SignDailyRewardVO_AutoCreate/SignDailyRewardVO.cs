/****************************************************
    文件：SignDailyRewardVO.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：M_每日签到表_A 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class SignDailyRewardVO:BaseVO
    {
        
        /// <summary>
        /// 奖励id 
        /// </summary>
        public int item1;

        /// <summary>
        /// 累计资产对应的奖励 
        /// </summary>
        public int[] quantity;

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
        