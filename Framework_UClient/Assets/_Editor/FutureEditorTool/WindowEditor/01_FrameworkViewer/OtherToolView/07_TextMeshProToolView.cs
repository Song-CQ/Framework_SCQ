/****************************************************
    文件: TextMeshProToolView.cs
    作者: Clear
    日期: 2026/3/6
    类型: 工具脚本
    功能: TextMeshPro 工具 - 管理字体字符集
*****************************************************/
using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.TextCore.LowLevel;
using System.Text;

namespace FutureEditor
{
   
        public static class TextMeshProToolView
        {
            private static Vector2 scrollPos;
            private static Vector2 txtFileScrollPos;
            private static Vector2 fontCharsScrollPos;
            private static Vector2 diffScrollPos;
            private static string inputText = "";
            private static TMP_FontAsset targetFontAsset;
            private static TextAsset targetTextFile;  // 目标TXT文件
            private static string txtFileContent = ""; // TXT文件内容（只读显示）
            private static string fontCharsContent = ""; // 字体文件中的字符
            private static bool showFontChars = false; // 是否显示字体字符
            private static string searchChar = ""; // 搜索的字符
            private static string searchResult = ""; // 搜索结果
            private static bool showDiff = false; // 是否显示差异
            private static string diffInfo = ""; // 差异信息

            // 缓存键名
            private const string CACHE_FONT_ASSET_PATH = "TextMeshProTool_FontAssetPath";
            private const string CACHE_TEXT_FILE_PATH = "TextMeshProTool_TextFilePath";

            public static void InitData()
            {
                inputText = "";
                txtFileContent = "";
                fontCharsContent = "";
                showFontChars = false;
                searchChar = "";
                searchResult = "";
                showDiff = false;
                diffInfo = "";

                // 加载缓存的字体文件
                LoadCachedFontAsset();

                // 加载缓存的字符库文件
                LoadCachedTextFile();
            }

            private static void LoadCachedFontAsset()
            {
                string cachedPath = EditorPrefs.GetString(CACHE_FONT_ASSET_PATH, "");
                if (!string.IsNullOrEmpty(cachedPath))
                {
                    targetFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(cachedPath);
                    if (targetFontAsset != null)
                    {
                        RefreshFontChars();
                        if (targetTextFile != null)
                        {
                            CompareDifferences();
                        }
                        Debug.Log($"已加载缓存的字体文件: {targetFontAsset.name}");
                    }
                    else
                    {
                        // 缓存路径无效，清除
                        EditorPrefs.DeleteKey(CACHE_FONT_ASSET_PATH);
                    }
                }
            }

            private static void LoadCachedTextFile()
            {
                string cachedPath = EditorPrefs.GetString(CACHE_TEXT_FILE_PATH, "");
                if (!string.IsNullOrEmpty(cachedPath))
                {
                    targetTextFile = AssetDatabase.LoadAssetAtPath<TextAsset>(cachedPath);
                    if (targetTextFile != null)
                    {
                        RefreshTxtFileContent();
                        if (targetFontAsset != null)
                        {
                            CompareDifferences();
                        }
                        Debug.Log($"已加载缓存的字符库文件: {targetTextFile.name}");
                    }
                    else
                    {
                        // 缓存路径无效，清除
                        EditorPrefs.DeleteKey(CACHE_TEXT_FILE_PATH);
                    }
                }
            }

            private static void SaveCache()
            {
                if (targetFontAsset != null)
                {
                    string path = AssetDatabase.GetAssetPath(targetFontAsset);
                    EditorPrefs.SetString(CACHE_FONT_ASSET_PATH, path);
                }

                if (targetTextFile != null)
                {
                    string path = AssetDatabase.GetAssetPath(targetTextFile);
                    EditorPrefs.SetString(CACHE_TEXT_FILE_PATH, path);
                }
            }

            public static void OnGUI(System.Action closeAction)
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos);

                // 标题
                EditorGUILayout.LabelField("字体字符管理工具", EditorStyles.boldLabel);
                GUILayout.Space(10);

                // 字体文件选择区域
                DrawFontSelection();

                GUILayout.Space(10);

                // 字体文件字符显示区域（放在字体文件选择区域下面）
                DrawFontCharsDisplay();

                GUILayout.Space(10);

                // TXT文件选择区域
                DrawTxtFileSelection();

                GUILayout.Space(10);

                // TXT文件内容显示（只读）
                DrawTxtFileContent();

                GUILayout.Space(10);

                // 差异比对区域
                DrawDiffDisplay();

                GUILayout.Space(10);

                // 字符输入区域
                DrawCharInput();

                GUILayout.Space(15);

                // 操作按钮
                DrawActionButtons();

                GUILayout.FlexibleSpace();

                // 底部关闭按钮
                DrawFooter(closeAction);

                GUILayout.EndScrollView();
            }

            private static void DrawFontSelection()
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("目标字体文件", EditorStyles.boldLabel);

                // 字体文件选择
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("字体文件:", GUILayout.Width(60));
                TMP_FontAsset newFont = (TMP_FontAsset)EditorGUILayout.ObjectField(targetFontAsset, typeof(TMP_FontAsset), false);

                if (newFont != targetFontAsset)
                {
                    targetFontAsset = newFont;
                    if (targetFontAsset != null)
                    {
                        RefreshFontChars();
                        if (targetTextFile != null)
                        {
                            CompareDifferences();
                        }
                        SaveCache();
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (targetFontAsset == null)
                {
                    EditorGUILayout.HelpBox("请选择一个 TMP_FontAsset 字体文件", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox($"已选择字体: {targetFontAsset.name}", MessageType.Info);
                    EditorGUILayout.LabelField($"字体模式: {targetFontAsset.atlasPopulationMode}", EditorStyles.miniLabel);

                    // 检查源字体文件
                    if (targetFontAsset.sourceFontFile == null)
                    {
                        EditorGUILayout.HelpBox("错误：字体没有关联源字体文件！请在 Inspector 中设置 Source Font File。", MessageType.Error);
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"源字体: {targetFontAsset.sourceFontFile.name}", EditorStyles.miniLabel);
                    }

                    // 显示字体字符统计
                    int charCount = targetFontAsset.characterTable?.Count ?? 0;
                    EditorGUILayout.LabelField($"已包含字符数: {charCount}", EditorStyles.miniLabel);

                    // 多纹理支持状态显示
                    EditorGUILayout.LabelField($"多纹理支持: {(targetFontAsset.isMultiAtlasTexturesEnabled ? "✓ 已启用" : "✗ 未启用")}", EditorStyles.miniLabel);

                    // 如果未启用多纹理，显示提示
                    if (!targetFontAsset.isMultiAtlasTexturesEnabled)
                    {
                        EditorGUILayout.HelpBox("建议：启用多纹理支持可以避免纹理空间不足的问题。", MessageType.Info);
                    }

                    // 如果字体不是动态模式，显示警告
                    if (targetFontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic)
                    {
                        EditorGUILayout.HelpBox("警告：字体不是动态模式，无法添加新字符！请将字体设置为 Dynamic 模式。", MessageType.Error);
                    }
                }

                EditorGUILayout.EndVertical();
            }

            private static void DrawFontCharsDisplay()
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("字体文件字符", EditorStyles.boldLabel);

                if (targetFontAsset != null)
                {
                    if (GUILayout.Button(showFontChars ? "隐藏字符" : "显示字符", GUILayout.Width(80)))
                    {
                        showFontChars = !showFontChars;
                        if (showFontChars && string.IsNullOrEmpty(fontCharsContent))
                        {
                            RefreshFontChars();
                        }
                        if (!showFontChars)
                        {
                            searchChar = "";
                            searchResult = "";
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (targetFontAsset == null)
                {
                    EditorGUILayout.HelpBox("请先选择字体文件", MessageType.Info);
                }
                else if (showFontChars)
                {
                    if (string.IsNullOrEmpty(fontCharsContent))
                    {
                        EditorGUILayout.HelpBox("字体文件中没有字符", MessageType.Info);
                    }
                    else
                    {
                        // 显示统计信息
                        char[] allChars = fontCharsContent.ToCharArray();
                        int uniqueCount = allChars.Distinct().Count();
                        EditorGUILayout.LabelField($"总字符数: {fontCharsContent.Length}  |  唯一字符数: {uniqueCount}", EditorStyles.miniLabel);
                        GUILayout.Space(5);

                        // 搜索框
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("搜索字符:", GUILayout.Width(60));
                        string newSearchChar = EditorGUILayout.TextField(searchChar, GUILayout.Width(100));
                        if (newSearchChar != searchChar)
                        {
                            searchChar = newSearchChar;
                            if (!string.IsNullOrEmpty(searchChar))
                            {
                                SearchCharacter();
                            }
                            else
                            {
                                searchResult = "";
                            }
                        }

                        if (GUILayout.Button("清除", GUILayout.Width(50)))
                        {
                            searchChar = "";
                            searchResult = "";
                            GUI.FocusControl(null);
                        }
                        EditorGUILayout.EndHorizontal();

                        // 显示搜索结果
                        if (!string.IsNullOrEmpty(searchResult))
                        {
                            EditorGUILayout.HelpBox(searchResult, searchResult.Contains("存在") ? MessageType.Info : MessageType.Warning);
                            GUILayout.Space(5);
                        }

                        // 字符显示区域
                        fontCharsScrollPos = EditorGUILayout.BeginScrollView(fontCharsScrollPos, GUILayout.Height(120));

                        // 格式化显示字符
                        string displayChars = fontCharsContent;
                        if (!string.IsNullOrEmpty(searchChar) && searchChar.Length == 1)
                        {
                            displayChars = HighlightCharacter(fontCharsContent, searchChar[0]);
                        }

                        EditorGUILayout.TextArea(displayChars, GUILayout.ExpandHeight(true));

                        EditorGUILayout.EndScrollView();
                    }
                }

                EditorGUILayout.EndVertical();
            }

            private static void DrawTxtFileSelection()
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("字符库文件", EditorStyles.boldLabel);

                // Unity 标准对象选择框
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("文本文件:", GUILayout.Width(60));
                TextAsset newTextFile = (TextAsset)EditorGUILayout.ObjectField(targetTextFile, typeof(TextAsset), false);

                if (newTextFile != targetTextFile)
                {
                    targetTextFile = newTextFile;
                    if (targetTextFile != null)
                    {
                        RefreshTxtFileContent();
                        if (targetFontAsset != null)
                        {
                            CompareDifferences();
                        }
                        SaveCache();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // 显示当前选择的文件信息
                if (targetTextFile != null)
                {
                    EditorGUILayout.LabelField($"文件路径: {AssetDatabase.GetAssetPath(targetTextFile)}", EditorStyles.miniLabel);
                }

                GUILayout.Space(5);

                // 拖拽区域提示
                EditorGUILayout.LabelField("或拖拽TXT文件到下方区域", EditorStyles.centeredGreyMiniLabel);
                Rect dropArea = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
                GUI.Box(dropArea, "拖拽TXT文件到此处", EditorStyles.helpBox);
                HandleDragAndDrop(dropArea);

                // 辅助按钮
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (targetTextFile != null && GUILayout.Button("刷新内容", GUILayout.Width(80)))
                {
                    RefreshTxtFileContent();
                    if (targetFontAsset != null)
                    {
                        CompareDifferences();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            private static void DrawTxtFileContent()
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("字符库内容（只读）", EditorStyles.boldLabel);

                if (string.IsNullOrEmpty(txtFileContent))
                {
                    EditorGUILayout.HelpBox("请先选择一个TXT文件", MessageType.Info);
                }
                else
                {
                    // 显示统计信息
                    char[] allChars = txtFileContent.ToCharArray();
                    int uniqueCount = allChars.Distinct().Count();
                    EditorGUILayout.LabelField($"总字符数: {txtFileContent.Length}  |  唯一字符数: {uniqueCount}", EditorStyles.miniLabel);
                    GUILayout.Space(5);

                    // 只读文本框显示内容
                    txtFileScrollPos = EditorGUILayout.BeginScrollView(txtFileScrollPos, GUILayout.Height(120));
                    EditorGUILayout.TextArea(txtFileContent, GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.EndVertical();
            }

            private static void DrawDiffDisplay()
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("差异比对", EditorStyles.boldLabel);

                if (targetFontAsset != null && targetTextFile != null)
                {
                    if (GUILayout.Button(showDiff ? "隐藏差异" : "显示差异", GUILayout.Width(80)))
                    {
                        showDiff = !showDiff;
                        if (showDiff && string.IsNullOrEmpty(diffInfo))
                        {
                            CompareDifferences();
                        }
                    }

                    if (GUILayout.Button("刷新比对", GUILayout.Width(80)))
                    {
                        CompareDifferences();
                        showDiff = true;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (targetFontAsset == null || targetTextFile == null)
                {
                    EditorGUILayout.HelpBox("请同时选择字体文件和字符库文件以进行差异比对", MessageType.Info);
                }
                else if (showDiff)
                {
                    if (string.IsNullOrEmpty(diffInfo))
                    {
                        EditorGUILayout.HelpBox("暂无差异信息，点击\"刷新比对\"按钮", MessageType.Info);
                    }
                    else
                    {
                        diffScrollPos = EditorGUILayout.BeginScrollView(diffScrollPos, GUILayout.Height(150));
                        EditorGUILayout.TextArea(diffInfo, GUILayout.ExpandHeight(true));
                        EditorGUILayout.EndScrollView();

                        // 一键同步按钮
                        if (targetFontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic)
                        {
                            GUI.backgroundColor = Color.green;
                            if (GUILayout.Button("将字符库同步到字体文件", GUILayout.Height(30)))
                            {
                                SyncTxtFileToFont();
                            }
                            GUI.backgroundColor = Color.white;
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("字体不是动态模式，无法同步！请将字体文件设置为 Dynamic 模式。", MessageType.Error);
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }

            private static void DrawCharInput()
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("添加新字符", EditorStyles.boldLabel);

                // 输入框
                EditorGUILayout.LabelField("输入要添加的字符（支持中英文、符号等）:");
                inputText = EditorGUILayout.TextArea(inputText, GUILayout.Height(60));

                // 显示输入统计
                if (!string.IsNullOrEmpty(inputText))
                {
                    char[] inputChars = inputText.ToCharArray();
                    int uniqueCount = inputChars.Distinct().Count();
                    EditorGUILayout.LabelField($"输入字符数: {inputText.Length}  |  唯一字符数: {uniqueCount}", EditorStyles.miniLabel);
                }

                EditorGUILayout.EndVertical();
            }

            private static void DrawActionButtons()
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("操作", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                GUI.enabled = targetFontAsset != null && targetFontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic;
                GUI.backgroundColor = Color.cyan;
                if (GUILayout.Button("添加到字体文件", GUILayout.Height(35)))
                {
                    AddToFontFile();
                }

                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("添加到字符库文件", GUILayout.Height(35)))
                {
                    AddToTxtFile();
                }

                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("同时添加到两者", GUILayout.Height(35)))
                {
                    AddToBoth();
                }

                GUI.backgroundColor = Color.white;
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                if (targetFontAsset != null && targetFontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic)
                {
                    EditorGUILayout.HelpBox("字体不是动态模式，无法添加字符！请在字体文件 Inspector 中将 Atlas Population Mode 设置为 Dynamic。", MessageType.Error);
                }

                EditorGUILayout.EndVertical();
            }

            private static void DrawFooter(System.Action closeAction)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("关闭", GUILayout.Width(80), GUILayout.Height(30)))
                {
                    closeAction?.Invoke();
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndHorizontal();
            }

            private static void CompareDifferences()
            {
                if (targetFontAsset == null || targetTextFile == null)
                    return;

                // 获取字体文件中的字符集合
                HashSet<char> fontChars = new HashSet<char>();
                if (targetFontAsset.characterTable != null)
                {
                    foreach (var character in targetFontAsset.characterTable)
                    {
                        fontChars.Add((char)character.unicode);
                    }
                }

                // 获取字符库文件中的字符集合
                HashSet<char> txtChars = new HashSet<char>();
                if (!string.IsNullOrEmpty(txtFileContent))
                {
                    foreach (char c in txtFileContent)
                    {
                        txtChars.Add(c);
                    }
                }

                // 计算差异
                var onlyInFont = fontChars.Except(txtChars).ToList();
                var onlyInTxt = txtChars.Except(fontChars).ToList();
                var inBoth = fontChars.Intersect(txtChars).ToList();

                // 构建差异信息
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("========== 字符差异比对 ==========\n");
                sb.AppendLine($"字体文件字符数: {fontChars.Count}");
                sb.AppendLine($"字符库文件字符数: {txtChars.Count}");
                sb.AppendLine($"共同字符数: {inBoth.Count}\n");

                sb.AppendLine($"【仅在字体文件中存在】({onlyInFont.Count} 个):");
                if (onlyInFont.Count > 0)
                {
                    string chars = new string(onlyInFont.ToArray());
                    sb.AppendLine(FormatCharsForDisplay(chars, 30));
                }
                else
                {
                    sb.AppendLine("  无");
                }
                sb.AppendLine();

                sb.AppendLine($"【仅在字符库文件中存在】({onlyInTxt.Count} 个):");
                if (onlyInTxt.Count > 0)
                {
                    string chars = new string(onlyInTxt.ToArray());
                    sb.AppendLine(FormatCharsForDisplay(chars, 30));
                }
                else
                {
                    sb.AppendLine("  无");
                }
                sb.AppendLine();

                sb.AppendLine("==================================");

                diffInfo = sb.ToString();
            }

            private static void SyncTxtFileToFont()
            {
                if (targetFontAsset == null)
                {
                    EditorUtility.DisplayDialog("提示", "请先选择目标字体文件", "确定");
                    return;
                }

                if (targetFontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic)
                {
                    EditorUtility.DisplayDialog("提示", "字体不是动态模式，无法添加字符！\n请在字体文件 Inspector 中将 Atlas Population Mode 设置为 Dynamic。", "确定");
                    return;
                }

                // 多纹理支持检查
                if (!targetFontAsset.isMultiAtlasTexturesEnabled)
                {
                    bool enableMultiAtlas = EditorUtility.DisplayDialog(
                        "提示",
                        "字体未启用多纹理支持，这可能导致同步失败。\n是否启用多纹理支持？",
                        "启用",
                        "取消");

                    if (enableMultiAtlas)
                    {
                        targetFontAsset.isMultiAtlasTexturesEnabled = true;
                        EditorUtility.SetDirty(targetFontAsset);
                        AssetDatabase.SaveAssets();
                        Debug.Log("已启用多纹理支持");
                        EditorUtility.DisplayDialog("提示", "已启用多纹理支持，建议重新启动Unity编辑器以确保生效。", "确定");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "未启用多纹理支持，同步可能失败。", "确定");
                    }
                }

                if (targetTextFile == null || string.IsNullOrEmpty(txtFileContent))
                {
                    EditorUtility.DisplayDialog("提示", "请先选择字符库文件", "确定");
                    return;
                }

                try
                {
                    // 获取字符库中的所有唯一字符
                    char[] allChars = txtFileContent.ToCharArray().Distinct().ToArray();
                    string allUniqueChars = new string(allChars);

                    // 记录 Undo 操作
                    Undo.RecordObject(targetFontAsset, "Sync Characters from TXT");

                    // 使用 TryAddCharacters 方法批量添加字符
                    string missingCharacters;
                    bool success = targetFontAsset.TryAddCharacters(allUniqueChars, out missingCharacters, includeFontFeatures: true);

                    int addedCount = allChars.Length - (missingCharacters?.Length ?? 0);

                    // 标记为脏，保存资源
                    EditorUtility.SetDirty(targetFontAsset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    // 刷新显示
                    RefreshFontChars();
                    CompareDifferences();

                    Debug.Log($"同步完成，成功添加 {addedCount} 个字符到字体文件");

                    string message = $"同步完成!\n\n" +
                        $"字符库总字符数: {allChars.Length}\n" +
                        $"成功添加到字体: {addedCount} 个字符\n" +
                        $"失败字符数: {missingCharacters?.Length ?? 0}\n\n";

                    if (!string.IsNullOrEmpty(missingCharacters))
                    {
                        message += $"无法添加的字符: {missingCharacters}\n";
                        message += "可能原因:\n";
                        message += "1. 字符在源字体文件中不存在\n";
                        message += "2. 字体纹理空间不足\n\n";
                    }

                    message += "提示: 如果字符显示异常，请确保:\n";
                    message += "1. 字体文件已设置为动态模式\n";
                    message += "2. 源字体文件包含这些字符\n";
                    message += "3. 已启用多纹理支持";

                    EditorUtility.DisplayDialog(success ? "同步成功" : "部分成功", message, "确定");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"同步失败: {ex.Message}");
                    EditorUtility.DisplayDialog("同步失败", $"同步时出错:\n{ex.Message}", "确定");
                }
            }

            private static void HandleDragAndDrop(Rect dropArea)
            {
                Event evt = Event.current;

                switch (evt.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!dropArea.Contains(evt.mousePosition))
                            return;

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            foreach (var obj in DragAndDrop.objectReferences)
                            {
                                if (obj is TextAsset textAsset)
                                {
                                    targetTextFile = textAsset;
                                    RefreshTxtFileContent();
                                    if (targetFontAsset != null)
                                    {
                                        CompareDifferences();
                                    }
                                    SaveCache();
                                    break;
                                }
                                else if (obj is UnityEngine.Object unityObj)
                                {
                                    string path = AssetDatabase.GetAssetPath(unityObj);
                                    if (path.EndsWith(".txt"))
                                    {
                                        targetTextFile = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                                        RefreshTxtFileContent();
                                        if (targetFontAsset != null)
                                        {
                                            CompareDifferences();
                                        }
                                        SaveCache();
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            private static void SelectTxtFile()
            {
                string path = EditorUtility.OpenFilePanel("选择字符库文件", "Assets", "txt");
                if (!string.IsNullOrEmpty(path))
                {
                    // 转换为相对路径
                    if (path.StartsWith(Application.dataPath))
                    {
                        string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                        targetTextFile = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);
                        RefreshTxtFileContent();
                        if (targetFontAsset != null)
                        {
                            CompareDifferences();
                        }
                        SaveCache();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("错误", "请选择Assets目录下的TXT文件", "确定");
                    }
                }
            }

            private static void RefreshTxtFileContent()
            {
                if (targetTextFile != null)
                {
                    txtFileContent = targetTextFile.text;
                    // 去重处理
                    txtFileContent = RemoveDuplicateChars(txtFileContent);
                }
            }

            private static void RefreshFontChars()
            {
                if (targetFontAsset != null && targetFontAsset.characterTable != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var character in targetFontAsset.characterTable)
                    {
                        sb.Append((char)character.unicode);
                    }
                    fontCharsContent = sb.ToString();
                }
                else
                {
                    fontCharsContent = "";
                }
            }

            private static void SearchCharacter()
            {
                if (string.IsNullOrEmpty(searchChar) || string.IsNullOrEmpty(fontCharsContent))
                {
                    searchResult = "";
                    return;
                }

                // 支持搜索单个字符或字符串
                if (searchChar.Length == 1)
                {
                    char target = searchChar[0];
                    if (fontCharsContent.Contains(target))
                    {
                        // 统计出现次数
                        int count = fontCharsContent.Count(c => c == target);
                        searchResult = $"✓ 字符 '{target}' 存在于字体文件中，出现 {count} 次";
                    }
                    else
                    {
                        searchResult = $"✗ 字符 '{target}' 不存在于字体文件中";

                        // 检查是否在字符库中
                        if (targetTextFile != null && !string.IsNullOrEmpty(txtFileContent))
                        {
                            if (txtFileContent.Contains(target))
                            {
                                searchResult += $"\n  但在字符库文件中存在，可以使用\"将字符库同步到字体文件\"按钮添加";
                            }
                        }
                    }
                }
                else
                {
                    // 搜索字符串
                    if (fontCharsContent.Contains(searchChar))
                    {
                        searchResult = $"✓ 字符串 \"{searchChar}\" 存在于字体文件中";
                    }
                    else
                    {
                        searchResult = $"✗ 字符串 \"{searchChar}\" 不存在于字体文件中";

                        if (targetTextFile != null && !string.IsNullOrEmpty(txtFileContent))
                        {
                            if (txtFileContent.Contains(searchChar))
                            {
                                searchResult += $"\n  但在字符库文件中存在，可以使用\"将字符库同步到字体文件\"按钮添加";
                            }
                        }
                    }
                }
            }

            private static string HighlightCharacter(string text, char target)
            {
                if (string.IsNullOrEmpty(text)) return text;

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == target)
                    {
                        sb.Append("[");
                        sb.Append(text[i]);
                        sb.Append("]");
                    }
                    else
                    {
                        sb.Append(text[i]);
                    }
                }
                return sb.ToString();
            }

            private static string FormatCharsForDisplay(string chars, int charsPerLine = 20)
            {
                if (string.IsNullOrEmpty(chars)) return "";

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < chars.Length; i++)
                {
                    sb.Append(chars[i]);
                    sb.Append("  ");

                    if ((i + 1) % charsPerLine == 0)
                    {
                        sb.AppendLine();
                    }
                }

                return sb.ToString();
            }

            private static string RemoveDuplicateChars(string text)
            {
                if (string.IsNullOrEmpty(text)) return "";

                // 去重并保持原有顺序
                var uniqueChars = new HashSet<char>();
                var result = new StringBuilder();

                foreach (char c in text)
                {
                    if (!uniqueChars.Contains(c))
                    {
                        uniqueChars.Add(c);
                        result.Append(c);
                    }
                }

                return result.ToString();
            }

            private static void AddToFontFile()
            {
                if (targetFontAsset == null)
                {
                    EditorUtility.DisplayDialog("提示", "请先选择目标字体文件", "确定");
                    return;
                }

                if (targetFontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic)
                {
                    EditorUtility.DisplayDialog("提示", "字体不是动态模式，无法添加字符！\n请在字体文件 Inspector 中将 Atlas Population Mode 设置为 Dynamic。", "确定");
                    return;
                }

                if (string.IsNullOrEmpty(inputText))
                {
                    EditorUtility.DisplayDialog("提示", "请输入要添加的字符", "确定");
                    return;
                }

                try
                {
                    // 先检查字体源文件是否存在
                    if (targetFontAsset.sourceFontFile == null)
                    {
                        EditorUtility.DisplayDialog("错误", "字体文件没有关联源字体文件！\n请在字体文件 Inspector 中设置 Source Font File。", "确定");
                        return;
                    }

                    // 多纹理支持检查
                    if (!targetFontAsset.isMultiAtlasTexturesEnabled)
                    {
                        bool enableMultiAtlas = EditorUtility.DisplayDialog(
                            "提示",
                            "字体未启用多纹理支持，这可能导致添加字符失败。\n是否启用多纹理支持？",
                            "启用",
                            "取消");

                        if (enableMultiAtlas)
                        {
                            targetFontAsset.isMultiAtlasTexturesEnabled = true;
                            EditorUtility.SetDirty(targetFontAsset);
                            AssetDatabase.SaveAssets();
                            Debug.Log("已启用多纹理支持");
                            EditorUtility.DisplayDialog("提示", "已启用多纹理支持，建议重新启动Unity编辑器以确保生效。", "确定");
                        }
                    }

                    // 获取输入的唯一字符
                    char[] inputChars = inputText.ToCharArray().Distinct().ToArray();
                    string uniqueInput = new string(inputChars);

                    // 诊断：检查哪些字符已经在字体中
                    HashSet<uint> existingChars = new HashSet<uint>();
                    if (targetFontAsset.characterTable != null)
                    {
                        foreach (var character in targetFontAsset.characterTable)
                        {
                            existingChars.Add(character.unicode);
                        }
                    }

                    List<char> needToAdd = new List<char>();
                    List<char> alreadyExist = new List<char>();

                    foreach (char c in uniqueInput)
                    {
                        uint unicode = (uint)c;
                        if (existingChars.Contains(unicode))
                        {
                            alreadyExist.Add(c);
                        }
                        else
                        {
                            needToAdd.Add(c);
                        }
                    }

                    if (needToAdd.Count == 0)
                    {
                        EditorUtility.DisplayDialog("提示",
                            $"所有字符都已存在于字体中！\n已有字符: {new string(alreadyExist.ToArray())}",
                            "确定");
                        return;
                    }

                    // 显示诊断信息
                    string diagMsg = $"需要添加的字符: {new string(needToAdd.ToArray())}\n" +
                                     $"字符数: {needToAdd.Count}\n\n" +
                                     $"已存在字符: {new string(alreadyExist.ToArray())}\n" +
                                     $"纹理模式: {(targetFontAsset.isMultiAtlasTexturesEnabled ? "多纹理支持" : "单纹理")}\n\n" +
                                     $"开始添加...";

                    Debug.Log(diagMsg);

                    // 记录 Undo 操作
                    Undo.RecordObject(targetFontAsset, "Add Characters to Font");

                    // 使用 TryAddCharacters 方法批量添加有效字符
                    string missingCharacters;
                    bool success = targetFontAsset.TryAddCharacters(new string(needToAdd.ToArray()), out missingCharacters, includeFontFeatures: true);

                    int addedCount = needToAdd.Count - (missingCharacters?.Length ?? 0);

                    // 标记为脏，保存资源
                    EditorUtility.SetDirty(targetFontAsset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    // 刷新显示
                    RefreshFontChars();
                    if (targetTextFile != null)
                    {
                        CompareDifferences();
                    }

                    Debug.Log($"字体 {targetFontAsset.name} 已更新，成功添加 {addedCount} 个字符");

                    // 构建结果消息
                    string message = success ? "✓ 添加成功" : "⚠ 部分成功";
                    message += $"\n\n尝试添加: {needToAdd.Count} 个字符";
                    message += $"\n成功添加: {addedCount} 个字符";
                    message += $"\n失败数量: {missingCharacters?.Length ?? 0} 个字符";

                    if (!string.IsNullOrEmpty(missingCharacters))
                    {
                        message += $"\n\n❌ 无法添加的字符: {missingCharacters}";
                        message += "\n\n可能原因:\n";
                        message += "1. 字符在源字体文件中不存在\n";
                        message += "2. 字体纹理空间不足\n";
                        message += "3. 字符是特殊控制字符\n\n";

                        message += "建议:\n";
                        message += "1. 检查源字体文件是否包含这些字符\n";
                        message += "2. 尝试增大字体纹理尺寸（Atlas Width/Height）\n";
                        message += "3. 确保已启用多纹理支持\n";
                    }

                    if (alreadyExist.Count > 0)
                    {
                        message += $"\n✓ 已存在字符: {new string(alreadyExist.ToArray())}";
                    }

                    message += "\n\n提示: 如果字符显示异常，请确保:\n";
                    message += "1. 字体文件已设置为动态模式\n";
                    message += "2. 源字体文件包含这些字符\n";
                    message += "3. 已启用多纹理支持\n";
                    message += "4. 重新启动 Unity 编辑器";

                    EditorUtility.DisplayDialog(success ? "添加成功" : "部分成功", message, "确定");

                    // 清空输入框
                    if (success || addedCount > 0)
                    {
                        inputText = "";
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"添加到字体失败: {ex.Message}\n{ex.StackTrace}");
                    EditorUtility.DisplayDialog("添加失败", $"添加到字体时出错:\n{ex.Message}\n\n请检查:\n1. 字体文件是否设置为动态模式\n2. 源字体文件是否有效\n3. 字符是否在源字体文件中存在\n4. 是否启用了多纹理支持", "确定");
                }
            }

            private static void AddToTxtFile()
            {
                if (targetTextFile == null)
                {
                    EditorUtility.DisplayDialog("提示", "请先选择字符库文件", "确定");
                    return;
                }

                if (string.IsNullOrEmpty(inputText))
                {
                    EditorUtility.DisplayDialog("提示", "请输入要添加的字符", "确定");
                    return;
                }

                try
                {
                    // 获取当前文件路径
                    string assetPath = AssetDatabase.GetAssetPath(targetTextFile);
                    string fullPath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));

                    // 读取当前内容
                    string currentContent = File.ReadAllText(fullPath, Encoding.UTF8);

                    // 合并并去重
                    string newContent = currentContent + inputText;
                    newContent = RemoveDuplicateChars(newContent);

                    // 排序（可选，按Unicode排序）
                    char[] sortedChars = newContent.ToCharArray();
                    System.Array.Sort(sortedChars);
                    newContent = new string(sortedChars);

                    // 写入文件
                    File.WriteAllText(fullPath, newContent, Encoding.UTF8);
                    AssetDatabase.Refresh();

                    // 刷新显示
                    targetTextFile = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                    RefreshTxtFileContent();
                    if (targetFontAsset != null)
                    {
                        CompareDifferences();
                    }

                    // 清空输入框
                    inputText = "";

                    EditorUtility.DisplayDialog("添加成功", $"字符已添加到字符库文件\n当前唯一字符数: {newContent.Length}", "确定");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"添加到TXT文件失败: {ex.Message}");
                    EditorUtility.DisplayDialog("添加失败", $"添加到文件时出错:\n{ex.Message}", "确定");
                }
            }

            private static void AddToBoth()
            {
                if (targetFontAsset == null)
                {
                    EditorUtility.DisplayDialog("提示", "请先选择目标字体文件", "确定");
                    return;
                }

                if (targetFontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic)
                {
                    EditorUtility.DisplayDialog("提示", "字体不是动态模式，无法添加字符！\n请在字体文件 Inspector 中将 Atlas Population Mode 设置为 Dynamic。", "确定");
                    return;
                }

                if (targetTextFile == null)
                {
                    EditorUtility.DisplayDialog("提示", "请先选择字符库文件", "确定");
                    return;
                }

                if (string.IsNullOrEmpty(inputText))
                {
                    EditorUtility.DisplayDialog("提示", "请输入要添加的字符", "确定");
                    return;
                }

                try
                {
                    // 多纹理支持检查
                    if (!targetFontAsset.isMultiAtlasTexturesEnabled)
                    {
                        bool enableMultiAtlas = EditorUtility.DisplayDialog(
                            "提示",
                            "字体未启用多纹理支持，这可能导致添加字符失败。\n是否启用多纹理支持？",
                            "启用",
                            "取消");

                        if (enableMultiAtlas)
                        {
                            targetFontAsset.isMultiAtlasTexturesEnabled = true;
                            EditorUtility.SetDirty(targetFontAsset);
                            AssetDatabase.SaveAssets();
                            Debug.Log("已启用多纹理支持");
                        }
                    }

                    // 1. 添加到字体文件
                    Undo.RecordObject(targetFontAsset, "Add Characters to Font");

                    char[] inputChars = inputText.ToCharArray().Distinct().ToArray();
                    string uniqueInput = new string(inputChars);

                    string missingCharacters;
                    bool success = targetFontAsset.TryAddCharacters(uniqueInput, out missingCharacters, includeFontFeatures: true);
                    int addedToFont = inputChars.Length - (missingCharacters?.Length ?? 0);

                    EditorUtility.SetDirty(targetFontAsset);

                    // 2. 添加到TXT文件
                    string assetPath = AssetDatabase.GetAssetPath(targetTextFile);
                    string fullPath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));
                    string currentContent = File.ReadAllText(fullPath, Encoding.UTF8);

                    string newContent = currentContent + inputText;
                    newContent = RemoveDuplicateChars(newContent);

                    char[] sortedChars = newContent.ToCharArray();
                    System.Array.Sort(sortedChars);
                    newContent = new string(sortedChars);

                    File.WriteAllText(fullPath, newContent, Encoding.UTF8);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    // 刷新显示
                    RefreshFontChars();
                    targetTextFile = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                    RefreshTxtFileContent();
                    CompareDifferences();

                    // 清空输入框
                    inputText = "";

                    string message = $"同时添加完成!\n\n" +
                        $"添加到字体: {addedToFont} 个字符\n" +
                        $"添加到字符库: 成功\n" +
                        $"当前字符库唯一字符数: {newContent.Length}\n\n";

                    if (!string.IsNullOrEmpty(missingCharacters))
                    {
                        message += $"字体无法添加的字符: {missingCharacters}\n";
                    }

                    EditorUtility.DisplayDialog("添加成功", message, "确定");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"添加失败: {ex.Message}");
                    EditorUtility.DisplayDialog("添加失败", $"添加时出错:\n{ex.Message}", "确定");
                }
            }
        }
    

    public class TMPFontTool : EditorWindow
    {
        private TMP_FontAsset fontAsset;
        private string textToAdd = "";

        [MenuItem("[FC Tool]/TMP字体工具")]
        public static void ShowWindow()
        {
            GetWindow<TMPFontTool>("TMP字体工具");
        }

        void OnGUI()
        {
            fontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("目标字体", fontAsset, typeof(TMP_FontAsset), false);

            if (fontAsset != null)
            {
                // 显示字体模式
                EditorGUILayout.LabelField("字体模式", fontAsset.atlasPopulationMode.ToString());

                // 显示已有字符数
                int charCount = fontAsset.characterTable?.Count ?? 0;
                EditorGUILayout.LabelField("已有字符数", charCount.ToString());
            }

            textToAdd = EditorGUILayout.TextArea(textToAdd, GUILayout.Height(80));

            if (GUILayout.Button("添加字符", GUILayout.Height(40)))
            {
                if (fontAsset == null)
                {
                    Debug.LogError("请先选择字体文件");
                    EditorUtility.DisplayDialog("提示", "请先选择字体文件", "确定");
                    return;
                }

                // 检查字体是否为动态模式
                if (fontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic)
                {
                    EditorUtility.DisplayDialog("错误",
                        "字体不是动态模式，无法添加字符！\n请在字体文件 Inspector 中将 Atlas Population Mode 设置为 Dynamic。",
                        "确定");
                    return;
                }

                // 检查源字体文件
                if (fontAsset.sourceFontFile == null)
                {
                    EditorUtility.DisplayDialog("错误",
                        "字体文件没有关联源字体文件！\n请在字体文件 Inspector 中设置 Source Font File。",
                        "确定");
                    return;
                }

                if (string.IsNullOrEmpty(textToAdd))
                {
                    EditorUtility.DisplayDialog("提示", "请输入要添加的字符", "确定");
                    return;
                }

                // 确保启用多纹理支持
                if (!fontAsset.isMultiAtlasTexturesEnabled)
                {
                    bool enableMultiAtlas = EditorUtility.DisplayDialog(
                        "提示",
                        "字体未启用多纹理支持，这可能导致添加字符失败。\n是否启用多纹理支持？",
                        "启用",
                        "取消");

                    if (enableMultiAtlas)
                    {
                        fontAsset.isMultiAtlasTexturesEnabled = true;
                        EditorUtility.SetDirty(fontAsset);
                        Debug.Log("已启用多纹理支持");
                    }
                }

                // 去重
                string uniqueChars = new string(textToAdd.Distinct().ToArray());

                // 诊断：检查哪些字符已经在字体中
                HashSet<uint> existingChars = new HashSet<uint>();
                if (fontAsset.characterTable != null)
                {
                    foreach (var character in fontAsset.characterTable)
                    {
                        existingChars.Add(character.unicode);
                    }
                }

                List<char> needToAdd = new List<char>();
                List<char> alreadyExist = new List<char>();

                foreach (char c in uniqueChars)
                {
                    uint unicode = (uint)c;
                    if (existingChars.Contains(unicode))
                    {
                        alreadyExist.Add(c);
                    }
                    else
                    {
                        needToAdd.Add(c);
                    }
                }

                if (needToAdd.Count == 0)
                {
                    EditorUtility.DisplayDialog("提示",
                        $"所有字符都已存在于字体中！\n已有字符: {new string(alreadyExist.ToArray())}",
                        "确定");
                    return;
                }

                // 显示诊断信息
                string diagMsg = $"需要添加的字符: {new string(needToAdd.ToArray())}\n" +
                                 $"字符数: {needToAdd.Count}\n\n" +
                                 $"已存在字符: {new string(alreadyExist.ToArray())}\n" +
                                 $"纹理模式: {(fontAsset.isMultiAtlasTexturesEnabled ? "多纹理支持" : "单纹理")}\n\n" +
                                 $"开始添加...";

                Debug.Log(diagMsg);

                // 记录 Undo
                Undo.RecordObject(fontAsset, "Add Characters");

                // 添加字符（只添加需要添加的）
                string missingChars;
                bool success = fontAsset.TryAddCharacters(new string(needToAdd.ToArray()), out missingChars, true);

                // 保存
                EditorUtility.SetDirty(fontAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // 构建结果消息
                int addedCount = needToAdd.Count - (missingChars?.Length ?? 0);

                string message = success ? "✓ 添加成功" : "⚠ 部分成功";
                message += $"\n\n尝试添加: {needToAdd.Count} 个字符";
                message += $"\n成功添加: {addedCount} 个字符";
                message += $"\n失败数量: {missingChars?.Length ?? 0} 个字符";

                if (!string.IsNullOrEmpty(missingChars))
                {
                    message += $"\n\n❌ 无法添加的字符: {missingChars}";
                    message += "\n\n可能原因:\n";
                    message += "1. 字符在源字体文件中不存在\n";
                    message += "2. 字体纹理空间不足\n";
                    message += "3. 字符是特殊控制字符\n\n";

                    message += "建议:\n";
                    message += "1. 检查源字体文件是否包含这些字符\n";
                    message += "2. 尝试增大字体纹理尺寸（Atlas Width/Height）\n";
                    message += "3. 确保已启用多纹理支持\n";
                }

                if (alreadyExist.Count > 0)
                {
                    message += $"\n✓ 已存在字符: {new string(alreadyExist.ToArray())}";
                }

                EditorUtility.DisplayDialog("添加结果", message, "确定");

                // 清空输入框
                if (success || addedCount > 0)
                {
                    textToAdd = "";
                }
            }

        }
    }

}