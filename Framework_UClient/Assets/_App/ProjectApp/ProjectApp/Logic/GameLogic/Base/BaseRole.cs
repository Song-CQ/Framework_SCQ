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
        public RoleKey Key { get => data.key; }

        public RoleState State { private set; get; }

        public BaseRole()
        {
            State = new RoleState();
        }

        public void Init(Role_Data data)
        {
            this.data = data;

        }

        public void EnterSceneEvent(BasePicture basePicture)
        {
           
        }

        public void ExitSceneEvent(BasePicture basePicture)
        {
           
        }
    }

   

}