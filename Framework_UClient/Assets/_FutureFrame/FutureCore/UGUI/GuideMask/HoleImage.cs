/****************************************************
    文件: HoleImage.cs
    作者: Clear
    日期: 2026/2/13 2:6:37
    类型: 逻辑脚本
    功能: Ugui遮罩挖洞脚本    将该脚本放在要的图片
*****************************************************/
using UnityEngine;
using UnityEngine.UI;
namespace ProjectApp
{


    using UnityEngine;
    using UnityEngine.UI;

    public class HoleImage : MonoBehaviour
    {
        public int textureSize = 256;

        private RectTransform rectTransform;
        private Image image;
        private Texture2D holeTexture;

        [HideInInspector]
        public HoleShape currentShape;
        [HideInInspector]
        public Vector2 currentSize;
        [HideInInspector]
        public Vector2 currentPosition;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }

        public void UpdateHole(Vector2 size, Vector2 position, HoleShape shape)
        {
            currentSize = size;
            currentPosition = position;
            currentShape = shape;

            // 更新 RectTransform
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = position - new Vector2(Screen.width / 2, Screen.height / 2);

            // 生成纹理
            GenerateHoleTexture();
        }

        void GenerateHoleTexture()
        {
            // 创建纹理：默认全白
            Texture2D tex = new Texture2D(textureSize, textureSize);

            for (int x = 0; x < textureSize; x++)
                for (int y = 0; y < textureSize; y++)
                    tex.SetPixel(x, y, Color.white);  // ⭐ 默认白色（不透明）

            // 挖透明洞
            int centerX = textureSize / 2;
            int centerY = textureSize / 2;

            switch (currentShape)
            {
                case HoleShape.Circle:
                    DrawCircle(tex, centerX, centerY, textureSize / 2);
                    break;
                case HoleShape.Square:
                case HoleShape.Rectangle:
                    DrawRect(tex, 0, 0, textureSize, textureSize);
                    break;
            }

            tex.Apply();

            // 应用纹理
            image.sprite = Sprite.Create(tex,
                new Rect(0, 0, textureSize, textureSize),
                new Vector2(0.5f, 0.5f));
            image.color = Color.white;
            image.type = Image.Type.Simple;

            // ⭐ 关键：透明区域不响应射线
            image.alphaHitTestMinimumThreshold = 0.1f;
        }

        void DrawCircle(Texture2D tex, int cx, int cy, int radius)
        {
            for (int x = 0; x < textureSize; x++)
                for (int y = 0; y < textureSize; y++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    if (dx * dx + dy * dy < radius * radius)
                        tex.SetPixel(x, y, Color.clear);  // ✅ 透明洞
                }
        }

        void DrawRect(Texture2D tex, int x1, int y1, int x2, int y2)
        {
            for (int x = x1; x < x2; x++)
                for (int y = y1; y < y2; y++)
                    if (x >= 0 && x < textureSize && y >= 0 && y < textureSize)
                        tex.SetPixel(x, y, Color.clear);  // ✅ 透明洞
        }

        public bool IsInHole(Vector2 screenPos)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, screenPos, null, out localPos);

            localPos += rectTransform.rect.size * 0.5f;

            return localPos.x >= 0 && localPos.x < rectTransform.rect.width &&
                   localPos.y >= 0 && localPos.y < rectTransform.rect.height;
        }
    }

    public enum HoleShape
    {
        Circle,
        Square,
        Rectangle
    }
}