/****************************************************
    文件：ExcelDataMgr.cs
	作者：Clear
    日期：2026/3/22 14:31:35
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
        private const bool isEnciphermentData = true;
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
                configStaticVODic.Add(ConfigVO.General, GetExcalData<GeneralStaticVO>(ConfigVO.General,true) as GeneralStaticVO);
                configVODic.Add(ConfigVO.Level, GetExcalData<LevelVO>(ConfigVO.Level,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.prop, GetExcalData<propVO>(ConfigVO.prop,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.Quest, GetExcalData<QuestVO>(ConfigVO.Quest,false) as List<BaseVO>);
            }
            else
            {

                ConfigData configData = LoadConfigData();

                configStaticVODic.Add(ConfigVO.path, configData.path);
                configStaticVODic.Add(ConfigVO.General, configData.General);
                configVODic.Add(ConfigVO.Level, configData.Level_List.OfType<LevelVO,BaseVO>());
                configVODic.Add(ConfigVO.prop, configData.prop_List.OfType<propVO,BaseVO>());
                configVODic.Add(ConfigVO.Quest, configData.Quest_List.OfType<QuestVO,BaseVO>());

                configData = null;
            }


            pathStaticVO.SetData(configStaticVODic[ConfigVO.path] as pathStaticVO);
            GeneralStaticVO.SetData(configStaticVODic[ConfigVO.General] as GeneralStaticVO);


            AddVOModel(ConfigVO.Level,LevelVOModel.Instance);
            AddVOModel(ConfigVO.prop,propVOModel.Instance);
            AddVOModel(ConfigVO.Quest,QuestVOModel.Instance);
        }


        public void ResetData()
        {

            pathStaticVO.ResetData();
            GeneralStaticVO.ResetData();
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
            object textAsset = ResMgr.Instance.GetConfigData("ConfigData", isEnciphermentData);
            ConfigData configData = null;
            if (textAsset != null)
            {
                string val = null;
                if (isEnciphermentData)
                {
                    val = AESEncryptUtil.Decrypt(textAsset as byte[]);
                }
                else
                {
                    val = textAsset as string;
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

            object textAsset = ResMgr.Instance.GetConfigData(path, isEnciphermentData);

            if (textAsset != null)
            {
                string val = null;
                if (isEnciphermentData)
                {
                    val = AESEncryptUtil.Decrypt(textAsset as byte[]);
                }
                else
                {
                    val = textAsset as string;
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