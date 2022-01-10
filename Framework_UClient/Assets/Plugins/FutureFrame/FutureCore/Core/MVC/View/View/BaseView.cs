/****************************************************
    文件：BaseView.cs
	作者：Clear
    日期：2022/1/10 20:14:29
    类型: 框架核心脚本(请勿修改)
	功能：Nothing
*****************************************************/

namespace FutureCore
{
    public abstract class BaseView
    {
        protected ModuleMgr moduleMgr;

        //protected ModelDispatcher modelDispatcher;
        //protected ViewDispatcher viewDispatcher;
        protected CtrlDispatcher ctrlDispatcher;
        protected UICtrlDispatcher uiCtrlDispatcher;
        //protected DataDispatcher dataDispatcher;
        //protected GameDispatcher gameDispatcher;
        //protected WSNetDispatcher wsNetDispatcher;

        public virtual void Init()
        {
            Assignment();
            AddListener();
        }

        public virtual void Dispose()
        {
          
            UnAssignment();
        }

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

        protected abstract void AddListener();
        protected abstract void RemoveListener();
    }
}