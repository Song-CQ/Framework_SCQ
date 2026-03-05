/****************************************************
    文件: ConvertScriptsToUTF8.cs
    作者: Clear
    日期: 2026/3/4 18:14:5
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;

namespace FutureEditor
{
    public class ConvertScriptsToUTF8
    {
        // [MenuItem("Tools/转换所有C#脚本为UTF-8（无BOM）")]
        public static void ConvertAllToUTF8NoBOM()
        {
            ConvertScriptsWithProgress(false);
        }

        // [MenuItem("Tools/转换所有C#脚本为UTF-8（带BOM）")]
        public static void ConvertAllToUTF8WithBOM()
        {
            ConvertScriptsWithProgress(true);
        }

        static void ConvertScriptsWithProgress(bool withBOM)
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);

            // 获取所有 .cs 文件（排除临时文件夹）
            string[] csFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains("\\Library\\")
                            && !path.Contains("\\Temp\\")
                            && !path.Contains("\\obj\\")
                            && !path.Contains("\\PackageCache\\"))
                .ToArray();

            int totalFiles = csFiles.Length;
            int convertedCount = 0;
            int skippedCount = 0;
            bool cancelled = false;

            try
            {
                for (int i = 0; i < totalFiles; i++)
                {
                    string filePath = csFiles[i];
                    
                    // 计算进度
                    float progress = (float)i / totalFiles;
                    
                    // 显示进度对话框
                    if (EditorUtility.DisplayCancelableProgressBar(
                        "转换C#脚本编码",
                        $"正在处理: {Path.GetFileName(filePath)}\n进度: {i + 1}/{totalFiles}",
                        progress))
                    {
                        // 用户点击了取消按钮
                        cancelled = true;
                        break;
                    }

                    // 转换文件
                    if (ConvertFileToUTF8(filePath, withBOM))
                    {
                        convertedCount++;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }
            }
            finally
            {
                // 关闭进度对话框
                EditorUtility.ClearProgressBar();
            }

            // 显示结果对话框
            string bomType = withBOM ? "带BOM" : "无BOM";
            string message = cancelled ? 
                $"转换已取消！\n已转换: {convertedCount} 个文件\n跳过: {skippedCount} 个文件" :
                $"转换完成！\n成功转换: {convertedCount} 个文件\n跳过(已是UTF-8): {skippedCount} 个文件";
            
            EditorUtility.DisplayDialog($"UTF-8 {bomType} 转换", message, "确定");
            
            if (!cancelled)
            {
                Debug.Log($"转换完成：{convertedCount} 个文件转为 UTF-8 {bomType}，跳过：{skippedCount} 个文件");
            }
            else
            {
                Debug.LogWarning($"转换已取消：已转换 {convertedCount} 个文件，跳过 {skippedCount} 个文件");
            }

            AssetDatabase.Refresh();
        }

        static bool ConvertFileToUTF8(string filePath, bool withBOM)
        {
            try
            {
                // 读取所有字节检测当前编码
                byte[] fileBytes = File.ReadAllBytes(filePath);
                Encoding currentEncoding = DetectEncoding(fileBytes);

                // 检查是否已经是目标UTF-8格式
                bool isTargetUTF8 = false;
                if (withBOM)
                {
                    isTargetUTF8 = currentEncoding == Encoding.UTF8 &&
                                   fileBytes.Length >= 3 &&
                                   fileBytes[0] == 0xEF && fileBytes[1] == 0xBB && fileBytes[2] == 0xBF;
                }
                else
                {
                    isTargetUTF8 = currentEncoding == Encoding.UTF8 &&
                                   !(fileBytes.Length >= 3 &&
                                     fileBytes[0] == 0xEF && fileBytes[1] == 0xBB && fileBytes[2] == 0xBF);
                }

                if (isTargetUTF8)
                {
                    return false; // 已经是目标格式，跳过
                }

                // 读取文件内容
                string content = File.ReadAllText(filePath, currentEncoding);

                // 用目标UTF-8格式写入
                File.WriteAllText(filePath, content, new UTF8Encoding(withBOM));

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"转换失败 {filePath}: {e.Message}");
                return false;
            }
        }

        static Encoding DetectEncoding(byte[] bytes)
        {
            // 检查BOM
            if (bytes.Length >= 2)
            {
                if (bytes[0] == 0xFE && bytes[1] == 0xFF)
                    return Encoding.BigEndianUnicode;
                if (bytes[0] == 0xFF && bytes[1] == 0xFE)
                    return Encoding.Unicode;
            }
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return Encoding.UTF8;
            if (bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF)
                return Encoding.UTF32;

            // 没有BOM，默认系统ANSI
            return Encoding.Default;
        }
    }
}