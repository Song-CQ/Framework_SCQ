/****************************************************
    文件: GameWinUI.cs
    作者: Clear
    日期: 2026/2/11 23:12:53
    类型: MVC_AutoCread
    功能: GameWinUI界面
*****************************************************/
using FutureCore;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectApp
{
    public class GameWinUI : BaseUI
    {
        #region 控件常量
        //框架自动创建请勿在此处修改内容
        private const string ui_btn_rest_Key = "ui_btn_rest";
        private const string ui_btn_exit_Key = "ui_btn_exit";

        #endregion
        
		private GameWinUICtrl uiCtrl;
        private GameWinModel model;
        private UGUIEntity u_Entity;

        private UIEventListener btn_Rest;
        private UIEventListener btn_Exit;

        public GameWinUI(GameWinUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.GameWinUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "GameWin";
            uiInfo.assetName = "GameWin_Plane";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = true;
            uiInfo.isNeedUIMask = true;
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.GameWinModel) as GameWinModel;
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;

            btn_Rest = GetComponent<UIEventListener>( ui_btn_rest_Key,true);
            btn_Exit = GetComponent<UIEventListener>( ui_btn_exit_Key,true);

            btn_Exit.PointerClick_Event += OnClickExitBtn;
            btn_Rest.PointerClick_Event += OnClickRestBtn;

        }

        private void OnClickExitBtn(PointerEventData eventData)
        {
            Close();
            uiCtrlDispatcher.Dispatch(UICtrlMsg.GameUI_Close);
            GameTool.GameCore.ExitGame();

        }

        private void OnClickRestBtn(PointerEventData eventData)
        {
            Close();
            uiCtrlDispatcher.Dispatch(UICtrlMsg.GameUI_Close);
            GameTool.GameCore.ResetGame();
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