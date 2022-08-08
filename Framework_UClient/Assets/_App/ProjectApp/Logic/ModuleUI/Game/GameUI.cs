/****************************************************
    文件: GameUI.cs
    作者: Clear
    日期: 2022/2/7 11:44:43
    类型: MVC_AutoCread
    功能: GameUI界面
*****************************************************/
using FutureCore;
using FairyGUI;
using UnityEngine;
//using UI.G000_Game;

namespace ProjectApp
{
    public class GameUI : BaseUI
    {
        private GameUICtrl uiCtrl;
        private GameModel model;

        public GameUI(GameUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.GameUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "G000_Game";
            uiInfo.assetName = "G000_Game";
            uiInfo.layerType = UILayerType.None;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = true;
            uiInfo.isNeedUIMask = true;
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.GameModel) as GameModel;
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            //ui = baseUI as UI.G000_Game.UI_Game;
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