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

        public List<IRole> Roles => SceneEvent.Roles;

        public int Index { get; private set; }

        public ComboEventData Combo { get; private set; }

        public Action<ComboEventData> ChangeCombo_Event { get; set; }

       
        public virtual void Init()
        {
            Debug.Log("创建画布");


        }

        public virtual void Show(int _Index)
        {
            Index = _Index;
            Combo = new ComboEventData(Index);

            Combo.SceneEvent_Key = string.Empty;
            Combo.RoleEvent_Keys.Clear();
        }

        public virtual bool AddRole(IRole role, int index = -1)
        {
            if (SceneEvent == null)
            {
                return false;
            }
            if (index < 0 || index >= Roles.Count)
            {
                return false;
            }
            RemoveRole(index);

            Roles[index].FillModule.EnterPicture(this);
            Roles[index] = role;
            return true;
        }

        public bool CheckComplete()
        {
            if (SceneEvent==null)
            {
                return false;
            }
            return SceneEvent.IsComplete;
        }

        public virtual bool RemoveRole(int index)
        {
            if (SceneEvent == null)
            {
                return false;
            }
            if (index < 0 || index >= Roles.Count || Roles[index] == null)
            {
                return false;
            }

            Roles[index].FillModule.ExitPicture(this);
            Roles[index] = null;

            return true;
        }

        public virtual void Rest()
        {
            SceneEvent?.FillModule.ExitPicture(this);
            SceneEvent = null;         
        }

        public virtual void SetScene(ISceneEvent newScene)
        {
            SceneEvent?.FillModule.ExitPicture(this);

            SceneEvent = newScene;
            if (newScene != null)
            {
                SceneEvent.FillModule.EnterPicture(this);
            }
            ChaneComboEvent();
        }

        /// <summary>
        /// 当前的combo改变
        /// </summary>
        /// <param name="oldCombo"></param>
        /// <param name="newCombo"></param>
        protected virtual void ChaneComboEvent()
        {
            GameLogicTool.FillComboData(Combo,this);

            ChangeCombo_Event?.Invoke(Combo);
        }
    }
}