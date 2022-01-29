/************************************************************
    文件: ScriptsInfoRecoder.cs
	作者: 承清
    邮箱: 2728285639@qq.com
    日期: 2018/10/13 12:01
	功能: 记录脚本信息
*************************************************************/

using FutureCore;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public class RegisterStriptTemplate : UnityEditor.AssetModificationProcessor
    {
        private static void OnWillCreateAsset(string path)
        { 
            path = path.Replace(".meta", "");
            if (path.EndsWith(".cs"))
            {
                string str = File.ReadAllText(path);
                str = str.Replace("#CreateAuthor#", Environment.UserName).Replace(
                                  "#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
                File.WriteAllText(path, str);
            }
        }
        public static void StartRegisterTemplate()
        {
            string prth = EditorApplication.applicationContentsPath + "/Resources/ScriptTemplates/";
            string srcPath = Application.dataPath + "/_Editor/FutureEditorTool/Editor/StriptTemplateTool/Template";
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i.Name.EndsWith(".meta")) continue;
                    if (!(i is DirectoryInfo))     //判断是否文件夹
                    {
                        File.Copy(i.FullName, prth + i.Name, true);
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.LogError("注册C#模版出错");
                throw e;
            }
            LogUtil.Log("[FC Register]注册C#脚本模版完成");
            AssetDatabase.Refresh();
        }


    }
}