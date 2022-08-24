/****************************************************
    文件：out_PPVO.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：D_动态A卡发放表_A 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class out_PPVO:BaseVO
    {
        
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
        