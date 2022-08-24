/****************************************************
    文件：BaseUICtrl.cs
	作者：Clear
    日期：2022/1/10 19:42:57
    类型: 主框架(请勿修改)
	功能：MVC[基础UI控制器]
*****************************************************/

using UnityEngine;

namespace FutureCore
{
    public abstract class BaseUICtrl : BaseCtrl
    {
        protected override void OnInit()
        {
            
        }

        

        public void DispatchCloseUI(string uiName = null, object args = null)
        {
            uint msgId = GetCloseUIMsg(uiName);
            if (msgId == 0)
            {
                CloseUI();
                return;
            }
            if (uiCtrlDispatcher != null)
            {
                uiCtrlDispatcher.Dispatch(msgId, args);
            }
        }
        public virtual uint GetOpenUIMsg(string uiName)
        {
            return 0;
        }
        public virtual uint GetCloseUIMsg(string uiName)
        {
            return 0;
        }
        public abstract void OpenUI(object args = null);
        public abstract void CloseUI(object args = null);
        protected override void OnDispose()
        {

        }
    }
}