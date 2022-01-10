
using UnityEngine;

public static class LogUtil
{
    public static bool IsEnable { private set; get; } = true;

    public static void Log(object e)
    {
        if (!IsEnable)
        {
            return;
        }
        Debug.Log(e);
    } 
    public static void Log(object e, Object context)
    {
        if (!IsEnable)
        {
            return;
        }
        Debug.Log(e,context);
    }

    public static void LogFormat(string v, params object[] data)
    {
        if (!IsEnable)
        {
            return;
        }
        Debug.LogFormat(v,data);
    } 
    
    public static void LogErrorFormat(string v, params object[] data)
    {
        if (!IsEnable)
        {
            return;
        }
        Debug.LogErrorFormat(v,data);
    }

    public static void LogError(object message, Object context)
    {
        if (!IsEnable)
        {
            return;
        }
        Debug.LogError(message,context);
    }
    public static void LogError(object e)
    {
        if (!IsEnable)
        {
            return;
        }
        Debug.LogError(e);
    }
    public static void LogWarning(object message, Object context)
    {
        if (!IsEnable)
        {
            return;
        }
        Debug.LogWarning(message,context);
    }
    public static void LogWarning(object e)
    {
        if (!IsEnable)
        {
            return;
        }
        Debug.LogWarning(e);
    }
    public static void EnableLog(bool isEnabledDebugLog)
    {
        IsEnable = isEnabledDebugLog;
    }

}
