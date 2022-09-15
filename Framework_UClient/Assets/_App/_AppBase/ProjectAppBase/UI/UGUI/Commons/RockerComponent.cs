/****************************************************
    文件：RockerComponent.cs
    作者：相柳
    邮箱: SongCQ-XL@Outlook.com
    日期：2020/1/29 18:50:0
    功能：摇杆组件
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FutureCore;

namespace ProjectApp.UGUI
{
    public class RockerComponent : MonoBehaviour
    {
        #region 摇杆相关
        [Tooltip("摇杆出现面板"), SerializeField]

        private Image imgTouch;
        [Tooltip("摇杆背景"), SerializeField]
        private Image imgDirBg;
        [Tooltip("摇杆中心点"), SerializeField]
        private Image imgDirPoint;
        [Tooltip("箭头图标"), SerializeField]
        private RectTransform ArrowsRect;

        //按下的坐标
        private Vector2 startPoint;
        public bool IsFixed = false;

        private enum UpRestType
        {
            None,
            /// <summary>
            /// 消失
            /// </summary>
            Vanish,
            /// <summary>
            /// 重置回原点
            /// </summary>
            RestStartPot
        }

        [Tooltip("摇杆抬起效果"), SerializeField]
        private UpRestType upRestType = UpRestType.None;
        private Vector2 imgDirBgStartPot;



        //摇杆最大拖拽距离

        public float MaxPoint;
        public float ScreenHeight;
        [HideInInspector]
        public Vector2 currentDir;


        private Vector3 imgDirPointStartPot;
        #endregion

        public event Action<Vector2> StartDrag;
        public event Action<Vector2> Drag;
        public event Action EndDrag;

        public UnityEvent_StartDrag OnStartDrag;
        public UnityEvent_Drag OnDrag;
        public UnityEvent_EndDrag OnEndDrag;

        [Serializable]
        public class UnityEvent_StartDrag : UnityEvent<Vector2>
        {
            public UnityEvent_StartDrag() { }
        }
        [Serializable]
        public class UnityEvent_Drag : UnityEvent<Vector2>
        {
            public UnityEvent_Drag() { }
        }
        [Serializable]
        public class UnityEvent_EndDrag : UnityEvent<Vector2>
        {
            public UnityEvent_EndDrag() { }
        }



        private void Awake()
        {

            MaxPoint = Screen.height / ScreenHeight * MaxPoint;
            imgDirPointStartPot = imgDirPoint.transform.localPosition;
            RegisterTouchevts();
            ArrowsRect?.SetActive(false);
        }


        /// <summary>
        /// 摇杆注册事件
        /// </summary>
        public void RegisterTouchevts()
        {
            UIEventListener uIEventListener;
            //按下事件
            if (!IsFixed)
            {
                uIEventListener = UIEventListener.GetEventListener(imgTouch.transform);
            }
            else
            {
                uIEventListener = UIEventListener.GetEventListener(imgDirBg.transform);
            }



            uIEventListener.BeginDrag += (e) =>
              {

                  switch (upRestType)
                  {
                      case UpRestType.None:
                          break;
                      case UpRestType.Vanish:
                          imgDirBg.SetActive(true);
                          break;
                      case UpRestType.RestStartPot:
                          imgDirBgStartPot = imgDirBg.rectTransform.position;
                          break;
                      default:
                          break;
                  }
                  ArrowsRect?.SetActive(true);

                  Vector2 endPoint = Vector2.zero;
                  if (!IsFixed)
                  {
                      imgDirBg.gameObject.SetActive(true);
                      imgDirBg.transform.position = e.position;
                      startPoint = e.position;
                  }
                  else
                  {
                      startPoint = imgDirBg.transform.position;
                      endPoint = e.position - startPoint;
                      if (endPoint.magnitude > MaxPoint)
                      {
                          Vector2 max = Vector2.ClampMagnitude(endPoint, MaxPoint);
                          imgDirPoint.transform.position = startPoint + max;
                      }
                      else
                      {
                          imgDirPoint.transform.position = e.position;
                      }
                      currentDir = endPoint.normalized;

                  }
                  OnStartDrag?.Invoke(endPoint);
                  StartDrag?.Invoke(endPoint);
              };
            //抬起事件
            uIEventListener.EndDrag += (e) =>
             {

                 startPoint = Vector2.zero;

                 imgDirPoint.rectTransform.localPosition = imgDirPointStartPot;
                 switch (upRestType)
                 {
                     case UpRestType.None:
                         break;
                     case UpRestType.Vanish:
                         imgTouch.SetActive(false);
                         break;
                     case UpRestType.RestStartPot:
                         imgDirBg.rectTransform.position = imgDirBgStartPot;
                         break;
                     default:
                         break;
                 }
                 ArrowsRect?.SetActive(false);

                 //输出向量
                 currentDir = Vector2.zero;
                 OnEndDrag?.Invoke(e.position);
                 EndDrag?.Invoke();
             };
            //拖拽
            uIEventListener.Drag += (e) =>
              {


                  Vector2 endPoint = e.position - startPoint;
                  if (endPoint.magnitude > MaxPoint)
                  {
                      Vector2 max = Vector2.ClampMagnitude(endPoint, MaxPoint);
                      imgDirPoint.transform.position = startPoint + max;
                      endPoint = max;
                  }
                  else
                  {
                      imgDirPoint.transform.position = e.position;
                  }
                  currentDir = endPoint;
                  if (ArrowsRect != null)
                  {
                      float Angle = Vector2.Angle(Vector2.up, currentDir.normalized);


                      if (Vector3.Cross(Vector2.up, currentDir.normalized).z < 0)
                      {
                          Angle *= -1;
                      }
                      ArrowsRect.localEulerAngles = new Vector3(0, 0, Angle);
                  }
                  Drag?.Invoke(currentDir);
                  OnDrag?.Invoke(currentDir);
              };



        }



    }

}
