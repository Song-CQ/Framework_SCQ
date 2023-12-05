/****************************************************
    文件:MainUI.cs
    作者:Clear
    日期:2022/1/29 23:8:15
    类型:MVC_AutoCread
    功能:MainUI界面
*****************************************************/
using FutureCore;
using FairyGUI;
using UnityEngine;
using UnityEngine.UI;
using System;
//using UI.G000_Main;

namespace ProjectApp
{
    public class MainUI : BaseUI
    {
        #region 控件常量
        private const string ui_OnClick_Key = "ui_OnClick";
        #endregion
        
        private MainUICtrl uiCtrl;
        private MainModel model;
        private UGUIEntity u_Entity;

        public MainUI(MainUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.MainUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "Main";
            uiInfo.assetName = "Main_Plane";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = false;
           
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.MainModel) as MainModel;
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;
        }

        protected override void OnOpenBefore(object args)
        {
            u_Entity.GetComponent<Button>(ui_OnClick_Key).onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            Close();

        }

        protected override void OnOpen(object args)
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnDisplay(object args)
        {
        }
        #endregion

        #region 消息
        protected override void AddListener()
        {
            //modelDispatcher.AddListener(ModelMsg.XXX, OnXXX);
        }
        protected override void RemoveListener()
        {
            //modelDispatcher.RemoveListener(ModelMsg.XXX, OnXXX);
        }
        #endregion

    }
}