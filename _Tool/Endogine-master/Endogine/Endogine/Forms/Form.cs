using System;
using System.Drawing;
using Endogine;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for Form.
	/// </summary>
	public class Form : Sprite
	{
		protected Frame m_frame;
		protected Dragbar m_dragbar;
		protected Sprite m_resizeCorner;
		public Form()
		{
			this.m_bNoScalingOnSetRect = true;
			Name = "Form";

			m_frame = new Frame();
			m_frame.Parent = this;
			m_frame.Ink = RasterOps.ROPs.D3DTest2;
			//MemberSpriteBitmap mb = (MemberSpriteBitmap)m_endogine.CastLib.GetOrCreate("Button2Up");
			//m_frame.Member = mb;
			m_frame.MemberName = "Button2Up";
			m_frame.LocZ = 0;
			m_frame.MouseActive = true; //as to not let mouse clicks fall through to sprites behind

			m_dragbar = new Dragbar();
			m_dragbar.LocZ = 1;
			m_dragbar.Parent = this;

			m_resizeCorner = new Sprite();
			m_resizeCorner.Parent = this;
			m_resizeCorner.LocZ = 1;
			m_resizeCorner.Name = "ResizeCorner";
			m_resizeCorner.MemberName = "Button2Up";
			//m_resizeCorner.Member = mb;
			m_resizeCorner.Ink = RasterOps.ROPs.BgTransparent;
			m_resizeCorner.SourceRect = new ERectangle(0,0,15,15);
			m_resizeCorner.MouseActive = true;
			m_resizeCorner.MouseEvent+=new MouseEventDelegate(m_resizeCorner_MouseEvent);
		}

		public bool Sizeable
		{
			set
			{
				this.m_resizeCorner.Visible = value;
				this.m_resizeCorner.MouseActive = value;
			}
		}
		public bool Dragbar
		{
			set
			{
				this.m_dragbar.Visible = value;
			}
		}

		public override ERectangleF Rect
		{
			get{	return base.Rect;}
			set
			{
				base.Rect = value;
				m_frame.Rect = new ERectangleF(0,0,Rect.Width,Rect.Height);
				m_dragbar.Rect = new ERectangleF(0,0,Rect.Width,40);
				m_resizeCorner.Loc = new EPointF(Rect.Width-20,Rect.Height-20);
			}
		}

		public override Color Color
		{
			get{return base.Color;}
			set
			{
				for (int i = this.ChildCount-1; i >= 0; i--)
					this.GetChildByIndex(i).Color = value;
				base.Color = value;
			}
		}


		public void Close()
		{
			Dispose();
		}

		public void SetAllFrameMembers(string memberName)
		{
			this.m_frame.MemberName = memberName;
			this.m_resizeCorner.MemberName = memberName;
			//this.m_dragbar.m_btnClose.M
			this.m_dragbar.m_frame.MemberName = memberName;
		}

		private void m_resizeCorner_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, MouseEventType t)
		{
			if (t==Sprite.MouseEventType.StillDown)
			{
				EPoint pntDiff = new EPoint(e.X-m_resizeCorner.MouseLastLoc.X, e.Y-m_resizeCorner.MouseLastLoc.Y);
				Rect = new ERectangleF(Rect.Location, Rect.Size+pntDiff.ToEPointF());//SizeF(Rect.Size.Width+pntDiff.X,Rect.Size.Height+pntDiff.Y));
			}
		}
	}
}
