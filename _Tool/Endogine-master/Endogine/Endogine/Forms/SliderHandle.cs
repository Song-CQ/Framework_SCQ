using System;
using System.Drawing;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for SliderHandle.
	/// </summary>
	public class SliderHandle : Sprite
	{
		public SliderHandle()
		{
			Name = "SliderHandle";
			this.MouseActive = true;
		}

		protected override void OnMouse(System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
		{
			if (t==Endogine.Sprite.MouseEventType.StillDown)
			{
				EPointF pntDiff = new EPointF(e.X-MouseLastLoc.X, e.Y-MouseLastLoc.Y);
				Move(pntDiff);
				//if (!GetConstrainRect().Contains(new Point((int)Loc.X,(int)Loc.Y)))
				//{
				//	Move(new PointF(-pntDiff.X,-pntDiff.Y));
				//}
			}
			base.OnMouse (e, t);
		}

		protected ERectangle GetConstrainRect()
		{
			return new ERectangle(
				new EPoint(Parent.SourceRect.Left, Parent.SourceRect.Top+15),
				new EPoint(Parent.SourceRect.Width,0)); //this.SourceRect.Width
		}

		public override EPointF Loc
		{
			get
			{
				return base.Loc;
			}
			set
			{
				ERectangle rctI = GetConstrainRect();
				ERectangleF rct = new ERectangleF(rctI.Location.ToEPointF(), rctI.Size.ToEPointF());
				if (!rct.Contains(value))
				{
					if (value.X < rct.Left)
						value.X = rct.Left;
					else if (value.X > rct.Right)
						value.X = rct.Right;
		
					if (value.Y < rct.Top)
						value.Y = rct.Top;
					else if (value.Y > rct.Bottom)
						value.Y = rct.Bottom;
				}
				base.Loc = value;
			}
		}

		public float Position
		{
			get 
			{
				ERectangle rct = GetConstrainRect();
				return Math.Min(1, Math.Max(0, (LocX- rct.Left)/rct.Width));
			}
			set
			{
				float val = Math.Min(1,Math.Max(0,value));
				ERectangle rct = GetConstrainRect();
				Loc = new EPointF(val*rct.Width + rct.Left, 0);
			}
		}

		protected void SetPositionFromLoc(PointF a_pntLoc)
		{
			ERectangle rct = GetConstrainRect();
			Position = (a_pntLoc.X-rct.Left)/rct.Width;
		}
	}
}
