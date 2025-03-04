/****************************************************
    文件: interface.cs
    作者: Clear
    日期: 2023/12/6 11:22:7
    类型: 逻辑脚本
    功能: 接口
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public interface IPicture//画面
    {
        public ISceneEvent SceneEvent { get;}
        public Dictionary<RoleKey, IRole> Roles { get; }

        public int Index { get; }



        public void Init();
        public void SetEvent(ISceneEvent newScene);
        public void RemoveEvent();

        public void SetRole(IRole role, int index = -1);
        public void RemoveRole(RoleKey roleKey);

        public bool CheckCanSetRole(RoleKey key, out int potIndex_old, int potIndex = -1);
        public void Rest();


    }

    public interface ISceneEvent//事件
    {
        public EventKey Key { get; }

        public List<RoleKey> AllRolePot { get; }


        public void EnterPicture(IPicture picture);
        public void ExitPicture(IPicture picture);
        public void SetRole(int index,RoleKey key);
        public void RemoveRole(RoleKey key);
        public void Run(Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles);
        public void RefreshView(Dictionary<RoleKey, IRole> roles);
        public void ShowView(int showType);
    }

    
    public interface IRole//角色
    {
        public RoleKey Key { get; }
        public RoleState State { get; }

        public bool GetLabelKey(LabelKey labelKey);

        public void RefreshView();


        public void EnterSceneEvent(BasePicture basePicture);
        public void ExitSceneEvent(BasePicture basePicture);
    }

    public interface IDrag
    {



        
    }

}
