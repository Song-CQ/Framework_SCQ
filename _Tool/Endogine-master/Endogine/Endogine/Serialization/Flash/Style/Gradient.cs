using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Endogine.Serialization.Flash.Style
{
	/// <summary>
	/// Summary description for Gradient.
	/// </summary>
	public class Gradient
	{
		public ColorBlend ColorBlend;
		//public byte Ratio;
		//public System.Drawing.Color Color;

		public Gradient()
		{}

		public static ColorBlend[] ReadGradient(BinaryFlashReader reader, bool bWithAlpha, bool morph)
		{
			ColorBlend[] clrs = new ColorBlend[2];

			int nNumRecords = reader.ReadByte();
			ColorBlend clrBlend = new ColorBlend(nNumRecords);
			ColorBlend clrBlendEnd = null;
			if (morph)
				clrBlendEnd = new ColorBlend(nNumRecords);

			for (int i = 0; i < nNumRecords; i++)
			{
				Color clr;
				byte ratio;
				ReadRecord(reader, bWithAlpha, out clr, out ratio);
				clrBlend.Colors[i] = clr;
				clrBlend.Positions[i] = (float)ratio/255;

				if (morph)
				{
					ReadRecord(reader, bWithAlpha, out clr, out ratio);
					clrBlendEnd.Colors[i] = clr;
					clrBlendEnd.Positions[i] = (float)ratio/255;
				}
			}
			return clrs;
		}

		public static ColorBlend ReadGradient(BinaryFlashReader reader, bool bWithAlpha)
		{
			ColorBlend[] clrBlends = ReadGradient(reader, bWithAlpha, false);
			return clrBlends[0];
		}

		public static void ReadRecord(BinaryFlashReader reader, bool useAlpha, out Color color, out byte ratio)
		{
			ratio = reader.ReadByte();
			color = reader.ReadColor(useAlpha);
		}
	}
}
