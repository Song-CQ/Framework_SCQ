using System;
using Newtonsoft.Json;

namespace FutureCore
{
    /// <summary>
    /// 对Newtonsoft.Json包装
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
            // 不缩进
            Formatting = Formatting.None,
            // 日期类型默认格式化处理  
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            // 日期类型默认格式化处理 
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            // 忽略默认值
            //DefaultValueHandling = DefaultValueHandling.Ignore,
            // 忽略空值
            //NullValueHandling = NullValueHandling.Ignore,
        };

        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string ToJson(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, JsonSettings);
        }
        public static string ToJson<T>(object obj)
        {
            return ToJson(obj, typeof(T));
        }

        public static T ToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object ToObject(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        //public static string UnityToJson(object obj)
        //{
        //    return UnityEngine.JsonUtility.ToJson(obj);
        //}

        //public static object UnityToObject(string json)
        //{
        //    return UnityEngine.JsonUtility.FromJson<object>(json);
        //}

    }
}

