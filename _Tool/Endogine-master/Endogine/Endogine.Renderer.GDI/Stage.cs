using System;
using System.Windows.Forms;
using System.Drawing;

namespace Endogine.Renderer.GDI
{
	/// <summary>
	/// Summary description for Stage.
	/// </summary>
	public class Stage : StageBase
	{
        public Stage(Control RenderControl)
            : base(RenderControl) //EndogineHub a_endogine
		{
		}

		public override void Init()
		{
            base.Init();
            //this.CreateRootSprite(new ERectangle(0, 0, this.ControlSize.X, this.ControlSize.Y));
			Bitmap bmp = new Bitmap(this.ControlSize.X, this.ControlSize.Y, this._renderControl.CreateGraphics());
			_spRoot.Member = new MemberSpriteBitmap(bmp);
		}

        public override Endogine.BitmapHelpers.PixelDataProvider CreateRenderTarget(int width, int height, int numChannels)
        {
            return new Endogine.BitmapHelpers.PixelDataProviderGDI(width, height, numChannels);
        }

        public override Endogine.BitmapHelpers.PixelDataProvider RenderTarget
        {
            set { }
            get { return this._spRoot.Member.RenderStrategy.PixelDataProvider; }
        }

		public override void Update()
		{
			Graphics g;


			g = Graphics.FromImage(_spRoot.Member.Bitmap);
            if (this._clearTarget)
                g.Clear(Color);
			_spRoot.EnterFrame();
			_spRoot.Draw();


            bool bRenderAsText = false;
            bool bRenderASCII = false;
			if (bRenderASCII) //for fun: render graphics as ASCII art
			{
				string sAll = "";
				if (bRenderAsText)
				{
                    string s = " ¨.,-:\"+itoHwM"; // " .,:|IOMW";
					int nDivider = 255/(s.Length);
					for (int y = 0; y < _spRoot.SourceRect.Height; y+=16)
					{
						for (int x = 0; x < _spRoot.SourceRect.Width; x+=8)
						{
							Color clr = _spRoot.Member.Bitmap.GetPixel(x,y);
							int nGray = (clr.R+clr.G+clr.B)/3;
							int n = nGray/nDivider;
							if (n >= s.Length) n = s.Length-1;
							sAll+=s[n];
						}
						sAll+="\n";
					}
					g.Clear(Color);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
					Font font = new Font("Courier New", 10);
					g.DrawString(sAll, font, new SolidBrush(Color.White), 10,10);
				}
			}

			g = this._renderControl.CreateGraphics();
			g.DrawImage(_spRoot.Member.Bitmap, new PointF(0,0));


			g.Dispose();

			ERectangle rct = _spRoot.Rect.ToERectangle();
			Point pntScreenTopLeft = this._renderControl.PointToScreen(new System.Drawing.Point(0,0));
			rct.X+=pntScreenTopLeft.X;
			rct.Y+=pntScreenTopLeft.Y;

			this._renderControl.Invalidate();
		}

		public override SpriteRenderStrategy CreateRenderStrategy()
		{
			SpriteRenderStrategy rs = new SpriteRenderStrategyA();
			return rs;
		}

		public override Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy CreateMemberStrategy()
		{
			Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy rs = new MemberSpriteBitmapRenderStrategyA();
			return rs;
		}

        public override Endogine.BitmapHelpers.PixelDataProvider CreatePixelDataProvider(System.Drawing.Bitmap bmp)
        {
            Endogine.BitmapHelpers.PixelDataProviderGDI pdp = new Endogine.BitmapHelpers.PixelDataProviderGDI(bmp);
            return pdp;
        }
        public override Endogine.BitmapHelpers.PixelDataProvider CreatePixelDataProvider(int width, int height, int numChannels)
        {
            return new Endogine.BitmapHelpers.PixelDataProviderGDI(width, height, numChannels);
        }
	}
}
