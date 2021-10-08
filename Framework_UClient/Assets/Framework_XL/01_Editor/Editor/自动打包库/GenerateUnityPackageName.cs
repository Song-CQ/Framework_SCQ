using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace XL.QFramework
{
    public class GenerateUnityPackageName
    {
#if UNITY_EDITOR
        [MenuItem("QFramework/1.打包成Package %E")]
        private static void MenuClicked()
        {
            var assetPathName = "Assets/Framework_XL";
            var fileName= "Framework_XL_" + DateTime.Now.ToString("yyyy_MMdd_HH") + ".unitypackage";
            AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
            
            Application.OpenURL("file:///" + Path.Combine(Application.dataPath, "../"));
        }
#endif
    }
}

