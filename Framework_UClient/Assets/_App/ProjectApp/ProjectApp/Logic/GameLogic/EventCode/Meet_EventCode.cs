/****************************************************
    文件: EventCode.cs
    作者: Clear
    日期: 2024/8/20 16:56:47
    类型: 逻辑脚本
    功能: 遇见事件
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using ProjectApp.Data;

namespace ProjectApp
{

    public class Meet_EventCode : EventCode
    {



        public override void Init()
        {
            Key = EventKey.Empty;
        }



        public override void RunEvent(int pictureIndex,Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles, List<RoleKey> allRolePot)
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


            // StateData data = StateData.Idle;



            // var allState = roleState_0.GetStateDic();

            // for (int i = 0; i < pictureIndex; i++)
            // {
            //     if (allState.ContainsKey(i))
            //     {
            //         StateData stateData = allState[i];


            //         if (stateData.stateKey == StateKey.死亡)
            //         {
            //             data = new StateData(StateKey.幽灵);
            //         }
            //         else
            //         {
            //             if (stateData.stateKey == StateKey.幽灵)
            //             { 

            //             }

            //         }
                    
                    

            //     }

            // }

            
            




           





        }


        public override void UpdataState(BaseSceneEvent sceneEvent, Dictionary<RoleKey, IRole> roles)
        {


            bool isShow = false;

            foreach (var item in roles)
            {
                var state = item.Value.State;

                state.GetCurrView();


            }





            sceneEvent.OnShowView(isShow ? 1 : 0);






        }






        #region CheckCondition
        private enum CheckType
        {
            isMeet = 0,//是否遇见
        }


        public override bool CheckConditionFinish(CheckConditionVO condition, RoleState roleState)
        {

            return GetIsMeet(roleState, (RoleKey)condition.data);

        }

        /// <summary>
        /// 是否遇见过
        /// </summary>
        /// <param name="roleState"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private bool GetIsMeet(RoleState roleState, RoleKey role)
        {
            // if (roleState.GetState(EventKey.Love, out StateData stateData_0))
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


        #endregion


    }

}