/****************************************************
    文件：piggyVO.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：X_小猪存钱表_A 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class piggyVO:BaseVO
    {
        
        /// <summary>
        /// 奖品类型 
        /// </summary>
        public int itemID;

        /// <summary>
        /// 目标美元 
        /// </summary>
        public int cashGoal;

        /// <summary>
        /// 每局获得美元范围 
        /// </summary>
        public float[] cashReceive;

        /// <summary>
        /// 增减浮动值 
        /// </summary>
        public float range;

        /// <summary>
        /// 提现需要局数量 
        /// </summary>
        public int gameNum;

        /// <summary>
        /// 提现需要视频数量 
        /// </summary>
        public int adNum;

    }
}
        