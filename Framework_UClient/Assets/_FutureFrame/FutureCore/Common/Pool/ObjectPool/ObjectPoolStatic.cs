using System.Collections;
using System.Collections.Generic;

namespace FutureCore
{
    public static class ObjectPoolStatic<T> where T :new()
    {
        private static ObjectPool<T> _pool = new ObjectPool<T>();

        private static void Init()
        {
            _pool = new ObjectPool<T>();
        }

        public static void Dispose()
        {
            _pool.Dispose();
            _pool = null;
        }
        public static void Clear()
        {
            if (_pool == null)
            {
                return;
            }
            _pool.Clear();
        }

        public static T Get()
        {
            if (_pool == null)
            {
                Init();
            }
            return _pool.Get();
        }
        public static void Release(T item)
        {
            if (_pool == null)
            {
                Init();
            }
            _pool.Release(item);
        }
    }
} 


