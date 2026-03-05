using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FutureCore
{
    /// <summary>
    /// 输入管理器 - 统一管理触摸、点击、滑动、拖拽等输入事件
    /// 单例模式，继承自BaseMonoMgr，确保全局唯一
    /// </summary>
    public sealed class InputMgr : BaseMonoMgr<InputMgr>
    {
        /// <summary>
        /// 是否忽略UI检测（即使点击在UI上也触发事件）
        /// 默认false：点击在UI上不触发事件
        /// true：即使点击在UI上也触发事件
        /// </summary>
        public static bool IgnoreUICheck { get; set; } = false;

        // ==================== 事件定义区域 ====================

        /// <summary>全局点击事件，任何点击都会触发（包括点击在UI上）</summary>
        public static event Action<Vector2> OnScreenClick;

        // 点击相关事件
        /// <summary>屏幕点击事件，参数为点击的屏幕坐标(Vector2)</summary>
        public static event Action<Vector2> OnClick;
        /// <summary>游戏对象点击事件，参数为被点击的GameObject</summary>
        public static event Action<GameObject> OnGameObjectClick;

        // 滑动相关事件
        /// <summary>完整滑动事件，参数：滑动方向、起始坐标、结束坐标</summary>
        public static event Action<SwipeDirection, Vector2, Vector2> OnSwipe;
        /// <summary>简化滑动事件，参数：仅滑动方向</summary>
        public static event Action<SwipeDirection> OnSwipeSimple;

        // 长按相关事件
        /// <summary>长按事件，参数：长按位置的屏幕坐标</summary>
        public static event Action<Vector2> OnLongPress;

        // 拖拽相关事件
        /// <summary>拖拽开始事件，参数：起始坐标、当前坐标</summary>
        public static event Action<Vector2, Vector2> OnDragStart;
        /// <summary>拖拽过程中事件，参数：上一帧坐标、当前坐标</summary>
        public static event Action<Vector2, Vector2> OnDrag;
        /// <summary>拖拽结束事件，参数：上一帧坐标、结束坐标</summary>
        public static event Action<Vector2, Vector2> OnDragEnd;

        // ==================== 双指事件区域 ====================

        /// <summary>双指缩放事件，参数：缩放增量(像素)</summary>
        public static event Action<float> OnPinchZoom;
        /// <summary>双指旋转事件，参数：旋转角度增量</summary>
        public static event Action<float> OnTwoFingerRotate;
        /// <summary>双指滑动事件，参数：滑动向量</summary>
        public static event Action<Vector2> OnTwoFingerDrag;
        /// <summary>双指点击事件，参数：双指中心点</summary>
        public static event Action<Vector2> OnTwoFingerTap;
        /// <summary>双指长按事件，参数：双指中心点</summary>
        public static event Action<Vector2> OnTwoFingerLongPress;
        /// <summary>双指开始触摸事件</summary>
        public static event Action OnTwoFingerTouchStart;
        /// <summary>双指结束触摸事件</summary>
        public static event Action OnTwoFingerTouchEnd;

        // ==================== 配置常量区域 ====================

        /// <summary>最小滑动距离(像素)，低于此距离视为点击</summary>
        private const float MIN_SWIPE_DISTANCE = 50f;
        /// <summary>最大滑动时间(秒)，超过此时间视为无效滑动</summary>
        private const float MAX_SWIPE_TIME = 0.7f;
        /// <summary>长按判定时间(秒)，按住超过此时间触发长按事件</summary>
        private const float LONG_PRESS_TIME = 1.0f;
        /// <summary>拖拽判定距离(像素)，移动超过此距离视为拖拽开始</summary>
        private const float DRAG_START_DISTANCE = 5f;
        /// <summary>点击判定距离(像素)，移动距离小于此值视为点击</summary>
        private const float CLICK_DISTANCE_THRESHOLD = 10f;

        // ==================== 双指配置常量 ====================

        /// <summary>双指点击判定时间(秒)</summary>
        private const float TWO_FINGER_TAP_TIME = 0.3f;
        /// <summary>双指长按判定时间(秒)</summary>
        private const float TWO_FINGER_LONG_PRESS_TIME = 1.0f;
        /// <summary>双指缩放灵敏度系数</summary>
        private const float PINCH_ZOOM_SENSITIVITY = 0.01f;
        /// <summary>双指旋转灵敏度系数</summary>
        private const float ROTATE_SENSITIVITY = 0.5f;
        /// <summary>双指滑动灵敏度系数</summary>
        private const float TWO_FINGER_DRAG_SENSITIVITY = 0.01f;

        // ==================== 状态变量区域 ====================

        /// <summary>触摸开始时的屏幕坐标</summary>
        private Vector2 touchStartPos;
        /// <summary>上一次记录的触摸坐标（用于拖拽增量计算）</summary>
        private Vector2 lastTouchPos;
        /// <summary>触摸开始的时间戳</summary>
        private float touchStartTime;
        /// <summary>是否正在触摸中</summary>
        private bool isTouching = false;
        /// <summary>是否正在拖拽中</summary>
        private bool isDragging = false;
        /// <summary>长按事件是否已触发（防止重复触发）</summary>
        private bool isLongPressInvoked = false;

        // ==================== 双指状态变量 ====================

        /// <summary>双指状态</summary>
        private TwoFingerState twoFingerState = TwoFingerState.None;
        /// <summary>双指开始触摸时间</summary>
        private float twoFingerStartTime;
        /// <summary>双指上一帧的距离</summary>
        private float previousTwoFingerDistance;
        /// <summary>双指上一帧的角度</summary>
        private float previousTwoFingerAngle;
        /// <summary>双指上一帧的中心点</summary>
        private Vector2 previousTwoFingerCenter;
        /// <summary>双指是否有效（已初始化）</summary>
        private bool isTwoFingerValid = false;
        /// <summary>双指长按是否已触发</summary>
        private bool isTwoFingerLongPressInvoked = false;
        /// <summary>双指点击起始位置1</summary>
        private Vector2 twoFingerStartPos1;
        /// <summary>双指点击起始位置2</summary>
        private Vector2 twoFingerStartPos2;

        // ==================== 生命周期方法 ====================

        /// <summary>
        /// 初始化方法，由框架自动调用
        /// </summary>
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Unity每帧更新，处理输入事件
        /// </summary>
        private void Update()
        {
            // 检查管理器状态，未启动或已销毁则不处理输入
            if (!IsStartUp || IsDispose) return;

            // 处理双指触摸
            HandleTwoFingerInput();

            // 处理单指触摸（仅在无双指时处理）
            if (Input.touchCount <= 1)
            {
                HandleSingleTouchInput();
            }
        }

        // ==================== 单指输入处理方法 ====================

        /// <summary>
        /// 处理单指输入事件
        /// </summary>
        private void HandleSingleTouchInput()
        {
            // 鼠标/触摸开始
            if (Input.GetMouseButtonDown(0))
            {
                StartTouch(Input.mousePosition);
            }

            // 鼠标/触摸持续（按住状态）
            if (isTouching)
            {
                Vector2 currentPos = Input.mousePosition;

                // 拖拽检测：如果移动距离超过阈值，则开始拖拽
                if (!isDragging && Vector2.Distance(touchStartPos, currentPos) > DRAG_START_DISTANCE)
                {
                    StartDrag();
                }

                // 拖拽过程中，每帧触发拖拽事件
                if (isDragging)
                {
                    UpdateDrag(currentPos);
                }

                // 长按检测：如果按住时间超过阈值且未触发过长按事件
                if (!isLongPressInvoked && Time.time - touchStartTime > LONG_PRESS_TIME)
                {
                    TriggerLongPress();
                }

                // 记录当前坐标，用于下一帧计算增量
                lastTouchPos = currentPos;
            }

            // 鼠标/触摸结束
            if (Input.GetMouseButtonUp(0) && isTouching)
            {
                EndTouch(Input.mousePosition);
            }
        }

        /// <summary>
        /// 开始触摸处理
        /// </summary>
        /// <param name="position">触摸开始的屏幕坐标</param>
        private void StartTouch(Vector2 position)
        {
            OnScreenClick?.Invoke(position);

            // 检查是否点击在UI上，如果是则忽略此次触摸
            if (!IgnoreUICheck && IsPointerOverUI()) return;

            // 记录触摸信息
            touchStartPos = position;
            lastTouchPos = position;
            touchStartTime = Time.time;
            isTouching = true;
            isDragging = false;
            isLongPressInvoked = false;
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        private void StartDrag()
        {
            isDragging = true;
            // 触发拖拽开始事件，参数为起始位置和当前位置
            OnDragStart?.Invoke(touchStartPos, lastTouchPos);
        }

        /// <summary>
        /// 更新拖拽状态
        /// </summary>
        /// <param name="currentPos">当前触摸坐标</param>
        private void UpdateDrag(Vector2 currentPos)
        {
            // 触发拖拽过程中事件，参数为上一帧位置和当前位置
            OnDrag?.Invoke(lastTouchPos, currentPos);
        }

        /// <summary>
        /// 触发长按事件
        /// </summary>
        private void TriggerLongPress()
        {
            isLongPressInvoked = true;
            // 触发长按事件，参数为长按位置
            OnLongPress?.Invoke(touchStartPos);
        }

        /// <summary>
        /// 结束触摸处理
        /// </summary>
        /// <param name="endPos">触摸结束的屏幕坐标</param>
        private void EndTouch(Vector2 endPos)
        {
            // 计算触摸持续时间和移动距离
            float duration = Time.time - touchStartTime;
            Vector2 delta = endPos - touchStartPos;
            float distance = delta.magnitude;

            // 如果处于拖拽状态，触发拖拽结束事件
            if (isDragging)
            {
                OnDragEnd?.Invoke(lastTouchPos, endPos);
            }

            // 滑动检测：满足距离和时间条件
            if (distance >= MIN_SWIPE_DISTANCE && duration <= MAX_SWIPE_TIME)
            {
                // 计算滑动方向
                SwipeDirection direction = GetSwipeDirection(delta);

                // 触发完整滑动事件（包含方向、起始点、结束点）
                OnSwipe?.Invoke(direction, touchStartPos, endPos);
                // 触发简化滑动事件（仅方向）
                OnSwipeSimple?.Invoke(direction);

                // 滑动时不触发点击事件，直接重置状态返回
                ResetTouch();
                return;
            }

            // 点击检测：移动距离很小且未触发过长按事件
            if (distance < CLICK_DISTANCE_THRESHOLD && !isLongPressInvoked)
            {
                // 触发屏幕点击事件
                OnClick?.Invoke(endPos);

                // 如果有对象点击事件的监听者，尝试获取点击的游戏对象
                if (OnGameObjectClick != null)
                {
                    GameObject clickedObj = GetClickedGameObject(endPos);
                    if (clickedObj != null)
                    {
                        OnGameObjectClick?.Invoke(clickedObj);
                    }
                }
            }

            // 重置触摸状态
            ResetTouch();
        }

        // ==================== 双指输入处理方法 ====================

        /// <summary>
        /// 处理双指输入事件
        /// </summary>
        private void HandleTwoFingerInput()
        {
            // 检查是否为双指触摸
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                // 检查是否在UI上（根据IgnoreUICheck决定）
                if (!IgnoreUICheck && (IsPointerOverUI(touch1.position) || IsPointerOverUI(touch2.position)))
                {
                    ResetTwoFingerState();
                    return;
                }

                // 处理双指状态机
                switch (twoFingerState)
                {
                    case TwoFingerState.None:
                        OnTwoFingerTouchStart?.Invoke();
                        twoFingerState = TwoFingerState.Touching;
                        twoFingerStartTime = Time.time;
                        twoFingerStartPos1 = touch1.position;
                        twoFingerStartPos2 = touch2.position;
                        isTwoFingerLongPressInvoked = false;
                        break;

                    case TwoFingerState.Touching:
                        // 检测双指移动
                        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                        {
                            twoFingerState = TwoFingerState.Moving;
                            InitializeTwoFingerGesture(touch1, touch2);
                        }
                        // 检测双指长按
                        else if (!isTwoFingerLongPressInvoked && 
                                 Time.time - twoFingerStartTime > TWO_FINGER_LONG_PRESS_TIME)
                        {
                            Vector2 center = (touch1.position + touch2.position) * 0.5f;
                            OnTwoFingerLongPress?.Invoke(center);
                            isTwoFingerLongPressInvoked = true;
                            twoFingerState = TwoFingerState.LongPress;
                        }
                        break;

                    case TwoFingerState.Moving:
                        ProcessTwoFingerGestures(touch1, touch2);
                        break;

                    case TwoFingerState.LongPress:
                        // 长按后如果有移动，仍然可以处理手势
                        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                        {
                            if (!isTwoFingerValid)
                            {
                                InitializeTwoFingerGesture(touch1, touch2);
                            }
                            ProcessTwoFingerGestures(touch1, touch2);
                        }
                        break;
                }
            }
            else
            {
                // 双指触摸结束
                if (twoFingerState != TwoFingerState.None)
                {
                    OnTwoFingerTouchEnd?.Invoke();

                    // 检测双指点击（触摸时间短且移动距离小）
                    if (twoFingerState == TwoFingerState.Touching && 
                        Time.time - twoFingerStartTime <= TWO_FINGER_TAP_TIME)
                    {
                        Vector2 center = (twoFingerStartPos1 + twoFingerStartPos2) * 0.5f;
                        OnTwoFingerTap?.Invoke(center);
                    }

                    ResetTwoFingerState();
                }
            }
        }

        /// <summary>
        /// 初始化双指手势数据
        /// </summary>
        private void InitializeTwoFingerGesture(Touch touch1, Touch touch2)
        {
            previousTwoFingerDistance = Vector2.Distance(touch1.position, touch2.position);
            previousTwoFingerAngle = GetAngle(touch1.position, touch2.position);
            previousTwoFingerCenter = (touch1.position + touch2.position) * 0.5f;
            isTwoFingerValid = true;
        }

        /// <summary>
        /// 处理双指手势（缩放、旋转、滑动）
        /// </summary>
        private void ProcessTwoFingerGestures(Touch touch1, Touch touch2)
        {
            if (!isTwoFingerValid)
            {
                InitializeTwoFingerGesture(touch1, touch2);
                return;
            }

            // 当前帧数据
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);
            float currentAngle = GetAngle(touch1.position, touch2.position);
            Vector2 currentCenter = (touch1.position + touch2.position) * 0.5f;

            // 1. 缩放检测
            float deltaDistance = currentDistance - previousTwoFingerDistance;
            if (Mathf.Abs(deltaDistance) > 0.5f) // 忽略微小变化
            {
                OnPinchZoom?.Invoke(deltaDistance * PINCH_ZOOM_SENSITIVITY);
            }

            // 2. 旋转检测
            float deltaAngle = Mathf.DeltaAngle(previousTwoFingerAngle, currentAngle);
            if (Mathf.Abs(deltaAngle) > 0.5f) // 忽略微小角度变化
            {
                OnTwoFingerRotate?.Invoke(deltaAngle * ROTATE_SENSITIVITY);
            }

            // 3. 双指滑动检测
            Vector2 deltaCenter = currentCenter - previousTwoFingerCenter;
            if (deltaCenter.magnitude > 0.5f)
            {
                OnTwoFingerDrag?.Invoke(deltaCenter * TWO_FINGER_DRAG_SENSITIVITY);
            }

            // 更新上一帧数据
            previousTwoFingerDistance = currentDistance;
            previousTwoFingerAngle = currentAngle;
            previousTwoFingerCenter = currentCenter;
        }

        /// <summary>
        /// 重置双指状态
        /// </summary>
        private void ResetTwoFingerState()
        {
            twoFingerState = TwoFingerState.None;
            isTwoFingerValid = false;
            isTwoFingerLongPressInvoked = false;
        }

        // ==================== 工具方法 ====================

        /// <summary>
        /// 根据移动向量计算滑动方向（8方向）
        /// </summary>
        /// <param name="delta">移动向量（结束位置 - 起始位置）</param>
        /// <returns>滑动方向枚举</returns>
        private SwipeDirection GetSwipeDirection(Vector2 delta)
        {
            // 计算移动向量与X轴正方向的夹角（-180° 到 180°）
            float angle = Vector2.SignedAngle(Vector2.right, delta);

            // 根据角度范围判断方向（8方向）
            if (angle >= -22.5f && angle < 22.5f)
                return SwipeDirection.Right;
            else if (angle >= 22.5f && angle < 67.5f)
                return SwipeDirection.UpRight;
            else if (angle >= 67.5f && angle < 112.5f)
                return SwipeDirection.Up;
            else if (angle >= 112.5f && angle < 157.5f)
                return SwipeDirection.UpLeft;
            else if (angle >= 157.5f || angle < -157.5f)
                return SwipeDirection.Left;
            else if (angle >= -157.5f && angle < -112.5f)
                return SwipeDirection.DownLeft;
            else if (angle >= -112.5f && angle < -67.5f)
                return SwipeDirection.Down;
            else // -67.5f 到 -22.5f
                return SwipeDirection.DownRight;
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
        /// 通过屏幕坐标获取被点击的游戏对象
        /// </summary>
        /// <param name="screenPos">屏幕坐标</param>
        /// <returns>被点击的GameObject，如无则为null</returns>
        private GameObject GetClickedGameObject(Vector2 screenPos)
        {
            // 3D射线检测
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                return hit.collider.gameObject;
            }

            // 2D射线检测（如果3D检测失败）
            RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(screenPos), Vector2.zero);
            if (hit2D.collider != null)
            {
                return hit2D.collider.gameObject;
            }

            return null;
        }

        /// <summary>
        /// 检查指定位置的触摸/点击是否在UI元素上
        /// </summary>
        /// <param name="position">屏幕坐标</param>
        /// <returns>true表示在UI上，false表示不在UI上</returns>
        private bool IsPointerOverUI(Vector2 position)
        {
            if (EventSystem.current == null) return false;
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = position;
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            return results.Count > 0;
        }

        /// <summary>
        /// 检查当前触摸/点击是否在UI元素上
        /// </summary>
        /// <returns>true表示在UI上，应忽略此次输入；false表示不在UI上</returns>
        private bool IsPointerOverUI()
        {
            return IsPointerOverUI(Input.mousePosition);
        }

        /// <summary>
        /// 重置触摸相关状态变量
        /// </summary>
        private void ResetTouch()
        {
            isTouching = false;
            isDragging = false;
            isLongPressInvoked = false;
        }

        // ==================== 公共静态方法 ====================

        /// <summary>
        /// 清除所有已注册的事件委托（通常在场景切换时调用）
        /// </summary>
        public static void ClearAllEvents()
        {
            OnClick = null;
            OnGameObjectClick = null;
            OnSwipe = null;
            OnSwipeSimple = null;
            OnLongPress = null;
            OnDragStart = null;
            OnDrag = null;
            OnDragEnd = null;
            
            // 清除双指事件
            OnPinchZoom = null;
            OnTwoFingerRotate = null;
            OnTwoFingerDrag = null;
            OnTwoFingerTap = null;
            OnTwoFingerLongPress = null;
            OnTwoFingerTouchStart = null;
            OnTwoFingerTouchEnd = null;
        }

        /// <summary>
        /// 检查当前触摸是否有效（不在UI上）
        /// </summary>
        /// <returns>true表示触摸有效，false表示触摸被UI阻挡</returns>
        public static bool IsTouchValid()
        {
            // 如果单例未初始化，默认返回true
            if (Instance == null) return true;
            return IgnoreUICheck || !Instance.IsPointerOverUI();
        }

        // ==================== 双指公共静态方法 ====================

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
        /// 获取当前双指中心点
        /// </summary>
        public static Vector2 GetTwoFingerCenter()
        {
            if (Input.touchCount == 2)
            {
                return (Input.GetTouch(0).position + Input.GetTouch(1).position) * 0.5f;
            }
            return Vector2.zero;
        }

        /// <summary>
        /// 当前是否为双指触摸状态
        /// </summary>
        public static bool IsTwoFingerTouching
        {
            get { return Instance != null && Instance.twoFingerState != TwoFingerState.None; }
        }

        /// <summary>
        /// 当前双指是否在移动中
        /// </summary>
        public static bool IsTwoFingerMoving
        {
            get { return Instance != null && Instance.twoFingerState == TwoFingerState.Moving; }
        }

        /// <summary>
        /// 设置双指手势灵敏度
        /// </summary>
        /// <param name="zoom">缩放灵敏度 (默认0.01f)</param>
        /// <param name="rotate">旋转灵敏度 (默认0.5f)</param>
        /// <param name="drag">滑动灵敏度 (默认0.01f)</param>
        public static void SetTwoFingerSensitivity(float zoom = 0.01f, float rotate = 0.5f, float drag = 0.01f)
        {
            if (Instance != null)
            {
                // 这里可以添加设置灵敏度的逻辑
                // 由于是常量，如果需要运行时调整，可以改为变量
            }
        }
    }

    // ==================== 外部使用的枚举 ====================

    /// <summary>
    /// 滑动方向枚举（8方向）
    /// </summary>
    public enum SwipeDirection
    {
        None,       // 无方向
        Up,         // 上
        Down,       // 下
        Left,       // 左
        Right,      // 右
        UpLeft,     // 左上
        UpRight,    // 右上
        DownLeft,   // 左下
        DownRight   // 右下
    }

    /// <summary>
    /// 双指状态枚举
    /// </summary>
    public enum TwoFingerState
    {
        None,       // 无双指
        Touching,   // 双指触摸（未移动）
        Moving,     // 双指移动中
        LongPress   // 双指长按
    }
}