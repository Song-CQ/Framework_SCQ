using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FutureCore;
using DG.Tweening;
using Random = UnityEngine.Random;


namespace ProjectApp
{
    public class MainUI : BaseUI
    {
        private MainUICtrl ctrl;
        private MainModel model;
        public UI.G004_main.com_main ui;


        public MainUI(MainUICtrl ctrl) : base(ctrl)
        {
            uiName = "main";
            this.ctrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "G004_main";
            uiInfo.assetName = "com_main";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = false;
            uiInfo.isNeedUIMask = true;
            uiInfo.isTickUpdate = true;


        }

        #region 生命周期

        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.MainModel) as MainModel;
          
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
           
        }


        protected override void OnOpenBefore(object args)
        {
            //Debug.LogError("打开");

        }


         

        protected override void OnOpen(object args)
        {
           

        }

       

        protected override void OnHide()
        {
          
        }

        protected override void OnDisplay(object args)
        {
            
        }

        #endregion

        #region 消息

        protected override void AddListener()
        {
            //modelDispatcher.AddListener(ModelMsg.XXX, OnXXX);
           
        }     
        protected override void RemoveListener()
        {
            //modelDispatcher.RemoveListener(ModelMsg.XXX, OnXXX);
           

        }

        #endregion
    }
}