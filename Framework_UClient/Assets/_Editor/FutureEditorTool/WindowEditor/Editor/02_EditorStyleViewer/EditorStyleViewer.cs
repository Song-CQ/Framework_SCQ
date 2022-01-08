using UnityEngine;
using UnityEditor;

namespace FutureEditor
{
    /// <summary>
    /// 编辑器样式预览器
    /// </summary>
    public class EditorStyleViewer : EditorWindow
    {
        static EditorWindow window;

        [MenuItem("Assets/Open Window/编辑器内置样式预览器")]
        static void OpenWindowAss()
        {
            OpenWindow();
        }

        [MenuItem("[FC Window]/Editor/编辑器内置样式预览器")]
        static void OpenWindow()
        {
            if (window == null)
            {
                window = CreateWindow<EditorStyleViewer>("编辑器内置样式预览器");
            }

            window.minSize = new Vector2(900, 300);
            window.Show();
            window.Focus();
        }

        Vector2 scrollPosition = Vector2.zero;
        string searchStr = "";

        private void OnGUI()
        {
            GUILayout.BeginHorizontal("helpbox");
            GUILayout.Label("查找内置样式：");
            searchStr = GUILayout.TextField(searchStr, "SearchTextField");
            if (GUILayout.Button("", "SearchCancelButton"))
            {
                searchStr = "";
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, "box");
            foreach (GUIStyle style in GUI.skin)
            {
                if (style.name.ToLower().Contains(searchStr.ToLower()))
                {
                    DrawStyle(style);
                }
            }

            GUILayout.EndScrollView();
        }

        void DrawStyle(GUIStyle style)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Button(style.name, style.name);
            GUILayout.FlexibleSpace();
            EditorGUILayout.SelectableLabel(style.name);
            if (GUILayout.Button("复制样式名称"))
            {
                EditorGUIUtility.systemCopyBuffer = style.name;
            }

            GUILayout.EndHorizontal();
        }

    }
}