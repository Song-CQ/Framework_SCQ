using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Serialization.Photoshop
{
    public class Mask
    {
        //TODO: write a base class for both Mask and Channel?
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

        public ERectangle Rectangle;

        public Mask(BinaryReverseReader reader, Layer layer)
        {
            this._layer = layer;

            int nLength = (int)reader.ReadUInt32();
            if (nLength == 0)
                return;

            long nStart = reader.BaseStream.Position;

            this.Rectangle = new ERectangle();
            this.Rectangle.Y = reader.ReadInt32();
            this.Rectangle.X = reader.ReadInt32();
            this.Rectangle.Height = reader.ReadInt32() - Rectangle.Y;
            this.Rectangle.Width = reader.ReadInt32() - Rectangle.X;

            byte color = reader.ReadByte();
            
            byte flags = reader.ReadByte();

            if (nLength == 36)
            {
                int someOtherFlags = reader.ReadByte();

                byte maskBg = reader.ReadByte();

                ERectangle rect = new ERectangle();
                rect.Y = reader.ReadInt32();
                rect.X = reader.ReadInt32();
                rect.Height = reader.ReadInt32() - rect.Y;
                rect.Width = reader.ReadInt32() - rect.X;
            }

            reader.BaseStream.Position = nStart + nLength;
        }

        public void ReadPixels(BinaryReverseReader reader)
        {
            //long posBefore = reader.BaseStream.Position;
            this._data = PixelsProcessing.ReadPixels(reader, this.Layer.Width, this.Layer.Height, this.Layer.BitsPerPixel, false);
            //reader.BaseStream.Position = posBefore + this._length;
        }
    }
}
