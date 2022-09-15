using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using XLNet;
using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Assets/AssetBundle Tool/Build All AssetBundle")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/Test/ab";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,BuildAssetBundleOptions.None,BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Assets/AssetBundle Tool/Clear All AssetBundle Name")]
    static void RemoveAllAssetBundleName()
    {

        string[] name =  AssetDatabase.GetAllAssetBundleNames();

        foreach (var item in name)
        {
            AssetDatabase.RemoveAssetBundleName(item,true);
        }
      
        AssetDatabase.Refresh();
    }



}

