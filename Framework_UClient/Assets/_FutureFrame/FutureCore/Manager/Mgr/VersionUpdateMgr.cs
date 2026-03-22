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
using static FutureCore.ConfigDataVerify;

namespace FutureCore
{
    public class VersionUpdateMgr : BaseMonoMgr<VersionUpdateMgr>
    {
        private Action<bool> OnComplete;
        private string serverDownload;


        private string AssetBundlePath_Server;
        private string ConfigDataPath_Server;
        private string HotFixPath_Server;

        private string AssetBundlePath_Cache;
        private string ConfigDataPath_Cache;
        private string HotFixPath_Cache;

        private string AssetBundlePath_Local;
        private string ConfigDataPath_Local;
        private string HotFixPath_Local;




        public override void Init()
        {
            base.Init();

            InitVersionPath();

        }

        private void InitVersionPath()
        {

#if UNITY_EDITOR
            serverDownload = "https://raw.githubusercontent.com/Song-CQ/UnityRes/main";
            serverDownload = "https://gitee.com/Song_CQ/match3_game/raw/master/UpData";
#else
            serverDownload = AppFacade_Frame.ServerAssestUrl;
#endif

            AssetBundlePath_Server = serverDownload + "/AssetBundles/" + PathConst.AssetBundlesTarget;
            HotFixPath_Server = serverDownload + "/HotFix";
            ConfigDataPath_Server = serverDownload + "/ConfigData";

            AssetBundlePath_Cache = Path.Combine(PathConst.AssetBundleCachePath);
            HotFixPath_Cache = Path.Combine(PathConst.HotFixCachePath);
            ConfigDataPath_Cache = Path.Combine(PathConst.ConfigDataCachePath);

            AssetBundlePath_Local = Path.Combine(PathConst.AssetBundlesPath);
            HotFixPath_Local = Path.Combine(PathConst.HotFixPath);
            ConfigDataPath_Local = Path.Combine(PathConst.ConfigDataPath);




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
            UpdataAllVerify();

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
            CheckLocalConfigData();
            LogUtil.Log("[VersionUpdateMgr]检测本地文件完成");
        }

        private void CheckLocalHotFixFile()
        {
            if (AppConst.HotUpdateType == HotUpdateType.None)
            {
                return;
            }

            if (AppConst.HotUpdateType == HotUpdateType.ILRuntime)
            {
                string filePath = HotFixPath_Local + "/HotFix.dll";
                if (!File.Exists(filePath))
                {
                    FileUtil.CreadFileLastDirectory(filePath);
                    File.Copy(Application.streamingAssetsPath + "/HotFix.dll", filePath);
                }
                string verifyPath = HotFixPath_Local + "/version.json";
                if (!File.Exists(verifyPath))
                {
                    FileUtil.CreadFileLastDirectory(filePath);
                    File.Copy(Application.streamingAssetsPath + "/version.json", verifyPath);
                }
            }
            else if (AppConst.HotUpdateType == HotUpdateType.XLua)
            {

            }


        }



        private void CheckLocalAssetsFile()
        {
            if (!AppConst.IsCheckResVer)
            {
                return;
            }
            string filePath = AssetBundlePath_Local;
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

        private void CheckLocalConfigData()
        {
            if (!AppConst.IsCheckConfigDataVer)
            {
                return;
            }
            string filePath = ConfigDataPath_Local;
            if (!Directory.Exists(filePath))
            {
                string streamingAssetsPath = Application.streamingAssetsPath + "/ConfigData";
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
        private int CheckConfigDataCont;

        private HotFixVerify serverHotFixVerify;
        private AssetBundleVerify serverAssetBundleVerify;
        private ConfigDataVerify serverConfigDataVerify;

        private List<DownloadUnit> allDownloadUnit = new List<DownloadUnit>();
        /// <summary>
        /// 下载进度
        /// </summary>
        private Dictionary<DownloadUnit, long> allFileDownloadProgre = new Dictionary<DownloadUnit, long>();

        private bool isCheckHotFixVerify = false;
        private bool isCheckAssetBundleVerify = false;
        private bool isCheckConfigDataVerify = false;

        private bool AssetBundle_Updata;
        private bool ConfigData_Updata;
        private bool HotFix_Updata;

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
            isCheckConfigDataVerify = false;

            AssetBundle_Updata = false;
            ConfigData_Updata = false;
            HotFix_Updata = false;

            allDownloadUnit.Clear();
            allFileDownloadProgre.Clear();

            serverHotFixVerify = null;
            serverAssetBundleVerify = null;
            serverConfigDataVerify = null;

            doneloadCompleteSum = 0;
            allDoneloadSize = 0;

            CheckServerHotFixVersion();
            CheckServerAssetBundleVersion();
            CheckServerConfigDataVersion();

            CheckCoroutine = StartDownload();
            CoroutineMgr.Instance.StartCoroutine(CheckCoroutine);
        }

        private IEnumerator StartDownload()
        {
            while (!isCheckAssetBundleVerify || !isCheckHotFixVerify || !isCheckConfigDataVerify)
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
            if (AppConst.HotUpdateType == HotUpdateType.None)
            {

                isCheckHotFixVerify = true;
                return;
            }
            //检测
            string verifySeverPath = Path.Combine(HotFixPath_Server, "version.json");
            HttpMgr.Instance.Send(verifySeverPath, GetHotFixVersion);
        }

        private void GetHotFixVersion(bool isError, DownloadHandler download)
        {
            if (isError)
            {
                LogUtil.LogError("获取热更版本信息失败");
                CheckHotFixCont++;
                if (CheckHotFixCont < MaxCheckFixCont)
                {
                    CheckServerHotFixVersion();
                }
                return;
            }

            //获取本地
            HotFixVerify localhotFixVerify = new HotFixVerify();
            string filePath = HotFixPath_Local + "/version.json";

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

            LogUtil.Log("HotFixVerify：本地版本：" + localhotFixVerify.version + "网络版本：" + serverHotFixVerify.version);

            //网络版本高 进入更新
            if (serverHotFixVerify.version > localhotFixVerify.version)
            {
                if (serverHotFixVerify.MD5 != localhotFixVerify.MD5)
                {
                    string saveFilePath = Path.Combine(HotFixPath_Cache, "HotFix.dll");
                    string downFilePath = Path.Combine(serverDownload, "HotFix/HotFix.dll");
                    //缓存本地数据
                    DownloadUnit downloadUnit = new DownloadUnit()
                    {
                        name = "HotFix",
                        md5 = serverHotFixVerify.MD5,
                        savePath = saveFilePath,
                        downUrl = downFilePath,
                        completeFun = DownComplete,
                        progressFun = UpdateDownProgre,
                        errorFun = DownError
                    };
                    allDownloadUnit.Add(downloadUnit);
                    allDoneloadSize += serverHotFixVerify.size;
                    allFileDownloadProgre.Add(downloadUnit, 0);

                }

                HotFix_Updata = true;
            }

            isCheckHotFixVerify = true;
        }
        #endregion

        #region AssetBundle
        private void CheckServerAssetBundleVersion()
        {
            if (!AppConst.IsCheckResVer)
            {
                isCheckAssetBundleVerify = true;
                return;
            }
            //检测云端
            string verifySeverPath = Path.Combine(AssetBundlePath_Server, "version.json");
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
            string filePath = AssetBundlePath_Local + "/version.json";
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

            LogUtil.Log("AssetBundleVerify：本地版本：" + localAssetBundleVerify.version + "网络版本：" + serverAssetBundleVerify.version);

            //网络版本高 进入更新
            if (serverAssetBundleVerify.version > localAssetBundleVerify.version)
            {
                foreach (var bundleMsg in serverAssetBundleVerify.bagmap)
                {
                    bool isNeedUpdata = true;
                    //检查本地文件是否和远端的相同 相同就不用更新
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

                AssetBundle_Updata = true;


            }

            isCheckAssetBundleVerify = true;
        }

        private void SetAssetBundleDownladUnit(BundlMsg bundleMsg)
        {
            //缓存本地数据
            DownloadUnit downloadUnit = new DownloadUnit()
            {
                name = bundleMsg.bagName,
                md5 = bundleMsg.MD5,
                savePath = Path.Combine(AssetBundlePath_Cache, bundleMsg.bagName),
                downUrl = Path.Combine(AssetBundlePath_Server, bundleMsg.bagName),
                progressFun = UpdateDownProgre,
                completeFun = DownComplete,
                errorFun = DownError
            };
            allDownloadUnit.Add(downloadUnit);
            allDoneloadSize += bundleMsg.size;
            allFileDownloadProgre.Add(downloadUnit, 0);

            DownloadUnit manifestUnit = new DownloadUnit()
            {
                name = bundleMsg.bagName + ".manifest",
                savePath = Path.Combine(AssetBundlePath_Cache, bundleMsg.bagName + ".manifest"),
                downUrl = Path.Combine(AssetBundlePath_Server, bundleMsg.bagName + ".manifest"),
                completeFun = DownComplete,
                progressFun = UpdateDownProgre,
                errorFun = DownError
            };
            allDownloadUnit.Add(manifestUnit);
            allDoneloadSize += manifestUnit.size;
            allFileDownloadProgre.Add(manifestUnit, 0);

        }

        #endregion

        #region ConfigData
        private void CheckServerConfigDataVersion()
        {
            if (!AppConst.IsCheckConfigDataVer)
            {
                isCheckConfigDataVerify = true;
                return;
            }
            //检测云端
            string verifySeverPath = Path.Combine(ConfigDataPath_Server, "ConfigDataVerify.json");
            HttpMgr.Instance.Send(verifySeverPath, GetConfigDataVersion);
        }

        private void GetConfigDataVersion(bool isError, DownloadHandler download)
        {
            if (isError)
            {
                LogUtil.LogError("获取配置表版本信息失败");
                CheckAssetBundleCont++;
                if (CheckAssetBundleCont < MaxCheckFixCont)
                {
                    CheckServerAssetBundleVersion();
                }
                return;
            }

            //获取本地
            ConfigDataVerify localConfigDataVerify = new ConfigDataVerify();
            string filePath = ConfigDataPath_Local + "/ConfigDataVerify.json";
            if (File.Exists(filePath))
            {
                string LocaFileJson = File.ReadAllText(filePath);
                ConfigDataVerify _hotFixVerify = SerializeUtil.UnityToObject<ConfigDataVerify>(LocaFileJson);
                if (_hotFixVerify != null)
                {
                    localConfigDataVerify = _hotFixVerify;
                }
            }

            //得到网络版本
            string fileJson = download.text;

            serverConfigDataVerify = SerializeUtil.UnityToObject<ConfigDataVerify>(fileJson);
            LogUtil.Log("ConfigDataVerify：本地版本：" + localConfigDataVerify.version + "网络版本：" + serverConfigDataVerify.version);

            //网络版本高 进入更新
            if (serverConfigDataVerify.version > localConfigDataVerify.version)
            {
                foreach (var serverFilesMsg in serverConfigDataVerify.files)
                {
                    bool isNeedUpdata = true;

                    if (localConfigDataVerify.files != null)
                    {
                        //检查本地文件是否和远端的相同 相同就不用更新
                        foreach (var loadConfigDataMsg in localConfigDataVerify.files)
                        {
                            if (loadConfigDataMsg.Path == serverFilesMsg.Path)
                            {
                                if (loadConfigDataMsg.MD5 == serverFilesMsg.MD5)
                                {
                                    //相同跳过
                                    isNeedUpdata = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (isNeedUpdata)
                    {
                        SetConfigDataDownladUnit(serverFilesMsg);
                    }

                }

                ConfigData_Updata = true;
            }

            isCheckConfigDataVerify = true;
        }

        private void SetConfigDataDownladUnit(ConfigData_FileMsg msg)
        {
            //缓存本地数据
            DownloadUnit downloadUnit = new DownloadUnit()
            {
                name = msg.Path,
                md5 = msg.MD5,
                maxTryCount = 10,
                savePath = Path.Combine(ConfigDataPath_Cache, msg.Path),
                downUrl = Path.Combine(ConfigDataPath_Server, msg.Path),
                progressFun = UpdateDownProgre,
                completeFun = DownComplete,
                errorFun = DownError

            };

            allDownloadUnit.Add(downloadUnit);

            allDoneloadSize += msg.Size;
            allFileDownloadProgre.Add(downloadUnit, 0);

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

        private void DownError(DownloadUnit downUnit, string msg)
        {
            LogUtil.LogError($"[VersionUpdateMgr]文件{downUnit.name}下载失败:{msg}");
            LogUtil.LogError($"[VersionUpdateMgr]下载地址:{downUnit.downUrl}");
            LogUtil.LogError($"[VersionUpdateMgr]保存地址:{downUnit.savePath}");

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
            GenericDispatcher.Instance.Dispatch<int, Action>(AppMsg.UI_SetLoadingValueUI, ProgreVal, null);
        }

        /// <summary>
        /// 检测更新完成
        /// </summary>
        /// <returns></returns>
        private bool CheckVersionUpdateComplete()
        {
            if (doneloadCompleteSum >= allDownloadUnit.Count)
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

            LogUtil.Log($"[VersionUpdateMgr]开始移动临时文件到正式目录");
            foreach (var item in allDownloadUnit)
            {
                if (item.isDownload)
                {
                    string path = GetLoadPath(item);

                    LogUtil.Log($"[VersionUpdateMgr]开始移动文件{item.savePath}到{path}");
                    FileUtil.CreadFileLastDirectory(path);
                    if (File.Exists(path)) File.Delete(path);
                    File.Move(item.savePath, path);
                }
            }

            LogUtil.Log($"[VersionUpdateMgr]移动临时文件到正式目录完成");

            //回调
            OnUpdateComplete(true);


        }

        private void UpdataAllVerify()
        {
            if (HotFix_Updata)
            {
                //将网络版本写入本地目录
                string _hotFixPath = Path.Combine(HotFixPath_Local, "version.json");
                string _hotFixFileJson = SerializeUtil.UnityToJson(serverHotFixVerify);
                FileUtil.WriteAllText(_hotFixPath, _hotFixFileJson, true);
                LogUtil.Log(_hotFixPath + "更新完成");
            }

            if (AssetBundle_Updata)
            {
                //网络版本写入本地目录 
                string _assetBundlePath = Path.Combine(AssetBundlePath_Local, "version.json");
                string _assetBundleFileJson = SerializeUtil.UnityToJson(serverAssetBundleVerify);
                FileUtil.WriteAllText(_assetBundlePath, _assetBundleFileJson, true);
                LogUtil.Log(_assetBundlePath + "更新完成");
            }

            if (ConfigData_Updata)
            {
                //网络版本写入本地目录 
                string _ConfigDataPath = Path.Combine(ConfigDataPath_Local, "ConfigDataVerify.json");
                string _ConfigDataFileJson = SerializeUtil.UnityToJson(serverConfigDataVerify);
                FileUtil.WriteAllText(_ConfigDataPath, _ConfigDataFileJson, true);
                LogUtil.Log(_ConfigDataPath + "更新完成");
            }



        }

        public string GetLoadPath(DownloadUnit unit)
        {
            string savePath = Path.Combine(unit.savePath);

            string path = "";

            if (savePath.Contains(HotFixPath_Cache))
            {
                path = HotFixPath_Local;
            }

            if (savePath.Contains(AssetBundlePath_Cache))
            {
                path = AssetBundlePath_Local;
            }

            if (savePath.Contains(ConfigDataPath_Cache))
            {
                path = ConfigDataPath_Local;
            }

            return Path.Combine(path, unit.name);

        }


    }





}