/****************************************************
    文件：TransformExtend.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/2/23 18:50
    功能：Transform组件扩展
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public static class TransformExtend
    {

        public static Transform FindTransformByName(this Transform trf, string name)
        {
            if (trf == null)
            {
                Debug.LogError("空物体不能查找");
                return null;
            }
            Transform target = trf.name == name ? trf : null;
            if (target != null) return target;
            for (int i = 0; i < trf.childCount; i++)
            {
                Transform childtr = trf.GetChild(i);
                target = childtr.name == name ? childtr : null;
            }
            if (target != null) return target;
            for (int i = 0; i < trf.childCount; i++)
            {
                target = FindTransformByName(trf.GetChild(i), name);
                if (target != null) return target;
            }
            return null;
        }
        public static List<Transform> FindTransformsByTag(this Transform trf, string Tag)
        {
            List<Transform> transforms = new List<Transform>();
            foreach (var item in trf.GetComponentsInChildren<Transform>())
            {
                if (item.tag == Tag)
                {
                    transforms.Add(item);
                }
            }
            return transforms;
        }
        public static List<Transform> FindTransformsByLayer(this Transform trf, int LayerIndex)
        {
            List<Transform> transforms = new List<Transform>();
            foreach (var item in trf.GetComponentsInChildren<Transform>())
            {
                if (item.gameObject.layer == LayerIndex)
                {
                    transforms.Add(item);
                }
            }
            return transforms;
        }
        public static T Find<T>(this T[] ts, Func<T, bool> func) where T : class
        {
            foreach (var item in ts)
            {
                if (func(item))
                {
                    return item;
                }
            }

            return null;
        }

       



    }
}
