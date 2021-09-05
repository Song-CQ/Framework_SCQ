using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Endogine.BitmapHelpers
{
    public unsafe class Canvas24 : Canvas
    {
        public Canvas24()
        {
        }

        public override void SetPixel(int x, int y, Color clr)
        {
            byte* p = (this._data + this._stride * y) + x * 3;
            *p = clr.B;
            *(p + 1) = clr.G;
            *(p + 2) = clr.R;
        }
        public override Color GetPixel(int x, int y)
        {
            byte* p = (this._data + this._stride * y) + x * 3;
            return Color.FromArgb(*(p + 2), *(p + 1), *p);
        }

        public override void SetPixelInt(int x, int y, int clr)
        {
            byte* p = (this._data + this._stride * y) + x * 3;
            *p = (byte)(clr & 0xff);
            *(p + 1) = (byte)((clr & 0xff00) >> 8);
            *(p + 2) = (byte)((clr & 0xff0000) >> 16);
        }
        public override int GetPixelInt(int x, int y)
        {
            int* p = (int*)((this._data + this._stride * y) + x * 3);
            return *p & 0xffffff;
        }

        public override byte GetPixelByte(int x, int y)
        {
            byte* p = (this._data + this._stride * y) + x * 3;
            return *p;
        }
        public override void SetPixelByte(int x, int y, byte clr)
        {
            byte* p = (this._data + this._stride * y) + x * 3;
            *p = clr;
        }


        public override void Fill(Color clr)
        {
            int nTmp = clr.ToArgb(); // << 8;
            byte r = (byte)((nTmp & 0xff0000) >> 16);
            byte g = (byte)((nTmp & 0xff00) >> 8);
            byte b = (byte)(nTmp & 0xff);
            //int nClr = ((nTmp & 0xff) << 16) | (nTmp & 0xff00) | ((nTmp) >> 16);

            byte* ptr = null;
            int* iptr = null;
            for (int y = this.Height - 1; y >= 0; y--)
            {
                ptr = (this._data + this._stride * y);
                for (int x = 0; x < this.Width; x++)
                {
                    *ptr = r;
                    *(ptr + 1) = g;
                    *(ptr + 2) = b;
                    //iptr = (int*)ptr;
                    //*iptr = nClr;
                    ptr+=3;
                }
            }
        }

    }
}
