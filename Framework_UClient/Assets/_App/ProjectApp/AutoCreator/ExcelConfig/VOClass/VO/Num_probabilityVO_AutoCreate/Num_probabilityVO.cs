/****************************************************
    文件：Num_probabilityVO.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：P_匹配概率表_A 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class Num_probabilityVO:BaseVO
    {
        
        /// <summary>
        /// 牌桌数 
        /// </summary>
        public int cardNum;

        /// <summary>
        /// 叫号匹配概率 
        /// </summary>
        public int probability;

        /// <summary>
        /// 全局daub数量判断 
        /// </summary>
        public int[] daubNum;

        /// <summary>
        /// 优先叫号可连bingo线的概率 
        /// </summary>
        public int[] bingoprob;

    }
}
        