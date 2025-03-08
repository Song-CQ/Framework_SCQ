/****************************************************
    文件：ExcelDataMgr.cs
	作者：Clear
    日期：2025/3/9 2:21:8
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
        private Dictionary<ConfigVO, BaseVO[]> configVODic;
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
            configVODic = new Dictionary<ConfigVO, BaseVO[]>();
            configModelDic = new Dictionary<ConfigVO, BaseVOModel>();
        }

        public override void Init()
        { 
            base.Init();
            
            
            configTypeDic.Add(ConfigVO.path,typeof(pathStaticVO));
            configTypeDic.Add(ConfigVO.Common,typeof(CommonStaticVO));
            configTypeDic.Add(ConfigVO.paymentType,typeof(paymentTypeVO[]));
            configTypeDic.Add(ConfigVO.forceAd,typeof(forceAdVO[]));
            configTypeDic.Add(ConfigVO.giftcard,typeof(giftcardVO[]));
            configTypeDic.Add(ConfigVO.bingoRedeem,typeof(bingoRedeemVO[]));
            configTypeDic.Add(ConfigVO.out_PP,typeof(out_PPVO[]));
            configTypeDic.Add(ConfigVO.out_coin,typeof(out_coinVO[]));
            configTypeDic.Add(ConfigVO.out_cash,typeof(out_cashVO[]));
            configTypeDic.Add(ConfigVO.Flop_reward,typeof(Flop_rewardVO[]));
            configTypeDic.Add(ConfigVO.pickBox,typeof(pickBoxVO[]));
            configTypeDic.Add(ConfigVO.adReward,typeof(adRewardVO[]));
            configTypeDic.Add(ConfigVO.rolling,typeof(rollingVO[]));
            configTypeDic.Add(ConfigVO.slotreward,typeof(slotrewardVO[]));
            configTypeDic.Add(ConfigVO.SignDailyReward,typeof(SignDailyRewardVO[]));
            configTypeDic.Add(ConfigVO.Num_probability,typeof(Num_probabilityVO[]));
            configTypeDic.Add(ConfigVO.TaskList,typeof(TaskListVO[]));
            configTypeDic.Add(ConfigVO.giftList,typeof(giftListVO[]));
            configTypeDic.Add(ConfigVO.prop,typeof(propVO[]));
            configTypeDic.Add(ConfigVO.piggy,typeof(piggyVO[]));
            configTypeDic.Add(ConfigVO.Guidance,typeof(GuidanceVO[]));
            configTypeDic.Add(ConfigVO.lang,typeof(langVO[]));
            configTypeDic.Add(ConfigVO.Item,typeof(ItemVO[]));
            configTypeDic.Add(ConfigVO.turntable,typeof(turntableVO[]));           
        }


        public void ReadData()
        {
            if (isOutMultipleDatas)
            {
                foreach (var item in configTypeDic)
                {
                    GetExcalData(item.Key);
                }
            }
            else
            {

                ConfigData configData = LoadConfigData();
                
                configStaticVODic.Add(ConfigVO.path, configData.path);
                configStaticVODic.Add(ConfigVO.Common, configData.Common);
                configVODic.Add(ConfigVO.paymentType, configData.paymentType_List);
                configVODic.Add(ConfigVO.forceAd, configData.forceAd_List);
                configVODic.Add(ConfigVO.giftcard, configData.giftcard_List);
                configVODic.Add(ConfigVO.bingoRedeem, configData.bingoRedeem_List);
                configVODic.Add(ConfigVO.out_PP, configData.out_PP_List);
                configVODic.Add(ConfigVO.out_coin, configData.out_coin_List);
                configVODic.Add(ConfigVO.out_cash, configData.out_cash_List);
                configVODic.Add(ConfigVO.Flop_reward, configData.Flop_reward_List);
                configVODic.Add(ConfigVO.pickBox, configData.pickBox_List);
                configVODic.Add(ConfigVO.adReward, configData.adReward_List);
                configVODic.Add(ConfigVO.rolling, configData.rolling_List);
                configVODic.Add(ConfigVO.slotreward, configData.slotreward_List);
                configVODic.Add(ConfigVO.SignDailyReward, configData.SignDailyReward_List);
                configVODic.Add(ConfigVO.Num_probability, configData.Num_probability_List);
                configVODic.Add(ConfigVO.TaskList, configData.TaskList_List);
                configVODic.Add(ConfigVO.giftList, configData.giftList_List);
                configVODic.Add(ConfigVO.prop, configData.prop_List);
                configVODic.Add(ConfigVO.piggy, configData.piggy_List);
                configVODic.Add(ConfigVO.Guidance, configData.Guidance_List);
                configVODic.Add(ConfigVO.lang, configData.lang_List);
                configVODic.Add(ConfigVO.Item, configData.Item_List);
                configVODic.Add(ConfigVO.turntable, configData.turntable_List);
                
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




        private void GetExcalData(ConfigVO configVO)
        {
            bool isStatic = (int)configVO <= 100;
            string path = string.Empty;
            //小与等于100 为静态表
            if (isStatic)
            {
                path = @"StaticExcelData\" + configVO.ToString() + "_Data";
            }
            else
            {
                path = @"ExcelData\" + configVO.ToString() + "_Data";
            }

            TextAsset textAsset = ResMgr.Instance.GetConfigData(path);
            Type type = configTypeDic[configVO];
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
                    configStaticVODic.Add(configVO, vos as BaseStaticVO);
                }
                else
                {
                    configVODic.Add(configVO, vos as BaseVO[]);
                }
            }
            else
            {
                LogUtil.LogError("未找到表:" + configVO.ToString() + "的数据");
            }

        }

    }
}