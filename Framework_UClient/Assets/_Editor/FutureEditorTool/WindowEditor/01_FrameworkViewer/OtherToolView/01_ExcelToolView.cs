using UnityEngine;
using UnityEditor;
using FutureCore;
using ProjectApp.Data;
using ProjectApp;
using System.Text;
using System.IO;

namespace FutureEditor
{
    public static class ExcelToolView
    {
        private static int buildOutType;
        private static bool isEnciphermentData = false;
        private static bool isOutMultipleDatas = false;
        private static bool showAESKey = false;
        private static bool showIVector = false;

        // 新增：可编辑的版本号
        private static int customVersion = -1; // -1表示使用默认版本
        private static bool showVersionEdit = false;

        // 配置文件路径
        private static readonly string VERSION_CONFIG_PATH = "Assets/Resources/Config/VersionConfig.txt";
        private static readonly string VERSION_CS_PATH = "Assets/Scripts/ProjectApp/Data/ConfigVOVersion.cs";

        public static void InitData()
        {
            buildOutType = EditorPrefs.GetInt("ExcelTool_BuildOutType", 0);
            isEnciphermentData = EditorPrefs.GetBool("ExcelTool_IsEnciphermentData", false);
            isOutMultipleDatas = EditorPrefs.GetBool("ExcelTool_IsOutMultipleDatas", false);

            // 加载自定义版本号
            LoadCustomVersion();
        }

        public static void OnGUI(System.Action closeAction)
        {
            GUILayout.Space(5);

            ConfigBatTool.BuildOutType type = (ConfigBatTool.BuildOutType)EditorGUILayout.EnumPopup("[生成Class类型]:", (ConfigBatTool.BuildOutType)buildOutType, GUILayout.Width(400));
            buildOutType = (int)type;

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("[加密表数据]");
            GUILayout.Space(5);
            isEnciphermentData = EditorGUILayout.Toggle(isEnciphermentData);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("[每张表单独生成数据文件]");
            GUILayout.Space(5);
            isOutMultipleDatas = EditorGUILayout.Toggle(isOutMultipleDatas);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 添加版本设置区域
            DrawVersionSettings();

            GUILayout.Space(10);

            if (GUILayout.Button("自动打表", GUILayout.Height(40), GUILayout.Width(100)))
            {
                EditorPrefs.SetInt("ExcelTool_BuildOutType", buildOutType);
                EditorPrefs.SetBool("ExcelTool_IsEnciphermentData", isEnciphermentData);
                EditorPrefs.SetBool("ExcelTool_IsOutMultipleDatas", isOutMultipleDatas);

                closeAction?.Invoke();

                // 获取最终使用的版本号
                int finalVersion = GetFinalVersion();

                // 调用打表工具
                ConfigBatTool.SyncConfigData(finalVersion, type, isOutMultipleDatas, isEnciphermentData, AppFacade.AESIVector, AppFacade.AESKey);

                // 保存自定义版本号
                SaveCustomVersion();
            }

            if (GUILayout.Button("打开表格目录", GUILayout.Height(40), GUILayout.Width(100)))
            {
                ConfigBatTool.OpenExcelPath();
            }

            GUILayout.Space(20);

            // 添加分割线
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.Space(5);

            // 显示版本信息（放到下方）
            DrawVersionInfo();
        }

        /// <summary>
        /// 绘制版本设置区域
        /// </summary>
        private static void DrawVersionSettings()
        {
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("版本设置", EditorStyles.boldLabel, GUILayout.Width(80));

            if (GUILayout.Button(showVersionEdit ? "收起" : "展开", GUILayout.Width(60)))
            {
                showVersionEdit = !showVersionEdit;
            }
            GUILayout.EndHorizontal();

            if (showVersionEdit)
            {
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("当前默认版本:", GUILayout.Width(100));
                EditorGUILayout.LabelField(ConfigVOVersion.InternalVersion.ToString(), EditorStyles.boldLabel);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("自定义版本:", GUILayout.Width(100));

                string versionStr = EditorGUILayout.TextField(customVersion == -1 ? "" : customVersion.ToString(), GUILayout.Width(100));

                // 尝试解析输入的版本号
                if (int.TryParse(versionStr, out int newVersion) && newVersion > 0)
                {
                    customVersion = newVersion;
                }
                else if (string.IsNullOrEmpty(versionStr))
                {
                    customVersion = -1; // 使用默认版本
                }

                if (customVersion == -1)
                {
                    EditorGUILayout.LabelField("(将使用默认版本)", GUILayout.Width(120));
                }
                else
                {
                    EditorGUILayout.LabelField("(将使用自定义版本)", GUILayout.Width(120));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(100);
                if (GUILayout.Button("应用并更新配置文件", GUILayout.Width(150)))
                {
                    ApplyVersionToConfig();
                }
                if (GUILayout.Button("重置为默认版本", GUILayout.Width(120)))
                {
                    customVersion = -1;
                    SaveCustomVersion();
                    EditorUtility.DisplayDialog("提示", "已重置为默认版本", "确定");
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                EditorGUILayout.HelpBox("设置自定义版本号后，打表时会使用此版本号。如需永久修改默认版本，请点击\"应用并更新配置文件\"。", MessageType.Info);
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 获取最终使用的版本号
        /// </summary>
        private static int GetFinalVersion()
        {
            if (customVersion > 0)
            {
                return customVersion;
            }
            return ConfigVOVersion.InternalVersion;
        }

        /// <summary>
        /// 加载自定义版本号
        /// </summary>
        private static void LoadCustomVersion()
        {
            // 从EditorPrefs加载
            customVersion = EditorPrefs.GetInt("ExcelTool_CustomVersion", -1);
        }

        /// <summary>
        /// 保存自定义版本号
        /// </summary>
        private static void SaveCustomVersion()
        {
            EditorPrefs.SetInt("ExcelTool_CustomVersion", customVersion);
        }

        /// <summary>
        /// 应用版本到配置文件
        /// </summary>
        private static void ApplyVersionToConfig()
        {
            if (customVersion <= 0)
            {
                EditorUtility.DisplayDialog("提示", "请先设置有效的版本号（大于0的正整数）", "确定");
                return;
            }

            try
            {
                // 更新ConfigVOVersion.cs文件
                UpdateVersionFile(customVersion);

                // 重新加载配置
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

                EditorUtility.DisplayDialog("成功", $"版本号已更新为 {customVersion}\n请重新编译项目以使更改生效。", "确定");

                // 提示重新编译
                Debug.Log($"版本号已更新为 {customVersion}，请重新编译Unity项目");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("错误", $"更新版本文件失败：{e.Message}", "确定");
                Debug.LogError($"更新版本文件失败：{e.Message}");
            }
        }

        /// <summary>
        /// 更新ConfigVOVersion.cs文件中的版本号
        /// </summary>
        private static void UpdateVersionFile(int newVersion)
        {
            string filePath = Path.Combine(Application.dataPath, "../", VERSION_CS_PATH);

            if (!File.Exists(filePath))
            {
                // 尝试相对路径
                filePath = Path.Combine(Application.dataPath, VERSION_CS_PATH.Replace("Assets/", ""));
                if (!File.Exists(filePath))
                {
                    throw new System.Exception($"找不到版本文件：{VERSION_CS_PATH}");
                }
            }

            string content = File.ReadAllText(filePath, Encoding.UTF8);

            // 替换版本号
            string pattern = @"public const int InternalVersion = \d+;";
            string replacement = $"public const int InternalVersion = {newVersion};";

            if (System.Text.RegularExpressions.Regex.IsMatch(content, pattern))
            {
                string newContent = System.Text.RegularExpressions.Regex.Replace(content, pattern, replacement);
                File.WriteAllText(filePath, newContent, Encoding.UTF8);
                Debug.Log($"已更新版本文件：{filePath}，新版本号：{newVersion}");

                // 更新当前显示的版本
                typeof(ConfigVOVersion).GetField("InternalVersion", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    ?.SetValue(null, newVersion);
            }
            else
            {
                throw new System.Exception("在文件中找不到版本号定义");
            }
        }

        /// <summary>
        /// 绘制版本信息
        /// </summary>
        private static void DrawVersionInfo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginVertical();

            GUIStyle versionStyle = new GUIStyle(EditorStyles.boldLabel);
            versionStyle.normal.textColor = new Color(0.2f, 0.6f, 0.8f);
            versionStyle.fontSize = 12;

            GUIStyle hashStyle = new GUIStyle(EditorStyles.label);
            hashStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            hashStyle.fontSize = 10;

            GUIStyle customStyle = new GUIStyle(EditorStyles.label);
            customStyle.normal.textColor = new Color(0.8f, 0.6f, 0.2f);
            customStyle.fontSize = 10;

            // 显示实际使用的版本号
            int usingVersion = GetFinalVersion();
            bool isUsingCustom = customVersion > 0;

            if (isUsingCustom)
            {
                EditorGUILayout.LabelField($"配置表版本: v{usingVersion} (自定义)", versionStyle);
                EditorGUILayout.LabelField($"默认版本: v{ConfigVOVersion.InternalVersion}", hashStyle);
            }
            else
            {
                EditorGUILayout.LabelField($"配置表版本: v{usingVersion}", versionStyle);
            }

            // 显示哈希值
            string shortHash = ConfigVOVersion.InternalHash.Length > 8 ?
                ConfigVOVersion.InternalHash.Substring(0, 8) + "..." :
                ConfigVOVersion.InternalHash;
            EditorGUILayout.LabelField($"版本标识: {shortHash}", hashStyle);

            // 显示打表时间
            string displayTime = FormatBuildTime(ConfigVOVersion.InternalTime);
            EditorGUILayout.LabelField($"打表时间: {displayTime}", hashStyle);

            // 如果开启了加密，显示AES信息
            if (isEnciphermentData)
            {
                GUILayout.Space(5);
                GUIStyle keyStyle = new GUIStyle(EditorStyles.label);
                keyStyle.fontSize = 10;
                keyStyle.fontStyle = FontStyle.Italic;

                DrawAESField("AES IVector", AppFacade.AESIVector, ref showIVector, keyStyle);
                DrawAESField("AES Key", AppFacade.AESKey, ref showAESKey, keyStyle);

                // 显示密钥长度验证
                int keyLength = AppFacade.AESKey.Length / 2;
                int ivLength = AppFacade.AESIVector.Length / 2;

                GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
                statusStyle.fontSize = 9;

                if (keyLength == 32)
                {
                    statusStyle.normal.textColor = Color.green;
                    EditorGUILayout.LabelField($"✅ AES密钥强度: 256位", statusStyle);
                }
                else if (keyLength == 16)
                {
                    statusStyle.normal.textColor = new Color(1f, 0.6f, 0f);
                    EditorGUILayout.LabelField($"⚠️ AES密钥强度: 128位", statusStyle);
                }
                else
                {
                    statusStyle.normal.textColor = Color.red;
                    EditorGUILayout.LabelField($"❌ AES密钥长度异常: {keyLength}字节 (需要16或32字节)", statusStyle);
                }

                if (ivLength == 16)
                {
                    statusStyle.normal.textColor = Color.green;
                    EditorGUILayout.LabelField($"✅ IVector长度: 128位", statusStyle);
                }
                else
                {
                    statusStyle.normal.textColor = Color.red;
                    EditorGUILayout.LabelField($"❌ IVector长度异常: {ivLength}字节 (需要16字节)", statusStyle);
                }
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制AES字段（带显示/隐藏按钮）- 统一显示规则
        /// </summary>
        private static void DrawAESField(string label, string value, ref bool showValue, GUIStyle style)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"{label}:", GUILayout.Width(90));

            string displayValue;
            if (showValue)
            {
                displayValue = value;
                style.normal.textColor = new Color(0.2f, 0.8f, 0.2f);
            }
            else
            {
                int showLength = Mathf.Min(8, value.Length);
                string hiddenPart = new string('*', value.Length - showLength);
                string visiblePart = value.Substring(value.Length - showLength);
                displayValue = hiddenPart + visiblePart;
                style.normal.textColor = new Color(0.8f, 0.4f, 0.2f);
            }

            EditorGUILayout.LabelField(displayValue, style, GUILayout.MinWidth(220));

            if (GUILayout.Button(showValue ? "隐藏" : "显示", GUILayout.Width(40), GUILayout.Height(18)))
            {
                showValue = !showValue;
            }

            if (GUILayout.Button("复制", GUILayout.Width(40), GUILayout.Height(18)))
            {
                EditorGUIUtility.systemCopyBuffer = value;
                Debug.Log($"已复制{label}: {value}");
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 格式化打表时间显示
        /// </summary>
        private static string FormatBuildTime(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr) || timeStr.Length != 14)
            {
                return timeStr;
            }

            try
            {
                string year = timeStr.Substring(0, 4);
                string month = timeStr.Substring(4, 2);
                string day = timeStr.Substring(6, 2);
                string hour = timeStr.Substring(8, 2);
                string minute = timeStr.Substring(10, 2);
                string second = timeStr.Substring(12, 2);

                return $"{year}-{month}-{day} {hour}:{minute}:{second}";
            }
            catch
            {
                return timeStr;
            }
        }
    }
}