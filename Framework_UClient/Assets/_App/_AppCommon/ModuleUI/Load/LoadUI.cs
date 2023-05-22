/****************************************************
    文件: LoadUI.cs
    作者: Clear
    日期: 2022/5/3 14:23:13
    类型: MVC_AutoCread
    功能: LoadUI界面
*****************************************************/
using FutureCore;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectApp
{
    #region FGUI
    /*
     * using UI.A002_loading;
     * using FairyGUI;
    public class LoadUI : BaseUI
    {
        private LoadUICtrl uiCtrl;
        private LoadModel model;
        private FGUIEntity fGuiEntity;
        private com_loading ui;

        private float currVal;

        private GTweener pdGTweener;

        public LoadUI(LoadUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.LoadUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "A002_loading";
            uiInfo.assetName = "com_loading";
            uiInfo.layerType = UILayerType.Loading;

        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.LoadModel) as LoadModel;
        }



        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            fGuiEntity = uiEntity as FGUIEntity;
            ui = (com_loading)fGuiEntity.UI;

        }

        protected override void OnOpenBefore(object args)
        {
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
        public void SetLoadingValue(int v)
        {
            if (ui != null)
            {
                currVal = v;
                ui.pb_loading.value = currVal;
            }
        }
        public void SetLoadingValue(int val, float duration, Action cb)
        {
            if (ui == null)
            {
                return;
            }
            if (val <= currVal)
            {
                return;
            }
            if (pdGTweener != null)
            {
                pdGTweener.Kill();
            }
            currVal = val;
            pdGTweener = ui.pb_loading.TweenValue(currVal, duration).OnComplete(() =>
            {
                pdGTweener = null;
                ui.pb_loading.value = currVal; 
                cb?.Invoke();
            });

        }

        public void SetLoadingMsg(string val)
        {
            ui.text_severStatus.text = val;
        }
    }
    */
    #endregion

    public class LoadUI : BaseUI
    {
        private LoadUICtrl uiCtrl;
        private LoadModel model;
        private UGUIEntity u_Entity;
        //to do 代码创建时根据名字创建字符串
        private const string ui_loadingImg_Key = "ui_loading_Img";
        private const string ui_loadingText_Key = "ui_loadingText_Img";

        private float currVal;
        private Image loadingImg;
        private Text loadingText;


        public LoadUI(LoadUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.LoadUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "Loading";
            uiInfo.assetName = "Loading";
            uiInfo.layerType = UILayerType.Loading;

        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.LoadModel) as LoadModel;
        }



        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;

            loadingImg = u_Entity.GetComponent<Image>(LoadingImg);

        }

        protected override void OnOpenBefore(object args)
        {
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
        public void SetLoadingValue(int v)
        {
            currVal = v;
            loadingImg.fillAmount = currVal;
        }
        public void SetLoadingValue(int val, float duration, Action cb)
        {
            if (val <= currVal)
            {
                return;
            }
            if (pdTweener != null)
            {
                pdTweener.Kill();
            }
            currVal = val;
            pdTweener = ui.pb_loading.TweenValue(currVal, duration).OnComplete(() =>
            {
                pdGTweener = null;
                ui.pb_loading.value = currVal; 
                cb?.Invoke();
            });

        }

        public void SetLoadingMsg(string val)
        {
            ui.text_severStatus.text = val;
        }
    }
}