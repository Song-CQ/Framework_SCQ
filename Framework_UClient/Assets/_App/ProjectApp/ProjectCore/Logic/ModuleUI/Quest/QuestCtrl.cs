/****************************************************
    文件: Ctr.cs
    作者: Clear
    日期: 2026/3/16 19:6:40
    类型: MVC_AutoCread
    功能: Quest控制器
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    public class QuestCtrl : BaseCtrl
    {
        public static QuestCtrl Instance { get; private set; }

        private QuestModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.QuestModel) as QuestModel;
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