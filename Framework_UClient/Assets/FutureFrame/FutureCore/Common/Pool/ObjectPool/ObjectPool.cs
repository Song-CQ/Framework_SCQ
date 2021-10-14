using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public class ObjectPool<T>:IDisposable where T : new()
    {
        private Stack<T> m_Stack = new Stack<T>();

        private Func<T> m_FuncOnNew;
        private Action<T> m_ActionOnGet;
        private Action<T> m_ActionOnRelease;

        public int CountAll { get; private set; }
        public int CountInactive { get { return m_Stack.Count; } }
        public int CountActive { get { return CountAll - CountInactive; } }
        
        public ObjectPool()
        {
        }

        public ObjectPool(Action<T> mActionOnGet, Action<T> mActionOnRelease)
        {
            m_ActionOnGet = mActionOnGet;
            m_ActionOnRelease = mActionOnRelease;
        }

        public ObjectPool(Func<T> mFuncOnNew, Action<T> mActionOnGet, Action<T> mActionOnRelease)
        {
            m_FuncOnNew = mFuncOnNew;
            m_ActionOnGet = mActionOnGet;
            m_ActionOnRelease = mActionOnRelease;
        }

        public T Get()
        {
            T obj;
            if (m_Stack.Count==0)
            {
                CountAll++;
                if (m_FuncOnNew!=null)
                {
                    obj = m_FuncOnNew.Invoke();
                }
                else
                { 
                    obj = new T();
                }
            }
            else
            {
                obj = m_Stack.Pop();
            }
            m_ActionOnGet?.Invoke(obj);
            return obj;
        }

        public void Release(T obj)
        {
            if (m_Stack.Contains(obj))
            {
                Debug.LogError("[ObjectPool]Error: Trying to destroy object that is already released to pool!");
            }
            else
            {
                m_ActionOnRelease?.Invoke(obj);
                m_Stack.Push(obj);
            }
        }

        public void Clear()
        {
            m_Stack.Clear();
        }

        public void Dispose()
        {
            m_Stack.Clear();
            m_Stack = null;

            m_FuncOnNew = null;
            m_ActionOnGet = null;
            m_ActionOnRelease = null;
        }
    }
}
