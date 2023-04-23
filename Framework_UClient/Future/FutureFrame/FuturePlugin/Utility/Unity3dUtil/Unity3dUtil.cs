using UnityEngine;
using UObject = UnityEngine.Object;

namespace FuturePlugin
{
    public static class Unity3dUtil
    {
        // 默认不销毁模式 是否切场景不销毁模式
        public const bool IsDontDestroyOnLoad = true;

        public static void SetDontDestroyOnLoad(GameObject go)
        {
            if (IsDontDestroyOnLoad)
            {
                UObject.DontDestroyOnLoad(go);
            }
        }

        public static void SetDontDestroyOnLoad(Component com)
        {
            if (IsDontDestroyOnLoad)
            {
                UObject.DontDestroyOnLoad(com);
            }
        }
    }
}