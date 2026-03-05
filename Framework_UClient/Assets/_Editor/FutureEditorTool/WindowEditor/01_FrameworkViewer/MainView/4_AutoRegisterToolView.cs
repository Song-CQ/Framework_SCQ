/****************************************************
    文件: AutoRegisterToolView.cs
    作者: Clear
    日期: 2026/3/6
    类型: 工具脚本
    功能: 自动注册工具视图
*****************************************************/
using UnityEngine;
using UnityEditor;
using FutureCore;

namespace FutureEditor
{
    public static class AutoRegisterToolView
    {
        public static void InitData()
        {
            // 初始化数据（如果需要）
        }

        public static void OnGUI(float contentHeight, float windowWidth, System.Action closeAction)
        {
            GUILayout.BeginArea(new Rect(5, 25, windowWidth - 10, contentHeight), new GUIStyle("grey_border"));
            GUILayout.BeginArea(new Rect(10, 10, windowWidth - 40, contentHeight - 20));
            GUILayout.BeginVertical();

            GUILayout.Label("🔄 自动注册工具", EditorStyles.boldLabel, GUILayout.Height(30));
            GUILayout.Space(15);

            // 提示信息区域
            DrawGuideInfo();

            GUILayout.Space(20);

            // 两列布局
            GUILayout.BeginHorizontal();

            float colWidth = (windowWidth - 80) / 2;

            // 第一列 - 编辑器环境
            DrawEditorEnv(colWidth, closeAction);

            GUILayout.Space(20);

            // 第二列 - 项目数据
            DrawProjectData(colWidth, closeAction);

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
            GUILayout.EndArea();
        }

        private static void DrawGuideInfo()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("📌 新项目启动指南", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // 步骤1
            GUILayout.BeginHorizontal();
            GUILayout.Label("1️⃣", new GUIStyle(EditorStyles.boldLabel) { fontSize = 18, normal = { textColor = Color.white } }, GUILayout.Width(35));
            GUILayout.Label("首次打开项目，请先运行", GUILayout.Width(150));
            GUI.color = Color.green;
            GUILayout.Label("【编辑器环境注册】", GUILayout.Width(120));
            GUI.color = Color.white;
            GUILayout.Label("→ 注册编辑器工具、菜单、配置", GUILayout.Width(250));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // 步骤2
            GUILayout.BeginHorizontal();
            GUILayout.Label("2️⃣", new GUIStyle(EditorStyles.boldLabel) { fontSize = 18, normal = { textColor = Color.white } }, GUILayout.Width(35));
            GUILayout.Label("然后运行", GUILayout.Width(150));
            GUI.color = Color.cyan;
            GUILayout.Label("【项目环境注册】", GUILayout.Width(120));
            GUI.color = Color.white;
            GUILayout.Label("→ 注册项目数据、模块、配置", GUILayout.Width(250));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // 额外提示
            GUILayout.BeginHorizontal();
            GUILayout.Label("💡", GUILayout.Width(30));
            GUILayout.Label("以后每次拉取最新代码，建议重新运行这两步", GUILayout.Width(400));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private static void DrawEditorEnv(float colWidth, System.Action closeAction)
        {
            GUILayout.BeginVertical(GUILayout.Width(colWidth));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.color = Color.green;
            GUILayout.Label("▲ 第一步 ▲", EditorStyles.boldLabel, GUILayout.Width(100));
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.Label("⚙️ 编辑器环境注册", EditorStyles.boldLabel);
            GUILayout.Space(15);

            if (GUILayout.Button("注册编辑器环境", GUILayout.Height(50), GUILayout.Width(colWidth - 20)))
            {
                EditorAutoRegisterTool_Editor.AutoRegisterAll(closeAction);
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("注册编辑器相关的工具和配置", MessageType.Info);
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private static void DrawProjectData(float colWidth, System.Action closeAction)
        {
            GUILayout.BeginVertical(GUILayout.Width(colWidth));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.color = Color.cyan;
            GUILayout.Label("▲ 第二步 ▲", EditorStyles.boldLabel, GUILayout.Width(100));
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.Label("📁 项目环境注册", EditorStyles.boldLabel);
            GUILayout.Space(15);

            if (GUILayout.Button("自动注册项目数据", GUILayout.Height(50), GUILayout.Width(colWidth - 20)))
            {
                ProjectAutoRegisterTool.AutoRegisterAll(closeAction);
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("注册项目相关的数据和配置", MessageType.Info);
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
    }
}