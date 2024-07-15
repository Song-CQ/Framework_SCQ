/****************************************************
    文件: BaseSceneEntity.cs
    作者: Clear
    日期: 2023/12/6 11:25:12
    类型: 逻辑脚本
    功能: 基础场景事件
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class BaseSceneEvent : ISceneEvent ,IFill
    {
        public IFill FillModule => this;
        public List<IRole> Roles { private set; get; }

        public bool IsComplete => GetComplete();

        public string Key { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public JudgeData JudgeData { private set; get; }
        protected IPicture picture;
        public void Init()
        {
            

        }

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


        public void EnterPicture(IPicture picture)
        {
            Debug.Log("Show场景");
            this.picture = picture;
        }

        public void ExitPicture(IPicture picture)
        {
            Debug.Log("Exit场景");
            this.picture = null;

        }


        protected bool GetComplete()
        {
            if (picture==null)
            {
                return false;
            }
            bool isFace = true;
            foreach (var item in Roles)
            {
                if (item == null)
                {
                    isFace = false;
                    break;
                }
            }
            if (!isFace)
            {
                return false;
            }

            isFace = JudgeData.RunJudgeData(picture.Index,this,Roles);

            return isFace;
        }
    }

    public class JudgeData
    {

        //需要该事件触发才可 触发该事件
        public string needSceneEvent;

        public RoleMatchType roleMatch;

        public bool isOnly;
        public bool RunJudgeData(int index,BaseSceneEvent sceneEvent,List<IRole> roles)
        {
            bool isFace = true;


            if (!isFace) return false;

            
            foreach (var role in roles)
            {
       

            }

            return isFace;
        }
        //首先判断需要的角色
    }

    public enum RoleMatchType
    {
        AllMatch,



    }
}