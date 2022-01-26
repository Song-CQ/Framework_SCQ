/****************************************************
    文件：BaseUI.cs
	作者：Clear
    日期：2022/1/25 10:45:49
    类型: 框架核心脚本(请勿修改)
	功能：基础UI
*****************************************************/
using System;

namespace FutureCore
{
    public abstract class BaseUI
    {
        #region Field
        protected UIMgr uiMgr;
        protected ModuleMgr moduleMgr;

        protected UICtrlDispatcher uiCtrlDispatcher;

        public BaseUICtrl baseUICtrl;

        public string uiName;
        public UIInfo uiInfo;

        public UIEntity uiEntity;

        /// <summary>
        /// ui参数
        /// </summary>
        public object uiArgs;

        public UILayerType currUILayer;


        public bool isOpen;
        public bool isVisible;
        public bool isClose;

        #endregion


        public BaseUI()
        {

        }

        public BaseUI(BaseUICtrl baseUICtrl)
        {
            New(baseUICtrl);
        }

        public void New(BaseUICtrl baseUICtrl)
        {
            this.baseUICtrl = baseUICtrl;

            Assignment();
            OnNew();
            Process_Init();
        }
        protected virtual void Assignment()
        {

            uiMgr = UIMgr.Instance;
            moduleMgr = ModuleMgr.Instance;

            uiCtrlDispatcher = UICtrlDispatcher.Instance;

        }
        protected virtual void UnAssignment()
        {
            uiMgr = null;
            moduleMgr = null;

            uiCtrlDispatcher = null;
        }

        #region Process
        private void Process_Init()
        {
            isOpen = false;
            isVisible = false;
            isClose = false;

            uiInfo = ObjectPoolStatic<UIInfo>.Get();
            uiInfo.Reset();
            SetUIInfo(uiInfo);
            OnInit();

        }

        public void Process_Bind()
        {
            OnBind();
        }

        public void Process_OpenBefore(object args)
        {
            OnOpenBefore(args);
        }
        public void Process_Open(object args)
        {
            OnOpen(args);
            AddListener();

            isOpen = true;
        }

        public void Process_Close()
        {
            RemoveListener();
            OnClose();
            
            isClose = true;
        }

        public void Process_Destroy()
        {           
            OnDestroy();
            UnAssignment();
            isOpen = false;
            isVisible = false;
            isClose = true;
            ObjectPoolStatic<UIInfo>.Release(uiInfo);
        }
        public void Process_Hide()
        {
            isVisible = false;
            uiEntity.SetVisible(isVisible);
            OnHide();
        }

        public void Process_Display(object args)
        {
            isVisible = true;
            uiEntity.SetVisible(isVisible);
            OnDisplay(args);
        }

        public  void Process_OpenUIAnimEnd() 
        {
            OpenUIAnimEnd();
        }

        
        public void Process_CloseUIAnimEnd() {
            CloseUIAnimEnd();
        }
       

        #endregion

        #region Interface: UI

        public void Open(object args = null)
        {
            uiArgs = args;
            uiMgr.Internal_OpenUI(this, args);
        }

     

        public void Close()
        {
            if (isClose) return;
            uiMgr.Internal_CloseUI(this);
        }
        public void Hide()
        {
            uiMgr.Internal_HideUI(this);
        }

        public void Display(object args = null)
        {
            uiArgs = args;
            uiMgr.Internal_DisplayUI(this, args);
        }

        #endregion

        #region Virtual Logic
        protected virtual void OnNew()
        {

        }
        protected abstract void SetUIInfo(UIInfo uiInfo);
        protected virtual void OnInit() { }
        protected virtual void OnBind() { }   
        protected virtual void OnOpenBefore(object args) { }
        protected virtual void OnOpen(object args) { }
        protected virtual void AddListener() { }
        protected virtual void OpenUIAnimEnd() { }
        protected virtual void OnDisplay(object arge) { }
        public virtual void OnUpdate() { }
        protected virtual void OnHide() { }
        protected virtual void CloseUIAnimEnd() { }
        protected virtual void RemoveListener() { }
        protected virtual void OnClose() { }
        protected virtual void OnDestroy() { }
        #endregion

        #region UIEntity
        public void CloseUIAnim(Action onComplete)
        {
            uiEntity.CloseUIAnim(onComplete);
        }

        public void OpenUIAnim(Action onComplete)
        {
            uiEntity.OpenUIAnim(onComplete);
        }
        #endregion

    }
}