/****************************************************
    文件：VersionUpdateMgr.cs
	作者：Clear
    日期：2022/5/4 16:6:35
    类型: 框架核心脚本(请勿修改)
	功能：检测更新资源版本
*****************************************************/
using FutureCore.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace FutureCore
{
    public class VersionUpdateMgr : BaseMonoMgr<VersionUpdateMgr>
    {
        private Action<bool> OnComplete;

        private string ABPath_Server;

        private string HotFixPath_Server;

        private string serverDownload;
        /// <summary>
        /// 本地资源路径
        /// </summary>
        public string LocalAssestUrl = Application.dataPath + "/..";

        public override void Init()
        {
            base.Init();

            InitVersionPath();

        }

        private void InitVersionPath()
        {

#if UNITY_EDITOR

            serverDownload = LocalAssestUrl;
#elif UNITY_STANDALONE

#elif UNITY_ANDROID
            
#endif

            ABPath_Server = serverDownload + "/AssetBundles/" + PathConst.AssetBundlesTarget;
            HotFixPath_Server = serverDownload + "/HotFix";




        }

        public void StartUpProcess(Action<bool> initAssets)
        {
            //检测资源加载
            OnComplete = initAssets;

            //本地文件检测
            App.SetLoadingSchedule(ProgressState.AssetsPrepare);
            CheckLocalFile();
           
            //版本云端检测更新
            App.SetLoadingSchedule(ProgressState.VersionUpdate);
            CheckServerUpdate();

        }

        #region 本地文件检测

        private void CheckLocalFile()
        {
            if (AppConst.IsDevelopMode)
            {
                return;
            }
            CheckLocalHotFixFile();
            CheckLocalAssetsFile();
            LogUtil.Log("[VersionUpdateMgr]检测本地文件完成");
        }

        private void CheckLocalHotFixFile()
        {
            if (!AppConst.IsHotUpdateMode)
            {
                return;
            }
            string filePath = PathConst.HotFixPath + "/HotFix.dll";
            if (!File.Exists(filePath))
            {
                File.Copy(PathConst.HotFixPath_StreamingAssets + "/HotFix.dll", filePath);
            }
            string verifyPath = PathConst.HotFixPath + "/verify.json";
            if (!File.Exists(verifyPath))
            {
                File.Copy(PathConst.HotFixPath_StreamingAssets + "/verify.json", verifyPath);
            }

        }

        private void CheckLocalAssetsFile()
        {
            if (!AppConst.IsAssetBundlesUpdateMode)
            {
                return;
            }
            string filePath = PathConst.AssetBundlesPath;
            if (!Directory.Exists(filePath))
            {
                string streamingAssetsPath = Application.streamingAssetsPath + "/" + PathConst.AssetBundlesTarget;
                if (Directory.Exists(streamingAssetsPath))
                {
                    FileUtil.CopyFolder(streamingAssetsPath, filePath);
                }
                else
                {
                    Directory.CreateDirectory(filePath);
                }

            }




        }
        #endregion


        #region 服务器更新检测

        private HotFixVerify serverHotFixVerify;
        private AssetBundleVerify assetBundleVerify;

        private List<DownloadUnit> allDownloadUnit = new List<DownloadUnit>();
        /// <summary>
        /// 下载进度
        /// </summary>
        private Dictionary<DownloadUnit, long> allFileDownloadProgre = new Dictionary<DownloadUnit, long>();

        private bool isCheckHotFixVerify = false;
        private bool isCheckAssetBundleVerify = false;
     
        /// <summary>
        /// 当前已经完成下载的文件数量
        /// </summary>
        private int doneloadCompleteSum;
        
        /// <summary>
        /// 全部下载的大小
        /// </summary>
        private long allDoneloadSize;

        private int startPro;
        private int allProgre;
        private bool IsUpdate = false;

        private void CheckServerUpdate()
        {
            isCheckHotFixVerify = false;
            isCheckAssetBundleVerify = false;
            allDownloadUnit.Clear();
            allFileDownloadProgre.Clear();
            serverHotFixVerify = null;
            assetBundleVerify = null;
            doneloadCompleteSum = 0;
            allDoneloadSize = 0;

            CheckServerHotFixVerify();
            CheckServerAssetBundleVerify();
            StartDownload();
        }

        private void StartDownload()
        {
            if (!CheckVersion())
            {
                startPro = (int)ProgressState.AssetsPrepare;
                allProgre = (int)ProgressState.AssetsInit - startPro;
                IsUpdate = true;

                foreach (var item in allDownloadUnit)
                {
                    DownloadTaskMgr.Instance.DownloadAsync(item);
                }
            }

        }

        #region HotFix
        private void CheckServerHotFixVerify()
        {
            //检测
            string verifySeverPath = Path.Combine(HotFixPath_Server + "verify.json");
            HttpMgr.Instance.Send(verifySeverPath, GetHotFixVersion);
        }

        private void GetHotFixVersion(bool isError, DownloadHandler download)
        {
            if (isError)
            {
                LogUtil.LogError("获取热更版本信息失败");

                return;
            }

            //获取本地
            HotFixVerify localhotFixVerify = new HotFixVerify();
            string filePath = PathConst.HotFixPath + "/verify.json";

            if (File.Exists(filePath))
            {
                string LocaFileJson = File.ReadAllText(filePath);
                HotFixVerify _hotFixVerify = JsonUtility.FromJson<HotFixVerify>(LocaFileJson);
                if (_hotFixVerify != null)
                {
                    localhotFixVerify = _hotFixVerify;
                }
            }

            //得到网络版本
            string fileJson = download.text;
            serverHotFixVerify = JsonUtility.FromJson<HotFixVerify>(fileJson);

            //网络版本高 进入更新
            if (serverHotFixVerify.version > localhotFixVerify.version)
            {
                if (serverHotFixVerify.MD5 != localhotFixVerify.MD5)
                {
                    //网络版本写入本地临时目录 在所有下载完后移入正式目录
                    string cachePath = Path.Combine(PathConst.HotFixCachePath, "verify.json");
                    FileUtil.WriteAllText(cachePath, fileJson);

                    string saveFilePath = Path.Combine(PathConst.HotFixCachePath, "HotFix.dll");
                    string downFilePath = Path.Combine(serverDownload, "HotFix/HotFix.dll");
                    //缓存本地数据
                    DownloadUnit downloadUnit = new DownloadUnit()
                    {
                        name = "HotFix",
                        md5 = serverHotFixVerify.MD5,
                        savePath = saveFilePath,
                        downUrl = downFilePath,
                        completeFun = DownComplete,
                        errorFun = DownError
                    };
                    allDownloadUnit.Add(downloadUnit);
                    allDoneloadSize += serverHotFixVerify.size;
                    allFileDownloadProgre.Add(downloadUnit, 0);
                    
                }
            }

            isCheckHotFixVerify = true;
        }
        #endregion

        #region AssetBundle
        private void CheckServerAssetBundleVerify()
        {
            //检测云端
            string verifySeverPath = Path.Combine(ABPath_Server + "verify.json");
            HttpMgr.Instance.Send(verifySeverPath, GetAssetBundleVersion);
        }

        private void GetAssetBundleVersion(bool isError, DownloadHandler download)
        {
            if (isError)
            {
                LogUtil.LogError("获取AB包版本信息失败");

                return;
            }

            //获取本地
            AssetBundleVerify localAssetBundleVerify = new AssetBundleVerify();
            string filePath = PathConst.AssetBundlesPath + "/verify.json";
            if (File.Exists(filePath))
            {
                string LocaFileJson = File.ReadAllText(filePath);
                AssetBundleVerify _hotFixVerify = JsonUtility.FromJson<AssetBundleVerify>(LocaFileJson);
                if (_hotFixVerify != null)
                {
                    localAssetBundleVerify = _hotFixVerify;
                }
            }

            //得到网络版本
            string fileJson = download.text;
            assetBundleVerify = JsonUtility.FromJson<AssetBundleVerify>(fileJson);

            //网络版本高 进入更新
            if (assetBundleVerify.version > localAssetBundleVerify.version)
            {
                //网络版本写入本地临时目录 在所有下载完后移入正式目录
                string tempPath = Path.Combine(PathConst.AssetBundleCachePath, "verify.json");
                FileUtil.WriteAllText(tempPath, fileJson);

                foreach (var bundleMsg in assetBundleVerify.bagmap)
                {
                    bool isNeedUpdata = true;
                    foreach (var loadbundleMsg in localAssetBundleVerify.bagmap)
                    {
                        if (loadbundleMsg.bagName == bundleMsg.bagName)
                        {
                            if (loadbundleMsg.MD5 == bundleMsg.MD5)
                            {
                                //相同跳过
                                isNeedUpdata = false;
                                break;
                            }
                        }
                    }

                    if (isNeedUpdata)
                    {
                        SetAssetBundleDownladUnit(bundleMsg);
                    }
                  
                }

               
            }

            isCheckAssetBundleVerify = true;
        }

        private void SetAssetBundleDownladUnit(BundleMsg bundleMsg)
        {
            //缓存本地数据
            DownloadUnit downloadUnit = new DownloadUnit()
            {
                name = "HotFix",
                md5 = bundleMsg.MD5,
                savePath = Path.Combine(PathConst.AssetBundleCachePath, bundleMsg.bagName),
                downUrl = Path.Combine(ABPath_Server, bundleMsg.bagName),
                progressFun = UpdateDownProgre,
                completeFun = DownComplete,
                errorFun = DownError
            };
            allDoneloadSize += serverHotFixVerify.size;
            allFileDownloadProgre.Add(downloadUnit, 0);

            DownloadUnit manifestUnit = new DownloadUnit()
            {
                name = bundleMsg.bagName + ".manifest",
                savePath = Path.Combine(PathConst.AssetBundleCachePath, bundleMsg.bagName + ".manifest"),
                downUrl = Path.Combine(ABPath_Server, bundleMsg.bagName + ".manifest"),
            };
            allDownloadUnit.Add(manifestUnit);
        }

        #endregion
        private void UpdateDownProgre(DownloadUnit downUnit, int curSize, int allSize)
        {
            if (allFileDownloadProgre.ContainsKey(downUnit))
            {
                allFileDownloadProgre[downUnit] = curSize;
            }
        }

        private void DownComplete(DownloadUnit downUnit)
        {
            doneloadCompleteSum++;
            if (allFileDownloadProgre.ContainsKey(downUnit))
            {
                allFileDownloadProgre[downUnit] = downUnit.size;
            }

            CheckVersion();
        }

        private void DownError(DownloadUnit downUnit,string msg)
        {
            LogUtil.LogError($"[VersionUpdateMgr]文件{downUnit.name}下载失败:{msg}");
        }
        
        #endregion

        public void Update()
        {
            if (!IsUpdate)
            {
                return;
            }
            long val = 0;
            foreach (var item in allFileDownloadProgre)
            {
                val += item.Value;
            }
            float Progre = val * 1f / allDoneloadSize;
            int ProgreVal = startPro + (int)(allProgre * Progre);       
            GenericDispatcher.Instance.Dispatch<int, Action>(AppMsg.UI_SetLoadingValueUI,ProgreVal,null);
        }

        private bool CheckVersion()
        {
            if (doneloadCompleteSum>= allDownloadUnit.Count)
            {
                IsUpdate = false;
                LogUtil.Log("[VersionUpdateMgr]版本检测更新完成");
                OnComplete.Invoke(true);
                return true;
            }
            return false;
        }

    }





}