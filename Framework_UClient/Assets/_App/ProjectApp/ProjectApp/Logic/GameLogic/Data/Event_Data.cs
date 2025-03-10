/****************************************************
    文件: Event_Data.cs
    作者: Clear
    日期: 2024/8/13 17:28:16
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using ProjectApp.Data;
using System;
using UnityEngine;

namespace ProjectApp
{
    public class Event_Data : Base_Data
    {
        public override string Desc => VO.desc;

        public EventKey Key { private set; get; }
        public int RoleSum => VO.roleSum;

        public EventDataVO VO { private set; get; }

        public Event_Data(EventKey key)
        {
            Key = key;
            VO = ConfigDataMgr.Instance.GetConfigVO<EventDataVO>( ConfigVO.EventData, key.ToString());
        }

    }


    public struct RolePot
    {
        public RoleKey pot;


    }
}