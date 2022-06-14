/****************************************************************************
* ScriptType: 主框架
* 请勿修改!!!
****************************************************************************/

//#define UNITY_EDITOR

using System.Collections.Generic;
using FuturePlugin;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace FutureCore
{
    /// <summary>
    /// unity通用
    /// </summary>
    public static class EngineUtil
    {
        #region Engine
        
        public static void Destroy(UObject obj)
        {
            if (obj)
            {
                UObject.Destroy(obj);
            }
        }

        public static void Destroy(UObject obj, float time)
        {
            if (obj)
            {
                UObject.Destroy(obj, time);
            }
        }

        public static bool CompareTag(GameObject gameObject, string tag)
        {
            return gameObject.CompareTag(tag);
        }

        public static void SetDontDestroyOnLoad(GameObject go)
        {
            UObject.DontDestroyOnLoad(go);
        }

        public static int GetInstanceID(UObject uObject)
        {
            return uObject.GetInstanceID();
        }

        public static int GetHashCode(UObject uObject)
        {
            return uObject.GetHashCode();
        }
        #endregion

        #region Func
        public static T GetComponentOrAdd<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (component)
            {
                return component;
            }
            return go.AddComponent<T>();
        }

        public static T GetComponentOrAdd<T>(Component com) where T : Component
        {
            return GetComponentOrAdd<T>(com.gameObject);
        }

        public static GameObject RecursionFind(GameObject go, string name)
        {
            if (go.name == name)
            {
                return go;
            }
            foreach (Transform child in go.transform)
            {
                go = RecursionFind(child.gameObject, name);
                if (go)
                {
                    return go;
                }
            }
            return null;
        }

        public static Transform RecursionFind(Transform tf, string name)
        {
            if (tf.name == name)
            {
                return tf;
            }
            foreach (Transform child in tf)
            {
                tf = RecursionFind(child, name);
                if (tf)
                {
                    return tf;
                }
            }
            return null;
        }

        public static void RecursionFindTransforms(List<string> tarNames, Transform tran, ref List<Transform> trans)
        {
            if (tarNames.Contains(tran.name))
            {
                trans.Add(tran);
            }
            if (trans.Count == tarNames.Count)
            {
                return;
            }
            for (int i = 0; i < tran.childCount; i++)
            {
                RecursionFindTransforms(tarNames, tran.GetChild(i), ref trans);
            }
        }

        public static void SetSortingOrder(GameObject gameObject, int sortingOrder)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.sortingOrder = sortingOrder;
            }
        }

        public static SpriteRenderer CreateSpriteGameObject(Sprite sprite, int sortOrder = 0)
        {
            GameObject gameObject = new GameObject(sprite.name);
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortOrder;
            return spriteRenderer;
        }

        public static GameObject CreatePrimitiveObject(PrimitiveType type)
        {
            return GameObject.CreatePrimitive(type);
        }

        //public static float GetAduioTrackTime(AudioSource audioSource)
        //{
        //    return 1f * audioSource.timeSamples / audioSource.clip.frequency;
        //}

        public static void ForceCrash()
        {
            UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.Abort);
        }

        public static void GetSpriteOuterUV(Sprite activeSprite)
        {
            //Sprites.DataUtility.GetOuterUV(activeSprite);
        }
        #endregion
    }
}