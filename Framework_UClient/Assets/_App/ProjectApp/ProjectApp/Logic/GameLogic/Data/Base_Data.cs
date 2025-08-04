/****************************************************
    文件: Base_Data.cs
    作者: Clear
    日期: 2024/8/13 17:43:36
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using System;
using UnityEngine;

namespace ProjectApp
{
    [Serializable]
    public abstract class Base_Data
    {
        public abstract string Desc { get; }
        public DragEntityType Type;

        public T GetData<T>() where T : Base_Data
        {
            return this as T;
        }
    }

    public enum GameEvent : uint
    {
        //关卡完成
        GameFinish = 0,
        //将事件设置进画面
        SetEvent,
        //将角色设置进事件
        SetRole,
        //将角色从事件中移除
        RemoveRole,
        //将事件从画面移除
        RemoveEvent,


    }



    /// <summary>
    /// 拖拽实体类型
    /// </summary>
    public enum DragEntityType
    {
        Role,
        Event
    }









    /// <summary>
    /// 角色唯一键
    /// </summary>
    public enum RoleKey
    {
        /// <summary>
        /// 无角色
        /// </summary>
        Node = 0,

        /// <summary>
        /// 伏羲
        /// </summary>
        Fuxi,
        /// <summary>
        /// 女娲
        /// </summary>
        Nuwa,
    }

    /// <summary>
    /// 事件唯一键
    /// </summary>
    public enum EventKey
    {
        ///空
        Empty = 0,
        /// <summary>
        /// 相爱
        /// </summary>
        Love,
        /// <summary>
        /// 死亡
        /// </summary>
        Dead,


    }

}