using System;

namespace Endogine.Serialization.Photoshop.ImageResources
{
	/// <summary>
	/// Summary description for DisplayInfo.
	/// </summary>
	public class DisplayInfo : ImageResource
	{
		public ColorModes ColorSpace;
		public short[] Color = new short[4];
		public short Opacity;			// 0..100
		public bool kind;				// selected = false, protected = true

		public DisplayInfo(ImageResource imgRes)
		{
			BinaryReverseReader reader = imgRes.GetDataReader();

			this.ColorSpace = (ColorModes)reader.ReadInt16();
			for(int i=0; i<4; i++)
				this.Color[i] = reader.ReadInt16();

			this.Opacity = (short)Math.Max(0,Math.Min(100,(int)reader.ReadInt16()));
			this.kind = reader.ReadByte()==0?false:true;

			reader.Close();
		}
	}
}
