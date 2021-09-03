/****************************************************
    文件：GenerateUnityPackageName.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/2/24 23:41
    功能：自动打包库
*****************************************************/
using System;
using UnityEngine;
using System.IO;
using UnityEditor;
namespace XL.Editor_XL
{
    public class GenerateUnityPackageName:Editor
    {
        [MenuItem("Framework_XL/将库打包成Package %E")]
        private static void MenuClicked()
        {
            var assetPathName = "Assets/Framework_XL";
            var fileName= "Framework_XL_" + DateTime.Now.ToString("yyyy_MMdd_HH") + ".unitypackage";
            AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
            
            Application.OpenURL("file:///" + Path.Combine(Application.dataPath, "../"));
        }
    }
}

