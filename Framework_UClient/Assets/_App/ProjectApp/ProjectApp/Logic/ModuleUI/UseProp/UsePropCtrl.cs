/****************************************************
    文件: Ctr.cs
    作者: Clear
    日期: 2026/2/13 1:1:48
    类型: MVC_AutoCread
    功能: UseProp控制器
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    public class UsePropCtrl : BaseCtrl
    {
        public static UsePropCtrl Instance { get; private set; }

        private UsePropModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.UsePropModel) as UsePropModel;
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