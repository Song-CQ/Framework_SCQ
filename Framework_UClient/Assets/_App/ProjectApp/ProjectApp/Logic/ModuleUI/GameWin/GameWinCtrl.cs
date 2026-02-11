/****************************************************
    文件: Ctr.cs
    作者: Clear
    日期: 2026/2/11 23:12:53
    类型: MVC_AutoCread
    功能: GameWin控制器
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    public class GameWinCtrl : BaseCtrl
    {
        public static GameWinCtrl Instance { get; private set; }

        private GameWinModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.GameWinModel) as GameWinModel;
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