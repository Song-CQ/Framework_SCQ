using System.Collections;
using System.Collections.Generic;

namespace FutureCore
{
    public static class ObjectPoolStatic<T> where T : new()
    {
        private static ObjectPool<T> _pool = new ObjectPool<T>();

        public static T Get()
        {
            return _pool.Get();
        }
        public static void Release(T item)
        {
            _pool.Release(item);
        }
    }
} 


