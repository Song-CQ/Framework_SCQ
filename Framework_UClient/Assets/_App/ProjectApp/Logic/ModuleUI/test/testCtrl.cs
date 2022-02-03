/****************************************************
    文件: Ctr.cs
    作者: Clear
    日期: 2022/2/3 17:50:52
    类型: MVC_AutoCread
    功能: test控制器
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public class testCtrl : BaseCtrl
    {
        public static testCtrl Instance { get; private set; }

        private testModel model;

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
            //model = moduleMgr.GetModel(ModelConst.testModel) as testModel;
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