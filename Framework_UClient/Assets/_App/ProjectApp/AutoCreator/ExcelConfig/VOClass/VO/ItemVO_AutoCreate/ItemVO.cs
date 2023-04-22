/****************************************************
    文件：ItemVO.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：Z_资源表_A 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class ItemVO:BaseVO
    {
        
        /// <summary>
        /// 中文描述 
        /// </summary>
        public string desc;

        /// <summary>
        /// 是否属于动态 
        /// </summary>
        public bool isDynamic;

        /// <summary>
        /// 是否实物 
        /// </summary>
        public bool material_object;

    }
}
        