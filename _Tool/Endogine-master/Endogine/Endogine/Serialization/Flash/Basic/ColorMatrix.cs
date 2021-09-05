using System;
using System.Drawing;

namespace Endogine.Serialization.Flash.Basic
{
	/// <summary>
	/// Summary description for ColorMatrix.
	/// </summary>
	public class ColorMatrix
	{
		public short AlphaMultiply;
		public short RedMultiply;
		public short GreenMultiply;
		public short BlueMultiply;
		public short AlphaAdd;
		public short RedAdd;
		public short GreenAdd;
		public short BlueAdd;
	
		public ColorMatrix(BinaryFlashReader reader, bool hasAlpha)
		{
			bool bHasAddTerms = reader.ReadBoolean();
			bool bHasMultTerms = reader.ReadBoolean();
			int nNumBits = (int)reader.ReadBits(4);

			this.AlphaMultiply = this.RedMultiply = this.GreenMultiply = this.BlueMultiply = 256;
			this.AlphaAdd = this.RedAdd = this.GreenAdd = this.BlueAdd = 0;

			if (bHasMultTerms)
			{
				this.RedMultiply = (short) reader.ReadBits(nNumBits);
				this.GreenMultiply = (short) reader.ReadBits(nNumBits);
				this.BlueMultiply = (short) reader.ReadBits(nNumBits);
				if (hasAlpha) 
					this.AlphaMultiply = (short) reader.ReadBits(nNumBits);
			}
			if (bHasAddTerms)
			{
				this.RedAdd = (short) reader.ReadBits(nNumBits);
				this.GreenAdd = (short) reader.ReadBits(nNumBits);
				this.BlueAdd = (short) reader.ReadBits(nNumBits);
				if (hasAlpha)
					this.AlphaAdd = (short) reader.ReadBits(nNumBits);
			}
			reader.JumpToNextByteStart();
		}


		public Color Transform(Color clr)
		{
			//System.Drawing.Imaging.ColorMatrix cm = new ColorMatrix();

			int alpha = this.AlphaAdd + (int)((float)this.AlphaMultiply/255 * clr.A);
			if (alpha > 255)
				alpha = 255; 

			int red = this.RedAdd + (int)((float)this.RedMultiply/255 * clr.R);
			if (red > 255)
				red = 255; 

			int green = this.GreenAdd + (int)((float)this.GreenMultiply/255 * clr.G);
			if (green > 255)
				green = 255; 
			
			int blue = this.BlueAdd + (int)((float)this.BlueMultiply/255 * clr.B);
			if (blue > 255)
				blue = 255; 

			return Color.FromArgb(alpha,red,green,blue);
		}
	}
}
