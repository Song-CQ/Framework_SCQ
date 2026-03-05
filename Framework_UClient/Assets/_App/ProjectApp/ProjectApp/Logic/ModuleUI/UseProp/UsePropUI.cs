/****************************************************
    文件: UsePropUI.cs
    作者: Clear
    日期: 2026/2/13 1:1:48
    类型: MVC_AutoCread
    功能: UsePropUI界面
*****************************************************/
using FutureCore;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectApp
{
    public class UsePropUI : BaseUI
    {
        #region 控件常量
        //框架自动创建请勿在此处修改内容
        private const string ui_mask_Key = "ui_mask";
        private const string ui_TipsText_Key = "ui_TipsText";

        #endregion
        
		private UsePropUICtrl uiCtrl;
        private UsePropModel model;
        private UGUIEntity u_Entity;

        private GuideMask ui_mask;

        public UsePropUI(UsePropUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.UsePropUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "UseProp";
            uiInfo.assetName = "UseProp_Plane";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedUIMask = false;
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.UsePropModel) as UsePropModel;
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;

            ui_mask = GetComponent<GuideMask>(ui_mask_Key);
            ui_mask.ClickMask = Ui_mask_PointerClick_Event;
        }

        private void Ui_mask_PointerClick_Event(UnityEngine.EventSystems.PointerEventData eventData)
        {
            GameTool.GameCore.Dispatch(GameMsg.CancelExternalProp);
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