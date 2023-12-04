/****************************************************
    文件: UGUIWnd_EditorView.cs
    作者: Clear
    日期: 2023/11/20 15:25:28
    类型: 框架核心脚本(请勿修改)
    功能: UGUI界面编辑器视图
*****************************************************/
using Sirenix.OdinInspector;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class UGUIWnd_EditorView
    {

        [MenuItem("Assets/[FC EditorTool]/[UGUI Editor]/[更新选中界面组件常量]", priority = -50), 
            MenuItem("GameObject/[FC EditorTool]/[UGUI Editor]/[更新选中界面组件常量]", priority = -50)]
        public static void AddUGUIEditorTool()
        {
            GameObject obj = Selection.activeGameObject;
            if (!obj)
            {
                return;
            }
            UpdateComponents(obj);
        }
        [MenuItem("Assets/[FC EditorTool]/[UGUI Editor]/[更新选中界面组件常量]", priority = -50, validate = true),
            MenuItem("GameObject/[FC EditorTool]/[UGUI Editor]/[更新选中界面组件常量]", priority = -50, validate = true)]
        public static bool Check()
        {
            GameObject obj = Selection.activeGameObject;
            if (!obj)
            {
                return false;
            }
            return true;

        }

        public static void UpdateComponents(GameObject obj)
        {
            
            string name = obj.name.Replace("_Plane","UI");
            string[] files = Directory.GetFiles(UnityEditorPathConst.ModuleUIPath, name + ".cs", SearchOption.AllDirectories);
            if (files.Length!=1)
            {
                Debug.LogError("界面不存在或名字重复:"+ name);
                return;
            }
            string path = files[0];
            string text = File.ReadAllText(path);

            if (!text.Contains("#region 控件常量"))
            {
                Debug.LogError("无控件常量标志: #region 控件常量");
                return;
            }

            int startIndex = text.IndexOf("#region 控件常量")+12;
            int endIndex = text.Substring(startIndex).IndexOf("#endregion")+ startIndex;
            string uiClassStr = MVC_CreadTool.Fill_UGUICont(obj);
            string allText = text.Substring(0, startIndex)+ "\n        " + uiClassStr+ "        " + text.Substring(endIndex, text.Length-endIndex);

            File.Delete(path);
            File.WriteAllText(path, allText, Encoding.UTF8);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}