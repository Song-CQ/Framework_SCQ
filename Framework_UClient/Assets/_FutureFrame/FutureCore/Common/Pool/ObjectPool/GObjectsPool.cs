/****************************************************
    文件: ResLoadNode.cs
    作者: Clear
    日期: 2023/5/17 20:4:24
    类型: 框架核心脚本(请勿修改)
    功能: GameObject类型对象池
          [通过设置不同对象的加载回调函数 实现不同物体加载回调]  
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FutureCore
{
    public class GObjectsPool
    {
        private Transform m_Root;


        private Dictionary<string, ObjectPool<Object>> m_pools = new Dictionary<string, ObjectPool<Object>>();
        private Dictionary<string, Transform> m_poolParent = new Dictionary<string, Transform>();

        private Dictionary<string, Func<Object>> m_onLoadNew_CallBack = new Dictionary<string, Func<Object>>();
        private Dictionary<string, Action<Object>> m_onNew_CallBack = new Dictionary<string, Action<Object>>();
        private Dictionary<string, Action<Object>> m_onGet_CallBack = new Dictionary<string, Action<Object>>();
        private Dictionary<string, Action<Object>> m_onRelease_CallBack = new Dictionary<string, Action<Object>>();

        public int CountAll
        {
            get
            {
                int countAll = 0;
                foreach (ObjectPool<Object> poolItem in m_pools.Values)
                {
                    countAll += poolItem.CountAll;
                }
                return countAll;
            }
        }

        public void InitRoot(string name, Transform releaseRoot = null)
        {

            m_Root = new GameObject(name).transform;

            if (releaseRoot)
            {
                m_Root.SetParent(releaseRoot);
            }
            else
            {
                GameObject.DontDestroyOnLoad(m_Root);
            }

            m_Root.transform.localPosition = new Vector3(1000,0,0);
        }


        /// <summary>
        /// 设置指定对象池的创建新物体函数
        /// </summary>
        public void SetCallBack_loadNewObj(string prefabPath, Func<Object> fc)
        {
            m_onLoadNew_CallBack[prefabPath] = fc;
        }
        /// <summary>
        /// 设置指定对象池的New函数
        /// </summary>
        public void SetCallBack_onNew(string prefabPath, Action<Object> fc)
        {
            m_onNew_CallBack[prefabPath] = fc;
        }

        /// <summary>
        /// 设置指定对象池的Get函数
        /// </summary>
        public void SetCallBack_onGet(string prefabPath, Action<Object> fc)
        {
            m_onGet_CallBack[prefabPath] = fc;
        }
        /// <summary>
        /// 设置指定对象池的Get函数
        /// </summary>
        public void SetCallBack_onRelease(string prefabPath, Action<Object> fc)
        {
            m_onRelease_CallBack[prefabPath] = fc;
        }


        /// <summary>
        /// 生产对象
        /// </summary>
        public T Get<T>(string prefabPath) where T : Component
        {
            if (!m_pools.ContainsKey(prefabPath))
            {
                RegisterNew(prefabPath);
            }
            ObjectPool<Object> pool = m_pools[prefabPath];

            GameObject go = pool.Get() as GameObject;

            return go.GetComponent<T>();
        }

        public GameObject Get(string prefabPath)
        {

            if (!m_pools.ContainsKey(prefabPath))
            {
                RegisterNew(prefabPath);
            }
            ObjectPool<Object> pool = m_pools[prefabPath];

            GameObject go = pool.Get() as GameObject;

            return go;
        }



        /// <summary>
        /// 回收对象
        /// </summary>
        public void Release(string prefabPath, Object obj)
        {
            ObjectPool<Object> pool = null;
            if (m_pools.TryGetValue(prefabPath, out pool))
            {
                pool.Release(obj);
            }
        }

        /// <summary>
        /// 回收对象(无法确定路径消耗更大)
        /// </summary>
        public void Release(Object obj)
        {
            ObjectPool<Object> pool = null;
            foreach (ObjectPool<Object> poolItem in m_pools.Values)
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
            ObjectPool<Object> pool = new ObjectPool<Object>(() => OnNew_CallBack(prefabPath), (obj) => OnGet_CallBack(prefabPath, obj), (obj
                ) => OnRelease_CallBack(prefabPath, obj));
            Transform pooltrf = new GameObject(prefabPath).transform;
            pooltrf.parent = m_Root;
            pooltrf.localPosition = Vector3.zero;
            m_poolParent.Add(prefabPath, pooltrf);
            m_pools.Add(prefabPath, pool);
        }

        /// <summary>
        /// 加载新的obj
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <returns></returns>
        private Object OnNew_CallBack(string prefabPath)
        {
            Object gameObject = null;
            if (m_onLoadNew_CallBack.ContainsKey(prefabPath))
            {
                gameObject = m_onLoadNew_CallBack[prefabPath]?.Invoke();
            }
            else
            {
                gameObject = GameObject.Instantiate(ResMgr.Instance.LoadLocalRes<GameObject>(prefabPath));
            }
            if (m_onNew_CallBack.ContainsKey(prefabPath))
            {
                m_onNew_CallBack[prefabPath]?.Invoke(gameObject);
            }


            return gameObject;
        }
        private void OnGet_CallBack(string prefabPath, Object obj)
        {
            if (obj is GameObject)
            {
                (obj as GameObject).SetActive(true);
            }
            else
            {
                Component go = obj as Component;
                if (go != null)
                {
                    go.SetActive(true);
                }


            }


            if (m_onGet_CallBack.ContainsKey(prefabPath))
            {
                m_onGet_CallBack[prefabPath]?.Invoke(obj);
            }
        }
        private void OnRelease_CallBack(string prefabPath, Object obj)
        {
            if (m_onRelease_CallBack.ContainsKey(prefabPath))
            {
                m_onRelease_CallBack[prefabPath]?.Invoke(obj);
            }
            GameObject go = (obj as GameObject);


            if (go == null)
            {
                Component component = obj as Component;
                if (component != null)
                {
                    go = component.gameObject;
                }

            }

            go.SetActive(false);
            go.transform.SetParent(m_poolParent[prefabPath]);
            go.transform.localPosition = Vector3.one;

        }
        /// <summary>
        /// 清除指定子对象池
        /// </summary>
        public void ClearPool(string prefabPath)
        {
            ObjectPool<Object> pool = null;
            if (m_pools.TryGetValue(prefabPath, out pool))
            {
                pool.Clear();
                m_pools.Remove(prefabPath);
            }

            Transform val = m_poolParent[prefabPath];
            m_poolParent.Remove(prefabPath);
            GameObject.Destroy(val.gameObject);

            if (m_onLoadNew_CallBack.ContainsKey(prefabPath))
            {
                m_onLoadNew_CallBack.Remove(prefabPath);
            }
            if (m_onNew_CallBack.ContainsKey(prefabPath))
            {
                m_onNew_CallBack.Remove(prefabPath);
            }
            if (m_onGet_CallBack.ContainsKey(prefabPath))
            {
                m_onGet_CallBack.Remove(prefabPath);
            }
            if (m_onRelease_CallBack.ContainsKey(prefabPath))
            {
                m_onRelease_CallBack.Remove(prefabPath);
            }
        }

        /// <summary>
        /// 清除所有对象池
        /// </summary>
        public void Clear()
        {
            foreach (ObjectPool<Object> pool in m_pools.Values)
            {
                pool.Clear();
            }
            m_pools.Clear();
            foreach (var item in m_poolParent)
            {
                Transform val = item.Value;
                m_poolParent[item.Key] = null;
                GameObject.Destroy(val.gameObject);
            }
            m_onLoadNew_CallBack.Clear();
            m_onNew_CallBack.Clear();
            m_onGet_CallBack.Clear();
            m_onRelease_CallBack.Clear();

        }

        public void ReleaseAll()
        {
            foreach (ObjectPool<Object> pool in m_pools.Values)
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
            GameObject.Destroy(m_Root);
            m_Root = null;
        }
    }
}