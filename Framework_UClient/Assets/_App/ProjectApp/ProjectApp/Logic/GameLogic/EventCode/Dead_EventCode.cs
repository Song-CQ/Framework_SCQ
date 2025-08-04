/****************************************************
    文件: Daed_EventCode.cs
    作者: Clear
    日期: 2025/6/24 11:7:17
    类型: 逻辑脚本
    功能: 死亡事件
*****************************************************/
using System;
using System.Collections.Generic;
using ProjectApp.Data;

namespace ProjectApp
{
    public class Dead_EventCode : EventCode
    {


        public override void Init()
        {
            Key = EventKey.Dead;
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

            bool role_1_Dead = CanAddDeadState(role_1);
            if (role_1_Dead)
            {
                roleState_1.AddRunEvent(pictureIndex, RunEventKey.Dead);

                if (roleState_0.GetRunEvent(RunEventKey.Love, out RunEventData[] datas))
                {
                    foreach (var data in datas)
                    {
                        if (data.data_int == (int)roleKey_0)
                        { 
                            roleState_0.AddRunEvent(pictureIndex, RunEventKey.LoveDead,(int)roleKey_0);
                        }
                        
                    }


                }

            }
            
  
           
        }

        private bool CanAddDeadState(IRole role_1)
        {
            return true;
        }



        public override void UpdataState(BaseSceneEvent sceneEvent, Dictionary<RoleKey, IRole> roles)        
        {

            foreach (var role in roles) 
            {
                // if (role.Value.State)
                // { 
 
                // }


            }

            
 
        }
        

        public override bool CheckConditionFinish(CheckConditionVO condition, RoleState roleState)
        {
            return false;   
        }
    }
} 