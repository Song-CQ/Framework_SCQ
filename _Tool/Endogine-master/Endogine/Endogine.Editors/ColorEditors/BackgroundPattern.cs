using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Endogine.Editors.ColorEditors
{
    class BackgroundPattern
    {
        static TextureBrush _texture;
        public static void GeneratePattern()
        {
            Bitmap bmpPattern = new Bitmap(20,20, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmpPattern);
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)), 0, 0, bmpPattern.Width / 2, bmpPattern.Height / 2);
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)), bmpPattern.Width / 2, bmpPattern.Height / 2, bmpPattern.Width / 2, bmpPattern.Height / 2);

            _texture = new TextureBrush(bmpPattern);
        }

        public static void Fill(Graphics g, Rectangle rect)
        {
            if (_texture == null)
                GeneratePattern();
            System.Drawing.Imaging.ImageAttributes imgAttr = new System.Drawing.Imaging.ImageAttributes();
            imgAttr.SetWrapMode(WrapMode.TileFlipXY);
            g.FillRectangle(_texture, rect); //new Rectangle(0, 0, bmpBg.Width, bmpBg.Height));
        }
        public static void Fill(Graphics g)
        {
            Fill(g, new Rectangle((int)g.ClipBounds.X, (int)g.ClipBounds.Y, (int)g.ClipBounds.Width, (int)g.ClipBounds.Height));
        }
    }
}
