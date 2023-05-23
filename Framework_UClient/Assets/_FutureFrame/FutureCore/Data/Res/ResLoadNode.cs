/****************************************************
    文件: ResLoadNode.cs
    作者: Clear
    日期: 2023/5/17 20:4:24
    类型: 框架核心脚本(请勿修改)
    功能: 资源加载节点
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public class ResLoadNode
    {
        public static ResLoadNode Get()
        {
            return ObjectPoolStatic<ResLoadNode>.Get();
        }

        public delegate void loadCallBack(ResLoadNode node);
        /// <summary>
        /// 加载路径
        /// </summary>
        public string Path { get; private set; } = string.Empty;

        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelaPath { get; private set; } = string.Empty;

        /// 是否加载完成
        /// </summary>
        public bool IsFinish { get; private set; } = false;
        /// <summary>
        /// 是否成功加载
        /// </summary>
        public bool IsSuccess { get; private set; } = false;
        /// <summary>
        /// 是否ab文件
        /// </summary>
        public bool IsABFile { get; private set; } = false;

        public AssetBundle Bundle { get; private set; }
        /// <summary>
        /// 加载的数据 如果是ab文件则为空
        /// </summary>
        public UnityEngine.Object Data { get; private set; }
        /// <summary>
        /// 是否使用Res文件夹下载
        /// </summary>
        public bool IsRes { get; private set; }

        private ResourceRequest resource;

        public List<loadCallBack> loadCallBackLst = new List<loadCallBack>();


        public void Load(string dataRoot, string _relaPath,bool isRes)
        {
            IsFinish = false;
            IsSuccess = false;
            RelaPath = _relaPath;
            Path = dataRoot + "/" + _relaPath;
            IsABFile = RelaPath.IndexOf(".unity3d") != -1;
            IsRes = isRes;

            if (!IsRes)
            {
                //使用正式ab包加载资源
                if (IsABFile)
                    AssetBundleMgr.Instance.LoadAssetBundle_FromFileAsync(Path, ExecOne);
            }
            else
            {
                //使用res加载
                string loadPath = Path;
                int lastIndex = Path.LastIndexOf(".");
                if (lastIndex!=-1)
                {
                    loadPath = Path.Substring(0,lastIndex);
                }
                                
                resource = Resources.LoadAsync(loadPath);
                resource.completed += ResLoadNode_completed;

            }

        }



        public void AddLoadCallBack(loadCallBack callBack)
        {
            if (!loadCallBackLst.Contains(callBack))
            {
                loadCallBackLst.Add(callBack);
            }

        }

        public T GetAsset<T>(string name) where T : UnityEngine.Object
        {
            T val = null;
            if (IsSuccess)
            {
                if (IsABFile)
                {
                    val = Bundle.LoadAsset<T>(name);
                }
                else
                {
                    val = Data as T;
                }
               
            }
            return val;
        }

        public UnityEngine.Object GetAsset(string name)
        {
            UnityEngine.Object val = null;
            if (IsSuccess)
            {
               
                if (IsABFile)
                {
                    val = Bundle.LoadAsset(name);
                }
                else
                {
                    val = Data;
                }
            }
            return val;
        }
        /// <summary>
        /// 加载全部资源 ，只在文件是ab时有效
        /// </summary>
        /// <returns></returns>
        public UnityEngine.Object[] GetAssetAll()
        {
            UnityEngine.Object[] val = null;
            if (IsSuccess)
            {
                if (IsABFile)
                {
                    val = Bundle.LoadAllAssets();
                }
            }
            return val;
        }
        public T[] GetAssetAll<T>() where T : UnityEngine.Object
        {
            T[] val = null;
            if (IsSuccess)
            {
                if (IsABFile)
                {
                    val = Bundle.LoadAllAssets<T>();
                }
            }
            return val;
        }


        private void ExecOne(AssetBundle assetBundle)
        {
            IsFinish = true;
            IsSuccess = assetBundle != null;
            if (assetBundle == null)
            {
                IsFinish = false;
                LogUtil.LogError("未成功加载AB包,请检查路径:" + Path);
            }
            Bundle = assetBundle;

            foreach (var item in loadCallBackLst)
            {
                try
                {
                    item.Invoke(this);
                }
                catch (Exception e)
                {
                    LogUtil.LogError(e);
                }
            }
        }
        private void ResLoadNode_completed(AsyncOperation obj)
        {
            IsFinish = true;
            IsSuccess = resource.isDone&& resource.asset!=null;
            if (!IsSuccess)
            {
                IsFinish = false;
                LogUtil.LogError("未成功加载资源,请检查路径:" + Path);
            }
            if (IsABFile)
            {
                Bundle = resource.asset as AssetBundle;
            }
            Data = resource.asset;
            foreach (var item in loadCallBackLst)
            {
                try
                {
                    item.Invoke(this);
                }
                catch (Exception e)
                {
                    LogUtil.LogError(e);
                }
            }

        }
        public void UnLoad(bool unloadUsedObjects = false)
        {
            AssetBundleMgr.Instance.UnLoad(Path, unloadUsedObjects);
            loadCallBackLst.Clear();
            Path = string.Empty;
            RelaPath = string.Empty;
            IsFinish = false;
            IsSuccess = false;
            IsABFile = true;
            Bundle = null;
            Data = null;
            resource = null;
            ObjectPoolStatic<ResLoadNode>.Release(this);
        }
    }
}