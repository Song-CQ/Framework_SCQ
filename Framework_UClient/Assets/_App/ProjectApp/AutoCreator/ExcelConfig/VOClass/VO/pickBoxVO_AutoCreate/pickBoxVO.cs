/****************************************************
    文件：pickBoxVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：pickBox 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class pickBoxVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.pickBox;
        
        
        /// <summary>
        /// 奖励Id 
        /// </summary>
        public int itemId;

        /// <summary>
        /// 是否 
        /// </summary>
        public bool isCoin;

        /// <summary>
        /// 累计资产对应的奖励 
        /// </summary>
        public int[] quantity;

    }
}
        