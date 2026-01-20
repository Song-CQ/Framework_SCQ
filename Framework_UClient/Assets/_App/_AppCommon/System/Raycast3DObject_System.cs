using System.Collections;
using System.Collections.Generic;
using FutureCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectApp
{

    public interface IRaycast3D
    {
        public Collider Collider {get;}
        public void OnObjectClicked(Vector3 hitPoint);


    }

    public class Raycast3D_System : BaseSystem
    {
        public Camera mainCamera;

        public float maxDistance = Mathf.Infinity;
        public int layerMask = 1000;
        
        /// <summary>
        /// QueryTriggerInteraction.UseGlobal     // 使用Physics设置（默认）
        /// QueryTriggerInteraction.Ignore        // 忽略所有触发器
        /// QueryTriggerInteraction.Collide       // 检测触发器
        /// </summary>
        public QueryTriggerInteraction queryTriggerInteraction  = QueryTriggerInteraction.UseGlobal;

        public Dictionary<uint,IRaycast3D> raycast3DList = new Dictionary<uint,IRaycast3D>();

        public override void Init()
        {
            mainCamera = Camera.main;
            maxDistance = Mathf.Infinity;
            layerMask =  LayerMask.GetMask();
            queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

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

        void CheckClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit,maxDistance,layerMask,queryTriggerInteraction))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    OnObjectClicked(hit.point);
                }
            }
        }

        void OnObjectClicked(Vector3 hitPoint)
        {
            Debug.Log($"点击了3D物体:在位置: {hitPoint}");



        }

        public void RegisterEvent()
        {
            
        }
        public void UnregisterEvent() 
        {
            
        }

    }
}
