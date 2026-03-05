/****************************************************
    文件: AutoConvertNewScriptsToUTF8.cs
    作者: Clear
    日期: 2026/3/5
    类型: 工具脚本
    功能: 自动将新导入的C#脚本转换为UTF-8无BOM格式
*****************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace FutureEditor
{
    public class AutoConvertNewScriptsToUTF8 : AssetPostprocessor
    {
        // 缓存不确定编码的文件，避免重复弹窗
        private static Dictionary<string, bool> _pendingFiles = new Dictionary<string, bool>();

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                // 只处理 .cs 文件
                if (assetPath.EndsWith(".cs"))
                {
                    // 延迟一帧处理，避免在导入过程中弹窗
                    EditorApplication.delayCall += () => CheckAndConvertFile(assetPath);
                }
            }
        }

        static void CheckAndConvertFile(string assetPath)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);

            try
            {
                // 1. 先读取文件字节
                byte[] fileBytes = File.ReadAllBytes(fullPath);

                // 2. 检测文件编码
                Encoding detectedEncoding = EncodingHelper.DetectEncoding(fileBytes, out float confidence);
                bool hasBOM = EncodingHelper.HasBOM(fileBytes);

                // 3. 如果已经是 UTF-8 无 BOM，跳过
                if (detectedEncoding == Encoding.UTF8 && !hasBOM)
                {
                    return;
                }

                // 4. 如果是 UTF-8 带 BOM，直接转成无 BOM（安全）
                if (detectedEncoding == Encoding.UTF8 && hasBOM)
                {
                    string content = Encoding.UTF8.GetString(fileBytes);
                    File.WriteAllText(fullPath, content, new UTF8Encoding(false));
                    Debug.Log($"自动移除 BOM: {assetPath}");
                    return;
                }

                // 5. 如果检测到明确的非 UTF-8 编码，直接转换
                if (confidence > 0.8f && detectedEncoding != Encoding.UTF8)
                {
                    string content = detectedEncoding.GetString(fileBytes);
                    File.WriteAllText(fullPath, content, new UTF8Encoding(false));
                    Debug.Log($"自动转换脚本编码: {assetPath} (原编码: {detectedEncoding.EncodingName})");
                    return;
                }

                // 6. 如果不确定编码，弹出提示
                if (!_pendingFiles.ContainsKey(assetPath))
                {
                    _pendingFiles[assetPath] = true;
                    ShowEncodingDialog(assetPath, fullPath, fileBytes);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"处理文件失败 {assetPath}: {e.Message}");
            }
        }

        static void ShowEncodingDialog(string assetPath, string fullPath, byte[] fileBytes)
        {
            // 询问用户
            int option = EditorUtility.DisplayDialogComplex(
                "无法确定文件编码",
                $"文件: {assetPath}\n\n无法自动检测该文件的编码格式。\n\n请选择操作：",
                "用 GBK 读取并转换",
                "跳过此文件",
                "用 UTF-8 读取并转换（不推荐）"
            );

            try
            {
                switch (option)
                {
                    case 0: // 用 GBK
                        string gbkContent = Encoding.GetEncoding("GBK").GetString(fileBytes);
                        File.WriteAllText(fullPath, gbkContent, new UTF8Encoding(false));
                        Debug.Log($"手动选择 GBK 转换: {assetPath}");
                        break;

                    case 1: // 跳过
                        Debug.Log($"跳过文件: {assetPath}");
                        break;

                    case 2: // 用 UTF-8
                        string utf8Content = Encoding.UTF8.GetString(fileBytes);
                        File.WriteAllText(fullPath, utf8Content, new UTF8Encoding(false));
                        Debug.LogWarning($"用 UTF-8 读取并转换: {assetPath} (如果原文件不是UTF-8，可能会乱码)");
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"转换失败 {assetPath}: {e.Message}");
            }
            finally
            {
                // 从缓存中移除
                if (_pendingFiles.ContainsKey(assetPath))
                    _pendingFiles.Remove(assetPath);
            }
        }
    }
}