/****************************************************
    文件:UICtr.cs
    作者:Clear
    日期:2022/1/29 23:8:15
    类型:MVC_AutoCread
    功能:MainUI控制器
*****************************************************/
using UnityEngine;
using FutureCore;

namespace ProjectApp
{
    public class MainUICtrl : BaseUICtrl
    {
        private MainUI ui;
        private MainModel model;

        private uint openUIMsg = 0;//UICtrlMsg.MainUI_Open;
        private uint closeUIMsg = 0;//UICtrlMsg.MainUI_Close;

        #region 生命周期
        protected override void OnInit()
        {
           //model = moduleMgr.GetModel(ModelConst.MainModel) as MainModel;
        }

        protected override void OnDispose()
        {
        }

        public override void OpenUI(object args = null)
        {
            if (ui == null)
            {
                ui = new MainUI(this);
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