/****************************************************
    文件: EventCode.cs
    作者: Clear
    日期: 2024/8/20 16:56:47
    类型: 逻辑脚本
    功能: 事件核心
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public abstract class EventCode
    {
        public EventKey Key;

        public EventCode()
        {
            Init();
        }
        public abstract void Init();
        
        public abstract void RunEvent(Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles, List<RoleKey> allRolePot);

        public abstract void RefreshView(Dictionary<RoleKey, IRole> roles, List<RoleKey> allRolePot);
    }

    public class Empty_EventCode : EventCode
    {

        public override void Init()
        {
            Key = EventKey.Empty;
        }

        public override void RunEvent(Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles, List<RoleKey> allRolePot)
        {
            Debug.Log("运行事件：" + Key.ToString());
        }

        public override void RefreshView(Dictionary<RoleKey, IRole> roles, List<RoleKey> allRolePot)
        {
          
        }

        
    }

    public class Love_EventCode : EventCode
    {

        public enum ShowView
        {
            None=0,
            Love=1

        }

        public override void Init()
        {
            Key = EventKey.Love;
        }

       

        public override void RunEvent(Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles, List<RoleKey> allRolePot)
        {

            RoleKey roleKey_0 = allRolePot[0];
            RoleKey roleKey_1 = allRolePot[1];

            RoleState roleState_0 = allRoleState[roleKey_0];
            IRole role_0 = roles[roleKey_0];

            RoleState roleState_1 = allRoleState[roleKey_1];
            IRole role_1 = roles[roleKey_1];




        }

        public override void RefreshView(Dictionary<RoleKey, IRole> roles, List<RoleKey> allRolePot)
        {
            


        }

    }

}