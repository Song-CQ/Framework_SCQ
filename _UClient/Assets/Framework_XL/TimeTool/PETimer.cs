/****************************************************
    文件：PETimer.cs
    作者：相柳
    邮箱: 2728285639@qq.com
    日期：2020.1.6.11:18
    功能：计时器
*****************************************************/

using System;
using System.Collections.Generic;
using System.Timers;

public class PETimer
{           
    private DateTime startDataTime = new DateTime(1970,1,1,0,0,0);//计算机元年
    private Timer srvTimer;//提供给服务器的回调
    private Action<string> LogTask;//提供给外界设置日志委托
    private Action<Action<int>, int> taskHandle;//提供给外界调用委托

    private int tid;//计时任务全局ID
    private static readonly string lockTid = "LockTid";
    private List<int> TidLst = new List<int>();//计时任务全局ID集合
    private List<int> RecTidLst = new List<int>();//要移除计时任务全局ID集合

    private static readonly string lockTime = "LockTime";
    private List<TimeTask> tmpTimeTasksLst = new List<TimeTask>();//时间计时任务缓存集合
    //缓存列表的目的在于多线程的数据安全
    private List<TimeTask> TimeTasksLst = new List<TimeTask>();//时间计时任务集合
    private List<int> tmpDelTimeLst = new List<int>();//要删除的时间计时任务集合


    private int frameCounter;
    private static readonly string lockFrame = "LockFrame";
    private List<FrameTask> tmpFrameTasksLst = new List<FrameTask>();//帧数计时任务缓存集合
    private List<FrameTask> FrameTasksLst = new List<FrameTask>();//帧数计时任务集合
    private List<int> tmpDelFrameLst = new List<int>();//删除帧数计时任务集合

    /// <summary>
    /// 创建一个计时器
    /// </summary>
    /// <param name="interval">计时器多少毫秒更新一次，默认为不更新</param>
    public PETimer(int interval=0)
    {
        TidLst.Clear();
        RecTidLst.Clear();

        tmpTimeTasksLst.Clear();
        TimeTasksLst.Clear();

        tmpFrameTasksLst.Clear();
        FrameTasksLst.Clear();

        if (interval != 0)
        {
            srvTimer = new Timer(interval) {
                AutoReset = true
            };
            srvTimer.Elapsed += (object sender, ElapsedEventArgs e)=> {
                Update();
            };
            srvTimer.Start();
        }
    }
    /// <summary>
    /// 重置函数
    /// </summary>
    public void Reset()
    {
        tid = 0;
        TidLst.Clear();
        RecTidLst.Clear();

        tmpTimeTasksLst.Clear();
        TimeTasksLst.Clear();

        tmpFrameTasksLst.Clear();
        FrameTasksLst.Clear();

        LogTask = null;
        srvTimer.Stop();
    }  

    #region 时间计时任务模块
   
    /// <summary>
    /// 删除计时任务(时间)
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public void DeleteTimeTask(int tid)
    {
        lock (lockTime)
        {
            //添加进删除列表
            tmpDelTimeLst.Add(tid);
            //LogInfo("TmpDel ID:" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
        }
       
    }

    /// <summary>
    /// 替换计时任务(时间)
    /// </summary>
    /// <param name="tid">要替换的计时任务的ID</param>
    /// <param name="_cb">回调方法</param>
    /// <param name="_delsy">回调间隔</param>
    /// <param name="_count">回调次数，默认为一次</param>
    /// <param name="timeUnity">回调间隔的时间单位，默认为毫秒</param>
    /// <returns> 是否成功替换</returns>
    public bool ReplaceTimeTask(int tid, Action<int> _cb, double _delsy, int _count = 1, PETimeUnity timeUnity = PETimeUnity.Millisecond)
    {
        bool isRep = false;
        switch (timeUnity)//转换单位为毫秒
        {
            case PETimeUnity.Second:
                _delsy = _delsy * 1000;
                break;
            case PETimeUnity.Minute:
                _delsy = _delsy * 1000 * 60;
                break;
            case PETimeUnity.Hour:
                _delsy = _delsy * 1000 * 60 * 60;
                break;
            case PETimeUnity.Day:
                _delsy = _delsy * 1000 * 60 * 60 * 24;
                break;

        }

        double destTime = GetUCTMilliseconds() + _delsy;

        TimeTask RepItem = new TimeTask(tid, _cb, destTime, _delsy, _count);


        for (int i = 0; i < TimeTasksLst.Count; i++)
        {
            TimeTask item = TimeTasksLst[i];
            if (item.ID == tid)
            {
                TimeTasksLst[i] = RepItem;
                isRep = true;
                break;
            }
        }

        if (!isRep)
        {
            for (int i = 0; i < tmpTimeTasksLst.Count; i++)
            {
                TimeTask item = tmpTimeTasksLst[i];
                if (item.ID == tid)
                {
                    TimeTasksLst[i] = RepItem;
                    isRep = true;
                    break;
                }
            }

        }
        return isRep;
    }

    /// <summary>
    /// 添加计时任务(时间),返回该计时任务的全局ID
    /// </summary>
    /// <param name="_cb">回调方法</param>
    /// <param name="_delsy">回调间隔</param>
    /// <param name="_count">回调次数，默认为一次</param>
    /// <param name="timeUnity">回调间隔的时间单位，默认为毫秒</param>
    /// <returns></returns>
    public int AddTimeTask(Action<int> _cb, double _delsy, int _count = 1, PETimeUnity timeUnity = PETimeUnity.Millisecond)
    {
        switch (timeUnity)//转换单位为毫秒
        {
            case PETimeUnity.Second:
                _delsy = _delsy * 1000;
                break;
            case PETimeUnity.Minute:
                _delsy = _delsy * 1000 * 60;
                break;
            case PETimeUnity.Hour:
                _delsy = _delsy * 1000 * 60 * 60;
                break;
            case PETimeUnity.Day:
                _delsy = _delsy * 1000 * 60 * 60 * 24;
                break;

        }

        double destTime = GetUCTMilliseconds() + _delsy;

        int _tid = GetTid();
        lock (lockTime)
        {
         tmpTimeTasksLst.Add(new TimeTask(_tid, _cb, destTime, _delsy, _count));
        }      
        return _tid;
    }

    #endregion

    #region 帧计时任务模块

    /// <summary>
    /// 删除计时任务(帧数)
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public void DeleteFrameTask(int tid)
    {
        lock (lockFrame)
        {
            tmpDelFrameLst.Add(tid);
        }
    }

    /// <summary>
    /// 替换计时任务(帧数)
    /// </summary>
    /// <param name="tid">要替换的计时任务的ID</param>
    /// <param name="_cb">回调方法</param>
    /// <param name="_delsy">回调间隔</param>
    /// <returns></returns>
    public bool ReplaceFrameTask(int tid, Action<int> _cb, int _delsy, int _count = 1)
    {
        bool isRep = false;
        FrameTask RepItem = new FrameTask(tid, _cb, frameCounter + _delsy, _delsy, _count);
        for (int i = 0; i < FrameTasksLst.Count; i++)
        {
            FrameTask item = FrameTasksLst[i];
            if (item.ID == tid)
            {
                FrameTasksLst[i] = RepItem;
                isRep = true;
                break;
            }
        }

        if (!isRep)
        {
            for (int i = 0; i < tmpFrameTasksLst.Count; i++)
            {
                FrameTask item = tmpFrameTasksLst[i];
                if (item.ID == tid)
                {
                    FrameTasksLst[i] = RepItem;
                    isRep = true;
                    break;
                }
            }

        }
        return isRep;
    }

    /// <summary>
    /// 添加计时任务(帧数),返回该计时任务的全局ID
    /// </summary>
    /// <param name="_cb">回调方法</param>
    /// <param name="_delsy">回调帧数间隔</param>
    /// <param name="_count">回调次数，默认为一次</param>
    /// <returns></returns>
    public int AddFrameTask(Action<int> _cb, int _delsy, int _count = 1)
    {
        int _tid = GetTid();
        lock (lockFrame)
        {
         tmpFrameTasksLst.Add(new FrameTask(_tid, _cb, frameCounter + _delsy, _delsy, _count));
        }
        return _tid;
    }

    #endregion

    #region 更新模块
    /// <summary>
    /// 更新，执行函数
    /// </summary>
    public void Update()
    {
        //对时间计时任务进行检查，执行
        CheckTimeTask();
        //对帧数计时任务进行检查，执行
        CheckFrameTask();
        //删除已经完成
        DelTimeTask();
        DelFrameTask();
        //移除已完成的任务的全局ID
        if (RecTidLst.Count != 0)
        {
            RecycleTid();
        }

    }
    /// <summary>
    /// 检查删除列表
    /// </summary>
    private void DelTimeTask()
    {
        if (tmpDelTimeLst.Count > 0)
        {
            lock (lockTime)
            {
                for (int i = 0; i < tmpDelTimeLst.Count; i++)
                {
                    bool isDel = false;
                    int delTid = tmpDelTimeLst[i];
                    for (int j = 0; j < TimeTasksLst.Count; j++)
                    {
                        TimeTask task = TimeTasksLst[j];
                        if (task.ID == delTid)
                        {
                            isDel = true;
                            TimeTasksLst.RemoveAt(j);
                            RecTidLst.Add(delTid);
                            //LogInfo("Del taskTimeLst ID:" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                            break;
                        }
                    }

                    if (isDel)
                        continue;

                    for (int j = 0; j < tmpTimeTasksLst.Count; j++)
                    {
                        TimeTask task = tmpTimeTasksLst[j];
                        if (task.ID == delTid)
                        {
                            tmpTimeTasksLst.RemoveAt(j);
                            RecTidLst.Add(delTid);
                            //LogInfo("Del tmpTimeLst ID:" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                            break;
                        }
                    }
                }
                tmpDelTimeLst.Clear();
            }
        }
    }
    /// <summary>
    /// 检查删除列表
    /// </summary>
    private void DelFrameTask()
    {
        if (tmpDelFrameLst.Count > 0)
        {
            lock (lockFrame)
            {
                for (int i = 0; i < tmpDelFrameLst.Count; i++)
                {
                    bool isDel = false;
                    int delTid = tmpDelFrameLst[i];
                    for (int j = 0; j < FrameTasksLst.Count; j++)
                    {
                        FrameTask task = FrameTasksLst[j];
                        if (task.ID == delTid)
                        {
                            isDel = true;
                            FrameTasksLst.RemoveAt(j);
                            RecTidLst.Add(delTid);
                            break;
                        }
                    }

                    if (isDel)
                        continue;

                    for (int j = 0; j < tmpFrameTasksLst.Count; j++)
                    {
                        FrameTask task = tmpFrameTasksLst[j];
                        if (task.ID == delTid)
                        {
                            tmpFrameTasksLst.RemoveAt(j);
                            RecTidLst.Add(delTid);
                            break;
                        }
                    }
                }
                tmpDelFrameLst.Clear();
            }
        }
    }

    /// <summary>
    /// 检查，执行时间计时任务
    /// </summary>
    private void CheckTimeTask()
    {
        if (tmpTimeTasksLst.Count > 0)
        {
            lock (lockTime)
            {
                //把缓存集合的计时任务 加入到集合
                foreach (var item in tmpTimeTasksLst)
                {
                    TimeTasksLst.Add(item);
                }
                tmpTimeTasksLst.Clear(); 
            }
        }
       
        
        //遍历计时任务
        for (int index = 0; index < TimeTasksLst.Count; index++)
        {
            TimeTask task = TimeTasksLst[index];

            if (GetUCTMilliseconds() < task.DestTime)
            {
                continue;
            }
            else//当到达时间执行计时任务
            {
                if (task.CB != null)
                {

                    try
                    {
                        if (taskHandle != null)
                        {
                            taskHandle(task.CB, task.ID);
                        }
                        else
                        {

                            task.CB(task.ID);

                        }
                    }
                    catch (Exception e)
                    {
                        LogInfo(e.ToString());
                    }
                    

                }
                if (task.Count == 1)//只执行一次就移除集合
                {
                    TimeTasksLst.RemoveAt(index);
                    lock (lockTid)
                    {
                        RecTidLst.Add(task.ID);
                    }
                    index--;
                }
                else
                {
                    if (task.Count != 0)
                    {
                        task.Count--;
                    }
                    task.DestTime += task.Delsy;
                }

            }
        }
    }
   
    /// <summary>
    /// 检查，执行帧数计时任务
    /// </summary>
    private void CheckFrameTask()
    {
        if (tmpFrameTasksLst.Count > 0)
        {
            lock (lockFrame)
            {
                //把缓存集合的计时任务 加入到集合
                foreach (var item in tmpFrameTasksLst)
                {
                    FrameTasksLst.Add(item);
                }
                tmpFrameTasksLst.Clear(); 
            }
        }
       
        //遍历计时任务
        for (int index = 0; index < FrameTasksLst.Count; index++)
        {
            FrameTask task = FrameTasksLst[index];

            if (frameCounter < task.DestFrame)
            {
                continue;
            }
            else//当到达时间执行计时任务
            {
                if (task.CB != null)
                {
                    try
                    {
                        if (taskHandle != null)
                        {
                            taskHandle(task.CB, task.ID);
                        }
                        else
                        {

                            task.CB(task.ID);

                        }
                    }
                    catch (Exception e)
                    {
                        LogInfo(e.ToString());
                    }

                }
                if (task.Count == 1)//只执行一次就移除集合
                {
                    FrameTasksLst.RemoveAt(index);
                    lock (lockTid)
                    {
                        RecTidLst.Add(task.ID);
                    }
                    index--;
                }
                else
                {
                    if (task.Count != 0)
                    {
                        task.Count--;
                    }
                    task.DestFrame += task.Delsy;
                }

            }
        }
        frameCounter++;
    }

    #endregion

    #region API
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
    #endregion

    #region Tool Methonds

    /// <summary>
    /// 获取不重复的计时任务的全局ID
    /// </summary>
    /// <returns></returns>
    private int GetTid()
    {
        lock (lockTid)
        {
            if (tid < int.MaxValue)
            {
                tid += 1;
            }
            else
            {

                tid = 0;
                while (true)
                {

                    bool used = false;
                    foreach (var item in TidLst)
                    {
                        if (item == tid)
                        {
                            used = true;
                            break;
                        }

                    }
                    if (!used)
                    {
                        break;
                    }
                    tid += 1;

                }
            }
            TidLst.Add(tid);
        }

        return tid;
    }
    /// <summary>
    /// 移除已完成计时任务的ID
    /// </summary>
    private void RecycleTid()
    {
        lock (lockTid)
        {
            foreach (var item in RecTidLst)
            {
                for (int i = 0; i < TidLst.Count; i++)
                {
                    if (item == TidLst[i])
                    {
                        TidLst.RemoveAt(i);
                    }
                }
            }
        }
        
    }

    /// <summary>
    /// 设置打印日志的方法
    /// </summary>
    /// <param name="log"></param>
    public void SetLog(Action<string> log)
    {
        LogTask = log;
    }   
    /// <summary>
    /// 打印日志
    /// </summary>
    /// <param name="info"></param>
    public void LogInfo(string info)
    {
        if (LogTask != null)
        {
            LogTask(info);
        }
    }
   
    /// <summary>
    /// 设置TaskHandle
    /// </summary>
    /// <param name="_taskHandle"></param>
    public void SetTaskHandle(Action<Action<int>,int> _taskHandle)
    {
        taskHandle = _taskHandle;
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

    /// <summary>
    /// 时间计时任务类
    /// </summary>
    class TimeTask
    {
        public int ID { set; get; }
        public Action<int> CB { set; get; }
        public double DestTime { set; get; }
        public double Delsy { set; get; }
        public int Count { set; get; }

        public TimeTask(int id, Action<int> cb, double destTime, double delsy, int count)
        {
            ID = id;
            CB = cb;
            DestTime = destTime;
            Delsy = delsy;
            Count = count;
        }
    }

    /// <summary>
    /// 帧数计时任务类
    /// </summary>
    class FrameTask
    {
        public int ID { set; get; }
        public Action<int> CB { set; get; }
        public int DestFrame { set; get; }
        public int Delsy { set; get; }
        public int Count { set; get; }

        public FrameTask(int id, Action<int> cb, int destFrame, int delsy, int count)
        {
            ID = id;
            CB = cb;
            DestFrame = destFrame;
            Delsy = delsy;
            Count = count;
        }
    }

}

/// <summary>
/// 时间单位
/// </summary>
public enum PETimeUnity
{
    Millisecond,
    Second,
    Minute,
    Hour,
    Day
}


