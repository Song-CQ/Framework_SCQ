using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadFromFileExample : MonoBehaviour
{

    public Button button;
    private AssetBundle myLoadedAssetBundle;

    private string assetBundlePath ;

    void Start()
    {
        assetBundlePath = Application.dataPath + "/Test/ab";
        myLoadedAssetBundle
            = AssetBundle.LoadFromFile(Path.Combine(assetBundlePath, "ab_test"));
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }


        var mas = AssetBundle.LoadFromFile(Path.Combine(assetBundlePath, "ab"));

        AssetBundleManifest assetBundleManifest = mas.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        string[] strs = assetBundleManifest.GetDirectDependencies("ab_test");

        foreach (var item in strs)
        {
            Debug.Log(item);
            AssetBundle.LoadFromFile(Path.Combine(assetBundlePath, item));
        }

        var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("AB_Test1");
        Instantiate(prefab);

        button.onClick.AddListener(OnClick);

    }

    
    public void OnClick()
    {
        myLoadedAssetBundle.Unload(false);
    }

}