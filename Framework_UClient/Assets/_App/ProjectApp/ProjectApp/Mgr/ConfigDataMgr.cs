///****************************************************
//    文件：ExcelDataMgr.cs
//	作者：Clear
//    日期：2025/3/6 17:55:16
//    类型: 工具自动创建(请勿修改)
//	功能：表格数据管理器
//*****************************************************/
//using System;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using ProjectApp.Data;
//using FutureCore;
//using UnityEngine;

//namespace ProjectApp
//{
//    public enum ConfigVO
//    {
//# ConfigType
//    }

//    public sealed partial class ConfigDataMgr : BaseMgr<ConfigDataMgr>
//    {
//        public abstract class BaseStaticVO
//        {
//            public string key;
//        }
//        private class ConfigData
//        {
//            public Dictionary<ConfigVO, BaseStaticVO> configStaticVODic = new Dictionary<ConfigVO, BaseStaticVO>();
//            public Dictionary<ConfigVO, BaseVO[]> configVODic = new Dictionary<ConfigVO, BaseVO[]>();


//            public void Dispose()
//            {
//                configStaticVODic.Clear();
//                configVODic.Clear();


//                configVODic = null;
//                configStaticVODic = null;

//            }
//        }


//        private Dictionary<ConfigVO, Type> configTypeDic;
//        private Dictionary<ConfigVO, BaseStaticVO> configStaticVODic;
//        private Dictionary<ConfigVO, BaseVO[]> configVODic;
//        private Dictionary<ConfigVO, BaseVOModel> configModelDic;
//        private const bool isEnciphermentData = true;
//        private const bool isOutMultipleDatas = false;
//        protected override void New()
//        {
//            configTypeDic = new Dictionary<ConfigVO, Type>();
//            configStaticVODic = new Dictionary<ConfigVO, BaseStaticVO>();
//            configVODic = new Dictionary<ConfigVO, BaseVO[]>();
//            configModelDic = new Dictionary<ConfigVO, BaseVOModel>();
//        }

//        public override void Init()
//        {
//            base.Init();

//            configTypeDic.Add(ConfigVO.game, typeof(pathStaticVO[]));

//            paymentTypeVOModel.Instance.Init();
//            forceAdVOModel.Instance.Init();
//            giftcardVOModel.Instance.Init();
//            bingoRedeemVOModel.Instance.Init();
//            out_PPVOModel.Instance.Init();
//            out_coinVOModel.Instance.Init();
//            out_cashVOModel.Instance.Init();
//            Flop_rewardVOModel.Instance.Init();
//            pickBoxVOModel.Instance.Init();
//            adRewardVOModel.Instance.Init();
//            rollingVOModel.Instance.Init();
//            slotrewardVOModel.Instance.Init();
//            SignDailyRewardVOModel.Instance.Init();
//            Num_probabilityVOModel.Instance.Init();
//            TaskListVOModel.Instance.Init();
//            giftListVOModel.Instance.Init();
//            propVOModel.Instance.Init();
//            piggyVOModel.Instance.Init();
//            GuidanceVOModel.Instance.Init();
//            langVOModel.Instance.Init();
//            ItemVOModel.Instance.Init();
//            turntableVOModel.Instance.Init();
//        }



//        public void ReadData()
//        {

//            if (isOutMultipleDatas)
//            {
//                foreach (var item in configTypeDic)
//                {
//                    GetExcalData(item.Key);
//                }
//            }
//            else
//            {

//                ConfigData configData = LoadConfigData();
//                foreach (var item in configData.configStaticVODic)
//                {
//                    configStaticVODic.Add(item.Key, item.Value);
//                }
//                foreach (var item in configData.configVODic)
//                {
//                    configVODic.Add(item.Key, item.Value);
//                }
//            }


//            pathStaticVO.SetData(configStaticVODic[ConfigVO.game]);
//            AddVOModel(ConfigVO.game, turntableVOModel.Instance);




//            //SetExcalData<paymentTypeVO>("paymentType");
//            //SetExcalData<forceAdVO>("forceAd");
//            //SetExcalData<giftcardVO>("giftcard");
//            //SetExcalData<bingoRedeemVO>("bingoRedeem");
//            //SetExcalData<out_PPVO>("out_PP");
//            //SetExcalData<out_coinVO>("out_coin");
//            //SetExcalData<out_cashVO>("out_cash");
//            //SetExcalData<Flop_rewardVO>("Flop_reward");
//            //SetExcalData<pickBoxVO>("pickBox");
//            //SetExcalData<adRewardVO>("adReward");
//            //SetExcalData<rollingVO>("rolling");
//            //SetExcalData<slotrewardVO>("slotreward");
//            //SetExcalData<SignDailyRewardVO>("SignDailyReward");
//            //SetExcalData<Num_probabilityVO>("Num_probability");
//            //SetExcalData<TaskListVO>("TaskList");
//            //SetExcalData<giftListVO>("giftList");
//            //SetExcalData<propVO>("prop");
//            //SetExcalData<piggyVO>("piggy");
//            //SetExcalData<GuidanceVO>("Guidance");
//            //SetExcalData<langVO>("lang");
//            //SetExcalData<ItemVO>("Item");
//            //SetExcalData<turntableVO>("turntable");

//            //paymentTypeVOModel.Instance.SetData(excelDataStrDic[typeof(paymentTypeVO)] as paymentTypeVO[]);
//            //forceAdVOModel.Instance.SetData(excelDataStrDic[typeof(forceAdVO)] as forceAdVO[]);
//            //giftcardVOModel.Instance.SetData(excelDataStrDic[typeof(giftcardVO)] as giftcardVO[]);
//            //bingoRedeemVOModel.Instance.SetData(excelDataStrDic[typeof(bingoRedeemVO)] as bingoRedeemVO[]);
//            //out_PPVOModel.Instance.SetData(excelDataStrDic[typeof(out_PPVO)] as out_PPVO[]);
//            //out_coinVOModel.Instance.SetData(excelDataStrDic[typeof(out_coinVO)] as out_coinVO[]);
//            //out_cashVOModel.Instance.SetData(excelDataStrDic[typeof(out_cashVO)] as out_cashVO[]);
//            //Flop_rewardVOModel.Instance.SetData(excelDataStrDic[typeof(Flop_rewardVO)] as Flop_rewardVO[]);
//            //pickBoxVOModel.Instance.SetData(excelDataStrDic[typeof(pickBoxVO)] as pickBoxVO[]);
//            //adRewardVOModel.Instance.SetData(excelDataStrDic[typeof(adRewardVO)] as adRewardVO[]);
//            //rollingVOModel.Instance.SetData(excelDataStrDic[typeof(rollingVO)] as rollingVO[]);
//            //slotrewardVOModel.Instance.SetData(excelDataStrDic[typeof(slotrewardVO)] as slotrewardVO[]);
//            //SignDailyRewardVOModel.Instance.SetData(excelDataStrDic[typeof(SignDailyRewardVO)] as SignDailyRewardVO[]);
//            //Num_probabilityVOModel.Instance.SetData(excelDataStrDic[typeof(Num_probabilityVO)] as Num_probabilityVO[]);
//            //TaskListVOModel.Instance.SetData(excelDataStrDic[typeof(TaskListVO)] as TaskListVO[]);
//            //giftListVOModel.Instance.SetData(excelDataStrDic[typeof(giftListVO)] as giftListVO[]);
//            //propVOModel.Instance.SetData(excelDataStrDic[typeof(propVO)] as propVO[]);
//            //piggyVOModel.Instance.SetData(excelDataStrDic[typeof(piggyVO)] as piggyVO[]);
//            //GuidanceVOModel.Instance.SetData(excelDataStrDic[typeof(GuidanceVO)] as GuidanceVO[]);
//            //langVOModel.Instance.SetData(excelDataStrDic[typeof(langVO)] as langVO[]);
//            //ItemVOModel.Instance.SetData(excelDataStrDic[typeof(ItemVO)] as ItemVO[]);
//            //turntableVOModel.Instance.SetData(excelDataStrDic[typeof(turntableVO)] as turntableVO[]);
//        }



//        public void ResetData()
//        {
//            pathStaticVO.Reset();
//            foreach (var voModel in configModelDic)
//            {
//                voModel.Value.Reset();

//            }
//            configModelDic.Clear();

//            configStaticVODic.Clear();
//            configVODic.Clear();
//        }

//        private void AddVOModel(ConfigVO key, BaseVOModel model)
//        {

//            model.SetData(configVODic[key]);
//            configModelDic.Add(key, model);
//        }
//        private ConfigData LoadConfigData()
//        {
//            TextAsset textAsset = ResMgr.Instance.GetConfigData("ConfigData");
//            ConfigData configData = null;
//            if (textAsset != null)
//            {
//                string val = null;
//                if (isEnciphermentData)
//                {
//                    val = AESEncryptUtil.Decrypt(textAsset.bytes);
//                }
//                else
//                {
//                    val = textAsset.text;
//                }
//                configData = JsonConvert.DeserializeObject<ConfigData>(val);

//            }
//            else
//            {
//                Debug.LogError("未找到ConfigData数据");
//            }
//            return configData;

//        }


//        private void GetExcalData(ConfigVO configVO)
//        {
//            bool isStatic = (int)configVO <= 100;
//            string path = string.Empty;
//            //小与等于100 为静态表
//            if (isStatic)
//            {
//                path = @"StaticExcelData\" + configVO.ToString() + "_Data";
//            }
//            else
//            {
//                path = @"ExcelData\" + configVO.ToString() + "_Data";
//            }

//            TextAsset textAsset = ResMgr.Instance.GetConfigData(path);
//            Type type = configTypeDic[configVO];
//            if (textAsset != null)
//            {
//                string val = null;
//                if (isEnciphermentData)
//                {
//                    val = AESEncryptUtil.Decrypt(textAsset.bytes);
//                }
//                else
//                {
//                    val = textAsset.text;
//                }
//                object vos = JsonConvert.DeserializeObject(val, type);

//                if (isStatic)
//                {
//                    configStaticVODic.Add(configVO, vos as BaseStaticVO);
//                }
//                else
//                {
//                    configVODic.Add(configVO, vos as BaseVO[]);
//                }
//            }
//            else
//            {
//                LogUtil.LogError("未找到表:" + configVO.ToString() + "的数据");
//            }

//        }



//    }
//}