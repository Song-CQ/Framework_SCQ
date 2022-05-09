using FairyGUI;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace FutureCore
{
    public class ResMgr:BaseMgr<ResMgr>
    {

        public TextAsset GetExcelData(string tableName)
        {
            string path = @"Data\ExcelConfig"+@"\"+tableName;
            TextAsset data = Resources.Load<TextAsset>(path);
            return data;
        }

       

        #region SyncLoad
        public T SyncLoad<T>(string assetPath) where T : Object
        {
            T asset = Resources.Load<T>(assetPath);
            return asset;
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
}