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
<<<<<<< HEAD
        
=======
        OnCom();
        yield break;

>>>>>>> 1293644c65bff27a541748b2abb1bd5068ecff31
        var assetBundlePath = Application.persistentDataPath + "/AssetBundle/imgab";

        if (!Directory.Exists(assetBundlePath+"/.."))
        {
            Directory.CreateDirectory(assetBundlePath + "/..");
        }
    

        if (!File.Exists(assetBundlePath))
        {
            File.Delete(assetBundlePath);
     


        }
        Debug.Log("开始下载");

        UnityWebRequest unityWebRequestAssetBundle = UnityWebRequest.Get("https://xl2728295639-1307682036.cos.ap-guangzhou.myqcloud.com/AssetBundles/imgab");

       

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

<<<<<<< HEAD
        OnCom();
=======
        
>>>>>>> 1293644c65bff27a541748b2abb1bd5068ecff31








    }

    
    public void OnCom()
    {
        GameObject.FindObjectOfType<GameRoodMgr>().StartLoad();
    }

}

/*
 
 
 
 */