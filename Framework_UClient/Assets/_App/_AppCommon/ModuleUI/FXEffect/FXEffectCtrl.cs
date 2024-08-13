/****************************************************
    文件: Ctr.cs
    作者: Clear
    日期: 2023/11/23 19:34:52
    类型: MVC_AutoCread
    功能: FXEffect控制器
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    public class FXEffectCtrl : BaseCtrl
    {
        public static FXEffectCtrl Instance { get; private set; }

        private FXEffectModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.FXEffectModel) as FXEffectModel;
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