/****************************************************
    文件：pickBoxVO.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：F_翻盒子奖励表_A 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class pickBoxVO:BaseVO
    {
        
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
        