/****************************************************
    文件: testUI.cs
    作者: Clear
    日期: 2022/2/3 17:50:52
    类型: MVC_AutoCread
    功能: testUI界面
*****************************************************/
using FutureCore;
using FairyGUI;
using UnityEngine;
//using UI.G000_test;

namespace ProjectApp
{
    public class testUI : BaseUI
    {
        private testUICtrl uiCtrl;
        private testModel model;

        public testUI(testUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.testUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "G000_test";
            uiInfo.assetName = "G000_test";
            uiInfo.layerType = UILayerType.None;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = true;
            uiInfo.isNeedUIMask = true;
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.testModel) as testModel;
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            //ui = baseUI as UI.G000_test.UI_test;
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