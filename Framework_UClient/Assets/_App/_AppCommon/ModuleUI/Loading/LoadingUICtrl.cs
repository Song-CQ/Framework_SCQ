/****************************************************
    文件: UICtr.cs
    作者: Clear
    日期: 2022/5/3 14:23:13
    类型: MVC_AutoCread
    功能: LoadingUI控制器
*****************************************************/
using UnityEngine;
using FutureCore;
using System;

namespace ProjectApp
{
    public class LoadingUICtrl : BaseUICtrl
    {
        private LoadingUI ui;
        private LoadingModel model;

        private uint openUIMsg = UICtrlMsg.LoadingUI_Open;
        private uint closeUIMsg = UICtrlMsg.LoadingUI_Close;

        #region 生命周期
        protected override void OnInit()
        {
           //model = moduleMgr.GetModel(ModelConst.LoadModel) as LoadModel;
        }

        protected override void OnDispose()
        {
        }

        public override void OpenUI(object args = null)
        {
            if (ui == null)
            {
                ui = new LoadingUI(this);
                ui.Open(args);
            }
        }

        public override void CloseUI(object args = null)
        {
            if (ui != null && !ui.isClose)
            {
                ui.Close();
            }
            ui = null;
        }
        #endregion

        #region 消息
        public override uint GetOpenUIMsg(string uiName)
        {
            return openUIMsg;
        }
        public override uint GetCloseUIMsg(string uiName)
        {
            return closeUIMsg;
        }

        protected override void AddListener()
        {
            uiCtrlDispatcher.AddListener(openUIMsg, OpenUI);
            uiCtrlDispatcher.AddListener(closeUIMsg, CloseUI);
            
            AppDispatcher.Instance.AddListener(AppMsg.UI_DisplayLoadingUI,OpenUI);
            AppDispatcher.Instance.AddListener(AppMsg.UI_HideLoadingUI, CloseUI);

            GenericDispatcher.Instance.AddListener<int, Action>(AppMsg.UI_SetLoadingValueUI, SetLoadingValue);
            GenericDispatcher.Instance.AddListener<string>(AppMsg.UI_SetLoadingMsg, SetLoadingMsg);
        }
        protected override void RemoveListener()
        {
            uiCtrlDispatcher.RemoveListener(openUIMsg, OpenUI);
            uiCtrlDispatcher.RemoveListener(closeUIMsg, CloseUI);

            AppDispatcher.Instance.RemoveListener(AppMsg.UI_DisplayLoadingUI, OpenUI);         
            AppDispatcher.Instance.RemoveListener(AppMsg.UI_HideLoadingUI, CloseUI);

            GenericDispatcher.Instance.RemoveListener<int,Action>(AppMsg.UI_SetLoadingValueUI, SetLoadingValue);
            GenericDispatcher.Instance.RemoveListener<string>(AppMsg.UI_SetLoadingMsg, SetLoadingMsg);
        }
        #endregion

        private void SetLoadingValue(int val, Action OnComplete)
        {
            if (ui!=null)
            {
                ui.SetLoadingValue(val,LoadingTrasitionConst.loadingTraTime,OnComplete);
            }

        }
        private void SetLoadingMsg(string val)
        {
            if (ui!=null)
            {
                ui.SetLoadingMsg(val);
            }
        }

    }
}