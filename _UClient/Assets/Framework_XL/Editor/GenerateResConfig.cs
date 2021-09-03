/****************************************************
    文件：GenerateResConfig.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/02/24 23:10
    功能：查找资源并生成资源路径映射表
    并写入到SteamingAssets文件夹
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace XL.Editor_XL
{
    public class GenerateResConfig : Editor
    {
        private static string path = "Assets/Resources";
        private static string filter = "prefab";

        [MenuItem("Framework_XL/查找资源并生成文件映射")]
        public static void ResConfig()
        {
            string[] resFiles = AssetDatabase.FindAssets("t:"+filter, new string[] { path });
            for (int i = 0; i < resFiles.Length; i++)
            {
                string resPath = AssetDatabase.GUIDToAssetPath(resFiles[i]);
                string name = Path.GetFileNameWithoutExtension(resPath);
                resPath = resPath.Replace(path+"/",string.Empty).Replace("."+filter,string.Empty);

                resFiles[i] = name + "=" + resPath;
            }
            File.WriteAllLines("Assets/SteamingAssets/ConfigMap.txt",resFiles);

            AssetDatabase.Refresh();
        }

    }
}
