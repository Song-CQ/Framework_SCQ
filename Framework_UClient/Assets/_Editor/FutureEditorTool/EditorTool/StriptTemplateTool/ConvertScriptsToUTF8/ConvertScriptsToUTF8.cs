/****************************************************
    文件: ConvertScriptsToUTF8.cs
    作者: Clear
    日期: 2026/3/5
    类型: 工具脚本
    功能: 将项目中所有C#脚本转换为UTF-8无BOM格式，支持自动识别各种编码
*****************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace FutureEditor
{
    public class ConvertScriptsToUTF8
    {
        private enum ConvertResult
        {
            Converted,
            Skipped,
            Uncertain,
            Error
        }

        [MenuItem("Tools/转换所有C#脚本为UTF-8（无BOM）")]
        public static void ConvertAllToUTF8NoBOM()
        {
            ConvertScriptsWithProgress(false);
        }

        [MenuItem("Tools/转换所有C#脚本为UTF-8（带BOM）")]
        public static void ConvertAllToUTF8WithBOM()
        {
            ConvertScriptsWithProgress(true);
        }

        [MenuItem("Tools/强制移除所有BOM")]
        public static void ForceRemoveAllBOM()
        {
            bool confirm = EditorUtility.DisplayDialog(
                "⚠️ 警告：强制移除BOM",
                "强制移除BOM会直接修改所有C#文件的文件头。\n\n" +
                "风险说明：\n" +
                "• 此操作不可逆\n" +
                "• 建议先提交Git或备份文件\n" +
                "• 如果文件已经是UTF-8无BOM，不会发生变化\n\n" +
                "确定要继续吗？",
                "是的，我确定",
                "取消"
            );

            if (!confirm) return;

            string projectPath = Path.GetDirectoryName(Application.dataPath);
            string[] csFiles = GetCsFiles(projectPath);

            int removedCount = 0;
            int totalFiles = csFiles.Length;

            try
            {
                for (int i = 0; i < totalFiles; i++)
                {
                    string filePath = csFiles[i];

                    float progress = (float)i / totalFiles;

                    if (EditorUtility.DisplayCancelableProgressBar(
                        "强制移除BOM",
                        $"正在处理: {Path.GetFileName(filePath)}\n进度: {i + 1}/{totalFiles}",
                        progress))
                    {
                        break;
                    }

                    byte[] bytes = File.ReadAllBytes(filePath);

                    // 检查是否有 BOM
                    if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                    {
                        // 移除 BOM，用 UTF-8 无 BOM 写入
                        string content = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
                        File.WriteAllText(filePath, content, new UTF8Encoding(false));
                        removedCount++;
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            EditorUtility.DisplayDialog("强制移除BOM", $"处理完成！\n移除 BOM 的文件数: {removedCount}", "确定");
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/强制转换所有为UTF-8无BOM")]
        public static void ForceConvertAllToUTF8NoBOM()
        {
            bool confirm = EditorUtility.DisplayDialog(
                "⚠️ 警告：强制转换编码",
                "强制转换会修改所有C#文件的编码格式。\n\n" +
                "风险说明：\n" +
                "• 此操作不可逆\n" +
                "• 如果编码检测错误，可能导致文件乱码\n" +
                "• 强烈建议先提交Git或备份文件\n\n" +
                "确定要继续吗？",
                "是的，我确定",
                "取消"
            );

            if (!confirm) return;

            bool confirm2 = EditorUtility.DisplayDialog(
                "最后确认",
                "你真的确定要强制转换所有文件为UTF-8无BOM吗？\n\n这是一个危险操作！",
                "确定",
                "取消"
            );

            if (!confirm2) return;

            string projectPath = Path.GetDirectoryName(Application.dataPath);
            string[] csFiles = GetCsFiles(projectPath);

            int convertedCount = 0;
            int totalFiles = csFiles.Length;

            try
            {
                for (int i = 0; i < totalFiles; i++)
                {
                    string filePath = csFiles[i];

                    float progress = (float)i / totalFiles;

                    if (EditorUtility.DisplayCancelableProgressBar(
                        "强制转换UTF-8无BOM",
                        $"正在处理: {Path.GetFileName(filePath)}\n进度: {i + 1}/{totalFiles}",
                        progress))
                    {
                        break;
                    }

                    byte[] bytes = File.ReadAllBytes(filePath);

                    // 不管什么编码，强制用 UTF-8 读取并保存为无 BOM
                    string content;
                    try
                    {
                        // 尝试用 UTF-8 读取
                        content = Encoding.UTF8.GetString(bytes);
                    }
                    catch
                    {
                        // 如果 UTF-8 读取失败，用系统默认编码
                        content = Encoding.Default.GetString(bytes);
                    }

                    // 用 UTF-8 无 BOM 写入
                    File.WriteAllText(filePath, content, new UTF8Encoding(false));
                    convertedCount++;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            EditorUtility.DisplayDialog("强制转换完成", $"转换完成！\n成功转换: {convertedCount} 个文件", "确定");
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/检测带BOM的文件")]
        public static void DetectBOMFiles()
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);
            string[] csFiles = GetCsFiles(projectPath);

            List<string> bomFiles = new List<string>();

            foreach (string filePath in csFiles)
            {
                byte[] bytes = File.ReadAllBytes(filePath);

                if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                {
                    bomFiles.Add(filePath);
                }
            }

            if (bomFiles.Count == 0)
            {
                EditorUtility.DisplayDialog("检测结果", "没有发现带 BOM 的文件！✅", "确定");
            }
            else
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine($"发现 {bomFiles.Count} 个带 BOM 的文件：\n");
                foreach (string file in bomFiles.Take(20))
                {
                    message.AppendLine(Path.GetFileName(file));
                }
                if (bomFiles.Count > 20)
                    message.AppendLine($"... 还有 {bomFiles.Count - 20} 个");

                EditorUtility.DisplayDialog("检测结果", message.ToString(), "确定");
            }
        }

        [MenuItem("Tools/检测脚本编码")]
        public static void DetectScriptsEncoding()
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);
            string[] csFiles = GetCsFiles(projectPath);

            Dictionary<string, int> encodingStats = new Dictionary<string, int>();
            List<string> uncertainFiles = new List<string>();

            try
            {
                for (int i = 0; i < csFiles.Length; i++)
                {
                    string filePath = csFiles[i];

                    float progress = (float)i / csFiles.Length;

                    if (EditorUtility.DisplayCancelableProgressBar(
                        "检测脚本编码",
                        $"正在检测: {Path.GetFileName(filePath)}\n进度: {i + 1}/{csFiles.Length}",
                        progress))
                    {
                        break;
                    }

                    byte[] bytes = File.ReadAllBytes(filePath);
                    Encoding detected = EncodingHelper.DetectEncoding(bytes, out float confidence);

                    string encodingName = EncodingHelper.GetEncodingDisplayName(detected, bytes);

                    if (encodingStats.ContainsKey(encodingName))
                        encodingStats[encodingName]++;
                    else
                        encodingStats[encodingName] = 1;

                    if (confidence < 0.8f)
                    {
                        uncertainFiles.Add(filePath);
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            ShowDetectionResult(csFiles.Length, encodingStats, uncertainFiles);
        }

        [MenuItem("Tools/测试单个文件编码")]
        public static void TestSingleFile()
        {
            string filePath = EditorUtility.OpenFilePanel("选择要测试的CS文件", Application.dataPath, "cs");
            if (string.IsNullOrEmpty(filePath)) return;

            byte[] bytes = File.ReadAllBytes(filePath);

            Debug.Log("===== 文件编码测试 =====");
            Debug.Log($"文件: {Path.GetFileName(filePath)}");
            Debug.Log($"文件大小: {bytes.Length} 字节");
            Debug.Log($"是否有 BOM: {EncodingHelper.HasBOM(bytes)}");

            // 尝试用 UTF-8 读取
            string content = Encoding.UTF8.GetString(bytes);
            byte[] reencoded = Encoding.UTF8.GetBytes(content);

            bool isUTF8 = reencoded.Length == bytes.Length;
            for (int i = 0; i < bytes.Length && isUTF8; i++)
            {
                if (reencoded[i] != bytes[i])
                    isUTF8 = false;
            }

            Debug.Log($"是否是 UTF-8 无 BOM: {isUTF8}");

            if (isUTF8)
            {
                Debug.Log("✅ 文件已经是 UTF-8 无 BOM，不需要转换");
            }
        }

        private static void ConvertScriptsWithProgress(bool withBOM)
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);
            string[] csFiles = GetCsFiles(projectPath);

            int totalFiles = csFiles.Length;
            int convertedCount = 0;
            int skippedCount = 0;
            int uncertainCount = 0;
            int errorCount = 0;
            bool cancelled = false;

            List<string> uncertainFiles = new List<string>();
            List<string> errorFiles = new List<string>();

            try
            {
                for (int i = 0; i < totalFiles; i++)
                {
                    string filePath = csFiles[i];

                    float progress = (float)i / totalFiles;

                    if (EditorUtility.DisplayCancelableProgressBar(
                        "转换C#脚本编码",
                        $"正在处理: {Path.GetFileName(filePath)}\n进度: {i + 1}/{totalFiles}",
                        progress))
                    {
                        cancelled = true;
                        break;
                    }

                    try
                    {
                        var result = ConvertFileToUTF8(filePath, withBOM);

                        switch (result)
                        {
                            case ConvertResult.Converted:
                                convertedCount++;
                                break;
                            case ConvertResult.Skipped:
                                skippedCount++;
                                break;
                            case ConvertResult.Uncertain:
                                uncertainCount++;
                                uncertainFiles.Add(filePath);
                                break;
                            case ConvertResult.Error:
                                errorCount++;
                                errorFiles.Add(filePath);
                                break;
                        }
                    }
                    catch (System.Exception e)
                    {
                        errorCount++;
                        errorFiles.Add($"{Path.GetFileName(filePath)}: {e.Message}");
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            ShowConversionResult(withBOM, cancelled, convertedCount, skippedCount,
                                uncertainCount, errorCount, uncertainFiles, errorFiles);

            AssetDatabase.Refresh();
        }

        private static string[] GetCsFiles(string projectPath)
        {
            return Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains("\\Library\\")
                            && !path.Contains("\\Temp\\")
                            && !path.Contains("\\obj\\")
                            && !path.Contains("\\PackageCache\\"))
                .ToArray();
        }

        private static ConvertResult ConvertFileToUTF8(string filePath, bool withBOM)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            bool hasBOM = EncodingHelper.HasBOM(fileBytes);

            // 如果目标是不带 BOM，但文件带有 BOM → 需要转换
            if (!withBOM && hasBOM)
            {
                // 移除 BOM，用 UTF-8 无 BOM 写入
                string content = Encoding.UTF8.GetString(fileBytes, 3, fileBytes.Length - 3);
                File.WriteAllText(filePath, content, new UTF8Encoding(false));
                return ConvertResult.Converted;
            }

            // 如果目标是带 BOM，但文件不带 BOM → 需要转换
            if (withBOM && !hasBOM)
            {
                string content = File.ReadAllText(filePath, Encoding.UTF8);
                File.WriteAllText(filePath, content, new UTF8Encoding(true));
                return ConvertResult.Converted;
            }

            // 其他情况，用编码检测
            Encoding currentEncoding = EncodingHelper.DetectEncoding(fileBytes, out float confidence);

            // 检查是否已经是目标UTF-8格式
            bool isTargetUTF8 = false;
            if (withBOM)
            {
                isTargetUTF8 = currentEncoding == Encoding.UTF8 && hasBOM;
            }
            else
            {
                isTargetUTF8 = currentEncoding == Encoding.UTF8 && !hasBOM;
            }

            if (isTargetUTF8)
            {
                return ConvertResult.Skipped;
            }

            // 如果不确定编码，返回 Uncertain
            if (confidence < 0.3f)
            {
                return ConvertResult.Uncertain;
            }

            // 读取文件内容
            string content2 = currentEncoding.GetString(fileBytes);

            // 用目标UTF-8格式写入
            File.WriteAllText(filePath, content2, new UTF8Encoding(withBOM));

            return ConvertResult.Converted;
        }

        private static void ShowDetectionResult(int totalFiles, Dictionary<string, int> stats, List<string> uncertainFiles)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("编码检测结果：");
            result.AppendLine("==================");

            foreach (var pair in stats.OrderByDescending(p => p.Value))
            {
                result.AppendLine($"{pair.Key}: {pair.Value} 个文件");
            }

            result.AppendLine("==================");
            result.AppendLine($"总计: {totalFiles} 个文件");

            if (uncertainFiles.Count > 0)
            {
                result.AppendLine($"\n⚠️ 不确定编码的文件 ({uncertainFiles.Count} 个):");
                foreach (string file in uncertainFiles.Take(10))
                {
                    result.AppendLine($"  {Path.GetFileName(file)}");
                }
                if (uncertainFiles.Count > 10)
                    result.AppendLine($"  ... 还有 {uncertainFiles.Count - 10} 个");
            }

            EditorUtility.DisplayDialog("编码检测结果", result.ToString(), "确定");
            Debug.Log(result.ToString());
        }

        private static void ShowConversionResult(bool withBOM, bool cancelled, int converted, int skipped,
                                                int uncertain, int error, List<string> uncertainFiles, List<string> errorFiles)
        {
            string bomType = withBOM ? "带BOM" : "无BOM";
            StringBuilder message = new StringBuilder();

            if (cancelled)
            {
                message.AppendLine($"❌ 转换已取消！");
            }
            else
            {
                message.AppendLine($"✅ 转换完成！");
            }

            message.AppendLine($"成功转换: {converted} 个文件");
            message.AppendLine($"跳过(已是UTF-8): {skipped} 个文件");

            if (uncertain > 0)
            {
                message.AppendLine($"⚠️ 不确定编码: {uncertain} 个文件（已跳过）");
            }

            if (error > 0)
            {
                message.AppendLine($"❌ 转换失败: {error} 个文件");
            }

            if (uncertainFiles.Count > 0)
            {
                message.AppendLine("\n不确定编码的文件:");
                foreach (string file in uncertainFiles.Take(5))
                {
                    message.AppendLine($"  {Path.GetFileName(file)}");
                }
            }

            if (errorFiles.Count > 0)
            {
                message.AppendLine("\n失败文件:");
                foreach (string file in errorFiles.Take(5))
                {
                    message.AppendLine($"  {file}");
                }
            }

            EditorUtility.DisplayDialog($"UTF-8 {bomType} 转换", message.ToString(), "确定");

            if (!cancelled)
            {
                Debug.Log($"转换完成：{converted} 个文件转为 UTF-8 {bomType}，跳过：{skipped} 个，不确定：{uncertain} 个，失败：{error} 个");
            }
        }
    }
}