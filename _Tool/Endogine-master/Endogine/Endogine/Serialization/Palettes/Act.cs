using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Endogine.Serialization.Palettes
{
    [Category("act")]
    public class Act : Base
    {
        public Act()
        { }
        public override Endogine.ColorEx.Palette Load(string filename)
        {
            Serialization.BinaryReaderEx reader = new BinaryReaderEx(new System.IO.FileStream(filename, System.IO.FileMode.Open));
            int numColors = (int)(reader.BaseStream.Length / 3);
            Endogine.ColorEx.Palette palette = new Endogine.ColorEx.Palette();

            for (int i = 0; i < numColors; i++)
			{
                Vector4 v = new Vector4(0, 0, 0, 1);
                for (int channelNum = 0; channelNum < 3; channelNum++)
                    v[channelNum] = (float)reader.ReadByte() / 255;

                string name = i.ToString();
                ColorEx.ColorRgb rgb = new Endogine.ColorEx.ColorRgb();
                rgb.Vector = v;
                palette.Add(name, rgb);
			}
            return palette;
        }

        public override void Save(string filename, Endogine.ColorEx.Palette palette)
        {
            System.IO.BinaryWriter writer = new System.IO.BinaryWriter(new System.IO.FileStream(filename, System.IO.FileMode.OpenOrCreate));
            foreach (KeyValuePair<string, ColorEx.ColorBase> kv in palette)
            {
                //Vector4 v = color.VectorRGBA;
                System.Drawing.Color c = kv.Value.ColorRGBA;
                writer.Write((byte)(int)c.R);
                writer.Write((byte)(int)c.G);
                writer.Write((byte)(int)c.B);
            }
        }
    }
}
