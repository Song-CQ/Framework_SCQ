namespace XL.EditorTool
{
    using System;
    using UnityEngine;
    using System.IO;
    using UnityEditor;
    public class GenerateUnityPackageName
    {
        [MenuItem("QFramework/1.打包成Package %E",false,0)]
        private static void FrameworkPackage()
        {
            var assetPathName = "Assets/Framework_XL";
            var fileName= "Framework_XL_" + DateTime.Now.ToString("yyyy_MMdd_HH") + ".unitypackage";
            AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
            
            Application.OpenURL("file:///" + Path.Combine(Application.dataPath, "../"));
        }
        [MenuItem("Assets/QFramework/1.打包成Package",false,0)]
        private static void AssetsFrameworkPackage()
        {
            FrameworkPackage();
        }
        
    }
}

