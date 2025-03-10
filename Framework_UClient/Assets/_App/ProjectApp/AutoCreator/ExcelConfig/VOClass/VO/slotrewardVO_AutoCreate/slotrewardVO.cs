/****************************************************
    文件：slotrewardVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：slotreward 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class slotrewardVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.slotreward;
        
        
        /// <summary>
        /// 序号 
        /// </summary>
        public int Seq;

        /// <summary>
        /// 奖品类型 
        /// </summary>
        public int wheel_item;

        /// <summary>
        /// 累计资产对应的奖励 
        /// </summary>
        public int[] quantity;

        /// <summary>
        /// 概率 
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
        