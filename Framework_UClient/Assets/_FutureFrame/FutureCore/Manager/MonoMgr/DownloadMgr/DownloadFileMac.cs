/****************************************************
    文件：DownloadTask.cs
	作者：Clear
    日期：2022/6/11 18:48:45
    类型: 框架核心脚本(请勿修改)
	功能：下载任务
*****************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace FutureCore
{
    public enum DownloadMacState
    {
        None,
        ResetSize,
        Download,
        Md5,
        Complete,
        Error,
    }

    public class DownloadFileMac
    {
        const int oneReadLen = 16 * 1024;       // 一次读取长度 16384 = 16*kb
        const int Md5ReadLen = 16 * 1024;       // 一次读取长度 16384 = 16*kb
        const int ReadWriteTimeOut = 2 * 1000;  // 超时等待时间
        const int TimeOutWait = 5 * 1000;       // 超时等待时间

        public DownloadUnit _downUnit;

        public int _curSize = 0;
        public int _allSize = 0;
        public DownloadMacState _state = DownloadMacState.None;
        public int _tryCount = 0; //尝试次数
        public string _error = "";

        public DownloadFileMac(DownloadUnit downUnit)
        {
            _downUnit = downUnit;
        }

        //防止失败频繁回调，只在特定次数回调
        public bool IsNeedErrorCall()
        {
            if (_tryCount == 3
                || _tryCount == 10
                || _tryCount == 100)
                return true;
           
            return false;
        }

        public void Run()
        {
            _tryCount++;

            if (_downUnit.name== "gamerood")
            {
                string var = "gamerood";
            }

            _state = DownloadMacState.ResetSize;
            if (!ResetSize()) return;

            _state = DownloadMacState.Download;
            if (!Download()) return;

            _state = DownloadMacState.Md5;
            if (!CheckMd5()) //校验失败，重下一次
            {
                _state = DownloadMacState.Download;
                if (!Download()) return;

                _state = DownloadMacState.Md5;
                if (!CheckMd5()) return; //两次都失败，文件有问题
            }

            _state = DownloadMacState.Complete;
        }

        private bool ResetSize()
        {
            if (_downUnit.name == "gamerood")
            {
                string var = "gamerood";
            }

            if (_downUnit.size <= 0)
            {
                _downUnit.size = GetWebFileSize(_downUnit.downUrl);

                if (_downUnit.size == 0) return false;
            }

            _curSize = 0;
            _allSize = _downUnit.size;

            return true;
        }

        

        private bool CheckMd5()
        {
            if (string.IsNullOrEmpty(_downUnit.md5)) return true; //不做校验，默认成功

            string md5 = GetMD5HashFromFile(_downUnit.savePath);

            if (md5 != _downUnit.md5)
            {
                File.Delete(_downUnit.savePath);
                MainThreadLog.LogWarning("文件MD5校验错误：" + _downUnit.name);
                _state = DownloadMacState.Error;
                _error = "Check MD5 Error ";
                return false;
            }
            return true;
        }

        public bool Download()
        {
            //打开上次下载的文件
            long startPos = 0;
            string tempFile = _downUnit.savePath + ".temp";
            FileStream fs = null;
            if (File.Exists(_downUnit.savePath))
            {
                //文件已存在，跳过
                MainThreadLog.Log("File is Exists " + _downUnit.savePath);
                _curSize = _downUnit.size;
                return true;
            }
            else if (File.Exists(tempFile))
            {
                fs = File.OpenWrite(tempFile);
                startPos = fs.Length;
                fs.Seek(startPos, SeekOrigin.Current); //移动文件流中的当前指针

                //文件已经下载完，没改名字，结束
                if (startPos >= _downUnit.size)
                {
                    fs.Flush();
                    fs.Close();
                    fs = null;
                    if (File.Exists(_downUnit.savePath)) File.Delete(_downUnit.savePath);
                    File.Move(tempFile, _downUnit.savePath);

                    _curSize = (int)startPos;
                    return true;
                }
            }
            else
            {
                string direName = Path.GetDirectoryName(tempFile);
                if (!Directory.Exists(direName)) Directory.CreateDirectory(direName);
                fs = new FileStream(tempFile, FileMode.Create);
            }

            // 下载逻辑
            HttpWebRequest request = null;
            HttpWebResponse respone = null;
            Stream ns = null;
            try
            {
                request = WebRequest.Create(_downUnit.downUrl) as HttpWebRequest;
                request.ReadWriteTimeout = ReadWriteTimeOut;
                request.Timeout = TimeOutWait;
                if (startPos > 0) request.AddRange((int)startPos);  //设置Range值，断点续传
                                                                    //向服务器请求，获得服务器回应数据流
                respone = (HttpWebResponse)request.GetResponse();
                ns = respone.GetResponseStream();
                
                ns.ReadTimeout = TimeOutWait;
                long totalSize = respone.ContentLength;
               
                long curSize = startPos;
                if (curSize >= totalSize)
                {
                    fs.Flush();
                    fs.Close();
                    fs = null;
                    if (File.Exists(_downUnit.savePath)) File.Delete(_downUnit.savePath);
                    File.Move(tempFile, _downUnit.savePath);

                    _curSize = (int)curSize;
                }
                else
                {
                    byte[] bytes = new byte[oneReadLen];
                    int readSize = ns.Read(bytes, 0, oneReadLen); // 读取第一份数据
                    while (readSize > 0)
                    {
                        fs.Write(bytes, 0, readSize);       // 将下载到的数据写入临时文件
                        curSize += readSize;

                        // 判断是否下载完成
                        // 下载完成将temp文件，改成正式文件
                        if (curSize == totalSize)
                        {
                            fs.Flush();
                            fs.Close();
                            fs = null;
                            if (File.Exists(_downUnit.savePath)) File.Delete(_downUnit.savePath);
                            File.Move(tempFile, _downUnit.savePath);
                        }

                        // 回调一下
                        _curSize = (int)curSize;
                        // 往下继续读取
                        readSize = ns.Read(bytes, 0, oneReadLen);
                    }
                }
            }
            catch (Exception ex)
            {
                //下载失败，删除临时文件
                if (fs != null) { fs.Flush(); fs.Close(); fs = null; }

                if (File.Exists(tempFile))
                    File.Delete(tempFile);
                if (File.Exists(_downUnit.savePath))
                    File.Delete(_downUnit.savePath);

                MainThreadLog.Log($"文件{_downUnit.name}下载出错：{ex.Message}");
                _state = DownloadMacState.Error;
                _error = "Download Error " + ex.Message;
            }
            finally
            {
                if (fs != null) { fs.Flush(); fs.Close(); fs = null; }
                if (ns != null) { ns.Close(); ns = null; }
                if (respone != null) { respone.Close(); respone = null; }
                if (request != null) { request.Abort(); request = null; }
            }

            if (_state == DownloadMacState.Error) return false;
            return true;
        }

        private int GetWebFileSize(string url)
        {
            HttpWebRequest request = null;
            WebResponse respone = null;
            int length = 0;
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                request = webRequest as HttpWebRequest;
                if (request == null)
                {
                    MainThreadLog.LogWarning("文件不存在:" + url);
                    _state = DownloadMacState.Error;
                    _error = "文件不存在: " + url;                  
                }
                else
                {
                    request.Timeout = TimeOutWait;
                    request.ReadWriteTimeout = ReadWriteTimeOut;
                    //向服务器请求，获得服务器回应数据流
                    respone = request.GetResponse();
                    if (_downUnit.name == "gamerood")
                    {
                        string var = "gamerood";
                    }                 
                    length = (int)respone.ContentLength;

                }
               
            }
            catch (WebException e)
            {
                MainThreadLog.LogWarning($"获取文件{url}长度出错：{e.Message}" );
                _state = DownloadMacState.Error;
                _error = "Request File Length Error " + e.Message;
            }
            finally
            {
                if (respone != null) { respone.Close(); respone = null; }
                if (request != null) { request.Abort(); request = null; }
            }
            if (length==-1)
            {
                MainThreadLog.LogError($"获取文件{url}长度失败为-1");
                length = 0;
            }
            return length;
        }
       

        private string GetMD5HashFromFile(string fileName)
        {
            byte[] buffer = new byte[Md5ReadLen];
            int readLength = 0;//每次读取长度  
            var output = new byte[Md5ReadLen];

            using (Stream inputStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.MD5CryptoServiceProvider())
                {
                    while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        //计算MD5  
                        hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                    }
                    //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)  
                    hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                    byte[] retVal = hashAlgorithm.Hash;

                    System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }

                    hashAlgorithm.Clear();
                    inputStream.Close();
                    return sb.ToString();
                }
            }
        }

    }


}

