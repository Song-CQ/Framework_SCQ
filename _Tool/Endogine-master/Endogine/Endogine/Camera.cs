using System;

namespace Endogine
{
	/// <summary>
	/// Is a sprite node, but should reverse some stuff (move camera right, and the children should move left).
	/// </summary>
	public class Camera : Sprite
	{
		protected EPointF m_pntLocInternal;
		public Camera()
		{
			this.SourceRect = Parent.SourceRect.Copy();
			this.Rect = Parent.Rect.Copy();
			m_pntLocInternal = new EPointF();
		}

		public override EPointF Loc
		{
			get
			{
				return m_pntLocInternal;
			}
			set
			{
				for (int n = 0; n < this.ChildCount; n++)
				{
					Sprite sp = this.GetChildByIndex(n);
					sp.Loc = value*-1;
				}
				m_pntLocInternal = value;
			}
		}
		public override float LocX
		{
			get
			{
				return m_pntLocInternal.X;
			}
			set
			{
				Move(new EPointF(value,0));
			}
		}

		public EPointF CenterLoc
		{
			get
			{
				EPointF pntSize = new EPointF(m_endogine.Stage.RenderControl.Width, m_endogine.Stage.RenderControl.Height);
				return this.Loc + pntSize/this.Scaling*0.5f;
			}
			set
			{
				EPointF pntSize = new EPointF(m_endogine.Stage.RenderControl.Width, m_endogine.Stage.RenderControl.Height);
				this.Loc = value - pntSize/this.Scaling*0.5f;
			}
		}
	}
}
