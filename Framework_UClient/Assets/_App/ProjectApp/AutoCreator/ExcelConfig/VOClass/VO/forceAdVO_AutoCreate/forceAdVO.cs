/****************************************************
    文件：forceAdVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：forceAd 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class forceAdVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.forceAd;
        
        
        /// <summary>
        /// 中文描述 
        /// </summary>
        public string desc;

        /// <summary>
        /// 间隔次数 
        /// </summary>
        public int cdtime;

    }
}
        