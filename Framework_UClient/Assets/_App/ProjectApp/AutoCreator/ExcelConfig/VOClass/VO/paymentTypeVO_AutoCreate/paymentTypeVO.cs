/****************************************************
    文件：paymentTypeVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：paymentType 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class paymentTypeVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.paymentType;
        
        
        /// <summary>
        /// 币种符 
        /// </summary>
        public string payicon;

        /// <summary>
        /// 资源命名id 
        /// </summary>
        public int[] logo_id;

    }
}
        