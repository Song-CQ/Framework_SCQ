/****************************************************
    文件: DragEntity.cs
    作者: Clear
    日期: 2024/8/14 17:4:21
    类型: 逻辑脚本
    功能: 拖拽实体
*****************************************************/
using FutureCore;
using TMPro;
using UnityEngine;


namespace ProjectApp
{
    public class MenuEntity:MonoBehaviour, IDrag
    {
        public DragEntityType entityType;

        public Base_Data data;

        public TextMeshPro type;
        public TextMeshPro val;

        private UIEventListener listener;


        public void Awake()
        {
            listener = UIEventListener.GetEventListener(transform);

            listener.BeginDrag += Listener_BeginDrag;
            listener.Drag += Listener_Drag; 
            listener.EndDrag += Listener_EndDrag;
            listener.PointerClick_Event += Listener_PointerClick_Event; ;

            

        }

        private void Listener_PointerClick_Event(UnityEngine.EventSystems.PointerEventData eventData)
        {
            //UnityEngine.Debug.LogError("点击"+ eventData.position+"das"+Input.mousePosition);
        }

        private void Listener_BeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            GameWorldMgr.Instance.BeginDragEntity(this,eventData.position);
        }
        private void Listener_Drag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            GameWorldMgr.Instance.DragEntity(this, eventData.position);
        }
        private void Listener_EndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            GameWorldMgr.Instance.EndDragEntity(this, eventData.position);
        }

        public void SetData(Base_Data data)
        {
            this.data = data;
            entityType = data.Type;

            
            val.text = data.Desc;
            type.text = data.Type.ToString();
        }

        public void ResetEntity()
        {
            this.data = null;

        }

    }
}