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

            HandleInput();
        }

        // ==================== 输入处理核心方法 ====================

        /// <summary>
        /// 统一处理所有输入事件
        /// </summary>
        private void HandleInput()
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

            /* 简化版（4方向）代码：
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            else
                return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            */
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
        /// 检查当前触摸/点击是否在UI元素上
        /// </summary>
        /// <returns>true表示在UI上，应忽略此次输入；false表示不在UI上</returns>
        private bool IsPointerOverUI()
        {
            // EventSystem可能为空，需要检查
            if (EventSystem.current == null) return false;
            return EventSystem.current.IsPointerOverGameObject();
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
}