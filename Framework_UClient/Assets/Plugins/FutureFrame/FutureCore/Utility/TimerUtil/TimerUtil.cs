using System.Collections;
using System.Collections.Generic;
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
    }
}