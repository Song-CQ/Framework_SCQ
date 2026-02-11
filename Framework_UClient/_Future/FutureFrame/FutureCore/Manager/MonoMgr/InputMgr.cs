using System;
using UnityEngine;

namespace FutureCore
{
    public sealed class InputMgr : BaseMonoMgr<InputMgr>
    {
        public static event Action<Vector2> Event_ClickScreen;

        public static event Action Event_UpData;
        public static event Action Event_SecondUpData;

        // 双指触摸事件
        public static event Action<float> Event_PinchZoom;           // 缩放事件，返回缩放增量
        public static event Action<float> Event_TwoFingerRotate;    // 旋转事件，返回角度增量
        public static event Action<Vector2> Event_TwoFingerDrag;    // 双指滑动事件，返回滑动向量
        
        // 触摸状态
        public static bool IsTwoFingerTouching { get; private set; }
        public static Vector2 TwoFingerCenter { get; private set; } // 双指中心点

        private float timeTemp_Second = 0;

        // 双指触摸相关变量
        private float previousDistance;
        private float previousAngle;
        private Vector2 previousCenter;
        private bool isTwoFingerValid = false;

        public override void Init()
        {
            base.Init();
            Event_UpData += MainThreadLog.LoopLog;
        }

        private void Update()
        {
            if (!IsStartUp || IsDispose)
            {
                return;
            }

            // 单指点击检测
            if (Input.GetMouseButtonDown(0) && Input.touchCount <= 1)
            {
                Event_ClickScreen?.Invoke(Input.mousePosition);
            }

            // 双指触摸检测
            DetectTwoFingerGestures();

            Event_UpData?.Invoke();
            
            timeTemp_second += Time.deltaTime;
            if (timeTemp_second >= 1)
            {
                timeTemp_second = 0;
                Event_SecondUpData?.Invoke();
            }
        }

        /// <summary>
        /// 检测双指手势（缩放、旋转、滑动）
        /// </summary>
        private void DetectTwoFingerGestures()
        {
            // 检查是否为双指触摸
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                // 忽略刚触摸的帧，防止突变
                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    isTwoFingerValid = false;
                    IsTwoFingerTouching = true;
                    return;
                }

                // 初始化双指状态
                if (!isTwoFingerValid)
                {
                    previousDistance = Vector2.Distance(touch1.position, touch2.position);
                    previousAngle = GetAngle(touch1.position, touch2.position);
                    previousCenter = (touch1.position + touch2.position) * 0.5f;
                    isTwoFingerValid = true;
                    IsTwoFingerTouching = true;
                    return;
                }

                // 当前帧数据
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                float currentAngle = GetAngle(touch1.position, touch2.position);
                Vector2 currentCenter = (touch1.position + touch2.position) * 0.5f;

                // 1. 缩放检测（捏合）
                float deltaDistance = currentDistance - previousDistance;
                if (Mathf.Abs(deltaDistance) > 0.01f) // 忽略微小变化
                {
                    Event_PinchZoom?.Invoke(deltaDistance);
                }

                // 2. 旋转检测
                float deltaAngle = Mathf.DeltaAngle(previousAngle, currentAngle);
                if (Mathf.Abs(deltaAngle) > 0.1f) // 忽略微小角度变化
                {
                    Event_TwoFingerRotate?.Invoke(deltaAngle);
                }

                // 3. 双指滑动检测
                Vector2 deltaCenter = currentCenter - previousCenter;
                if (deltaCenter.magnitude > 0.1f)
                {
                    Event_TwoFingerDrag?.Invoke(deltaCenter);
                }

                // 更新上一帧数据
                previousDistance = currentDistance;
                previousAngle = currentAngle;
                previousCenter = currentCenter;
                TwoFingerCenter = currentCenter;
            }
            else
            {
                // 重置双指状态
                if (IsTwoFingerTouching)
                {
                    IsTwoFingerTouching = false;
                    isTwoFingerValid = false;
                }
            }
        }

        /// <summary>
        /// 计算两点间的角度（相对于屏幕X轴）
        /// </summary>
        private float GetAngle(Vector2 p1, Vector2 p2)
        {
            Vector2 dir = p1 - p2;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return angle;
        }

        /// <summary>
        /// 获取当前双指距离
        /// </summary>
        public static float GetTwoFingerDistance()
        {
            if (Input.touchCount == 2)
            {
                return Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            }
            return 0;
        }

        /// <summary>
        /// 检查是否为有效的双指操作
        /// </summary>
        public static bool IsValidTwoFingerGesture()
        {
            return Input.touchCount == 2;
        }

        private float timeTemp_Second; // 修复变量名不一致的问题
    }
}