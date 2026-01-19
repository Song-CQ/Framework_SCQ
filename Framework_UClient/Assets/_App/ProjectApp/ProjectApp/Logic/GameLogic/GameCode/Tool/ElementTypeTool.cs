using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public enum ElementType
    {
        Baihe,    // 红色基础元素
        fengjiao, // 黄色基础元素
        shuihu,   // 蓝色基础元素
        xihongshui,  // 绿色基础元素
        zhiwu,  // 紫色基础元素

        CanMatche = 100,// 站位比他小的允许参与匹配消除

        //不可下落
        NoDown = 900,//占位 比他大的不可下落


        //不可点击
        Special = 1000, // 空位标记
        None = 2000,//该方格被禁用方格 一般是地形 后面可能会有可破坏地形

    }
    public static class ElementTypeTool
    {
        /// <summary>
        /// 该元素是否可下落
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CheckType_FillEmpty(ElementType type)
        {

            if (type >= ElementType.NoDown)
            {
                return false;
            }
            return true;


        }

        /// <summary>
        /// 该元素 下落通道是否畅通 比如 空元素就允许
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

        public static bool CheckType_UpEmpty(ElementType type)
        {
            if (type == ElementType.Special)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 该元素是否 可 匹配
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CheckType_CanMatches(ElementType type)
        {
            if( type < ElementType.CanMatche)
            {
                return true;
            }

            return false;
            
        }
    }
}
