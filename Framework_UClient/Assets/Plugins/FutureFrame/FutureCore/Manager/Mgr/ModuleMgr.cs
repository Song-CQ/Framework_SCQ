/****************************************************
    文件：NewScript.cs
	作者：Clear
    日期：2022/1/9 14:26:35
	功能：Nothing
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public class ModuleMgr : BaseMgr<ModuleMgr>
    {
        private Dictionary<string, BaseModel> modelDict = new Dictionary<string, BaseModel>();
        private Dictionary<string, Type> uiTypeDict = new Dictionary<string, Type>();
        private Dictionary<string, BaseCtrl> ctrlDict = new Dictionary<string, BaseCtrl>();
        private Dictionary<string, BaseUICtrl> uiCtrlDict = new Dictionary<string, BaseUICtrl>();



        public void StartUpAllModule()
        {
           // throw new NotImplementedException();
        }
    }
}