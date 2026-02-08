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
using Sirenix.OdinInspector;
using System;
using UnityEditorInternal.VersionControl;



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


        [TitleGroup("组件引用")]
        [Required]

        [SerializeField] private ScrollRect scrollRect;
        [Required]

        [SerializeField] private RectTransform viewport;
        [Required]

        [SerializeField] private RectTransform content;
        [Required]
        [SerializeField] private GameObject itemPrefab;


        [FoldoutGroup("Layout Components")]
        [SerializeField] private LayoutGroup layoutGroup;
        [FoldoutGroup("Layout Components")]
        [SerializeField] private ContentSizeFitter contentSize;



        [TitleGroup("列表设置")]

        [LabelText("虚拟列表")]
        [SerializeField] private bool isVirtual = false; // 是否虚拟列表
        [FoldoutGroup("虚拟列表"), LabelText("缓冲区数量"), ShowIf("@isVirtual")]
        [SerializeField] private int bufferSize = 2; // 缓冲区数量
        [FoldoutGroup("虚拟列表"), LabelText("预加载数量"), ShowIf("@isVirtual")]
        [SerializeField] private int preloadItemSum = 5; // 预加载数量


        [FoldoutGroup("视窗设置")]
        [LabelText("自定义滑动设置"), FoldoutGroup("视窗设置")]
        [SerializeField] private bool useViewMovement = false;
        [LabelText("视窗移动类型"), FoldoutGroup("视窗设置"), ShowIf("@useViewMovement")]
        [SerializeField] private ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic;

        [LabelText("弹性系数"), FoldoutGroup("视窗设置"), ShowIf("@useViewMovement && movementType == ScrollRect.MovementType.Elastic")]
        [SerializeField] private float elasticity = 0.1f;

        [LabelText("视窗偏移"), FoldoutGroup("视窗设置")]
        [SerializeField] private bool useContentPadding = false;

        // 当 useContentPadding 为 true 时才显示这个折叠组
        [LabelText("视窗 左偏移"), FoldoutGroup("视窗设置"), ShowIf("@useContentPadding")]
        [SerializeField] private int padding_Left;
        [LabelText("视窗 右偏移"), FoldoutGroup("视窗设置"), ShowIf("@useContentPadding")]
        [SerializeField] private int padding_Right;
        [LabelText("视窗 上偏移"), FoldoutGroup("视窗设置"), ShowIf("@useContentPadding")]
        [SerializeField] private int padding_Top;
        [LabelText("视窗 下偏移"), FoldoutGroup("视窗设置"), ShowIf("@useContentPadding")]
        [SerializeField] private int padding_Bottom;



        [FoldoutGroup("子物体设置"), LabelText("间隔")]
        [SerializeField] private Vector2 spacing;
        [FoldoutGroup("子物体设置"), LabelText("使用子物体自身 Size")]
        [SerializeField] private bool useItemSize = true;
        [FoldoutGroup("子物体设置"), LabelText("Item Width"), HideIf("useItemSize")]
        [SerializeField] private float itemWidth = 100f;
        [FoldoutGroup("子物体设置"), LabelText("Item Height"), HideIf("useItemSize")]
        [SerializeField] private float itemHeight = 100f;

        [FoldoutGroup("子物体设置"), LabelText("排序方式")]
        [SerializeField] private LayoutGroupType layoutGroupType = LayoutGroupType.Horizontal;
        [FoldoutGroup("子物体设置"), LabelText("子物体对齐锚点")]
        [SerializeField] private TextAnchor anchor = TextAnchor.LowerLeft;







        private List<ItemData> dataList = new List<ItemData>();
        private List<BaseUIList_Item> activeItems = new List<BaseUIList_Item>();
        private List<BaseUIList_Item> currentShowItems = new List<BaseUIList_Item>();
        private Queue<BaseUIList_Item> itemPool = new Queue<BaseUIList_Item>();



        public delegate void UpdateItemData(BaseUIList_Item item, ItemData data);

        [HideInInspector]
        public UpdateItemData updateItemData;

        [HideInInspector]
        public UpdateItemData onCenterOnChild;

        [HideInInspector]
        public UpdateItemData changeSelectItemData;

        public Action<Vector2> SwipeEvent;

        private float viewportHeight;
        private float viewportWidth;
        private int totalItems;
        private int visibleItems;
        private int currentFirstIndex = 0;

        private bool isInit = false;



        private void Awake()
        {
            Initialize();
        }


        void Start()
        {
            RefreshList();
        }



        public void Initialize()
        {
            if (isInit) return;


            if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
            if (viewport == null) viewport = scrollRect.viewport;
            if (content == null) content = scrollRect.content;
            if (contentSize == null) contentSize = content.GetComponent<ContentSizeFitter>();

            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            scrollRect.horizontal = (layoutGroupType == LayoutGroupType.Grip || layoutGroupType == LayoutGroupType.Horizontal) ? true : false;
            scrollRect.vertical = (layoutGroupType == LayoutGroupType.Grip || layoutGroupType == LayoutGroupType.Version) ? true : false;

            if (useViewMovement)
            {
                scrollRect.movementType = movementType;
                if (movementType == ScrollRect.MovementType.Elastic)
                {
                    scrollRect.elasticity = elasticity;
                }
            }


            if (contentSize == null)
            {
                contentSize = content.gameObject.AddComponent<ContentSizeFitter>();

                contentSize.horizontalFit = (layoutGroupType == LayoutGroupType.Grip || layoutGroupType == LayoutGroupType.Horizontal) ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
                contentSize.verticalFit = (layoutGroupType == LayoutGroupType.Grip || layoutGroupType == LayoutGroupType.Version) ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;

            }


            layoutGroup = contentSize.GetComponent<LayoutGroup>();
            if (layoutGroup == null)
            {
                switch (layoutGroupType)
                {
                    case LayoutGroupType.Horizontal:
                        {
                            var _layoutGroup = content.gameObject.AddComponent<HorizontalLayoutGroup>();
                            _layoutGroup.spacing = spacing.x;
                            layoutGroup = _layoutGroup;
                        }
                        break;

                    case LayoutGroupType.Version:
                        {

                            var _layoutGroup = content.gameObject.AddComponent<VerticalLayoutGroup>();
                            _layoutGroup.spacing = spacing.y;
                            layoutGroup = _layoutGroup;

                        }
                        break;
                    case LayoutGroupType.Grip:
                        {
                            var _layoutGroup = content.gameObject.AddComponent<GridLayoutGroup>();
                            _layoutGroup.spacing = spacing;
                            _layoutGroup.cellSize = new Vector2(itemWidth, itemHeight);

                            layoutGroup = _layoutGroup;
                        }
                        break;

                }

                layoutGroup.padding = new RectOffset(padding_Left, padding_Right, padding_Top, padding_Bottom);
                layoutGroup.childAlignment = anchor;
            }

            viewportHeight = viewport.rect.height;
            viewportWidth = viewport.rect.width;

            if (isVirtual)
            {
                // 准备对象池
                PreparePool(preloadItemSum);
            }

            isInit = true;
        }

        void PreparePool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                BaseUIList_Item item = InstantiateItem();
                item.SetActive(false);
                itemPool.Enqueue(item);
            }
        }

        BaseUIList_Item GetPooledItem()
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

        private BaseUIList_Item InstantiateItem()
        {
            BaseUIList_Item item = Instantiate(itemPrefab, content).GetComponent<BaseUIList_Item>();
            if (!useItemSize)
            {
                item.SetSize(itemWidth, itemHeight);
            }
            return item;
        }

        void ReturnToPool(BaseUIList_Item item)
        {
            item.SetActive(false);
            itemPool.Enqueue(item);
        }



        public void SetData(List<ItemData> newData)
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
            currentShowItems.Clear();
            for (int i = 0; i < dataList.Count; i++)
            {
                BaseUIList_Item item = GetPooledItem();
                item.SetActive(true);

                activeItems.Add(item);
                currentShowItems.Add(item);
            }

            if (isVirtual)
            {
                // 计算内容高度
                float contentHeight = totalItems * itemHeight + (totalItems - 1) * spacing.x;
                content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

                // 计算可见项数量

                visibleItems = Mathf.CeilToInt(viewportHeight / (itemHeight + spacing.x)) + bufferSize * 2;
                visibleItems = Mathf.Min(visibleItems, totalItems);
            }

            // 显示初始项
            currentFirstIndex = 0;

            //刷新当前显示的Item数据
            RefreshCurrentShowItems();

        }

        void OnScrollValueChanged(Vector2 normalizedPos)
        {
            SwipeEvent?.Invoke(normalizedPos);

            if (isVirtual)
            {
                UpdateVisibleItems();
            }
        }
        void UpdateVisibleItems()
        {

            if (totalItems == 0) return;

            if (!isVirtual)
            {
                return;
            }


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
                int itemIndex = (int)activeItems[i].GetComponent<BaseUIList_Item>().Index;
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
                    if ((int)item.GetComponent<BaseUIList_Item>().Index == i)
                    {
                        alreadyActive = true;
                        break;
                    }
                }

                if (!alreadyActive)
                {
                    BaseUIList_Item item = GetPooledItem();
                    item.SetActive(true);

                    BaseUIList_Item listItem = item.GetComponent<BaseUIList_Item>();


                    listItem.Initialize(i, dataList[i]);

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
                    currentShowItems[i].Initialize(i, dataList[i]);
                    updateItemData(currentShowItems[i], dataList[i]);
                }
            }
        }
        public List<BaseUIList_Item> GetCurrentShowItems()
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