/****************************************************
    文件: GameToolView.cs
    作者: Clear
    日期: 2026/3/4 20:38:16
    类型: 逻辑脚本
    功能: 项目数据统计视图 + AppFacade配置编辑器 + AppConst配置编辑器
*****************************************************/
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using FutureCore;
using ProjectApp;
using UnityEngine.Profiling;

namespace FutureEditor
{
    public static class GameToolView
    {
        private static Vector2 gameToolLeftScrollPos;
        private static Vector2 gameToolRightScrollPos;
        private static Vector2 appConstScrollPos;
        private static Vector2 appFacadeScrollPos;

        // 项目统计数据
        private static string[] projectStats = new string[12];
        private static string[] recentActivities = new string[]
        {
            "10:23 - 场景 'Main' 已保存",
            "10:15 - 资源 'Player.prefab' 已修改",
            "09:58 - 脚本 'GameManager.cs' 已添加",
            "09:42 - 构建 AssetBundle 完成",
            "09:30 - 项目已打开",
        };

        // 系统信息
        private static string[] systemInfo = new string[6];
        private static string[] performanceMetrics = new string[6];

        // AppFacade配置相关
        private static Dictionary<string, FieldInfo> appFacadeFields = new Dictionary<string, FieldInfo>();
        private static Dictionary<string, object> appFacadeValues = new Dictionary<string, object>();
        private static Dictionary<string, object> appFacadeOriginalValues = new Dictionary<string, object>();
        private static Dictionary<string, string> appFacadeDescriptions = new Dictionary<string, string>();
        private static bool isAppFacadeDirty = false;
        private static string appFacadeFilePath = "Assets/_App/ProjectApp/ProjectCore/App/AppFacade.cs";

        // AppConst配置相关
        private static Dictionary<string, FieldInfo> appConstFields = new Dictionary<string, FieldInfo>();
        private static Dictionary<string, object> appConstValues = new Dictionary<string, object>();
        private static Dictionary<string, object> appConstOriginalValues = new Dictionary<string, object>();
        private static Dictionary<string, string> appConstDescriptions = new Dictionary<string, string>();
        private static bool isAppConstDirty = false;
        private static string appConstFilePath = "Assets/_FutureFrame/FutureCore/Define/Const/AppConst.cs";

        // AppConst字段黑名单（在AppFacade中出现过的字段）
        private static HashSet<string> appConstFieldBlacklist = new HashSet<string>
        {
            "AppName",
            "AppDesc",
            "PackageName",
            "AESKey",
            "AESIVector",
            "ServerTag",
            "WebSocketUrls",
            "WebSocketPort",
            "WebSocketTestPort",
            "Domain",
            "SDKApiPrefix",
            "BuglyAppIDForAndroid",
            "BuglyAppIDForiOS",
            "IsWeakNetwork",
            "IsOfflineGame",
            "CustomSDK",
            "IsUseUGameAndroid",
            "ServerAssestUrl",
            "InitFunc",
            "StartUpFunc",
            "GameStartFunc"
        };

        // 分类显示（仅用于AppConst）
        private static string[] appConstCategories = new string[]
        {
            "全部",
            "基础设置",
            "资源模式",
            "调试设置",
            "帧率设置",
            "UI设置",
            "语言设置",
            "版本设置",
            "其他设置"
        };
        private static int selectedCategory = 0;

        // 左侧菜单选中项
        private static string selectedMenuItem = "📊 项目总览";

        public static void InitData()
        {
            RefreshProjectStats();
            LoadAppFacadeFields();
            LoadAppConstFields();
            InitFieldDescriptions();
        }

        private static void InitFieldDescriptions()
        {
            // AppFacade字段说明
            appFacadeDescriptions["AppName"] = "应用代号";
            appFacadeDescriptions["AppDesc"] = "项目描述";
            appFacadeDescriptions["PackageName"] = "包名";
            appFacadeDescriptions["AESKey"] = "密钥Key";
            appFacadeDescriptions["AESIVector"] = "密钥IVector";
            appFacadeDescriptions["ServerTag"] = "服务器标签";
            appFacadeDescriptions["WebSocketUrls"] = "游戏服连接组";
            appFacadeDescriptions["WebSocketPort"] = "正服端口";
            appFacadeDescriptions["WebSocketTestPort"] = "测服端口";
            appFacadeDescriptions["Domain"] = "域名";
            appFacadeDescriptions["SDKApiPrefix"] = "SDK接口前缀";
            appFacadeDescriptions["BuglyAppIDForAndroid"] = "Bugly Android AppID";
            appFacadeDescriptions["BuglyAppIDForiOS"] = "Bugly iOS AppID";
            appFacadeDescriptions["IsWeakNetwork"] = "是否弱联网";
            appFacadeDescriptions["IsOfflineGame"] = "是否离线游戏";
            appFacadeDescriptions["CustomSDK"] = "自定义SDK";
            appFacadeDescriptions["IsUseUGameAndroid"] = "是否使用UGameAndroid编译";
            appFacadeDescriptions["ServerAssestUrl"] = "服务器资源路径";
            appFacadeDescriptions["InitFunc"] = "初始化前自定义设置";
            appFacadeDescriptions["StartUpFunc"] = "启动游戏自定义设置";
            appFacadeDescriptions["GameStartFunc"] = "开始游戏前自定义设置";

            // AppConst字段说明
            appConstDescriptions["IsDebugVersion"] = "是否是调试版本";
            appConstDescriptions["PackageVersion"] = "包版本号";
            appConstDescriptions["ConfigInternalHash"] = "内置配置表哈希值";
            appConstDescriptions["ConfigInternalVersion"] = "内置配置表版本号";
            appConstDescriptions["IsResourcesMode_Default"] = "默认是否使用Resources模式";
            appConstDescriptions["IsSyncLoadMode_Default"] = "默认是否使用同步加载模式";
            appConstDescriptions["ExcelConfig_UseDll"] = "数据表类文件是否使用Dll";
            appConstDescriptions["IsCheckResVer"] = "是否检测资源版本";
            appConstDescriptions["IsUseAssetBundlesLoad"] = "是否使用AB包加载资源";
            appConstDescriptions["IsUseReleaseAB"] = "是否使用正式安装路径下的AB包";
            appConstDescriptions["IsDevelopmentBuild"] = "是否是开发构建";
            appConstDescriptions["IsDevelopMode"] = "是否开发模式（不拷贝资源）";
            appConstDescriptions["HotUpdateType"] = "热更新代码模式";
            appConstDescriptions["IsEnabledEngineProfiler"] = "是否开启调试引擎分析器";
            appConstDescriptions["IsEnableAssistAppProfiler"] = "是否开启调试应用分析器";
            appConstDescriptions["IsEnableAssistAppConsole"] = "是否开启调试应用控制台";
            appConstDescriptions["IsEnabledEngineLog"] = "是否开启引擎调试日志";
            appConstDescriptions["IsEnabledLog"] = "是否开启日志";
            appConstDescriptions["EnabledFilterLogType"] = "启用的日志过滤类型";
            appConstDescriptions["IsRunInBG"] = "是否允许后台运行";
            appConstDescriptions["SleepTimeoutMode"] = "休眠超时模式";
            appConstDescriptions["AntiAliasing"] = "抗锯齿级别";
            appConstDescriptions["HighFrameRate"] = "高帧率目标值";
            appConstDescriptions["LowFrameRate"] = "低帧率目标值";
            appConstDescriptions["HDHighViewScale"] = "高清高视野缩放比例";
            appConstDescriptions["HDLowViewScale"] = "高清低视野缩放比例";
            appConstDescriptions["PixelsPerUnit"] = "每单位像素数";
            appConstDescriptions["FrameRateTimestep"] = "帧率时间步长";
            appConstDescriptions["LowFrameRateTimestep"] = "低帧率时间步长";
            appConstDescriptions["ABExtName"] = "AB包文件扩展名";
            appConstDescriptions["HasAssetPackage"] = "是否有资源包";
            appConstDescriptions["StandardResolution"] = "标准分辨率";
            appConstDescriptions["UIResolution"] = "UI分辨率";
            appConstDescriptions["PCTestResolution"] = "PC测试分辨率";
            appConstDescriptions["IsConfigEditorLoadInternally"] = "是否编辑器加载内置配置";
            appConstDescriptions["IsConfigRollback"] = "是否允许配置表回滚";
            appConstDescriptions["IsConfigPreInit"] = "是否配置表提前本地初始化";
            appConstDescriptions["UseInternalSetting"] = "是否使用内置设置";
            appConstDescriptions["IsMultiLanguage"] = "是否支持多语言";
            appConstDescriptions["DefaultLangue"] = "默认语言";
            appConstDescriptions["InternalLangue"] = "内置语言";
            appConstDescriptions["IsLoadingDelay"] = "Loading进度是否延迟";
            appConstDescriptions["LoadingDelayTime"] = "Loading进度延迟时间";
            appConstDescriptions["LoadingCompleteDelayTime"] = "Loading完成延迟时间";
            appConstDescriptions["GameStartReadyDelayTime"] = "游戏开始前准备延迟时间";
            appConstDescriptions["LogsViewerShowNum_Debug"] = "Debug模式下日志查看器显示数量";
            appConstDescriptions["LogsViewerShowNum_Release"] = "Release模式下日志查看器显示数量";
            appConstDescriptions["UIDriver"] = "UI驱动类型";
            appConstDescriptions["FGUIRatio"] = "Fgui和世界物体大小比例";
            appConstDescriptions["CtrlDisableList"] = "控制器禁用列表";
            appConstDescriptions["IsReleaseApp"] = "是否发布版应用";
            appConstDescriptions["AppVersions"] = "应用版本数组";
            appConstDescriptions["LocalAssetVersions"] = "本地资源版本";
            appConstDescriptions["ServerAssetVersions"] = "服务器资源版本";
            appConstDescriptions["LaunchDateTime"] = "应用启动时间";
        }

        private static void LoadAppFacadeFields()
        {
            appFacadeFields.Clear();
            appFacadeValues.Clear();
            appFacadeOriginalValues.Clear();

            Type appFacadeType = typeof(ProjectApp.AppFacade);
            FieldInfo[] fields = appFacadeType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (FieldInfo field in fields)
            {
                string fieldName = field.Name;
                appFacadeFields[fieldName] = field;

                try
                {
                    object value = field.GetValue(null);
                    appFacadeValues[fieldName] = value;
                    appFacadeOriginalValues[fieldName] = CloneValue(value);
                }
                catch (Exception e)
                {
                    Debug.LogError($"读取字段 {fieldName} 失败: {e.Message}");
                }
            }

            isAppFacadeDirty = false;
        }

        private static void LoadAppConstFields()
        {
            appConstFields.Clear();
            appConstValues.Clear();
            appConstOriginalValues.Clear();

            Type appConstType = typeof(AppConst);
            FieldInfo[] fields = appConstType.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                string fieldName = field.Name;

                // 跳过黑名单中的字段（在AppFacade中出现过的）
                if (appConstFieldBlacklist.Contains(fieldName))
                    continue;

                appConstFields[fieldName] = field;

                try
                {
                    object value = field.GetValue(null);
                    appConstValues[fieldName] = value;
                    appConstOriginalValues[fieldName] = CloneValue(value);
                }
                catch (Exception e)
                {
                    Debug.LogError($"读取字段 {fieldName} 失败: {e.Message}");
                }
            }

            isAppConstDirty = false;
        }

        private static object CloneValue(object value)
        {
            if (value == null) return null;

            Type type = value.GetType();

            // 处理数组类型
            if (type.IsArray)
            {
                Array arr = (Array)value;
                Array clone = (Array)arr.Clone();

                // 如果是多维数组或元素也是数组，需要深度克隆
                for (int i = 0; i < arr.Length; i++)
                {
                    object element = arr.GetValue(i);
                    if (element is Array)
                    {
                        clone.SetValue(CloneValue(element), i);
                    }
                }
                return clone;
            }

            // 处理List类型
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var list = (System.Collections.IList)value;
                var listType = typeof(List<>).MakeGenericType(type.GetGenericArguments()[0]);
                var clone = (System.Collections.IList)Activator.CreateInstance(listType);

                foreach (var item in list)
                {
                    clone.Add(item);
                }
                return clone;
            }

            // 值类型和字符串直接返回
            if (type.IsValueType || type == typeof(string))
                return value;

            // 其他引用类型返回原值（假设不可变）
            return value;
        }

        private static bool AreValuesEqual(object val1, object val2)
        {
            if (val1 == null && val2 == null) return true;
            if (val1 == null || val2 == null) return false;

            // 处理数组类型
            if (val1 is Array arr1 && val2 is Array arr2)
            {
                if (arr1.Length != arr2.Length) return false;
                for (int i = 0; i < arr1.Length; i++)
                {
                    object item1 = arr1.GetValue(i);
                    object item2 = arr2.GetValue(i);

                    // 递归比较数组中的每个元素
                    if (item1 is Array nestedArr1 && item2 is Array nestedArr2)
                    {
                        if (!AreValuesEqual(nestedArr1, nestedArr2))
                            return false;
                    }
                    else if (!Equals(item1, item2))
                    {
                        return false;
                    }
                }
                return true;
            }

            // 处理List类型
            if (val1 is System.Collections.IList list1 && val2 is System.Collections.IList list2)
            {
                if (list1.Count != list2.Count) return false;
                for (int i = 0; i < list1.Count; i++)
                {
                    if (!Equals(list1[i], list2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            return Equals(val1, val2);
        }

        private static void CheckAppFacadeDirty()
        {
            foreach (var kvp in appFacadeFields)
            {
                string fieldName = kvp.Key;
                if (appFacadeValues.ContainsKey(fieldName) && appFacadeOriginalValues.ContainsKey(fieldName))
                {
                    if (!AreValuesEqual(appFacadeValues[fieldName], appFacadeOriginalValues[fieldName]))
                    {
                        isAppFacadeDirty = true;
                        return;
                    }
                }
            }
            isAppFacadeDirty = false;
        }

        private static void CheckAppConstDirty()
        {
            foreach (var kvp in appConstFields)
            {
                string fieldName = kvp.Key;
                if (appConstValues.ContainsKey(fieldName) && appConstOriginalValues.ContainsKey(fieldName))
                {
                    if (!AreValuesEqual(appConstValues[fieldName], appConstOriginalValues[fieldName]))
                    {
                        isAppConstDirty = true;
                        return;
                    }
                }
            }
            isAppConstDirty = false;
        }

        public static void OnGUI(float contentHeight, float windowWidth)
        {
            UpdatePerformanceMetrics();

            GUILayout.BeginArea(new Rect(5, 25, windowWidth - 10, contentHeight), new GUIStyle("grey_border"));
            GUILayout.BeginArea(new Rect(10, 5, windowWidth - 40, contentHeight - 10));

            GUILayout.BeginVertical();

            GUILayout.Label("📊 项目数据统计中心", new GUIStyle(EditorStyles.boldLabel) { fontSize = 20, alignment = TextAnchor.MiddleCenter }, GUILayout.Height(35));
            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            DrawLeftMenu();
            GUILayout.Space(15);
            DrawRightContent();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
            GUILayout.EndArea();
        }

        private static void DrawLeftMenu()
        {
            GUILayout.BeginVertical(GUILayout.Width(160));
            GUILayout.Label("功能菜单", EditorStyles.boldLabel);
            GUILayout.Space(10);

            gameToolLeftScrollPos = GUILayout.BeginScrollView(gameToolLeftScrollPos, GUILayout.Width(160), GUILayout.ExpandHeight(true));

            string[] menuItems = new string[]
            {
                "📊 项目总览",
                "📦 AppFacade配置",
                "⚙️ AppConst配置",
                "📁 资源统计",
                "📝 代码统计",
                "🎮 场景统计",
                "⚙️ 性能分析",
                "📋 最近活动",
                "🔧 工具设置",
            };

            foreach (string item in menuItems)
            {
                GUI.backgroundColor = selectedMenuItem == item ? new Color(0.3f, 0.6f, 1f) : Color.white;
                if (GUILayout.Button(item, GUILayout.Height(30), GUILayout.Width(140)))
                {
                    selectedMenuItem = item;
                }
                GUI.backgroundColor = Color.white;
                GUILayout.Space(2);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private static void DrawRightContent()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            gameToolRightScrollPos = GUILayout.BeginScrollView(gameToolRightScrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            switch (selectedMenuItem)
            {
                case "📊 项目总览": DrawProjectOverview(); break;
                case "📦 AppFacade配置": DrawAppFacadeEditor(); break;
                case "⚙️ AppConst配置": DrawAppConstEditor(); break;
                case "📁 资源统计": DrawResourceStats(); break;
                case "📝 代码统计": DrawCodeStats(); break;
                case "🎮 场景统计": DrawSceneStats(); break;
                case "⚙️ 性能分析": DrawPerformanceAnalysis(); break;
                case "📋 最近活动": DrawRecentActivities(); break;
                case "🔧 工具设置": DrawToolSettings(); break;
                default: DrawProjectOverview(); break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private static void DrawProjectOverview()
        {
            GUILayout.Label("📊 项目总览", new GUIStyle(EditorStyles.boldLabel) { fontSize = 16 });
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            GUILayout.Label("📊 项目统计", EditorStyles.boldLabel);
            GUILayout.Space(8);
            foreach (string stat in projectStats) { if (!string.IsNullOrEmpty(stat)) GUILayout.Label(stat); }
            GUILayout.EndVertical();

            GUILayout.Space(15);

            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            GUILayout.Label("ℹ️ 系统信息", EditorStyles.boldLabel);
            GUILayout.Space(8);
            systemInfo[0] = $"Unity版本: {Application.unityVersion}";
            systemInfo[1] = $"目标平台: {EditorUserBuildSettings.activeBuildTarget}";
            systemInfo[2] = $"脚本后端: {GetScriptingBackend()}";
            systemInfo[3] = $"API级别: {GetApiCompatibilityLevel()}";
            systemInfo[4] = $"渲染管线: {GetRenderPipeline()}";
            systemInfo[5] = $"内存使用: {GetMemoryUsage()}";
            foreach (string info in systemInfo) GUILayout.Label(info);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            GUILayout.Label("⚡ 性能指标", EditorStyles.boldLabel);
            GUILayout.Space(8);
            foreach (string metric in performanceMetrics) GUILayout.Label(metric);
            GUILayout.EndVertical();

            GUILayout.Space(15);

            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            GUILayout.Label("📋 最近活动", EditorStyles.boldLabel);
            GUILayout.Space(8);
            foreach (string activity in recentActivities) GUILayout.Label($"• {activity}");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            DrawQuickActions();
        }

        private static void DrawAppFacadeEditor()
        {
            CheckAppFacadeDirty();

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.Label("📦 AppFacade 配置编辑器", EditorStyles.boldLabel, GUILayout.Height(25));
            GUILayout.FlexibleSpace();

            if (isAppFacadeDirty)
            {
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("保存修改", GUILayout.Width(80), GUILayout.Height(25)))
                {
                    SaveAppFacadeChanges();
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("刷新", GUILayout.Width(80), GUILayout.Height(25)))
                {
                    LoadAppFacadeFields();
                    Repaint();
                }
                GUI.backgroundColor = Color.white;
            }

            if (GUILayout.Button("打开文件", GUILayout.Width(80), GUILayout.Height(25)))
                UnityEditorTool.OpenScriptByPath(appFacadeFilePath);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"字段数量: {appFacadeFields.Count}", EditorStyles.miniLabel);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("📌 常量/静态字段均可编辑，保存自动更新文件", EditorStyles.miniLabel);
            GUILayout.EndVertical();
            GUILayout.Space(10);

            foreach (var field in appFacadeFields)
            {
                string fieldName = field.Key;
                FieldInfo fieldInfo = field.Value;
                if (fieldInfo.FieldType == typeof(Action) || fieldInfo.FieldType.Name.Contains("Action")) continue;

                GUILayout.BeginVertical("box");
                bool isConst = fieldInfo.IsLiteral && !fieldInfo.IsInitOnly;
                GUILayout.BeginHorizontal();
                GUI.color = isConst ? new Color(1f, 0.8f, 0.4f) : Color.white;
                GUILayout.Label((isConst ? "⚡ " : "") + fieldName, EditorStyles.boldLabel, GUILayout.Width(180));

                string desc = appFacadeDescriptions.ContainsKey(fieldName) ? appFacadeDescriptions[fieldName] : "暂无说明";
                GUILayout.Label($"【{desc}】", new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.6f, 0.8f, 0.6f) }, fontStyle = FontStyle.Italic }, GUILayout.Width(200));
                GUILayout.FlexibleSpace();
                GUILayout.Label(isConst ? $"【常量】({fieldInfo.FieldType.Name})" : $"({fieldInfo.FieldType.Name})", EditorStyles.miniLabel, GUILayout.Width(isConst ? 100 : 80));
                GUILayout.EndHorizontal();

                GUI.color = Color.white;
                GUILayout.Space(5);
                object curVal = appFacadeValues[fieldName];
                object newVal = DrawFieldEditor(fieldName, fieldInfo.FieldType, curVal);
                if (!AreValuesEqual(curVal, newVal))
                {
                    appFacadeValues[fieldName] = newVal;
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }

            GUILayout.EndVertical();
        }

        private static void DrawAppConstEditor()
        {
            CheckAppConstDirty();

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.Label("⚙️ AppConst 配置编辑器", EditorStyles.boldLabel, GUILayout.Height(25));
            GUILayout.FlexibleSpace();

            if (isAppConstDirty)
            {
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("保存修改", GUILayout.Width(80), GUILayout.Height(25)))
                {
                    SaveAppConstChanges();
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("刷新", GUILayout.Width(80), GUILayout.Height(25)))
                {
                    LoadAppConstFields();
                    Repaint();
                }
                GUI.backgroundColor = Color.white;
            }

            if (GUILayout.Button("打开文件", GUILayout.Width(80), GUILayout.Height(25)))
                UnityEditorTool.OpenScriptByPath(appConstFilePath);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("分类筛选:", GUILayout.Width(60));
            selectedCategory = EditorGUILayout.Popup(selectedCategory, appConstCategories, GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            GUILayout.Label($"字段数量: {appConstFields.Count}", EditorStyles.miniLabel);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("📌 修改后点击保存更新文件", EditorStyles.miniLabel);
            GUILayout.EndVertical();
            GUILayout.Space(10);

            foreach (var field in appConstFields)
            {
                string fieldName = field.Key;
                FieldInfo fieldInfo = field.Value;
                if (!ShouldShowField(fieldName, selectedCategory)) continue;

                GUILayout.BeginVertical("box");
                GUILayout.BeginHorizontal();
                GUILayout.Label(fieldName, EditorStyles.boldLabel, GUILayout.Width(180));
                string desc = appConstDescriptions.ContainsKey(fieldName) ? appConstDescriptions[fieldName] : "暂无说明";
                GUILayout.Label($"【{desc}】", new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.6f, 0.8f, 0.6f) }, fontStyle = FontStyle.Italic }, GUILayout.Width(200));
                GUILayout.FlexibleSpace();
                GUILayout.Label($"({fieldInfo.FieldType.Name})", EditorStyles.miniLabel, GUILayout.Width(80));
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                object curVal = appConstValues[fieldName];
                object newVal = DrawFieldEditor(fieldName, fieldInfo.FieldType, curVal);
                if (!AreValuesEqual(curVal, newVal))
                {
                    appConstValues[fieldName] = newVal;
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }

            GUILayout.EndVertical();
        }

        private static bool ShouldShowField(string fieldName, int category)
        {
            if (category == 0) return true;
            switch (category)
            {
                case 1: return fieldName.Contains("Package") || fieldName.Contains("Version") || fieldName.Contains("App");
                case 2: return fieldName.Contains("Resource") || fieldName.Contains("Asset") || fieldName.Contains("AB");
                case 3: return fieldName.Contains("Debug") || fieldName.Contains("Log") || fieldName.Contains("Profiler");
                case 4: return fieldName.Contains("Frame") || fieldName.Contains("FPS") || fieldName.Contains("Rate");
                case 5: return fieldName.Contains("UI") || fieldName.Contains("Resolution") || fieldName.Contains("FGUI");
                case 6: return fieldName.Contains("Lang") || fieldName.Contains("Language");
                case 7: return fieldName.Contains("Version") || fieldName.Contains("Ver") || fieldName.Contains("Hash");
                default: return true;
            }
        }

        private static object DrawFieldEditor(string fieldName, Type fieldType, object curValue)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);

            object result = curValue;

            if (fieldType == typeof(string))
            {
                GUILayout.Label("📝", GUILayout.Width(20));
                result = EditorGUILayout.TextField(curValue?.ToString() ?? "");
            }
            else if (fieldType == typeof(int))
            {
                GUILayout.Label("🔢", GUILayout.Width(20));
                result = EditorGUILayout.IntField(curValue != null ? (int)curValue : 0);
            }
            else if (fieldType == typeof(float))
            {
                GUILayout.Label("📊", GUILayout.Width(20));
                result = EditorGUILayout.FloatField(curValue != null ? (float)curValue : 0f);
            }
            else if (fieldType == typeof(bool))
            {
                GUILayout.Label("✅", GUILayout.Width(20));
                result = EditorGUILayout.Toggle(curValue != null && (bool)curValue);
            }
            else if (fieldType.IsEnum)
            {
                GUILayout.Label("📋", GUILayout.Width(20));
                result = EditorGUILayout.EnumPopup(curValue as Enum);
            }
            else if (fieldType == typeof(Vector2Int))
            {
                GUILayout.Label("📐", GUILayout.Width(20));
                result = EditorGUILayout.Vector2IntField("", curValue != null ? (Vector2Int)curValue : Vector2Int.zero);
            }
            else if (fieldType == typeof(Vector2))
            {
                GUILayout.Label("📐", GUILayout.Width(20));
                result = EditorGUILayout.Vector2Field("", curValue != null ? (Vector2)curValue : Vector2.zero);
            }
            else if (fieldType == typeof(string[]))
            {
                GUILayout.Label("📚", GUILayout.Width(20));
                if (curValue != null)
                {
                    string[] arr = (string[])curValue;
                    string s = EditorGUILayout.TextField(string.Join(", ", arr));
                    result = string.IsNullOrEmpty(s) ? arr : s.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    EditorGUILayout.LabelField("(null)");
                    result = curValue;
                }
            }
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                GUILayout.Label("📋", GUILayout.Width(20));
                EditorGUILayout.LabelField("(列表不支持编辑)");
                result = curValue;
            }
            else
            {
                GUILayout.Label("❓", GUILayout.Width(20));
                EditorGUILayout.LabelField($"不支持编辑");
                result = curValue;
            }

            GUILayout.EndHorizontal();
            return result;
        }

        private static void SaveAppFacadeChanges()
        {
            try
            {
                string path = Path.GetFullPath(appFacadeFilePath);
                if (!File.Exists(path))
                {
                    EditorUtility.DisplayDialog("错误", "文件不存在", "确定");
                    return;
                }

                string content = File.ReadAllText(path);

                foreach (var kvp in appFacadeValues)
                {
                    string fieldName = kvp.Key;
                    object value = kvp.Value;

                    if (!appFacadeFields.TryGetValue(fieldName, out FieldInfo fieldInfo))
                        continue;

                    // 生成保持原格式的字段代码
                    string replacement = GenerateFieldReplacementPreserveFormat(fieldInfo, value);

                    // 匹配字段的正则表达式
                    string pattern = $@"public\s+(const|static|readonly)?\s*\w+\s+{fieldName}\s*=\s*[^;]*;";
                    if (System.Text.RegularExpressions.Regex.IsMatch(content, pattern))
                    {
                        content = System.Text.RegularExpressions.Regex.Replace(content, pattern, replacement);
                    }
                }

                File.WriteAllText(path, content, Encoding.UTF8);
                AssetDatabase.Refresh();

                LoadAppFacadeFields();
                EditorUtility.DisplayDialog("成功", "AppFacade 已保存", "确定");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("错误", e.Message, "确定");
            }
        }

        private static void SaveAppConstChanges()
        {
            try
            {
                string path = Path.GetFullPath(appConstFilePath);
                if (!File.Exists(path))
                {
                    EditorUtility.DisplayDialog("错误", "文件不存在", "确定");
                    return;
                }

                string content = File.ReadAllText(path);

                foreach (var kvp in appConstValues)
                {
                    string fieldName = kvp.Key;
                    object value = kvp.Value;

                    if (!appConstFields.TryGetValue(fieldName, out FieldInfo fieldInfo))
                        continue;

                    // 生成保持原格式的字段代码
                    string replacement = GenerateFieldReplacementPreserveFormat(fieldInfo, value);

                    // 匹配字段的正则表达式
                    string pattern = $@"public\s+(const|static|readonly)?\s*\w+\s+{fieldName}\s*=\s*[^;]*;";
                    if (System.Text.RegularExpressions.Regex.IsMatch(content, pattern))
                    {
                        content = System.Text.RegularExpressions.Regex.Replace(content, pattern, replacement);
                    }
                }

                File.WriteAllText(path, content, Encoding.UTF8);
                AssetDatabase.Refresh();

                LoadAppConstFields();
                EditorUtility.DisplayDialog("成功", "AppConst 已保存", "确定");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("错误", e.Message, "确定");
            }
        }

        /// <summary>
        /// 生成保持原格式的字段代码
        /// </summary>
        private static string GenerateFieldReplacementPreserveFormat(FieldInfo fieldInfo, object value)
        {
            bool isConst = fieldInfo.IsLiteral && !fieldInfo.IsInitOnly;
            string modifier = isConst ? "const" : "static";
            Type fieldType = fieldInfo.FieldType;
            string fieldName = fieldInfo.Name;

            if (value == null)
                return $"public {modifier} {GetCSharpTypeName(fieldType)} {fieldName};";

            // 字符串
            if (fieldType == typeof(string))
            {
                return $"public {modifier} string {fieldName} = \"{value}\";";
            }

            // 布尔
            if (fieldType == typeof(bool))
            {
                bool b = (bool)value;
                return $"public {modifier} bool {fieldName} = {b.ToString().ToLower()};";
            }

            // 枚举 - 特殊处理位标志组合
            if (fieldType.IsEnum)
            {
                // 检查是否有FlagsAttribute
                bool hasFlags = fieldType.GetCustomAttribute<FlagsAttribute>() != null;

                if (hasFlags)
                {
                    // 处理位标志组合
                    long longValue = Convert.ToInt64(value);
                    if (longValue == 0)
                    {
                        return $"public {modifier} {fieldType.Name} {fieldName} = 0;";
                    }

                    // 分解位标志
                    List<string> flagNames = new List<string>();
                    Array enumValues = Enum.GetValues(fieldType);

                    // 对每个枚举值，检查是否在value中
                    foreach (object enumValue in enumValues)
                    {
                        long enumLong = Convert.ToInt64(enumValue);
                        if (enumLong != 0 && (longValue & enumLong) == enumLong)
                        {
                            flagNames.Add($"{fieldType.Name}.{enumValue}");
                        }
                    }

                    if (flagNames.Count > 0)
                    {
                        return $"public {modifier} {fieldType.Name} {fieldName} = {string.Join(" | ", flagNames)};";
                    }
                }

                // 普通枚举
                string enumValueStr = Enum.GetName(fieldType, value) ?? value.ToString();
                return $"public {modifier} {fieldType.Name} {fieldName} = {fieldType.Name}.{enumValueStr};";
            }

            // int
            if (fieldType == typeof(int))
            {
                return $"public {modifier} int {fieldName} = {value};";
            }

            // float
            if (fieldType == typeof(float))
            {
                float f = (float)value;
                // 检查是否是整数
                if (Math.Abs(f - Mathf.Round(f)) < 0.00001f)
                {
                    return $"public {modifier} float {fieldName} = {Mathf.RoundToInt(f)}f;";
                }
                return $"public {modifier} float {fieldName} = {f}f;";
            }

            // Vector2Int
            if (fieldType == typeof(Vector2Int))
            {
                Vector2Int v = (Vector2Int)value;
                return $"public {modifier} Vector2Int {fieldName} = new Vector2Int({v.x}, {v.y});";
            }

            // Vector2
            if (fieldType == typeof(Vector2))
            {
                Vector2 v = (Vector2)value;
                return $"public {modifier} Vector2 {fieldName} = new Vector2({v.x}f, {v.y}f);";
            }

            // 字符串数组
            if (fieldType == typeof(string[]))
            {
                string[] arr = (string[])value;
                if (arr.Length == 0)
                {
                    return $"public {modifier} string[] {fieldName};";
                }

                string arrayContent = string.Join(", ", Array.ConvertAll(arr, s => $"\"{s}\""));
                return $"public {modifier} string[] {fieldName} = new string[] {{ {arrayContent} }};";
            }

            // 泛型List
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = fieldType.GetGenericArguments()[0];
                return $"public {modifier} List<{GetCSharpTypeName(elementType)}> {fieldName} = new List<{GetCSharpTypeName(elementType)}>();";
            }

            // 默认格式
            return $"public {modifier} {GetCSharpTypeName(fieldType)} {fieldName} = {value};";
        }

        /// <summary>
        /// 获取C#类型名称（string, bool, int, float等）
        /// </summary>
        private static string GetCSharpTypeName(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(short)) return "short";
            if (type == typeof(long)) return "long";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(ushort)) return "ushort";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(char)) return "char";
            if (type == typeof(object)) return "object";
            if (type == typeof(decimal)) return "decimal";

            if (type.IsArray)
            {
                return GetCSharpTypeName(type.GetElementType()) + "[]";
            }

            if (type.IsGenericType)
            {
                string genericName = type.Name.Split('`')[0];
                string[] argNames = Array.ConvertAll(type.GetGenericArguments(), t => GetCSharpTypeName(t));
                return $"{genericName}<{string.Join(", ", argNames)}>";
            }

            return type.Name;
        }

        private static void Repaint()
        {
            if (EditorWindow.HasOpenInstances<FrameworkToolView>())
                EditorWindow.GetWindow<FrameworkToolView>().Repaint();
        }

        private static void DrawQuickActions()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("⚡ 快捷操作", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新统计数据", GUILayout.Height(35), GUILayout.Width(130))) RefreshProjectStats();
            if (GUILayout.Button("导出报告", GUILayout.Height(35), GUILayout.Width(130))) ExportStatisticsReport();
            if (GUILayout.Button("清理缓存", GUILayout.Height(35), GUILayout.Width(130))) CleanProjectCache();
            if (GUILayout.Button("运行分析", GUILayout.Height(35), GUILayout.Width(130))) RunDeepAnalysis();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static void ExportStatisticsReport()
        {
            string path = EditorUtility.SaveFilePanel("导出报告", "", "ProjectStats", "txt");
            if (!string.IsNullOrEmpty(path)) { File.WriteAllLines(path, new List<string>(projectStats), Encoding.UTF8); EditorUtility.RevealInFinder(path); }
        }

        private static void CleanProjectCache()
        {
            if (EditorUtility.DisplayDialog("清理缓存", "确定清理？", "确定", "取消"))
            {
                string temp = Path.Combine(Application.dataPath, "../Temp");
                if (Directory.Exists(temp)) Directory.Delete(temp, true);
                Debug.Log("缓存已清理");
            }
        }

        private static void RunDeepAnalysis() { RefreshProjectStats(); EditorUtility.DisplayDialog("完成", "分析完成", "确定"); }

        private static void RefreshProjectStats()
        {
            try
            {
                var cs = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
                int lines = 0; foreach (var f in cs) try { lines += File.ReadAllLines(f).Length; } catch { }
                projectStats[0] = $"脚本：{cs.Length}";
                projectStats[1] = $"代码行：{lines:N0}";
                projectStats[2] = $"场景：{Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories).Length}";
                projectStats[3] = $"预设：{Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories).Length}";
            }
            catch { }
        }

        private static void UpdatePerformanceMetrics()
        {
            performanceMetrics[0] = $"FPS：{Mathf.RoundToInt(1f / Time.deltaTime)}";
            performanceMetrics[1] = $"内存：{Profiler.GetTotalAllocatedMemoryLong() / 1048576} MB";
        }

        private static string GetScriptingBackend()
        {
#if UNITY_2020_1_OR_NEWER
            return PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup).ToString();
#else
            return PlayerSettings.scriptingRuntimeVersion.ToString();
#endif
        }

        private static string GetApiCompatibilityLevel()
        {
#if UNITY_2019_3_OR_NEWER
            return PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup).ToString();
#else
            return PlayerSettings.apiCompatibilityLevel.ToString();
#endif
        }

        private static string GetRenderPipeline()
        {
            return GraphicsSettings.currentRenderPipeline != null ? GraphicsSettings.currentRenderPipeline.name : "内置";
        }

        private static string GetMemoryUsage()
        {
            return $"{Profiler.GetTotalReservedMemoryLong() / 1048576f:F1} MB";
        }

        private static void DrawResourceStats()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("资源统计", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        private static void DrawCodeStats()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("代码统计", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        private static void DrawSceneStats()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("场景统计", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        private static void DrawPerformanceAnalysis()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("性能分析", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        private static void DrawRecentActivities()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("最近活动", EditorStyles.boldLabel);
            foreach (var a in recentActivities)
                GUILayout.Label($"• {a}");
            GUILayout.EndVertical();
        }

        private static void DrawToolSettings()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("工具设置", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }
    }
}