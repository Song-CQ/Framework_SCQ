/****************************************************
    文件: GuideMask.cs
    作者: Clear
    日期: 2026/2/13 2:27:52
    类型: 逻辑脚本
    功能: 新手引导遮罩
*****************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace ProjectApp
{

    /// <summary>
    /// 多形状新手引导遮罩 - 支持矩形、圆形、多边形挖洞 + 事件穿透
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/新手引导遮罩")]
    public class GuideMask : Image, IPointerClickHandler, ICanvasRaycastFilter
    {
        public enum ShapeType
        {
            Rectangle,  // 矩形
            Circle,     // 圆形
            Polygon     // 自定义多边形
        }

        [Tooltip("洞的形状")]
        public ShapeType shape = ShapeType.Rectangle;

        [Tooltip("目标UI元素的RectTransform（矩形/圆形模式）")]
        public RectTransform targetRect;

        [Tooltip("自定义多边形顶点（屏幕坐标）")]
        public Vector2[] polygonPoints;

        [Tooltip("遮罩颜色")]
        public Color maskColor = new Color(0, 0, 0, 180f / 255f);

        [Tooltip("边缘羽化宽度")]
        public float featherWidth = 5f;

        [Tooltip("圆形额外半径缩放")]
        public float circleRadiusScale = 1.0f;

        private Canvas rootCanvas;
        private Vector3[] worldCorners = new Vector3[4];

        // 缓存上一次的目标位置，用于检测变化
        private Vector3[] lastTargetCorners = new Vector3[4];
        private Rect lastTargetRect = new Rect();
        private Vector2 lastTargetPosition;
        private Vector2 lastTargetSize;

        // 是否需要强制刷新
        private bool forceRebuild = true;

        public Action<PointerEventData> ClickMask;

        protected override void Start()
        {
            base.Start();
            raycastTarget = true;
            maskable = false;
            type = Image.Type.Simple;
            rootCanvas = GetComponentInParent<Canvas>();

            // 初始化缓存
            CacheTargetData();

            SetVerticesDirty();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            forceRebuild = true;
            SetVerticesDirty();
            ClickMask = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            forceRebuild = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (rootCanvas == null)
                rootCanvas = GetComponentInParent<Canvas>();

            // 获取遮罩范围
            RectTransform rt = transform as RectTransform;
            Vector2 size = rt.rect.size;
            Vector2 pivot = rt.pivot;

            float left = -pivot.x * size.x;
            float right = (1 - pivot.x) * size.x;
            float bottom = -pivot.y * size.y;
            float top = (1 - pivot.y) * size.y;

            Color32 maskColor32 = maskColor;

            // ============ 根据形状绘制带洞的网格 ============

            if (shape == ShapeType.Rectangle)
            {
                if (targetRect == null) return;

                Vector2 holeMin, holeMax;
                GetTargetLocalRect(out holeMin, out holeMax);

                // 边界检查
                holeMin.x = Mathf.Clamp(holeMin.x, left, right);
                holeMax.x = Mathf.Clamp(holeMax.x, left, right);
                holeMin.y = Mathf.Clamp(holeMin.y, bottom, top);
                holeMax.y = Mathf.Clamp(holeMax.y, bottom, top);

                // 上区域
                AddQuad(vh,
                    new Vector2(left, holeMax.y),
                    new Vector2(right, top),
                    maskColor32);

                // 下区域
                AddQuad(vh,
                    new Vector2(left, bottom),
                    new Vector2(right, holeMin.y),
                    maskColor32);

                // 左区域
                AddQuad(vh,
                    new Vector2(left, holeMin.y),
                    new Vector2(holeMin.x, holeMax.y),
                    maskColor32);

                // 右区域
                AddQuad(vh,
                    new Vector2(holeMax.x, holeMin.y),
                    new Vector2(right, holeMax.y),
                    maskColor32);
            }
            else if (shape == ShapeType.Circle)
            {
                if (targetRect == null) return;

                Vector2 holeCenter;
                float holeRadius;
                GetTargetCircleParams(out holeCenter, out holeRadius);

                // 绘制全屏带圆形洞
                DrawFullScreenWithCircleHole(vh, left, right, bottom, top, holeCenter, holeRadius, maskColor32);
            }
            else if (shape == ShapeType.Polygon)
            {
                if (polygonPoints == null || polygonPoints.Length < 3) return;

                List<Vector2> holePolygon = GetPolygonLocalPoints();

                // 绘制全屏带多边形洞
                DrawFullScreenWithPolygonHole(vh, left, right, bottom, top, holePolygon, maskColor32);
            }

            forceRebuild = false;
        }

        void LateUpdate()
        {
            // 每一帧检查目标物体是否发生变化
            if (IsTargetDirty())
            {
                CacheTargetData();
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// 检查目标是否发生变化
        /// </summary>
        private bool IsTargetDirty()
        {
            if (shape == ShapeType.Polygon)
            {
                // 多边形模式不需要检查targetRect
                return false;
            }

            if (targetRect == null) return false;

            // 检查位置和大小是否变化
            if (targetRect.hasChanged)
            {
                targetRect.hasChanged = false;
                return true;
            }

            // 获取当前矩形的世界坐标
            targetRect.GetWorldCorners(worldCorners);

            // 与上一次缓存的坐标比较
            for (int i = 0; i < 4; i++)
            {
                if (Vector3.Distance(worldCorners[i], lastTargetCorners[i]) > 0.01f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 缓存目标数据
        /// </summary>
        private void CacheTargetData()
        {
            if (targetRect != null)
            {
                targetRect.GetWorldCorners(lastTargetCorners);
            }
        }

        /// <summary>
        /// 绘制全屏带圆形洞
        /// </summary>
        private void DrawFullScreenWithCircleHole(VertexHelper vh, float left, float right, float bottom, float top,
                                                Vector2 center, float radius, Color32 maskColor)
        {
            // 先画全屏遮罩
            AddQuad(vh, new Vector2(left, bottom), new Vector2(right, top), maskColor);

            // 再在圆形位置画透明三角形覆盖（挖洞）
            Color32 transparent = new Color32(0, 0, 0, 0);
            int segments = 60;

            int centerIndex = vh.currentVertCount;
            vh.AddVert(new Vector3(center.x, center.y, 0), transparent, Vector2.zero);

            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2;
                float x = center.x + Mathf.Cos(angle) * radius;
                float y = center.y + Mathf.Sin(angle) * radius;
                vh.AddVert(new Vector3(x, y, 0), transparent, Vector2.zero);
            }

            for (int i = 0; i < segments; i++)
            {
                vh.AddTriangle(centerIndex, centerIndex + i + 1, centerIndex + i + 2);
            }
        }

        /// <summary>
        /// 绘制全屏带多边形洞
        /// </summary>
        private void DrawFullScreenWithPolygonHole(VertexHelper vh, float left, float right, float bottom, float top,
                                                 List<Vector2> polygon, Color32 maskColor)
        {
            // 先画全屏遮罩
            AddQuad(vh, new Vector2(left, bottom), new Vector2(right, top), maskColor);

            // 再在多边形位置画透明三角形覆盖（挖洞）
            Color32 transparent = new Color32(0, 0, 0, 0);
            int count = polygon.Count;

            int startIndex = vh.currentVertCount;

            // 添加多边形顶点
            foreach (Vector2 p in polygon)
            {
                vh.AddVert(new Vector3(p.x, p.y, 0), transparent, Vector2.zero);
            }

            // 连接成三角形（假设凸多边形）
            for (int i = 1; i < count - 1; i++)
            {
                vh.AddTriangle(startIndex, startIndex + i, startIndex + i + 1);
            }
        }

        private void GetTargetLocalRect(out Vector2 min, out Vector2 max)
        {
            targetRect.GetWorldCorners(worldCorners);

            Vector2 local0 = transform.InverseTransformPoint(worldCorners[0]);
            Vector2 local2 = transform.InverseTransformPoint(worldCorners[2]);

            min = new Vector2(
                Mathf.Min(local0.x, local2.x),
                Mathf.Min(local0.y, local2.y)
            );
            max = new Vector2(
                Mathf.Max(local0.x, local2.x),
                Mathf.Max(local0.y, local2.y)
            );
        }

        private void GetTargetCircleParams(out Vector2 center, out float radius)
        {
            targetRect.GetWorldCorners(worldCorners);

            Vector2 local0 = transform.InverseTransformPoint(worldCorners[0]);
            Vector2 local2 = transform.InverseTransformPoint(worldCorners[2]);

            center = (local0 + local2) * 0.5f;
            Vector2 size = local2 - local0;
            radius = size.magnitude * 0.5f * circleRadiusScale;
        }

        private List<Vector2> GetPolygonLocalPoints()
        {
            List<Vector2> localPoints = new List<Vector2>();

            foreach (Vector2 point in polygonPoints)
            {
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    transform as RectTransform, point, null, out localPoint))
                {
                    localPoints.Add(localPoint);
                }
            }

            return localPoints;
        }

        private void AddQuad(VertexHelper vh, Vector2 a, Vector2 b, Color32 color)
        {
            int startIndex = vh.currentVertCount;

            vh.AddVert(new Vector3(a.x, a.y, 0), color, Vector2.zero);
            vh.AddVert(new Vector3(b.x, a.y, 0), color, Vector2.zero);
            vh.AddVert(new Vector3(a.x, b.y, 0), color, Vector2.zero);
            vh.AddVert(new Vector3(b.x, b.y, 0), color, Vector2.zero);

            vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vh.AddTriangle(startIndex + 2, startIndex + 1, startIndex + 3);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (shape == ShapeType.Polygon && polygonPoints != null)
            {
                if (IsPointInPolygon(eventData.position))
                    return;
            }

            if (targetRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(targetRect, eventData.position,
                    rootCanvas?.worldCamera ?? eventData.pressEventCamera))
            {
                return;
            }
            ClickMask?.Invoke(eventData);
            //Debug.Log("点击遮罩区域");
        }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            switch (shape)
            {
                case ShapeType.Rectangle:
                case ShapeType.Circle:
                    if (targetRect == null) return true;
                    return !RectTransformUtility.RectangleContainsScreenPoint(targetRect, screenPoint, eventCamera);

                case ShapeType.Polygon:
                    if (polygonPoints == null || polygonPoints.Length < 3) return true;
                    return !IsPointInPolygon(screenPoint);

                default:
                    return true;
            }
        }

        private bool IsPointInPolygon(Vector2 screenPoint)
        {
            if (polygonPoints == null || polygonPoints.Length < 3) return false;

            Vector2[] points = polygonPoints;
            int count = points.Length;
            bool inside = false;

            for (int i = 0, j = count - 1; i < count; j = i, i++)
            {
                Vector2 pi = points[i];
                Vector2 pj = points[j];

                if (((pi.y > screenPoint.y) != (pj.y > screenPoint.y)) &&
                    (screenPoint.x < (pj.x - pi.x) * (screenPoint.y - pi.y) / (pj.y - pi.y) + pi.x))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}