/****************************************************
    文件：UIEventListener.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/2/23 18:50
    功能：UI事件监听器：管理所有UGUI事件，提供事件参数类
    附加到需要交互的UI元素上，用于监听用户的操作，类似于EventTrigger
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XL.UI
{
    public class UIEventListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
    {
      
        public static UIEventListener GetEventListener(Transform tf)
        {
            return tf.GetComponent<UIEventListener>() ?? tf.gameObject.AddComponent<UIEventListener>();
        }

        public delegate void PointerHandler(PointerEventData eventData);
        #region 鼠标指针事件

        public event PointerHandler PointerEnter;//指针移入控件时
        public event PointerHandler PointerExit;//指针移出控件时
        public event PointerHandler PointerDown;//鼠标按下控件时
        public event PointerHandler PointerUp;//鼠标抬起控件时
        public event PointerHandler PointerClick;//鼠标单击控件时
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (PointerEnter != null) PointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (PointerExit != null) PointerExit(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (PointerDown != null) PointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (PointerUp != null) PointerUp(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (PointerClick != null) PointerClick(eventData);
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
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Drag != null) Drag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (EndDrag != null) EndDrag(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (Drop != null) Drop(eventData);
        }



        #endregion

        #region 鼠标事件
        public event PointerHandler Scroll;//鼠标滑轮滑动时

        public void OnScroll(PointerEventData eventData)
        {
            if (Scroll != null) Scroll(eventData);
        }


        #endregion

        #region 点选事件
        public delegate void BaseHandler(BaseEventData eventData);

        public event BaseHandler UpdateSelected;//焦点选中时每帧执行
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
