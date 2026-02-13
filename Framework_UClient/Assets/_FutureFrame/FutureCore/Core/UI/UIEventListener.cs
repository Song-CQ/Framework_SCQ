/****************************************************
    文件：UIEventListener.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/2/23 18:50
    功能：UI事件监听器：管理所有UGUI事件，提供事件参数类
    附加到需要交互的UI元素上，用于监听用户的操作，类似于EventTrigger
    如果附加到3d物体上则需要添加碰撞器
*****************************************************/
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FutureCore
{
    public class UIEventListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
    {
        public delegate void PointerHandler(PointerEventData eventData);
        public static UIEventListener GetEventListener(Transform tf)
        {
            return tf.GetComponent<UIEventListener>() ?? tf.gameObject.AddComponent<UIEventListener>();
        }

        #region 事件传递设置

        [Header("UI事件传递")]
        [LabelText("是否传递给父级")]
        [SerializeField] private bool forwardEventsToParent = true;

        [LabelText("允许点击传递"), ShowIf(@"forwardEventsToParent"), FoldoutGroup("UIEvent")]
        [SerializeField] private bool allowClickPropagation = false; // 允许点击事件传递给父级
        [LabelText("允许拖拽传递"), ShowIf(@"forwardEventsToParent"), FoldoutGroup("UIEvent")]
        [SerializeField] private bool allowDragPropagation = true; // 允许拖拽事件传递给父级

        [Space(5)]
        [LabelText("自动查找ScrollRect")]
        [SerializeField] private bool autoFindScrollRect = true; // 自动查找ScrollRect

        private ScrollRect parentScrollRect;
        private bool hasParentScrollRect = false;

        private void Awake()
        {
            if (autoFindScrollRect)
            {
                FindParentScrollRect();
            }
        }
        // 方法1：查找父级所有ScrollRect，找到最近的
        private void FindParentScrollRect()
        {
            parentScrollRect = null;
            Transform parent = transform.parent;

            while (parent != null)
            {
                // 尝试获取当前父级的ScrollRect
                parentScrollRect = parent.GetComponent<ScrollRect>();
                if (parentScrollRect != null)
                {
                    hasParentScrollRect = true;
                    //LogUtil.LogFormat($"{name} 找到父级ScrollRect: {parentScrollRect.name}", this);
                    return;
                }

                // 继续向上查找
                parent = parent.parent;
            }

            hasParentScrollRect = false;
            // Debug.Log($"{name} 没有找到父级ScrollRect", this);
        }
        // 方法2：使用缓存和按需查找
        public ScrollRect GetParentScrollRect()
        {
            if (parentScrollRect != null) return parentScrollRect;

            // 如果没有缓存，查找一次
            if (autoFindScrollRect)
            {
                FindParentScrollRect();
            }

            return parentScrollRect;
        }

        // 方法3：使用属性，自动查找
        public ScrollRect ParentScrollRect
        {
            get
            {
                if (parentScrollRect == null && autoFindScrollRect)
                {
                    parentScrollRect = GetComponentInParent<ScrollRect>();
                    hasParentScrollRect = parentScrollRect != null;
                }
                return parentScrollRect;
            }
        }

        #endregion

        #region 鼠标指针事件

        /// <summary>
        /// 指针移入控件时
        /// </summary>
        public event PointerHandler PointerEnter_Event;//指针移入控件时
        /// <summary>
        /// 指针移出控件时
        /// </summary>
        public event PointerHandler PointerExit_Event;//指针移出控件时
        /// <summary>
        /// 鼠标按下控件时
        /// </summary>
        public event PointerHandler PointerDown_Event;//鼠标按下控件时
        /// <summary>
        /// 鼠标抬起控件时
        /// </summary>
        public event PointerHandler PointerUp_Event;//鼠标抬起控件时
        /// <summary>
        /// 鼠标单击控件时
        /// </summary>
        public event PointerHandler PointerClick_Event;//鼠标抬起控件时

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (PointerEnter_Event != null) PointerEnter_Event(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (PointerExit_Event != null) PointerExit_Event(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (PointerDown_Event != null) PointerDown_Event(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (PointerUp_Event != null) PointerUp_Event(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (PointerClick_Event != null) PointerClick_Event(eventData);
        }
        #endregion

        #region 拖拽事件

        public event PointerHandler InitializePotentialDrag;//当找到拖动但在开始拖动有效之前       
        public event PointerHandler BeginDrag;//开始拖动
        public event PointerHandler Drag;//正在拖动
        public event PointerHandler EndDrag;//拖动结束
        public event PointerHandler Drop;//拖动结束，松手的时候

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (InitializePotentialDrag != null) InitializePotentialDrag(eventData);

           
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (BeginDrag != null) BeginDrag(eventData);

            // 传递事件给父级ScrollRect
            if (allowDragPropagation && forwardEventsToParent)
            {
                var scrollRect = ParentScrollRect;
                if (scrollRect != null)
                {
                    // 手动调用ScrollRect的事件方法
                    ExecuteEvents.Execute<IBeginDragHandler>(
                        scrollRect.gameObject,
                        eventData,
                        (handler, data) => handler.OnBeginDrag((PointerEventData)data)
                    );
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Drag != null) Drag(eventData);
            if (allowDragPropagation && forwardEventsToParent)
            {
                var scrollRect = ParentScrollRect;
                if (scrollRect != null)
                {
                    ExecuteEvents.Execute<IDragHandler>(
                        scrollRect.gameObject,
                        eventData,
                        (handler, data) => handler.OnDrag((PointerEventData)data)
                    );
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (EndDrag != null) EndDrag(eventData);
            if (allowDragPropagation && forwardEventsToParent)
            {
                var scrollRect = ParentScrollRect;
                if (scrollRect != null)
                {
                    ExecuteEvents.Execute<IDragHandler>(
                        scrollRect.gameObject,
                        eventData,
                        (handler, data) => handler.OnDrag((PointerEventData)data)
                    );
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (Drop != null) Drop(eventData);

            if (allowDragPropagation && forwardEventsToParent)
            {
                var scrollRect = ParentScrollRect;
                if (scrollRect != null)
                {
                    ExecuteEvents.Execute<IEndDragHandler>(
                        scrollRect.gameObject,
                        eventData,
                        (handler, data) => handler.OnEndDrag((PointerEventData)data)
                    );
                }
            }
        }
        public void SetParentScrollRect(ScrollRect scrollRect)
        {
            parentScrollRect = scrollRect;
            hasParentScrollRect = scrollRect != null;
        }


        #endregion

        #region 鼠标事件
        public event PointerHandler Scroll;//鼠标滑轮滑动时

        public void OnScroll(PointerEventData eventData)
        {
            if (Scroll != null) Scroll(eventData);

            if (allowDragPropagation && forwardEventsToParent)
            {
                var scrollRect = ParentScrollRect;
                if (scrollRect != null)
                {
                    ExecuteEvents.Execute<IScrollHandler>(
                        scrollRect.gameObject,
                        eventData,
                        (handler, data) => handler.OnScroll((PointerEventData)data)
                    );
                }
            }
        }


        #endregion

        #region 点选事件
        public delegate void BaseHandler(BaseEventData eventData);

        /// <summary>
        /// 焦点选中时每帧执行
        /// </summary>
        public event BaseHandler UpdateSelected;//
        public event BaseHandler Select;//焦点选中时那一帧执行
        public event BaseHandler Deselect;//焦点离开时那一帧执行

        public void OnUpdateSelected(BaseEventData eventData)
        {
            if (UpdateSelected != null) UpdateSelected(eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (Select != null) Select(eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (Deselect != null) Deselect(eventData);
        }

        #endregion

        #region 选择事件
        public delegate void AxisEvent(AxisEventData eventData);
        //物体移动时(与InputManager里的Horizontal和Vertica按键相对应)，前提条件是物体被选中
        public event AxisEvent MoveAxis;
        //提交按钮被按下时(与InputManager里的Submit按键相对应，PC上默认的是Enter键)，前提条件是物体被选中
        public event BaseHandler Submit;
        //取消按钮被按下时(与InputManager里的Cancel按键相对应，PC上默认的是Esc键)，前提条件是物体被选中
        public event BaseHandler Cancel;

        public void OnMove(AxisEventData eventData)
        {
            if (MoveAxis != null) MoveAxis(eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (Submit != null) Submit(eventData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (Cancel != null) Cancel(eventData);
        }

        #endregion
    }
}
