/****************************************************
    文件: BaseRoleEntity.cs
    作者: Clear
    日期: 2023/12/6 11:29:54
    类型: 逻辑脚本
    功能: 基础角色
*****************************************************/
using System.Collections.Generic;
using ProjectApp.Data;
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

        public virtual void Init(Role_Data data, RoleState state)
        {
            this.data = data;
            this.State = state;
        }

        public virtual void EnterSceneEvent(ISceneEvent sceneEvent)
        {
            Debug.Log("角色:"+Key.ToString()+"进入场景:"+sceneEvent.Key.ToString());
            this.sceneEvent = sceneEvent;

        }

        public virtual void ExitSceneEvent(ISceneEvent sceneEvent)
        {
            Debug.Log("角色:" + Key.ToString() + "退出场景:" + sceneEvent.Key.ToString());
            this.sceneEvent = null;

        }

        public bool GetLabelKey(string labelKey)
        {
            foreach (var labeID in data.VO.LabeID_list)
            {
                var vo = RoleLabelVOModel.Instance.GetVO(labeID);
                if (vo != null)
                {
                    if (vo.key == labelKey)
                    {
                        return true;
                    }
                }

            }


            return false;
        }

        public RoleStateKey GetShowState()
        {
            var currState = RoleStateKey.待机;
            List<RoleStateKey> roleStateKeys = State.GetAllState(out List<int> datas);

            for (int i = 0; i < roleStateKeys.Count; i++)
            {
                RoleStateKey key = roleStateKeys[i];
                int data = datas[i];

                if (currState == RoleStateKey.幽灵)
                {
                    if (key == RoleStateKey.复活)
                    {
                        currState = RoleStateKey.待机;
                    }
                }
                else
                {
                    currState = key;

                }


            }

            return currState;




        }

        public virtual void RefreshView()
        {



        }
    }

   

}