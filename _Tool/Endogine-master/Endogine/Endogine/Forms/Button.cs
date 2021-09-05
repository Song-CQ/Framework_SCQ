using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;


namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for Button.
	/// </summary>
	public class Button : Sprite
	{
		
		protected SortedList m_plStateSprites;

		public Button()
		{
			this.m_bNoScalingOnSetRect = true;
			Name = "Button";
			this.MouseActive = true;
			m_plStateSprites = new SortedList();

			Frame spFrame = new Frame();
			spFrame.Parent = this;
			spFrame.Member = new MemberSpriteBitmap("Button2Up");
			spFrame.Ink = RasterOps.ROPs.BgTransparent;
			spFrame.Member.ColorKey = Color.FromArgb(0,0,0);
			spFrame.Rect = new ERectangleF(0,0,50,50);
			m_plStateSprites.Add(Sprite.MouseEventType.Leave, (Sprite)spFrame);

			spFrame = new Frame();
			spFrame.Parent = this;
			spFrame.Member = new MemberSpriteBitmap("Button2Down");
			spFrame.Ink = RasterOps.ROPs.BgTransparent;
			spFrame.Member.ColorKey = Color.FromArgb(0,0,0);
			spFrame.Rect = new ERectangleF(0,0,50,50);
			m_plStateSprites.Add(Sprite.MouseEventType.Enter, (Sprite)spFrame);

			for (int i = 0; i < m_plStateSprites.Count; i++)
			{
				((Sprite)m_plStateSprites.GetByIndex(i)).Visible = false;
			}
			((Sprite)m_plStateSprites[MouseEventType.Leave]).Visible = true;
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
				for (int i = 0; i < m_plStateSprites.Count; i++)
				{
					Sprite sp = (Sprite)m_plStateSprites.GetByIndex(i);
					sp.Rect = new ERectangleF(0,0,Rect.Width,Rect.Height);
				}
			}
		}

		public void SetState(MouseEventType t)
		{
			object o = m_plStateSprites[t];
			if (o != null)
			{
				for (int i = 0; i < m_plStateSprites.Count; i++)
				{
					((Sprite)m_plStateSprites.GetByIndex(i)).Visible = false;
				}

				Sprite sp = (Sprite)o;
				sp.Visible = true;
			}
		}

		protected override void OnMouse(MouseEventArgs e, MouseEventType t)
		{
			this.SetState(t);
			base.OnMouse(e,t);
		}

		public override Color Color
		{
			get
			{
				return base.Color;
			}
			set
			{
				for (int i = 0; i < m_plStateSprites.Count; i++)
				{
					Sprite sp = (Sprite)m_plStateSprites.GetByIndex(i);
					sp.Color = value;
				}
				base.Color = value;
			}
		}

	}
}
