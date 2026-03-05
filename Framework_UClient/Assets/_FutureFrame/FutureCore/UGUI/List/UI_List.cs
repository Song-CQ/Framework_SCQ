/****************************************************
    �ļ�: ListView.cs
    ����: Clear
    ����: 2026/2/6 1:35:24
    ����: UI
    ����: ���������б�
*****************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;



namespace FutureCore
{

    /// <summary>
    /// ���������б� ��ֻ��Ҫ�����б� �� item����̳�UI_ListBaseItem��
    /// </summary>
    public class UI_List : MonoBehaviour
    {
        enum LayoutGroupType
        {
            Horizontal,
            Version,

            Grip

        }


        [TitleGroup("�������")]
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



        [TitleGroup("�б�����")]

        [LabelText("�����б�")]
        [SerializeField] private bool isVirtual = false; // �Ƿ������б�
        [FoldoutGroup("�����б�"), LabelText("����������"), ShowIf("@isVirtual")]
        [SerializeField] private int bufferSize = 2; // ����������
        [FoldoutGroup("�����б�"), LabelText("Ԥ��������"), ShowIf("@isVirtual")]
        [SerializeField] private int preloadItemSum = 5; // Ԥ��������


        [FoldoutGroup("�Ӵ�����")]
        [LabelText("�Զ��廬������"), FoldoutGroup("�Ӵ�����")]
        [SerializeField] private bool useViewMovement = false;
        [LabelText("�Ӵ��ƶ�����"), FoldoutGroup("�Ӵ�����"), ShowIf("@useViewMovement")]
        [SerializeField] private ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic;

        [LabelText("����ϵ��"), FoldoutGroup("�Ӵ�����"), ShowIf("@useViewMovement && movementType == ScrollRect.MovementType.Elastic")]
        [SerializeField] private float elasticity = 0.1f;

        [LabelText("�Ӵ�ƫ��"), FoldoutGroup("�Ӵ�����")]
        [SerializeField] private bool useContentPadding = false;

        // �� useContentPadding Ϊ true ʱ����ʾ����۵���
        [LabelText("�Ӵ� ��ƫ��"), FoldoutGroup("�Ӵ�����"), ShowIf("@useContentPadding")]
        [SerializeField] private int padding_Left;
        [LabelText("�Ӵ� ��ƫ��"), FoldoutGroup("�Ӵ�����"), ShowIf("@useContentPadding")]
        [SerializeField] private int padding_Right;
        [LabelText("�Ӵ� ��ƫ��"), FoldoutGroup("�Ӵ�����"), ShowIf("@useContentPadding")]
        [SerializeField] private int padding_Top;
        [LabelText("�Ӵ� ��ƫ��"), FoldoutGroup("�Ӵ�����"), ShowIf("@useContentPadding")]
        [SerializeField] private int padding_Bottom;



        [FoldoutGroup("����������"), LabelText("���")]
        [SerializeField] private Vector2 spacing;
        [FoldoutGroup("����������"), LabelText("ʹ������������ Size")]
        [SerializeField] private bool useItemSize = true;
        [FoldoutGroup("����������"), LabelText("Item Width"), HideIf("useItemSize")]
        [SerializeField] private float itemWidth = 100f;
        [FoldoutGroup("����������"), LabelText("Item Height"), HideIf("useItemSize")]
        [SerializeField] private float itemHeight = 100f;

        [FoldoutGroup("����������"), LabelText("����ʽ")]
        [SerializeField] private LayoutGroupType layoutGroupType = LayoutGroupType.Horizontal;
        [FoldoutGroup("����������"), LabelText("���������ê��")]
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
                // ׼�������
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
            // ��յ�ǰ��ʾ��
            foreach (var item in activeItems)
            {
                ReturnToPool(item);
            }
            activeItems.Clear();
            currentShowItems.Clear();
            for (int i = 0; i < totalItems; i++)
            {
                BaseUIList_Item item = GetPooledItem();
                item.SetActive(true);

                activeItems.Add(item);
                currentShowItems.Add(item);
            }

            if (isVirtual)
            {
                // �������ݸ߶�
                float contentHeight = totalItems * itemHeight + (totalItems - 1) * spacing.x;
                content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

                // ����ɼ�������

                visibleItems = Mathf.CeilToInt(viewportHeight / (itemHeight + spacing.x)) + bufferSize * 2;
                visibleItems = Mathf.Min(visibleItems, totalItems);
            }

            // ��ʾ��ʼ��
            currentFirstIndex = 0;

            //ˢ�µ�ǰ��ʾ��Item����
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


            // ����Ӧ����ʾ�ĵ�һ������
            float scrollPos = content.anchoredPosition.y;
            int newFirstIndex = Mathf.FloorToInt(scrollPos / (itemHeight + spacing.x));
            newFirstIndex = Mathf.Max(0, newFirstIndex - bufferSize);

            // �������û�仯��������
            if (newFirstIndex == currentFirstIndex) return;

            // ��������
            int oldFirstIndex = currentFirstIndex;
            currentFirstIndex = newFirstIndex;

            // ���ղ�����ʾ����
            for (int i = activeItems.Count - 1; i >= 0; i--)
            {
                int itemIndex = (int)activeItems[i].GetComponent<BaseUIList_Item>().Index;
                if (itemIndex < currentFirstIndex || itemIndex >= currentFirstIndex + visibleItems)
                {
                    ReturnToPool(activeItems[i]);
                    activeItems.RemoveAt(i);
                }
            }

            // �������
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

                    ItemData itemData = null;
                    if (dataList != null)
                    {
                        itemData = dataList[i];
                    }
                    listItem.Initialize(i, itemData); 

                    // ����λ��
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
                    ItemData itemData = null;
                    if (dataList != null)
                    {
                        itemData = dataList[i];
                    }
                    currentShowItems[i].Initialize(i, itemData);
                    updateItemData(currentShowItems[i], itemData);
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