using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Endogine.BitmapHelpers
{
    public unsafe class Canvas8 : Canvas
    {
        public Canvas8()
        {
        }

        public override void SetPixel(int x, int y, Color clr)
        {
            byte* p = (this._data + this._stride * y) + x;
            *p = clr.R;
        }
        public override Color GetPixel(int x, int y)
        {
            byte* p = (this._data + this._stride * y) + x;
            return Color.FromArgb(*p,*p,*p);
        }

        public override void SetPixelInt(int x, int y, int clr)
        {
            byte* p = (this._data + this._stride * y) + x;
            *p = (byte)clr;
        }
        public override int GetPixelInt(int x, int y)
        {
            byte* p = (this._data + this._stride * y) + x;
            return (int)*p;
        }

        public override void SetPixelByte(int x, int y, byte clr)
        {
            byte* p = (this._data + this._stride * y) + x;
            *p = clr;
        }
        public override byte GetPixelByte(int x, int y)
        {
            byte* p = (this._data + this._stride * y) + x;
            return *p;
        }


        //octtree palette optimization:
        //http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnaspp/html/colorquant.asp

        public override void Fill(Color clr)
        {
            byte nClr = clr.R;
            byte* ptr = null;
            for (int y = this.Height - 1; y >= 0; y--)
            {
                ptr = (this._data + this._stride * y);
                for (int x = 0; x < this.Width; x++)
                {
                    *ptr = nClr;
                    ptr++;
                }
            }
        }
    }
}
