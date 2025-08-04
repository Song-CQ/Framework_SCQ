/****************************************************
    文件：ExcelDataMgr.cs
	作者：Clear
    日期：2025/6/11 16:2:36
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
                configStaticVODic.Add(ConfigVO.Common, GetExcalData<CommonStaticVO>(ConfigVO.Common,true) as CommonStaticVO);
                configVODic.Add(ConfigVO.EventData, GetExcalData<EventDataVO>(ConfigVO.EventData,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.LevelData, GetExcalData<LevelDataVO>(ConfigVO.LevelData,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.CheckCondition, GetExcalData<CheckConditionVO>(ConfigVO.CheckCondition,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.RoleData, GetExcalData<RoleDataVO>(ConfigVO.RoleData,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.RoleKey, GetExcalData<RoleKeyVO>(ConfigVO.RoleKey,false) as List<BaseVO>);
                configVODic.Add(ConfigVO.RoleLabel, GetExcalData<RoleLabelVO>(ConfigVO.RoleLabel,false) as List<BaseVO>);
            }
            else
            {

                ConfigData configData = LoadConfigData();
                
                configStaticVODic.Add(ConfigVO.path, configData.path);
                configStaticVODic.Add(ConfigVO.Common, configData.Common);
                configVODic.Add(ConfigVO.EventData, configData.EventData_List.OfType<EventDataVO,BaseVO>());
                configVODic.Add(ConfigVO.LevelData, configData.LevelData_List.OfType<LevelDataVO,BaseVO>());
                configVODic.Add(ConfigVO.CheckCondition, configData.CheckCondition_List.OfType<CheckConditionVO,BaseVO>());
                configVODic.Add(ConfigVO.RoleData, configData.RoleData_List.OfType<RoleDataVO,BaseVO>());
                configVODic.Add(ConfigVO.RoleKey, configData.RoleKey_List.OfType<RoleKeyVO,BaseVO>());
                configVODic.Add(ConfigVO.RoleLabel, configData.RoleLabel_List.OfType<RoleLabelVO,BaseVO>());
                
                configData = null;
            }

            
            pathStaticVO.SetData(configStaticVODic[ConfigVO.path] as pathStaticVO);
            CommonStaticVO.SetData(configStaticVODic[ConfigVO.Common] as CommonStaticVO);

            
            AddVOModel(ConfigVO.EventData,EventDataVOModel.Instance);
            AddVOModel(ConfigVO.LevelData,LevelDataVOModel.Instance);
            AddVOModel(ConfigVO.CheckCondition,CheckConditionVOModel.Instance);
            AddVOModel(ConfigVO.RoleData,RoleDataVOModel.Instance);
            AddVOModel(ConfigVO.RoleKey,RoleKeyVOModel.Instance);
            AddVOModel(ConfigVO.RoleLabel,RoleLabelVOModel.Instance);
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