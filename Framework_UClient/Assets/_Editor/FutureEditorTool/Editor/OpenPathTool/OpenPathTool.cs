/****************************************************
    文件：OpenPathTool.cs
	作者：Clear
    日期：2022/1/16 19:42:14
    类型: 逻辑脚本
	功能：打开目录工具
*****************************************************/
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class OpenPathTool
    {

        #region 打开编辑器的路劲
        [MenuItem("[FC OpenPath]/编辑器的目录", false,0)]
        public static void OpenEditorPath()
        {
            Application.OpenURL(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName+"/..");
        }

        #endregion
    }
}