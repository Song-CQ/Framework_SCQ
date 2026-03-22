using System;
using System.Text;
using Newtonsoft.Json;

namespace FutureCore
{
    /// <summary>
    /// 对 Newtonsoft.Json 的封装
    /// </summary>
    public class SerializeUtil
    {
        static SerializeUtil()
        {
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                return JsonSettings;
            });
        }

        private static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            // 格式化
            Formatting = Formatting.None,
            // 使用微软默认日期格式
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            // 日期格式字符串
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };

        /// <summary>
        /// 清理 JSON 字符串中的 BOM 和其他特殊字符
        /// </summary>
        private static string CleanJsonString(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            // 移除 UTF-8 BOM
            json = json.TrimStart('\uFEFF');
            // 移除零宽空格等不可见字符
            json = json.TrimStart('\u200B');

            return json;
        }

        /// <summary>
        /// 序列化对象为 JSON 字符串
        /// </summary>
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 序列化对象为 JSON 字符串（指定类型）
        /// </summary>
        public static string ToJson(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, JsonSettings);
        }

        /// <summary>
        /// 序列化对象为 JSON 字符串（泛型）
        /// </summary>
        public static string ToJson<T>(object obj)
        {
            return ToJson(obj, typeof(T));
        }

        /// <summary>
        /// 反序列化 JSON 字符串为对象（带 BOM 清理）
        /// </summary>
        public static T ToObject<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);

            try
            {
                // 清理 BOM 字符
                json = CleanJsonString(json);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"反序列化失败: {ex.Message}\nJSON: {json}");
                return default(T);
            }
        }

        /// <summary>
        /// 反序列化 JSON 字符串为对象（指定类型，带 BOM 清理）
        /// </summary>
        public static object ToObject(string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                json = CleanJsonString(json);
                return JsonConvert.DeserializeObject(json, type);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"反序列化失败: {ex.Message}\nJSON: {json}");
                return null;
            }
        }

        /// <summary>
        /// 从文件加载并反序列化 JSON（自动处理编码）
        /// </summary>
        public static T LoadFromFile<T>(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                UnityEngine.Debug.LogError($"文件不存在: {filePath}");
                return default(T);
            }

            try
            {
                // 使用 StreamReader 自动处理 BOM
                using (var reader = new System.IO.StreamReader(filePath, Encoding.UTF8))
                {
                    string json = reader.ReadToEnd();
                    return ToObject<T>(json);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"加载文件失败: {filePath}\n{ex.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// 保存对象到文件（UTF-8 without BOM）
        /// </summary>
        public static void SaveToFile(string filePath, object obj)
        {
            try
            {
                string json = ToJson(obj);
                string directory = System.IO.Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                // UTF-8 without BOM
                Encoding utf8WithoutBom = new UTF8Encoding(false);
                System.IO.File.WriteAllText(filePath, json, utf8WithoutBom);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"保存文件失败: {filePath}\n{ex.Message}");
            }
        }

        /// <summary>
        /// Unity JsonUtility 序列化（带 BOM 清理）
        /// </summary>
        public static string UnityToJson(object obj,bool prettyPrint = false)
        {
            return UnityEngine.JsonUtility.ToJson(obj,prettyPrint);
        }

        /// <summary>
        /// Unity JsonUtility 反序列化（带 BOM 清理）
        /// </summary>
        public static T UnityToObject<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
                return null;

            json = CleanJsonString(json);
            return UnityEngine.JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// Unity JsonUtility 反序列化（非泛型）
        /// </summary>
        public static object UnityToObject(string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            json = CleanJsonString(json);
            return UnityEngine.JsonUtility.FromJson(json, type);
        }

        /// <summary>
        /// 验证 JSON 字符串是否有效
        /// </summary>
        public static bool IsValidJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;

            json = CleanJsonString(json);

            try
            {
                var obj = JsonConvert.DeserializeObject(json);
                return obj != null;
            }
            catch
            {
                return false;
            }
        }
    }
}