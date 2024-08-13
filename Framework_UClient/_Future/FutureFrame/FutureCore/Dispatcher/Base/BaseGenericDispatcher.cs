/****************************************************
    文件：BaseGenericDispatcher.cs
	作者：Clear
    日期：2022/5/4 16:25:18
    类型: 框架核心脚本(请勿修改)
	功能：基础泛型消息派发器
*****************************************************/
using System;
using System.Collections.Generic;

namespace FutureCore
{
    public abstract class BaseGenericDispatcher<T> where T : class, new()
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

        private Dictionary<uint, List<Delegate>> listeners = new Dictionary<uint, List<Delegate>>();

        #region Add

        public void AddListener<T1, T2, T3, T4>(uint msgId, Action<T1, T2, T3, T4> callback)
        {
            AddListener(msgId, (Delegate)callback);
        }

        public void AddListener<T1, T2, T3>(uint msgId, Action<T1, T2, T3> callback)
        {
            AddListener(msgId, (Delegate)callback);
        }

        public void AddListener<T1, T2>(uint msgId, Action<T1, T2> callback)
        {
            AddListener(msgId, (Delegate)callback);
        }

        public void AddListener<T1>(uint msgId, Action<T1> callback)
        {
            AddListener(msgId, (Delegate)callback);
        }

        public void AddListener(uint msgId, Action callback)
        {
            AddListener(msgId, (Delegate)callback);
        }

        public void AddListener(uint msgId, Delegate callback)
        {
            List<Delegate> list;
            if (listeners.TryGetValue(msgId, out list))
            {
                list.Add(callback);
            }
            else
            {
                list = ListPool<Delegate>.Get();
                list.Add(callback);
                listeners.Add(msgId, list);
            }
        }

        #endregion Add

        #region Remove

        public void RemoveListener<T1, T2, T3, T4>(uint msgId, Action<T1, T2, T3, T4> callback)
        {
            RemoveListener(msgId, (Delegate)callback);
        }

        public void RemoveListener<T1, T2, T3>(uint msgId, Action<T1, T2, T3> callback)
        {
            RemoveListener(msgId, (Delegate)callback);
        }

        public void RemoveListener<T1, T2>(uint msgId, Action<T1, T2> callback)
        {
            RemoveListener(msgId, (Delegate)callback);
        }

        public void RemoveListener<T1>(uint msgId, Action<T1> callback)
        {
            RemoveListener(msgId, (Delegate)callback);
        }

        public void RemoveListener(uint msgId, Action callback)
        {
            RemoveListener(msgId, (Delegate)callback);
        }

        private void RemoveListener(uint msgId, Delegate callback)
        {
            List<Delegate> list;
            if (listeners.TryGetValue(msgId, out list))
            {
                if (list.Contains(callback))
                {
                    list.Remove(callback);
                    if (list.Count == 0)
                    {
                        ListPool<Delegate>.Release(list);
                        listeners.Remove(msgId);
                    }
                }
            }
        }

        #endregion Remove

        #region Dispatch

        public void Dispatch<T1, T2, T3, T4>(uint msgId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            List<Delegate> methods = GetDelegates(msgId);
            if (methods == null) return;

            int funcCount = methods.Count;
            if (funcCount == 1)
            {
                Delegate m = methods[0];
                ((Action<T1, T2, T3, T4>)m)(arg1, arg2, arg3, arg4);
                return;
            }

            List<Delegate> invokeFuncs = ListPool<Delegate>.Get();
            invokeFuncs.AddRange(methods);
            for (int i = 0; i < funcCount; i++)
            {
                try
                {
                    Delegate m = invokeFuncs[i];
                    ((Action<T1, T2, T3, T4>)m)(arg1, arg2, arg3, arg4);
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }
            ListPool<Delegate>.Release(invokeFuncs);
        }

        public void Dispatch<T1, T2, T3>(uint msgId, T1 arg1, T2 arg2, T3 arg3)
        {
            List<Delegate> methods = GetDelegates(msgId);
            if (methods == null) return;

            int funcCount = methods.Count;
            if (funcCount == 1)
            {
                Delegate m = methods[0];
                ((Action<T1, T2, T3>)m)(arg1, arg2, arg3);
                return;
            }

            List<Delegate> invokeFuncs = ListPool<Delegate>.Get();
            invokeFuncs.AddRange(methods);
            for (int i = 0; i < funcCount; i++)
            {
                try
                {
                    Delegate m = invokeFuncs[i];
                    ((Action<T1, T2, T3>)m)(arg1, arg2, arg3);
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }
            ListPool<Delegate>.Release(invokeFuncs);
        }

        public void Dispatch<T1, T2>(uint msgId, T1 arg1, T2 arg2)
        {
            List<Delegate> methods = GetDelegates(msgId);
            if (methods == null) return;

            int funcCount = methods.Count;
            if (funcCount == 1)
            {
                Delegate m = methods[0];
                ((Action<T1, T2>)m)(arg1, arg2);
                return;
            }

            List<Delegate> invokeFuncs = ListPool<Delegate>.Get();
            invokeFuncs.AddRange(methods);
            for (int i = 0; i < funcCount; i++)
            {
                try
                {
                    Delegate m = invokeFuncs[i];
                    ((Action<T1, T2>)m)(arg1, arg2);
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }
            ListPool<Delegate>.Release(invokeFuncs);
        }

        public void Dispatch<T1>(uint msgId, T1 arg1)
        {
            List<Delegate> methods = GetDelegates(msgId);
            if (methods == null) return;

            int funcCount = methods.Count;
            if (funcCount == 1)
            {
                Delegate m = methods[0];
                ((Action<T1>)m)(arg1);
                return;
            }

            List<Delegate> invokeFuncs = ListPool<Delegate>.Get();
            invokeFuncs.AddRange(methods);
            for (int i = 0; i < funcCount; i++)
            {
                try
                {
                    Delegate m = invokeFuncs[i];
                    ((Action<T1>)m)(arg1);
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }
            ListPool<Delegate>.Release(invokeFuncs);
        }

        public void Dispatch(uint msgId)
        {
            List<Delegate> methods = GetDelegates(msgId);
            if (methods == null) return;

            int funcCount = methods.Count;
            if (funcCount == 1)
            {
                Delegate m = methods[0];
                ((Action)m)();
                return;
            }

            List<Delegate> invokeFuncs = ListPool<Delegate>.Get();
            invokeFuncs.AddRange(methods);
            for (int i = 0; i < funcCount; i++)
            {
                try
                {
                    Delegate m = methods[i];
                    ((Action)m)();
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }
            ListPool<Delegate>.Release(invokeFuncs);
        }

        private List<Delegate> GetDelegates(uint msgId)
        {
            List<Delegate> list;
            if (listeners.TryGetValue(msgId, out list))
            {
                return list;
            }
            return null;
        }

        private static void LogError(Exception e)
        {
            LogUtil.LogError(e);
        }

        #endregion Dispatch

        public void Dispose()
        {
            m_instance = null;
            listeners.Clear();
            listeners = null;
        }
    }
}