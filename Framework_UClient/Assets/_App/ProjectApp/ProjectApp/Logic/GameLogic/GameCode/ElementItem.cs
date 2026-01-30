using System;
using System.Diagnostics.Tracing;
using System.Text;
using DG.Tweening;
using FutureCore;
using ProjectApp;
using ProjectApp.Data;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlasticGui.PlasticTableColumn;
using static UnityEditor.LightingExplorerTableColumn;

namespace ProjectApp
{
    [Serializable]
    public struct ElementData
    {
        public int X;
        public int Y;
        public ElementType Type;
        public int data1;
        public int data2;
        public int data3;



        // Unity需要默认构造函数
        public ElementData(ElementType type = ElementType.Fixed_None)
        {
            X = 0;
            Y = 0;

            data1 = 0;
            data2 = 0;
            data3 = 0;

            Type = type;
        }
        public ElementData Set(ElementData data)
        {
            X = data.X;
            Y = data.Y;
            Type = data.Type;

            data1 = data.data1;
            data2 = data.data2;
            data3 = data.data3;
            return this;
        }


        public ElementData SetType(ElementType type)
        {
            Type = type;
            return this;
        }

        // 提供修改坐标的方法（返回新实例）
        public ElementData SetPot(int x, int y)
        {
            X = x;
            Y = y;
            return this;
        }


        // 实现 IEquatable
        public bool Equals(ElementData other)
        {
            return X == other.X && Y == other.Y && Type == other.Type;
        }



        // 重载运算符
        public static bool operator ==(ElementData left, ElementData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ElementData left, ElementData right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]:{2}", X, Y, Type);
        }


    }

    //元素类
    [Serializable]
    public class ElementItem : IRaycast3D
    {
        public Transform Transform { get; private set; }

        public ElementData Data { get; private set; }

        private SpriteRenderer icon;

        public GameObject selectGo;

        public Vector3 Pos
        {
            get => pos;
            set
            {
                pos = value;
                Transform.localPosition = pos;
            }
        }

        public Collider Collider { get; private set; }

        private Vector3 pos;
        public bool isSelect;

        private bool _active = false;
        public bool Active
        {
            get => _active;
        }

        private Transform changeTrf;

        private SpriteRenderer[] changeIcons = new SpriteRenderer[3];

        public ElementItem() { }

        public ElementItem(Transform transform)
        {
            Transform = transform;
            Collider = Transform.GetComponent<BoxCollider>();
            icon = Transform.Find("icon").GetComponent<SpriteRenderer>();
            selectGo = Transform.Find("selectGo").gameObject;
            Transform.gameObject.AddComponent<DebugElementItem>().Init(this);

            changeTrf = Transform.Find("changeIcon");
            changeIcons[0] = changeTrf.GetChild(0).GetComponent<SpriteRenderer>();
            changeIcons[1] = changeTrf.GetChild(1).GetComponent<SpriteRenderer>();
            changeIcons[2] = changeTrf.GetChild(2).GetComponent<SpriteRenderer>();


            _active = Transform.gameObject.activeSelf;
            SetActive(true);


        }


        public void Init(ElementData _data)
        {

            SetData(_data);
            RefreshView();

            // 设置初始布局
            UpdateIconLayout();

        }

        public void SetData(ElementData _data)
        {
            Data = _data;
        }

        public void SetActive(bool value)
        {
            if (value != _active)
            {
                Transform.SetActive(value);
                _active = value;
            }

        }


        public void RefreshView()
        {
            if (ElementTypeTool.CheckType_HasIcon(Data.Type))
            {
                icon.sprite = GameTool.GetSprite(Data.Type);
            }

            if (Data.Type == ElementType.Item_Change)
            {
                changeTrf.gameObject.SetActive(true);

                changeIcons[0].sprite = GameTool.GetSprite((ElementType)Data.data1);
                changeIcons[1].sprite = GameTool.GetSprite((ElementType)Data.data2);
                changeIcons[2].sprite = GameTool.GetSprite((ElementType)Data.data3);

            }
            else
            {
                changeTrf.gameObject.SetActive(false);
            }

            selectGo.SetActive(isSelect);

        }

        public void Release()
        {
            SetSelect(false);
            SetActive(false);

        }

        public void SetSelect(bool v)
        {
            if (v == isSelect) return;
            isSelect = v;

            selectGo.SetActive(isSelect);

        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="hitPoint"></param>
        public void Raycast_OnClick(Vector3 hitPoint)
        {

            GameTool.GameCore.ClickElementItem(this);
            selectGo.SetActive(isSelect);
        }

        /// <summary>
        /// 滑动
        /// </summary>
        /// <param name="hitPoint"></param>
        public void Raycast_OnSwipe(Vector3 startPoint, Vector3 endPoint, IRaycast3D raycast3D_end)
        {
            ElementItem endItem = (raycast3D_end as ElementItem);

            if (endItem != this)
            {
                GameTool.GameCore.SwipeItemToItem(this, endItem);
            }
            else
            {
                Vector2 dir = startPoint - endPoint;
                GameTool.GameCore.SwipeElementItem(this, dir.normalized);
            }



        }


        public void StopAllDOTween()
        {
            Transform.localScale = Vector3.one;
            Transform.rotation = Quaternion.identity;
            pos = Vector3.zero;

        }






        private int currentIndex = 0; // 当前中间的图标索引

        [Header("动画设置")]
        [SerializeField] private float switchDuration = 0.3f;
        [SerializeField] private float centerScale = 0.7f;
        [SerializeField] private float sideScale = 0.4f;
        [SerializeField] private Vector3 topLeftPosition = new Vector3(-0.357f, 0.345f, -0.1f);
        [SerializeField] private Vector3 centerPosition = new Vector3(0f, 0f, -0.2f);
        [SerializeField] private Vector3 bottomRightPosition = new Vector3(0.397f, -0.345f, -0.1f);



        // 切换到下一个图标
        public void SwitchToNext()
        {

            UpdateIconLayout();
            // 计算下一个索引
            int nextIndex = (currentIndex + 1) % 3;
            PerformSwitchAnimation(true);
        }

        // 切换到上一个图标
        public void SwitchToPrevious()
        {

            UpdateIconLayout();
            // 计算上一个索引
            int prevIndex = (currentIndex - 1 + 3) % 3;
            PerformSwitchAnimation(false);
        }


        private bool isAnimating = false;

        private Sequence switchSequenceR;
        private Sequence switchSequenceL;
        private void PerformSwitchAnimation(bool isR)
        {
            // 停止所有现有动画
            switchSequenceR?.Pause();
            switchSequenceL?.Pause();
            if (isR)
            {
                if (switchSequenceR == null)
                {
                    // 执行滑动动画
                    Sequence sequence = DOTween.Sequence().SetAutoKill(false);

                    // 左上图标移动到中间
                    sequence.Join(changeIcons[0].transform
                        .DOLocalMove(centerPosition, switchDuration));
                    sequence.Join(changeIcons[0].transform
                        .DOScale(centerScale, switchDuration));

                    // 中间图标移动到右下
                    sequence.Join(changeIcons[2].transform
                        .DOLocalMove(bottomRightPosition, switchDuration));
                    sequence.Join(changeIcons[2].transform
                        .DOScale(sideScale, switchDuration));

                    // 右下图标移动到左上（完成循环）
                    sequence.Join(changeIcons[1].transform
                        .DOLocalMove(topLeftPosition, switchDuration));
                    sequence.Join(changeIcons[1].transform
                        .DOScale(sideScale, switchDuration));

                    // 更新显示顺序
                    sequence.SetEase(Ease.OutCubic).onComplete = () => UpdateIconLayout();
                }
                else
                {
                    switchSequenceR.Restart();
                }
            }
            else
            {
                if (switchSequenceL == null)
                {
                    // 执行滑动动画
                    Sequence sequence = DOTween.Sequence().SetAutoKill(false);

                    // 右下图标移动到中间
                    sequence.Join(changeIcons[2].transform
                        .DOLocalMove(centerPosition, switchDuration));
                    sequence.Join(changeIcons[2].transform
                        .DOScale(centerScale, switchDuration));

                    // 中间图标移动到左上
                    sequence.Join(changeIcons[0].transform
                        .DOLocalMove(topLeftPosition , switchDuration));
                    sequence.Join(changeIcons[0].transform
                        .DOScale(sideScale, switchDuration));

                    // 左上图标移动到右下（完成循环）
                    sequence.Join(changeIcons[1].transform
                        .DOLocalMove(bottomRightPosition,switchDuration));
                    sequence.Join(changeIcons[1].transform
                        .DOScale(sideScale, switchDuration));

                    // 更新显示顺序
                    sequence.SetEase(Ease.OutCubic).onComplete = () => UpdateIconLayout();
                }
                else
                {
                    switchSequenceL.Restart();
                }
            }

        }

        // 更新图标布局（无动画）
        private void UpdateIconLayout()
        {

            // 设置位置和大小
            changeIcons[1].transform.localPosition = topLeftPosition;
            changeIcons[1].transform.localScale = Vector3.one * sideScale;

            changeIcons[0].transform.localPosition = centerPosition;
            changeIcons[0].transform.localScale = Vector3.one * centerScale;

            changeIcons[2].transform.localPosition = bottomRightPosition;
            changeIcons[2].transform.localScale = Vector3.one * sideScale;

            changeIcons[0].sprite = GameTool.GetSprite((ElementType)Data.data1);
            changeIcons[1].sprite = GameTool.GetSprite((ElementType)Data.data2);
            changeIcons[2].sprite = GameTool.GetSprite((ElementType)Data.data3);
        }

        // 重置位置（用于调试）
        public void ResetPositions()
        {
            isAnimating = false;
            currentIndex = 0;
            UpdateIconLayout();
        }

        void OnDestroy()
        {
            // 清理DOTween动画
            switchSequenceR.Kill();
            switchSequenceR = null;
        }





    }


}



public class DebugElementItem : MonoBehaviour
{
    public ElementItem elementItem;

    [SerializeField]
    public ElementData Data;
    public bool isSelect;

    public ElementType newType;


    public System.Collections.Generic.List<string> InfoText = new System.Collections.Generic.List<string>();
    string oldname;

    [Button("设置新类型")]
    public void SetData()
    {
        Data.SetType(newType);
        elementItem.SetData(Data);

        GameTool.GameCore.Data.SetElementData(Data);
        elementItem.RefreshView();
    }

    public void Init(ElementItem _elementItem)
    {
        elementItem = _elementItem;
        oldname = string.Format("{0}_{1} : {2}", Data.X, Data.Y, Data.Type);

        InfoText.Add(oldname);
    }

    [Button("next")]
    public void SwitchToNext()
        {
        if (elementItem == null) return;
        elementItem.SwitchToNext();

    }
    [Button("previou")]
    public void SwitchToPrevious()
        {
        if (elementItem == null) return;
        elementItem.SwitchToPrevious();

    }

    public void Update()
    {
        if (elementItem == null) return;

        Data = elementItem.Data;
        isSelect = elementItem.isSelect;
        //elementItem.selectGo.SetActive(isSelect);
        transform.name = string.Format("{0}_{1} : {2}", Data.X, Data.Y, Data.Type);
        if (transform.name != oldname)
        {
            oldname = transform.name;

            InfoText.Add(oldname);
        }


    }
}

