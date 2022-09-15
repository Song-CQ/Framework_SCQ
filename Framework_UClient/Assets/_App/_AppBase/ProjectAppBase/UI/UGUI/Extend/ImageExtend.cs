/****************************************************
    文件：ImageExtend.cs
	作者：清
    邮箱: 2728285639@qq.com
	功能：Image扩展
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace ProjectApp.UGUI
{
    [RequireComponent(typeof(Image))]
    public class ImageExtend : BaseMeshEffect
    {
        /// <summary>
        /// 是否水平翻转
        /// </summary>
        public bool FlipHorizontal
        {
            get { return flipHor; }
            set
            {
                flipHor = value;
            }
        }
        /// <summary>
        /// 是否垂直翻转
        /// </summary>
        public bool FlipVertical
        {
            get { return flipVer; }
            set
            {
                flipVer = value;
            }
        }

        [SerializeField]
        private bool flipHor;
        [SerializeField]
        private bool flipVer;


        private RectTransform _rectTrans;
        public RectTransform RectTrans
        {
            get
            {
                if (null == _rectTrans)
                {
                    _rectTrans = GetComponent<RectTransform>();
                }
                return _rectTrans;
            }
        }

        private Image _Image;
        public Image Image
        {
            get
            {
                if (null == Image)
                {
                    _Image = GetComponent<Image>();
                }
                return _Image;
            }
        }


        public override void ModifyMesh(VertexHelper toFill)
        {
            if (flipHor || flipVer)
            {
                Vector2 rectCenter = RectTrans.rect.center;

                int vertCount = toFill.currentVertCount;
                for (int i = 0; i < vertCount; i++)
                {
                    UIVertex uiVertex = new UIVertex();
                    toFill.PopulateUIVertex(ref uiVertex, i);

                    Vector3 pos = uiVertex.position;
                    uiVertex.position = new Vector3(
                        flipHor ? (pos.x + (rectCenter.x - pos.x) * 2) : pos.x,
                        flipVer ? (pos.y + (rectCenter.y - pos.y) * 2) : pos.y,
                        pos.z);

                    toFill.SetUIVertex(uiVertex, i);
                }
            }
        }
    }
}