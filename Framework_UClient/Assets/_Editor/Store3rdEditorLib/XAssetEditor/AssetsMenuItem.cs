//
// AssetsMenuItem.cs
//
// Author:
//       fjy <jiyuan.feng@live.com>
//
// Copyright (c) 2019 fjy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Plugins.XAsset.Editor
{
    public static class AssetsMenuItem
    {
        // [Du 修改]
        private const string EncryptKey = "asdfasdfjklasdfargaergdcvjirnfioajsdviojaoiperjf";

        // [Du 修改]
        private const string KCopyPath = "Assets/[XAsset 快速复制路径]";

        private const string KMarkAssetsWithDir = "Assets/[XAsset]/资源标记/按目录标记";
        private const string KMarkAssetsWithFile = "Assets/[XAsset]/资源标记/按文件标记";
        private const string KMarkAssetsWithName = "Assets/[XAsset]/资源标记/按名称标记";

        private const string KClearAssetBundles = "Assets/[XAsset]/生成/1) 清除AssetBundles";
        private const string KBuildEncryptAssetBundles = "Assets/[XAsset]/生成/2) 生成加密资源包";
        private const string KBuildAssetBundles = "Assets/[XAsset]/生成/3) 生成资源包";
        private const string KCopyToStreamingAssets = "Assets/[XAsset]/生成/4) 同步到StreamingAssets";

        private const string KBuildManifest = "Assets/[XAsset]/初始化/生成配置";
        private const string KBuildPlayer = "Assets/[XAsset]/编译/生成播放器";

        private const string KMarkAssets = "标记资源中...";

        /*
        private const string KMarkAssetsWithDir = "Assets/AssetBundles/按目录标记";
        private const string KMarkAssetsWithFile = "Assets/AssetBundles/按文件标记";
        private const string KMarkAssetsWithName = "Assets/AssetBundles/按名称标记";
        private const string KBuildManifest = "Assets/AssetBundles/生成配置";
        private const string KBuildAssetBundles = "Assets/AssetBundles/生成資源包";
        private const string KBuildPlayer = "Assets/AssetBundles/生成播放器";
        private const string KCopyPath = "Assets/快速复制路径";
        private const string KMarkAssets = "标记资源";
        private const string KCopyToStreamingAssets = "Assets/AssetBundles/拷贝到StreamingAssets";
        */
        public static string assetRootPath;

        // [修改]
        //[InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            EditorUtility.ClearProgressBar();
            var settings = BuildScript.GetSettings(); 
            if (settings.localServer)
            {
                bool isRunning = LaunchLocalServer.IsRunning();
                if (!isRunning)
                {
                    LaunchLocalServer.Run();
                }
            }
            else
            {
                bool isRunning = LaunchLocalServer.IsRunning();
                if (isRunning)
                {
                    LaunchLocalServer.KillRunningAssetBundleServer();
                }
            }
            Utility.dataPath = System.Environment.CurrentDirectory; // 讲读取目录设置到生成目录
            Utility.downloadURL = BuildScript.GetManifest().downloadURL;
            Utility.assetBundleMode = settings.runtimeMode;
            Utility.getPlatformDelegate = BuildScript.GetPlatformName;
            Utility.loadDelegate = AssetDatabase.LoadAssetAtPath;
        }

        public static string TrimedAssetBundleName(string assetBundleName)
        {
            if(string.IsNullOrEmpty(assetRootPath))
                return assetBundleName;
            return assetBundleName.Replace(assetRootPath, "");
        }

        [MenuItem(KMarkAssetsWithDir)]
        private static void MarkAssetsWithDir()
        {
            var settings = BuildScript.GetSettings();
            assetRootPath = settings.assetRootPath; 
            var assetsManifest = BuildScript.GetManifest();
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset);
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(KMarkAssets, path, i * 1f / assets.Length))
                    break;
                var assetBundleName = TrimedAssetBundleName(Path.GetDirectoryName(path).Replace("\\", "/")) + "_g";
                BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null, assetsManifest);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(KMarkAssetsWithFile)]
        private static void MarkAssetsWithFile()
        {
            var settings = BuildScript.GetSettings();
            assetRootPath = settings.assetRootPath; 
            var assetsManifest = BuildScript.GetManifest();
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset);
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(KMarkAssets, path, i * 1f / assets.Length))
                    break;

                var dir = Path.GetDirectoryName(path);
                var name = Path.GetFileNameWithoutExtension(path);
                if (dir == null)
                    continue;
                dir = dir.Replace("\\", "/") + "/";
                if (name == null)
                    continue;

                var assetBundleName = TrimedAssetBundleName(Path.Combine(dir, name));
                BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null, assetsManifest);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(KMarkAssetsWithName)]
        private static void MarkAssetsWithName()
        {
            var settings = BuildScript.GetSettings();
            assetRootPath = settings.assetRootPath; 
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            var assetsManifest = BuildScript.GetManifest();
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset);
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(KMarkAssets, path, i * 1f / assets.Length))
                    break;
                var assetBundleName = Path.GetFileNameWithoutExtension(path);
                BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null, assetsManifest);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(KBuildManifest)]
        private static void BuildManifest()
        {
            BuildScript.BuildManifest();
        }

        [MenuItem(KClearAssetBundles)]
        private static void ClearAssetBundles()
        {
            string path = Utility.AssetBundles;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            path = Path.Combine(Application.streamingAssetsPath, Utility.AssetBundles);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem(KBuildEncryptAssetBundles)]
        private static void BuildEncryptAssetBundles()
        {
            BuildScript.BuildManifest();
            AssetBundleManifest manifest = BuildScript.BuildAssetBundles();
            string outputPath = BuildScript.CreateAssetBundleDirectory();
            EncryptOffestAB(manifest, outputPath);
        }

        [MenuItem(KBuildAssetBundles)]
        private static void BuildAssetBundles()
        {
            BuildScript.BuildManifest();
            BuildScript.BuildAssetBundles();
        }

        [MenuItem(KCopyToStreamingAssets)]
        private static void CopyAssetBundles()
        {
            BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundles));
            AssetDatabase.Refresh();
        }

        private static void EncryptOffestAB(AssetBundleManifest manifest, string outputPath)
        {
            foreach (string bundleName in manifest.GetAllAssetBundles())
            {
                string abFilePath = outputPath + "/" + bundleName;
                byte[] encryptBytes = Encoding.UTF8.GetBytes(EncryptKey);
                int encryptOffsetLen = encryptBytes.Length;
                if (encryptOffsetLen >= 0)
                {
                    byte[] abFileData = File.ReadAllBytes(abFilePath);
                    byte[] newBuffer = encryptBytes.Concat(abFileData).ToArray();
                    int newFileLen = newBuffer.Length;
                    FileStream fs = File.OpenWrite(abFilePath);
                    fs.Write(newBuffer, 0, newFileLen);
                    fs.Close();
                }
                Debug.Log("[XAsset]EncryptOffestAB Offset: " + encryptOffsetLen);
            }
        }

        [MenuItem(KBuildPlayer)]
        private static void BuildStandalonePlayer()
        {
            BuildScript.BuildStandalonePlayer();
        }

        [MenuItem(KCopyPath)]
        private static void CopyPath()
        {
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            EditorGUIUtility.systemCopyBuffer = assetPath;
            Debug.Log(assetPath);
        }
    }
}