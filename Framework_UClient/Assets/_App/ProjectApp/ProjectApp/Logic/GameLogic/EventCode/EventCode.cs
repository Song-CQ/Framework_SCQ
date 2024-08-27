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


        public abstract void RefreshView(BaseSceneEvent sceneEvent, Dictionary<RoleKey, IRole> roles);
        
        protected bool GetRoleDead(IRole role)
        {
            if (role.State.GetState(StateKey.Dead, out StateData stateData))
            {
                return true;
            }
            return false;
        }

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

        public override void RefreshView(BaseSceneEvent sceneEvent, Dictionary<RoleKey, IRole> roles)
        {
          
        }
    }

    public class Love_EventCode : EventCode
    {

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

            bool role_0_OK = CanAddLoveState(role_0, roleState_0);
            bool role_1_OK = CanAddLoveState(role_1, roleState_1);


            if (role_0_OK&&role_1_OK)
            {
                if (!roleState_0.GetState(StateKey.Love, out StateData data_0))
                {
                    data_0.dataList_RoleKey = new List<RoleKey>(GameLogicTool.GetGameStructure_RoleCount());

                }
                data_0.dataList_RoleKey.Add(roleKey_1);

                roleState_0.SetState(StateKey.Love, data_0);

            }




        }

        /// <summary>
        /// 能否添加相爱状态
        /// </summary>
        /// <returns></returns>
        private bool CanAddLoveState(IRole role, RoleState roleState)
        {
     
            if (GetRoleDead(role))
            {
                return false;
            }

            if (role.GetLabelKey(LabelKey.CanLove))
            {
                if (role.GetLabelKey(LabelKey.Passionate))
                {
                    return true;
                }
                       

                bool isLove = roleState.GetState(StateKey.Love,out data);

                if (!isLove)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }



        public override void RefreshView(BaseSceneEvent sceneEvent, Dictionary<RoleKey, IRole> roles)
        {
            bool isShow = false;

            List<RoleKey> allPots = sceneEvent.AllRolePot;

            if (allPots[0] == RoleKey.Node || allPots[1] == RoleKey.Node)
            {

            }
            else 
            {
                IRole role_0 = roles[allPots[0]];
                IRole role_1 = roles[allPots[1]];

             
                bool isLove_0 = GetIsLoveTo(role_0, role_1);
                bool isLove_1 = GetIsLoveTo(role_1, role_0);





                if (isLove_0 && isLove_1)
                {
                    //成功
                    isShow = true;
                }

            }

            if (isShow)
            {

            }
            else
            {
                
            }
            


        }

        


        private bool GetIsLoveTo(IRole role_0, IRole role_1)
        {
            bool isLove = false;

            if (role_0.State.GetState(StateKey.Love, out StateData stateData_0))
            {
                RoleKey love_Role = (RoleKey)stateData_0.data_0;

                if (love_Role == role_1.Key)
                {
                    isLove = true;

                }


            }

            return isLove;

        }

    }

}