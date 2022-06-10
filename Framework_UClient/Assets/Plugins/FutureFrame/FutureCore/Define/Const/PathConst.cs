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
        /// AB包存放目录
        /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE
        public static string AssetBundlesPath = Application.persistentDataPath + "/AssetBundles";
#elif UNITY_IOS
        public static string AssetBundlesPath = Application.temporaryCachePath + "/AssetBundles";
#elif UNITY_ANDROID
        public static string AssetBundlesPath = Application.persistentDataPath + "/AssetBundles";
#endif

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
        /// AB包下载缓存目录
        /// </summary>
        public static string AssetBundleCachePath = Application.temporaryCachePath + "/LoadABCache/";
        
        
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
        /// HotFix下载缓存目录
        /// </summary>
        public static string HotFixCachePath = Application.temporaryCachePath + "/Cache/HotFix/";
        /// <summary>
        /// HotFix本地目录
        /// </summary>
        public static string HotFixPath_StreamingAssets = Application.streamingAssetsPath + "/HotFix/";


    }
}