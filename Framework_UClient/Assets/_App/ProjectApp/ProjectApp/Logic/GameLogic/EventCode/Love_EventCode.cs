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
using ProjectApp.Data;
using FutureCore;

namespace ProjectApp
{

    public class Love_EventCode : EventCode
    {



        public override void Init()
        {
            Key = EventKey.Love;
        }



        public override void RunEvent(int pictureIndex, Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles, List<RoleKey> allRolePot)
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

            bool role_0_OK = CanAddLoveState(role_0, roleKey_1, out bool isNo_0);
            bool role_1_OK = CanAddLoveState(role_1, roleKey_0, out bool isNo_1);


            if (role_0_OK && role_1_OK)
            {
                // if (!roleState_0.GetState(StateKey.Love, out StateData data_0))
                // {
                //     data_0.dataList_RoleKey = new List<RoleKey>(GameLogicTool.GetGameStructure_RoleCount());
                //     data_0.dataList_RoleKey2 = new List<RoleKey>(GameLogicTool.GetGameStructure_RoleCount());

                // }
                // data_0.dataList_RoleKey.Add(roleKey_1);// 加入正在爱列表
                // data_0.dataList_RoleKey2.Add(roleKey_1);// 加入爱过列表 
                roleState_0.AddRunEvent(pictureIndex, RunEventKey.Love, (int)roleKey_1);
                roleState_0.AddState(RoleStateKey.相爱, (int)role_1.Key);

                // if (!roleState_1.GetState(StateKey.Love, out StateData data_1))
                // {
                //     data_1.dataList_RoleKey = new List<RoleKey>(GameLogicTool.GetGameStructure_RoleCount());
                //     data_1.dataList_RoleKey2 = new List<RoleKey>(GameLogicTool.GetGameStructure_RoleCount());
                // }
                // data_1.dataList_RoleKey.Add(roleKey_0); // 加入正在爱列表
                // data_1.dataList_RoleKey2.Add(roleKey_0); // 加入爱过列表 
                roleState_1.AddRunEvent(pictureIndex, RunEventKey.Love, (int)roleKey_0);
                roleState_1.AddState(RoleStateKey.相爱, (int)role_0.Key);



            }
            else
            {

                if (isNo_0 && isNo_1)
                {
                    //同时给与对方拒绝
                    roleState_0.AddState(RoleStateKey.拒绝, (int)role_1.Key);
                    roleState_1.AddState(RoleStateKey.拒绝, (int)role_0.Key);

                }
                else
                {
                    if (isNo_0)
                    {
                        roleState_0.AddState(RoleStateKey.拒绝, (int)role_1.Key);

                        roleState_1.AddState(RoleStateKey.心碎, (int)role_0.Key);

                    }
                    else
                    if (isNo_1)
                    {
                        roleState_0.AddState(RoleStateKey.心碎, (int)role_1.Key);
                        
                        roleState_1.AddState(RoleStateKey.拒绝, (int)role_0.Key);

                    }

                }


            }




        }

        /// <summary>
        /// 能否添加相爱状态
        /// 
        /// HisLove: 是否是拒绝对方
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CanAddLoveState(IRole role, RoleKey checkRoleKey, out bool HisLove)
        {
            HisLove = false;

            if (GetRoleDead(role))
            {
                return false;
            }

            if (role.GetLabelKey(RoleLabelStaticKey.CanLove))
            {
                if (role.GetLabelKey(RoleLabelStaticKey.Passionate))
                {
                    return true;
                }

                bool isLove = true;

                foreach (var stateData in role.State.GetAllRunEvents())
                {

                    if (stateData.Value.stateKey == RunEventKey.Love && stateData.Value.data_int != (int)checkRoleKey)
                    {
                        isLove = false;
                        //给与拒绝
                        HisLove = true;
                    }

                    if (stateData.Value.stateKey == RunEventKey.LoveDead)
                    {
                        isLove = true;
                        HisLove = false;

                    }


                }

                return isLove;
            }
            return false;
        }



        public override void UpdataState(BaseSceneEvent sceneEvent, Dictionary<RoleKey, IRole> roles)
        {


            bool isShow = false;

            RoleStateKey eventState_0 = RoleStateKey.待机;
            RoleStateKey eventState_1 = RoleStateKey.待机;
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


                    bool isLove = GetIsLoveTo(role_0, role_1);
                    if (isLove)
                    {
                        //成功
                        isShow = true;
                        eventState_0 = RoleStateKey.相爱;
                        eventState_1 = RoleStateKey.相爱;
                    }
                    else
                    {




                    }





                }


                // sceneEvent.SetRoleEventState(0, eventState_0);
                // sceneEvent.SetRoleEventState(1, eventState_1);
            }

            eventState_0 = role_0.GetShowState();


            sceneEvent.OnShowView(isShow ? 1 : 0);






        }





        private bool GetIsLoveTo(IRole role_0, IRole role_1)
        {
            bool isLove = false;

            if (role_0.State.GetState(RoleStateKey.相爱, out List<int> datas))
            {
                foreach (var data in datas)
                {
                    if (data == (int)role_1.Key)
                    {
                        isLove = true;
                        break;
                    }

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


        public override bool CheckConditionFinish(CheckConditionVO condition, RoleState roleState)
        {
            switch ((CheckType)condition.eventCheckType)
            {

                case CheckType.isLove:
                    {
                        return GetIsLove(roleState, (RoleKey)condition.data);
                    }
                case CheckType.isLoved:
                    {
                        return GetIsLoved(roleState, (RoleKey)condition.data);
                    }
                default:

                    LogUtil.LogError("没有这种检测：" + Key + "  检测类型：" + (CheckType)condition.eventCheckType);
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
            // if (roleState.GetState(StateKey.Love, out StateData stateData_0))
            // {
            //     if (role == RoleKey.Node)
            //     {
            //         return stateData_0.dataList_RoleKey.Count >= 1;
            //     }

            //     if (stateData_0.dataList_RoleKey.Contains(role))
            //     {
            //         return true;

            //     }
            // }

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
            // if (roleState.GetState(StateKey.Love, out StateData stateData_0))
            // {


            //     if (role == RoleKey.Node)
            //     {
            //         return stateData_0.dataList_RoleKey2.Count >= 1;
            //     }

            //     if (stateData_0.dataList_RoleKey2.Contains(role))
            //     {
            //         return true;

            //     }
            // }

            return false;



        }

        #endregion


    }

}