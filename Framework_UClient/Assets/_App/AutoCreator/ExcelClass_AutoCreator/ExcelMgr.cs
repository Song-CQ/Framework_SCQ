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
            
            CommonsStaticVO.SetData(GetStaticExcalData<CommonsStaticVO>("Commons"));
            
            SetExcalData<Sheet8VO>("Sheet8");
            SetExcalData<Sheet_gVO>("Sheet_g");
            SetExcalData<Sheet1VO>("Sheet1");
            
            Sheet8VOModel.Instance.Init();
            Sheet_gVOModel.Instance.Init();
            Sheet1VOModel.Instance.Init();
            
            Sheet8VOModel.Instance.SetData(excelDataStrDic[typeof(Sheet8VO)] as Sheet8VO[]);
            Sheet_gVOModel.Instance.SetData(excelDataStrDic[typeof(Sheet_gVO)] as Sheet_gVO[]);
            Sheet1VOModel.Instance.SetData(excelDataStrDic[typeof(Sheet1VO)] as Sheet1VO[]);
        }
         
        public void SetExcalData<T>(string tableName) where T:BaseVO
        {
            TextAsset textAsset = ResMgr.Instance.GetExcelData(@"ExcelData\"+tableName + "_Data");
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
        
        private T GetStaticExcalData<T>(string tableName) where T : new()
        {
            TextAsset textAsset = ResMgr.Instance.GetExcelData(@"StaticExcelData\"+tableName + "_StaticData");
            T vo = default;
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