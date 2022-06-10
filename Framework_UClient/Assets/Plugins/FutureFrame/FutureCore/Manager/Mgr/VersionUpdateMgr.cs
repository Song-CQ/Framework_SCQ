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
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace FutureCore
{
    public class VersionUpdateMgr : BaseMonoMgr<VersionUpdateMgr>
    {
        private Action<bool> OnComplete;

        private string ABPath_Server;


        private string HotFixPath;

        


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
            ABPath_Server =  LocalAssestUrl + "/AssetBundles/" + UnityEditor.BuildTarget.StandaloneWindows.ToString();


#elif UNITY_STANDALONE

#elif UNITY_ANDROID
            
#endif



#if UNITY_EDITOR 
            HotFixPath = LocalAssestUrl + "/HotFix";
#elif UNITY_STANDALONE

#elif UNITY_ANDROID
          
#endif



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
            string filePath = PathConst.HotFixPath+"/HotFix.dll";
            if (!File.Exists(filePath))
            {
                File.Copy(PathConst.HotFixPath_StreamingAssets+"/HotFix.dll",filePath);              
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
            string filePath = (PathConst.AssetBundlesPath + "/"+PathConst.AssetBundlesTarget);
            if (!Directory.Exists(filePath))
            {
                string streamingAssetsPath = Application.streamingAssetsPath + "/" + PathConst.AssetBundlesTarget;
                if (Directory.Exists(streamingAssetsPath))
                {
                    FileUtil.CopyFolder(streamingAssetsPath,filePath);
                }
                else
                {
                    Directory.CreateDirectory(filePath);
                }

            }
            



        }
        #endregion


        #region 服务器更新检测

        private HotFixVerify localhotFixVerify;
        private HotFixVerify serverFixVerify;
        private void CheckServerUpdate()
        {
            CheckServerHotFixFile();



            
        }

        private void CheckServerHotFixFile()
        {
            //取本地
            localhotFixVerify = new HotFixVerify();
            string filePath = PathConst.HotFixPath + "/verify.json";
            if (File.Exists(filePath))
            {
                string fileJson = File.ReadAllText(filePath);
                HotFixVerify _hotFixVerify = JsonUtility.FromJson<HotFixVerify>(fileJson);
                if (_hotFixVerify != null)
                {
                    localhotFixVerify = _hotFixVerify;
                }
            }
            //取网络
            HttpMgr.Instance.Send(HotFixPath, GetHotFixVersion);
        }

        private void GetHotFixVersion(bool isError, DownloadHandler download)
        {
            if (isError)
            {
                LogUtil.LogError("获取热更版本信息失败");
                
                return;
            }
            string filePath = Path.Combine(PathConst.HotFixPath, "verify.json");

            string fileJson = download.text;       
            serverFixVerify = JsonUtility.FromJson<HotFixVerify>(fileJson);
            if (serverFixVerify.MD5!=localhotFixVerify.MD5)
            {
                if (serverFixVerify.version >= localhotFixVerify.version)
                {

                    File.WriteAllText(filePath, fileJson);
                }
            }
            

        }





        #endregion


    }
}