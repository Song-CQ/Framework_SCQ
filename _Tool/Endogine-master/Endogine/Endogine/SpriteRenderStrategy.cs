using System;
using System.Drawing;
using Endogine.ResourceManagement;

namespace Endogine
{
	/// <summary>
	/// Summary description for SpriteRenderStrategy.
	/// </summary>
	public abstract class SpriteRenderStrategy
	{
		protected EndogineHub m_endogine;
		protected Sprite _sp;
        protected Shader _shader;
        protected ERectangleF _sourceClipRect;

		public void SetEndogine(EndogineHub a_endogine)
		{
			m_endogine = a_endogine;
		}
		public void SetSprite(Sprite a_sp)
		{
			_sp = a_sp;
		}
		abstract public void Dispose();

		abstract public void Init();
		

		abstract public void SetColor(Color a_clr);
		abstract public void EnterFrame();
		abstract public void SetMember(MemberBase a_mb);
        /// <summary>
        /// Use instead of SetMember when members aren't applicable (e.g. when using rendering without Endogine game loop and sprite system)
        /// </summary>
        /// <param name="pdp"></param>
        abstract public void SetPixelDataProvider(Endogine.BitmapHelpers.PixelDataProvider pdp);
        
		abstract public void SetMemberAnimationFrame(int n);
//		abstract public void SetSourceRect(ERectangle rct);
		abstract public void RecalcedParentOutput();
        abstract public void CalcRenderRegion(ERectangleF rctDrawTarget, float rotation, EPoint regPoint, EPoint sourceRectSize);
		abstract public void SubDraw();

        public virtual Shader Shader
        {
            get { return this._shader; }
            set { this._shader = value; }
        }
        public ERectangleF SourceClipRect
        {
            get { return this._sourceClipRect; }
            set { this.SetSourceClipRect(value); this._sourceClipRect = value; }
        }
        abstract protected void SetSourceClipRect(ERectangleF rct);
	}
}
