/****************************************************
    文件: FXEffectUI.cs
    作者: Clear
    日期: 2023/11/23 19:34:51
    类型: MVC_AutoCread
    功能: FXEffectUI界面
*****************************************************/
using FutureCore;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ProjectApp
{
    public class FXEffectUI : BaseUI
    {
        #region 控件常量
        #endregion
        private FXEffectUICtrl uiCtrl;
        private FXEffectModel model;
        private UGUIEntity u_Entity;

        public FXEffectUI(FXEffectUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.FXEffectUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "FXEffect";
            uiInfo.assetName = "FXEffect_Plane";
            uiInfo.layerType = UILayerType.Animation;
            uiInfo.isNeedOpenAnim = false;
            uiInfo.isNeedCloseAnim = false;
            uiInfo.isNeedUIMask = false;
            
        }

        #region 生命周期
        protected override void OnInit()
        {
            model = moduleMgr.GetModel(ModelConst.FXEffectModel) as FXEffectModel;
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;
            
        }

        public void PlayEffect(Effect uiEffect, Vector2 screenPot)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(u_Entity.Transform, screenPot, CameraMgr.Instance.uiCamera, out Vector2 LocalPoint);
            uiEffect.entity.transform.SetParent(u_Entity.Transform);
            uiEffect.entity.transform.localPosition = LocalPoint;
            uiEffect.entity.transform.localScale = Vector3.one;

            uiEffect.Play();
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