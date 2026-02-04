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
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace FutureCore
{
    public class SceneSwitchMgr : BaseMonoMgr<SceneSwitchMgr>
    {
        /// <summary>
        /// 是否使用unityApi切换场景
        /// </summary>
        private const bool IsUseUnityScene = true;

        public void SwitchInitialScene(int sceneId, Action<object> LoadComplete, object param)
        {
            StartCoroutine(OnLoadInitialScene(sceneId, LoadComplete, param));

        }
        public void SwitchScene(int idx, Action<object> LoadComplete, object param)
        {
            StartCoroutine(OnLoadScene(idx, LoadComplete, param));
        }
        public void AdditiveScene(int idx, Action<object> LoadComplete, object param,bool isGc = false)
        {
            StartCoroutine(OnAdditiveScene(idx, LoadComplete, param,isGc));
        }

        private IEnumerator OnAdditiveScene(int idx, Action<object> LoadComplete, object param, bool isGc)
        {
            yield return YieldConst.WaitFor100ms;

            if (isGc)
            {
                AsyncOperation asyncGC = ResMgr.Instance.GCAssets(true);
                yield return asyncGC;
            }

            //LogUtil.Log("AB内存快照:", AssetBundleMgr.Instance.GetLoadedABsInfo());


            if (IsUseUnityScene)
            {
                AsyncOperation asyncUnityScene = SceneManager.LoadSceneAsync(idx, LoadSceneMode.Additive);
                yield return asyncUnityScene;
            }

            if (LoadComplete != null)
            {
                LoadComplete(param);
            }
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