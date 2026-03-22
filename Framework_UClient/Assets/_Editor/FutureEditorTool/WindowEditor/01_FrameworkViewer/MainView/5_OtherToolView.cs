/****************************************************
    文件: OtherToolView.cs
    作者: Clear
    日期: 2026/3/6
    类型: 工具脚本
    功能: 其他工具视图
*****************************************************/
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FutureEditor
{
    public static class OtherToolView
    {
        private static Vector2 otherToolPos;
        private static Vector2 otherToolPos2;
        private static string selectOtherToolKey;
        private static Color selectColor = new Color(0.95f, 0.73f, 0.01f); // #F2BB03 金黄色
        private static Color selectedBg = new Color(1.0f, 0.9f, 0.4f); // 亮黄色
        private static Dictionary<string, Action> OtherTooDic = new Dictionary<string, Action>();

        // 工具颜色 - 浅色系
        private static readonly Color ColorExcel;
        private static readonly Color ColorPath;
        private static readonly Color ColorAssetBundle;
        private static readonly Color ColorSVN;
        private static readonly Color ColorILRuntime;
        private static readonly Color ColorEncoding;
        private static readonly Color ColorTextMeshPro;

        // 静态构造函数 - 在类第一次使用时执行，生成随机但固定的颜色
        static OtherToolView()
        {
            // 使用工具名称的哈希值作为随机种子，确保每次运行颜色相同
            ColorExcel = GenerateLightColorFromString("Excel");
            ColorPath = GenerateLightColorFromString("Path");
            ColorAssetBundle = GenerateLightColorFromString("AssetBundle");
            ColorSVN = GenerateLightColorFromString("SVN");
            ColorILRuntime = GenerateLightColorFromString("ILRuntime");
            ColorEncoding = GenerateLightColorFromString("Encoding");
            ColorTextMeshPro = GenerateLightColorFromString("TextMeshPro");
        }

        // 生成浅色
        private static Color GenerateLightColorFromString(string input)
        {
            int hash = input.GetHashCode();
            System.Random rand = new System.Random(hash);

            // 浅色系：0.7-1.0 范围
            float r = (float)rand.NextDouble() * 0.3f + 0.7f;
            float g = (float)rand.NextDouble() * 0.3f + 0.7f;
            float b = (float)rand.NextDouble() * 0.3f + 0.7f;

            return new Color(r, g, b);
        }

        public static void InitData()
        {
            selectOtherToolKey = string.Empty;
            OtherTooDic.Clear();

            OtherTooDic.Add("[01] Excel Tool", null);
            OtherTooDic.Add("[02] Path Tool", null);
            OtherTooDic.Add("[03] AssetBundle Tool", null);
            OtherTooDic.Add("[04] SVN Tool", null);
            OtherTooDic.Add("[05] ILRuntime Tool", null);
            OtherTooDic.Add("[06] Encoding Tool", null);
            OtherTooDic.Add("[07] TextMeshPro Tool", null);

            for (int i = 0; i < 30; i++)
            {
                OtherTooDic.Add($"[Common_Info] {(i + 1):D2}", null);
            }

            ExcelToolView.InitData();
            PathToolView.InitData();
            AssetBundleToolView.InitData();
            SVNToolView.InitData();
            ILRuntimeToolView.InitData();
            EncodingToolView.InitData();
            TextMeshProToolView.InitData();
        }

        public static void OnGUI(float contentHeight, System.Action closeAction, string filter, System.Action<string> onFilterChanged)
        {
            float leftWidth = 185;
            float rightWidth = Screen.width - leftWidth - 15;

            // 左边按钮列表
            DrawLeftPanel(leftWidth, contentHeight, filter, onFilterChanged);

            // 右边界面
            DrawRightPanel(leftWidth, rightWidth, contentHeight, closeAction);
        }

        private static void DrawLeftPanel(float leftWidth, float contentHeight, string filter, System.Action<string> onFilterChanged)
        {
            GUILayout.BeginArea(new Rect(5, 25, leftWidth, contentHeight), new GUIStyle("grey_border"));
            GUILayout.BeginArea(new Rect(0, 2, leftWidth - 3, contentHeight - 5));

            // 添加顶部空隙
            GUILayout.Space(5);

            // 创建一个垂直居中的样式
            var iconStyle = new GUIStyle
            {
                fixedHeight = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageOnly
            };

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label(EditorGUIUtility.IconContent("Search Icon"), iconStyle, GUILayout.Width(20));

            string newFilter = GUILayout.TextField(filter, EditorStyles.toolbarTextField, GUILayout.Width(120));
            if (newFilter != filter)
            {
                onFilterChanged?.Invoke(newFilter);
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), iconStyle, GUILayout.Width(20)))
            {
                onFilterChanged?.Invoke("");
                GUI.FocusControl(null);
            }

            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            otherToolPos = GUILayout.BeginScrollView(otherToolPos, false, true, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);

            var filteredItems = OtherTooDic
                .Where(kv => string.IsNullOrEmpty(newFilter) ||
                            kv.Key.ToLower().Contains(newFilter.ToLower()))
                .OrderBy(kv => kv.Key, StringComparer.Ordinal);

            foreach (var item in filteredItems)
            {
                if (selectOtherToolKey == string.Empty)
                {
                    selectOtherToolKey = item.Key;
                }

                bool isSelected = (selectOtherToolKey == item.Key);

                // 设置背景色
                if (isSelected)
                {
                    GUI.backgroundColor = selectedBg; // 选中状态：亮黄色
                }
                else
                {
                    // 非选中状态：根据工具类型设置浅色
                    if (item.Key.Contains("Excel"))
                    {
                        GUI.backgroundColor = ColorExcel;
                    }
                    else if (item.Key.Contains("Path"))
                    {
                        GUI.backgroundColor = ColorPath;
                    }
                    else if (item.Key.Contains("AssetBundle"))
                    {
                        GUI.backgroundColor = ColorAssetBundle;
                    }
                    else if (item.Key.Contains("SVN"))
                    {
                        GUI.backgroundColor = ColorSVN;
                    }
                    else if (item.Key.Contains("ILRuntime"))
                    {
                        GUI.backgroundColor = ColorILRuntime;
                    }
                    else if (item.Key.Contains("Encoding"))
                    {
                        GUI.backgroundColor = ColorEncoding;
                    }
                    else if (item.Key.Contains("TextMeshPro"))
                    {
                        GUI.backgroundColor = ColorTextMeshPro;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.white;
                    }
                }

                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.alignment = TextAnchor.MiddleLeft;
                buttonStyle.padding = new RectOffset(10, 5, 2, 2);

                // 选中按钮：字体用金黄色并加粗
                if (isSelected)
                {
                    buttonStyle.fontStyle = FontStyle.Bold;
                    buttonStyle.normal.textColor = selectColor;
                    buttonStyle.hover.textColor = selectColor;
                }

                if (GUILayout.Button(item.Key, buttonStyle, GUILayout.Width(leftWidth - 25), GUILayout.Height(30)))
                {
                    selectOtherToolKey = item.Key;
                }

                GUI.backgroundColor = Color.white;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUILayout.EndArea();
        }

        private static void DrawRightPanel(float leftWidth, float rightWidth, float contentHeight, System.Action closeAction)
        {
            GUILayout.BeginArea(new Rect(leftWidth + 10, 25, rightWidth, contentHeight), new GUIStyle("FrameBox"));

            otherToolPos2 = GUILayout.BeginScrollView(otherToolPos2);

            string displayTitle = selectOtherToolKey;
            if (selectOtherToolKey.StartsWith("[") && selectOtherToolKey.Contains("]"))
            {
                displayTitle = selectOtherToolKey;
            }
            GUILayout.Label(displayTitle, new GUIStyle("CN StatusWarn"));

            if (!string.IsNullOrEmpty(selectOtherToolKey) && OtherTooDic.ContainsKey(selectOtherToolKey))
            {
                if (selectOtherToolKey.StartsWith("[Common_Info]"))
                {
                    CommonInfoView.OnGUI();
                }
                else
                {
                    switch (selectOtherToolKey)
                    {
                        case "[01] Excel Tool":
                            ExcelToolView.OnGUI(closeAction);
                            break;
                        case "[02] Path Tool":
                            PathToolView.OnGUI(closeAction);
                            break;
                        case "[03] AssetBundle Tool":
                            AssetBundleToolView.OnGUI(closeAction);
                            break;
                        case "[04] SVN Tool":
                            SVNToolView.OnGUI(closeAction);
                            break;
                        case "[05] ILRuntime Tool":
                            ILRuntimeToolView.OnGUI(closeAction);
                            break;
                        case "[06] Encoding Tool":
                            EncodingToolView.OnGUI(closeAction);
                            break;
                        case "[07] TextMeshPro Tool":
                            TextMeshProToolView.OnGUI(closeAction);
                            break;
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}