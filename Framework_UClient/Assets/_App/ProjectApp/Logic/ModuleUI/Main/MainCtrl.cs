using System;
using System.Collections;
using System.Collections.Generic;
using FutureCore;
using ProjectApp.Data;
using UnityEngine;

namespace ProjectApp
{
    public class MainCtrl : BaseCtrl
    {
        public static MainCtrl Instance { get; private set; }

        private MainModel model;
        

        #region 生命周期
        protected override void OnInit()
        {
            Instance = this;
           // model = moduleMgr.GetModel(ModelConst.MainModel) as MainModel;
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