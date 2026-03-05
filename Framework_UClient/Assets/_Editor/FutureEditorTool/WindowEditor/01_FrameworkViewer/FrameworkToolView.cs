using System;
using UnityEditor;
using UnityEngine;
using FutureCore;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace FutureEditor
{
    public class FrameworkToolView : EditorWindow
    {
        [MenuItem("GameObject/[Open Framework View]", false, -1000)]
        private static void GoOpenFrameworkWin()
        {
            OpenFrameworkWin();
        }
        [MenuItem("Assets/[Open Framework View]", false, -1000)]
        private static void AssOpenFrameworkWin()
        {
            OpenFrameworkWin();
        }
        [MenuItem("[FC Window]/框架工具窗口", false, -100)]
        public static void OpenFrameworkWin()
        {
            CreateWindow<FrameworkToolView>("Framework Tool");
        }

        private void OnEnable()
        {
            // 黄金比例 1.618:1 (890x550)
            minSize = new Vector2(890, 550);
            maxSize = minSize;
            InitData();
            Show();
            Focus();
        }

        private void InitData()
        {
            toolbatVal = new string[System.Enum.GetValues(typeof(ShowType)).Length];
            System.Collections.IList list = System.Enum.GetValues(typeof(ShowType));
            for (int i = 0; i < list.Count; i++)
            {
                ShowType t = (ShowType)list[i];
                toolbatVal[i] = t.ToString();
            }

            OnInitOtherToolData();

            // 初始化GameTool数据
            GameToolView.InitData();
        }

        private string[] toolbatVal;
        private int toolbatIndex = 0;

        // 悬停状态变量
        private bool isGearHovered = false;
        private Rect gearRect;

        private void OnGUI()
        {
            // 在右上角添加齿轮菜单
            ShowButton(new Rect(position.width - 60, 5, 20, 20));

            toolbatIndex = GUILayout.Toolbar(toolbatIndex, toolbatVal);

            switch (toolbatIndex)
            {
                case (int)ShowType.UnityTool:
                    RefreshUI_UnityTool();
                    break;
                case (int)ShowType.GameTool:
                    RefreshUI_GameTool();
                    break;
                case (int)ShowType.CodeGenTool:
                    RefreshUI_CodeGenTool();
                    break;
                case (int)ShowType.AutoRegisterTool:
                    RefreshUI_AutoRegisterTool();
                    break;
                case (int)ShowType.OtherTool:
                    RefreshUI_OtherTool();
                    break;
            }

            // 在所有界面底部显示工具集提示
            DrawBottomTooltip();
        }

        private void DrawBottomTooltip()
        {
            GUILayout.BeginArea(new Rect(0, position.height - 18, position.width, 20));
            GUILayout.Label("--- 工具集 v2.0 ---", EditorStyles.centeredGreyMiniLabel);
            GUILayout.EndArea();
        }

        private void ShowButton(Rect position)
        {
            gearRect = position;

            // 扩大悬停检测区域（增加10像素的边界）
            Rect hoverRect = new Rect(gearRect.x - 5, gearRect.y - 5, gearRect.width + 10, gearRect.height + 10);

            // 获取当前鼠标位置
            Vector2 mousePos = Event.current.mousePosition;

            // 检查鼠标是否在扩大后的区域内
            bool newHoverState = hoverRect.Contains(mousePos);

            // 如果悬停状态改变，需要重绘
            if (newHoverState != isGearHovered)
            {
                isGearHovered = newHoverState;
                Repaint();
            }

            // 绘制齿轮按钮背景（只在齿轮图标范围内绘制，保持视觉一致）
            if (isGearHovered)
            {
                // 背景还是在原齿轮范围内绘制，不会跟着扩大
                EditorGUI.DrawRect(gearRect, new Color(0.3f, 0.3f, 0.3f, 0.5f));
            }

            // 绘制齿轮图标
            GUI.color = isGearHovered ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            GUIContent gearContent = EditorGUIUtility.IconContent("_Popup");
            if (GUI.Button(gearRect, gearContent, GUIStyle.none))
            {
                ShowGearMenu();
            }
            GUI.color = Color.white;
        }

        private void ShowGearMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("编辑该界面脚本"), false, () =>
            {
                UnityEditorTool.OpenScriptByPath("Assets/_Editor/FutureEditorTool/WindowEditor/01_FrameworkViewer/FrameworkToolView");
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("关于"), false, () =>
            {
                EditorUtility.DisplayDialog("关于", "框架工具窗口 v1.0", "确定");
            });
            menu.ShowAsContext();
        }

        private enum ShowType
        {
            UnityTool = 0,
            GameTool,
            CodeGenTool,
            AutoRegisterTool,
            OtherTool,
        }

        #region UnityTool
        private Vector2 unityToolScrollPos;

        private void RefreshUI_UnityTool()
        {
            // ===== 左侧工具区域 - 宽度370，两列布局，高度500，y=25 =====
            GUILayout.BeginArea(new Rect(10, 25, 370, 500), new GUIStyle("grey_border"));

            // 内部区域
            GUILayout.BeginArea(new Rect(5, 5, 360, 490));
            GUILayout.Label("⚙️ Unity Editor 工具集", EditorStyles.boldLabel, GUILayout.Height(25));
            GUILayout.Space(10);

            // 添加滚动视图 - 高度设置为470，几乎贴合底部
            unityToolScrollPos = GUILayout.BeginScrollView(unityToolScrollPos, GUILayout.Width(360), GUILayout.Height(470));

            // 内容区域 - 宽度设为350，小于360，避免横向滚动条
            GUILayout.BeginVertical(GUILayout.Width(350));

            // ===== 两列布局开始 =====
            GUILayout.BeginHorizontal();

            // 第一列
            GUILayout.BeginVertical(GUILayout.Width(165));

            // ===== 基础操作 =====
            GUILayout.BeginVertical("box");
            GUILayout.Label("⚡ 基础操作", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (GUILayout.Button("重启Unity", GUILayout.Height(30), GUILayout.Width(145)))
            {
                UnityEditorTool.StartRest();
            }

            if (GUILayout.Button("刷新Asset DB", GUILayout.Height(30), GUILayout.Width(145)))
            {
                AssetDatabase.Refresh();
                Debug.Log("Asset Database 刷新完成");
            }

            if (GUILayout.Button("强制重新导入", GUILayout.Height(30), GUILayout.Width(145)))
            {
                AssetDatabase.ImportAsset(Application.dataPath, ImportAssetOptions.ForceUpdate);
                Debug.Log("资源强制重新导入中...");
            }

            if (GUILayout.Button("清除控制台", GUILayout.Height(30), GUILayout.Width(145)))
            {
                var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
                logEntries.GetMethod("Clear").Invoke(null, null);
                Debug.Log("控制台已清除");
            }

            // 添加空按钮让滚动条出现
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // ===== 编辑器设置 =====
            GUILayout.BeginVertical("box");
            GUILayout.Label("⚙️ 编辑器设置", EditorStyles.boldLabel);
            GUILayout.Space(5);

            bool debugMode = EditorPrefs.GetBool("DeveloperMode", false);
            bool newDebugMode = GUILayout.Toggle(debugMode, "Debug模式", GUILayout.Height(20), GUILayout.Width(145));
            if (newDebugMode != debugMode)
            {
                EditorPrefs.SetBool("DeveloperMode", newDebugMode);
                Debug.Log($"Debug模式: {(newDebugMode ? "开启" : "关闭")}");
            }

            if (GUILayout.Button("重置布局", GUILayout.Height(30), GUILayout.Width(145)))
            {
                EditorUtility.DisplayDialog("提示", "窗口布局已重置", "确定");
            }

            if (GUILayout.Button("打开Editor日志", GUILayout.Height(30), GUILayout.Width(145)))
            {
                string editorLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity/Editor");
                if (Directory.Exists(editorLogPath))
                {
                    Application.OpenURL(editorLogPath);
                }
            }

            // 添加空按钮让滚动条出现
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            GUILayout.EndVertical();

            GUILayout.EndVertical(); // 第一列结束

            GUILayout.Space(5);

            // 第二列
            GUILayout.BeginVertical(GUILayout.Width(165));

            // ===== PlayerPrefs管理 =====
            GUILayout.BeginVertical("box");
            GUILayout.Label("📦 PlayerPrefs", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (GUILayout.Button("清除所有", GUILayout.Height(30), GUILayout.Width(145)))
            {
                if (EditorUtility.DisplayDialog("确认", "确定要清除所有PlayerPrefs吗？", "确定", "取消"))
                {
                    PlayerPrefs.DeleteAll();
                    Debug.Log("PlayerPrefs 已清除");
                }
            }

            if (GUILayout.Button("查看位置", GUILayout.Height(30), GUILayout.Width(145)))
            {
                string message = "PlayerPrefs 存储位置:\n";
                message += $"公司: {Application.companyName}\n";
                message += $"产品: {Application.productName}\n\n";
                message += "Windows注册表路径:\n";
                message += @"HKEY_CURRENT_USER\Software\Unity\UnityEditor\" + Application.companyName + @"\" + Application.productName;
                EditorUtility.DisplayDialog("PlayerPrefs 信息", message, "确定");
            }

            if (GUILayout.Button("打开注册表", GUILayout.Height(30), GUILayout.Width(145)))
            {
                System.Diagnostics.Process.Start("regedit");
            }

            // 添加空按钮让滚动条出现
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // ===== 项目清理 =====
            GUILayout.BeginVertical("box");
            GUILayout.Label("🧹 项目清理", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (GUILayout.Button("清除Library", GUILayout.Height(30), GUILayout.Width(145)))
            {
                string libraryPath = Path.Combine(Application.dataPath, "../Library");
                if (Directory.Exists(libraryPath) && EditorUtility.DisplayDialog("警告",
                    "清除Library缓存需要重启Unity，确定要继续吗？", "确定", "取消"))
                {
                    Directory.Delete(libraryPath, true);
                    Debug.Log("Library缓存已清除，请重启Unity");
                }
            }

            if (GUILayout.Button("清除Temp", GUILayout.Height(30), GUILayout.Width(145)))
            {
                string tempPath = Path.Combine(Application.dataPath, "../Temp");
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                    Debug.Log("Temp文件夹已清除");
                }
            }

            if (GUILayout.Button("删除 .vs", GUILayout.Height(30), GUILayout.Width(145)))
            {
                string vsPath = Path.Combine(Application.dataPath, "../.vs");
                if (Directory.Exists(vsPath))
                {
                    Directory.Delete(vsPath, true);
                    Debug.Log(".vs 文件夹已删除");
                }
            }

            if (GUILayout.Button("删除 .vscode", GUILayout.Height(30), GUILayout.Width(145)))
            {
                string vscodePath = Path.Combine(Application.dataPath, "../.vscode");
                if (Directory.Exists(vscodePath))
                {
                    Directory.Delete(vscodePath, true);
                    Debug.Log(".vscode 文件夹已删除");
                }
            }

            if (GUILayout.Button("删除解决方案", GUILayout.Height(30), GUILayout.Width(145)))
            {
                string projectPath = Path.GetDirectoryName(Application.dataPath);
                string[] solutionFiles = Directory.GetFiles(projectPath, "*.sln");
                string[] csprojFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.AllDirectories);

                foreach (string file in solutionFiles) File.Delete(file);
                foreach (string file in csprojFiles) File.Delete(file);

                Debug.Log("解决方案文件已删除");
            }

            // 添加大量空按钮让滚动条出现
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            if (GUILayout.Button(" ", GUILayout.Height(30), GUILayout.Width(145))) { }
            GUILayout.EndVertical();

            GUILayout.EndVertical(); // 第二列结束
            GUILayout.EndHorizontal(); // 两列布局结束

            GUILayout.EndVertical(); // 内容区域结束

            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUILayout.EndArea();

            // ===== AppName 和 AppDesc 区域 - 保持原位置不变 =====
            GUILayout.BeginArea(new Rect(535, 30, 340, 200));
            GUILayout.BeginVertical("box");
            GUILayout.Label("📱 应用信息", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("AppName:", GUILayout.Width(70));
            GUILayout.TextField(ProjectApp.AppFacade.AppName, GUILayout.Width(250));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("AppDesc:", GUILayout.Width(70));
            GUILayout.TextField(ProjectApp.AppFacade.AppDesc, GUILayout.Width(250));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.EndArea();

            // ===== 系统信息区域 - 保持原位置不变 =====
            GUILayout.BeginArea(new Rect(535, 240, 340, 280));
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
        #endregion

        #region GameTool
        private void RefreshUI_GameTool()
        {
            // 给GameTool添加grey_border背景，高度500，y=25
            GUILayout.BeginArea(new Rect(10, 25, 870, 500), new GUIStyle("grey_border"));
            GUILayout.BeginArea(new Rect(10, 5, 850, 495));

            // 这里调用GameToolView.OnGUI
            GameToolView.OnGUI();

            GUILayout.EndArea();
            GUILayout.EndArea();
        }
        #endregion

        #region CodeGenTool
        private void RefreshUI_CodeGenTool()
        {
            // 扩张到全界面宽度，高度500，y=25
            GUILayout.BeginArea(new Rect(10, 25, 870, 500), new GUIStyle("grey_border"));
            GUILayout.BeginArea(new Rect(10, 5, 850, 495));
            GUILayout.BeginVertical();

            // 标题
            GUILayout.Label("📝 代码生成工具", EditorStyles.boldLabel, GUILayout.Height(30));
            GUILayout.Space(15);

            // 两列布局
            GUILayout.BeginHorizontal();

            // 第一列 - MVC工具
            GUILayout.BeginVertical(GUILayout.Width(400));
            GUILayout.BeginVertical("box");
            GUILayout.Label("MVC代码生成器", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("UI驱动类型:", GUILayout.Width(100));
            GUILayout.TextField("GUI", GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("创建GUI_MVC代码模版", GUILayout.Height(40), GUILayout.Width(380)))
            {
                MVC_CreadTool.OpenGUICread();
                Close();
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();

            GUILayout.Space(20);

            // 第二列 - 编码转换工具
            GUILayout.BeginVertical(GUILayout.Width(400));
            GUILayout.BeginVertical("box");
            GUILayout.Label("C#脚本编码转换", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("转换所有脚本为UTF-8（无BOM）", GUILayout.Height(35), GUILayout.Width(380)))
            {
                ConvertScriptsToUTF8.ConvertAllToUTF8NoBOM();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("转换所有脚本为UTF-8（带BOM）", GUILayout.Height(35), GUILayout.Width(380)))
            {
                ConvertScriptsToUTF8.ConvertAllToUTF8WithBOM();
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("无BOM跨平台兼容性好，推荐使用", MessageType.Info);
            GUILayout.EndVertical();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
            GUILayout.EndArea();
        }
        #endregion

        #region AutoRegisterTool
        private void RefreshUI_AutoRegisterTool()
        {
            // 扩张到全界面宽度，高度500，y=25
            GUILayout.BeginArea(new Rect(10, 25, 870, 500), new GUIStyle("grey_border"));
            GUILayout.BeginArea(new Rect(10, 5, 850, 495));
            GUILayout.BeginVertical();

            // 标题
            GUILayout.Label("🔄 自动注册工具", EditorStyles.boldLabel, GUILayout.Height(30));
            GUILayout.Space(20);

            // 两列布局
            GUILayout.BeginHorizontal();

            // 第一列 - 编辑器环境
            GUILayout.BeginVertical(GUILayout.Width(400));
            GUILayout.BeginVertical("box");
            GUILayout.Label("编辑器环境注册", EditorStyles.boldLabel);
            GUILayout.Space(15);

            if (GUILayout.Button("注册编辑器环境", GUILayout.Height(50), GUILayout.Width(380)))
            {
                EditorAutoRegisterTool_Editor.AutoRegisterAll(Close);
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("注册编辑器相关的工具和配置", MessageType.Info);
            GUILayout.EndVertical();
            GUILayout.EndVertical();

            GUILayout.Space(20);

            // 第二列 - 项目数据
            GUILayout.BeginVertical(GUILayout.Width(400));
            GUILayout.BeginVertical("box");
            GUILayout.Label("项目数据注册", EditorStyles.boldLabel);
            GUILayout.Space(15);

            if (GUILayout.Button("自动注册项目数据", GUILayout.Height(50), GUILayout.Width(380)))
            {
                ProjectAutoRegisterTool.AutoRegisterAll(Close);
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("注册项目相关的数据和配置", MessageType.Info);
            GUILayout.EndVertical();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
            GUILayout.EndArea();
        }
        #endregion

        #region OtherTool
        private Vector2 otherToolPos;
        private Vector2 otherToolPos2;
        private string selectOtherToolKey;
        private Color selectColor = new Color(100 / 255f, 207 / 255f, 255 / 255f);

        private Dictionary<string, Action> OtherTooDic = new Dictionary<string, Action>();

        private void RefreshUI_OtherTool()
        {
            // 左边按钮列表 - 高度500，y=25
            GUILayout.BeginArea(new Rect(5, 25, 185, 500), new GUIStyle("grey_border"));

            GUILayout.BeginArea(new Rect(0, 2, 182, 495));
            otherToolPos = GUILayout.BeginScrollView(otherToolPos, false, true, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);

            foreach (var item in OtherTooDic)
            {
                if (selectOtherToolKey == string.Empty)
                {
                    selectOtherToolKey = item.Key;
                }

                GUI.backgroundColor = Color.white;
                if (selectOtherToolKey == item.Key)
                {
                    GUI.backgroundColor = selectColor;
                }

                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.alignment = TextAnchor.MiddleLeft;
                buttonStyle.padding = new RectOffset(10, 5, 2, 2);

                if (GUILayout.Button(item.Key, buttonStyle, GUILayout.Width(160), GUILayout.Height(30)))
                {
                    selectOtherToolKey = item.Key;
                }

                GUI.backgroundColor = Color.white;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUILayout.EndArea();

            // 右边界面 - 高度500，y=25
            GUILayout.BeginArea(new Rect(200, 25, 680, 500), new GUIStyle("FrameBox"));

            otherToolPos2 = GUILayout.BeginScrollView(otherToolPos2);

            string displayTitle = selectOtherToolKey;
            if (selectOtherToolKey.StartsWith("[") && selectOtherToolKey.Contains("]"))
            {
                displayTitle = selectOtherToolKey;
            }
            GUILayout.Label(displayTitle, new GUIStyle("CN StatusWarn"));

            switch (selectOtherToolKey)
            {
                case "[01] Excel Tool":
                    ExcelToolView.OnGUI(Close);
                    break;
                case "[02] Path Tool":
                    PathToolView.OnGUI(Close);
                    break;
                case "[03] AssetBundle Tool":
                    AssetBundleToolView.OnGUI(Close);
                    break;
                case "[04] SVN Tool":
                    SVNToolView.OnGUI(Close);
                    break;
                case "[05] ILRuntime Tool":
                    ILRuntimeToolView.OnGUI(Close);
                    break;
                default:
                    if (selectOtherToolKey.StartsWith("[Common_Info]"))
                    {
                        OnInfoDisplay();
                    }
                    else
                    {
                        OtherTooDic[selectOtherToolKey]?.Invoke();
                    }
                    break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void OnInitOtherToolData()
        {
            selectOtherToolKey = string.Empty;
            OtherTooDic.Clear();

            OtherTooDic.Add("[01] Excel Tool", null);
            OtherTooDic.Add("[02] Path Tool", null);
            OtherTooDic.Add("[03] AssetBundle Tool", null);
            OtherTooDic.Add("[04] SVN Tool", null);
            OtherTooDic.Add("[05] ILRuntime Tool", null);

            for (int i = 0; i < 30; i++)
            {
                OtherTooDic.Add($"[Common_Info] {(i + 1):D2}", OnInfoDisplay);
            }

            ExcelToolView.InitData();
            PathToolView.InitData();
            AssetBundleToolView.InitData();
            SVNToolView.InitData();
            ILRuntimeToolView.InitData();
        }
        #endregion

        #region 通用信息显示
        private Vector2 infoScrollPos;
        private string searchFilter = "";
        private string[] logMessages = new string[]
        {
            "[INFO] 系统初始化完成 - 2024/1/15 10:23:45",
            "[INFO] 资源加载完成 - 2024/1/15 10:23:46",
            "[WARNING] 配置文件缺失，使用默认配置 - 2024/1/15 10:23:47",
            "[ERROR] 网络连接超时，重试中... - 2024/1/15 10:23:48",
            "[INFO] 重新连接成功 - 2024/1/15 10:23:50",
            "[DEBUG] 玩家数据加载: 1000ms - 2024/1/15 10:23:51",
            "[INFO] 场景切换完成 - 2024/1/15 10:23:52",
            "[WARNING] 材质丢失，使用默认材质 - 2024/1/15 10:23:53",
            "[INFO] 音频系统初始化 - 2024/1/15 10:23:54",
            "[INFO] 输入系统初始化 - 2024/1/15 10:23:55",
            "[ERROR] 着色器编译失败 - 2024/1/15 10:23:56",
            "[INFO] 重新编译着色器 - 2024/1/15 10:23:57",
            "[INFO] 编译成功 - 2024/1/15 10:23:58",
        };

        private void OnInfoDisplay()
        {
            GUILayout.BeginVertical(GUILayout.Width(650));

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("🔍 搜索:", GUILayout.Width(50));
            searchFilter = GUILayout.TextField(searchFilter, EditorStyles.toolbarTextField, GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("刷新", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                Debug.Log("刷新信息");
            }
            if (GUILayout.Button("清空日志", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                Debug.Log("清空日志");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            GUILayout.Label("📋 实时日志", EditorStyles.boldLabel);
            GUILayout.Space(5);

            infoScrollPos = GUILayout.BeginScrollView(infoScrollPos, GUILayout.Height(350));

            foreach (string log in logMessages)
            {
                if (string.IsNullOrEmpty(searchFilter) || log.ToLower().Contains(searchFilter.ToLower()))
                {
                    GUILayout.BeginHorizontal();

                    if (log.Contains("[ERROR]"))
                    {
                        GUI.color = Color.red;
                        GUILayout.Label("●", GUILayout.Width(15));
                    }
                    else if (log.Contains("[WARNING]"))
                    {
                        GUI.color = Color.yellow;
                        GUILayout.Label("●", GUILayout.Width(15));
                    }
                    else if (log.Contains("[INFO]"))
                    {
                        GUI.color = Color.green;
                        GUILayout.Label("●", GUILayout.Width(15));
                    }
                    else
                    {
                        GUI.color = Color.gray;
                        GUILayout.Label("●", GUILayout.Width(15));
                    }

                    GUI.color = Color.white;
                    GUILayout.Label(log);

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }
        #endregion
    }
}