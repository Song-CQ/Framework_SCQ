﻿using System;
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
           #SetDataToDic
          
           #Init
          
           #SetDataModel
        }
        
        public void SetExcalData<T>(string tableName) where T:BaseVO
        {
            string val = File.ReadAllText(@"#OutPath"+@"\"+tableName+"_Data.txt");
            T[] vos = JsonConvert.DeserializeObject<T[]>(val);
            excelDataStrDic.Add(typeof(T),vos);
        }

    }
}