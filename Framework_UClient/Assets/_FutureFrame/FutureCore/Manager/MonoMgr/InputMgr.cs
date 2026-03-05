using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FutureCore
{
    /// <summary>
    /// ��������� - ͳһ����������������������ק�������¼�
    /// ����ģʽ���̳���BaseMonoMgr��ȷ��ȫ��Ψһ
    /// </summary>
    public sealed class InputMgr : BaseMonoMgr<InputMgr>
    {
        /// <summary>
        /// �Ƿ����UI��⣨��ʹ�����UI��Ҳ�����¼���
        /// Ĭ��false�������UI�ϲ������¼�
        /// true����ʹ�����UI��Ҳ�����¼�
        /// </summary>
        public static bool IgnoreUICheck { get; set; } = false;

        // ==================== �¼��������� ====================

        /// <summary>ȫ�ֵ���¼����κε�����ᴥ�������������UI�ϣ�</summary>
        public static event Action<Vector2> OnScreenClick;

        // �������¼�
        /// <summary>��Ļ����¼�������Ϊ�������Ļ����(Vector2)</summary>
        public static event Action<Vector2> OnClick;
        /// <summary>��Ϸ�������¼�������Ϊ�������GameObject</summary>
        public static event Action<GameObject> OnGameObjectClick;

        // ��������¼�
        /// <summary>���������¼�������������������ʼ���ꡢ��������</summary>
        public static event Action<SwipeDirection, Vector2, Vector2> OnSwipe;
        /// <summary>�򻯻����¼�������������������</summary>
        public static event Action<SwipeDirection> OnSwipeSimple;

        // ��������¼�
        /// <summary>�����¼�������������λ�õ���Ļ����</summary>
        public static event Action<Vector2> OnLongPress;

        // ��ק����¼�
        /// <summary>��ק��ʼ�¼�����������ʼ���ꡢ��ǰ����</summary>
        public static event Action<Vector2, Vector2> OnDragStart;
        /// <summary>��ק�������¼�����������һ֡���ꡢ��ǰ����</summary>
        public static event Action<Vector2, Vector2> OnDrag;
        /// <summary>��ק�����¼�����������һ֡���ꡢ��������</summary>
        public static event Action<Vector2, Vector2> OnDragEnd;

        // ==================== ˫ָ�¼����� ====================

        /// <summary>˫ָ�����¼�����������������(����)</summary>
        public static event Action<float> OnPinchZoom;
        /// <summary>˫ָ��ת�¼�����������ת�Ƕ�����</summary>
        public static event Action<float> OnTwoFingerRotate;
        /// <summary>˫ָ�����¼�����������������</summary>
        public static event Action<Vector2> OnTwoFingerDrag;
        /// <summary>˫ָ����¼���������˫ָ���ĵ�</summary>
        public static event Action<Vector2> OnTwoFingerTap;
        /// <summary>˫ָ�����¼���������˫ָ���ĵ�</summary>
        public static event Action<Vector2> OnTwoFingerLongPress;
        /// <summary>˫ָ��ʼ�����¼�</summary>
        public static event Action OnTwoFingerTouchStart;
        /// <summary>˫ָ���������¼�</summary>
        public static event Action OnTwoFingerTouchEnd;

        // ==================== ���ó������� ====================

        /// <summary>��С��������(����)�����ڴ˾�����Ϊ���</summary>
        private const float MIN_SWIPE_DISTANCE = 50f;
        /// <summary>��󻬶�ʱ��(��)��������ʱ����Ϊ��Ч����</summary>
        private const float MAX_SWIPE_TIME = 0.7f;
        /// <summary>�����ж�ʱ��(��)����ס������ʱ�䴥�������¼�</summary>
        private const float LONG_PRESS_TIME = 1.0f;
        /// <summary>��ק�ж�����(����)���ƶ������˾�����Ϊ��ק��ʼ</summary>
        private const float DRAG_START_DISTANCE = 5f;
        /// <summary>����ж�����(����)���ƶ�����С�ڴ�ֵ��Ϊ���</summary>
        private const float CLICK_DISTANCE_THRESHOLD = 10f;

        // ==================== ˫ָ���ó��� ====================

        /// <summary>˫ָ����ж�ʱ��(��)</summary>
        private const float TWO_FINGER_TAP_TIME = 0.3f;
        /// <summary>˫ָ�����ж�ʱ��(��)</summary>
        private const float TWO_FINGER_LONG_PRESS_TIME = 1.0f;
        /// <summary>˫ָ���������ϵ��</summary>
        private const float PINCH_ZOOM_SENSITIVITY = 0.01f;
        /// <summary>˫ָ��ת�����ϵ��</summary>
        private const float ROTATE_SENSITIVITY = 0.5f;
        /// <summary>˫ָ���������ϵ��</summary>
        private const float TWO_FINGER_DRAG_SENSITIVITY = 0.01f;

        // ==================== ״̬�������� ====================

        /// <summary>������ʼʱ����Ļ����</summary>
        private Vector2 touchStartPos;
        /// <summary>��һ�μ�¼�Ĵ������꣨������ק�������㣩</summary>
        private Vector2 lastTouchPos;
        /// <summary>������ʼ��ʱ���</summary>
        private float touchStartTime;
        /// <summary>�Ƿ����ڴ�����</summary>
        private bool isTouching = false;
        /// <summary>�Ƿ�������ק��</summary>
        private bool isDragging = false;
        /// <summary>�����¼��Ƿ��Ѵ�������ֹ�ظ�������</summary>
        private bool isLongPressInvoked = false;

        // ==================== ˫ָ״̬���� ====================

        /// <summary>˫ָ״̬</summary>
        private TwoFingerState twoFingerState = TwoFingerState.None;
        /// <summary>˫ָ��ʼ����ʱ��</summary>
        private float twoFingerStartTime;
        /// <summary>˫ָ��һ֡�ľ���</summary>
        private float previousTwoFingerDistance;
        /// <summary>˫ָ��һ֡�ĽǶ�</summary>
        private float previousTwoFingerAngle;
        /// <summary>˫ָ��һ֡�����ĵ�</summary>
        private Vector2 previousTwoFingerCenter;
        /// <summary>˫ָ�Ƿ���Ч���ѳ�ʼ����</summary>
        private bool isTwoFingerValid = false;
        /// <summary>˫ָ�����Ƿ��Ѵ���</summary>
        private bool isTwoFingerLongPressInvoked = false;
        /// <summary>˫ָ�����ʼλ��1</summary>
        private Vector2 twoFingerStartPos1;
        /// <summary>˫ָ�����ʼλ��2</summary>
        private Vector2 twoFingerStartPos2;

        // ==================== �������ڷ��� ====================

        /// <summary>
        /// ��ʼ���������ɿ���Զ�����
        /// </summary>
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Unityÿ֡���£����������¼�
        /// </summary>
        private void Update()
        {
            // ��������״̬��δ������������򲻴�������
            if (!IsStartUp || IsDispose) return;

            // ����˫ָ����
            HandleTwoFingerInput();

            // �����ָ������������˫ָʱ�����
            if (Input.touchCount <= 1)
            {
                HandleSingleTouchInput();
            }
        }

        // ==================== ��ָ���봦����� ====================

        /// <summary>
        /// �����ָ�����¼�
        /// </summary>
        private void HandleSingleTouchInput()
        {
            // ���/������ʼ
            if (Input.GetMouseButtonDown(0))
            {
                StartTouch(Input.mousePosition);
            }

            // ���/������������ס״̬��
            if (isTouching)
            {
                Vector2 currentPos = Input.mousePosition;

                // ��ק��⣺����ƶ����볬����ֵ����ʼ��ק
                if (!isDragging && Vector2.Distance(touchStartPos, currentPos) > DRAG_START_DISTANCE)
                {
                    StartDrag();
                }

                // ��ק�����У�ÿ֡������ק�¼�
                if (isDragging)
                {
                    UpdateDrag(currentPos);
                }

                // ������⣺�����סʱ�䳬����ֵ��δ�����������¼�
                if (!isLongPressInvoked && Time.time - touchStartTime > LONG_PRESS_TIME)
                {
                    TriggerLongPress();
                }

                // ��¼��ǰ���꣬������һ֡��������
                lastTouchPos = currentPos;
            }

            // ���/��������
            if (Input.GetMouseButtonUp(0) && isTouching)
            {
                EndTouch(Input.mousePosition);
            }
        }

        /// <summary>
        /// ��ʼ��������
        /// </summary>
        /// <param name="position">������ʼ����Ļ����</param>
        private void StartTouch(Vector2 position)
        {
            OnScreenClick?.Invoke(position);

            // ����Ƿ�����UI�ϣ����������Դ˴δ���
            if (!IgnoreUICheck && IsPointerOverUI()) return;

            // ��¼������Ϣ
            touchStartPos = position;
            lastTouchPos = position;
            touchStartTime = Time.time;
            isTouching = true;
            isDragging = false;
            isLongPressInvoked = false;
        }

        /// <summary>
        /// ��ʼ��ק
        /// </summary>
        private void StartDrag()
        {
            isDragging = true;
            // ������ק��ʼ�¼�������Ϊ��ʼλ�ú͵�ǰλ��
            OnDragStart?.Invoke(touchStartPos, lastTouchPos);
        }

        /// <summary>
        /// ������ק״̬
        /// </summary>
        /// <param name="currentPos">��ǰ��������</param>
        private void UpdateDrag(Vector2 currentPos)
        {
            // ������ק�������¼�������Ϊ��һ֡λ�ú͵�ǰλ��
            OnDrag?.Invoke(lastTouchPos, currentPos);
        }

        /// <summary>
        /// ���������¼�
        /// </summary>
        private void TriggerLongPress()
        {
            isLongPressInvoked = true;
            // ���������¼�������Ϊ����λ��
            OnLongPress?.Invoke(touchStartPos);
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="endPos">������������Ļ����</param>
        private void EndTouch(Vector2 endPos)
        {
            // ���㴥������ʱ����ƶ�����
            float duration = Time.time - touchStartTime;
            Vector2 delta = endPos - touchStartPos;
            float distance = delta.magnitude;

            // ���������ק״̬��������ק�����¼�
            if (isDragging)
            {
                OnDragEnd?.Invoke(lastTouchPos, endPos);
            }

            // ������⣺��������ʱ������
            if (distance >= MIN_SWIPE_DISTANCE && duration <= MAX_SWIPE_TIME)
            {
                // ���㻬������
                SwipeDirection direction = GetSwipeDirection(delta);

                // �������������¼�������������ʼ�㡢�����㣩
                OnSwipe?.Invoke(direction, touchStartPos, endPos);
                // �����򻯻����¼���������
                OnSwipeSimple?.Invoke(direction);

                // ����ʱ����������¼���ֱ������״̬����
                ResetTouch();
                return;
            }

            // �����⣺�ƶ������С��δ�����������¼�
            if (distance < CLICK_DISTANCE_THRESHOLD && !isLongPressInvoked)
            {
                // ������Ļ����¼�
                OnClick?.Invoke(endPos);

                // ����ж������¼��ļ����ߣ����Ի�ȡ�������Ϸ����
                if (OnGameObjectClick != null)
                {
                    GameObject clickedObj = GetClickedGameObject(endPos);
                    if (clickedObj != null)
                    {
                        OnGameObjectClick?.Invoke(clickedObj);
                    }
                }
            }

            // ���ô���״̬
            ResetTouch();
        }

        // ==================== ˫ָ���봦����� ====================

        /// <summary>
        /// ����˫ָ�����¼�
        /// </summary>
        private void HandleTwoFingerInput()
        {
            // ����Ƿ�Ϊ˫ָ����
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                // ����Ƿ���UI�ϣ�����IgnoreUICheck������
                if (!IgnoreUICheck && (IsPointerOverUI(touch1.position) || IsPointerOverUI(touch2.position)))
                {
                    ResetTwoFingerState();
                    return;
                }

                // ����˫ָ״̬��
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
                        // ���˫ָ�ƶ�
                        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                        {
                            twoFingerState = TwoFingerState.Moving;
                            InitializeTwoFingerGesture(touch1, touch2);
                        }
                        // ���˫ָ����
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
                        // ������������ƶ�����Ȼ���Դ�������
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
                // ˫ָ��������
                if (twoFingerState != TwoFingerState.None)
                {
                    OnTwoFingerTouchEnd?.Invoke();

                    // ���˫ָ���������ʱ������ƶ�����С��
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
        /// ��ʼ��˫ָ��������
        /// </summary>
        private void InitializeTwoFingerGesture(Touch touch1, Touch touch2)
        {
            previousTwoFingerDistance = Vector2.Distance(touch1.position, touch2.position);
            previousTwoFingerAngle = GetAngle(touch1.position, touch2.position);
            previousTwoFingerCenter = (touch1.position + touch2.position) * 0.5f;
            isTwoFingerValid = true;
        }

        /// <summary>
        /// ����˫ָ���ƣ����š���ת��������
        /// </summary>
        private void ProcessTwoFingerGestures(Touch touch1, Touch touch2)
        {
            if (!isTwoFingerValid)
            {
                InitializeTwoFingerGesture(touch1, touch2);
                return;
            }

            // ��ǰ֡����
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);
            float currentAngle = GetAngle(touch1.position, touch2.position);
            Vector2 currentCenter = (touch1.position + touch2.position) * 0.5f;

            // 1. ���ż��
            float deltaDistance = currentDistance - previousTwoFingerDistance;
            if (Mathf.Abs(deltaDistance) > 0.5f) // ����΢С�仯
            {
                OnPinchZoom?.Invoke(deltaDistance * PINCH_ZOOM_SENSITIVITY);
            }

            // 2. ��ת���
            float deltaAngle = Mathf.DeltaAngle(previousTwoFingerAngle, currentAngle);
            if (Mathf.Abs(deltaAngle) > 0.5f) // ����΢С�Ƕȱ仯
            {
                OnTwoFingerRotate?.Invoke(deltaAngle * ROTATE_SENSITIVITY);
            }

            // 3. ˫ָ�������
            Vector2 deltaCenter = currentCenter - previousTwoFingerCenter;
            if (deltaCenter.magnitude > 0.5f)
            {
                OnTwoFingerDrag?.Invoke(deltaCenter * TWO_FINGER_DRAG_SENSITIVITY);
            }

            // ������һ֡����
            previousTwoFingerDistance = currentDistance;
            previousTwoFingerAngle = currentAngle;
            previousTwoFingerCenter = currentCenter;
        }

        /// <summary>
        /// ����˫ָ״̬
        /// </summary>
        private void ResetTwoFingerState()
        {
            twoFingerState = TwoFingerState.None;
            isTwoFingerValid = false;
            isTwoFingerLongPressInvoked = false;
        }

        // ==================== ���߷��� ====================

        /// <summary>
        /// �����ƶ��������㻬������8����
        /// </summary>
        /// <param name="delta">�ƶ�����������λ�� - ��ʼλ�ã�</param>
        /// <returns>��������ö��</returns>
        private SwipeDirection GetSwipeDirection(Vector2 delta)
        {
            // �����ƶ�������X��������ļнǣ�-180�� �� 180�㣩
            float angle = Vector2.SignedAngle(Vector2.right, delta);

            // ���ݽǶȷ�Χ�жϷ���8����
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
            else // -67.5f �� -22.5f
                return SwipeDirection.DownRight;
        }

        /// <summary>
        /// ���������ĽǶȣ��������ĻX�ᣩ
        /// </summary>
        private float GetAngle(Vector2 p1, Vector2 p2)
        {
            Vector2 dir = p1 - p2;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return angle;
        }

        /// <summary>
        /// ͨ����Ļ�����ȡ���������Ϸ����
        /// </summary>
        /// <param name="screenPos">��Ļ����</param>
        /// <returns>�������GameObject��������Ϊnull</returns>
        private GameObject GetClickedGameObject(Vector2 screenPos)
        {
            // 3D���߼��
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                return hit.collider.gameObject;
            }

            // 2D���߼�⣨���3D���ʧ�ܣ�
            RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(screenPos), Vector2.zero);
            if (hit2D.collider != null)
            {
                return hit2D.collider.gameObject;
            }

            return null;
        }

        /// <summary>
        /// ���ָ��λ�õĴ���/����Ƿ���UIԪ����
        /// </summary>
        /// <param name="position">��Ļ����</param>
        /// <returns>true��ʾ��UI�ϣ�false��ʾ����UI��</returns>
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
        /// ��鵱ǰ����/����Ƿ���UIԪ����
        /// </summary>
        /// <returns>true��ʾ��UI�ϣ�Ӧ���Դ˴����룻false��ʾ����UI��</returns>
        private bool IsPointerOverUI()
        {
            return IsPointerOverUI(Input.mousePosition);
        }

        /// <summary>
        /// ���ô������״̬����
        /// </summary>
        private void ResetTouch()
        {
            isTouching = false;
            isDragging = false;
            isLongPressInvoked = false;
        }

        // ==================== ������̬���� ====================

        /// <summary>
        /// ���������ע����¼�ί�У�ͨ���ڳ����л�ʱ���ã�
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
            
            // ���˫ָ�¼�
            OnPinchZoom = null;
            OnTwoFingerRotate = null;
            OnTwoFingerDrag = null;
            OnTwoFingerTap = null;
            OnTwoFingerLongPress = null;
            OnTwoFingerTouchStart = null;
            OnTwoFingerTouchEnd = null;
        }

        /// <summary>
        /// ��鵱ǰ�����Ƿ���Ч������UI�ϣ�
        /// </summary>
        /// <returns>true��ʾ������Ч��false��ʾ������UI�赲</returns>
        public static bool IsTouchValid()
        {
            // �������δ��ʼ����Ĭ�Ϸ���true
            if (Instance == null) return true;
            return IgnoreUICheck || !Instance.IsPointerOverUI();
        }

        // ==================== ˫ָ������̬���� ====================

        /// <summary>
        /// ��ȡ��ǰ˫ָ����
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
        /// ��ȡ��ǰ˫ָ���ĵ�
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
        /// ��ǰ�Ƿ�Ϊ˫ָ����״̬
        /// </summary>
        public static bool IsTwoFingerTouching
        {
            get { return Instance != null && Instance.twoFingerState != TwoFingerState.None; }
        }

        /// <summary>
        /// ��ǰ˫ָ�Ƿ����ƶ���
        /// </summary>
        public static bool IsTwoFingerMoving
        {
            get { return Instance != null && Instance.twoFingerState == TwoFingerState.Moving; }
        }

        /// <summary>
        /// ����˫ָ���������
        /// </summary>
        /// <param name="zoom">��������� (Ĭ��0.01f)</param>
        /// <param name="rotate">��ת����� (Ĭ��0.5f)</param>
        /// <param name="drag">��������� (Ĭ��0.01f)</param>
        public static void SetTwoFingerSensitivity(float zoom = 0.01f, float rotate = 0.5f, float drag = 0.01f)
        {
            if (Instance != null)
            {
                // ������������������ȵ��߼�
                // �����ǳ����������Ҫ����ʱ���������Ը�Ϊ����
            }
        }
    }

    // ==================== �ⲿʹ�õ�ö�� ====================

    /// <summary>
    /// ��������ö�٣�8����
    /// </summary>
    public enum SwipeDirection
    {
        None,       // �޷���
        Up,         // ��
        Down,       // ��
        Left,       // ��
        Right,      // ��
        UpLeft,     // ����
        UpRight,    // ����
        DownLeft,   // ����
        DownRight   // ����
    }

    /// <summary>
    /// ˫ָ״̬ö��
    /// </summary>
    public enum TwoFingerState
    {
        None,       // ��˫ָ
        Touching,   // ˫ָ������δ�ƶ���
        Moving,     // ˫ָ�ƶ���
        LongPress   // ˫ָ����
    }
}