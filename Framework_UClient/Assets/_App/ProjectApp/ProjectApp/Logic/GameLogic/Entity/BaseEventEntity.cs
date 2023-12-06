/****************************************************
    文件: BaseSceneEntity.cs
    作者: Clear
    日期: 2023/12/6 11:25:12
    类型: 逻辑脚本
    功能: 基础事件实体
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class BaseEventEntity : IEvent
    {
        public List<IRole> Roles { private set; get; }


        private int GetNullRolePot()
        {
            for (int i = 0; i < Roles.Count; i++)
            {
                if (Roles[i] == null)
                {
                    return i;
                }
            }
            return Roles.Count - 1;
        }

        public bool AddRole(IRole role, int index = -1)
        {
            if (index == -1)
            {
                index = GetNullRolePot();
            }

            Roles[index] = role;
            return true;
        }

        public bool RemoveRole(int index)
        {
            if (index < Roles.Count)
            {
                if (Roles[index] != null)
                {
                    Roles[index] = null;

                    return true;
                }

            }
            return false;
        }

        public void Show()
        {
            Debug.Log("Show场景");
        }
        public void Exit()
        {
            Debug.Log("Exit场景");
        }
    }
}