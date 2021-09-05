using System;
using Endogine;
using System.Drawing;

namespace Snooker
{
	/// <summary>
	/// Summary description for PlayerBall.
	/// </summary>
	public class PlayerBall : Ball
	{
		private Sprite forceMarker;
		private EPointF forceVector;

		public PlayerBall()
		{
			this.MouseActive = true;
			this.MouseEvent+=new MouseEventDelegate(PlayerBall_MouseEvent);

			forceMarker = new Sprite();
			forceMarker.LocZ = 15;
			forceMarker.Ink = RasterOps.ROPs.AddPin;
		}

		private void PlayerBall_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, MouseEventType t)
		{
			if (t == Sprite.MouseEventType.Down)
			{
				forceMarker.Visible = true;
			}
			else if (t == Sprite.MouseEventType.StillDown)
			{
				MemberSpriteBitmap mb = forceMarker.Member;
				if (mb!=null)
					mb.Dispose();

				//this.MouseDownLoc.X, this.MouseDownLoc.Y
				ERectangleF rctLine = ERectangleF.FromLTRB(this.Loc.X, this.Loc.Y, this.MouseLastLoc.X, this.MouseLastLoc.Y);
				forceVector = rctLine.Size;

				if (rctLine.Width != 0 && rctLine.Height != 0)
				{
					Bitmap bmp = new Bitmap((int)Math.Abs(rctLine.Width)+1, (int)Math.Abs(rctLine.Height)+1, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
					Graphics g = Graphics.FromImage(bmp);

					ERectangleF rctOrigo = rctLine.Copy();
					rctOrigo.MakeTopLeftAtOrigo();

					Pen pen = new Pen(Color.Red, 2);
					g.DrawLine(pen, rctOrigo.X, rctOrigo.Y, rctOrigo.X+rctOrigo.Width, rctOrigo.Y+rctOrigo.Height);
					g.Dispose();
					
					EPointF locOffset = new EPointF();
					if (rctOrigo.Width < 0)
						locOffset.X = rctOrigo.Width;
					if (rctOrigo.Height < 0)
						locOffset.Y = rctOrigo.Height;
					
					mb = new MemberSpriteBitmap(bmp);
					forceMarker.Member = mb;
					forceMarker.Loc = this.Loc + locOffset;
				}
				else
					forceMarker.Member = null;
			}
			else if (t == Sprite.MouseEventType.UpOutside || t == Sprite.MouseEventType.Click)
			{
				this.Velocity = forceVector*-0.1f;

				forceMarker.Visible = false;
				MemberSpriteBitmap mb = forceMarker.Member;
				if (mb!=null)
				{
					forceMarker.Member = null;
					mb.Dispose();
				}
			}
		}
	}
}
