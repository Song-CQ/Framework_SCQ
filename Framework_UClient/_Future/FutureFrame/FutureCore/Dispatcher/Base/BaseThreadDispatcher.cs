using System;
using System.Collections;
using System.Collections.Generic;

namespace FutureCore
{
    public abstract class BaseThreadDispatcher<T,Msg,Param>:IDisposable
    where T : class,new()
    where Param : class
    {
        private static T m_instance;
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new T();
                }
                return m_instance;
            }
        }
        
        private struct ThreadMsg
        {
            public Msg currMsgId;
            public Param currParam;

            public ThreadMsg(Msg _currMsgId,Param _currParam)
            {
                currMsgId = _currMsgId;
                currParam = _currParam;
            }
        }

        private const string m_queueLock = "QueueLock";
        
        private Queue<ThreadMsg> m_msgQueue = new Queue<ThreadMsg>();
        private Dictionary<Msg, List<Action<Param>>> m_msgPriorityDict = new Dictionary<Msg, List<Action<Param>>>();
        private Dictionary<Msg, List<Action<Param>>> m_msgDict = new Dictionary<Msg, List<Action<Param>>>();
        private Dictionary<Msg, List<Action<Param>>> m_msgFinallyDict = new Dictionary<Msg, List<Action<Param>>>();
        private Dictionary<Msg, List<Action<Param>>> m_msgOnceDict = new Dictionary<Msg, List<Action<Param>>>();
        
        public void AddPriorityListener(Msg msg, Action<Param> paramCB)
        {
            if (!m_msgPriorityDict.TryGetValue(msg,out List<Action<Param>> actionLst))
            {
                actionLst = ListPool<Action<Param>>.Get();
                m_msgPriorityDict.Add(msg,actionLst);
            }
            if (paramCB!=null)
            {
                actionLst.Add(paramCB);
            }
        }
        public void AddListener(Msg msg, Action<Param> paramCB)
        {
            if (!m_msgDict.TryGetValue(msg,out List<Action<Param>> actionLst))
            {
                actionLst = ListPool<Action<Param>>.Get();
                m_msgDict.Add(msg,actionLst);
            }
            if (paramCB!=null)
            {
                actionLst.Add(paramCB);
            }
        }
        public void AddFinallyListener(Msg msg, Action<Param> paramCB)
        {
            if (!m_msgFinallyDict.TryGetValue(msg,out List<Action<Param>> actionLst))
            {
                actionLst = ListPool<Action<Param>>.Get();
                m_msgFinallyDict.Add(msg,actionLst);
            }
            if (paramCB!=null)
            {
                actionLst.Add(paramCB);
            }
        }
        public void AddOnceListener(Msg msg, Action<Param> paramCB)
        {
            if (!m_msgOnceDict.TryGetValue(msg,out List<Action<Param>> actionLst))
            {
                actionLst = ListPool<Action<Param>>.Get();
                m_msgOnceDict.Add(msg,actionLst);
            }
            if (paramCB!=null)
            {
                actionLst.Add(paramCB);
            }
        }

        public void RemovePriorityListener(Msg msg, Action<Param> paramCB)
        {
            if (!m_msgPriorityDict.TryGetValue(msg,out List<Action<Param>> actionLst))
            {
                actionLst = ListPool<Action<Param>>.Get();
                m_msgPriorityDict.Add(msg,actionLst);
            }
            if (paramCB!=null)
            {
                actionLst.Remove(paramCB);
            }
        }
        public void RemoveListener(Msg msg, Action<Param> paramCB)
        {
            if (!m_msgDict.TryGetValue(msg,out List<Action<Param>> actionLst))
            {
                actionLst = ListPool<Action<Param>>.Get();
                m_msgDict.Add(msg,actionLst);
            }
            if (paramCB!=null)
            {
                actionLst.Remove(paramCB);
            }
        }
        public void RemoveFinallyListener(Msg msg, Action<Param> paramCB)
        {
            if (!m_msgFinallyDict.TryGetValue(msg,out List<Action<Param>> actionLst))
            {
                actionLst = ListPool<Action<Param>>.Get();
                m_msgFinallyDict.Add(msg,actionLst);
            }
            if (paramCB!=null)
            {
                actionLst.Remove(paramCB);
            }
        }
        public void RemoveOnceListener(Msg msg, Action<Param> paramCB)
        {
            if (!m_msgOnceDict.TryGetValue(msg,out List<Action<Param>> actionLst))
            {
                actionLst = ListPool<Action<Param>>.Get();
                m_msgOnceDict.Add(msg,actionLst);
            }
            if (paramCB!=null)
            {
                actionLst.Remove(paramCB);
            }
        }

        private bool ContainsListener(Msg msgId, Action<Param> listener, Dictionary<Msg, List<Action<Param>>> msgDict)
        {
            if (msgDict.ContainsKey(msgId))
            {
                List<Action<Param>> list = msgDict[msgId];
                return list.Contains(listener);
            }
            return false;
        }
      
        public bool ContainsPriorityListener(Msg msgId, Action<Param> listener)
        {
            return ContainsListener(msgId, listener, m_msgPriorityDict);
        }

        public bool ContainsListener(Msg msgId, Action<Param> listener)
        {
            return ContainsListener(msgId, listener, m_msgDict);
        }

        public bool ContainsFinallyListener(Msg msgId, Action<Param> listener)
        {
            return ContainsListener(msgId, listener, m_msgFinallyDict);
        }

        public bool ContainsOnceListener(Msg msgId, Action<Param> listener)
        {
            return ContainsListener(msgId, listener, m_msgOnceDict);
        }

        public void Dispatch(Msg msg,Param param = null)
        {
            if (!m_msgPriorityDict.ContainsKey(msg)
                && !m_msgDict.ContainsKey(msg)
                && !m_msgFinallyDict.ContainsKey(msg)
                && !m_msgOnceDict.ContainsKey(msg))
                return;
            ThreadMsg threadMsg = new ThreadMsg(msg, param);
            
            lock (m_queueLock)
            {
                m_msgQueue.Enqueue(threadMsg);
            }
        }

        private void AutoDispatch(Msg msg,Param param)
        {
            InvokeMethods(m_msgPriorityDict,msg,param);
            InvokeMethods(m_msgDict,msg,param);
            InvokeMethods(m_msgFinallyDict,msg,param);
            InvokeMethods(m_msgOnceDict,msg,param);
            if (m_msgOnceDict.TryGetValue(msg,out List<Action<Param>> val))
            {
                ListPool<Action<Param>>.Release(val);
                m_msgOnceDict.Remove(msg);
            }
        }
        
        public void Update()
        {
            if (m_msgQueue.Count <= 0) return;

            while (m_msgQueue.Count > 0)
            {
                ThreadMsg msg;
                lock (m_queueLock)
                {
                    msg = m_msgQueue.Dequeue();
                }
                AutoDispatch(msg.currMsgId, msg.currParam);
            }
        }
        
        private void InvokeMethods(Dictionary<Msg, List<Action<Param>>> msgDict, Msg msgId, Param param)
        {
            if (msgDict.TryGetValue(msgId,out List<Action<Param>> value))
            {
                try
                {
                    if (value.Count==1)
                    {
                        value[0]?.Invoke(param);
                        return;
                    }
                    foreach (var methods in value)
                    {
                        methods?.Invoke(param);
                    }
                }
                catch (Exception e)
                {
                    LogUtil.LogError(e);
                }
            }
        }

        public void Clear()
        {
            m_msgPriorityDict.Clear();
            m_msgDict.Clear();
            m_msgFinallyDict.Clear();
            m_msgOnceDict.Clear();
        }

        public void Dispose()
        {
            m_instance = null;
            foreach (var item in m_msgPriorityDict)
            {
                List<Action<Param>> list = item.Value;
                ListPool<Action<Param>>.Release(list);
            }
            foreach (var item in m_msgDict)
            {
                List<Action<Param>> list = item.Value;
                ListPool<Action<Param>>.Release(list);
            }
            foreach (var item in m_msgFinallyDict)
            {
                List<Action<Param>> list = item.Value;
                ListPool<Action<Param>>.Release(list);
            }
            foreach (var item in m_msgOnceDict)
            {
                List<Action<Param>> list = item.Value;
                ListPool<Action<Param>>.Release(list);
            }
            Clear();
            m_msgPriorityDict = null;
            m_msgDict = null;
            m_msgFinallyDict = null;
            m_msgOnceDict = null;
        }
    }
}


