using System;
using System.Collections;
using System.Collections.Generic;
using FutureCore;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectApp
{

    public interface IRaycast3D
    {
        public Collider Collider { get; }
        public void Raycast_OnClick(Vector3 hitPoint);
        
        public void Raycast_OnSwipe(Vector3 startPoint,Vector3 endPoint,IRaycast3D endIRaycast3D);

    }

    public class Raycast3D_System : BaseSystem
    {
        public Camera mainCamera;

        public float maxDistance = 1000;
        public int layerMask = 0;

        /// <summary>
        /// .UseGlobal     // 使用Physics设置（默认）
        /// .Ignore        // 忽略所有触发器
        /// .Collide       // 检测触发器
        /// </summary>
        public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

        public Dictionary<int, IRaycast3D> raycast3D_OnClick = new Dictionary<int, IRaycast3D>();


        public override void Init()
        {
            mainCamera = Camera.main;
            maxDistance = 1000;
            layerMask = ~0;
            queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

        }

        public override void Start()
        {

            base.Start();
            InputMgr.OnClick += OnClick;
            InputMgr.OnSwipe += OnSwipe;
            
        }

        public override void Shutdown()
        {
            base.Shutdown();
            InputMgr.OnClick -= OnClick;
            InputMgr.OnSwipe -= OnSwipe;
       
        }

        public override void Run()
        {
            if (!IsRunning) return;
        }

        private void OnClick(Vector2 pot)
        {
 

            if (GetClickRaycast3D(pot, out IRaycast3D raycast3D, out Vector3 hitPot))
            {
                raycast3D.Raycast_OnClick(hitPot);
            }

        }


        private void OnSwipe(SwipeDirection arg1, Vector2 potStart, Vector2 potEnd)
        {


            if (GetClickRaycast3D(potStart, out IRaycast3D raycast3D_start, out Vector3 hitPot_start))
            {
                GetClickRaycast3D(potEnd, out IRaycast3D raycast3D_end, out Vector3 hitPot_end);

                raycast3D_start.Raycast_OnSwipe(hitPot_start,hitPot_end,raycast3D_end);             
            }


        }




        private bool GetClickRaycast3D(Vector3 pot, out IRaycast3D raycast, out Vector3 hitPot)
        {
            Ray ray = mainCamera.ScreenPointToRay(pot);

            hitPot = Vector3.zero;
            raycast = null;

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, queryTriggerInteraction))
            {
                int id = hit.colliderInstanceID;
                if (raycast3D_OnClick.TryGetValue(id, out raycast))
                {
                    hitPot = hit.point;
                    return true;
                }
            }
            else //2D 
            {
                // 保存原始设置
                bool originalHitTriggers = Physics2D.queriesHitTriggers;
                // 设置为忽略触发器（与3D保持一致）
                Physics2D.queriesHitTriggers = queryTriggerInteraction == QueryTriggerInteraction.Ignore ? false : true;

                // 2D 的 layerMask 是 int 类型
                RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, maxDistance, layerMask);

                // 恢复原始设置
                Physics2D.queriesHitTriggers = originalHitTriggers;

                if (hit2D.collider != null)
                {
                    int id = hit2D.collider.GetInstanceID();
                    if (raycast3D_OnClick.TryGetValue(id, out raycast))
                    {
                        hitPot = hit2D.point;
                        return true;

                    }
                }
            }

            return false;
        }

        public void RegisterEvent_OnClick(IRaycast3D raycast3D)
        {
            int id = raycast3D.Collider.GetInstanceID();
            if (raycast3D_OnClick.ContainsKey(id)) return;

            raycast3D_OnClick.Add(id, raycast3D);
        }

        public void UnregisterEvent_OnClick(IRaycast3D raycast3D)
        {
            raycast3D_OnClick.Remove(raycast3D.Collider.GetInstanceID());
        }

        public void AddCheckLayerMask(string layerName)
        {
            AddCheckLayerMask(LayerMask.NameToLayer(layerName));
        }
        public void RemoveCheckLayerMask(string layerName)
        {
            RemoveCheckLayerMask(LayerMask.NameToLayer(layerName));
        }

        public void AddCheckLayerMask(int layerID)
        {
            if (layerID >= 0 && layerID < 32)
            {
                layerMask |= (1 << layerID);
            }
        }
        public void RemoveCheckLayerMask(int layerID)
        {
            if (layerID >= 0 && layerID < 32)
            {
                layerMask &= ~(1 << layerID);
            }
        }

        // 检查是否包含某个层级
        public bool ContainsLayer(string layerName)
        {
            int layerID = LayerMask.NameToLayer(layerName);
            return ContainsLayer(layerID);
        }

        public bool ContainsLayer(int layerID)
        {
            if (layerID < 0 || layerID >= 32) return false;
            return (layerMask & (1 << layerID)) != 0;
        }

        // 清空所有层级
        public void ClearCheckAllLayers()
        {
            layerMask = 0;
            // Debug.Log("已清空所有层级");
        }


        // 设置包含所有层级
        public void SetCheckAllLayers()
        {
            layerMask = ~0; // 所有位都为1
            // Debug.Log("已包含所有层级");
        }

        public override void Dispose()
        {
            base.Dispose();
            raycast3D_OnClick.Clear();

        }



    }
}
