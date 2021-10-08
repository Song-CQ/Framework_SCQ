/****************************************************
    文件：ObjectPool.cs
	作者：清
    邮箱: 2728285639@qq.com
	功能：对象池
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using Object=UnityEngine.Object;
public class ObjectPool<T>:IDisposable where T:Enum
{
    private Dictionary<T, List<Object>> PoolDic;
    private Dictionary<T, Transform> PollPar;
    private List<T> temp;
    private Transform objectPoolPar;
    /// <summary>
    /// 对象池父物体
    /// </summary>
    /// <param name="_objectPoolPar">null则自自动创建</param>
    public ObjectPool(Transform _objectPoolPar = null)
    {     
        objectPoolPar = _objectPoolPar;
        if (objectPoolPar == null)
        {
            objectPoolPar = new GameObject("ObjectPool").transform;
            objectPoolPar.localPosition = Vector3.zero;
            objectPoolPar.localEulerAngles = Vector3.zero;
        }
        PoolDic = new Dictionary<T, List<Object>>();
        PollPar = new Dictionary<T, Transform>();
        temp = new List<T>();
    }
    /// <summary>
    /// 获取实体
    /// </summary>
    /// <typeparam name="Obj">获取的实体类型</typeparam>
    /// <param name="key">对象池Key值</param>
    /// <param name="Go">当对象池没有实体时，则创建新的实体，Null为不创建,默认为Null</param>
    /// <returns></returns>
    public Obj GetObject<Obj>(T key, Obj Go =null) where Obj : Object
    {
        Obj obj =null;
        if (!PoolDic.ContainsKey(key))
        {
            AddKey(key);
        }
        if (PoolDic[key].Count > 0)
        {
            obj = PoolDic[key][0] as Obj;
            PoolDic[key].RemoveAt(0);
        }else
        {
            obj = LoadObject(key, Go);
        }
        return obj;
    }

    private void AddKey(T key)
    {
        PoolDic.Add(key, new List<Object>());
        Transform par = new GameObject(key.ToString()).transform;
        par.SetParent(objectPoolPar);
        par.localScale = Vector3.one;
        par.localPosition = Vector3.zero;
        par.localEulerAngles = Vector3.zero;
        PollPar.Add(key, par.transform);
    }

    private Obj LoadObject<Obj>(T key, Obj Comer) where Obj : UnityEngine.Object
    {
        Obj obj = GameObject.Instantiate(Comer);
        GameObject go = obj as GameObject;
        if (go == null)
        {
            go = (obj as Component).gameObject;
        }
        if (PollPar.TryGetValue(key, out Transform par))
        {
            go.transform.SetParent(par);
        }
        return obj;
    }

    public void RecObject(T key, Object obj)
    {
        if (obj == null) return;
        if (!PoolDic.ContainsKey(key))
        {
            PoolDic.Add(key, new List<Object>());
        }
        PoolDic[key].Add(obj);

    }

    public void RemoveKey(T key)
    {
        if (PoolDic.ContainsKey(key))
        {
            foreach (Object item in PoolDic[key])
            {
                if (item != null)
                {
                    GameObject go = item as GameObject;
                    if (go==null)
                    {
                        Component cot = item as Component;
                        go = cot.gameObject;
                    }
                    GameObject.Destroy(go);
                }
            }
            PoolDic[key].Clear();
            PoolDic.Remove(key);
        }
    }
    public void RemoveAll()
    {
        temp.Clear();
        foreach (var item in PoolDic)
        {
            temp.Add(item.Key);
          
        }
        foreach (var item in temp)
        {
            RemoveKey(item);
        }
        temp.Clear();
    }

    /// <summary>
    /// 销毁对象池
    /// </summary>
    public void Dispose()
    {
        RemoveAll();
        PoolDic = null;
        PollPar = null;
        temp = null;
        

    }
}