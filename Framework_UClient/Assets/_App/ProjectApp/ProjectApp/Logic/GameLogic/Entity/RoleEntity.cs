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

        private PictureEntity pictureEntity => sceneEvent.Picture as PictureEntity;

        public SceneEventEntity sceneEventEntity => sceneEvent as SceneEventEntity;

        private UIEventListener.PointerHandler beginDrag_Delegate;
        private UIEventListener.PointerHandler drag_Delegate;
        private UIEventListener.PointerHandler endDrag_Delegate;


        private RoleAvatar roleAvatar;



        
        

        public override void Init(Role_Data data,RoleState State)
        {
            base.Init(data,State);

            LoadEntity();

            AddListener();

        }

        private void LoadEntity()
        {
            Entity = GameWorldMgr.Instance.GameEntity.GetPrefabGo(GameWordEntity.RoleEntityPath);

            roleAvatar = Entity.GetComponent<RoleAvatar>() ?? Entity.AddComponent<RoleAvatar>();

            roleAvatar.Init(data);

           

            

        }

        public void AddListener()
        {
            Listener = UIEventListener.GetEventListener(Entity.transform);
            beginDrag_Delegate = (e) =>
            {
                GameWorldMgr.Instance.GameEntity.BeginDragEntity(this, e.position);
            };
            drag_Delegate = (e) =>
            {
                GameWorldMgr.Instance.GameEntity.DragEntity(this, e.position);
            };
            endDrag_Delegate = (e) =>
            {
                GameWorldMgr.Instance.GameEntity.EndDragEntity(this, e.position);
            };

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

        public override void EnterSceneEvent(ISceneEvent sceneEvent)
        {
            base.EnterSceneEvent(sceneEvent);
           
            Entity.transform.SetParent(pictureEntity.RolesTrf);
            Entity.transform.localScale = Vector3.one;
            Entity.transform.localPosition = sceneEventEntity.GetRolePot(Key);

        }

        public override void ExitSceneEvent(ISceneEvent sceneEvent)
        {
            base.ExitSceneEvent(sceneEvent);

            Entity.transform.SetParent(null);
            Dispose();

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

        public override void RefreshView()
        {
            base.RefreshView();
           

            roleAvatar.SetState(State);

        }

        public IPicture GetPicture()
        {
            return sceneEvent.Picture;
        }

        public void Dispose()
        {
            RomveListener();

            roleAvatar.Clear();
            roleAvatar = null;

            GameWorldMgr.Instance.GameEntity.ReleasePrefabGo(GameWordEntity.RoleEntityPath, Entity);
            ObjectPoolStatic<RoleEntity>.Release(this);
        }
    }
}