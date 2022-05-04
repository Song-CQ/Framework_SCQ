/****************************************************
    文件: Ctr.cs
    作者: Clear
    日期: 2022/5/3 14:23:13
    类型: MVC_AutoCread
    功能: Load控制器
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public class LoadCtrl : BaseCtrl
    {
        public static LoadCtrl Instance { get; private set; }

        private LoadModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.LoadModel) as LoadModel;
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