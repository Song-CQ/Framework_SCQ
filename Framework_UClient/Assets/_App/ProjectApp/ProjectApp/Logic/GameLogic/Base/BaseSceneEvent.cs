/****************************************************
    文件: BaseSceneEntity.cs
    作者: Clear
    日期: 2023/12/6 11:25:12
    类型: 逻辑脚本
    功能: 基础场景事件
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class BaseSceneEvent : ISceneEvent
    {
        private Event_Data data;
        private EventCode code;

        public List<RoleKey> AllRolePot { private set; get; }    

        public EventKey Key { get => data.key; }

        public List<int> eventStateList = new List<int>();

        protected IPicture picture;
        public virtual void Init(Event_Data _data,EventCode eventCode)
        {
            data = _data;
            code = eventCode;


            for (int i = 0; i < data.RoleSum; i++)
            {
                AllRolePot.Add(RoleKey.Node);
            }

        }

        private int GetNullRolePot()
        {
            for (int i = 0; i < AllRolePot.Count; i++)
            {
                if (AllRolePot[i] ==  RoleKey.Node)
                {
                    return i;
                }
            }
            return AllRolePot.Count - 1;
        }

        public void SetRole(int index, RoleKey key)
        {

            AllRolePot[index] = key;
        }

        public void RemoveRole(RoleKey key)
        {
            int index = GetIndexToRoleKey(key);

            AllRolePot[index] = RoleKey.Node;
        }

        private int GetIndexToRoleKey(RoleKey key)
        {
            if (key == RoleKey.Node)
            {
                return -1;
            }

            for (int i = 0; i < AllRolePot.Count; i++)
            {
                if (key == AllRolePot[i])
                {
                    return i;
                }
            }

            return -1;

        }


        public bool AddRole(RoleKey role, int index = -1)
        {
            if (index == -1)
            {
                index = GetNullRolePot();
            }

            AllRolePot[index] = role;
            return true;
        }

        public bool RemoveRole(int index)
        {
            if (index < AllRolePot.Count)
            {
                if (AllRolePot[index] !=  RoleKey.Node)
                {
                    AllRolePot[index] = RoleKey.Node;

                    return true;
                }

            }
            return false;
        }


        public virtual void EnterPicture(IPicture picture)
        {
            Debug.Log("Show场景");
            this.picture = picture;
        }

        public virtual void ExitPicture(IPicture picture)
        {
            Debug.Log("Exit场景");

            //回收
            this.picture = null;
            AllRolePot.Clear();
        }

        public void Run(Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles)
        {

            code.RunEvent(allRoleState, roles, AllRolePot);

            foreach (var role in roles)
            {
                RoleState state = allRoleState[role.Key];
                //将运行出来的角色数据复制到事件角色
                state.CopyTo(role.Value.State);

            }

        }

        public void RefreshView(Dictionary<RoleKey, IRole> roles)
        {

             code.RefreshView(this,roles);

            foreach (var role in roles)
            {
                role.Value.RefreshView();
            }
         

        }

        public void ShowView(int showType)
        {
           
        }

        public virtual void SetRoleEventState(int rolePot, int eventState)
        {
            RoleKey roleKey = AllRolePot[rolePot];


        }
    }




}