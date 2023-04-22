/****************************************************
    文件：ScriptableObjectTool.cs
	作者：Clear
    日期：2022/2/7 16:31:29
    类型: 编辑器脚本
	功能：创建ScriptableObject
*****************************************************/
using FutureCore;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class ScriptableObjectTool 
    {
        [MenuItem("[FC Tool]/ScriptableObject Tool/Cread UnityEditorPath")]
        public static void CreadUnityEditorPath()
        {
            CreadScriptableObject<UnityEditorPath>(UnityEditorPath.AsssetPath);     
        }



        public static void CreadScriptableObject<T>(string path) where T : ScriptableObject
        {
            Object val = ScriptableObject.CreateInstance<T>();
            path = path+@"\"+typeof(T).Name+".asset";
            AssetDatabase.CreateAsset(val, path);
        }


    }
}