/****************************************************
    文件: UICtr.cs
    作者: Clear
    日期: 2026/3/16 19:6:40
    类型: MVC_AutoCread
    功能: QuestUI控制器
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    public class QuestUICtrl : BaseUICtrl
    {
        private QuestUI ui;
        private QuestModel model;

        private uint openUIMsg = UICtrlMsg.QuestUI_Open;
        private uint closeUIMsg = UICtrlMsg.QuestUI_Close;

        #region 生命周期
        protected override void OnInit()
        {
           //model = moduleMgr.GetModel(ModelConst.QuestModel) as QuestModel;
        }

        protected override void OnDispose()
        {
        }

        public override void OpenUI(object args = null)
        {
            if (ui == null)
            {
                ui = new QuestUI(this);
                ui.Open(args);
            }
        }

        public override void CloseUI(object args = null)
        {
            if (ui != null && !ui.isClose)
            {
                ui.Close();
            }
            ui = null;
        }
        #endregion

        #region 消息
        public override uint GetOpenUIMsg(string uiName)
        {
            return openUIMsg;
        }
        public override uint GetCloseUIMsg(string uiName)
        {
            return closeUIMsg;
        }

        protected override void AddListener()
        {
            uiCtrlDispatcher.AddListener(openUIMsg, OpenUI);
            uiCtrlDispatcher.AddListener(closeUIMsg, CloseUI);
        }
        protected override void RemoveListener()
        {
            uiCtrlDispatcher.RemoveListener(openUIMsg, OpenUI);
            uiCtrlDispatcher.RemoveListener(closeUIMsg, CloseUI);
        }
        #endregion

    }
}