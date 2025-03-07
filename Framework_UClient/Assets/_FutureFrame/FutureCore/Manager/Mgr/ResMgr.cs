using FairyGUI;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace FutureCore
{
    public class ResMgr : BaseMgr<ResMgr>
    {

        public TextAsset GetConfigData(string tableName)
        {
            string path = @"Data\ExcelConfig" + @"\" + tableName;
            TextAsset data = Resources.Load<TextAsset>(path);
            return data;
        }



        #region SyncLoad
        public T LoadLocalRes<T>(string assetPath) where T : Object
        {
            T asset = Resources.Load<T>(assetPath);
            return asset;
        }
        #endregion

        #region AsyncLoad
        public async Task<T> LoadResourcesAsync<T>(string path) where T : UnityEngine.Object
        {
            return await ExtensionsResources.LoadResourcesAsync<T>(path);
        }


        public void LoadRes(string[] assetPath, ResLoadInfo.callback callback, object callbackPara = null, string rootPath = null, bool isRes = false)
        {

            if (rootPath == null)
            {
                rootPath = PathConst.AssetBundlesPath;
            }
            ResLoadInfo info = ObjectPoolStatic<ResLoadInfo>.Get();

            info.Start(assetPath, callback, callbackPara, rootPath, isRes);


        }

        public void LoadUI(string[] UIName, ResLoadInfo.callback callback, object callbackPara = null)
        {

            string rootPath = string.Empty;
            bool IsRes = false;
            if (AppConst.IsUseAssetBundlesLoad)
            {
                IsRes = false;
                for (int i = 0; i < UIName.Length; i++)
                {
                    UIName[i] += ".unity3d";
                }
               
                rootPath = PathConst.AssetBundlesPath + "/";
#if UNITY_EDITOR
                if (!AppConst.IsUseReleaseAB)
                {
                    rootPath = PathConst.EditorAssetBundlesPath;
                }
#endif
            }
            else
            {
                IsRes = true;


            }

            rootPath = rootPath + AppConst.UIDriver.ToString();
            LoadRes(UIName, callback, callbackPara, rootPath, IsRes);
        }
        public void LoadUI(string UIName, ResLoadInfo.callback callback, object callbackPara = null)
        {
            string rootPath = string.Empty;
            bool IsRes = false;
            if (AppConst.IsUseAssetBundlesLoad)
            {
                IsRes = false;
                UIName += ".unity3d";
                rootPath = PathConst.AssetBundlesPath + "/";
#if UNITY_EDITOR
                if (!AppConst.IsUseReleaseAB)
                {
                    rootPath = PathConst.EditorAssetBundlesPath;
                }
#endif
            }
            else
            {
                IsRes = true;
                rootPath = "GUI/";
            }

            rootPath = rootPath + AppConst.UIDriver.ToString();
            LoadRes(new string[] { UIName }, callback, callbackPara, rootPath, IsRes);
        }

        #endregion

        #region Unload

        /// <summary>
        /// 清空加载了的资源的引用
        /// </summary>
        public void UnloadNullReferenceAssets()
        {

        }
        /// <summary>
        /// 清除动态缓存
        /// </summary>
        public void ClearDynamicCache()
        {

        }
        /// <summary>
        /// 释放Gc资源
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public AsyncOperation GCAssets(bool isSystemGC = false)
        {
            // 清理资源之前，一般不需要调用GC
            if (isSystemGC)
            {
                //UnityWebRequest.ClearCookieCache();
                Caching.ClearCache();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return Resources.UnloadUnusedAssets();
        }

        public void InitAssets()
        {
            ////检测版本资源更新

            AppDispatcher.Instance.Dispatch(AppMsg.System_AssetsInitComplete);
        }

        public void AddFguiPackage(string packageName, string PackageUIPath)
        {
            UIPackage.AddPackage(PackageUIPath);
        }

        #endregion

    }
    public static class ExtensionsResources
    {
        public static ResourceRequestAwaiter GetAwaiter(this ResourceRequest request) => new ResourceRequestAwaiter(request);

        public static async Task<T> LoadResourcesAsync<T>(string path) where T : UnityEngine.Object
        {
            var gres = Resources.LoadAsync(path);
            await gres;
            return gres.asset as T;
        }
    }
    public class ResourceRequestAwaiter : INotifyCompletion
    {
        public Action Continuation;
        public ResourceRequest resourceRequest;
        public bool IsCompleted => resourceRequest.isDone;
        public ResourceRequestAwaiter(ResourceRequest resourceRequest)
        {
            this.resourceRequest = resourceRequest;

            //注册完成时的回调
            this.resourceRequest.completed += Accomplish;
        }

        //awati 后面的代码包装成 continuation ，保存在类中方便完成是调用
        public void OnCompleted(Action continuation) => this.Continuation = continuation;

        public void Accomplish(AsyncOperation asyncOperation) => Continuation?.Invoke();

        public void GetResult() { }
    }


 
}

