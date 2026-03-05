using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class AutoConvertNewScriptsToUTF8 : AssetPostprocessor
{
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
                ConvertToUTF8WithoutBOM(assetPath);
            }
        }
    }

    static void ConvertToUTF8WithoutBOM(string assetPath)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);

        try
        {
            // 读取原文件内容（保持原编码）
            string content = File.ReadAllText(fullPath);

            // 用 UTF-8 无 BOM 写回
            File.WriteAllText(fullPath, content, new UTF8Encoding(false));

            Debug.Log($"自动转换脚本编码: {assetPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"转换失败 {assetPath}: {e.Message}");
        }
    }
}