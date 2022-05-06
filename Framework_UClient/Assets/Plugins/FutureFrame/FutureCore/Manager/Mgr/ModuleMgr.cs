/****************************************************
    文件：ModuleMgr.cs
	作者：Clear
    日期：2022/1/9 14:26:35
	功能：模块管理器
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public sealed class ModuleMgr : BaseMgr<ModuleMgr>
    {
        private Dictionary<string, BaseModel> modelDict = new Dictionary<string, BaseModel>();
        private Dictionary<string, Type> uiTypeDict = new Dictionary<string, Type>();
        private Dictionary<string, BaseCtrl> ctrlDict = new Dictionary<string, BaseCtrl>();
        private Dictionary<string, BaseUICtrl> uiCtrlDict = new Dictionary<string, BaseUICtrl>();


        public override void Init()
        {
            base.Init();
            InitAllModule();
        }

        private void InitAllModule()
        {
            //不激活的控制器列表
            List<string> ctrlDisableList = AppConst.CtrlDisableList;
            ///New
            foreach (BaseModel model in modelDict.Values)
            {
                model.New();
            }
            foreach (BaseCtrl ctrl in ctrlDict.Values)
            {
                if (!ctrl.isEnable)
                {
                    continue;
                }
                if (ctrlDisableList.Contains(ctrl.ctrlName))
                {
                    ctrl.isEnable = false;
                    continue;
                }
                ctrl.isEnable = true;
                ctrl.New();
            }
            foreach (BaseUICtrl uiCtrl in uiCtrlDict.Values)
            {
                if (!uiCtrl.isEnable)
                {
                    continue;
                }
                if (ctrlDisableList.Contains(uiCtrl.ctrlName))
                {
                    uiCtrl.isEnable = false;
                    continue;
                }
                uiCtrl.isEnable = true;
                uiCtrl.New();
            }
            // Init
            foreach (BaseModel model in modelDict.Values)
            {
                model.Init();
            }
            foreach (BaseCtrl ctrl in ctrlDict.Values)
            {
                ctrl.Init();
            }
            foreach (BaseUICtrl uiCtrl in uiCtrlDict.Values)
            {
                uiCtrl.Init();
            }
            LogUtil.Log("[ModuleMgr]InitModule".AddColor(ColorType.Green));

        }

        public void StartUpAllModule()
        {

            foreach (BaseModel model in modelDict.Values)
            {
                model.StartUp();
            }
            foreach (BaseCtrl ctrl in ctrlDict.Values)
            {
                ctrl.StartUp();
            }
            foreach (BaseUICtrl uiCtrl in uiCtrlDict.Values)
            {
                uiCtrl.StartUp();
            }
            LogUtil.Log("[ModuleMgr]StartUpAllModule");

        }
        public void AllModuleReadData()
        {
            foreach (BaseModel model in modelDict.Values)
            {
                model.ReadData();
            }
            foreach (BaseCtrl ctrl in ctrlDict.Values)
            {
                ctrl.ReadData();
            }
            foreach (BaseUICtrl uiCtrl in uiCtrlDict.Values)
            {
                uiCtrl.ReadData();
            }
            LogUtil.Log("[ModuleMgr]AllModuleReadData");
        }

        public void AllModuleGameStart()
        {
            foreach (BaseModel model in modelDict.Values)
            {
                model.GameStart();
            }
            foreach (BaseCtrl ctrl in ctrlDict.Values)
            {
                ctrl.ReadData();
            }
            foreach (BaseUICtrl uiCtrl in uiCtrlDict.Values)
            {
                uiCtrl.ReadData();
            }
            LogUtil.Log("[ModuleMgr]AllModuleGameStart");
        }

        public BaseModel GetModel(string modelName)
        {
            if (!modelDict.TryGetValue(modelName, out BaseModel model))
            {
                LogUtil.LogError("[ModuleMgr]No Have This Model " + modelName);
            }
            return model;
        }
        public Type GetUIType(string uiName)
        {
            Type uitype = null;
            if (!uiTypeDict.TryGetValue(uiName, out uitype))
            {
                LogUtil.LogError("[ModuleMgr] No Have this UI " + uiName);
            }
            return uitype;
        }

        public BaseCtrl GetCtrl(string ctrlName)
        {
            BaseCtrl ctrl = null;
            if (!ctrlDict.TryGetValue(ctrlName, out ctrl))
            {
                LogUtil.LogError("[ModuleMgr]No Have This Ctrl " + ctrlName);
            }
            return ctrl;
        }
        public BaseUICtrl GetUICtrl(string uiCtrlName)
        {
            BaseUICtrl uiCtrl = null;
            if (!uiCtrlDict.TryGetValue(uiCtrlName, out uiCtrl))
            {
                LogUtil.LogError("[ModuleMgr]No Have This UICtrl " + uiCtrlName);
            }
            return uiCtrl;
        }
        public void SetActiveCtrl(string ctrlName, bool isEnable)
        {
            BaseCtrl ctrl = GetCtrl(ctrlName);
            if (isEnable)
            {
                if (!ctrl.isEnable && !ctrl.IsNew)
                {
                    ctrl.isEnable = true;
                    ctrl.New();
                    ctrl.Init();
                    ctrl.StartUp();
                }
            }
            else
            {
                if (ctrl.isEnable && ctrl.IsNew)
                {
                    ctrl.isEnable = false;
                    ctrl.Dispose();
                }
            }
        }
        public void SetActiveUICtrl(string uiCtrlName, bool isEnable)
        {
            BaseUICtrl uiCtrl = GetUICtrl(uiCtrlName);
            if (isEnable)
            {
                if (!uiCtrl.isEnable && !uiCtrl.IsNew)
                {
                    uiCtrl.isEnable = true;
                    uiCtrl.New();
                    uiCtrl.Init();
                    uiCtrl.StartUp();
                }
            }
            else
            {
                if (uiCtrl.isEnable && uiCtrl.IsNew)
                {
                    uiCtrl.isEnable = false;
                    uiCtrl.Dispose();
                }
            }
        }
        public void ResetModel(string modelName)
        {
            BaseModel model = GetModel(modelName);
            model.Reset();
        }

        public void AddModel(string name,BaseModel model)
        {
            model.modelName = name;
            modelDict[name] = model;
        }
        public void AddUIType(string uiName, Type uiType)
        {
            uiTypeDict[uiName] = uiType;
        }

        public void AddCtrl(string ctrlName, BaseCtrl ctrl)
        {
            ctrl.ctrlName = ctrlName;
            ctrlDict[ctrlName] = ctrl;
        }

        public void AddUICtrl(string ctrlName, BaseUICtrl uiCtrl)
        {
            uiCtrl.ctrlName = ctrlName;
            uiCtrlDict[ctrlName] = uiCtrl;
        }
        public void ResetAllModel()
        {
            foreach (BaseModel model in modelDict.Values)
            {
                model.Reset();
            }
        }
        public void DisposeAllModel()
        {
            foreach (BaseModel model in modelDict.Values)
            {
                model.Dispose();
            }
            modelDict.Clear();
        }
        public void DisposeAllCtrl()
        {
            foreach (BaseCtrl ctrl in ctrlDict.Values)
            {
                ctrl.Dispose();
            }
            foreach (BaseUICtrl uiCtrl in uiCtrlDict.Values)
            {
                uiCtrl.Dispose();
            }
            ctrlDict.Clear();
            uiCtrlDict.Clear();

        }
        public void DisposeAllModule()
        {
            DisposeAllModel();
            DisposeAllCtrl();
            uiTypeDict.Clear();
        }

        public override void Dispose()
        {
            base.Dispose();
            modelDict = null;
            uiTypeDict = null;
            ctrlDict = null;
            uiCtrlDict = null;
        }

    }
}