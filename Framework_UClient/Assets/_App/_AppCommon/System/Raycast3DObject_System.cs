using System;
using System.Collections;
using System.Collections.Generic;
using FutureCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectApp
{

    public interface IRaycast3D_OnClick
    {
        public Collider Collider {get;}
        public void Raycast_OnClick(Vector3 hitPoint);


    }

    public class Raycast3D_System : BaseSystem
    {
        public Camera mainCamera;

        public float maxDistance = Mathf.Infinity;
        public int layerMask = ~0;
        
        /// <summary>
        /// .UseGlobal     // 使用Physics设置（默认）
        /// .Ignore        // 忽略所有触发器
        /// .Collide       // 检测触发器
        /// </summary>
        public QueryTriggerInteraction queryTriggerInteraction  = QueryTriggerInteraction.UseGlobal;

        public Dictionary<int, IRaycast3D_OnClick> raycast3D_OnClick = new Dictionary<int, IRaycast3D_OnClick>();

        public override void Init()
        {
            mainCamera = Camera.main;
            maxDistance = Mathf.Infinity;
            layerMask =  LayerMask.GetMask();
            queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

        }

        public override void Start()
        {
            base.Start();
        }

        public override void Run()
        {
            if(!IsRunning)return;
            if (Input.GetMouseButtonDown(0))
            {
                // 检查UI遮挡
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject())
                    return;

                CheckClick();
            }
        }

        private void CheckClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit,maxDistance,layerMask,queryTriggerInteraction))
            {
                int id = hit.colliderInstanceID;
                if (raycast3D_OnClick.TryGetValue(id, out IRaycast3D_OnClick raycast))
                {
                    raycast.Raycast_OnClick(hit.point);
                }
            }
        }


        public void RegisterEvent_OnClick(IRaycast3D_OnClick raycast3D)
        {
            int id = raycast3D.Collider.GetInstanceID();
            if (raycast3D_OnClick.ContainsKey(id)) return;
            
            raycast3D_OnClick.Add(id,raycast3D);
        }

        public void UnregisterEvent_OnClick(IRaycast3D_OnClick raycast3D) 
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
        public void ClearAllLayers()
        {
            layerMask = 0;
            //Debug.Log("已清空所有层级");
        }

        // 设置包含所有层级
        public void SetAllLayers()
        {
            layerMask = ~0; // 所有位都为1
            //Debug.Log("已包含所有层级");
        }

        public override void Dispose()
        {
            base.Dispose();
            raycast3D_OnClick.Clear();
            
        }



    }
}
