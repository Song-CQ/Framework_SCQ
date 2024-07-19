/****************************************************
    文件: GameUI.cs
    作者: Clear
    日期: 2023/11/23 20:49:21
    类型: MVC_AutoCread
    功能: GameUI界面
*****************************************************/
using FutureCore;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectApp
{
    public class GameUI : BaseUI
    {
        #region 控件常量
        #endregion
        private GameUICtrl uiCtrl;
        private GameModel model;
        private UGUIEntity u_Entity;

        public GameUI(GameUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.GameUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "Game";
            uiInfo.assetName = "Game_Plane";
            uiInfo.layerType = UILayerType.Normal;
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