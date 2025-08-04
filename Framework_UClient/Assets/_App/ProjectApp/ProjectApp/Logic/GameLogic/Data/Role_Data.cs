/****************************************************
    文件: RoleData.cs
    作者: Clear
    日期: 2024/8/13 17:5:21
    类型: 逻辑脚本
    功能: 人物数据
*****************************************************/
using System;
using System.Collections.Generic;
using ProjectApp.Data;
using UnityEngine;

namespace ProjectApp
{
    [Serializable]
    public class Role_Data : Base_Data
    {
        public override string Desc => Key.ToString();

        public RoleKey Key;

        public List<int> allLabelId = new List<int>();
        
        public RoleDataVO VO { private set; get; }

        public Role_Data(int roleId)
        {
            VO = ConfigDataMgr.Instance.GetConfigVO<RoleDataVO>(ConfigVO.RoleData, roleId);
            
            Key = GameSys.GetRoleKey(VO.Rolekey);

            var _rolekey = ConfigDataMgr.Instance.GetConfigVO<RoleKeyVO>(ConfigVO.RoleKey, (int)Key);




            


            
        }


    }
}