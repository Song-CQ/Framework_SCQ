/****************************************************
    文件：BaseModel.cs
	作者：Clear
    日期：2022/1/10 20:6:25
    类型: 框架核心脚本(请勿修改)
	功能：MVC[基础数据]
*****************************************************/

namespace FutureCore
{
    public abstract class BaseModel
    {
        public string modelName;
        private string localStorageKey;

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
            OnNew();
        }
        public void Init()
        {
            Assignment();
            AddListener();
            OnInit();
        }
        public void StartUp()
        {
            OnStartUp();
        }
        public void ReadData()
        {
            OnReadData();
        }
        public void GameStart()
        {
            OnGameStart();
        }
        public void Reset()
        {
            Dispose();
            Init();
        }
        public void Dispose()
        {
            RemoveListener();
            OnDispose();
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

        protected virtual void OnNew() { }
        protected abstract void OnInit();
        protected virtual void OnStartUp() { }
        protected virtual void OnReadData() { }
        protected virtual void OnGameStart() { }
        protected abstract void OnReset();
        protected abstract void OnDispose();

        protected virtual void AddListener() { }
        protected virtual void RemoveListener() { }

        #region LocalStorage
        protected virtual void WriteLocalStorage() { }
        private string GetLocalStorageKey()
        {
            if (string.IsNullOrEmpty(localStorageKey))
            {
                localStorageKey = string.Concat("M_", modelName, "_", App.UserInfo.userId);
            }
            return localStorageKey;
        }
        protected void SetCustomLocalStorageKey(string customSuffixKey)
        {
            localStorageKey = string.Concat(modelName, "_", customSuffixKey);
        }
        protected void WriteLocalStorage(object storageData)
        {
            string key = GetLocalStorageKey();
            PrefsUtil.WriteObject(key, storageData);
        }
        protected object ReadLocalStorage<T>()
        {
            string key = GetLocalStorageKey();
            return PrefsUtil.ReadObject<T>(key);
        }
        protected bool HasLocalStorage()
        {
            string key = GetLocalStorageKey();
            return PrefsUtil.HasKey(key);
        }
        protected void DeleteLocalStorage()
        {
            string key = GetLocalStorageKey();
            PrefsUtil.DeleteKey(key);
        }
        #endregion

        #region 公共数据层自定义扩展
        //public static XXXModel Get()
        //{
        //    return ModuleMgr.Instance.GetModel(modelName);
        //}
        #endregion
    }
}