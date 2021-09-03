using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XL.Common
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
                        //创建的同时会调用Awake
                        return new GameObject("Singleton_of_" + typeof(T)).AddComponent<T>();
                        
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
              
            }

        }
        public virtual void Init()
        { }
    }
}
    //普通单例的缺点:
    //代码重复
    //由于在Awake赋值,所以客户端代码只能在Awake以后的脚本生命周期中访问

    /*解决方案:定义MonoSingleton类1,
    适用性:场景中存在唯一的对象,即可让该对象继承当前类。
    2.如何适用:-继承时必须传递子类类型。
    在任意脚本生命周期中,通过子类类型访问Instance属性。*/
