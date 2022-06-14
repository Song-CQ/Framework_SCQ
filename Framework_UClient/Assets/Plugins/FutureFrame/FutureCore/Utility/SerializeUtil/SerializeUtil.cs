using System;
using Newtonsoft.Json;

namespace FutureCore
{
    /// <summary>
    /// ��Newtonsoft.Json��װ
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
            // ������
            Formatting = Formatting.None,
            // ��������Ĭ�ϸ�ʽ������  
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            // ��������Ĭ�ϸ�ʽ������ 
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            // ����Ĭ��ֵ
            //DefaultValueHandling = DefaultValueHandling.Ignore,
            // ���Կ�ֵ
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

        public static string UnityToJson(object obj)
        {
            return UnityEngine.JsonUtility.ToJson(obj);
        }

        public static object UnityToObject(string json)
        {
            return UnityEngine.JsonUtility.FromJson<object>(json);
        }

    }
}

