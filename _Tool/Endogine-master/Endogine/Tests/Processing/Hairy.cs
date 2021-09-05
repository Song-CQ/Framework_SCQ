using System;
using System.Drawing;
using Endogine;

namespace Tests.Processing
{
	/// <summary>
	/// Summary description for Hairy.
	/// </summary>
	public class Hairy : Canvas
	{
		public Hairy()
		{
			this.Create(200,200);
			this.OnUpdateCanvas+=new EnterFrameEventDelegate(Hairy_OnUpdateCanvas);
		}

		private void Hairy_OnUpdateCanvas()
		{
			Random rnd = new Random();
			Endogine.ColorEx.ColorHsb hsb = new Endogine.ColorEx.ColorHsb(rnd.Next(360), 1 ,1);
			Color clr = hsb.ColorRGBA;
			
			for (int i=0; i<100000; i++)
                this._canvas.SetPixel(rnd.Next(this._canvas.Width), rnd.Next(this._canvas.Height), clr);
		}
	}
}
