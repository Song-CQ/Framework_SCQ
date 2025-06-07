/****************************************************
    文件: EventCode.cs
    作者: Clear
    日期: 2024/8/20 16:56:47
    类型: 逻辑脚本
    功能: 相爱事件
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{

    public class Love_EventCode : EventCode
    {



        public override void Init()
        {
            Key = EventKey.Love;
        }



        public override void RunEvent(Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles, List<RoleKey> allRolePot)
        {

            if (!IsFullRole(allRolePot))
            {
                return;
            }

            RoleKey roleKey_0 = allRolePot[0];
            RoleKey roleKey_1 = allRolePot[1];

            RoleState roleState_0 = allRoleState[roleKey_0];
            IRole role_0 = roles[roleKey_0];

            RoleState roleState_1 = allRoleState[roleKey_1];
            IRole role_1 = roles[roleKey_1];

            bool role_0_OK = CanAddLoveState(role_0, roleState_0);
            bool role_1_OK = CanAddLoveState(role_1, roleState_1);


            if (role_0_OK && role_1_OK)
            {
                if (!roleState_0.GetState(StateKey.Love, out StateData data_0))
                {
                    data_0.dataList_RoleKey = new List<RoleKey>(GameLogicTool.GetGameStructure_RoleCount());

                }
                data_0.dataList_RoleKey.Add(roleKey_1);
                roleState_0.SetState(StateKey.Love, data_0);

                if (!roleState_1.GetState(StateKey.Love, out StateData data_1))
                {
                    data_1.dataList_RoleKey = new List<RoleKey>(GameLogicTool.GetGameStructure_RoleCount());

                }
                data_1.dataList_RoleKey.Add(roleKey_0);
                roleState_1.SetState(StateKey.Love, data_1);

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


                bool isLove = roleState.GetState(StateKey.Love);

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



        public override void UpdataState(BaseSceneEvent sceneEvent, Dictionary<RoleKey, IRole> roles)
        {


            bool isShow = false;

            RoleStateToEvent eventState_0 = RoleStateToEvent.待机;
            RoleStateToEvent eventState_1 = RoleStateToEvent.待机;
            IRole role_0 = null;
            IRole role_1 = null;
            if (IsFullRole(sceneEvent.AllRolePot))
            {

                List<RoleKey> allPots = sceneEvent.AllRolePot;

                role_0 = roles[allPots[0]];
                role_1 = roles[allPots[1]];

                if (allPots[0] != RoleKey.Node && allPots[1] != RoleKey.Node)
                {
                    role_0 = roles[allPots[0]];
                    role_1 = roles[allPots[1]];


                    bool isLove_0 = GetIsLoveTo(role_0, role_1);
                    bool isLove_1 = GetIsLoveTo(role_1, role_0);


                    if (isLove_0 && isLove_1)
                    {
                        //成功
                        isShow = true;
                    }
                }

                if (role_0 != null && role_1 != null)
                {
                    if (isShow)
                    {
                        //满足条件设置相爱状态
                        eventState_0 = RoleStateToEvent.相爱;
                        eventState_1 = RoleStateToEvent.相爱;

                    }
                    else
                    {
                        GetRoleStateToEvent(role_0, role_1, ref eventState_0, ref eventState_1);

                    }
                }

                sceneEvent.SetRoleEventState(0, eventState_0);
                sceneEvent.SetRoleEventState(1, eventState_1);
            }



            sceneEvent.OnShowView(isShow ? 1 : 0);






        }

        private void GetRoleStateToEvent(IRole role_0, IRole role_1, ref RoleStateToEvent eventState_0, ref RoleStateToEvent eventState_1)
        {

            //判断是否有死亡
            if (SetRolesStateEvent_Dead(role_0, role_1, ref eventState_0, ref eventState_1)) return;






        }



        private bool GetIsLoveTo(IRole role_0, IRole role_1)
        {
            bool isLove = false;

            if (role_0.State.GetState(StateKey.Love, out StateData stateData_0))
            {

                if (stateData_0.dataList_RoleKey.Contains(role_1.Key))
                {
                    isLove = true;

                }
            }

            return isLove;

        }






        #region CheckCondition
        private enum CheckType
        {
            isLove = 0,//是否正在爱
            isLoved = 1,//是否曾经爱过


        }


        public override bool CheckConditionFinish(CheckCondition condition, RoleState roleState)
        {
            switch ((CheckType)condition.checkType)
            {

                case CheckType.isLove:
                    {
                        return GetIsLove(roleState,(RoleKey)condition.data);
                    }
                case CheckType.isLoved:
                    {
                        return GetIsLoved(roleState,(RoleKey)condition.data);
                    }

                default:

                    LogUtil.LogError("没有这种检测：" + Key + "  检测类型：" + (CheckType)condition.checkType);
                    return false;

            }





        }

        /// <summary>
        /// 是否正在喜欢
        /// </summary>
        /// <param name="roleState"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private bool GetIsLove(RoleState roleState, RoleKey role)
        {
            if (roleState.GetState(StateKey.Love, out StateData stateData_0))
            {
                if (role == RoleKey.Node)
                {
                    return stateData_0.dataList_RoleKey.Count >= 1;
                }

                if (stateData_0.dataList_RoleKey.Contains(role))
                {
                    return true;

                }
            }

            return false;



        }

        /// <summary>
        /// 是否曾经喜欢过
        /// </summary>
        /// <param name="roleState"></param>
        /// <param name="role"></param>
        /// <returns></returns>

        private bool GetIsLoved(RoleState roleState, RoleKey role)
        {
            if (roleState.GetState(StateKey.Love, out StateData stateData_0))
            {
                if (role == RoleKey.Node)
                {
                    return stateData_0.dataList_RoleKey2.Count >= 1;
                }

                if (stateData_0.dataList_RoleKey2.Contains(role))
                {
                    return true;

                }
            }

            return false;



        }

        #endregion


    }

}