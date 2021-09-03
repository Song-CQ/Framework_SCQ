/****************************************************
    文件：TransformHelper.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/2/23 18:50
    功能：Transform组件变换
*****************************************************/
using UnityEngine;

namespace XL.Common
{
    public static class TransformHelper
    { 

        public static Transform FindTransformByName(this Transform trf,string name)
        {
            Transform target = trf.Find(name);
            if (target != null) return target;
            for (int i = 0; i < trf.childCount; i++)
            {
                target = FindTransformByName(trf.GetChild(i),name);
                if (target != null) return target;
            }
            return null;
        }

    }
}
