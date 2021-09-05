using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Endogine.Serialization.Palettes
{
    [Category("aco")]
    public class Aco : Base
    {
        public Aco()
        { }

        public override Endogine.ColorEx.Palette Load(string filename)
        {
            Serialization.BinaryReverseReader reader = new BinaryReverseReader(new System.IO.FileStream(filename, System.IO.FileMode.Open));
            //Serialization.BinaryReaderEx reader = new BinaryReaderEx(new System.IO.FileStream(filename, System.IO.FileMode.Open));
            ushort version = reader.ReadUInt16();
            ushort numColors = reader.ReadUInt16();

            if (version > 2)
            {
                throw new Exception("Can't parse .ACO palettes with version > 2: "+filename);
            }

            Endogine.ColorEx.Palette palette = new Endogine.ColorEx.Palette();
            for (int colorNum = 0; colorNum < numColors; colorNum++)
			{
                ushort colorSpaceId = reader.ReadUInt16();

                ushort[] vals = new ushort[4];
                for (int i = 0; i < 4; i++)
                    vals[i] = reader.ReadUInt16();

                string name = colorNum.ToString();
                if (version == 2)
                {
                    reader.ReadUInt16(); //padded
                    int nameLength = reader.ReadUInt16();

                    if (reader.BaseStream.Position % 2 == 1) // ?
                        reader.ReadUInt16(); //padded

                    for (int letterNum = 0; letterNum < nameLength; letterNum++)
        			{
                        char c = (char)reader.ReadUInt16();
                        name+=c.ToString();
		        	}
                }

                ColorEx.ColorBase color;
                //TODO; uses 16 bits per channel, but here I truncate it to 8 bits
                switch (colorSpaceId)
                {
                    case 0: //RGB
                        ColorEx.ColorRgb rgb = new Endogine.ColorEx.ColorRgb();
                        rgb.R = vals[0] / 256;
                        rgb.G = vals[1] / 256;
                        rgb.B = vals[2] / 256;
                        color = rgb;
                        break;
                    case 1: //HSB
                        ColorEx.ColorHsb hsb = new Endogine.ColorEx.ColorHsb();
                        hsb.H = (float)vals[0] / 182.04f;
                        hsb.S = (float)vals[1] / 655.35f / 100f;
                        hsb.B = (float)vals[2] / 655.35f / 100f;
                        color = hsb;
                        break;
                    case 2: //CMYK
                        ColorEx.ColorCmyk cmyk = new Endogine.ColorEx.ColorCmyk();
                        cmyk.C = (float)vals[0] / 256;
                        cmyk.M = (float)vals[1] / 256;
                        cmyk.Y = (float)vals[2] / 256;
                        cmyk.K = (float)vals[3] / 256;
                        color = cmyk;
                        break;
                    case 7: //Lab
                        float[] ab = new float[2];
                        for (int i = 1; i < 3; i++)
                        {
                            if (vals[i] <= 12000) ab[i - 1] = (float)vals[i] / 100;
                            else ab[i - 1] = (float)(65536 - vals[i]) / 100;
                        }
                        ColorEx.ColorLab lab = new Endogine.ColorEx.ColorLab();
                        lab.L = (float)vals[0]/100;
                        lab.a = vals[1];
                        lab.b = vals[2];
                        //Angle:2, Observer:D65
                        color = lab;
                        break;
                    case 8: //Grayscale
                        ColorEx.ColorRgb rgbGray = new Endogine.ColorEx.ColorRgb();
                        int gray = (int)((float)vals[0] / 39.0625f);
                        rgbGray.R = gray;
                        rgbGray.G = gray;
                        rgbGray.B = gray;
                        color = rgbGray;
                        break;
                    case 9: //Wide CMYK
                    default:
                        throw new Exception("Can't read colorspace "+colorSpaceId+" in "+filename);
                }
                palette.Add(name, color);
			}
            return palette;
        }

        public override void Save(string filename, Endogine.ColorEx.Palette palette)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
