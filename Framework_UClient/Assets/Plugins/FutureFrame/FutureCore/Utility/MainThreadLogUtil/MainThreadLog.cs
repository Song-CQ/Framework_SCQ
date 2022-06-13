/****************************************************
    文件：ThreadDebugLog.cs
	作者：Clear
    日期：2022/6/11 19:0:41
    类型: 框架核心脚本(请勿修改)
	功能：主线程线程Log
*****************************************************/
using System;
using System.Collections.Generic;

namespace FutureCore
{
    public static class MainThreadLog 
    {
        private const string Lock = "Lock";
        private static List<string> allLogCallBack = new List<string>();
        private static List<string> allLogWarningCallBack = new List<string>();
        private static List<string> allLogErrorCallBack = new List<string>();
        public static void LoopLog()
        {
            if (allLogCallBack.Count == 0 && allLogErrorCallBack.Count == 0)
            {
                return;
            }
            lock (Lock)
            {
                foreach (var msg in allLogCallBack)
                {
                    LogUtil.Log(msg);
                }
                allLogCallBack.Clear();
                
                foreach (var msg in allLogWarningCallBack)
                {
                    LogUtil.LogWarning(msg);
                }
                allLogWarningCallBack.Clear();

                foreach (var msg in allLogErrorCallBack)
                {
                    LogUtil.LogError(msg);
                }
                allLogErrorCallBack.Clear();
            }        
        }

        public static void Log(string msg)
        {
            lock (Lock)
            {
                allLogCallBack.Add(msg);
            }
        }   
        public static void LogWarning(string msg)
        {
            lock (Lock)
            {
                allLogWarningCallBack.Add(msg);
            }
        }
        public static void LogError(string msg)
        {
            lock (Lock)
            {
                allLogErrorCallBack.Add(msg);
            }
        }

    }
}