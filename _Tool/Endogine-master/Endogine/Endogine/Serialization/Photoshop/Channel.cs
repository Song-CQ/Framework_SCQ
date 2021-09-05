using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Serialization.Photoshop
{
    public class Channel
    {
        int _usage;
        public int Usage
        {
            get { return _usage; }
        }

        long _length;
        //public long Length
        //{
        //    get { return _length; }
        //}

        Layer _layer;
        public Layer Layer
        {
            get { return _layer; }
        }

        byte[] _data;
        public byte[] Data
        {
            get { return _data; }
        }

        public Channel(BinaryReverseReader reader, Layer layer)
        {
            this._usage = reader.ReadInt16();

            this._length = (long)reader.ReadUInt32();
            this._layer = layer;
        }

        public void ReadPixels(BinaryReverseReader reader)
        {
            long posBefore = reader.BaseStream.Position;
            this._data = PixelsProcessing.ReadPixels(reader, this.Layer.Width, this.Layer.Height, this.Layer.BitsPerPixel, false);
            reader.BaseStream.Position = posBefore + this._length;
        }

        public float GetPixel(int x, int y)
        {
            
            if (this.Layer.BitsPerPixel == 16)
            {
                return (float)(((int)this._data[x + y * this.Layer.Width * 2]) << 8 + this._data[x + y * this.Layer.Width * 2 + 1]) / 65535;
            }
            else if (true || this.Layer.BitsPerPixel == 8)
            {
                return (float)this._data[x + y * this.Layer.Width] / 255;
            }
        }
    }
}
