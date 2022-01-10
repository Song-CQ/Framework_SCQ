/****************************************************
    文件：BaseCtrl.cs
	作者：Clear
    日期：2022/1/10 17:45:20
    类型: 主框架(请勿修改)
	功能：MVC[基础控制器]
*****************************************************/

using UnityEngine;

namespace FutureCore
{
    public abstract class BaseCtrl
    {
        public string ctrName;
        public bool isEnable = false;
        public bool IsNew { get; private set; }
        protected ModuleMgr moduleMgr;

        //protected ModelDispatcher modelDispatcher;
        //protected ViewDispatcher viewDispatcher;
        protected CtrlDispatcher ctrlDispatcher;
        protected UICtrlDispatcher uiCtrlDispatcher;
        //protected DataDispatcher dataDispatcher;
        //protected GameDispatcher gameDispatcher;
        //protected WSNetDispatcher wsNetDispatcher;

        public void New()
        {
            if (!isEnable) return;

            OnNew();
            IsNew = true;
        }
        public virtual void Init()
        {
            if (!isEnable) return;

            Assignment();
            AddListener();

            OnInit();
        }
        public virtual void StartUp()
        {
            if (!isEnable) return;

            OnStartUp();
        }
        public void ReadData()
        {
            OnReadData();
        }
        public virtual void GameStart()
        {
            if (!isEnable) return;

            OnGameStart();
        }
        public virtual void Dispose()
        {
            if (!isEnable) return;

            RemoveListener();
            OnDispose();
            UnAssignment();
            IsNew = false;
        }
        /// <summary>
        /// 消息派发器注入
        /// </summary>
        protected virtual void Assignment()
        {
            moduleMgr = ModuleMgr.Instance;

            //modelDispatcher = ModelDispatcher.Instance;
            //viewDispatcher = ViewDispatcher.Instance;
            ctrlDispatcher = CtrlDispatcher.Instance;
            uiCtrlDispatcher = UICtrlDispatcher.Instance;
            //dataDispatcher = DataDispatcher.Instance;
            //gameDispatcher = GameDispatcher.Instance;
            //wsNetDispatcher = WSNetDispatcher.Instance;
        }

        protected virtual void UnAssignment()
        {
            moduleMgr = null;

            //modelDispatcher = null;
            //viewDispatcher = null;
            ctrlDispatcher = null;
            uiCtrlDispatcher = null;
            //dataDispatcher = null;
            //gameDispatcher = null;
            //wsNetDispatcher = null;
        }

        protected virtual void OnNew() { }
        protected abstract void OnInit();
        protected virtual void OnStartUp() { }
        protected virtual void OnReadData() { }
        protected virtual void OnGameStart() { }
        protected abstract void OnDispose();

        protected virtual void AddListener() { }
        protected virtual void RemoveListener() { }
    }
}