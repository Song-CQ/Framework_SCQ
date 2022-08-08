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

        if (!Directory.Exists(assetBundlePath+"/.."))
        {
            Directory.CreateDirectory(assetBundlePath + "/..");
        }
        
    

        if (!File.Exists(assetBundlePath))
        {
            File.Delete(assetBundlePath);
            


        }
        Debug.Log("��ʼ����");

        UnityWebRequest unityWebRequestAssetBundle = UnityWebRequest.Get("https://xl2728295639-1307682036.cos.ap-guangzhou.myqcloud.com/AssetBundles/imgab");

       

        unityWebRequestAssetBundle.downloadHandler = new DownloadHandlerFile(assetBundlePath);

        var resy = unityWebRequestAssetBundle.SendWebRequest(); 
       
        while (!unityWebRequestAssetBundle.isDone)
        {
            Debug.Log("���ؽ���" + unityWebRequestAssetBundle.downloadProgress);
            yield return new WaitForSeconds(1);
        }


        Debug.Log("���ؽ���" + unityWebRequestAssetBundle.downloadProgress);
        if (unityWebRequestAssetBundle.isNetworkError)
        {
            Debug.LogError("���ش���");
            yield break;
        }

        //myLoadedAssetBundle
        //    = DownloadHandlerAssetBundle.GetContent(unityWebRequestAssetBundle);
        //if (myLoadedAssetBundle == null)
        //{
        //    Debug.Log("Failed to load AssetBundle!");
        //    yield break;
        //}


        OnCom();









    }

    
    public void OnCom()
    {
       // GameObject.FindObjectOfType<GameRoodMgr>().StartLoad();
    }

}

/*
 
 
 
 */