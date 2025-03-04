/****************************************************
    文件: DragEntity.cs
    作者: Clear
    日期: 2024/8/14 17:4:21
    类型: 逻辑脚本
    功能: 拖拽实体
*****************************************************/
using FutureCore;
using System;
using TMPro;
using UnityEngine;
using static FutureCore.UIEventListener;


namespace ProjectApp
{
    public class MenuEntity:MonoBehaviour, IDrag
    {
        public DragEntityType entityType;

        public Base_Data Data { get; private set; }

        public UIEventListener Listener { get; private set; }

        public TextMeshPro type;
        public TextMeshPro val;

        private PointerHandler beginDrag_Delegate;
        private PointerHandler drag_Delegate;
        private PointerHandler endDrag_Delegate;
        private bool IsInit =false;

        public void Init()
        {
            if (IsInit) return;
            Listener = UIEventListener.GetEventListener(transform);

            IsInit = true;



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

        private void Listener_PointerClick_Event(UnityEngine.EventSystems.PointerEventData eventData)
        {
            //UnityEngine.Debug.LogError("点击"+ eventData.position+"das"+Input.mousePosition);
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

        public void SetData(Base_Data data)
        {
            this.Data = data;
            entityType = data.Type;

            
            val.text = data.Desc;
            type.text = data.Type.ToString();
        }

        public void ResetEntity()
        {
            this.Data = null;

        }

    }
}