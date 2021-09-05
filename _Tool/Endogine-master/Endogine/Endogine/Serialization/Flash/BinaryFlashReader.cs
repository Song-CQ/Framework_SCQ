using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Endogine.Serialization.Flash
{
	/// <summary>
	/// Summary description for BinaryFlashReader.
	/// </summary>
	public class BinaryFlashReader : BinarySubByteReader
	{
		public int positionInByte = 0;

		public BinaryFlashReader(System.IO.Stream a_stream) : base(a_stream)
		{
		}

		public float ReadFixed16()
		{
			int nDecPart = this.ReadByte();
			int nIntPart = this.ReadChar();
			return (float)nIntPart + (float)nDecPart/256;
		}
		public float ReadFixed32()
		{
			int nDecPart = this.ReadUInt16();
			int nIntPart = this.ReadInt16();
			return (float)nIntPart + (float)nDecPart/65536;
		}

		public ERectangle ReadRect()
		{
			long nNumBitsPerValue = this.ReadBits(5, false);
			long[] a = this.ReadBitArray(4, (int)nNumBitsPerValue, true);
			//ERectangle rct = ERectangle.FromLTRB((int)a[0], (int)a[2], (int)a[1], (int)a[3]); //according to docs - but they're wrong
			ERectangle rct = new ERectangle((int)a[0], (int)a[2], (int)a[1], (int)a[3]);
			if (rct.Width < 0)
				rct.Width = -rct.Width;
			if (rct.Height < 0)
				rct.Height = -rct.Height;
			this.JumpToNextByteStart(); //opposed to the documentation...
			return rct;
		}

		public Color ReadColor(bool useAlpha)
		{
			if (useAlpha)
				return this.ReadRGBA();
			return this.ReadRGB();
		}
		public Color ReadRGB()
		{
			//			this.JumpToNextByteStart();
			return Color.FromArgb(this.ReadByte(), this.ReadByte(), this.ReadByte());
		}
		public Color ReadRGBA()
		{
			//			this.JumpToNextByteStart();
			//Alpha comes last...
			int r = this.ReadByte();
			int g = this.ReadByte();
			int b = this.ReadByte();
			return Color.FromArgb(this.ReadByte(), r,g,b);
		}

//		public ColorMatrix ReadColorMatrixWithAlpha()
//		{
//			ColorMatrix cmatrix = new ColorMatrix();
//			bool bHasAddTerms = this.ReadBoolean();
//			bool bHasMultTerms = this.ReadBoolean();
//			int nNumBits = (int)this.ReadBits(4);
//
//			if (bHasMultTerms)
//			{
//				this.ReadBitArray(4, nNumBits, false); //TODO: signed?
//				//cmatrix. Multiply clr.Red etc...
//			}
//			if (bHasAddTerms)
//			{
//				this.ReadBitArray(4, nNumBits, false);
//				//cmatrix. Add clr.Red etc...
//			}
//
//			this.JumpToNextByteStart();
//
//			return cmatrix;
//		}
//		public ColorMatrix ReadColorMatrix()
//		{
//			ColorMatrix cmatrix = new ColorMatrix();
//			bool bHasAddTerms = this.ReadBoolean();
//			bool bHasMultTerms = this.ReadBoolean();
//			int nNumBits = (int)this.ReadBits(4);
//
//			if (bHasMultTerms)
//			{
//				this.ReadBitArray(3, nNumBits, false); //TODO: signed?
//				//cmatrix. Multiply clr.Red etc...
//			}
//			if (bHasAddTerms)
//			{
//				this.ReadBitArray(3, nNumBits, false);
//				//cmatrix. Add clr.Red etc...
//			}
//
//			this.JumpToNextByteStart();
//
//			return cmatrix;
//		}


		public ArrayList ReadFillStyleArray(bool bWithAlpha, bool bExtended, bool morph)
		{
			ArrayList styles = new ArrayList();

			this.JumpToNextByteStart();
			int nFillStyleCount = this.ReadByte();
			if (nFillStyleCount == 255 && bExtended)
				nFillStyleCount = this.ReadUInt16();
			
			for (int i = 0; i < nFillStyleCount; i++)
			{
				Style.FillStyle style = new Style.FillStyle(this, bWithAlpha, morph);
				styles.Add(style);
			}
			return styles;
		}

		public ArrayList ReadLineStyleArray(bool bWithAlpha, bool bExtended, bool morph, bool bHasX)
		{
			ArrayList styles = new ArrayList();

			this.JumpToNextByteStart();
			int nLineStyleCount = this.ReadByte();
			if (nLineStyleCount == 255 && bExtended)
				nLineStyleCount = this.ReadUInt16();
			
			for (int i = 0; i < nLineStyleCount; i++)
			{
				Style.LineStyle style = new Style.LineStyle(this, bWithAlpha, morph, bHasX); //(Flash.Tags)this.TagCode);
				styles.Add(style);
			}
			return styles;
		}

		public string ReadUnicodePascalString()
		{
			//TODO:!!
			string s = "";
			byte nLength = base.ReadByte();
			for (byte i = 0; i < nLength; i++)
			{
				char c = base.ReadChar();
				if (i %2 == 1)
					s+=c;
			}
			if ((nLength % 2) == 0)
				base.ReadByte();
			return s;
		}
	}
}
