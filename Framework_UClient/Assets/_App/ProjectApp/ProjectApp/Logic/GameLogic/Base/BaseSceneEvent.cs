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
        protected Event_Data data;
        protected EventCode code;

        //protected 

        public List<RoleKey> AllRolePot { private set; get; } = new List<RoleKey>();   

        public EventKey Key { get => data.Key; }

        public Dictionary<RoleKey, RoleStateToEvent> eventStateList = new Dictionary<RoleKey, RoleStateToEvent>();

        public IPicture Picture { private set; get;}
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

        public virtual bool SetRole(int index, RoleKey key)
        {
            if (index == -1)
            {
                index = GetNullRolePot();
            }
            if (index < AllRolePot.Count)
            { 
                AllRolePot[index] = key;
                return true;
            }
            return false;
        }

        public virtual bool RemoveRole(RoleKey key)
        {
            int index = GetRoleIndexToRoleKey(key);
            return RemoveRole(index);
        }

        public virtual bool RemoveRole(int index)
        {
            if (index >= 0 && index < AllRolePot.Count)
            {
                AllRolePot[index] = RoleKey.Node;
                return true;
            }
            return false;
        }



        public int GetRoleIndexToRoleKey(RoleKey key)
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



        public virtual void EnterPicture(IPicture picture)
        {
            Debug.Log("Show场景:"+ Key.ToString());
            this.Picture = picture;
        }

        public virtual void ExitPicture(IPicture picture)
        {
            Debug.Log("Exit场景:" + Key.ToString());

            //回收
            this.Picture = null;
            AllRolePot.Clear();
        }

        public void Run(Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles)
        {
            eventStateList.Clear();

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
            code.UpdataState(this,roles);
   
        }

        public virtual void OnShowView(int showType)
        {
            Debug.Log( " 画面 " +Picture.Index + " 的ShowType："+showType);

        }

        public virtual void SetRoleEventState(int rolePot, RoleStateToEvent eventState)
        {
            RoleKey roleKey = AllRolePot[rolePot];

            eventStateList.Add(roleKey,eventState);
        }

        
    }




}