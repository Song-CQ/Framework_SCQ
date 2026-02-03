using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common;
using UnityEngine;

namespace FutureCore
{
    public static class TimerUtil
    {
        private static string name = "TimerUtil";
        
        private static SimpleTimer _SimpleTimer;
        private static PETimer _PETimer;     

        public static SimpleTimer Simple
        {
            get
            {
                if (_SimpleTimer == null)
                {
                    _SimpleTimer = TimerMgr.Instance.SimpleTimer;
                }
                return _SimpleTimer;
            }
        }
        public static PETimer Timer
        {
            get
            {
                if (_PETimer == null)
                {
                    _PETimer = TimerMgr.Instance.Timer;
                }
                return _PETimer;
            }
        }

        public static string GetLacalTimeYMD_HHMMSS()
        {
            return string.Concat(DateTime.Now.Year, "/", DateTime.Now.Month, "/",
                                    DateTime.Now.Day, " ", DateTime.Now.Hour, ":", DateTime.Now.Minute, ":", DateTime.Now.Second);
        }

        public static void AddDelayTask(Action task,float delay)
        {
            Simple.AddDelayTask(task,delay);
        }
        public static int AddTimeTask(Action<int> task,double delay, int _count = 1,TimeType timeType = TimeType.Second)
        {
            return Timer.AddTimeTask(task,delay,_count,timeType);
        }

        public static float GetGameTime()
        {
            return UnityEngine.Time.time;
        }
    }
}