using System;
using System.Drawing;

namespace Endogine.Serialization.Flash.Style
{
	/// <summary>
	/// Summary description for LineStyle.
	/// </summary>
	public class LineStyle
	{
		public int Width;
		public int WidthEnd;
		public Color Color;
		public Color ColorEnd;
		private float _morphPosition;

		public LineStyle(Color color, int width)
		{
			this.Color = color;
			this.Width = width;
		}

		public LineStyle(BinaryFlashReader reader, bool useAlpha, bool morph, bool bHasX)
		{
			reader.JumpToNextByteStart();
			this.Width = reader.ReadUInt16();
			if (morph)
				this.WidthEnd = reader.ReadUInt16();
			//this.Width = this.WidthEnd = 60;

			this.Color = reader.ReadColor(useAlpha);
			if (morph)
				this.ColorEnd = reader.ReadColor(useAlpha);

			if (bHasX)
				reader.ReadUInt16(); //TODO: what's this for? Shape4 and 5.
		}

		public float MorphPosition
		{
			set {this._morphPosition = value;}
			get {return this._morphPosition;}
		}

//		public LineStyle GetMorphed(float ratio)
//		{
//			return new LineStyle(
//				ColorEx.ColorHsb.InterpolateRgbInHsbSpace(this.Color, this.ColorEnd, ratio),
//				(int)(this._morphPosition*(this.Width-this.WidthEnd)+this.WidthEnd));
//		}

		public Pen GetPen()
		{
			if (this._morphPosition > 0)
				return new Pen(ColorEx.ColorHsb.InterpolateRgbInHsbSpace(
					this.Color, this.ColorEnd, this._morphPosition),
					this._morphPosition*(this.WidthEnd-this.Width)+this.Width);
			return new Pen(this.Color, this.Width);
		}
	}
}
