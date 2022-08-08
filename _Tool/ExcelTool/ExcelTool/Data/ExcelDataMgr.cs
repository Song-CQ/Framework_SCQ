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
    public sealed class ExcelDataMgr:BaseMgr<ExcelDataMgr>
    {

        private Dictionary<Type, BaseVO[]> excelDataStrDic;
        private const bool isEnciphermentData = #IsEnciphermentData;
        protected override void New()
        {
            excelDataStrDic = new Dictionary<Type,BaseVO[]>();
        }

        public override void Init()
        { 
            base.Init();
            #SetStaticDataToDic
            #SetDataToDic
            #Init
            #SetDataModel
        }
         
        public void SetExcalData<T>(string tableName) where T:BaseVO
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