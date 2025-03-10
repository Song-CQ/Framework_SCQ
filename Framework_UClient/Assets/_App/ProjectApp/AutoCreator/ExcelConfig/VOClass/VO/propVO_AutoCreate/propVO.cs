/****************************************************
    文件：propVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：prop 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class propVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.prop;
        
        
        /// <summary>
        /// 影响1,2,4卡桌格子数量 
        /// </summary>
        public int[] quantity;

    }
}
        