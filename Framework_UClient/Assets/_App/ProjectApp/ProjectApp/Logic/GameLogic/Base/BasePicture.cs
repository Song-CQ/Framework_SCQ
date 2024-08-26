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

        public virtual void SetRole(IRole role, int potIndex = -1)
        {
            //去除要设置位置本身的角色 
            RemoveRole(potIndex);


            SceneEvent.SetRole(potIndex, role.Key);

            Roles[role.Key] = role;

            Roles[role.Key].EnterSceneEvent(this);


        }

        public virtual void RemoveRole(int index)
        {
            if (SceneEvent == null)
            {
                return;
            }
            if (index < 0 || index >= SceneEvent.AllRolePot.Count)
            {
                return;
            }
         
             RoleKey roleKey = SceneEvent.AllRolePot[index];    

             RemoveRole(roleKey);
        }

        public virtual void RemoveRole(RoleKey roleKey)
        {
            if (SceneEvent == null)
            {
                return;
            }

            if (roleKey == RoleKey.Node)
            {
                return;
            }

            SceneEvent.RemoveRole(roleKey);

            Roles[roleKey].ExitSceneEvent(this);
            Roles[roleKey] = null;

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

        private void RefreshView()
        {
            //刷新角色状态

            //刷新场景显示
            SceneEvent.RefreshView(Roles);
        }

        public virtual void Rest()
        {
            SceneEvent?.ExitPicture(this);
            SceneEvent = null;         
        }

        
    }
}