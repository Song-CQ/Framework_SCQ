/****************************************************
    文件：ExcelDataMgr.cs
	作者：Clear
    日期：#CreateTime#
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
        private const bool isEnciphermentData = #IsEnciphermentData;
        /// <summary>
        /// 是否将每张表都生成一个数据文件
        /// </summary>
        private const bool isOutMultipleDatas = #IsOutMultipleDatas;
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
            
            #Init           
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
                foreach (var item in configData.configStaticVODic)
                {
                    configStaticVODic.Add((ConfigVO)item.Key, item.Value);
                }
                foreach (var item in configData.configVODic)
                {
                    configVODic.Add((ConfigVO)item.Key, item.Value);
                }
                configData.Dispose();
            }

            #SetStaticDataToDic

            #SetDataModel
        }


        public void ResetData()
        {
            #Reset
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