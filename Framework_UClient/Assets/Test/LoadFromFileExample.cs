using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadFromFileExample : MonoBehaviour
{
    void Start()
    {
        var myLoadedAssetBundle
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "ab_test"));
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        
         var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("AB_Test");
        Instantiate(prefab);
    }
}