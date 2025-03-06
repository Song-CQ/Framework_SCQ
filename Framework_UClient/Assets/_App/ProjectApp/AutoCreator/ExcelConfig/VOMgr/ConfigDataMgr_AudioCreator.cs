/****************************************************
    文件：ExcelDataMgr.cs
	作者：Clear
    日期：2025/3/6 17:55:16
    类型: 工具自动创建(请勿修改)
	功能：表格数据管理器
*****************************************************/
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ProjectApp.Data;
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public sealed partial class ConfigDataMgr : BaseMgr<ConfigDataMgr>
    {

        private Dictionary<Type, BaseVO[]> excelDataStrDic;
        private const bool isEnciphermentData = true;
        protected override void New()
        {
            excelDataStrDic = new Dictionary<Type,BaseVO[]>();
        }

        public override void Init()
        { 
            base.Init();
            
            
            paymentTypeVOModel.Instance.Init();
            forceAdVOModel.Instance.Init();
            giftcardVOModel.Instance.Init();
            bingoRedeemVOModel.Instance.Init();
            out_PPVOModel.Instance.Init();
            out_coinVOModel.Instance.Init();
            out_cashVOModel.Instance.Init();
            Flop_rewardVOModel.Instance.Init();
            pickBoxVOModel.Instance.Init();
            adRewardVOModel.Instance.Init();
            rollingVOModel.Instance.Init();
            slotrewardVOModel.Instance.Init();
            SignDailyRewardVOModel.Instance.Init();
            Num_probabilityVOModel.Instance.Init();
            TaskListVOModel.Instance.Init();
            giftListVOModel.Instance.Init();
            propVOModel.Instance.Init();
            piggyVOModel.Instance.Init();
            GuidanceVOModel.Instance.Init();
            langVOModel.Instance.Init();
            ItemVOModel.Instance.Init();
            turntableVOModel.Instance.Init();           
        }


        public void ReadData()
        {
            
            pathStaticVO.SetData(GetStaticExcalData<pathStaticVO>("path"));
            CommonStaticVO.SetData(GetStaticExcalData<CommonStaticVO>("Common"));
            
            SetExcalData<paymentTypeVO>("paymentType");
            SetExcalData<forceAdVO>("forceAd");
            SetExcalData<giftcardVO>("giftcard");
            SetExcalData<bingoRedeemVO>("bingoRedeem");
            SetExcalData<out_PPVO>("out_PP");
            SetExcalData<out_coinVO>("out_coin");
            SetExcalData<out_cashVO>("out_cash");
            SetExcalData<Flop_rewardVO>("Flop_reward");
            SetExcalData<pickBoxVO>("pickBox");
            SetExcalData<adRewardVO>("adReward");
            SetExcalData<rollingVO>("rolling");
            SetExcalData<slotrewardVO>("slotreward");
            SetExcalData<SignDailyRewardVO>("SignDailyReward");
            SetExcalData<Num_probabilityVO>("Num_probability");
            SetExcalData<TaskListVO>("TaskList");
            SetExcalData<giftListVO>("giftList");
            SetExcalData<propVO>("prop");
            SetExcalData<piggyVO>("piggy");
            SetExcalData<GuidanceVO>("Guidance");
            SetExcalData<langVO>("lang");
            SetExcalData<ItemVO>("Item");
            SetExcalData<turntableVO>("turntable");
            
            paymentTypeVOModel.Instance.SetData(excelDataStrDic[typeof(paymentTypeVO)] as paymentTypeVO[]);
            forceAdVOModel.Instance.SetData(excelDataStrDic[typeof(forceAdVO)] as forceAdVO[]);
            giftcardVOModel.Instance.SetData(excelDataStrDic[typeof(giftcardVO)] as giftcardVO[]);
            bingoRedeemVOModel.Instance.SetData(excelDataStrDic[typeof(bingoRedeemVO)] as bingoRedeemVO[]);
            out_PPVOModel.Instance.SetData(excelDataStrDic[typeof(out_PPVO)] as out_PPVO[]);
            out_coinVOModel.Instance.SetData(excelDataStrDic[typeof(out_coinVO)] as out_coinVO[]);
            out_cashVOModel.Instance.SetData(excelDataStrDic[typeof(out_cashVO)] as out_cashVO[]);
            Flop_rewardVOModel.Instance.SetData(excelDataStrDic[typeof(Flop_rewardVO)] as Flop_rewardVO[]);
            pickBoxVOModel.Instance.SetData(excelDataStrDic[typeof(pickBoxVO)] as pickBoxVO[]);
            adRewardVOModel.Instance.SetData(excelDataStrDic[typeof(adRewardVO)] as adRewardVO[]);
            rollingVOModel.Instance.SetData(excelDataStrDic[typeof(rollingVO)] as rollingVO[]);
            slotrewardVOModel.Instance.SetData(excelDataStrDic[typeof(slotrewardVO)] as slotrewardVO[]);
            SignDailyRewardVOModel.Instance.SetData(excelDataStrDic[typeof(SignDailyRewardVO)] as SignDailyRewardVO[]);
            Num_probabilityVOModel.Instance.SetData(excelDataStrDic[typeof(Num_probabilityVO)] as Num_probabilityVO[]);
            TaskListVOModel.Instance.SetData(excelDataStrDic[typeof(TaskListVO)] as TaskListVO[]);
            giftListVOModel.Instance.SetData(excelDataStrDic[typeof(giftListVO)] as giftListVO[]);
            propVOModel.Instance.SetData(excelDataStrDic[typeof(propVO)] as propVO[]);
            piggyVOModel.Instance.SetData(excelDataStrDic[typeof(piggyVO)] as piggyVO[]);
            GuidanceVOModel.Instance.SetData(excelDataStrDic[typeof(GuidanceVO)] as GuidanceVO[]);
            langVOModel.Instance.SetData(excelDataStrDic[typeof(langVO)] as langVO[]);
            ItemVOModel.Instance.SetData(excelDataStrDic[typeof(ItemVO)] as ItemVO[]);
            turntableVOModel.Instance.SetData(excelDataStrDic[typeof(turntableVO)] as turntableVO[]);
        }


        public void ResetData()
        {           
            
            paymentTypeVOModel.Instance.Reset();
            forceAdVOModel.Instance.Reset();
            giftcardVOModel.Instance.Reset();
            bingoRedeemVOModel.Instance.Reset();
            out_PPVOModel.Instance.Reset();
            out_coinVOModel.Instance.Reset();
            out_cashVOModel.Instance.Reset();
            Flop_rewardVOModel.Instance.Reset();
            pickBoxVOModel.Instance.Reset();
            adRewardVOModel.Instance.Reset();
            rollingVOModel.Instance.Reset();
            slotrewardVOModel.Instance.Reset();
            SignDailyRewardVOModel.Instance.Reset();
            Num_probabilityVOModel.Instance.Reset();
            TaskListVOModel.Instance.Reset();
            giftListVOModel.Instance.Reset();
            propVOModel.Instance.Reset();
            piggyVOModel.Instance.Reset();
            GuidanceVOModel.Instance.Reset();
            langVOModel.Instance.Reset();
            ItemVOModel.Instance.Reset();
            turntableVOModel.Instance.Reset();
        }


        private void SetExcalData<T>(string tableName) where T:BaseVO
        {
            TextAsset textAsset = ResMgr.Instance.GetExcelData(@"ExcelData\"+tableName + "_Data");
            T[] vos = null;
            if (textAsset!=null)
            {
                string val = null;
                if (isEnciphermentData)
                {
                    val = AESEncryptUtil.Decrypt(textAsset.bytes);
                }
                else
                {
                    val = textAsset.text;
                }
                vos = JsonConvert.DeserializeObject<T[]>(val);
            }
            else
            {
                Debug.LogError("未找到表:"+tableName+"的数据");
            }
            excelDataStrDic.Add(typeof(T),vos);
        }
        
        private T GetStaticExcalData<T>(string tableName) where T : new()
        {
            TextAsset textAsset = ResMgr.Instance.GetExcelData(@"StaticExcelData\"+tableName + "_StaticData");
            T vo = default;
            if (textAsset!=null)
            {
                string val = null;
                if (isEnciphermentData)
                {
                    val = AESEncryptUtil.Decrypt(textAsset.bytes);
                }
                else
                {
                    val = textAsset.text;
                }
                vo = JsonConvert.DeserializeObject<T>(val);
            }else
            {
                Debug.LogError("未找到表:"+tableName+"的数据");
            }
            return vo;
        }

    }
}