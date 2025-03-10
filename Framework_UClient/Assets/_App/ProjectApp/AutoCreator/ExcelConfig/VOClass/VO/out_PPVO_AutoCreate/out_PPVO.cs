/****************************************************
    文件：out_PPVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：out_PP 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class out_PPVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.out_PP;
        
        
        /// <summary>
        /// 累计获得A卡数量 
        /// </summary>
        public int[] totalPP;

        /// <summary>
        /// 是否视频 
        /// </summary>
        public bool isVideo;

    }
}
        