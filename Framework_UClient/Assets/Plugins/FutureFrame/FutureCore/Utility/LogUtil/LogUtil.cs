
using UnityEngine;

public static class LogUtil
{

    public static bool IsEnable { private set; get; } = true;

    public static void Log(object e)
    {
       
        Debug.Log(e);
    } 
    public static void Log(object e, Object context)
    {
        
        Debug.Log(e,context);
    } 
    public static void LogError(object message, Object context)
    {
       
        Debug.LogError(message,context);
    }
    public static void LogError(object e)
    {
        
        Debug.LogError(e);
    }
    public static void LogWarning(object message, Object context)
    {
        
        Debug.LogWarning(message,context);
    }
    public static void LogWarning(object e)
    {
        
        Debug.LogWarning(e);
    }
    public static void EnableLog(bool isEnabledDebugLog)
    {
        IsEnable = isEnabledDebugLog;
        
        Debug.unityLogger.logEnabled = IsEnable;
    }
}
