/****************************************************
    文件：bingoRedeemVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：bingoRedeem 数据类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class bingoRedeemVO:BaseVO
    {
        public const ConfigVO VOType = ConfigVO.bingoRedeem;
        
        
        /// <summary>
        /// 货币id 
        /// </summary>
        public int item;

        /// <summary>
        /// 提现目标面额 
        /// </summary>
        public int redeem_num;

        /// <summary>
        /// 需要货币数量 
        /// </summary>
        public int item_need;

        /// <summary>
        /// 兑换视频与审核视频次数 
        /// </summary>
        public int[] ad_redeem;

        /// <summary>
        /// 审核视频倒计时 
        /// </summary>
        public int ad_redeem_time;

        /// <summary>
        /// 礼品卡资源命名 
        /// </summary>
        public string card_id;

        /// <summary>
        /// logo资源命名 
        /// </summary>
        public string logo_id;

        /// <summary>
        /// 印尼目标面额 
        /// </summary>
        public int redeem_ID;

    }
}
        