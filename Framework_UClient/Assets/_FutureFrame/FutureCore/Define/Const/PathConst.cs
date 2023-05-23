/****************************************************
    文件: AbConst.cs
    作者: Clear
    日期: 2022/5/18 11:24:42
    类型: 逻辑脚本
    功能: 路径常量
*****************************************************/
using UnityEngine;

namespace FutureCore
{
    public static class PathConst
    {
        /// <summary>
        ///  Ab包平台
        /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE
        public static string AssetBundlesTarget = "StandaloneWindows";
#elif UNITY_IOS
        public static string AssetBundlesTarget = "IOS";
#elif UNITY_ANDROID
        public static string AssetBundlesTarget = "Android";
#endif

        /// <summary>
        /// 下载缓存目录
        /// </summary>
        public static string DownloadCachePath = Application.temporaryCachePath + "/DownloadCache";

        /// <summary>
        /// AB包存放目录
        /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE
        public static string AssetBundlesPath = Application.persistentDataPath  + "/AssetBundles/" + AssetBundlesTarget;
#elif UNITY_IOS
        public static string AssetBundlesPath = Application.temporaryCachePath + "/AssetBundles/" + AssetBundlesTarget;
#elif UNITY_ANDROID
        public static string AssetBundlesPath = Application.persistentDataPath + "/AssetBundles/" + AssetBundlesTarget;
#endif


#if UNITY_EDITOR
        /// <summary>
        /// AB包编辑器存放目录
        /// </summary>
        public static string EditorAssetBundlesPath = Application.persistentDataPath + "/AssetBundles/" + AssetBundlesTarget;
#endif



        /// <summary>
        /// AB包下载缓存目录
        /// </summary>
        public static string AssetBundleCachePath = DownloadCachePath + "/AssetBundles/" + AssetBundlesTarget;
        
        
        /// <summary>
        /// HotFix存放目录
        /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE
        public static string HotFixPath = Application.persistentDataPath + "/HotFix";
#elif UNITY_IOS
        public static string HotFixPath = Application.temporaryCachePath + "/HotFix";
#elif UNITY_ANDROID
        public static string HotFixPath = Application.persistentDataPath + "/HotFix";
#endif


        
        /// <summary>
        /// HotFix本地目录
        /// </summary>
        public static string HotFixPath_StreamingAssets = Application.streamingAssetsPath + "/HotFix";
        /// <summary>
        /// HotFix缓存目录
        /// </summary>
        public static string HotFixCachePath = DownloadCachePath + "/HotFix";

    }
}