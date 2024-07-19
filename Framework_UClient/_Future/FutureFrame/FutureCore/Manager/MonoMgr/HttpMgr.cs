/****************************************************
    文件：HttpMgr.cs
	作者：Clear
    日期：2022/6/6 21:19:16
    类型: 框架核心脚本(请勿修改)
	功能：http管理器
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace FutureCore
{
    public sealed class HttpMgr : BaseMonoMgr<HttpMgr>
    {
        private const int DefaultTimeout = 5;
        private Dictionary<string, Texture2D> textureCaches = new Dictionary<string, Texture2D>();
        public void Send(string webUrl, Action<bool, DownloadHandler> callBack, int timeout = DefaultTimeout, bool isPost = false, Dictionary<string, object> dic = null)
        {
            if (callBack == null)
            {
                return;
            }
            if (!isPost)
            {
                GetUrl(webUrl, callBack, timeout);
            }
            else
            {
                PostUrl(webUrl, callBack, timeout, dic == null ? string.Empty : SerializeUtil.ToJson(dic));
            }

        }
        private void GetUrl(string webUrl, Action<bool, DownloadHandler> callBack, int timeout)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(webUrl);
            webRequest.timeout = timeout;
            StartCoroutine(OnHttpRequest(webRequest, callBack));
        }
        private void PostUrl(string webUrl, Action<bool, DownloadHandler> callBack, int timeout, string json)
        {
            WWWForm form = new WWWForm();
            form.AddField(string.Empty, json);
            UnityWebRequest webReq = UnityWebRequest.Post(webUrl, form);
            webReq.timeout = timeout;
            StartCoroutine(OnHttpRequest(webReq, callBack)); 
        }
        

        private IEnumerator OnHttpRequest(UnityWebRequest webRequest, Action<bool, DownloadHandler> callBack)
        {
            yield return webRequest.SendWebRequest();
            bool isError = false;
            if (webRequest.isNetworkError || webRequest.isHttpError || webRequest.responseCode != WebRequestConst.Succeed)
            {
                isError = true;
                string errorMsg = webRequest.url + " Error: " + webRequest.error + " Code: " + webRequest.responseCode + " Text: " + webRequest.downloadHandler.text;
                LogUtil.LogError("[HttpMgr]" + errorMsg);
            }
            else if (webRequest.downloadHandler.text == "error")
            {
                isError = true;
                string errorMsg = "请求失败";
                LogUtil.LogError("[HttpMgr]" + errorMsg);
            }
            callBack(isError,webRequest.downloadHandler);
            webRequest.Dispose();
        }
        public void DownTexture(string webUrl, Action<Texture2D> callback, bool isCache = true)
        {
            if (textureCaches.ContainsKey(webUrl))
            {
                callback(textureCaches[webUrl]);
                return;
            }
            StartCoroutine(OnDownTexture(webUrl, callback, isCache));
        }

        IEnumerator OnDownTexture(string webUrl, Action<Texture2D> callback, bool isCache)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(webUrl))
            {
                uwr.timeout = DefaultTimeout;
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    LogUtil.LogError("[HttpMgr]" + uwr.error);
                    callback(null);
                    yield break;
                }

                Texture2D texture2D = DownloadHandlerTexture.GetContent(uwr);
                callback(texture2D);
                if (isCache)
                {
                    if (!textureCaches.ContainsKey(webUrl))
                    {
                        textureCaches.Add(webUrl, texture2D);
                    }
                }
            }
        }


        public void DownAssetBundle(string webUrl,Action<AssetBundle> callback)
        {
            StartCoroutine(OnDownAssetBundle(webUrl,callback)); 
        }

        private IEnumerator OnDownAssetBundle(string webUrl, Action<AssetBundle> callback)
        {
            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(webUrl))
            {
                uwr.timeout = DefaultTimeout;
                yield return uwr.SendWebRequest();
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    LogUtil.LogError("[HttpMgr]" + uwr.error);
                    callback(null);
                    yield break;
                }
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                callback(bundle);
            }


        }
    }
}