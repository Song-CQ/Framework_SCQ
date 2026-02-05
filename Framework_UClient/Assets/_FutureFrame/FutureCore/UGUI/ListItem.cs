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
    public class ListItem : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private Text titleText;
        [SerializeField] private Text descText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;
        [SerializeField] private RectTransform rectTransform;

        public object index { get; private set; }
        private object userData;
        private System.Action<object> clickCallback;

        void Awake()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (button == null) button = GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(OnItemClick);
            }
        }

        public void Initialize(ListView.ListItemData data, object index)
        {
            this.index = index;
            this.userData = data.userData;

            if (titleText != null) titleText.text = data.title;
            if (descText != null) descText.text = data.description;
            if (iconImage != null && data.icon != null) iconImage.sprite = data.icon;
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
            if (button != null)
            {
                button.onClick.RemoveListener(OnItemClick);
            }
        }
    }

}