/****************************************************
    文件：VersionUpdateMgr.cs
	作者：Clear
    日期：2022/5/4 16:6:35
    类型: 框架核心脚本(请勿修改)
	功能：检测更新资源版本
*****************************************************/
using System;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

namespace FutureCore
{
    public class VersionUpdateMgr : BaseMonoMgr<VersionUpdateMgr>
    {
        private Action<bool> OnComplete;

        private string versionPath;

        public override void Init()
        {
            base.Init();

            InitVersionPath();

        }

        private void InitVersionPath()
        {
#if UNITY_EDITOR 
            versionPath =  AppConst.LocalAssestUrl + "/" + UnityEditor.BuildTarget.StandaloneWindows + "/version.json";
#elif UNITY_STANDALONE

#elif UNITY_ANDROID
            versionPath = AppFacade_Frame.ServerAssestUrl;
#endif

        }


        public void StartUpProcess(Action<bool> initAssets)
        {
            //检测资源加载
            OnComplete = initAssets;

            //本地文件检测
            App.SetLoadingSchedule(ProgressState.AssetsPrepare);
            CheckLocalFile();

            //版本检测
            App.SetLoadingSchedule(ProgressState.VersionUpdate);
            HttpMgr.Instance.Send(versionPath, GetVersion);
        }

        #region 本地文件检测

        private void CheckLocalFile()
        {
            CheckLocalHotFixFile();
        }

  
        private void CheckLocalHotFixFile()
        {
            string filePath = PathConst.HotFixPath+"HotFix.dll";
            if (!File.Exists(filePath))
            {
                File.Copy(PathConst.HotFixPath_StreamingAssets+"HotFix.dll",filePath);              
            }


            LogUtil.Log("[VersionUpdateMgr]更新本地热更文件完成");
        }
        #endregion


        private void GetVersion(bool isError, DownloadHandler download)
        {
            if (isError)
            {
                LogUtil.LogError("获取版本信息失败");
                OnComplete(false);
                return;
            }

            LogUtil.Log("获取版本信息成功");
            OnComplete(true);

        }
    }
}