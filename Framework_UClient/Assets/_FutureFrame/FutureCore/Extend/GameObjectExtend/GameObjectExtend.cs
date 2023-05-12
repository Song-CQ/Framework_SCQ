using System.Text;
using UnityEngine;

namespace FutureCore
{
    public static class GameObjectExtend
    {
        public static bool IsDestroyed(this Object obj)
        {
            if (obj == null || obj.Equals(null))
            {
                return true;
            }
            return false;
        }
        public static void Destroy(this Object obj, float t = 0)
        {
            if (t == 0)
            {
                Object.Destroy(obj, t);
            }
            else
            {
                Object.Destroy(obj);
            }
            
        }

        public static void SetParent(this GameObject gameObject, GameObject parentGo, bool worldPositionStays = false)
        {
            if (parentGo)
            {
                // worldPositionStays默认: 局部坐标与原始坐标保存不变
                gameObject.transform.SetParent(parentGo.transform, worldPositionStays);
            }
        }

        public static void SetParent(this GameObject gameObject, Transform parentTf, bool worldPositionStays = false)
        {
            if (parentTf)
            {
                // worldPositionStays默认: 局部坐标与原始坐标保存不变
                gameObject.transform.SetParent(parentTf, worldPositionStays);
            }
        }

        public static void SetLayer(this GameObject gameObject, string layerName)
        {
            Transform[] transArr = gameObject.transform.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transArr.Length; i++)
            {
                transArr[i].gameObject.layer = LayerMask.NameToLayer(layerName);
            }
        }

        public static void SetLayer(this GameObject gameObject, LayerMask layer)
        {
            Transform[] transArr = gameObject.transform.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transArr.Length; i++)
            {
                transArr[i].gameObject.layer = layer;
            }
        }

        public static void DestroyAllChild(this GameObject gameObject)
        {
            foreach (Transform child in gameObject.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        public static void SetActive<T>(this T t, bool active = true) where T : Component
        {
            t.gameObject.SetActive(active);
        }

        /// <summary>
        /// 获得物体在Hierarchy面板上的路径
        /// </summary>
        public static string GetGameObjectPath(this GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            StringBuilder sb = new StringBuilder();
            sb.Append(transform.name);
            while (transform.parent != null)
            {
                transform = transform.parent;
                sb.Insert(0, transform.name + "/");
            }
            return sb.ToString();
        }

       

    }
}