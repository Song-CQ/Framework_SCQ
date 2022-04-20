using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using ProjectApp;

public class LoadFromFileExample : MonoBehaviour
{

    public Button button;
    private AssetBundle myLoadedAssetBundle;

    private string assetBundlePath ;



    IEnumerator Start()
    {
        var assetBundlePath = Application.persistentDataPath + "/AssetBundle/imgab";

       // DirectoryInfo directoryInfoas = Directory.CreateDirectory(assetBundlePath + "/...");
        //DirectoryInfo directoryInfoa = Directory.CreateDirectory(assetBundlePath + "/...");

        if (!File.Exists(assetBundlePath))
        {
            File.Delete(assetBundlePath);
     


        }
        Debug.Log("开始下载");
        UnityWebRequest unityWebRequestAssetBundle = UnityWebRequest.Get("https://gzc-download.weiyun.com/ftn_handler/c3c5b96e92f6384abf4c4f2fa970389cc03ba3b3fac9197637c53b5afedf1cb36e78262f56f3c71622b7d2f9d11c24afa9f8bfb0dc4fc6d45fa3411bf43d0264/imgab?fname=imgab&from=30113&version=3.3.3.3");

        unityWebRequestAssetBundle.downloadHandler = new DownloadHandlerFile(assetBundlePath);

        var resy = unityWebRequestAssetBundle.SendWebRequest(); 
       
        while (!unityWebRequestAssetBundle.isDone)
        {
            Debug.Log("下载进度" + unityWebRequestAssetBundle.downloadProgress);
            yield return new WaitForSeconds(1);
        }


        Debug.Log("下载进度" + unityWebRequestAssetBundle.downloadProgress);
        if (unityWebRequestAssetBundle.isNetworkError)
        {
            Debug.LogError("下载错误");
            yield break;
        }

        //myLoadedAssetBundle
        //    = DownloadHandlerAssetBundle.GetContent(unityWebRequestAssetBundle);
        //if (myLoadedAssetBundle == null)
        //{
        //    Debug.Log("Failed to load AssetBundle!");
        //    yield break;
        //}

        //OnCom();








    }

    
    public void OnCom()
    {
        GameObject.FindObjectOfType<GameRoodMgr>().StartLoad();
    }

}