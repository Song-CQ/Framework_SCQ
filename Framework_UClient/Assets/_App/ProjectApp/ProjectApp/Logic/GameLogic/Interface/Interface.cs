/****************************************************
    文件: interface.cs
    作者: Clear
    日期: 2023/12/6 11:22:7
    类型: 逻辑脚本
    功能: 接口
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static FutureCore.UIEventListener;

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

        public bool SetRole(IRole role, int index = -1);
        public bool RemoveRole(RoleKey roleKey);

        public bool CheckCanSetRole(RoleKey key, out int potIndex_old, int potIndex = -1);
        public void Rest();
        public void SetRoleEventState(RoleKey roleKey, RunEventData data);
    }

    public interface ISceneEvent//事件
    {
        public IPicture Picture {  get; }
        public EventKey Key { get; }

        public List<RoleKey> AllRolePot { get; }



        public void EnterPicture(IPicture picture);
        public void ExitPicture(IPicture picture);
        public bool SetRole(int index,RoleKey key);
        public bool RemoveRole(RoleKey key);
        public void Run(Dictionary<RoleKey, RoleState> allRoleState, Dictionary<RoleKey, IRole> roles);
        public void RefreshView(Dictionary<RoleKey, IRole> roles);
        public void OnShowView(int showType);

        public int GetRoleIndexToRoleKey(RoleKey roleKey);
    }

    
    public interface IRole//角色
    {
        public RoleKey Key { get; }
        public RoleState State { get; }

        public bool GetLabelKey(string labelKey);

        public void RefreshView();


        public void EnterSceneEvent(ISceneEvent sceneEvent);
        public void ExitSceneEvent(ISceneEvent sceneEvent);
        public RoleStateKey GetShowState();
    }

    public interface IDrag
    {
        public Base_Data Data { get; }
        void AddListener();
        void RomveListener();
        void BeginDrag();
        void Drag();
        void EndDrag();

        IPicture GetPicture();
    }

}
