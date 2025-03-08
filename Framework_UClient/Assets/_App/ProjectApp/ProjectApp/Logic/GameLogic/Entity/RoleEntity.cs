/****************************************************
    文件: RoleEntity.cs
    作者: Clear
    日期: 2024/8/15 17:59:23
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using FutureCore;
using System;
using UnityEngine;
using UnityEngine.EventSystems;



namespace ProjectApp
{
    public class RoleEntity : BaseRole, IDrag
    {

        public Base_Data Data => base.data;

        public UIEventListener Listener { get; private set; }
        public GameObject Entity { get; private set; }

        private UIEventListener.PointerHandler beginDrag_Delegate;
        private UIEventListener.PointerHandler drag_Delegate;
        private UIEventListener.PointerHandler endDrag_Delegate;

        public override void Init(Role_Data data)
        {
            base.Init(data);

            LoadEntity();

        }

        private void LoadEntity()
        {
            Entity = GameWorldMgr.Instance.GameEntity.GetPrefabGo(GameWordEntity.SceneEntityPath);
            


        }

        public void AddListener(UIEventListener.PointerHandler _BeginDrag, UIEventListener.PointerHandler _Drag_Delegate, UIEventListener.PointerHandler _EndDrag_Delegate)
        {
            beginDrag_Delegate = _BeginDrag;
            drag_Delegate = _Drag_Delegate;
            endDrag_Delegate = _EndDrag_Delegate;

            Listener.BeginDrag += beginDrag_Delegate;
            Listener.Drag += drag_Delegate;
            Listener.EndDrag += endDrag_Delegate;

        }
        public void RomveListener()
        {
            Listener.BeginDrag -= beginDrag_Delegate;
            Listener.Drag -= drag_Delegate;
            Listener.EndDrag -= endDrag_Delegate;
            beginDrag_Delegate = null;
            drag_Delegate = null;
            endDrag_Delegate = null;
        }


        public void BeginDrag()
        {
            
        }

        public void Drag()
        {
          
        }

        public void EndDrag()
        {
           
    

        }

        public IPicture GetPicture()
        {
            return sceneEvent.Picture;
        }

        public void Dispose()
        {
            RomveListener();



            GameWorldMgr.Instance.GameEntity.ReleasePrefabGo(GameWordEntity.RoleEntityPath, Entity);
            ObjectPoolStatic<RoleEntity>.Release(this);
        }
    }
}