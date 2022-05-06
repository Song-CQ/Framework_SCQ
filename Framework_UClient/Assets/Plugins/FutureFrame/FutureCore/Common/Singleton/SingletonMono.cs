using UnityEngine;

namespace FutureCore
{
    /// <summary>
    /// MonoBehaviour 单例类
    /// </summary>
    public class SingletonMono<T> : MonoBehaviour where T :SingletonMono<T>
    {
        protected virtual string ParentRootName { get { return null; } }
        private static bool isAppQuit = false;
       
        private static T instance;    
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    if (isAppQuit)
                    {
                        return null;
                    }
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
            

        }

        public virtual void Init()
        { }
        
        private static void CreateInstance()
        {
            GameObject instanceGO = new GameObject(typeof(T).Name);
            instance = instanceGO.AddComponent<T>();
            SetSelfParentRoot(instanceGO, instance.ParentRootName);
            instance.New();
        }
        private static void SetSelfParentRoot(GameObject go, string parentRootName)
        {
            if (!AppObjConst.EngineSingletonGo) return;

            if (parentRootName == null)
            {
                go.transform.SetParent(AppObjConst.EngineSingletonGo.transform, false);
            }
            else
            {
                Transform sinleonRoot = AppObjConst.EngineSingletonGo.transform;
                Transform rootTF = sinleonRoot.Find(parentRootName);
                if (rootTF == null)
                {
                    GameObject rootGo = new GameObject(parentRootName);                  
                    rootTF = rootGo.transform;
                    rootTF.transform.SetParent(sinleonRoot, false);
                }
                go.transform.SetParent(rootTF.transform, false);
            }
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
