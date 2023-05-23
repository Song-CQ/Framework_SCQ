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
        private enum ABState
        {
            /// <summary>
            /// 未加载
            /// </summary>
            NotLoaded = 0,
            /// <summary>
            /// 加载中
            /// </summary>
            Loading,
            /// <summary>
            /// 已加载
            /// </summary>
            Loaded,
            /// <summary>
            /// 卸载中(如果ab包正在加载，进行卸载，为卸载中状态)
            /// </summary>
            Unloading,
            /// <summary>
            /// 已卸载
            /// </summary>
            Unloaded,
        }

        private class AssetBundleInfo
        {
            public string m_ABName;
            public string[] m_Dependencies;
            public AssetBundle m_AssetBundle;
            private int m_referencedCount;
            public ABState state;

            private Action unLoadAction;

            public event Action<AssetBundle> OnLoadingCompleted;

            public AssetBundleInfo(string abName, string[] dependencies)
            {
                m_ABName = abName;
                m_Dependencies = dependencies;

                m_referencedCount = 0;
                state = ABState.NotLoaded;
            }
            /// <summary>
            /// 添加资源引用
            /// </summary>
            public void AddReferencedCount()
            {
                m_referencedCount++;
            }
            /// <summary>
            /// 移除资源引用
            /// </summary>
            public void RomveReferencedCount()
            {
                m_referencedCount--;
            }

            public void LoadingABCompleted(AssetBundle assetBundle)
            {
                if (state == ABState.Unloading)
                {
                    unLoadAction?.Invoke();
                    state = ABState.Unloaded;
                }
                if (state != ABState.Loading)
                {
                    return;
                }

                m_AssetBundle = assetBundle;
                state = ABState.Loaded;

                foreach (Action<AssetBundle> dele in OnLoadingCompleted.GetInvocationList())
                {
                    if (dele != null)
                    {
                        try
                        {
                            dele.Invoke(m_AssetBundle);
                        }
                        catch (Exception e)
                        {
                            LogUtil.LogError($"加载Ab包完成，回调失败{e}");
                        }
                    }
                }
                OnLoadingCompleted = null;
            }

            public void UnLoad(bool unloadAllLoadedObjects = false)
            {
                if (state == ABState.NotLoaded || state == ABState.Unloaded || state == ABState.Unloading)
                {
                    LogUtil.Log($"该资源包{m_ABName}未加载或已卸载或卸载中");
                    return;
                }
                if (m_referencedCount <= 0)
                {
                    if (state == ABState.Loaded)
                    {
                        m_AssetBundle.Unload(unloadAllLoadedObjects);
                        state = ABState.Unloaded;
                    }
                    else if (state == ABState.Loading)
                    {
                        state = ABState.Unloading;
                        unLoadAction = () =>
                        {
                            m_AssetBundle.Unload(unloadAllLoadedObjects);
                            state = ABState.Unloaded;
                        };
                    }
                    UnLoadData();
                }
            }

            private void UnLoadData()
            {
                m_referencedCount = 0;
                m_AssetBundle = null;
                OnLoadingCompleted = null;
            }

            public void Rest()
            {
                unLoadAction = null;
                m_referencedCount = 0;
                state = ABState.NotLoaded;
                m_AssetBundle = null;
                OnLoadingCompleted = null;
            }
        }



        //AB包缓存---解决AB包无法重复加载的问题 也有利于提高效率。
        private Dictionary<string, AssetBundleInfo> abInfoCache;

        //主包中配置文件---用以获取依赖包
        private AssetBundleManifest mainManifest = null;
        //各个平台下的主包名称 --- 用以加载主包获取依赖信息
        private string MainABName => PathConst.AssetBundlesTarget;
     
        //ab包存放目录
        private string AssetBundlesPath = PathConst.AssetBundlesPath;


        protected override void New()
        {
            base.New();
            //初始化字典
            abInfoCache = new Dictionary<string, AssetBundleInfo>();
        }

        //继承了单例模式提供的初始化函数
        public override void Init()
        {
            base.Init();
            AddListener();
        }

        private void AddListener()
        {
            AppDispatcher.Instance.AddListener(AppMsg.System_AssetsInitComplete, InitMainPackage);
        }

        private void InitMainPackage(object obj)
        {
#if UNITY_EDITOR
            if (!AppConst.IsUseReleaseAB)
            {
                AssetBundlesPath = PathConst.EditorAssetBundlesPath;
            }
#endif

            //根据各个平台下的基础路径和主包名加载主包
            AssetBundle mainAB = AssetBundle.LoadFromFile($"{AssetBundlesPath}/{MainABName}");
            //获取主包下的AssetBundleManifest资源文件（存有依赖信息）
            mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            mainAB.Unload(false);
        }



        //加载AB包
        public AssetBundle LoadAssetBundle_FromFile(string abName)
        {

            if (mainManifest == null)
            {
                LogUtil.LogError("AssetBundle未初始化无法加载");
                return null;
            }
            //没有缓存则新建
            if (!abInfoCache.TryGetValue(abName, out AssetBundleInfo bundleInfo))
            {
                string[] _dependencies = mainManifest.GetAllDependencies(abName);
                bundleInfo = new AssetBundleInfo(abName, _dependencies);
                abInfoCache.Add(abName, bundleInfo);
            }
            //同步加载 尽量不要和异步加载混用 不然可能会造成计数器异常
            if (bundleInfo.state == ABState.Loading || bundleInfo.state == ABState.Unloading)
            {
                LogUtil.LogError("当前包正在异步加载或卸载，请不要调用同步加载");
                return null;
            }
            //如果是卸载包则 重置卸载
            if (bundleInfo.state == ABState.Unloaded)
            {
                bundleInfo.Rest();
            }
            //如果未加载则  加载ab包
            if (bundleInfo.state == ABState.NotLoaded)
            {
                bundleInfo.state = ABState.Loading;
                AssetBundle bundle = AssetBundle.LoadFromFile(AssetBundlesPath + abName);
                if (bundle == null)
                {
                    Debug.LogError($"没有{abName}的ab包");
                    abInfoCache.Remove(abName);
                    return null;
                }
                bundleInfo.LoadingABCompleted(bundle);
            }
            //添加计数引用 
            if (bundleInfo.state == ABState.Loaded)
            {
                bundleInfo.AddReferencedCount();
                //加载引用包
                foreach (var dp in bundleInfo.m_Dependencies)
                {
                    LoadAssetBundle_FromFile(dp);
                }
            }

            return abInfoCache[abName].m_AssetBundle;
        }

        public void LoadAssetBundle_FromFileAsync(string abName, Action<AssetBundle> onCom)
        {
            StartCoroutine(OnLoadAssetBundle_FromFileAsync(abName, onCom));

        }

        private IEnumerator OnLoadAssetBundle_FromFileAsync(string abName, Action<AssetBundle> onCom)
        {
            //加载ab包，需一并加载其依赖包。
            if (mainManifest == null)
            {
                LogUtil.LogError("AssetBundle未初始化无法加载");
                onCom?.Invoke(null);
                yield break;
            }
            if (!abInfoCache.TryGetValue(abName, out AssetBundleInfo bundleInfo))
            {
                string[] _dependencies = mainManifest.GetAllDependencies(abName);
                bundleInfo = new AssetBundleInfo(abName, _dependencies);
                abInfoCache.Add(abName, bundleInfo);
            }
            //如果已经加载
            if (bundleInfo.state == ABState.Loaded)
            {
                bundleInfo.AddReferencedCount();
                if (onCom != null)
                {
                    onCom?.Invoke(bundleInfo.m_AssetBundle);
                }
                foreach (var dpName in bundleInfo.m_Dependencies)
                {
                    abInfoCache[dpName].AddReferencedCount();
                }
                yield break;
            }

            if (bundleInfo.state == ABState.Unloading || bundleInfo.state == ABState.Loading)
            {
                //如果正在卸载 则取消卸载
                if (bundleInfo.state == ABState.Unloading)
                {
                    //重置
                    bundleInfo.Rest();
                    //设置加载状态
                    bundleInfo.state = ABState.Loading;
                }
                //加载合并
                bundleInfo.AddReferencedCount();
                if (onCom != null)
                {
                    bundleInfo.OnLoadingCompleted += onCom;
                }
                //增加引用计数 先增加避免包因为另一个引用包 导致计数成0导致卸载
                foreach (var dpName in bundleInfo.m_Dependencies)
                {
                    if (!abInfoCache.ContainsKey(dpName))
                    {
                        string[] _dependencies = mainManifest.GetAllDependencies(dpName);
                        bundleInfo = new AssetBundleInfo(dpName, _dependencies);
                        abInfoCache.Add(dpName, bundleInfo);
                    }
                   
                    abInfoCache[dpName].AddReferencedCount();
                }
                yield break;
            }

            //如果是卸载包则 重置卸载
            if (bundleInfo.state == ABState.Unloaded)
            {
                bundleInfo.Rest();
            }
            if (bundleInfo.state == ABState.NotLoaded)
            {
                bundleInfo.state = ABState.Loading;
                AssetBundleCreateRequest bundleCreateRequest = AssetBundle.LoadFromFileAsync(AssetBundlesPath + abName);
                yield return bundleCreateRequest;
                AssetBundle assetBundle = bundleCreateRequest.assetBundle;
                if (assetBundle == null)
                {
                    Debug.LogError($"没有{abName}的ab包");
                    abInfoCache.Remove(abName);
                    yield break;
                }
                // 异步加载依赖包
                foreach (var dpName in bundleInfo.m_Dependencies)
                {
                    yield return StartCoroutine(OnLoadAssetBundle_FromFileAsync(dpName,null));
                }
                bundleInfo.LoadingABCompleted(assetBundle);
            }         

        }


        //====================AB包的两种卸载方式=================
        //单个包卸载
        public void UnLoad(string abName, bool unloadAllLoadedObjects = false)
        {
            if (abInfoCache.ContainsKey(abName))
            {
                abInfoCache[abName].RomveReferencedCount();
                abInfoCache[abName].UnLoad(unloadAllLoadedObjects);
            }
        }



        //所有包卸载
        public void UnLoadAll()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            abInfoCache.Clear();

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