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
using System.Net;
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
       

        public override void Init()
        {
            base.Init();

            InitVersionPath();

        }

        private void InitVersionPath()
        {

#if UNITY_EDITOR

            serverDownload = "http://192.168.31.195/";
#elif UNITY_STANDALONE

#elif UNITY_ANDROID
            
#endif

            ABPath_Server = serverDownload + "/AssetBundles/" + PathConst.AssetBundlesTarget;
            HotFixPath_Server = serverDownload + "/HotFix";




        }

        public void StartUpProcess(Action<bool> initAssets)
        {
            //HttpWebRequest request = null;
            //WebResponse respone = null;
            //string path = @"http://192.168.100.4//AssetBundles/StandaloneWindows\testgood\main1";
            //WebRequest webRequest = WebRequest.Create(path);
            //request = webRequest as HttpWebRequest;
            //if (request == null)
            //{
            //    MainThreadLog.Log("文件不存在:" + path);
                
            //}
            //else
            //{
            //    request.Timeout = 3000;
            //    request.ReadWriteTimeout = 300;
            //    //向服务器请求，获得服务器回应数据流
            //    respone = request.GetResponse();
            //}
            //Debug.Log(respone.ContentLength);
            ////string saveFilePath = Path.Combine(PathConst.HotFixCachePath, "HotFix.dlld");
            ////string downFilePath = Path.Combine(serverDownload, "HotFix/HotFix.dlld");
            //////缓存本地数据
            ////DownloadUnit downloadUnit = new DownloadUnit()
            ////{
            ////    name = "HotFix",
            ////    savePath = saveFilePath,
            ////    downUrl = downFilePath,
            ////    completeFun = (e) => { Debug.LogError("下载完成"); },
            ////    errorFun = (e,msg) => { Debug.LogError(msg);}
            ////};
            ////DownloadTaskMgr.Instance.DownloadAsync(downloadUnit);
            ////return;
            
      
            OnComplete = initAssets;
            
            //本地文件检测
            App.SetLoadingSchedule(ProgressState.AssetsPrepare);
            CheckLocalFile();
           
            //版本云端检测更新
            App.SetLoadingSchedule(ProgressState.VersionUpdate);
            CheckServerUpdate();

        }

        private void OnUpdateComplete(bool isUpdateSucceed)
        {
            LogUtil.Log("[VersionUpdateMgr]版本检测更新完成");
            OnComplete(isUpdateSucceed);
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
                FileUtil.CreadFileLastDirectory(filePath);
                File.Copy(PathConst.HotFixPath_StreamingAssets + "/HotFix.dll", filePath);
            }
            string verifyPath = PathConst.HotFixPath + "/version.json";
            if (!File.Exists(verifyPath))
            {
                FileUtil.CreadFileLastDirectory(filePath);
                File.Copy(PathConst.HotFixPath_StreamingAssets + "/version.json", verifyPath);
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

        private int MaxCheckFixCont = 3;

        private int CheckHotFixCont;
        private int CheckAssetBundleCont;

        private HotFixVerify serverHotFixVerify;
        private AssetBundleVerify serverAssetBundleVerify;

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

        private IEnumerator CheckCoroutine;

        private void CheckServerUpdate()
        {
            isCheckHotFixVerify = false;
            isCheckAssetBundleVerify = false;
            allDownloadUnit.Clear();
            allFileDownloadProgre.Clear();
            serverHotFixVerify = null;
            serverAssetBundleVerify = null;
            doneloadCompleteSum = 0;
            allDoneloadSize = 0;

            CheckServerHotFixVersion();
            CheckServerAssetBundleVersion();

            CheckCoroutine = StartDownload();
            CoroutineMgr.Instance.StartCoroutine(CheckCoroutine);
        }

        private IEnumerator StartDownload()
        {
            while (!isCheckAssetBundleVerify || !isCheckHotFixVerify)
            {
                yield return 0;
            }

            if (!CheckVersionUpdateComplete())
            {
                LogUtil.Log("[VersionUpdateMgr]开始进行文件更新");
                startPro = (int)ProgressState.AssetsPrepare;
                allProgre = (int)ProgressState.AssetsInit - startPro;
                IsUpdate = true;

                foreach (var item in allDownloadUnit)
                {
                    DownloadTaskMgr.Instance.DownloadAsync(item);
                }
            }
            //没有更新进入游戏
            else
            { 
                OnUpdateComplete(true);
            }

        }



        #region HotFix
        
     
        private void CheckServerHotFixVersion()
        {
            //检测
            string verifySeverPath = Path.Combine(HotFixPath_Server,"version.json");
            HttpMgr.Instance.Send(verifySeverPath, GetHotFixVersion);
        }

        private void GetHotFixVersion(bool isError, DownloadHandler download)
        {
            if (isError)
            {
                LogUtil.LogError("获取热更版本信息失败");
                CheckHotFixCont++;
                if (CheckHotFixCont<MaxCheckFixCont)
                {
                    CheckServerHotFixVersion();
                }
                return;
            }

            //获取本地
            HotFixVerify localhotFixVerify = new HotFixVerify();
            string filePath = PathConst.HotFixPath + "/version.json";

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
        private void CheckServerAssetBundleVersion()
        {
            //检测云端
            string verifySeverPath = Path.Combine(ABPath_Server,"version.json");
            HttpMgr.Instance.Send(verifySeverPath, GetAssetBundleVersion);
        }

        private void GetAssetBundleVersion(bool isError, DownloadHandler download)
        {
            if (isError)
            {
                LogUtil.LogError("获取AB包版本信息失败");
                CheckAssetBundleCont++;
                if (CheckAssetBundleCont < MaxCheckFixCont)
                {
                    CheckServerAssetBundleVersion();
                }
                return;
            }

            //获取本地
            AssetBundleVerify localAssetBundleVerify = new AssetBundleVerify();
            string filePath = PathConst.AssetBundlesPath + "/version.json";
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
            serverAssetBundleVerify = JsonUtility.FromJson<AssetBundleVerify>(fileJson);

            //网络版本高 进入更新
            if (serverAssetBundleVerify.version > localAssetBundleVerify.version)
            {
                foreach (var bundleMsg in serverAssetBundleVerify.bagmap)
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
                name = bundleMsg.bagName,
                md5 = bundleMsg.MD5,
                savePath = Path.Combine(PathConst.AssetBundleCachePath, bundleMsg.bagName),
                downUrl = Path.Combine(ABPath_Server, bundleMsg.bagName),
                progressFun = UpdateDownProgre,
                completeFun = DownComplete,
                errorFun = DownError
            };
            allDoneloadSize += bundleMsg.size;
            allFileDownloadProgre.Add(downloadUnit, 0);
            allDownloadUnit.Add(downloadUnit);

            DownloadUnit manifestUnit = new DownloadUnit()
            {
                name = bundleMsg.bagName + ".manifest",
                savePath = Path.Combine(PathConst.AssetBundleCachePath, bundleMsg.bagName + ".manifest"),
                downUrl = Path.Combine(ABPath_Server, bundleMsg.bagName + ".manifest"),
                completeFun = DownComplete,
                errorFun = DownError
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
            LogUtil.Log($"[VersionUpdateMgr]文件:{downUnit.name}下载完成!当前下载数量{doneloadCompleteSum}");
            if (CheckVersionUpdateComplete())
            {
                IsUpdate = false;
                MoveTempFileToOfficial();
            }           
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

        /// <summary>
        /// 检测更新完成
        /// </summary>
        /// <returns></returns>
        private bool CheckVersionUpdateComplete()
        {
            if (doneloadCompleteSum>= allDownloadUnit.Count)
            {
                          
                return true;
            }
            return false;
        }
        /// <summary>
        /// 移动下载好的缓存文件到正式文件夹
        /// </summary>
        private void MoveTempFileToOfficial()
        {
            if (!isCheckHotFixVerify)
            {
                LogUtil.LogError($"[VersionUpdateMgr]HotFix版本没有检测更新");
                return;
            }
            if (!isCheckAssetBundleVerify)
            {
                LogUtil.LogError($"[VersionUpdateMgr]AssetBundle版本没有检测更新");
                return;
            }
            LogUtil.Log($"[VersionUpdateMgr]开始移动临时文件到正式目录");
            foreach (var item in allDownloadUnit)
            {
                if (item.isDownload)
                {
                    string path = string.Empty;
                    if (Path.GetFileName(item.name).Contains("HotFix"))
                    {
                        path = Path.Combine(PathConst.HotFixPath,item.name);
                    }
                    else
                    {
                        path = Path.Combine(PathConst.AssetBundlesPath, item.name);
                    }
                    LogUtil.Log($"[VersionUpdateMgr]开始移动文件{item.savePath}到{path}");
                    FileUtil.CreadFileLastDirectory(path);
                    if (File.Exists(path)) File.Delete(path);
                    File.Move(item.savePath,path);           
                }
            }

            //将网络版本写入本地临时目录
            string _hotFixPath = Path.Combine(PathConst.HotFixPath, "version.json");
            string _hotFixFileJson = JsonUtility.ToJson(serverHotFixVerify);
            FileUtil.WriteAllText(_hotFixPath, _hotFixFileJson);

            //网络版本写入本地临时目录 
            string _assetBundlePath = Path.Combine(PathConst.AssetBundlesPath, "version.json");
            string _assetBundleFileJson = JsonUtility.ToJson(serverAssetBundleVerify);
            FileUtil.WriteAllText(_assetBundlePath, _assetBundleFileJson);

            LogUtil.Log($"[VersionUpdateMgr]版本更新完成");

            //回调
            OnUpdateComplete(true);


        }

       
    }





}