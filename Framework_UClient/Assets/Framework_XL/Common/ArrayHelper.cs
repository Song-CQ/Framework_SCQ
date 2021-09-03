/****************************************************
    文件：ArrayHelper.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/2/24 20:41
    功能：数组助手类
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace XL.Common
{
    public static class ArrayHelper
    {

        /// <summary>
        /// 查找满足条件的单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Find<T>(this T[] array,Func<T,bool> func)where T:class
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (func(array[i]))
                {
                    return array[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 查找满足条件的对象组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] array, Func<T, bool> func) where T : class
        {
            List<T> arrLst= new List<T>();

            for (int i = 0; i < array.Length; i++)
            {
                if (func(array[i]))
                {
                    arrLst.Add(arrLst[i]);
                }
            }
            return arrLst.ToArray(); ;
        }
    }
}
