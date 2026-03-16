/****************************************************
    文件: QuestUI.cs
    作者: Clear
    日期: 2026/3/16 19:6:40
    类型: MVC_AutoCread
    功能: QuestUI界面
*****************************************************/
using FutureCore;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectApp
{
    public class QuestUI : BaseUI
    {
        #region 控件常量
        //框架自动创建请勿在此处修改内容(会被覆盖掉)
        #endregion
        
		private QuestUICtrl uiCtrl;
        private QuestModel model;
        private UGUIEntity u_Entity;

        public QuestUI(QuestUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.QuestUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "Quest";
            uiInfo.assetName = "Quest_Plane";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = true;
            uiInfo.isNeedUIMask = true;
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.QuestModel) as QuestModel;
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