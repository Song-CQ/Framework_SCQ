/****************************************************
    文件: UGUIWnd_EditorView.cs
    作者: Clear
    日期: 2023/11/20 15:25:28
    类型: 框架核心脚本(请勿修改)
    功能: UGUI界面编辑器视图
*****************************************************/
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FutureEditor
{
    public static class UGUIWnd_EditorView
    {
        [MenuItem("Assets/[FC EditorTool]/[UGUI Editor]/[更新选中界面组件常量]", priority = -50),
            MenuItem("GameObject/[FC EditorTool]/[UGUI Editor]/[更新选中界面组件常量]", priority = -50)]
        public static void AddUGUIEditorTool()
        {
            GameObject obj = Selection.activeGameObject;
            if (!obj)
            {
                return;
            }
            UpdateComponents(obj);
        }

        [MenuItem("Assets/[FC EditorTool]/[UGUI Editor]/[更新选中界面组件常量]", priority = -50, validate = true),
            MenuItem("GameObject/[FC EditorTool]/[UGUI Editor]/[更新选中界面组件常量]", priority = -50, validate = true)]
        public static bool Check()
        {
            GameObject obj = Selection.activeGameObject;
            if (!obj)
            {
                return false;
            }
            return true;
        }

        public static void UpdateComponents(GameObject obj)
        {
            string name = obj.name.Replace("_Plane", "UI");
            string[] files = Directory.GetFiles(UnityEditorPathConst.ModuleUIPath, name + ".cs", SearchOption.AllDirectories);
            if (files.Length != 1)
            {
                Debug.LogError("界面不存在或名字重复:" + name);
                return;
            }
            string path = files[0];
            string text = File.ReadAllText(path);

            // 检查是否有控件常量区域
            if (!text.Contains("#region 控件常量"))
            {
                Debug.LogError("无控件常量标志: #region 控件常量");
                return;
            }

            // 1. 收集场景中的所有UI组件（只收集名称以 UI_ 或 ui_ 开头的）
            List<UIComponentInfo> allComponents = CollectUIComponents(obj);

            // 2. 解析现有代码中已经存在的常量和组件（通过常量Key映射）
            var existingConstants = ParseExistingConstants(text);
            var existingComponentMap = ParseExistingComponentsWithKey(text);

            // 3. 找出需要新增的常量和组件
            var newConstants = allComponents
                .Where(c => !existingConstants.Contains(c.ConstantKey))
                .GroupBy(c => c.ConstantKey)
                .Select(g => g.First())
                .ToList();

            var newComponents = allComponents
                .Where(c => !existingComponentMap.ContainsKey(c.ConstantKey))
                .ToList();

            if (newConstants.Count == 0 && newComponents.Count == 0)
            {
                Debug.Log("没有需要新增的UI组件");
                return;
            }

            // 4. 生成需要添加的代码
            string constantsToAdd = GenerateConstantsOnly(newConstants);
            string componentsToAdd = GenerateComponentsOnly(newComponents);
            string initCodeToAdd = GenerateInitCodeOnly(newComponents);

            // 5. 更新文件内容
            string updatedText = MergeIntoFile(text, constantsToAdd, componentsToAdd, initCodeToAdd);

            File.Delete(path);
            File.WriteAllText(path, updatedText, Encoding.UTF8);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"成功更新界面: {name}，新增常量 {newConstants.Count} 个，新增组件 {newComponents.Count} 个");
        }

        /// <summary>
        /// 收集UI组件信息（只收集名称以UI_或ui_开头的GameObject）
        /// </summary>
        private static List<UIComponentInfo> CollectUIComponents(GameObject root)
        {
            List<UIComponentInfo> components = new List<UIComponentInfo>();
            CollectComponentsRecursively(root.transform, components);
            return components;
        }

        private static void CollectComponentsRecursively(Transform transform, List<UIComponentInfo> components)
        {
            // 检查当前GameObject名称是否以 UI_ 或 ui_ 开头（兼容大小写）
            bool isUIObject = transform.name.StartsWith("UI_", System.StringComparison.OrdinalIgnoreCase);

            if (isUIObject)
            {
                // 获取当前对象上的所有UI组件
                var uiComponents = transform.GetComponents<Component>()
                    .Where(c => IsUIComponent(c))
                    .ToList();

                foreach (var comp in uiComponents)
                {
                    string componentType = GetComponentTypeName(comp);      // 完整类型名，如 TextMeshProUGUI
                    string fieldTypeShort = GetFieldTypeShort(comp);        // 字段名后缀简写，如 TMPText
                    string baseName = GetBaseName(transform.name);
                    string constantKey = GetConstantKey(transform.name);
                    string defaultFieldName = GetFieldName(baseName, fieldTypeShort);

                    components.Add(new UIComponentInfo
                    {
                        GameObjectName = transform.name,
                        ConstantKey = constantKey,
                        Component = comp,
                        ComponentType = componentType,      // 完整类型名，用于 GetComponent
                        FieldType = componentType,          // 字段类型使用完整类型名
                        DefaultFieldName = defaultFieldName, // 默认字段名
                        FieldName = defaultFieldName,       // 实际使用的字段名（可能被覆盖）
                        Path = GetRelativePath(transform)
                    });
                }
            }

            // 递归处理子物体
            foreach (Transform child in transform)
            {
                CollectComponentsRecursively(child, components);
            }
        }

        /// <summary>
        /// 获取基础名称（去掉UI_前缀）
        /// </summary>
        private static string GetBaseName(string gameObjectName)
        {
            string name = gameObjectName;
            if (name.StartsWith("UI_", System.StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(3); // 去掉 "UI_"
            }
            return name;
        }

        /// <summary>
        /// 获取常量Key（格式：ui_xxx_Key，保持原始大小写）
        /// </summary>
        private static string GetConstantKey(string gameObjectName)
        {
            // 保持原始大小写，只添加 _Key 后缀
            return $"{gameObjectName}_Key";
        }

        /// <summary>
        /// 获取字段名后缀简写（用于字段名）
        /// </summary>
        private static string GetFieldTypeShort(Component comp)
        {
            if (comp is Button) return "Button";
            if (comp is Text) return "Text";
            if (comp is Image) return "Image";
            if (comp is RawImage) return "RawImage";
            if (comp is InputField) return "InputField";
            if (comp is Slider) return "Slider";
            if (comp is Scrollbar) return "Scrollbar";
            if (comp is Toggle) return "Toggle";
            if (comp is ScrollRect) return "ScrollRect";
            if (comp is Dropdown) return "Dropdown";
            if (comp is TextMeshProUGUI) return "TMPText";
            if (comp is TMP_InputField) return "TMPInputField";
            if (comp is TMP_Dropdown) return "TMPDropdown";
            if (comp is TMP_Text) return "TMPText";
            return comp.GetType().Name;
        }

        /// <summary>
        /// 获取组件完整类型名称（用于字段类型声明和GetComponent）
        /// </summary>
        private static string GetComponentTypeName(Component comp)
        {
            if (comp is Button) return "Button";
            if (comp is Text) return "Text";
            if (comp is Image) return "Image";
            if (comp is RawImage) return "RawImage";
            if (comp is InputField) return "InputField";
            if (comp is Slider) return "Slider";
            if (comp is Scrollbar) return "Scrollbar";
            if (comp is Toggle) return "Toggle";
            if (comp is ScrollRect) return "ScrollRect";
            if (comp is Dropdown) return "Dropdown";
            if (comp is TextMeshProUGUI) return "TextMeshProUGUI";
            if (comp is TMP_InputField) return "TMP_InputField";
            if (comp is TMP_Dropdown) return "TMP_Dropdown";
            if (comp is TMP_Text) return "TMP_Text";
            return comp.GetType().Name;
        }

        /// <summary>
        /// 获取默认字段名（格式：ui_xxx_ShortType，首字母保持原始大小写）
        /// </summary>
        private static string GetFieldName(string baseName, string shortType)
        {
            // 保持 baseName 的原始大小写（不改变首字母大小写）
            // 格式：ui_ + baseName + _ + shortType
            return $"ui_{baseName}_{shortType}";
        }

        /// <summary>
        /// 判断是否是UI组件
        /// </summary>
        private static bool IsUIComponent(Component comp)
        {
            return comp is Button ||
                   comp is Text ||
                   comp is Image ||
                   comp is RawImage ||
                   comp is InputField ||
                   comp is Slider ||
                   comp is Scrollbar ||
                   comp is Toggle ||
                   comp is ScrollRect ||
                   comp is Dropdown ||
                   comp is TextMeshProUGUI ||
                   comp is TMP_InputField ||
                   comp is TMP_Dropdown ||
                   comp is TMP_Text;
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        private static string GetRelativePath(Transform transform)
        {
            string path = transform.name;
            Transform parent = transform.parent;

            while (parent != null && parent.name != "Canvas" && parent.name != "Panel")
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }

        /// <summary>
        /// 解析现有的常量定义
        /// </summary>
        private static HashSet<string> ParseExistingConstants(string fileContent)
        {
            HashSet<string> constants = new HashSet<string>();

            // 匹配 private const string xxx_Key = "xxx";
            string pattern = @"private\s+const\s+string\s+(\w+_Key)\s*=\s*""([^""]+)""";
            var matches = Regex.Matches(fileContent, pattern);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    constants.Add(match.Groups[1].Value);
                }
            }

            return constants;
        }

        /// <summary>
        /// 解析现有的组件字段定义，返回常量Key到字段名的映射
        /// </summary>
        private static Dictionary<string, string> ParseExistingComponentsWithKey(string fileContent)
        {
            Dictionary<string, string> componentMap = new Dictionary<string, string>();

            // 先解析InitUIComponents方法中的赋值语句，获取常量Key到字段名的映射
            // 匹配: xxx_FieldName = GetComponent<...>(ui_XXX_Key);
            string initMethodPattern = @"private\s+void\s+InitUIComponents\s*\(\s*\)\s*\n?\s*\{\s*\n(.*?)\n\s*\}";
            var initMethodMatch = Regex.Match(fileContent, initMethodPattern, RegexOptions.Singleline);

            if (initMethodMatch.Success)
            {
                string methodBody = initMethodMatch.Groups[1].Value;
                // 匹配: fieldName = GetComponent<Type>(constantKey);
                string assignmentPattern = @"(\w+)\s*=\s*GetComponent<[\w<>]+>\((\w+_Key)\);";
                var matches = Regex.Matches(methodBody, assignmentPattern);

                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 2)
                    {
                        string fieldName = match.Groups[1].Value;
                        string constantKey = match.Groups[2].Value;
                        componentMap[constantKey] = fieldName;
                    }
                }
            }

            return componentMap;
        }

        /// <summary>
        /// 生成常量代码（保持原始大小写）
        /// </summary>
        private static string GenerateConstantsOnly(List<UIComponentInfo> components)
        {
            if (components.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine();

            foreach (var comp in components)
            {
                // 常量值保持 GameObject 的原始名称
                sb.AppendLine($"        private const string {comp.ConstantKey} = \"{comp.GameObjectName}\";");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成组件字段代码（字段名带 ui_ 前缀）
        /// </summary>
        private static string GenerateComponentsOnly(List<UIComponentInfo> components)
        {
            if (components.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine();

            foreach (var comp in components)
            {
                // 使用带 ui_ 前缀的字段名
                sb.AppendLine($"        private {comp.ComponentType} {comp.DefaultFieldName};");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成初始化代码
        /// </summary>
        private static string GenerateInitCodeOnly(List<UIComponentInfo> components)
        {
            if (components.Count == 0) return "";

            StringBuilder sb = new StringBuilder();

            foreach (var comp in components)
            {
                // 使用带 ui_ 前缀的字段名
                sb.AppendLine($"            {comp.DefaultFieldName} = GetComponent<{comp.ComponentType}>({comp.ConstantKey});");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 智能合并到现有文件中
        /// </summary>
        private static string MergeIntoFile(string originalText, string constantsToAdd, string componentsToAdd, string initCodeToAdd)
        {
            // 查找控件常量区域
            int startIndex = originalText.IndexOf("#region 控件常量") + 12;
            int endIndex = originalText.Substring(startIndex).IndexOf("#endregion") + startIndex;

            // 查找InitUIComponents方法（如果存在）
            string initMethodPattern = @"private\s+void\s+InitUIComponents\s*\(\s*\)\s*\n?\s*\{\s*\n";
            var initMatch = Regex.Match(originalText, initMethodPattern);

            string updatedText = originalText;

            // 1. 添加新的常量
            if (!string.IsNullOrEmpty(constantsToAdd))
            {
                // 在常量区域末尾添加新常量
                string constantsRegion = originalText.Substring(startIndex, endIndex - startIndex);
                updatedText = originalText.Substring(0, startIndex) +
                             constantsRegion +
                             constantsToAdd +
                             "\n        " +
                             originalText.Substring(endIndex, originalText.Length - endIndex);

                // 更新索引
                startIndex = updatedText.IndexOf("#region 控件常量") + 12;
                endIndex = updatedText.Substring(startIndex).IndexOf("#endregion") + startIndex;
            }

            // 2. 添加新的组件字段（在控件常量区域之后，类定义中合适的位置）
            if (!string.IsNullOrEmpty(componentsToAdd))
            {
                // 在控件常量区域之后添加组件字段
                updatedText = updatedText.Insert(endIndex, componentsToAdd);

                // 更新索引
                startIndex = updatedText.IndexOf("#region 控件常量") + 12;
                endIndex = updatedText.Substring(startIndex).IndexOf("#endregion") + startIndex;
            }

            // 3. 添加初始化代码
            if (!string.IsNullOrEmpty(initCodeToAdd))
            {
                if (initMatch.Success)
                {
                    // 如果存在InitUIComponents方法，在方法内部添加
                    int methodStart = initMatch.Index + initMatch.Length;
                    updatedText = updatedText.Insert(methodStart, initCodeToAdd);
                }
                else
                {
                    // 如果不存在，创建InitUIComponents方法
                    string initMethod = @"
        /// <summary>
        /// 初始化所有UI组件引用
        /// </summary>
        private void InitUIComponents()
        {
" + initCodeToAdd + @"        }";

                    // 在合适的位置插入（通常在常量定义之后）
                    updatedText = updatedText.Insert(endIndex, initMethod);
                }
            }

            // 4. 修复 #endregion 换行问题
            updatedText = Regex.Replace(updatedText, @"([^\n])\#endregion", "$1\n        #endregion");

            return updatedText;
        }
    }

    /// <summary>
    /// UI组件信息
    /// </summary>
    public class UIComponentInfo
    {
        public string GameObjectName;      // GameObject名称
        public string ConstantKey;          // 常量Key，如：ui_BeesTrf_Key
        public Component Component;         // 组件引用
        public string ComponentType;        // 完整组件类型，如：TextMeshProUGUI
        public string FieldType;            // 字段类型（完整类型名）
        public string DefaultFieldName;     // 默认字段名，如：ui_BeesTrf_TMPText
        public string FieldName;            // 实际使用的字段名（可能被用户修改）
        public string Path;                 // 路径
    }
}