/****************************************************
    文件: EncodingHelper.cs
    作者: Clear
    日期: 2026/3/5
    类型: 工具脚本
    功能: 编码检测通用工具类
*****************************************************/
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace FutureEditor
{
    public static class EncodingHelper
    {
        // 常见的编码列表
        public static readonly Encoding[] CommonEncodings = new Encoding[]
        {
            Encoding.UTF8,
            GetEncodingSafe("GBK"),
            GetEncodingSafe("GB2312"),
            GetEncodingSafe("Big5"),
            GetEncodingSafe("Shift_JIS"),
            GetEncodingSafe("EUC-KR"),
            Encoding.Unicode,
            Encoding.BigEndianUnicode,
            Encoding.UTF32,
            GetEncodingSafe("windows-1252"),
            GetEncodingSafe("windows-1251"),
            GetEncodingSafe("windows-1250"),
            Encoding.Default
        };

        // 安全获取编码
        private static Encoding GetEncodingSafe(string name)
        {
            try
            {
                return Encoding.GetEncoding(name);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 检测文件编码
        /// </summary>
        /// <param name="bytes">文件字节数组</param>
        /// <param name="confidence">置信度：0-不确定，1-完全确定</param>
        /// <returns>检测到的编码</returns>
        public static Encoding DetectEncoding(byte[] bytes, out float confidence)
        {
            confidence = 0f;

            // 1. 先检查 BOM
            if (bytes.Length >= 2)
            {
                if (bytes[0] == 0xFE && bytes[1] == 0xFF)
                {
                    confidence = 1f;
                    return Encoding.BigEndianUnicode;
                }
                if (bytes[0] == 0xFF && bytes[1] == 0xFE)
                {
                    if (bytes.Length >= 4 && bytes[2] == 0x00 && bytes[3] == 0x00)
                    {
                        confidence = 1f;
                        return Encoding.UTF32;
                    }
                    confidence = 1f;
                    return Encoding.Unicode;
                }
            }
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                confidence = 1f;
                return Encoding.UTF8;
            }
            if (bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF)
            {
                confidence = 1f;
                return Encoding.UTF32;
            }

            // 2. 先尝试 UTF-8 检测（无BOM）
            try
            {
                string utf8Test = Encoding.UTF8.GetString(bytes);
                byte[] utf8Reencoded = Encoding.UTF8.GetBytes(utf8Test);

                if (utf8Reencoded.Length == bytes.Length)
                {
                    bool match = true;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        if (utf8Reencoded[i] != bytes[i])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        confidence = 0.95f;
                        return Encoding.UTF8;
                    }
                }
            }
            catch { }

            // 3. 尝试 GBK（你的项目最可能的编码）
            try
            {
                Encoding gbk = Encoding.GetEncoding("GBK");
                string gbkTest = gbk.GetString(bytes);
                byte[] gbkReencoded = gbk.GetBytes(gbkTest);

                if (gbkReencoded.Length == bytes.Length)
                {
                    bool match = true;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        if (gbkReencoded[i] != bytes[i])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        confidence = 0.9f;
                        return gbk;
                    }
                }
            }
            catch { }

            // 4. 尝试 GB2312
            try
            {
                Encoding gb2312 = Encoding.GetEncoding("GB2312");
                string test = gb2312.GetString(bytes);
                byte[] reencoded = gb2312.GetBytes(test);

                if (reencoded.Length == bytes.Length)
                {
                    bool match = true;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        if (reencoded[i] != bytes[i])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        confidence = 0.85f;
                        return gb2312;
                    }
                }
            }
            catch { }

            // 5. 如果文件包含中文字符特征，默认用 GBK
            bool hasChinese = false;
            for (int i = 0; i < Mathf.Min(1000, bytes.Length); i++)
            {
                if (bytes[i] > 0x80)
                {
                    hasChinese = true;
                    break;
                }
            }

            if (hasChinese)
            {
                try
                {
                    confidence = 0.7f;
                    return Encoding.GetEncoding("GBK");
                }
                catch { }
            }

            // 6. 最后返回系统默认（但给个低置信度）
            confidence = 0.5f;
            return Encoding.Default;
        }
        /// <summary>
        /// 简化版检测，不返回置信度
        /// </summary>
        public static Encoding DetectEncoding(byte[] bytes)
        {
            return DetectEncoding(bytes, out _);
        }

        private static float CalculateMatchRate(byte[] bytes, Encoding encoding)
        {
            try
            {
                string decoded = encoding.GetString(bytes);
                byte[] reencoded = encoding.GetBytes(decoded);

                int matchCount = 0;
                int minLength = Mathf.Min(bytes.Length, reencoded.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (bytes[i] == reencoded[i])
                        matchCount++;
                }

                return (float)matchCount / bytes.Length;
            }
            catch
            {
                return 0f;
            }
        }

        /// <summary>
        /// 检查是否有 BOM
        /// </summary>
        public static bool HasBOM(byte[] bytes)
        {
            return (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF) ||
                   (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE) ||
                   (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF) ||
                   (bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF);
        }

        /// <summary>
        /// 检查是否为 UTF-8 无 BOM
        /// </summary>
        public static bool IsUTF8WithoutBOM(byte[] bytes)
        {
            DetectEncoding(bytes, out float confidence);
            return confidence > 0.9f && !HasBOM(bytes);
        }

        /// <summary>
        /// 获取编码显示名称
        /// </summary>
        public static string GetEncodingDisplayName(Encoding encoding, byte[] bytes)
        {
            if (encoding == Encoding.UTF8 && HasBOM(bytes))
                return "UTF-8 带BOM";
            if (encoding == Encoding.UTF8 && !HasBOM(bytes))
                return "UTF-8 无BOM";
            if (encoding == Encoding.Default)
                return "系统默认编码 (不确定)";
            return encoding.EncodingName;
        }

        private class EncodingMatch
        {
            public Encoding Encoding { get; private set; }
            public float Confidence { get; private set; }

            public EncodingMatch(Encoding encoding, float confidence)
            {
                Encoding = encoding;
                Confidence = confidence;
            }
        }
    }
}