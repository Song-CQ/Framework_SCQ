using System.Collections.Generic;

namespace FutureCore
{
    public static class ListPool<T>
    {
        private static readonly ObjectPool<List<T>> _objectPool= new ObjectPool<List<T>>(null, OnRelease);

        public static List<T> Get()
        {
            return _objectPool.Get();
        }

        public static void Release(List<T> list)
        {
            _objectPool.Release(list);
        }

        private static void OnRelease(List<T> list)
        {
            list.Clear();
        }

        public static void Clear()
        {
            _objectPool.Clear();
        }
    }
}