using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ProjectApp.Data;
using XL.Common;
using System.IO;

namespace ProjectApp
{
    /// <summary>
    /// 表格数据管理器
    /// </summary>
    public class ExcelDataMgr:MonoSingleton<ExcelDataMgr>
    {

        private Dictionary<Type, BaseVO[]> excelDataStrDic;
        
        protected override void New()
        {
            excelDataStrDic = new Dictionary<Type,BaseVO[]>();
        }

        public override void Init()
        { 
           
            SetExcalData<Sheet_gVO>("Sheet_g");
            SetExcalData<Sheet1VO>("Sheet1");
          
           
            Sheet_gVOModel.Instance.Init();
            Sheet1VOModel.Instance.Init();
          
           
            Sheet_gVOModel.Instance.SetData(excelDataStrDic[typeof(Sheet_gVO)] as Sheet_gVO[]);
            Sheet1VOModel.Instance.SetData(excelDataStrDic[typeof(Sheet1VO)] as Sheet1VO[]);
        }
        
        public void SetExcalData<T>(string tableName) where T:BaseVO
        {
            string val = File.ReadAllText(@"E:\UnityPro\Framework_SCQ\Framework_UClient\Assets\_Res\Resources\Data\ExcelConfig"+@"\"+tableName+"_Data.txt");
            T[] vos = JsonConvert.DeserializeObject<T[]>(val);
            excelDataStrDic.Add(typeof(T),vos);
        }

    }
}