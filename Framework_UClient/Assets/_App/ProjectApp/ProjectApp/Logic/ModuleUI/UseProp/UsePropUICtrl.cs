/****************************************************
    文件: UICtr.cs
    作者: Clear
    日期: 2026/2/13 1:1:48
    类型: MVC_AutoCread
    功能: UsePropUI控制器
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    public class UsePropUICtrl : BaseUICtrl
    {
        private UsePropUI ui;
        private UsePropModel model;

        private uint openUIMsg = UICtrlMsg.UsePropUI_Open;
        private uint closeUIMsg = UICtrlMsg.UsePropUI_Close;

        #region 生命周期
        protected override void OnInit()
        {
           //model = moduleMgr.GetModel(ModelConst.UsePropModel) as UsePropModel;
        }

        protected override void OnDispose()
        {
        }

        public override void OpenUI(object args = null)
        {
            if (ui == null)
            {
                ui = new UsePropUI(this);
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