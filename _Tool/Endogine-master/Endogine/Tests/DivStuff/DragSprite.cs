using System;
using System.Drawing;
using System.Windows.Forms;
using Endogine;

namespace Tests
{
	/// <summary>
	/// Summary description for DragSprite.
	/// </summary>
	public class DragSprite : Sprite
	{
		public DragSprite()
		{
			Name = "DragSprite";
			this.MouseActive = true;
		}

		public override void EnterFrame()
		{
            //if (AutoAnimator!=null)
            //    AutoAnimator.StepSize = 0.1f;

			base.EnterFrame();
		}

		protected override void OnMouse(MouseEventArgs e, MouseEventType t)
		{
			if (t == MouseEventType.Down)
				this.Color = Color.FromArgb(this.Color.R, this.Color.G, 0);
			else if (t == MouseEventType.Up || t == MouseEventType.UpOutside || t == MouseEventType.Click)
				this.Color = Color.FromArgb(this.Color.R, this.Color.G, 255);
			else if (t == MouseEventType.Enter)
				this.Color = Color.FromArgb(0, this.Color.G, this.Color.B);
			else if (t == MouseEventType.Leave)
				this.Color = Color.FromArgb(255, this.Color.G, this.Color.B);
			else if (t == MouseEventType.StillDown)
			{
				Scaling = new EPointF(1.0f+0.005f*e.X, 1);
				Loc = new EPointF(e.X, e.Y);
				Blend = (int)(100*e.X*1.0/100);
			}
		}
	}
}
