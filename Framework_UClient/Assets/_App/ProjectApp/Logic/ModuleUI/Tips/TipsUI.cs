/****************************************************
    文件: TipsUI.cs
    作者: Clear
    日期: 2022/6/2 18:38:49
    类型: MVC_AutoCread
    功能: TipsUI界面
*****************************************************/
using FutureCore;
using FairyGUI;
//using UI.G000_Tips;

namespace ProjectApp
{
    public class TipsUI : BaseUI
    {
        private TipsUICtrl uiCtrl;
        private TipsModel model;
        private FGUIEntity fGuiEntity;
        //private UI.G000_Tips.UI_Tips ui;

        public TipsUI(TipsUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.TipsUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "G000_Tips";
            uiInfo.assetName = "G000_Tips";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = true;
            uiInfo.isNeedUIMask = true;
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.TipsModel) as TipsModel;
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            fGuiEntity = uiEntity as FGUIEntity;
            //ui = fGuiEntity.ui as UI.G000_Tips.UI_Tips;
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