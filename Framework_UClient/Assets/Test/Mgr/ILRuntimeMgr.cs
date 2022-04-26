/****************************************************
    文件: ILRuntimeMgr.cs
    作者: Clear
    日期: 2022/4/24 17:54:18
    类型: 框架核心脚本(请勿修改)
    功能: ILRuntime C#热更管理器
*****************************************************/
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace FutureCore
{
    public class ILRuntimeMgr : BaseMonoMgr<ILRuntimeMgr>
    {
        public AppDomain AppDomain { get; private set; }

        private MemoryStream dll_ms;
        private MemoryStream pdb_ms;

        private string dicPath;

        private string dllPath = "";
        private string pdbPath = "";

        public override void Init()
        {
            base.Init();

            dicPath = string.Empty;
#if UNITY_EDITOR
            dicPath = Application.streamingAssetsPath;
#else
            dicPath = Application.persistentDataPath;
#endif
            StartCoroutine(LoadAssembly());

        }

        private IEnumerator LoadAssembly()
        {
            string _dllPath = Path.Combine(dicPath,dllPath);
            UnityWebRequest loadRequestDll = UnityWebRequest.Get();






        }
    }
}