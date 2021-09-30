using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ProjectApp.Data;
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    /// <summary>
    /// 表格数据管理器
    /// </summary>
    public class ExcelDataMgr:BaseMgr<ExcelDataMgr>
    {

        private Dictionary<Type, BaseVO[]> excelDataStrDic;
        
        protected override void New()
        {
            excelDataStrDic = new Dictionary<Type,BaseVO[]>();
        }

        public override void Init()
        { 
            base.Init();
            #SetDataToDic
          
            #Init
          
            #SetDataModel
        }
        
        public void SetExcalData<T>(string tableName) where T:BaseVO
        {
            TextAsset textAsset = ResMgr.Instance.GetExcelData(tableName + "_Data");
            T[] vos = null;
            if (textAsset!=null)
            { 
                vos = JsonConvert.DeserializeObject<T[]>(textAsset.text);
            }
            else
            {
                Debug.LogError("未找到表:"+tableName+"的数据");
            }

            excelDataStrDic.Add(typeof(T),vos);
        }
        
        private T GetStaticExcalData<T>(string tableName) where T:BaseVO
        {
            TextAsset textAsset = ResMgr.Instance.GetExcelData(tableName + "_Data");
            T vo = null;
            if (textAsset!=null)
            {
                vo=JsonConvert.DeserializeObject<T>(textAsset.text);
            }else
            {
                Debug.LogError("未找到表:"+tableName+"的数据");
            }
            return vo;
        }

    }
}