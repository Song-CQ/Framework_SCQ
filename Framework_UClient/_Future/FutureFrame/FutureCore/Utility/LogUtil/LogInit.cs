using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace FutureCore
{
#if UNITY_EDITOR
    using UnityEditor;
    [InitializeOnLoad]
#endif
    public static class LogInit
    {

        static LogInit()
        {
            InitLog();
        }

        public static void InitLog()
        {
            LogUtil.SetLogCallBack_Log(Debug.Log, Debug.LogFormat);
            LogUtil.SetLogCallBack_LogError(Debug.LogError, Debug.LogErrorFormat);
            LogUtil.SetLogCallBack_LogWarning(Debug.LogWarning, Debug.LogWarningFormat);
            LogUtil.EnableLog(AppConst.IsEnabledLog);

        }
    }
}
