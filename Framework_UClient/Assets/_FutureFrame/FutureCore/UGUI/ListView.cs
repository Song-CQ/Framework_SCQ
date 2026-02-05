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


namespace FutureCore
{

    /// <summary>
    /// 基础滑动列表
    /// </summary>
    public class ListView : MonoBehaviour
    {
        [Header("组件引用")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab;

        [Header("列表设置")]
        [SerializeField] private float itemSpacing = 10f;
        [SerializeField] private float itemHeight = 100f;
        [SerializeField] private int bufferSize = 2; // 缓冲区数量

        private List<ListItemData> dataList = new List<ListItemData>();
        private List<GameObject> activeItems = new List<GameObject>();
        private Queue<GameObject> itemPool = new Queue<GameObject>();

        private float viewportHeight;
        private int totalItems;
        private int visibleItems;
        private int currentFirstIndex = 0;

        public class ListItemData
        {
            public string title;
            public string description;
            public Sprite icon;
            public object userData;

            public ListItemData(string title, string desc, Sprite icon = null, object data = null)
            {
                this.title = title;
                this.description = desc;
                this.icon = icon;
                this.userData = data;
            }
        }

        void Start()
        {
            Initialize();

            // 测试数据
            GenerateTestData(50);
            RefreshList();
        }

        void Initialize()
        {
            if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
            if (viewport == null) viewport = scrollRect.viewport;
            if (content == null) content = scrollRect.content;

            viewportHeight = viewport.rect.height;
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);

            // 设置垂直滑动
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            // 准备对象池
            PreparePool(20);
        }

        void PreparePool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject item = Instantiate(itemPrefab, content);
                item.SetActive(false);
                itemPool.Enqueue(item);
            }
        }

        GameObject GetPooledItem()
        {
            if (itemPool.Count > 0)
            {
                return itemPool.Dequeue();
            }
            else
            {
                return Instantiate(itemPrefab, content);
            }
        }

        void ReturnToPool(GameObject item)
        {
            item.SetActive(false);
            itemPool.Enqueue(item);
        }

        void GenerateTestData(int count)
        {
            dataList.Clear();
            for (int i = 0; i < count; i++)
            {
                dataList.Add(new ListItemData(
                    $"Item {i + 1}",
                    $"Description for item {i + 1}\nThis is sample content.",
                    null,
                    i
                ));
            }
            totalItems = dataList.Count;
        }

        public void SetData(List<ListItemData> newData)
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
            float contentHeight = totalItems * itemHeight + (totalItems - 1) * itemSpacing;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

            // 计算可见项数量
            visibleItems = Mathf.CeilToInt(viewportHeight / (itemHeight + itemSpacing)) + bufferSize * 2;
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
            if (totalItems == 0) return;

            // 计算应该显示的第一个索引
            float scrollPos = content.anchoredPosition.y;
            int newFirstIndex = Mathf.FloorToInt(scrollPos / (itemHeight + itemSpacing));
            newFirstIndex = Mathf.Max(0, newFirstIndex - bufferSize);

            // 如果索引没变化，不更新
            if (newFirstIndex == currentFirstIndex) return;

            // 更新索引
            int oldFirstIndex = currentFirstIndex;
            currentFirstIndex = newFirstIndex;

            // 回收不再显示的项
            for (int i = activeItems.Count - 1; i >= 0; i--)
            {
                int itemIndex = (int)activeItems[i].GetComponent<ListItem>().index;
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
                    if ((int)item.GetComponent<ListItem>().index == i)
                    {
                        alreadyActive = true;
                        break;
                    }
                }

                if (!alreadyActive)
                {
                    GameObject item = GetPooledItem();
                    item.SetActive(true);

                    ListItem listItem = item.GetComponent<ListItem>();
                    if (listItem == null) listItem = item.AddComponent<ListItem>();

                    listItem.Initialize(dataList[i], i);

                    // 设置位置
                    float yPos = -i * (itemHeight + itemSpacing);
                    (item.transform as RectTransform).anchoredPosition = new Vector2(0, yPos);

                    activeItems.Add(item);
                }
            }
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