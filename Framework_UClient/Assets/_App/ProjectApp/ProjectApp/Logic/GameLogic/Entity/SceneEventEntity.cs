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
using UnityEngine;
using UnityEngine.EventSystems;
using static FutureCore.UIEventListener;

namespace ProjectApp
{
    public class SceneEventEntity :BaseSceneEvent ,IDrag
    {
        public GameObject Entity; 

        public List<Transform> RoleSoltPot = new List<Transform>();

        public Base_Data Data => base.data;


        public UIEventListener Listener { get;private set; }
        private PointerHandler beginDrag_Delegate;
        private PointerHandler drag_Delegate;
        private PointerHandler endDrag_Delegate;

        public override void Init(Event_Data _data, EventCode eventCode)
        {
            base.Init(_data, eventCode);


            
    
            LoadEntity();
            
        }

        public void AddListener(PointerHandler _BeginDrag, PointerHandler _Drag_Delegate, PointerHandler _EndDrag_Delegate)
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


        private void LoadEntity()
        {
            Entity = GameWorldMgr.Instance.GameEntity.GetPrefabGo(GameWordEntity.SceneEntityPath);
            Transform rolesTrf = Entity.transform.Find("Roles");

            for (int i = 0; i < data.RoleSum; i++)
            {
                Transform potGo = null;
                if (i >= rolesTrf.childCount)
                {
                    potGo = new GameObject("RolePot_" + i).transform;
                    potGo.SetParent(rolesTrf);
                    potGo.SetSiblingIndex(i);
                }
                else 
                {
                    potGo = rolesTrf.GetChild(i);
                }
               
                potGo.SetActive(true);

                potGo.localPosition = new Vector3(data.VO.allRolePot[i*2], data.VO.allRolePot[i*2+1]);

                RoleSoltPot.Add(potGo);



            }


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
                if (dic < 0 || val<dic)
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
            bool isRest = base.SetRole(index, key);

            if (isRest) 
            {
                var roleEntity = GameWorldMgr.Instance.GameEntity.GetPrefabGo(GameWordEntity.SceneEntityPath);
            }

            return isRest;
        }


        public void Dispose()
        {
            RomveListener();
            foreach (Transform t in RoleSoltPot)
            {
                t.gameObject.SetActive(false);
            }
            RoleSoltPot.Clear();
           

            GameWorldMgr.Instance.GameEntity.ReleasePrefabGo(GameWordEntity.SceneEntityPath,Entity);
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
    }
}
