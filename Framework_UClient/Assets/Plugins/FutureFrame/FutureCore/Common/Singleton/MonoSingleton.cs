using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    /// <summary>
    /// 单例父类工具
    /// </summary>
    public class MonoSingleton<T> : MonoBehaviour where T :MonoSingleton<T>
    {

       
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        CreateInstance();
                    }
                    else
                    {
                        instance.New();
                    }
                }
                return instance;
            }

        }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                instance.New();
            }
        }

        protected virtual void New()
        {
            gameObject.name = "[MonoMgr]"+nameof(T);
        }

        public virtual void Init()
        { }
        
        private static void CreateInstance()
        {
            GameObject instanceGO = new GameObject(typeof(T).Name);
            instance = instanceGO.AddComponent<T>();
            instance.New();
        }

        public virtual void OnDestroy()
        {
            instance = null;
        }
    }
}
    //普通单例的缺点:
    //代码重复
    //由于在Awake赋值,所以客户端代码只能在Awake以后的脚本生命周期中访问

    /*解决方案:定义MonoSingleton类1,
    适用性:场景中存在唯一的对象,即可让该对象继承当前类。
    2.如何适用:-继承时必须传递子类类型。
    在任意脚本生命周期中,通过子类类型访问Instance属性。*/
