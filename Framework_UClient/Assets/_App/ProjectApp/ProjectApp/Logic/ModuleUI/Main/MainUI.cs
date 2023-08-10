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
//using UI.G000_Main;

namespace ProjectApp
{
    public class MainUI : BaseUI
    {
        private MainUICtrl uiCtrl;
        private MainModel model;

        public MainUI(MainUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.MainUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "Main";
            uiInfo.assetName = "Main_Wnd";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = true;
           
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
            //ui = baseUI as UI.G000_Main.UI_Main;
        }

        protected override void OnOpenBefore(object args)
        {
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