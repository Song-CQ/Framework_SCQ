/****************************************************
    文件：ConfigVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：表类型 枚举类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public enum ConfigVO
    {
        path = 1,
Common = 2,
        paymentType = 101,
        forceAd = 102,
        giftcard = 103,
        bingoRedeem = 104,
        out_PP = 105,
        out_coin = 106,
        out_cash = 107,
        Flop_reward = 108,
        Sheet1 = 109,
        pickBox = 110,
        adReward = 111,
        rolling = 112,
        slotreward = 113,
        SignDailyReward = 114,
        Num_probability = 115,
        TaskList = 116,
        giftList = 117,
        prop = 118,
        piggy = 119,
        Guidance = 120,
        lang = 121,
        Item = 122,
        turntable = 123,
    }

    public class ConfigData
    {
        
            public pathStaticVO path = null;
            public CommonStaticVO Common = null;
            public List<paymentTypeVO> paymentType_List = null;
            public List<forceAdVO> forceAd_List = null;
            public List<giftcardVO> giftcard_List = null;
            public List<bingoRedeemVO> bingoRedeem_List = null;
            public List<out_PPVO> out_PP_List = null;
            public List<out_coinVO> out_coin_List = null;
            public List<out_cashVO> out_cash_List = null;
            public List<Flop_rewardVO> Flop_reward_List = null;
            public List<Sheet1VO> Sheet1_List = null;
            public List<pickBoxVO> pickBox_List = null;
            public List<adRewardVO> adReward_List = null;
            public List<rollingVO> rolling_List = null;
            public List<slotrewardVO> slotreward_List = null;
            public List<SignDailyRewardVO> SignDailyReward_List = null;
            public List<Num_probabilityVO> Num_probability_List = null;
            public List<TaskListVO> TaskList_List = null;
            public List<giftListVO> giftList_List = null;
            public List<propVO> prop_List = null;
            public List<piggyVO> piggy_List = null;
            public List<GuidanceVO> Guidance_List = null;
            public List<langVO> lang_List = null;
            public List<ItemVO> Item_List = null;
            public List<turntableVO> turntable_List = null;
    }
}
        