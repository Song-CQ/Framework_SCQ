/****************************************************
    文件: Event.cs
    作者: Clear
    日期: 2026/3/16 15:16:17
    类型: 逻辑脚本
    功能: Nothing 
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace FutureCore
{
     /// <summary>
    /// 事件
    /// </summary>
    public class EventData : IDisposable,IResettable
    {
        public static bool eventPoolIsOpen = false;
        // 事件池
        private static Dictionary<string, ObjectPool<EventData>> _eventPool;

        public string eventType;
        public int dataInt;
        public object data;
        public object[] datas;

        private bool _isFormPool = false;

        public static void Init()
        {
            _eventPool = new Dictionary<string, ObjectPool<EventData>>();
            eventPoolIsOpen = true;
        }
        public static void Clear()
        {
            if (_eventPool != null)
            {
                _eventPool.Clear();
                _eventPool = null;
                eventPoolIsOpen = false;
            }
        }
        public EventData()
        {         
            
        }
        public EventData(string eventType, object data = null)
        {
            
            this.eventType = eventType;
            this.data = data;
        }

        public virtual void Reset()
        {
            eventType = null;
            data = null;
            datas = null;
            dataInt = 0;
        }

        public void Dispose()
        {
            Reset();
        }

        public static void ReturnEvent(EventData evt)
        {
            // 不是对象池创建的不予回收
            if (!evt._isFormPool) return;
            if (eventPoolIsOpen)
            {
                string type = evt.eventType;
                ObjectPool<EventData> stack;
                _eventPool.TryGetValue(type, out stack);
                if (stack != null)
                {
                    evt.Reset();
                    stack.Release(evt);
                }
            }
        }

        public static T GetEvent<T>(object data = null) where T : EventData, new()
        {

            // 获取类型
            T evt = null;
            string _cachedType = typeof(T).FullName;
            // 对象池开启
            if (eventPoolIsOpen)
            {       
                _eventPool.TryGetValue(_cachedType, out ObjectPool<EventData> pool);
                if (pool == null)
                {
                    pool = new ObjectPool<EventData>(() => new T());    
                    _eventPool[_cachedType] = pool;
                }
                evt = pool.Get() as T;
               
            }
            else // 未开启直接new
            {
                evt = new T();
            }

            

            evt._isFormPool = true;
            evt.eventType = _cachedType;
            evt.data = data;
            return evt;
        }
    }
}