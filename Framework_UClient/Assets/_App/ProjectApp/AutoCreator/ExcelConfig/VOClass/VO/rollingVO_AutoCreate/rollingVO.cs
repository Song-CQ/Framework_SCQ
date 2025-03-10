/****************************************************
    文件：rollingVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：rolling 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class rollingVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.rolling;
        
        
        /// <summary>
        /// 邮箱 
        /// </summary>
        public string email;

        /// <summary>
        /// 奖励 
        /// </summary>
        public string reward;

    }
}
        