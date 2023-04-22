using UnityEngine;

namespace FutureCore
{
    public static class PrefsUtil
    {
        public static void WriteInt(string key, int data)
        {
            PlayerPrefs.SetInt(key, data);
            LogUtil.LogFormat("[PrefsUtil WriteInt]Key: {0}\n{1}", key, data);
        }

        public static int ReadInt(string key, int defaultValue = 0)
        {
            int data = PlayerPrefs.GetInt(key, defaultValue);
            LogUtil.LogFormat("[PrefsUtil ReadInt]Key: {0}\n{1}", key, data);
            return data;
        }

        public static void WriteBool(string key, bool data)
        {
            int val = data ? 1 : 0;
            PlayerPrefs.SetInt(key, val);
            LogUtil.LogFormat("[PrefsUtil WriteBool]Key: {0}\n{1}", key, data);
        }

        public static bool ReadBool(string key, bool defaultValue = false)
        {
            int val = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
            bool data = val == 1 ? true : false;
            LogUtil.LogFormat("[PrefsUtil ReadBool]Key: {0}\n{1}", key, data);
            return data;
        }

        public static void WriteString(string key, string data)
        {
            PlayerPrefs.SetString(key, data);
            LogUtil.LogFormat("[PrefsUtil WriteString]Key: {0}\n{1}", key, data);
        }

        public static string ReadString(string key, string defaultValue = "")
        {
            string data = PlayerPrefs.GetString(key, defaultValue);
            LogUtil.LogFormat("[PrefsUtil ReadString]Key: {0}\n{1}", key, data);
            return data;
        }

        public static void WriteFloat(string key, float data)
        {
            PlayerPrefs.SetFloat(key, data);
            LogUtil.LogFormat("[PrefsUtil WriteFloat]Key: {0}\n{1}", key, data);
        }

        public static float ReadFloat(string key, float defaultValue = 0)
        {
            float data = PlayerPrefs.GetFloat(key, defaultValue);
            LogUtil.LogFormat("[PrefsUtil ReadFloat]Key: {0}\n{1}", key, data);
            return data;
        }

        public static void WriteObject(string key, object data)
        {
            string dataStr = SerializeUtil.ToJson(data);
            PlayerPrefs.SetString(key, dataStr);
            LogUtil.LogFormat("[PrefsUtil WriteObject]Key: {0}\n{1}", key, dataStr);
        }

        public static T ReadObject<T>(string key)
        {
            if (!HasKey(key))
            {
                LogUtil.LogErrorFormat("[PrefsUtil ReadObject]Key: {0}, Not Has Key", key);
                return default;
            }

            string dataStr = PlayerPrefs.GetString(key);
            if (!dataStr.IsNullOrEmpty())
            {
                LogUtil.LogFormat("[PrefsUtil ReadObject]Key: {0}, Data Is Null", key);
                return default;
            }

            LogUtil.LogFormat("[PrefsUtil ReadObject]Key: {0}\n{1}", key, dataStr);
            T data = SerializeUtil.ToObject<T>(dataStr);
            return data;
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            if (HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }

        public static void DeleteAllKey()
        {
            PlayerPrefs.DeleteAll();
        }

    }
}

