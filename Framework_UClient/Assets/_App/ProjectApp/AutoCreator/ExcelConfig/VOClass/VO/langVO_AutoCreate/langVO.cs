/****************************************************
    文件：langVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：lang 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class langVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.lang;
        
        
        /// <summary>
        /// 名称显示 
        /// </summary>
        public string name;

        /// <summary>
        /// 语言 
        /// </summary>
        public string language;

        /// <summary>
        /// 中文描述 
        /// </summary>
        public string desc;

    }
}
        