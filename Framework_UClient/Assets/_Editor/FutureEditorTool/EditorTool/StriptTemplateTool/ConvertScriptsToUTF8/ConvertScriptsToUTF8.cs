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
            int count = ConvertScripts(false);
            Debug.Log($"转换完成：{count} 个文件转为 UTF-8 无BOM");
        }

        // [MenuItem("Tools/转换所有C#脚本为UTF-8（带BOM）")]
        public static void ConvertAllToUTF8WithBOM()
        {
            int count = ConvertScripts(true);
            Debug.Log($"转换完成：{count} 个文件转为 UTF-8 带BOM");
        }

        static int ConvertScripts(bool withBOM)
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);

            // 获取所有 .cs 文件（排除临时文件夹）
            string[] csFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains("\\Library\\")
                            && !path.Contains("\\Temp\\")
                            && !path.Contains("\\obj\\")
                            && !path.Contains("\\PackageCache\\"))
                .ToArray();

            int convertedCount = 0;

            foreach (string filePath in csFiles)
            {
                if (ConvertFileToUTF8(filePath, withBOM))
                {
                    convertedCount++;
                }
            }

            AssetDatabase.Refresh();
            return convertedCount;
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

                Debug.Log($"转换: {GetRelativePath(filePath)}");
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

        static string GetRelativePath(string fullPath)
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);
            return fullPath.Replace(projectPath + "\\", "");
        }
    }
}