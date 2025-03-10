/****************************************************
    文件：turntableVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：turntable 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class turntableVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.turntable;
        
        
        /// <summary>
        /// 序号 
        /// </summary>
        public int Seq;

        /// <summary>
        /// 奖品类型 
        /// </summary>
        public int wheel_itemA;

        /// <summary>
        /// 累计资产对应的奖励_免费转盘 
        /// </summary>
        public int[] quantityA;

        /// <summary>
        /// 权重 
        /// </summary>
        public int weightA;

        /// <summary>
        /// 是否翻倍 
        /// </summary>
        public bool isMulti;

        /// <summary>
        /// 控制器 
        /// </summary>
        public int cont_index;

        /// <summary>
        /// 奖品类型 
        /// </summary>
        public int wheel_itemB;

        /// <summary>
        /// 累计资产对应的奖励_广告转盘 
        /// </summary>
        public int[] quantityB;

        /// <summary>
        /// 权重 
        /// </summary>
        public int weightB;

    }
}
        