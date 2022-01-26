public delegate void LogCallBack(object o);
public delegate void LogCallBack2(string o, params object[] o1);
public static class LogUtil
{
    public static bool IsEnable { private set; get; } = true;

    private static LogCallBack log;
    private static LogCallBack2 logFormat;

    private static LogCallBack logError;
    private static LogCallBack2 logErrorFormat;

    private static LogCallBack logWarning;
    private static LogCallBack2 logWarningFormat;

    public static void SetLogCallBack_Log(LogCallBack _log, LogCallBack2 _logFormat)
    {
        log = _log;
        logFormat = _logFormat;

    }
    public static void SetLogCallBack_LogError(LogCallBack _logError, LogCallBack2 _logErrorFormat)
    {
        logError = _logError;
        logErrorFormat = _logErrorFormat;
    }
    public static void SetLogCallBack_LogWarning(LogCallBack _logWarning, LogCallBack2 _logWarningFormat)
    {
        logWarning = _logWarning;
        logWarningFormat = _logWarningFormat;
    }

    public static void LogGirl()
    {
        Log("*********************************************");
        Log("        く__,.ヘヽ.        /  ,ー､ 〉        ");
        Log("           ＼ ', !-─‐-i  /  /´             ");
        Log("           ／｀ｰ'       L/／｀ヽ､            ");
        Log("         /   ／,   /|   ,   ,       ',       ");
        Log("       ｲ / / -‐/ ｉ L_ ﾊ ヽ!   i            ");
        Log("     ﾚ ﾍ 7ｲ｀ﾄ ﾚ'ｧ-ﾄ､!ハ|   |                ");
        Log("        !,/ 7 '0'     ´0iソ |    |           ");
        Log("         |.从*    _     ,,,, / |./    |      ");
        Log("          ﾚ'| i＞.､,,__  _,.イ /   .i   |    ");
        Log("            ﾚ'| | / k_７_/ﾚ'ヽ,  ﾊ.  |       ");
        Log("              | |/ i 〈|/ i  ,.ﾍ | i |       ");
        Log("            .|/ / ｉ：    ﾍ!    ＼  |        ");
        Log("            kヽ & gt;､ﾊ _,.ﾍ､    /､!         ");
        Log("              !'〈//｀Ｔ´', ＼ ｀'7'ｰr'      ");
        Log("              ﾚ'ヽL__|___i,___,ンﾚ|ノ        ");
        Log("                  ﾄ -,/  | ___./             ");
        Log("                   'ｰ'!_,.:                  ");
        Log("*********************************************");
    }

    public static void Log(object e)
    {
        if (!IsEnable)
        {
            return;
        }
        log?.Invoke(e);
    }

    public static void LogFormat(string v, params object[] data)
    {
        if (!IsEnable)
        {
            return;
        }
        logFormat?.Invoke(v, data);
    }

    public static void LogErrorFormat(string v, params object[] data)
    {
        if (!IsEnable)
        {
            return;
        }
        logErrorFormat?.Invoke(v, data);
    }

    public static void LogError(object e)
    {
        if (!IsEnable)
        {
            return;
        }
        logError?.Invoke(e);
    }
    public static void LogWarning(object e)
    {
        if (!IsEnable)
        {
            return;
        }
        logWarning?.Invoke(e);
    }
    public static void LogWarningFormat(string v, params object[] data)
    {
        if (!IsEnable)
        {
            return;
        }
        logWarningFormat?.Invoke(v, data);
    }
    public static void EnableLog(bool isEnabledDebugLog)
    {
        IsEnable = isEnabledDebugLog;
    }

}
