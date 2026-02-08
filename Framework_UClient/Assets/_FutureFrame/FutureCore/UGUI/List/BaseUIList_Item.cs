/****************************************************
    文件: ListItem.cs
    作者: Clear
    日期: 2026/2/6 1:35:24
    类型: 逻辑脚本
    功能: 列表项组件
*****************************************************/
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FutureCore
{

    public class ItemData
    {
        public int IntData;
        public object Data;

        public T GetData<T>() where T : class
        {
            return Data as T;
        }

    }


    /// <summary>
    /// 列表项组件
    /// </summary>
    public class BaseUIList_Item : MonoBehaviour
    {
        [TitleGroup("UI组件")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private LayoutElement layoutElement;
        
        [Space(10)]
        [LabelText("接收 UI事件")]
        [SerializeField] private bool useUIEventListener = false;
        [ShowIf("useUIEventListener")]
        [SerializeField] private UIEventListener eventListener;
        [FoldoutGroup("回调设置")]
        [LabelText("Click Event"), ShowInInspector,DisplayAsString]
        
        private Action<BaseUIList_Item, ItemData> clickCallback;

        [TitleGroup("数据")]
        [PropertySpace(10)]
        [LabelText("Index"), ShowInInspector]
        public int Index { get; private set; }

       


        private ItemData itemData;
        void Awake()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (layoutElement == null) layoutElement = GetComponent<LayoutElement>();

            if (useUIEventListener)
            { 
                if (eventListener == null) eventListener = UIEventListener.GetEventListener(transform);
                eventListener.PointerClick_Event += OnItemClick;
            }

            
        }

       

        public virtual void Initialize(int index, ItemData data)
        {
            this.Index = index;
            this.itemData = data;    
        }
        public void SetClickCallback(System.Action<BaseUIList_Item, ItemData> callback)
        {
            clickCallback = callback;
        }

        private void OnItemClick(PointerEventData eventData)
        {
            clickCallback?.Invoke(this, itemData);
            //Debug.Log($"Item clicked: {index}");
        }


        public void SetSize(float itemWidth, float itemHeight)
        {
            rectTransform.sizeDelta = new Vector2 (itemWidth, itemHeight);
            layoutElement.preferredHeight = itemHeight;
            layoutElement.preferredWidth = itemWidth;
        }
        void OnDestroy()
        {

        }
    }

}