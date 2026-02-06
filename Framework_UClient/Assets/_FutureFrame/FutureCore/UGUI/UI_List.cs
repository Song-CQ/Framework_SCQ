/****************************************************
    文件: ListView.cs
    作者: Clear
    日期: 2026/2/6 1:35:24
    类型: UI
    功能: 基础滑动列表
*****************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace FutureCore
{

    /// <summary>
    /// 基础滑动列表 （只需要传数列表 ， item必须继承UI_ListBaseItem）
    /// </summary>
    public class UI_List : MonoBehaviour
    {
        enum LayoutGroupType
        {
            Horizontal,
            Version,

            Grip

        }


        [Header("组件引用")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab;

        [Header("列表设置")]
        [SerializeField] private Vector2 spacing;
        [SerializeField] private float itemWidth = 100f;
        [SerializeField] private float itemHeight = 100f;
        [SerializeField] private int bufferSize = 2; // 缓冲区数量
        [SerializeField] private bool isVirtual = false; // 是否虚拟列表
        [SerializeField] private LayoutGroupType layoutGroupType  = LayoutGroupType.Horizontal;
        [Header("排列方式")]
        [SerializeField] private TextAnchor anchor = TextAnchor.LowerLeft;

        

        private List<object> dataList = new List<object>();
        private List<UI_ListBaseItem> activeItems = new List<UI_ListBaseItem>();
        private List<UI_ListBaseItem> currentShowItems = new List<UI_ListBaseItem>();
        private Queue<UI_ListBaseItem> itemPool = new Queue<UI_ListBaseItem>();

        private LayoutGroup layoutGroup;

        public delegate void UpdateItemData(UI_ListBaseItem item,object data);

        [HideInInspector]
        public UpdateItemData updateItemData;

        [HideInInspector]
        public UpdateItemData onCenterOnChild;

        [HideInInspector]
        public UpdateItemData changeSelectItemData;

        private float viewportHeight;
        private float viewportWidth;
        private int totalItems;
        private int visibleItems;
        private int currentFirstIndex = 0;

       

        private void OnEnable() 
        {
            Initialize();

            
            


        }

        void GenerateTestData(int count)
        {
            dataList.Clear();
            for (int i = 0; i < count; i++)
            {
                dataList.Add(new object());
            }
            totalItems = dataList.Count;
        }


        void Start()
        {
            Initialize();

            // 测试数据
            GenerateTestData(50);
            RefreshList();
        }

        private ContentSizeFitter contentSize;

        void Initialize()
        {
            if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
            if (viewport == null) viewport = scrollRect.viewport;
            if (content == null) content = scrollRect.content;

            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            viewportHeight = viewport.rect.height;
            viewportWidth = viewport.rect.width;

            contentSize = content.GetComponent<ContentSizeFitter>()??content.gameObject.AddComponent<ContentSizeFitter>();
            
            switch (layoutGroupType)
            {
                case LayoutGroupType.Horizontal:
                    {
                        var _layoutGroup = contentSize.GetComponent<HorizontalLayoutGroup>() ?? content.gameObject.AddComponent<HorizontalLayoutGroup>();
                        _layoutGroup.spacing = spacing.x;
                        contentSize.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                        contentSize.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                        scrollRect.horizontal = true;
                        scrollRect.vertical = false;
                        layoutGroup = _layoutGroup;
                    }
                    break;

                case LayoutGroupType.Version:
                    {
                        var _layoutGroup = contentSize.GetComponent<VerticalLayoutGroup>() ?? content.gameObject.AddComponent<VerticalLayoutGroup>();
                        layoutGroup = _layoutGroup;
                        contentSize.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                        contentSize.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                        scrollRect.horizontal = false;
                        scrollRect.vertical = true;
                        _layoutGroup.spacing = spacing.y;
                    }
                    break;
                case LayoutGroupType.Grip:
                    {
                        var _layoutGroup = contentSize.GetComponent<GridLayoutGroup>() ?? content.gameObject.AddComponent<GridLayoutGroup>();
                        layoutGroup = _layoutGroup;
                        contentSize.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                        contentSize.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                        scrollRect.horizontal = true;
                        scrollRect.vertical = true;
                        _layoutGroup.spacing = spacing;
                        _layoutGroup.cellSize = new Vector2(itemWidth, itemHeight);
                    }
                    break;

            }

            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            



            // 准备对象池
            PreparePool(20);
        }

        void PreparePool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                UI_ListBaseItem item = InstantiateItem();
                item.SetActive(false);
                itemPool.Enqueue(item);
            }
        }

        UI_ListBaseItem GetPooledItem()
        {
            if (itemPool.Count > 0)
            {
                return itemPool.Dequeue();
            }
            else
            {
                return InstantiateItem();
            }
        }

        private UI_ListBaseItem InstantiateItem()
        {
            UI_ListBaseItem item = Instantiate(itemPrefab, content).GetComponent<UI_ListBaseItem>();
            return item;
        }

        void ReturnToPool(UI_ListBaseItem item)
        {
            item.SetActive(false);
            itemPool.Enqueue(item);
        }

        

        public void SetData(List<object> newData)
        {
            dataList = newData;
            totalItems = dataList.Count;
            RefreshList();
        }

        void RefreshList()
        {
            // 清空当前显示项
            foreach (var item in activeItems)
            {
                ReturnToPool(item);
            }
            activeItems.Clear();

            // 计算内容高度
            float contentHeight = totalItems * itemHeight + (totalItems - 1) * spacing.x;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

            // 计算可见项数量
            visibleItems = Mathf.CeilToInt(viewportHeight / (itemHeight + spacing.x)) + bufferSize * 2;
            visibleItems = Mathf.Min(visibleItems, totalItems);

            // 显示初始项
            currentFirstIndex = 0;
            UpdateVisibleItems();
        }

        void OnScrollValueChanged(Vector2 normalizedPos)
        {
            UpdateVisibleItems();
        }

        void UpdateVisibleItems()
        {
            if(isVirtual)
            if (totalItems == 0) return;

            // 计算应该显示的第一个索引
            float scrollPos = content.anchoredPosition.y;
            int newFirstIndex = Mathf.FloorToInt(scrollPos / (itemHeight + spacing.x));
            newFirstIndex = Mathf.Max(0, newFirstIndex - bufferSize);

            // 如果索引没变化，不更新
            if (newFirstIndex == currentFirstIndex) return;

            // 更新索引
            int oldFirstIndex = currentFirstIndex;
            currentFirstIndex = newFirstIndex;

            // 回收不再显示的项
            for (int i = activeItems.Count - 1; i >= 0; i--)
            {
                int itemIndex = (int)activeItems[i].GetComponent<UI_ListBaseItem>().index;
                if (itemIndex < currentFirstIndex || itemIndex >= currentFirstIndex + visibleItems)
                {
                    ReturnToPool(activeItems[i]);
                    activeItems.RemoveAt(i);
                }
            }

            // 添加新项
            for (int i = currentFirstIndex; i < currentFirstIndex + visibleItems; i++)
            {
                if (i >= totalItems) break;

                bool alreadyActive = false;
                foreach (var item in activeItems)
                {
                    if ((int)item.GetComponent<UI_ListBaseItem>().index == i)
                    {
                        alreadyActive = true;
                        break;
                    }
                }

                if (!alreadyActive)
                {
                    UI_ListBaseItem item = GetPooledItem();
                    item.SetActive(true);

                    UI_ListBaseItem listItem = item.GetComponent<UI_ListBaseItem>();
                    

                    listItem.Initialize(dataList[i], i);

                    // 设置位置
                    float yPos = -i * (itemHeight + spacing.x);
                    (item.transform as RectTransform).anchoredPosition = new Vector2(0, yPos);

                    activeItems.Add(item);
                }
            }
        }

        public void RefreshCurrentShowItems()
        {
            if (currentShowItems == null || currentShowItems.Count < 1) return;
            for (int i = 0; i < currentShowItems.Count; i++)
            {
                if (updateItemData != null)
                {
                    updateItemData(currentShowItems[i],dataList[i]);
                }
            }
        }
        public List<UI_ListBaseItem> GetCurrentShowItems()
        {
            return currentShowItems;
        }

        void OnDestroy()
        {
            if (scrollRect != null)
            {
                scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
            }
        }
    }



}