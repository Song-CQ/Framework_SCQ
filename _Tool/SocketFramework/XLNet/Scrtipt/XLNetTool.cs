using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLNet
{
    public static class XLNetTool
    {

        private static Action<string> logCB;
        private static Action<string> LogWarnCB;
        private static Action<string> LogErrorCB;

        public static void SetLog(Action<string> _logCB, Action<string> _LogWarnCB, Action<string> _LogErrorCB)
        {
            logCB = _logCB;
            LogWarnCB = _LogWarnCB;
            LogErrorCB = _LogErrorCB;
        }

        public static void Log(string msg)
        {
            msg = DateTime.Now.ToLongTimeString() + " >> " + msg;
            if (logCB != null)
            {
                logCB(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }
        public static void LogWarn(string msg)
        {
            msg = DateTime.Now.ToLongTimeString() + " >> " + msg;
            if (LogWarnCB != null)
            {
                LogWarnCB(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }
        public static void LogError(string msg)
        {
            msg = DateTime.Now.ToLongTimeString() + " >> " + msg;
            if (LogErrorCB != null)
            {
                LogErrorCB(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

    }
}
