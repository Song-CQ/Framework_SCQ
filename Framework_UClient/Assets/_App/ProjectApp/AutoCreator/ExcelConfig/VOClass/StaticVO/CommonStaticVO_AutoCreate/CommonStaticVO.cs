/****************************************************
    文件：CommonStaticVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：Common-应用常量 静态表
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public class CommonStaticVO : BaseStaticVO
    {
        private static CommonStaticVO m_instance;
        public static CommonStaticVO Instance
        {
            get
            {

                return m_instance;
            }
        }

        public const ConfigVO VOType = ConfigVO.Common;

        public override string Key { get { return VOType.ToString(); } }



        public static void SetData(CommonStaticVO val)
        {
            m_instance = val;
        }
        public static void ResetData()
        {
            m_instance = null;
        }

        

        /// <summary>
        /// id = 1
        /// 1卡桌、2卡桌、4卡桌激活道具所需步数
        /// [2,3,4]
        /// </summary>
        public int[] Activateprop = new int[]{2,3,4};

        /// <summary>
        /// id = 2
        /// 叫号CD与数量上限
        /// [5,27]
        /// </summary>
        public int[] NumCD = new int[]{5,27};

        /// <summary>
        /// id = 3
        /// 单张卡桌初始道具生成数量[金币上下限,钞票上下限]
        /// [4,5,2,3]
        /// </summary>
        public int[] Initialprop = new int[]{4,5,2,3};

        /// <summary>
        /// id = 4
        /// 初始游戏卡个数
        /// [20]
        /// </summary>
        public int[] Fisrtcard = new int[]{20};

        /// <summary>
        /// id = 5
        /// 转盘CD
        /// 600
        /// </summary>
        public int TurntableCD = 600;

        /// <summary>
        /// id = 6
        /// 最大恢复卡牌数量
        /// [20]
        /// </summary>
        public int[] MaxCardBoardSum = new int[]{20};

        /// <summary>
        /// id = 7
        /// 恢复一张卡板需要的时间
        /// 600
        /// </summary>
        public int CardBoardRecoveryTime = 600;

        /// <summary>
        /// id = 8
        /// 增加叫号数的最大次数
        /// 1
        /// </summary>
        public int MaxAddCallSum = 1;

        /// <summary>
        /// id = 9
        /// 初始金币数量
        /// 0
        /// </summary>
        public int InitCoinNum = 0;

        /// <summary>
        /// id = 10
        /// 初始钞票数量
        /// 0
        /// </summary>
        public int InitCashNum = 0;

        /// <summary>
        /// id = 11
        /// 整体卡桌初始纸牌生成总数量
        /// 3
        /// </summary>
        public int Initialcardprop = 3;

        /// <summary>
        /// id = 12
        /// 订单审核排队人数起始范围
        /// [3000,5000]
        /// </summary>
        public int[] ReviewCount = new int[]{3000,5000};

        /// <summary>
        /// id = 13
        /// 订单审核每看一个广告排队人数减少
        /// [5,10]
        /// </summary>
        public int[] ReviewCountLess = new int[]{5,10};

        /// <summary>
        /// id = 14
        /// 订单审核排队时间起始范围
        /// [720000,1080000]
        /// </summary>
        public int[] ReviewTime = new int[]{720000,1080000};

        /// <summary>
        /// id = 15
        /// 订单审核每看一个广告排队时间减少
        /// [1800,3600]
        /// </summary>
        public int[] ReviewTimeLess = new int[]{1800,3600};

        /// <summary>
        /// id = 16
        /// 兑换后下一档位弹板出现激活的时间
        /// 60
        /// </summary>
        public int RedeemActiveTime = 60;

        /// <summary>
        /// id = 17
        /// CashOut图标变化时间
        /// 3
        /// </summary>
        public int CashoutIconTime = 3;

        /// <summary>
        /// id = 18
        /// 飞行气球奖励类型
        /// [102,103]
        /// </summary>
        public int[] ballonRewardType = new int[]{102,103};

        /// <summary>
        /// id = 19
        /// 飞行气球的钞票奖励
        /// [50,40,30,20,5]
        /// </summary>
        public int[] ballonRewardCash = new int[]{50,40,30,20,5};

        /// <summary>
        /// id = 20
        /// 飞行气球的A卡奖励
        /// [2,1,1,1,1]
        /// </summary>
        public int[] ballonRewardAmazon = new int[]{2,1,1,1,1};

        /// <summary>
        /// id = 21
        /// 飞行气球漂浮停留时间
        /// [5,10]
        /// </summary>
        public int[] ballonTime = new int[]{5,10};

        /// <summary>
        /// id = 22
        /// 飞行气球看完视频的CD
        /// 60
        /// </summary>
        public int ballonCD = 60;

        /// <summary>
        /// id = 23
        /// Gm秘钥
        /// 9527
        /// </summary>
        public string GmKey = "9527";

        /// <summary>
        /// id = 24
        /// 动态计算的类型[0(累计),1(当前)]
        /// 0
        /// </summary>
        public int DynamicType = 0;

        /// <summary>
        /// id = 25
        /// 第三次手表签到要看视频的数量
        /// 10
        /// </summary>
        public int IWatchDailyWatVideo = 10;
        
    }
}