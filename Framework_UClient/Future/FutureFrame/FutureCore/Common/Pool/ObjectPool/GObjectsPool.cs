/****************************************************************************
* ScriptType: 主框架
* 请勿修改!!!
****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public class GObjectsPool
    {
        private Transform m_releaseRoot;
        private Transform m_getRoot;
        private Action<GameObject> m_onNew;
        private Action<GameObject> m_onGet;
        private Action<GameObject> m_onRelease;

        private Dictionary<string, ObjectPool<GameObject>> m_pools = new Dictionary<string, ObjectPool<GameObject>>();

        public int CountAll
        {
            get
            {
                int countAll = 0;
                foreach (ObjectPool<GameObject> poolItem in m_pools.Values)
                {
                    countAll += poolItem.CountAll;
                }
                return countAll;
            }
        }

        public void InitRoot(Transform releaseRoot = null, Transform getRoot = null)
        {
            m_releaseRoot = releaseRoot;
            m_getRoot = getRoot;
        }
        
        public void InitCallBack(Action<GameObject> onNew, Action<GameObject> onGet, Action<GameObject> onRelease)
        {
            m_onNew = onNew;
            m_onGet = onGet;
            m_onRelease = onRelease;
        }

        /// <summary>
        /// 生产对象
        /// </summary>
        public GameObject Get(string prefabPath)
        {
            if (!m_pools.ContainsKey(prefabPath))
            {
                RegisterNew(prefabPath);
            }
            ObjectPool<GameObject> pool = m_pools[prefabPath];
            return pool.Get();
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Release(string prefabPath, GameObject obj)
        {
            ObjectPool<GameObject> pool = null;
            if (m_pools.TryGetValue(prefabPath, out pool))
            {
                pool.Release(obj);
            }
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Release(GameObject obj)
        {
            ObjectPool<GameObject> pool = null;
            foreach (ObjectPool<GameObject> poolItem in m_pools.Values)
            {
                if (poolItem.Contains(obj))
                {
                    pool = poolItem;
                    break;
                }
            }
            pool.Release(obj);
        }
        
        /// <summary>
        /// 创建新对象池
        /// </summary>
        private void RegisterNew(string prefabPath)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(()=>LoadNewObj(prefabPath), m_onGet, m_onRelease);
            
            m_pools.Add(prefabPath, pool);
        }

        /// <summary>
        /// 加载新的obj
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <returns></returns>
        private GameObject LoadNewObj(string prefabPath)
        { 
            GameObject gameObject = GameObject.Instantiate(ResMgr.Instance.SyncLoad<GameObject>(prefabPath));
            m_onNew?.Invoke(gameObject);
            return gameObject;
        }
        /// <summary>
        /// 清除指定子对象池
        /// </summary>
        public void ClearPool(string prefabPath)
        {
            ObjectPool<GameObject> pool = null;
            if (m_pools.TryGetValue(prefabPath, out pool))
            {
                pool.Clear();
                m_pools.Remove(prefabPath);
            }
        }

        /// <summary>
        /// 清除所有对象池
        /// </summary>
        public void Clear()
        {
            foreach (ObjectPool<GameObject> pool in m_pools.Values)
            {
                pool.Clear();
            }
            m_pools.Clear();
        }
        
        public void ReleaseAll()
        {
            foreach (ObjectPool<GameObject> pool in m_pools.Values)
            {
                pool.ReleaseAll();
            }
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            Clear();
            m_pools = null;
        }
    }
}