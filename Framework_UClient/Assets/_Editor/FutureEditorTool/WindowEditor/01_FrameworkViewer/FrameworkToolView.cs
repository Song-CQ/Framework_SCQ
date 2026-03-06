/****************************************************
    文件: FrameworkToolView.cs
    作者: Clear
    日期: 2026/3/6
    类型: 编辑器窗口
    功能: 框架工具主视图 - 集成所有编辑器工具的中央控制台
    包含: Unity工具、游戏数据统计、代码生成、自动注册、其他工具集成
    快捷键: Ctrl+Shift+F 打开窗口
*****************************************************/

using System;
using UnityEditor;
using UnityEngine;
using FutureCore;
using UnityEngine.Profiling;

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
        [MenuItem("[FC Window]/框架工具窗口 _%#F", false, -100)]
        public static void OpenFrameworkWin()
        {
            CreateWindow<FrameworkToolView>("Framework Tool");
        }

        // 配置保存键
        private const string PREFS_KEY_SELECTED_TAB = "FrameworkToolView_SelectedTab";
        private const string PREFS_KEY_OTHER_FILTER = "FrameworkToolView_OtherFilter";

        private string[] toolbatVal;
        private int toolbatIndex = 0;

        // 悬停状态变量
        private bool isGearHovered = false;
        private Rect gearRect;

        // OtherTool搜索过滤
        private string otherToolSearchFilter = "";

        private void OnEnable()
        {
            minSize = new Vector2(890, 550);
            maxSize = minSize;

            toolbatIndex = EditorPrefs.GetInt(PREFS_KEY_SELECTED_TAB, 0);
            otherToolSearchFilter = EditorPrefs.GetString(PREFS_KEY_OTHER_FILTER, "");

            InitData();
            Show();
            Focus();
        }

        private void InitData()
        {
            toolbatVal = new string[Enum.GetValues(typeof(ShowType)).Length];
            var list = Enum.GetValues(typeof(ShowType));
            for (int i = 0; i < list.Length; i++)
            {
                ShowType t = (ShowType)list.GetValue(i);
                toolbatVal[i] = t.ToString();
            }

            // 初始化各个工具的数据
            GameToolView.InitData();
            UnityToolView.InitData();
            CodeGenToolView.InitData();
            AutoRegisterToolView.InitData();
            OtherToolView.InitData();
        }

        private void OnGUI()
        {
            try
            {
                // 在右上角添加齿轮菜单
                ShowButton(new Rect(position.width - 60, 5, 20, 20));

                int newIndex = GUILayout.Toolbar(toolbatIndex, toolbatVal);
                if (newIndex != toolbatIndex)
                {
                    toolbatIndex = newIndex;
                    EditorPrefs.SetInt(PREFS_KEY_SELECTED_TAB, toolbatIndex);
                }

                float contentHeight = position.height - 50;
                float leftWidth = 370;
                float rightStart = leftWidth + 160;
                float rightWidth = position.width - rightStart - 20;

                switch (toolbatIndex)
                {
                    case (int)ShowType.UnityTool:
                        UnityToolView.OnGUI(leftWidth, contentHeight, rightStart, rightWidth);
                        break;
                    case (int)ShowType.GameTool:
                        GameToolView.OnGUI(contentHeight, position.width);
                        break;
                    case (int)ShowType.CodeGenTool:
                        CodeGenToolView.OnGUI(contentHeight, position.width);
                        break;
                    case (int)ShowType.AutoRegisterTool:
                        AutoRegisterToolView.OnGUI(contentHeight, position.width, Close);
                        break;
                    case (int)ShowType.OtherTool:
                        OtherToolView.OnGUI(contentHeight, Close,
                             otherToolSearchFilter,
                            (filter) => EditorPrefs.SetString(PREFS_KEY_OTHER_FILTER, filter));
                        break;
                }

                DrawBottomStatusBar();
            }
            catch (Exception e)
            {
                DrawErrorPanel(e);
            }
        }

        private void DrawBottomStatusBar()
        {
            GUILayout.BeginArea(new Rect(0, position.height - 20, position.width, 25));
            EditorGUI.DrawRect(new Rect(0, 0, position.width, 25), new Color(0.24f, 0.24f, 0.24f));
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();

            GUILayout.Space(10);
            GUILayout.Label($"⚡ {Application.productName} | {EditorUserBuildSettings.activeBuildTarget} | 内存: {Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f):F1} MB",
                new GUIStyle(EditorStyles.label) { normal = { textColor = new Color(0.8f, 0.8f, 0.8f) } });

            GUILayout.FlexibleSpace();
            GUILayout.Label("--- 工具集 v2.0 ---",
                new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    normal = { textColor = new Color(0.7f, 0.7f, 0.7f) },
                    fontStyle = FontStyle.Normal
                });
            GUILayout.FlexibleSpace();

            // 添加快捷键提示
            GUILayout.Label("⌨️ Ctrl+Shift+F",
                new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = new Color(0.6f, 0.8f, 0.6f) },
                    fontStyle = FontStyle.Italic
                });
            GUILayout.Space(5);

            GUILayout.Label(System.DateTime.Now.ToString("HH:mm"),
                new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = new Color(0.6f, 0.6f, 0.6f) },
                    fontStyle = FontStyle.Italic
                });
            GUILayout.Space(10);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawErrorPanel(Exception e)
        {
            GUILayout.BeginArea(new Rect(10, 35, position.width - 20, position.height - 60));
            GUILayout.Label($"❌ 发生错误: {e.Message}", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label(e.StackTrace, EditorStyles.wordWrappedLabel);
            GUILayout.Space(20);
            if (GUILayout.Button("重新加载", GUILayout.Height(30)))
            {
                InitData();
                Repaint();
            }
            GUILayout.EndArea();
        }

        private void ShowButton(Rect position)
        {
            gearRect = position;
            Rect hoverRect = new Rect(gearRect.x - 5, gearRect.y - 5, gearRect.width + 10, gearRect.height + 10);
            bool newHoverState = hoverRect.Contains(Event.current.mousePosition);

            if (newHoverState != isGearHovered)
            {
                isGearHovered = newHoverState;
                Repaint();
            }

            if (isGearHovered)
            {
                EditorGUI.DrawRect(gearRect, new Color(0.3f, 0.3f, 0.3f, 0.5f));
            }

            GUI.color = isGearHovered ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            if (GUI.Button(gearRect, EditorGUIUtility.IconContent("_Popup"), GUIStyle.none))
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
            
            // 添加快捷键显示选项
            menu.AddItem(new GUIContent("显示快捷键 _%#F"), false, () =>
            {
                string shortcutInfo = "📌 框架工具窗口快捷键\n\n";
                shortcutInfo += "⌘/Ctrl + Shift + F\t: 打开框架工具窗口\n";
                shortcutInfo += "⌘/Ctrl + S\t\t: 保存当前配置\n";
                shortcutInfo += "⌘/Ctrl + R\t\t: 刷新当前视图\n";
                shortcutInfo += "⌘/Ctrl + 1-5\t\t: 快速切换标签页\n";
                shortcutInfo += "⌘/Ctrl + F\t\t: 搜索（在OtherTool中）\n";
                shortcutInfo += "ESC\t\t\t: 清除搜索\n\n";
                shortcutInfo += "当前快捷键：Ctrl+Shift+F";
                
                EditorUtility.DisplayDialog("快捷键说明", shortcutInfo, "知道了");
            });
            
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("重置布局"), false, () =>
            {
                EditorPrefs.DeleteKey(PREFS_KEY_SELECTED_TAB);
                EditorPrefs.DeleteKey(PREFS_KEY_OTHER_FILTER);
                EditorUtility.DisplayDialog("提示", "布局已重置，下次打开窗口生效", "确定");
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("关于"), false, () =>
            {
                EditorUtility.DisplayDialog("关于", "框架工具窗口 v2.0\n\n功能：\n- Unity编辑器工具\n- 项目数据统计\n- 代码生成\n- 自动注册\n- 其他工具集成", "确定");
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
    }
}