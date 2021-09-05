using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Endogine.BitmapHelpers
{
    public unsafe class Canvas32 : Canvas
    {
        public Canvas32()
        {
        }

        public override void SetPixel(int x, int y, Color clr)
        {
            int* p = (int*)(this._data + this._stride * y) + x; //this.GetLineStartPointer(y) + x;
            *p = clr.ToArgb();
        }
        public override Color GetPixel(int x, int y)
        {
            int* p = (int*)(this._data + this._stride * y) + x;
            return Color.FromArgb(*p);
        }

        public override void SetPixelInt(int x, int y, int clr)
        {
            int* p = (int*)(this._data + this._stride * y) + x;
            *p = clr;
        }
        public override int GetPixelInt(int x, int y)
        {
            int* p = (int*)(this._data + this._stride * y) + x;
            return *p;
        }

        public override void SetPixelByte(int x, int y, byte clr)
        {
            byte* p = (byte*)((int*)(this._data + this._stride * y) + x);
            *p = clr;
        }
        public override byte GetPixelByte(int x, int y)
        {
            byte* p = (byte*)((int*)(this._data + this._stride * y) + x);
            return *p;
        }

        public override void Fill(Color clr)
        {
            int nClr = clr.ToArgb();
            int* ptr = null;
            for (int y = this.Height - 1; y >= 0; y--)
            {
                ptr = (int*)(this._data + this._stride * y);
                for (int x = 0; x < this.Width; x++)
                {
                    *ptr = nClr;
                    ptr++;
                }
            }
        }

    }
}
