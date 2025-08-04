/****************************************************
    文件: SceneEventEntity.cs
    作者: Clear
    日期: 2024/8/15 18:2:31
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using FutureCore;
using ProjectApp.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static FutureCore.UIEventListener;

namespace ProjectApp
{
    public class SceneEventEntity : BaseSceneEvent, IDrag
    {
        public GameObject Entity;

        public List<Transform> RoleSoltPot = new List<Transform>();

        public Base_Data Data => base.data;

        public PictureEntity PictureEntity => (Picture as PictureEntity);

        public UIEventListener Listener { get; private set; }
        private PointerHandler beginDrag_Delegate;
        private PointerHandler drag_Delegate;
        private PointerHandler endDrag_Delegate;
        private Transform SceneTrf;

        public override void Init(Event_Data _data, EventCode eventCode)
        {
            base.Init(_data, eventCode);



            LoadEntity();
            AddListener();

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
            Listener = null;
        }


        private void LoadEntity()
        {
            Entity = GameWorldMgr.Instance.GameEntity.GetPrefabGo(GameWordEntity.SceneEntityPath);
            Entity.transform.Find("desc").GetComponent<TextMeshPro>().text = Data.Desc;
            Transform Content = Entity.transform.Find("Content");

            SceneTrf = GameWorldMgr.Instance.GameEntity.GetPrefabGo(GameWordEntity.SceneKeyPath + data.Key.ToString()).transform;
            SceneTrf.SetParent(Content);
            SceneTrf.localPosition = Vector3.zero;
            SceneTrf.localScale = Vector3.one;

            Transform rolesTrf = SceneTrf.transform.Find("RolePot");
            for (int i = 0; i < data.RoleSum; i++)
            {
                Transform potGo = rolesTrf.GetChild(i);
                RoleSoltPot.Add(potGo);

            }

        }

        public override void EnterPicture(IPicture picture)
        {
            base.EnterPicture(picture);

            Entity.transform.SetParent(PictureEntity.EventTrf);
            Entity.transform.localPosition = Vector3.zero;
            Entity.transform.localScale = Vector3.one;



        }


        public override void ExitPicture(IPicture picture)
        {
            base.ExitPicture(picture);

            foreach (var role in picture.Roles)
            {
                if (role.Value != null)
                {
                    role.Value.ExitSceneEvent(this);
                }

            }


            Dispose();
        }


        /// <summary>
        /// 获取离输入的屏幕坐标最近的角色位置索引
        /// </summary>
        /// <param name="pot"></param>
        /// <returns></returns>
        public int GetRolePotToScendPot(Vector2 pot)
        {
            float dic = -1;

            int index = 0;
            for (int i = 0; i < RoleSoltPot.Count; i++)
            {
                Transform t = RoleSoltPot[i];
                Vector2 t_pot = CameraMgr.Instance.mainCamera.WorldToScreenPoint(t.position);
                float val = Vector2.Distance(t_pot, pot);
                if (dic < 0 || val < dic)
                {
                    dic = val;
                    index = i;
                }

            }
            return index;
        }

        public void BeginDrag()
        {

        }

        public void Drag()
        {

        }

        public void EndDrag()
        {
            //将自身从画面中移除 如果存在于画面中的话



        }

        public IPicture GetPicture()
        {
            return Picture;
        }

        public override bool SetRole(int index, RoleKey key)
        {

            return base.SetRole(index, key);
        }


        private void Dispose()
        {
            RomveListener();

            RoleSoltPot.Clear();

            GameWorldMgr.Instance.GameEntity.ReleasePrefabGo(GameWordEntity.SceneKeyPath + data.Key.ToString(), SceneTrf.gameObject);
            GameWorldMgr.Instance.GameEntity.ReleasePrefabGo(GameWordEntity.SceneEntityPath, Entity);
            Entity = null;
            SceneTrf = null;

            ObjectPoolStatic<SceneEventEntity>.Release(this);
        }

        public Vector2 GetRolePot(int potIndex)
        {
            if (potIndex < 0 || potIndex >= data.RoleSum)
            {
                return Vector2.zero;
            }

            return RoleSoltPot[potIndex].localPosition;


        }
        public Vector2 GetRolePot(RoleKey roleKey)
        {
            int potIndex = GetRoleIndexToRoleKey(roleKey);

            if (potIndex < 0 || potIndex >= data.RoleSum)
            {
                return Vector2.zero;
            }

            return RoleSoltPot[potIndex].localPosition;


        }

        public override void SetRoleEventState(RoleKey roleKey, RunEventData data)
        {
            base.SetRoleEventState(roleKey, data);
            if (AllRolePot.Contains(roleKey))
            {
                // GameWorldMgr.Instance.GameSys.Dispatch(GameEvent.ChangeRoleState);
            }

        }

    }
}
