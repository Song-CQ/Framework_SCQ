/****************************************************
    文件: UGUIEntity.cs
    作者: Clear
    日期: 2023/4/23 14:58:37
    类型: 框架核心脚本(请勿修改)
    功能: UGUI实体
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ProjectApp
{
    public class UGUIEntity : UIEntity
    {


        private Dictionary<string, Component> componentPool = new Dictionary<string, Component>();
        private Dictionary<string, GameObject> gameObjectPool = new Dictionary<string, GameObject>();

        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        /// <summary>
        /// 获取对应路径名字的组件
        /// </summary>
        /// <typeparam name="T">组件名</typeparam>
        /// <param name="namePath">路径名</param>
        /// <param name="isAdd">是否要添加</param>
        /// <param name="isCance">是否缓存</param>
        /// <returns></returns>
        public T GetComponent<T>(string namePath, bool isAdd = true ,bool isCance = true) where T : Component
        {
            Component _component = null;
            if (!componentPool.TryGetValue(namePath, out _component))
            {
                Transform trf = Transform.Find(namePath);
                if (trf)
                {
                    _component = trf.GetComponent<T>();
                    if (_component&&isAdd)
                    {
                        _component = trf.gameObject.AddComponent<T>();
                    }
                }
                else return null;
                if (isCance)
                {
                    componentPool.Add(namePath, _component);
                }
            }
            return _component as T; 
        }

        public Transform GetTransform(string namePath, bool isCance = true)
        {
            return GetComponent<Transform>(namePath, false,isCance);
        }

        public GameObject GetGameObject(string namePath, bool isCance = true)
        {
            GameObject _go = null;
            if (!gameObjectPool.TryGetValue(namePath, out _go))
            {
                Transform trf = Transform.Find(namePath);
                if (trf)
                {
                    _go = trf.gameObject;
                }
                if (isCance)
                {
                    gameObjectPool.Add(namePath, _go);
                }
            }
            return _go;
        }


        public override void SetVisible(bool State)
        {
           
        }

        public override void OpenUIAnim(Action onComplete)
        {
            

            base.OpenUIAnim(onComplete);
        } 
        
        public override void CloseUIAnim(Action onComplete)
        {


            base.CloseUIAnim(onComplete);
        }

        public override void Dispose()
        {
            componentPool.Clear();
            componentPool = null;
            gameObjectPool.Clear();
            gameObjectPool = null;

            GameObject.Destroy();
        }

    }
}