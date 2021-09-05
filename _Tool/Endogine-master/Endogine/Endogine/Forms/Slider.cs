using System;
using System.Drawing;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for Slider.
	/// </summary>
	public class Slider : Sprite
	{
		public delegate void SliderEventDelegate(float fPosition, MouseEventType t);

		protected SliderHandle m_sliderHandle;
		protected Frame m_frame;
		public event SliderEventDelegate SliderEvent;

		public Slider()
		{
			this.m_bNoScalingOnSetRect = true;
			this.Init();
		}

		protected virtual void Init()
		{
			m_frame = new Frame();
			m_frame.Parent = this;
			m_frame.MemberName = "Button2Up";
			m_frame.Ink = RasterOps.ROPs.BgTransparent;
			m_frame.Member.ColorKey = Color.FromArgb(0,0,0);
			m_frame.MouseActive = true;
			m_frame.LocZ = 0;

			m_sliderHandle = new SliderHandle();
			m_sliderHandle.Parent = this;
			m_sliderHandle.MemberName = "Button2Up";
			m_sliderHandle.Ink = RasterOps.ROPs.BgTransparent;
			m_sliderHandle.Member.ColorKey = Color.FromArgb(0,0,0);
			m_sliderHandle.RegPoint = new EPoint(15,15);
			m_sliderHandle.Position = 0.5f;
			m_sliderHandle.LocZ = 100;

			this.InitEnd();
		}

		protected void InitEnd()
		{
			m_frame.MouseEvent+=new MouseEventDelegate(m_frame_MouseEvent);
			m_sliderHandle.MouseEvent+=new MouseEventDelegate(m_sliderHandle_MouseEvent);
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
			}
		}

		public float SliderPosition
		{
			get {return this.m_sliderHandle.Position;}
			set {this.m_sliderHandle.Position = value;}
		}

		private void m_sliderHandle_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, MouseEventType t)
		{
			if (t == Endogine.Sprite.MouseEventType.StillDown || t == Endogine.Sprite.MouseEventType.Click)
			{
				//m_sliderHandle.Color = System.Drawing.Color.FromArgb((int)(m_sliderHandle.Position*255),255,255);
				if (SliderEvent!=null)
					SliderEvent(m_sliderHandle.Position, t);
			}
		}

		private void m_frame_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, MouseEventType t)
		{
			if (t == Endogine.Sprite.MouseEventType.StillDown || t == Endogine.Sprite.MouseEventType.Click)
			{
				Sprite spTo = (Sprite)m_sliderHandle;
				Sprite spFrom = (Sprite)m_frame;

				//spTo.invalidate(spTo.m_mvRenderOutput) --TODO:
				spFrom.CheckMouse(
					new System.Windows.Forms.MouseEventArgs(
						System.Windows.Forms.MouseButtons.None, 0, -10000,-10000, 0),
					new EPointF(-10000,-10000), true, false);
				//spTo.calcRenderOutput();

				EPointF pntLocInParent = spTo.ConvRootLocToParentLoc(m_endogine.MouseLoc.ToEPointF());
				spTo.Loc = pntLocInParent;
				spTo.CheckMouse(e, pntLocInParent, true, true);
			}
		}
	}
}
