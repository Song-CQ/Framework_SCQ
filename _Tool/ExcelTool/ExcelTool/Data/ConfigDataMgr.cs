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
        private Dictionary<ConfigVO, List<BaseVO>> configVODic;
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
                #IsOutMultiple_LoadAllData
            }
            else
            {

                ConfigData configData = LoadConfigData();
                #LoadAllData
                
                configData = null;
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