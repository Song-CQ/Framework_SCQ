/****************************************************
    文件: ListItem.cs
    作者: Clear
    日期: 2026/2/6 1:35:24
    类型: 逻辑脚本
    功能: 列表项组件
*****************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace FutureCore
{
    /// <summary>
    /// 列表项组件
    /// </summary>
    public class UI_ListBaseItem : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private RectTransform rectTransform;

        public object index { get; private set; }
        private object userData;
        private System.Action<object> clickCallback;

        void Awake()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
     
        }

        public void Initialize(object data, object index)
        {
            this.index = index;
            
        }

        public void SetClickCallback(System.Action<object> callback)
        {
            clickCallback = callback;
        }

        void OnItemClick()
        {
            clickCallback?.Invoke(userData);
            Debug.Log($"Item clicked: {index}");
        }

        void OnDestroy()
        {
            
        }
    }

}