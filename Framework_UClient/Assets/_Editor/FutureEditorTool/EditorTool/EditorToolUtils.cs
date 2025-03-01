/****************************************************
    文件: EditorUITool.cs
    作者: Clear
    日期: 2025/3/1 15:21:37
    类型: 逻辑脚本
    功能: EditorUtils
*****************************************************/
using UnityEngine;
using UnityEditor;

namespace FutureEditor
{
    public static class EditorToolUtils
    {
        
        public static Texture2D GetEditorUI(string name)
        {
            string path =  UnityEditorPathConst.EditorTexturePath+"/"+name;
     
            Texture2D githubIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (githubIcon == null)
            {
                Debug.LogWarning(name+"不存在："+path);
            }
            return githubIcon;
        }



    }
}