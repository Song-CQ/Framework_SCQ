/****************************************************
    文件: base.cs
    作者: Clear
    日期: 2024/1/16 12:15:41
    类型: 逻辑脚本
    功能: 基础画面
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class BasePicture : IPicture
    {
        public ISceneEvent SceneEvent { get; private set; }

        public Dictionary<RoleKey, IRole> Roles { get; private set; }

        public int Index { get; private set; }




        public virtual void Init()
        {
            Debug.Log("创建画布");
            Roles = new Dictionary<RoleKey, IRole>();

        }

        public virtual void Show(int _Index)
        {
            Index = _Index;

        }

        public virtual void SetEvent(ISceneEvent newScene)
        {
            SceneEvent?.ExitPicture(this);

            SceneEvent = newScene;
            if (SceneEvent != null)
            {
                SceneEvent.EnterPicture(this);
            }


        }

        public virtual void RemoveEvent()
        {
            SceneEvent?.ExitPicture(this);


            SceneEvent = null;

        }

        public virtual bool CheckCanSetRole(RoleKey key,out int potIndex_old, int potIndex = -1)
        {
            potIndex_old = 0;
            if (SceneEvent == null)
            {
                return false;
            }
            if (potIndex < 0 || potIndex >= SceneEvent.AllRolePot.Count)
            {
                return false;
            }

            potIndex_old = SceneEvent.AllRolePot.FindIndex((e) => key == e);

            if (potIndex_old == potIndex)
            {
                return false;
            }
            return true;
        }

        public virtual bool SetRole(IRole role, int potIndex = -1)
        {
            //去除要设置位置本身的角色 
            RemoveRole(potIndex);


            bool isRst = SceneEvent.SetRole(potIndex, role.Key);
            if (isRst)
            {
                Roles[role.Key] = role;

                Roles[role.Key].EnterSceneEvent(this.SceneEvent);
            }

            return isRst;


        }

        public virtual bool RemoveRole(int index)
        {
            if (SceneEvent == null)
            {
                return false;
            }
            if (index < 0 || index >= SceneEvent.AllRolePot.Count)
            {
                return false;
            }
            RoleKey roleKey = SceneEvent.AllRolePot[index];
            return RemoveRole(roleKey);
            
        }

     

        public virtual bool RemoveRole(RoleKey roleKey)
        {
            if (SceneEvent == null)
            {
                return false;
            }

            if (roleKey == RoleKey.Node)
            {
                return false;
            }

            if (SceneEvent.RemoveRole(roleKey)) 
            {
                Roles[roleKey].ExitSceneEvent(this.SceneEvent);
                Roles.Remove(roleKey);
                return true;
            }

            return false;

        }
        public void SetRoleEventState(RoleKey roleKey, RunEventData stateData)
        {

            // if (Roles.TryGetValue(roleKey, out IRole roleState))
            // {
            //     roleState.State.SetState(Index, stateData); 
                
            // }
        }

        public void Refactor(Dictionary<RoleKey, RoleState> allRoleState)
        {
            if (SceneEvent == null)
            {
                return;
            }

            SceneEvent.Run(allRoleState, Roles);

            RefreshView();
        }

        private void RefreshView()        {

            //刷新场景显示
            SceneEvent.RefreshView(Roles);

            //刷新角色状态
            foreach (var role in Roles)
            {
                role.Value.RefreshView();
            }

           
        }

        public virtual void Rest()
        {
            SceneEvent?.ExitPicture(this);
            SceneEvent = null;         
        }

        
    }
}