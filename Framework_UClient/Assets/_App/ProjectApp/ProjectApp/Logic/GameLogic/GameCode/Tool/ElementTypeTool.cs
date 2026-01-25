using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public enum ElementType
    {
        Item_A = 1,    // 红色基础元素
        Item_B, // 黄色基础元素
        Item_C,   // 蓝色基础元素
        Item_D,  // 绿色基础元素

        Item_Change,// 可变换元素


        // 可切换的特殊元素
        Prop_Rocket,           // 火箭组合（横竖可切换）
        RocketBombCombo,       // 火箭炸弹组合
        DoubleRocket,          // 双火箭（可切换方向）
        CrossRocket,           // 十字火箭
        MegaBomb,              // 超级炸弹（可切换模式）

        // 状态相关
        Transformable,         // 可变换元素


        Dummy_CanMatche = 100,// 站位 比他小的允许参与匹配消除

        //道具
        Prop_HorizontalRocket,      // 横向火箭
        VerticalRocket,        // 纵向火箭
        Bomb,                  // 炸弹
        ColorBomb,             // 彩色炸弹/万能元素

        Dummy_CanClickEvent = 200, // 占位 比他小的接受点击
        

        //不可下落
        Dummy_CanDown = 900,//占位 比他大的不可下落


        //不可点击
        Fixed_Special = 1000, // 空位标记
        Fixed_None = 2000,//该方格被禁用方格 一般是地形 后面可能会有可破坏地形

    }
    public static class ElementTypeTool
    {

        /// <summary>
        /// 该元素是否要显示图片
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool CheckType_HasIcon(ElementType type)
        {
            switch(type)
            {
                case ElementType.Dummy_CanMatche:
                case ElementType.Dummy_CanClickEvent:
                case ElementType.Dummy_CanDown:
                case ElementType.Fixed_Special:
                case ElementType.Fixed_None:
                return false;
            }
            return true;
        }
        /// <summary>
        /// 该元素是否可下落
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CheckType_FillEmpty(ElementType type)
        {

            if (type >= ElementType.Dummy_CanDown)
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
            if (type == ElementType.Fixed_Special)
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
            if( type < ElementType.Dummy_CanMatche)
            {
                return true;
            }

            return false;
            
        }

        /// <summary>
        /// 该元素是否接受 点击事件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool CheckType_ClickEvent(ElementType type)
        {
            if (type< ElementType.Dummy_CanClickEvent)
            {
                return true;
            }
            return false;
        }

        public static ElementType GetTypeToElementData(ElementData data)
        {
            if (data.Type == ElementType.Item_Change)
            {
                return (ElementType)data.data1;
            }
            return data.Type;
        }
    }
}
