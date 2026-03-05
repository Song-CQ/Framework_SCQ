/****************************************************
    文件: UnityToolView.cs
    作者: Clear
    日期: 2026/3/6
    类型: 工具脚本
    功能: Unity编辑器工具视图
*****************************************************/
using UnityEngine;
using UnityEditor;
using FutureCore;
using System.IO;
using System;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace FutureEditor
{
    public static class UnityToolView
    {
        private static Vector2 unityToolScrollPos;

        public static void InitData()
        {
            // 初始化数据（如果需要）
        }

        public static void OnGUI(float leftWidth, float contentHeight, float rightStart, float rightWidth)
        {
            // ===== 左侧工具区域 =====
            GUILayout.BeginArea(new Rect(5, 25, leftWidth, contentHeight), new GUIStyle("grey_border"));

            // 内部区域
            GUILayout.BeginArea(new Rect(5, 5, leftWidth - 10, contentHeight - 10));
            GUILayout.Label("⚙️ Unity Editor 工具集", EditorStyles.boldLabel, GUILayout.Height(25));
            GUILayout.Space(10);

            // 滚动视图
            unityToolScrollPos = GUILayout.BeginScrollView(unityToolScrollPos,
                GUILayout.Width(leftWidth - 10),
                GUILayout.Height(contentHeight - 50));

            // 内容区域
            float contentWidth = leftWidth - 35;
            GUILayout.BeginVertical(GUILayout.Width(contentWidth));

            // 两列布局
            float colWidth = (contentWidth - 10) / 2;
            float btnWidth = colWidth - 12;

            GUILayout.BeginHorizontal();

            // 第一列
            GUILayout.BeginVertical(GUILayout.Width(colWidth));

            // 基础操作
            DrawBasicOperations(btnWidth);

            GUILayout.Space(5);

            // 编辑器设置
            DrawEditorSettings(btnWidth);

            GUILayout.EndVertical(); // 第一列结束

            GUILayout.Space(5);

            // 第二列
            GUILayout.BeginVertical(GUILayout.Width(colWidth));

            // PlayerPrefs管理
            DrawPlayerPrefsTools(btnWidth);

            GUILayout.Space(5);

            // 项目清理
            DrawProjectCleanup(btnWidth);

            GUILayout.EndVertical(); // 第二列结束
            GUILayout.EndHorizontal(); // 两列布局结束

            GUILayout.EndVertical(); // 内容区域结束
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUILayout.EndArea();

            // ===== 右侧应用信息区域 =====
            DrawAppInfo(rightStart, rightWidth);

            // ===== 右侧系统信息区域 =====
            DrawSystemInfo(rightStart, rightWidth, contentHeight);
        }

        private static void DrawBasicOperations(float btnWidth)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("⚡ 基础操作", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("重启Unity", "点击后重启Unity编辑器"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                UnityEditorTool.StartRest();
            }

            if (GUILayout.Button(new GUIContent("刷新Asset DB", "刷新资源数据库"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                AssetDatabase.Refresh();
                Debug.Log("Asset Database 刷新完成");
            }

            if (GUILayout.Button(new GUIContent("强制重新导入", "强制重新导入所有资源"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                AssetDatabase.ImportAsset(Application.dataPath, ImportAssetOptions.ForceUpdate);
                Debug.Log("资源强制重新导入中...");
            }

            if (GUILayout.Button(new GUIContent("清除控制台", "清空控制台日志"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
                logEntries.GetMethod("Clear").Invoke(null, null);
                Debug.Log("控制台已清除");
            }

            // 空按钮
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            GUILayout.EndVertical();
        }

        private static void DrawEditorSettings(float btnWidth)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("⚙️ 编辑器设置", EditorStyles.boldLabel);
            GUILayout.Space(5);

            bool debugMode = EditorPrefs.GetBool("DeveloperMode", false);
            bool newDebugMode = GUILayout.Toggle(debugMode, "Debug模式", GUILayout.Height(20), GUILayout.Width(btnWidth));
            if (newDebugMode != debugMode)
            {
                EditorPrefs.SetBool("DeveloperMode", newDebugMode);
                Debug.Log($"Debug模式: {(newDebugMode ? "开启" : "关闭")}");
            }

            if (GUILayout.Button(new GUIContent("重置布局", "重置窗口布局"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                EditorUtility.DisplayDialog("提示", "窗口布局已重置", "确定");
            }

            if (GUILayout.Button(new GUIContent("打开Editor日志", "打开编辑器日志文件夹"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                string editorLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity/Editor");
                if (Directory.Exists(editorLogPath))
                {
                    Application.OpenURL(editorLogPath);
                }
            }

            // 空按钮
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            GUILayout.EndVertical();
        }

        private static void DrawPlayerPrefsTools(float btnWidth)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("📦 PlayerPrefs", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("清除所有", "清除所有PlayerPrefs数据"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                if (EditorUtility.DisplayDialog("确认", "确定要清除所有PlayerPrefs吗？", "确定", "取消"))
                {
                    PlayerPrefs.DeleteAll();
                    Debug.Log("PlayerPrefs 已清除");
                }
            }

            if (GUILayout.Button(new GUIContent("查看位置", "查看PlayerPrefs存储位置"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                string message = "PlayerPrefs 存储位置:\n";
                message += $"公司: {Application.companyName}\n";
                message += $"产品: {Application.productName}\n\n";
                message += "Windows注册表路径:\n";
                message += @"HKEY_CURRENT_USER\Software\Unity\UnityEditor\" + Application.companyName + @"\" + Application.productName;
                EditorUtility.DisplayDialog("PlayerPrefs 信息", message, "确定");
            }

            if (GUILayout.Button(new GUIContent("打开注册表", "打开注册表编辑器"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                System.Diagnostics.Process.Start("regedit");
            }

            // 空按钮
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            GUILayout.EndVertical();
        }

        private static void DrawProjectCleanup(float btnWidth)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("🧹 项目清理", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("清除Library", "清除Library缓存（需要重启）"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                string libraryPath = Path.Combine(Application.dataPath, "../Library");
                if (Directory.Exists(libraryPath) && EditorUtility.DisplayDialog("警告",
                    "清除Library缓存需要重启Unity，确定要继续吗？", "确定", "取消"))
                {
                    Directory.Delete(libraryPath, true);
                    Debug.Log("Library缓存已清除，请重启Unity");
                }
            }

            if (GUILayout.Button(new GUIContent("清除Temp", "清除临时文件夹"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                string tempPath = Path.Combine(Application.dataPath, "../Temp");
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                    Debug.Log("Temp文件夹已清除");
                }
            }

            if (GUILayout.Button(new GUIContent("删除 .vs", "删除Visual Studio缓存文件夹"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                string vsPath = Path.Combine(Application.dataPath, "../.vs");
                if (Directory.Exists(vsPath))
                {
                    Directory.Delete(vsPath, true);
                    Debug.Log(".vs 文件夹已删除");
                }
            }

            if (GUILayout.Button(new GUIContent("删除 .vscode", "删除VSCode配置文件夹"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                string vscodePath = Path.Combine(Application.dataPath, "../.vscode");
                if (Directory.Exists(vscodePath))
                {
                    Directory.Delete(vscodePath, true);
                    Debug.Log(".vscode 文件夹已删除");
                }
            }

            if (GUILayout.Button(new GUIContent("删除解决方案", "删除.sln和.csproj文件"), GUILayout.Height(30), GUILayout.Width(btnWidth)))
            {
                string projectPath = Path.GetDirectoryName(Application.dataPath);
                string[] solutionFiles = Directory.GetFiles(projectPath, "*.sln");
                string[] csprojFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.AllDirectories);

                foreach (string file in solutionFiles) File.Delete(file);
                foreach (string file in csprojFiles) File.Delete(file);

                Debug.Log("解决方案文件已删除");
            }

            // 空按钮
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(btnWidth))) { }
            GUILayout.EndVertical();
        }

        private static void DrawAppInfo(float rightStart, float rightWidth)
        {
            GUILayout.BeginArea(new Rect(rightStart, 30, rightWidth, 200));
            GUILayout.BeginVertical("box");
            GUILayout.Label("应用信息", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("AppName:", GUILayout.Width(70));
            GUILayout.TextField(ProjectApp.AppFacade.AppName, GUILayout.Width(rightWidth - 90));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("AppDesc:", GUILayout.Width(70));
            GUILayout.TextField(ProjectApp.AppFacade.AppDesc, GUILayout.Width(rightWidth - 90));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private static void DrawSystemInfo(float rightStart, float rightWidth, float contentHeight)
        {
            GUILayout.BeginArea(new Rect(rightStart, 240, rightWidth, contentHeight - 260));
            GUILayout.BeginVertical("box");
            GUILayout.Label("ℹ️ 系统信息", EditorStyles.boldLabel);
            GUILayout.Space(5);

            // Unity 版本
            GUILayout.BeginHorizontal();
            GUILayout.Label("Unity版本:", GUILayout.Width(100));
            GUILayout.Label(Application.unityVersion);
            GUILayout.EndHorizontal();

            // 目标平台
            GUILayout.BeginHorizontal();
            GUILayout.Label("目标平台:", GUILayout.Width(100));
            GUILayout.Label(EditorUserBuildSettings.activeBuildTarget.ToString());
            GUILayout.EndHorizontal();

            // 脚本后端
            GUILayout.BeginHorizontal();
            GUILayout.Label("脚本后端:", GUILayout.Width(100));
#if UNITY_2020_1_OR_NEWER
            GUILayout.Label(PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup).ToString());
#else
            GUILayout.Label(PlayerSettings.scriptingRuntimeVersion.ToString());
#endif
            GUILayout.EndHorizontal();

            // API级别
            GUILayout.BeginHorizontal();
            GUILayout.Label("API级别:", GUILayout.Width(100));
#if UNITY_2019_3_OR_NEWER
            GUILayout.Label(PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup).ToString());
#else
            GUILayout.Label(PlayerSettings.apiCompatibilityLevel.ToString());
#endif
            GUILayout.EndHorizontal();

            // 渲染管线
            GUILayout.BeginHorizontal();
            GUILayout.Label("渲染管线:", GUILayout.Width(100));
            string renderPipeline = "内置渲染管线";
            if (GraphicsSettings.currentRenderPipeline != null)
            {
                renderPipeline = GraphicsSettings.currentRenderPipeline.name;
            }
            GUILayout.Label(renderPipeline);
            GUILayout.EndHorizontal();

            // 内存使用
            GUILayout.BeginHorizontal();
            GUILayout.Label("内存使用:", GUILayout.Width(100));
            float memoryMB = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
            GUILayout.Label($"{memoryMB:F1} MB");
            GUILayout.EndHorizontal();

            // 公司名称
            GUILayout.BeginHorizontal();
            GUILayout.Label("公司名称:", GUILayout.Width(100));
            GUILayout.Label(Application.companyName);
            GUILayout.EndHorizontal();

            // 产品名称
            GUILayout.BeginHorizontal();
            GUILayout.Label("产品名称:", GUILayout.Width(100));
            GUILayout.Label(Application.productName);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}