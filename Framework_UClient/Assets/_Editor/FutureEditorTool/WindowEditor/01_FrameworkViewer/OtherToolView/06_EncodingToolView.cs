/****************************************************
    文件: EncodingToolView.cs
    作者: Clear
    日期: 2026/3/6
    类型: 工具脚本
    功能: 编码转换工具视图
*****************************************************/
using UnityEngine;
using UnityEditor;

namespace FutureEditor
{
    public static class EncodingToolView
    {
        private static Vector2 scrollPos;

        public static void InitData()
        {
            // 初始化数据（如果需要）
        }

        public static void OnGUI(System.Action closeAction)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.Label("🔤 编码转换工具", EditorStyles.boldLabel);
            GUILayout.Space(15);

            // ===== 编码检测工具 =====
            GUILayout.BeginVertical("box");
            GUILayout.Label("🔍 编码检测工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("检测所有脚本编码", GUILayout.Height(35)))
            {
                ConvertScriptsToUTF8.DetectScriptsEncoding();
            }

            GUILayout.Space(5);
            EditorGUILayout.HelpBox("检测项目中所有C#脚本的编码格式", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.Space(15);

            // ===== 编码转换工具 =====
            GUILayout.BeginVertical("box");
            GUILayout.Label("🔄 编码转换工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("转换所有脚本为UTF-8（无BOM）", GUILayout.Height(35)))
            {
                ConvertScriptsToUTF8.ConvertAllToUTF8NoBOM();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("转换所有脚本为UTF-8（带BOM）", GUILayout.Height(35)))
            {
                ConvertScriptsToUTF8.ConvertAllToUTF8WithBOM();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("检测带BOM的文件", GUILayout.Height(35)))
            {
                ConvertScriptsToUTF8.DetectBOMFiles();
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("无BOM跨平台兼容性好，推荐使用", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.Space(15);

            // ===== 强制转换工具 =====
            GUILayout.BeginVertical("box");
            GUILayout.Label("⚠️ 强制转换工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUI.color = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("强制移除所有BOM", GUILayout.Height(35)))
            {
                ConvertScriptsToUTF8.ForceRemoveAllBOM();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("强制转换所有为UTF-8无BOM", GUILayout.Height(35)))
            {
                ConvertScriptsToUTF8.ForceConvertAllToUTF8NoBOM();
            }
            GUI.color = Color.white;

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("强制转换可能导致文件乱码，请谨慎使用", MessageType.Warning);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }
    }
}