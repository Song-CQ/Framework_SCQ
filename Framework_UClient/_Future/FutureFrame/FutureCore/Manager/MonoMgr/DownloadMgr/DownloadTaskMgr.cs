/****************************************************
    文件：DownloadTask.cs
	作者：Clear
    日期：2022/6/10 23:54:15
    类型: 框架核心脚本(请勿修改)
	功能：下载管理器
*****************************************************/
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;

namespace FutureCore
{
    public delegate void DonwloadErrorCallBack(DownloadUnit downUnit, string msg);
    public delegate void DonwloadProgressCallBack(DownloadUnit downUnit, int curSize, int allSize);
    public delegate void DonwloadCompleteCallBack(DownloadUnit downUnit);

    public class DownloadUnit
    {
        public string name; //下载的文件，作为标识，
        public string downUrl; //远程地址
        public string savePath; //本地地址

        public int size; //文件长度，非必须
        public string md5; //需要校验的md5，非必须
        public bool isDelete; //用于清理正在下载的文件
        public bool isDownload;//是否下载完成

        public DonwloadErrorCallBack errorFun;
        public DonwloadProgressCallBack progressFun;
        public DonwloadCompleteCallBack completeFun;
    }

    public class DownloadTaskMgr : BaseMonoMgr<DownloadTaskMgr>
    {

        //多线程下载
        //https://blog.csdn.net/wowo1gt/article/details/100087547
    
        private static object _lock = new object();
        private const int MAX_THREAD_COUNT = 20;

        private Queue<DownloadFileMac> _readyList;
        private Dictionary<Thread, DownloadFileMac> _runningList;
        private List<DownloadUnit> _completeList;
        private List<DownloadFileMac> _errorList;

        public override void Init()
        {
            base.Init();
            _readyList = new Queue<DownloadFileMac>();
            _runningList = new Dictionary<Thread, DownloadFileMac>();
            _completeList = new List<DownloadUnit>();
            _errorList = new List<DownloadFileMac>();

            //https解析的设置
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
        }

        private bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }


        public void DownloadAsync(DownloadUnit info)
        {
            
            if (info == null) return;
            Debug.Log($"文件{info.name}开始加入下载队列");
            var fileMac = new DownloadFileMac(info);

            lock (_lock)
            {
                _readyList.Enqueue(fileMac);
            }

            if (_runningList.Count < MAX_THREAD_COUNT)
            {
                var thread = new Thread(StartThread);
                lock (_lock)
                {
                    _runningList.Add(thread, null);
                }
                thread.Start();
            }

        }

        //同步不会调用回调函数
        public bool DownloadSync(DownloadUnit info)
        {
            if (info == null) return false;

            var mac = new DownloadFileMac(info);
            try
            {//同步下载尝试三次
                mac.Run();
                if (mac._state == DownloadMacState.Complete) return true;
                mac.Run();
                if (mac._state == DownloadMacState.Complete) return true;
                mac.Run();
                if (mac._state == DownloadMacState.Complete) return true;
            }
            catch (Exception ex)
            {
                LogUtil.Log("Error DownloadSync " + mac._state + " " + mac._downUnit.name + " " + ex.Message + " " + ex.StackTrace);
            }

            return false;
        }

        public void DeleteDownload(DownloadUnit info)
        {
            lock (_lock)
            {
                info.isDelete = true;
            }
        }

        //清理所有下载
        public void ClearAllDownloads()
        {
            lock (_lock)
            {
                foreach (var mac in _readyList)
                {
                    if (mac != null) mac._downUnit.isDelete = true;
                }

                foreach (var item in _runningList)
                {
                    if (item.Value != null) item.Value._downUnit.isDelete = true;
                }

                foreach (var unit in _completeList)
                {
                    if (unit != null) unit.isDelete = true;
                }
            }
        }

        private void StartThread()
        {
            while (true)
            {
                DownloadFileMac mac = null;
                lock (_lock)
                {
                    if (_readyList.Count > 0)
                    {
                  
                        mac = _readyList.Dequeue();
                        //Debug.LogError($"文件{mac._downUnit.name}开始加入下载,当前_readyList数量{_readyList.Count}");
                        _runningList[Thread.CurrentThread] = mac;

                        if (mac != null && mac._downUnit.isDelete)
                        {//已经销毁，不提取运行，直接删除
                            _runningList[Thread.CurrentThread] = null;
                            continue;
                        }
                    }
                }

                //已经没有需要下载的了
                if (mac == null) break;

                MainThreadLog.Log($"文件{mac._downUnit.name}开始下载");
                mac.Run();

                if (mac._state == DownloadMacState.Complete)
                {
                    lock (_lock)
                    {
                        _completeList.Add(mac._downUnit);
                        _runningList[Thread.CurrentThread] = null;
                        MainThreadLog.Log($"文件{mac._downUnit.name}下载完成");
                    }
                }
                else if (mac._state == DownloadMacState.Error)
                {
                    lock (_lock)
                    {
                        _readyList.Enqueue(mac);
                        _runningList[Thread.CurrentThread] = null;
                        //Debug.LogError($"文件{mac._downUnit.name}下载错误加入下载对列,当前_readyList数量{_readyList.Count}");
                        //防止失败频繁回调，只在特定次数回调
                        if (mac.IsNeedErrorCall())
                            _errorList.Add(mac);
                    }
                    break;
                }
                else
                {
                    MainThreadLog.LogError("[ThreadDebugLog]Error DownloadMacState " + mac._state + " " + mac._downUnit.name);
                    break;
                }
            }

        }



        private void UpdateComplete()
        {//回调完成
            if (_completeList.Count == 0) return;

            DownloadUnit[] completeArr = null;
            lock (_lock)
            {
                completeArr = _completeList.ToArray();
                _completeList.Clear();
            }

            foreach (var info in completeArr)
            {
                if (info.isDelete) continue; //已经销毁，不进行回调
                info.isDelete = true;

                if (info.progressFun != null)
                {
                    info.progressFun(info, info.size, info.size);
                }

                if (info.completeFun != null)
                {
                    try
                    {
                        info.isDownload = true;
                        info.completeFun(info);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError("UpdateComplete " + ex.Message);
                    }
                }
            }

        }

        private void UpdateError()
        {//回调error
            if (_errorList.Count == 0) return;

            DownloadFileMac[] errorArr = null;
            lock (_lock)
            {
                errorArr = _errorList.ToArray();
                _errorList.Clear();
            }

            foreach (var mac in errorArr)
            {
                var info = mac._downUnit;
                if (info.isDelete) continue; //已经销毁，不进行回调

                if (info.errorFun != null)
                {
                    info.errorFun(info, mac._error);
                    mac._error = "";
                }
            }
        }

        private void UpdateProgress()
        {
            //回调进度
            if (_runningList.Count == 0) return;

            List<DownloadFileMac> runArr = new List<DownloadFileMac>();
            lock (_lock)
            {
                foreach (var mac in _runningList.Values)
                {
                    if (mac != null) runArr.Add(mac);
                }
            }

            foreach (var mac in runArr)
            {
                var info = mac._downUnit;
                if (info.isDelete) continue; //已经销毁，不进行回调

                if (info.progressFun != null)
                {
                    info.progressFun(info, mac._curSize, mac._allSize);
                }
            }
        }

        private void UpdateThread()
        {
            if (_readyList.Count == 0 && _runningList.Count == 0) return;

            lock (_lock)
            {//关闭卡死的线程
                List<Thread> threadArr = new List<Thread>();
                foreach (var item in _runningList)
                {
                    if (item.Key.IsAlive) continue;

                    if (item.Value != null) _readyList.Enqueue(item.Value);

                    threadArr.Add(item.Key);
                }

                foreach (var thread in threadArr)
                {
                    _runningList.Remove(thread);
                    thread.Abort();
                }
            }

            //如果没有网络，不开启新线程，旧线程会逐个关闭
            if (!CheckNetworkActive()) return;

            if (_runningList.Count >= MAX_THREAD_COUNT) return;
            if (_readyList.Count > 0)
            {
                var thread = new Thread(StartThread);
                lock (_lock)
                {
                    _runningList.Add(thread, null);
                }
                thread.Start();
            }

        }

        public bool CheckNetworkActive()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {//没有网络

                return false;
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {//234G网络

                return true;
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {//wifi网络
                return true;
            }

            return false;
        }

        public void Update()
        {
            if (!IsInit)
            {
                return;
            }
            UpdateComplete();
            UpdateProgress();
            UpdateError();
            UpdateThread();
        }
    }


}