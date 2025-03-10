/****************************************************
    文件：ExcelDataMgr.cs
	作者：Clear
    日期：2025/3/11 0:24:33
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
        private Dictionary<ConfigVO, Type> configTypeDic;
        private Dictionary<ConfigVO, BaseStaticVO> configStaticVODic;
        private Dictionary<ConfigVO, List<BaseVO>> configVODic;
        private Dictionary<ConfigVO, BaseVOModel> configModelDic;
        /// <summary>
        /// 是否加密表数据
        /// </summary>
        private const bool isEnciphermentData = false;
        /// <summary>
        /// 是否将每张表都生成一个数据文件
        /// </summary>
        private const bool isOutMultipleDatas = false;
        protected override void New()
        {
            configTypeDic = new Dictionary<ConfigVO, Type>();
            configStaticVODic = new Dictionary<ConfigVO, BaseStaticVO>();
            configVODic = new Dictionary<ConfigVO, List<BaseVO>>();
            configModelDic = new Dictionary<ConfigVO, BaseVOModel>();
        }

        public override void Init()
        { 
            base.Init();
          
        }


        public void ReadData()
        {
            if (isOutMultipleDatas)
            {
                
                configStaticVODic.Add(ConfigVO.path, GetExcalData<pathStaticVO>(ConfigVO.path,true) as pathStaticVO);
                configStaticVODic.Add(ConfigVO.Common, GetExcalData<CommonStaticVO>(ConfigVO.Common,true) as CommonStaticVO);
                configVODic.Add(ConfigVO.paymentType, GetExcalData<paymentTypeVO>(ConfigVO.paymentType,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.forceAd, GetExcalData<forceAdVO>(ConfigVO.forceAd,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.giftcard, GetExcalData<giftcardVO>(ConfigVO.giftcard,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.bingoRedeem, GetExcalData<bingoRedeemVO>(ConfigVO.bingoRedeem,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.out_PP, GetExcalData<out_PPVO>(ConfigVO.out_PP,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.out_coin, GetExcalData<out_coinVO>(ConfigVO.out_coin,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.out_cash, GetExcalData<out_cashVO>(ConfigVO.out_cash,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.Flop_reward, GetExcalData<Flop_rewardVO>(ConfigVO.Flop_reward,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.pickBox, GetExcalData<pickBoxVO>(ConfigVO.pickBox,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.adReward, GetExcalData<adRewardVO>(ConfigVO.adReward,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.rolling, GetExcalData<rollingVO>(ConfigVO.rolling,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.slotreward, GetExcalData<slotrewardVO>(ConfigVO.slotreward,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.SignDailyReward, GetExcalData<SignDailyRewardVO>(ConfigVO.SignDailyReward,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.Num_probability, GetExcalData<Num_probabilityVO>(ConfigVO.Num_probability,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.TaskList, GetExcalData<TaskListVO>(ConfigVO.TaskList,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.giftList, GetExcalData<giftListVO>(ConfigVO.giftList,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.prop, GetExcalData<propVO>(ConfigVO.prop,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.piggy, GetExcalData<piggyVO>(ConfigVO.piggy,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.Guidance, GetExcalData<GuidanceVO>(ConfigVO.Guidance,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.lang, GetExcalData<langVO>(ConfigVO.lang,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.Item, GetExcalData<ItemVO>(ConfigVO.Item,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.turntable, GetExcalData<turntableVO>(ConfigVO.turntable,false) as List<BaseVO>);
            }
            else
            {

                ConfigData configData = LoadConfigData();
                
                
                configStaticVODic.Add(ConfigVO.path, configData.path);
                configStaticVODic.Add(ConfigVO.Common, configData.Common);
                configVODic.Add(ConfigVO.paymentType, configData.paymentType_List.OfType<paymentTypeVO,BaseVO>());
                configVODic.Add(ConfigVO.forceAd, configData.forceAd_List.OfType<forceAdVO,BaseVO>());
                configVODic.Add(ConfigVO.giftcard, configData.giftcard_List.OfType<giftcardVO,BaseVO>());
                configVODic.Add(ConfigVO.bingoRedeem, configData.bingoRedeem_List.OfType<bingoRedeemVO,BaseVO>());
                configVODic.Add(ConfigVO.out_PP, configData.out_PP_List.OfType<out_PPVO,BaseVO>());
                configVODic.Add(ConfigVO.out_coin, configData.out_coin_List.OfType<out_coinVO,BaseVO>());
                configVODic.Add(ConfigVO.out_cash, configData.out_cash_List.OfType<out_cashVO,BaseVO>());
                configVODic.Add(ConfigVO.Flop_reward, configData.Flop_reward_List.OfType<Flop_rewardVO,BaseVO>());
                configVODic.Add(ConfigVO.pickBox, configData.pickBox_List.OfType<pickBoxVO,BaseVO>());
                configVODic.Add(ConfigVO.adReward, configData.adReward_List.OfType<adRewardVO,BaseVO>());
                configVODic.Add(ConfigVO.rolling, configData.rolling_List.OfType<rollingVO,BaseVO>());
                configVODic.Add(ConfigVO.slotreward, configData.slotreward_List.OfType<slotrewardVO,BaseVO>());
                configVODic.Add(ConfigVO.SignDailyReward, configData.SignDailyReward_List.OfType<SignDailyRewardVO,BaseVO>());
                configVODic.Add(ConfigVO.Num_probability, configData.Num_probability_List.OfType<Num_probabilityVO,BaseVO>());
                configVODic.Add(ConfigVO.TaskList, configData.TaskList_List.OfType<TaskListVO,BaseVO>());
                configVODic.Add(ConfigVO.giftList, configData.giftList_List.OfType<giftListVO,BaseVO>());
                configVODic.Add(ConfigVO.prop, configData.prop_List.OfType<propVO,BaseVO>());
                configVODic.Add(ConfigVO.piggy, configData.piggy_List.OfType<piggyVO,BaseVO>());
                configVODic.Add(ConfigVO.Guidance, configData.Guidance_List.OfType<GuidanceVO,BaseVO>());
                configVODic.Add(ConfigVO.lang, configData.lang_List.OfType<langVO,BaseVO>());
                configVODic.Add(ConfigVO.Item, configData.Item_List.OfType<ItemVO,BaseVO>());
                configVODic.Add(ConfigVO.turntable, configData.turntable_List.OfType<turntableVO,BaseVO>());
                
                configData = null;
            }

            
            pathStaticVO.SetData(configStaticVODic[ConfigVO.path] as pathStaticVO);
            CommonStaticVO.SetData(configStaticVODic[ConfigVO.Common] as CommonStaticVO);

            
            AddVOModel(ConfigVO.paymentType,paymentTypeVOModel.Instance);
            AddVOModel(ConfigVO.forceAd,forceAdVOModel.Instance);
            AddVOModel(ConfigVO.giftcard,giftcardVOModel.Instance);
            AddVOModel(ConfigVO.bingoRedeem,bingoRedeemVOModel.Instance);
            AddVOModel(ConfigVO.out_PP,out_PPVOModel.Instance);
            AddVOModel(ConfigVO.out_coin,out_coinVOModel.Instance);
            AddVOModel(ConfigVO.out_cash,out_cashVOModel.Instance);
            AddVOModel(ConfigVO.Flop_reward,Flop_rewardVOModel.Instance);
            AddVOModel(ConfigVO.pickBox,pickBoxVOModel.Instance);
            AddVOModel(ConfigVO.adReward,adRewardVOModel.Instance);
            AddVOModel(ConfigVO.rolling,rollingVOModel.Instance);
            AddVOModel(ConfigVO.slotreward,slotrewardVOModel.Instance);
            AddVOModel(ConfigVO.SignDailyReward,SignDailyRewardVOModel.Instance);
            AddVOModel(ConfigVO.Num_probability,Num_probabilityVOModel.Instance);
            AddVOModel(ConfigVO.TaskList,TaskListVOModel.Instance);
            AddVOModel(ConfigVO.giftList,giftListVOModel.Instance);
            AddVOModel(ConfigVO.prop,propVOModel.Instance);
            AddVOModel(ConfigVO.piggy,piggyVOModel.Instance);
            AddVOModel(ConfigVO.Guidance,GuidanceVOModel.Instance);
            AddVOModel(ConfigVO.lang,langVOModel.Instance);
            AddVOModel(ConfigVO.Item,ItemVOModel.Instance);
            AddVOModel(ConfigVO.turntable,turntableVOModel.Instance);
        }


        public void ResetData()
        {
            
            pathStaticVO.ResetData();
            CommonStaticVO.ResetData();
            foreach (var voModel in configModelDic)
            {
                voModel.Value.Reset();

            }
            configModelDic.Clear();

            configStaticVODic.Clear();
            configVODic.Clear();
        }

        private void AddVOModel(ConfigVO key, BaseVOModel model)
        {
            model.SetData(configVODic[key]);
            configModelDic.Add(key, model);
        }
        private ConfigData LoadConfigData()
        {
            TextAsset textAsset = ResMgr.Instance.GetConfigData("ConfigData");
            ConfigData configData = null;
            if (textAsset != null)
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
                configData = JsonConvert.DeserializeObject<ConfigData>(val);

            }
            else
            {
                Debug.LogError("未找到ConfigData数据");
            }
            return configData;

        }




        private object GetExcalData<T>(ConfigVO configVO, bool isStatic)
        {
            Type type = null;
            string path = string.Empty;
            //小与等于100 为静态表
            if (isStatic)
            {
                path = @"StaticExcelData\" + configVO.ToString() + "_StaticData";
                type = typeof(T);
            }
            else
            {
                path = @"ExcelData\" + configVO.ToString() + "_Data";
                type = typeof(List<T>);
            }

            TextAsset textAsset = ResMgr.Instance.GetConfigData(path);

            if (textAsset != null)
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
                object vos = JsonConvert.DeserializeObject(val, type);

                if (isStatic)
                {
                    return vos;
                }
                else
                {
                    return (vos as List<T>).OfType<T, BaseVO>();
                }
            }
            else
            {
                LogUtil.LogError("未找到表:" + configVO.ToString() + "的数据");
                return null;
            }

        }

    }
}