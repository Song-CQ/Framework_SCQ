/****************************************************
    文件: Ctr.cs
    作者: Clear
    日期: 2022/2/7 11:44:43
    类型: MVC_AutoCread
    功能: Game控制器
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public class GameCtrl : BaseCtrl
    {
        public static GameCtrl Instance { get; private set; }

        private GameModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.GameModel) as GameModel;
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