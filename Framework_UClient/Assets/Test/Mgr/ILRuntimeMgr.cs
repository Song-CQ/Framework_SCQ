/****************************************************
    文件: ILRuntimeMgr.cs
    作者: Clear
    日期: 2022/4/24 17:54:18
    类型: 框架核心脚本(请勿修改)
    功能: ILRuntime C#热更管理器
*****************************************************/
using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace FutureCore
{
    public class ILRuntimeMgr : BaseMonoMgr<ILRuntimeMgr>
    {
        private AppDomain this_AppDomain;

        private MemoryStream dll_ms;
        private MemoryStream pdb_ms;

        private string dicPath;

        private string dllPath = @"HotFix\HotFix.dll";
        private string pdbPath = @"HotFix\HotFix.pdb";


        private void Start()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();

            dicPath = string.Empty;
#if UNITY_EDITOR
            dicPath = "file:///" + Application.streamingAssetsPath;
#else
            dicPath = Application.persistentDataPath;
#endif
            

        }

        public void StartLoadHotFix()
        {
            StartCoroutine(LoadAssembly());
        }

        private IEnumerator LoadAssembly()
        {

            string _dllPath = Path.Combine(dicPath,dllPath);
            UnityWebRequest loadRequestDll = UnityWebRequest.Get(_dllPath);

            yield return loadRequestDll.SendWebRequest();
            if (loadRequestDll.isNetworkError)
            {
                LogUtil.LogError($"[ILRuntimeMgr]加载热更Dll错误,路劲:{_dllPath}");
                //to do 下载
            }
            else
            {
                dll_ms = new MemoryStream(loadRequestDll.downloadHandler.data);
            }

#if UNITY_EDITOR
            //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
            string _pdbPath = Path.Combine(dicPath, pdbPath);
            UnityWebRequest loadRequestPdb = UnityWebRequest.Get(_pdbPath);

            yield return loadRequestPdb.SendWebRequest();
            if (loadRequestPdb.isNetworkError)
            {
                LogUtil.LogError($"[ILRuntimeMgr]加载调试文件Pdb错误,路劲:{_pdbPath}");
            }
            else
            {
                pdb_ms = new MemoryStream(loadRequestPdb.downloadHandler.data);
            }
#endif

            this_AppDomain = new AppDomain();

#if UNITY_EDITOR
            this_AppDomain.LoadAssembly(dll_ms,pdb_ms,new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#else
            this_AppDomain.LoadAssembly(dll_ms);
#endif
            InitializeILRuntime();
            OnLoadHotFixComplete();
           
        }

        private void InitializeILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            this_AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif          

            ILRuntimeMgr_Register.RegisterAll(this_AppDomain);
            this_AppDomain.DelegateManager.RegisterMethodDelegate<object>();

        }

        private void OnLoadHotFixComplete()
        {
            OnHotFixLoaded();
            AppDispatcher.Instance.Dispatch(AppMsg.System_LoadHotFixComplete);
        }

        void OnHotFixLoaded()
        {

            //LogUtil.EnableLog(true);
            //LogUtil.SetLogCallBack_Log(Debug.Log, Debug.LogFormat);
            //LogUtil.SetLogCallBack_LogError(Debug.LogError, Debug.LogErrorFormat);
            //LogUtil.SetLogCallBack_LogWarning(Debug.LogWarning, Debug.LogWarningFormat);
            //HelloWorld，第一次方法调用
            this_AppDomain.Invoke("ProjectApp.HotFix.MainLaunch", "Init", null, null);

            LogUtil.Log("启动热更完毕");
        }

    }
}