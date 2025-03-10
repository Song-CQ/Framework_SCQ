/****************************************************
    文件：TaskListVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：TaskList 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class TaskListVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.TaskList;
        
        
        /// <summary>
        /// 任务描述 
        /// </summary>
        public string taskName;

        /// <summary>
        /// 任务数量 
        /// </summary>
        public int taskNum;

        /// <summary>
        /// 奖品id 
        /// </summary>
        public int rewardID;

        /// <summary>
        /// 首次奖励 
        /// </summary>
        public int rewardFirst;

        /// <summary>
        /// 累计资产挡位对应奖励 
        /// </summary>
        public int[] reward;

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
        