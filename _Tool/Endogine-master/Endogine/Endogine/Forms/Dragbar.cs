using System;
using System.Drawing;
using Endogine;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for Form.
	/// </summary>
	public class Dragbar : Sprite
	{
		public Frame m_frame;
		public Button m_btnClose;

		public Dragbar()
		{
			this.m_bNoScalingOnSetRect = true;
			Name = "Dragbar";

			m_frame = new Frame();
			m_frame.Parent = this;
			m_frame.MouseActive = true;
			m_frame.MouseEvent+=new MouseEventDelegate(m_frame_MouseEvent);
			MemberSpriteBitmap mb = (MemberSpriteBitmap)m_endogine.CastLib.GetOrCreate("Button2Up");
			m_frame.Member = mb;
			m_frame.Ink = RasterOps.ROPs.BgTransparent;
			m_frame.Member.ColorKey = Color.FromArgb(0,0,0);
			m_frame.LocZ = 1;

			m_btnClose = new Button();
			m_btnClose.Parent = this;
			m_btnClose.MouseActive = true;
			m_btnClose.MouseEvent+=new MouseEventDelegate(m_btnClose_MouseEvent);
			m_btnClose.Ink = RasterOps.ROPs.D3DTest2;
			m_btnClose.LocZ = 2;

			LabelGDI lbl = new LabelGDI();
			lbl.Parent = this;
			lbl.Name = "Title";
			lbl.Text = "Dialog";
			lbl.LocZ = 3;
		}

		public override ERectangleF Rect
		{
			get
			{
				return base.Rect;
			}
			set
			{
				base.Rect = value;
				m_frame.Rect = new ERectangleF(0,0,Rect.Width,Rect.Height);
				m_btnClose.Rect = new ERectangleF(Rect.Width-40,5,30,30);

				Sprite lbl = this.GetChildByName("Title");
				lbl.Loc = new EPointF(10,value.Height/2-lbl.Rect.Height/2);
			}
		}

		private void m_frame_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, MouseEventType t)
		{
			if (t == Sprite.MouseEventType.StillDown)
			{
				EPointF pntDiff = new EPointF(e.X-m_frame.MouseLastLoc.X, e.Y-m_frame.MouseLastLoc.Y);
				this.Parent.Move(pntDiff);
			}
		}

		private void m_btnClose_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, MouseEventType t)
		{
			if (t == Sprite.MouseEventType.Click)
			{
				((Form)Parent).Close();
			}
		}
	}
}
