/****************************************************
    文件: Ctr.cs
    作者: Clear
    日期: 2023/11/24 15:47:44
    类型: MVC_AutoCread
    功能: dfasdas控制器
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    public class dfasdasCtrl : BaseCtrl
    {
        public static dfasdasCtrl Instance { get; private set; }

        private dfasdasModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.dfasdasModel) as dfasdasModel;
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