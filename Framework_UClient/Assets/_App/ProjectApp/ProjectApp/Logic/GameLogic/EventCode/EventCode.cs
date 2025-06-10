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
using ProjectApp.Data;

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

        protected virtual bool IsFullRole(List<RoleKey> allRolePot)
        {
            foreach (var item in allRolePot)
            {
                if (item == RoleKey.Node) return false;
            }

            return true;
        }

        public abstract void UpdataState(BaseSceneEvent sceneEvent, Dictionary<RoleKey, IRole> roles);

        protected bool GetRoleDead(IRole role)
        {
            if (role.State.GetState(StateKey.Dead, out StateData stateData))
            {
                return true;
            }
            return false;
        }

        protected bool SetRolesStateEvent_Dead(IRole role_0, IRole role_1, ref RoleStateToEvent state_0, ref RoleStateToEvent state_1)
        {
            if (role_0 == null || role_1 == null) return false;

            bool isDead_0 = GetRoleDead(role_0);
            bool isDead_1 = GetRoleDead(role_1);
            if (isDead_0 || isDead_1)
            {
                if (isDead_0 && isDead_1)
                {
                    state_0 = RoleStateToEvent.幽灵;
                    state_1 = RoleStateToEvent.幽灵;
                }
                else if (isDead_0)
                {
                    state_0 = RoleStateToEvent.幽灵;
                    state_1 = RoleStateToEvent.惊吓;
                }
                else if (isDead_1)
                {
                    state_0 = RoleStateToEvent.惊吓;
                    state_1 = RoleStateToEvent.幽灵;
                }

                return true;
            }
            return false;
        }

        public abstract bool CheckConditionFinish(CheckConditionVO condition, RoleState roleState);
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

        public override void UpdataState(BaseSceneEvent sceneEvent, Dictionary<RoleKey, IRole> roles)
        {

        }

        public override bool CheckConditionFinish(CheckConditionVO condition, RoleState roleState)
        {
            return true;
        }

    }


}