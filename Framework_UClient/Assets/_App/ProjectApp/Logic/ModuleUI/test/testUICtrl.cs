/****************************************************
    文件: UICtr.cs
    作者: Clear
    日期: 2022/2/3 17:50:52
    类型: MVC_AutoCread
    功能: testUI控制器
*****************************************************/
using UnityEngine;
using FutureCore;

namespace ProjectApp
{
    public class testUICtrl : BaseUICtrl
    {
        private testUI ui;
        private testModel model;

        private uint openUIMsg = UICtrlMsg.testUI_Open;
        private uint closeUIMsg = UICtrlMsg.testUI_Close;

        #region 生命周期
        protected override void OnInit()
        {
           //model = moduleMgr.GetModel(ModelConst.testModel) as testModel;
        }

        protected override void OnDispose()
        {
        }

        public override void OpenUI(object args = null)
        {
            if (ui == null)
            {
                ui = new testUI(this);
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