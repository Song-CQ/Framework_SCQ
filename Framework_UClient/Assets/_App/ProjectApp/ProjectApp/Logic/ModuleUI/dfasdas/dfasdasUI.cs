/****************************************************
    文件: dfasdasUI.cs
    作者: Clear
    日期: 2023/11/24 15:47:44
    类型: MVC_AutoCread
    功能: dfasdasUI界面
*****************************************************/
using FutureCore;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectApp
{
    public class dfasdasUI : BaseUI
    {
        #region 控件常量
        #endregion
        private dfasdasUICtrl uiCtrl;
        private dfasdasModel model;
        private UGUIEntity u_Entity;

        public dfasdasUI(dfasdasUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.dfasdasUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "dfasdas";
            uiInfo.assetName = "dfasdas_Plane";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = true;
            uiInfo.isNeedUIMask = true;
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.dfasdasModel) as dfasdasModel;
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