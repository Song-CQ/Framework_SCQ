/****************************************************
    文件：SceneSwitchMgr.cs
	作者：Clear
    日期：2022/1/11 15:35:21
    类型: 框架核心脚本(请勿修改)
	功能：切换场景管理器（释放资源）
*****************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FutureCore
{
    public class SceneSwitchMgr : BaseMonoMgr<SceneSwitchMgr>
    {
        /// <summary>
        /// 是否使用unityApi切换场景
        /// </summary>
        private const bool IsUseUnityScene = false;

        public void SwitchInitialScene(int sceneId, Action<object> LoadComplete, object param)
        {
            StartCoroutine(OnLoadInitialScene(sceneId, LoadComplete, param));

        }
        public void SwitchScene(int idx, Action<object> LoadComplete, object param)
        {
            StartCoroutine(OnLoadScene(idx, LoadComplete, param));
        }

        

        private IEnumerator OnLoadInitialScene(int sceneId, Action<object> loadComplete, object param)
        {
            yield return YieldConst.Time10ms;

            if (loadComplete != null)
            {
                loadComplete(param);
            }
        }

        private IEnumerator OnLoadScene(int idx, Action<object> loadComplete, object param)
        {
            yield return YieldConst.WaitFor100ms;

            ResMgr.Instance.UnloadNullReferenceAssets();
            ResMgr.Instance.ClearDynamicCache();

            AsyncOperation asyncGC = ResMgr.Instance.GCAssets(true);
            yield return asyncGC;

   //       LogUtil.Log("AB内存快照:", AssetBundleMgr.Instance.GetLoadedABsInfo());

   
            if (IsUseUnityScene)
            {
                AsyncOperation asyncUnityScene = SceneManager.LoadSceneAsync(idx, LoadSceneMode.Single);
                yield return asyncUnityScene;
            }

            if (loadComplete != null)
            {
                loadComplete(param);
            }
        }

    }
}