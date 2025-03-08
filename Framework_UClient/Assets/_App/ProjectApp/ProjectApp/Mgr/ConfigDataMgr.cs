/****************************************************
    文件：ExcelDataMgr.cs
	作者：Clear
    日期：2025/3/6 17:55:16
    类型: 对表数据进行补充
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
    public sealed partial class ConfigDataMgr
    {


        public VO GetConfigVO<VO>(ConfigVO type, int key) where VO : BaseVO
        {
            if (configModelDic.ContainsKey(type))
            {
                return configModelDic[type].GetBaseVO(key) as VO;
            }
           
            LogUtil.LogError("表类型:"+ type+"不存在!");
            return null;
            


        }

        public VO GetConfigVO<VO>(ConfigVO type, string key) where VO : BaseVO
        {
            if (configModelDic.ContainsKey(type))
            {
                return configModelDic[type].GetBaseVO(key) as VO;
            }

            LogUtil.LogError("表类型:" + type + "不存在!");
            return null;
        }

        public VO GetLastConfigVO<VO>(ConfigVO type) where VO : BaseVO
        {
            if (configModelDic.ContainsKey(type))
            {
                return configModelDic[type].GetLastBaseVO() as VO;
            }

            LogUtil.LogError("表类型:" + type + "不存在!");
            return null;
        }

        public VO GetFirstBaseVO<VO>(ConfigVO type) where VO : BaseVO
        {
            if (configModelDic.ContainsKey(type))
            {
                return configModelDic[type].GetFirstBaseVO() as VO;
            }

            LogUtil.LogError("表类型:" + type + "不存在!");
            return null;
        }


        public VO GetStaticConfigVO<VO>(ConfigVO type) where VO : BaseStaticVO
        {
            if (configStaticVODic.ContainsKey(type))
            {
                return configStaticVODic[type] as VO;
            }

            LogUtil.LogError("静态表类型:" + type + "不存在!");
            return null;
        }




    }
}