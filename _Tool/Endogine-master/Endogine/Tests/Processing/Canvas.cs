using System;
using System.Drawing;
using Endogine;

namespace Tests.Processing
{
	/// <summary>
	/// Summary description for Canvas.
	/// </summary>
	public class Canvas : Sprite
	{
		public event Sprite.EnterFrameEventDelegate OnUpdateCanvas;

		//Endogine.BitmapHelpers.PixelManipulatorBase _pm;
        protected Endogine.BitmapHelpers.Canvas _canvas;

		public Canvas() : base()
		{
			RandomEx.Init();
		}

		public void Create(int width, int height)
		{
			Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bmp);
			g.FillRectangle(new SolidBrush(Color.White), 0,0,bmp.Width,bmp.Height);
			MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
			this.Member = mb;
            this._canvas = this.Member.Canvas;
			//this._pm = this.Member.PixelManipulator;
		}

		public void Clear(int clr)
		{
			this.Clear(Color.FromArgb(clr));
		}

		public void Clear(Color clr)
		{
            bool wasLocked = this._canvas.Locked;
			if (!wasLocked)
                this._canvas.Locked = true;

            this._canvas.Fill(clr);

			if (!wasLocked)
                this._canvas.Locked = false;
		}

		public void FillRectangle(int x, int y, int width, int height, int clr)
		{
			for (int yy=height+y-1; yy>=y; yy--)
			{
				for (int xx=width+x-1; xx>=x; xx--)
				{
                    this._canvas.SetPixelInt(xx, yy, clr);
				}
			}
		}

		public override void EnterFrame()
		{
            this._canvas.Locked = true;

			if (this.OnUpdateCanvas!=null)
				this.OnUpdateCanvas();
			this.UpdateCanvas();

            this._canvas.Locked = false;

			base.EnterFrame ();
		}

		public virtual void UpdateCanvas()
		{
		}

		public virtual void SetPixel(int x, int y, Color clr)
		{
            this._canvas.SetPixel(x, y, clr);
		}
		public virtual Color GetPixel(int x, int y)
		{
            return this._canvas.GetPixel(x, y);
		}
		public virtual void SetPixelInt(int x, int y, int clr)
		{
            this._canvas.SetPixelInt(x, y, clr);
		}
		public virtual int GetPixelInt(int x, int y)
		{
            return this._canvas.GetPixelInt(x, y);
		}

		public bool Locked
		{
            get { return this._canvas.Locked; }
            set { this._canvas.Locked = value; }
		}

		public int Width
		{
			get {return this.SourceRect.Width;}
		}
		public int Height
		{
			get {return this.SourceRect.Height;}
		}
	}
}
