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
    public class DragEntity:MonoBehaviour
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



        }

        private void Listener_BeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            GameWorldMgr.Instance.Dispatch();
        }
        private void Listener_Drag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            
        }
        private void Listener_EndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            

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