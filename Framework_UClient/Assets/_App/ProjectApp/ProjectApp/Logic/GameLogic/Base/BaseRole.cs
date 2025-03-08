/****************************************************
    文件: BaseRoleEntity.cs
    作者: Clear
    日期: 2023/12/6 11:29:54
    类型: 逻辑脚本
    功能: 基础角色
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class BaseRole: IRole
    {
        protected Role_Data data;
        protected ISceneEvent sceneEvent = null;
        public RoleKey Key { get => data.Key; }

        public RoleState State { private set; get; }

        public BaseRole()
        {
            State = new RoleState();
        }

        public virtual void Init(Role_Data data)
        {
            this.data = data;

        }

        public void EnterSceneEvent(ISceneEvent sceneEvent)
        {
            Debug.Log("角色:"+Key.ToString()+"进入场景:"+sceneEvent.Key.ToString());
            this.sceneEvent = sceneEvent;

        }

        public void ExitSceneEvent(ISceneEvent sceneEvent)
        {
            Debug.Log("角色:" + Key.ToString() + "退出场景:" + sceneEvent.Key.ToString());
            this.sceneEvent = null;

        }

        public bool GetLabelKey(LabelKey labelKey)
        {   
            return data.allLabelKey.Contains(labelKey);
        }

        public virtual void RefreshView()
        {
           


        }
    }

   

}