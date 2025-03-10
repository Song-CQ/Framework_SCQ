/****************************************************
    文件：giftcardVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：giftcard 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class giftcardVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.giftcard;
        
        
        /// <summary>
        /// 礼品卡资源命名 
        /// </summary>
        public string card_id;

        /// <summary>
        /// logo资源命名 
        /// </summary>
        public string logo_id;

    }
}
        