/****************************************************
    文件:Ctr.cs
    作者:Clear
    日期:2022/1/29 23:8:15
    类型:MVC_AutoCread
    功能:Main控制器
*****************************************************/
using FutureCore;

namespace ProjectApp.HotFix
{
    public class MainCtrl : BaseCtrl
    {
        public static MainCtrl Instance { get; private set; }

        private MainModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.MainModel) as MainModel;
        }

        protected override void OnDispose()
        {
            Instance = null;
        }
        #endregion

        #region 消息
        protected override void AddListener()
        {
            //ctrlDispatcher.AddListener(CtrlMsg.XXX, OnXXX);
        }
        protected override void RemoveListener()
        {
            //ctrlDispatcher.RemoveListener(CtrlMsg.XXX, OnXXX);
        }
        #endregion
       
    }
}