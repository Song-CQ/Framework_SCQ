using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Endogine;

namespace Driver
{
    public class Game
    {
        Car _car;
        CarGauges _gauges;
        public Game()
        {
            //Bitmap bmp = new Bitmap(100, 100, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //Graphics gfx = Graphics.FromImage(bmp);
            //gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //gfx.DrawEllipse(new Pen(Color.Red), new Rectangle(0, 0, 99, 99));
            //gfx.Dispose();
            
            //MemberSpriteBitmap mb = new Endogine.MemberSpriteBitmap(bmp);
            //Sprite sp = new Sprite();
            //sp.Member = mb;

            this._car = new Car();
            this._car.Loc = new EPointF(100, 30);

            this._gauges = new CarGauges(this._car);
        }
    }
}
