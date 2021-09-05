using System;
using System.Drawing;
using Endogine;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Emulates the colored film that was on top of the original Space Invaders game screen
	/// </summary>
	public class Colorizer : Sprite
	{
		public Colorizer()
		{
			Bitmap bmp = new Bitmap(1,1);
			Graphics g = Graphics.FromImage(bmp);
			g.FillRectangle(new SolidBrush(Color.FromArgb(255,255,255)), 0,0,1,1);
			Endogine.MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
			this.Member = mb;
			this.Ink = RasterOps.ROPs.SubtractPin;
		}
	}
}
