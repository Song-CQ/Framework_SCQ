/****************************************************
    文件：MVC_CreadTool.cs
	作者：Clear
    日期：2022/1/26 17:22:11
    类型: 框架核心脚本(请勿修改)
	功能：MVC创建
*****************************************************/
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class MVC_CreadTool 
    {
        private static string outPath = Application.dataPath + "/_App/ProjectApp/Logic/ModuleUI";
        public static void OpenFGUICread()
        {
            EditorCreadWnd.ShowWindow("创建 FGUI MVC 代码模版", CreadFGUIMVC);

        }

        private static string CreadFGUIMVC(string name)
        {
            string[] nasmes = Directory.GetDirectories(outPath);
            foreach (var item in nasmes)
            {
                if (item.Replace(outPath, string.Empty) == name)
                {
                    return "已有同名模块";
                }
            }
            

            //Task task = Task.Run();


            return string.Empty;
        }
    }
}