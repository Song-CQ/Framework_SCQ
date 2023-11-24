/****************************************************
    文件: LoadUI.cs
    作者: Clear
    日期: 2022/5/3 14:23:13
    类型: MVC_AutoCread
    功能: LoadingUI界面
*****************************************************/
using DG.Tweening;
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

    public class LoadingUI : BaseUI
    {
        #region 控件常量
        private const string ui_loadingImg_Key = "ui_loading_img";
        private const string ui_loadingText_Key = "ui_loading_text";
        #endregion

        private LoadingUICtrl uiCtrl;
        private LoadingModel model;
        private UGUIEntity u_Entity;
        //to do 代码创建时根据名字创建字符串
       

        private float currVal;
        private Image loadingImg;
        private Text loadingText;

        private Tweener pdTweener;

        public LoadingUI(LoadingUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.LoadUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "Common";
            uiInfo.assetName = "Loading_Plane";
            uiInfo.layerType = UILayerType.Loading;

        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.LoadModel) as LoadModel;
            SetLoadingValue(0);
        }



        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;

            loadingImg = u_Entity.GetComponent<Image>(ui_loadingImg_Key);
            loadingText = u_Entity.GetComponent<Text>(ui_loadingText_Key);

        }

        protected override void OnOpenBefore(object args)
        {
            
           
        }

        protected override void OnOpen(object args)
        {
            loadingImg.fillAmount = currVal;
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
            if (loadingImg)
            { 
               loadingImg.fillAmount = currVal;
            }
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
            pdTweener = DOTween.To(()=>
            {
                if (loadingImg)
                {
                    return loadingImg.fillAmount;
                }
                else
                {
                    return 0;
                }
               
            }
            ,(v)=>
            {
                if (loadingImg)
                {
                    loadingImg.fillAmount = v;
                } 
            } ,val,duration);
            pdTweener.OnComplete(() =>
            {
                pdTweener = null;
                if (loadingImg)
                {
                    loadingImg.fillAmount = currVal;
                }             
                cb?.Invoke();
            });

        }

        public void SetLoadingMsg(string val)
        {
            if (loadingText)
            {
                loadingText.text = val;
            }
          
        }
    }
}