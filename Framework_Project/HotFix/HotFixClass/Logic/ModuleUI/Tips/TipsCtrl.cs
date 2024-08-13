/****************************************************
    文件: Ctr.cs
    作者: Clear
    日期: 2022/6/2 18:38:49
    类型: MVC_AutoCread
    功能: Tips控制器
*****************************************************/
using FutureCore;

namespace ProjectApp.HotFix
{
    public class TipsCtrl : BaseCtrl
    {
        public static TipsCtrl Instance { get; private set; }

        private TipsModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.TipsModel) as TipsModel;
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