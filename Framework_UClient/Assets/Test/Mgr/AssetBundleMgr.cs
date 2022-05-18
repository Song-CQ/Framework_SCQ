/****************************************************
    文件: ABMgr.cs
    作者: Clear
    日期: 2022/5/17 14:27:16
    类型: 框架核心脚本(请勿修改)
    功能: AssetBundle 管理器
*****************************************************/
using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public class AssetBundleMgr : BaseMonoMgr<AssetBundleMgr>
    {
        private class LoadAssetBundleInfo
        {
            public AssetBundle m_AssetBundle;
            public int m_referencedCount;

            public LoadAssetBundleInfo(AssetBundle assetBundle)
            {
                m_AssetBundle = assetBundle;
                m_referencedCount = 1;
            }

        }

        //AB包缓存---解决AB包无法重复加载的问题 也有利于提高效率。
        private Dictionary<string, LoadAssetBundleInfo> abInfoCache;
        //加载中的ab包
        private Dictionary<string, Action<AssetBundle>> loadAbCache;
        //主包
        private AssetBundle mainAB = null;
        //主包中配置文件---用以获取依赖包
        private AssetBundleManifest mainManifest = null; 
        //各个平台下的主包名称 --- 用以加载主包获取依赖信息
        private string mainABName
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return "/StandaloneWindows/StandaloneWindows";
#elif UNITY_IOS
                return "/IOS/IOS";
#elif UNITY_ANDROID
                return "/Android/Android";
#endif
            }
        }
        //ab包存放目录
        private string AssetBundlesPath => PathConst.AssetBundlesPath;

        protected override void New()
        {
            base.New();
            //初始化字典
            abInfoCache = new Dictionary<string, LoadAssetBundleInfo>();
        }

        //继承了单例模式提供的初始化函数
        public override void Init()
        {
            base.Init();
            AddListener();
        }

        private  void AddListener()
        {
            AppDispatcher.Instance.AddListener(AppMsg.System_AssetsInitComplete,InitMainPackage);
        }

        private void InitMainPackage(object obj)
        {
            //根据各个平台下的基础路径和主包名加载主包
            mainAB = AssetBundle.LoadFromFile(AssetBundlesPath + mainABName);
            //获取主包下的AssetBundleManifest资源文件（存有依赖信息）
            mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        }



        //加载AB包
        public AssetBundle LoadAssetBundle_FromFile(string abName)
        {
            //加载ab包，需一并加载其依赖包。
            if (mainAB == null)
            {
                LogUtil.LogError("AssetBundle未初始化无法加载");
                return null;
            }
            if (abInfoCache.ContainsKey(abName))
            {
                abInfoCache[abName].m_referencedCount++;
                return abInfoCache[abName].m_AssetBundle;
            }

            AssetBundle bundle = AssetBundle.LoadFromFile(AssetBundlesPath + abName);
            LoadAssetBundleInfo bundleInfo = new LoadAssetBundleInfo(bundle);
            abInfoCache.Add(abName, bundleInfo);

            //根据manifest获取所有依赖包的名称 固定API
            string[] dependencies = mainManifest.GetAllDependencies(abName);
            //循环加载所有依赖包
            foreach (var dp in dependencies)
            {
                LoadAssetBundle_FromFile(dp);
            }
            return abInfoCache[abName].m_AssetBundle;
        }

        public void LoadAssetBundle_FromFileAsync(string abName, Action<AssetBundle> onCom)
        {

            StartCoroutine(OnLoadAssetBundle_FromFileAsync(abName,onCom));
            
        }

        private IEnumerator OnLoadAssetBundle_FromFileAsync(string abName, Action<AssetBundle> onCom)
        {
            //加载ab包，需一并加载其依赖包。
            if (mainAB == null)
            {
                LogUtil.LogError("AssetBundle未初始化无法加载");
                onCom?.Invoke(null);
                yield break;
            }
            if (abInfoCache.ContainsKey(abName))
            {
                abInfoCache[abName].m_referencedCount++;
                onCom?.Invoke(abInfoCache[abName].m_AssetBundle);
                yield break;
            }

            if (loadAbCache.ContainsKey(abName))
            {
                if (onCom!=null)
                {
                    loadAbCache[abName] += onCom;
                }
                yield break;
            }
            
            yield return AssetBundle.LoadFromFileAsync(AssetBundlesPath + abName);
        }


        //====================AB包的两种卸载方式=================
        //单个包卸载
        public void UnLoad(string abName,bool unloadAllLoadedObjects = false)
        {
            if (abInfoCache.ContainsKey(abName))
            {
                abInfoCache[abName].m_referencedCount--;
                if (abInfoCache[abName].m_referencedCount<=0)
                {
                    abInfoCache[abName].m_AssetBundle.Unload(unloadAllLoadedObjects);
                    abInfoCache.Remove(abName);
                    //根据manifest获取所有依赖包的名称 固定API
                    string[] dependencies = mainManifest.GetAllDependencies(abName);
                    //循环卸载所有依赖包
                    foreach (var dp in dependencies)
                    {
                        UnLoad(dp, unloadAllLoadedObjects);
                    } 
                }
            }
        }
    


        //所有包卸载
        public void UnLoadAll()
        {
            AssetBundle.UnloadAllAssetBundles(false);

            abInfoCache.Clear();
            mainAB = null;
            mainManifest = null;
        }

        private void RemoveListener()
        {
            AppDispatcher.Instance.RemoveListener(AppMsg.System_AssetsInitComplete, InitMainPackage);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            UnLoadAll();
            RemoveListener();
        }
    }
}