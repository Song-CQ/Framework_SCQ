using System;

namespace FutureCore
{
    public sealed class TimerMgr : BaseMonoMgr<TimerMgr>
    {
        private DateTime startDataTime = new DateTime(1970, 1, 1, 0, 0, 0);//计算机元年

        public SimpleTimer SimpleTimer { private set; get; }
        public PETimer Timer { private set; get; }
        /// <summary>
        /// 每固定帧执行的事件
        /// </summary>
        public static event Action FixedUpdate_Event_ToFrame;
        /// <summary>
        /// 每帧执行的事件
        /// </summary>
        public static event Action UpData_Event_ToFrame;
        /// <summary>
        /// 每秒执行的事件
        /// </summary>
        public static event Action UpData_Event_ToSecond;
        private float timeTemp_Second = 0;


        private void InitTimeRood()
        {
            CreatePeTimer();
            CreateSimpleTimer();
        }
        private void CreatePeTimer()
        {
            Timer = new PETimer();
            Timer.SetLog(LogUtil.Log);
        }
        private void CreateSimpleTimer()
        {
            SimpleTimer = new SimpleTimer();

        }


        public override void Init()
        {
            base.Init();
            InitTimeRood();

        }

        private void Update()
        {
            if (!IsInit)
            {
                return;
            }
            SimpleTimer.Update();
            Timer.Update();

            UpData_Event_ToFrame?.Invoke();
            timeTemp_Second += UnityEngine.Time.deltaTime;
            if (timeTemp_Second >= 1)
            {
                timeTemp_Second = 0;
                UpData_Event_ToSecond?.Invoke();
            }

        }

        private void FixedUpdate()
        {
            if (!IsInit)
            {
                return;
            }
            FixedUpdate_Event_ToFrame?.Invoke();
        }

        public override void Dispose()
        {
            SimpleTimer.Dispose();
            Timer.Dispose();
        }

        #region api

        /// <summary>
        /// 获取当前年
        /// </summary>
        /// <returns></returns>
        public int GetYear()
        {
            return GetLocalDateTime().Year;
        }

        /// <summary>
        /// 获取当前月
        /// </summary>
        /// <returns></returns>
        public int GetMonth()
        {
            return GetLocalDateTime().Month;
        }

        /// <summary>
        /// 获取当前天
        /// </summary>
        /// <returns></returns>
        public int GetDay()
        {
            return GetLocalDateTime().Day;
        }

        /// <summary>
        /// 获取当前星期
        /// </summary>
        /// <returns></returns>
        public int GetWeek()
        {
            return (int)GetLocalDateTime().DayOfWeek;
        }

        /// <summary>
        /// 获取当前时间（DataTime）
        /// </summary>
        /// <returns></returns>
        public DateTime GetLocalDateTime()
        {
            DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(startDataTime.AddMilliseconds(GetUCTMilliseconds()));
            return dt;
        }

        /// <summary>
        /// 获取当前时间（毫秒）
        /// </summary>
        /// <returns></returns>
        public double GetMillisecondsTime()
        {
            return GetUCTMilliseconds();
        }

        /// <summary>
        /// 获取当前时间（字符）
        /// </summary>
        /// <returns></returns>
        public string GetLocalTimeStr()
        {
            DateTime dt = GetLocalDateTime();
            string str = GetTimeStr(dt.Hour) + ":" + GetTimeStr(dt.Minute) + ":" + GetTimeStr(dt.Second);
            return str;
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        private double GetUCTMilliseconds()
        {
            TimeSpan span = DateTime.UtcNow - startDataTime;
            return span.TotalMilliseconds;
        }

        private string GetTimeStr(int time)
        {
            if (time < 10)
            {
                return "0" + time;
            }
            else
            {
                return time.ToString();
            }
        }

        #endregion
    }
}