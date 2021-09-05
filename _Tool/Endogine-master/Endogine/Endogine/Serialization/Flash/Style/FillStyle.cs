using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Endogine.Serialization.Flash.Style
{
	/// <summary>
	/// Summary description for FillStyle.
	/// </summary>
	public class FillStyle
	{
		public enum FillStyleTypes
		{
			Solid=0,
			LinearGradient=16,
			RadialGradient=18,
			RepeatingBitmap=64,
			ClippedBitmap=65,
			NonSmoothedRepeatingBitmap=66,
			NonSmoothedClippedBitmap=67
		}

		public FillStyleTypes FillStyleType;
		public Basic.Matrix Matrix;
		public Basic.Matrix MatrixEnd;
		public Color Color;
		public Color ColorEnd;

		public ushort CharacterId;
		public Basic.Matrix BitmapFillMatrix;
		public Basic.Matrix BitmapFillMatrixEnd;
		public ColorBlend Gradient;
		public ColorBlend GradientEnd;

		private float _morphPosition;

		public FillStyle(FillStyleTypes type)
		{
			this.FillStyleType = type;
		}

		public FillStyle(BinaryFlashReader reader, bool useAlpha, bool morph)
		{
			reader.JumpToNextByteStart();
			this.FillStyleType = (FillStyleTypes)(int)reader.ReadByte();

			switch (this.FillStyleType)
			{
				case FillStyleTypes.Solid:
					this.Color = reader.ReadColor(useAlpha);
					if (morph)
						this.ColorEnd = reader.ReadColor(useAlpha);
					break;
				case FillStyleTypes.LinearGradient:
				case FillStyleTypes.RadialGradient:
					this.Matrix = new Basic.Matrix(reader);
					if (morph)
					{
						this.MatrixEnd = new Basic.Matrix(reader);
					}
					ColorBlend[] gradients = Style.Gradient.ReadGradient(reader, useAlpha, morph);
					this.Gradient = gradients[0];
					if (morph)
						this.GradientEnd = gradients[1];
					break;

				case FillStyleTypes.RepeatingBitmap:
				case FillStyleTypes.ClippedBitmap:
				case FillStyleTypes.NonSmoothedRepeatingBitmap:
				case FillStyleTypes.NonSmoothedClippedBitmap:
					this.CharacterId = reader.ReadUInt16();
					this.BitmapFillMatrix = new Basic.Matrix(reader);
					if (morph)
						this.BitmapFillMatrixEnd = new Basic.Matrix(reader);
					break;
			}
		}

		public float MorphPosition
		{
			set {this._morphPosition = value;}
			get {return this._morphPosition;}
		}

		public Brush GetBrush()
		{
			Brush brush = null;
			switch (this.FillStyleType)
			{
				case FillStyleTypes.Solid:
					if (this.MorphPosition > 0)
						//brush = new SolidBrush(ColorEx.ColorHsb.InterpolateRgbInHsbSpace(Color.Red, Color.Green, this.MorphPosition));
						brush = new SolidBrush(ColorEx.ColorHsb.InterpolateRgbInHsbSpace(this.Color, this.ColorEnd, this.MorphPosition));
					else
						brush = new SolidBrush(this.Color);
					break;

				case FillStyleTypes.RadialGradient:
				case FillStyleTypes.LinearGradient:
					//TODO: radial
					if (this.MorphPosition > 0)
					{
						//TODO: this.GradientEnd this.MatrixEnd
					}
					LinearGradientBrush gradBrush = new LinearGradientBrush(new PointF(0,0), new PointF(0,1), Color.White, Color.Black);
					gradBrush.InterpolationColors = this.Gradient;
					gradBrush.Transform = this.Matrix.GetDotNet();
					brush = gradBrush;
					break;
				
				case FillStyleTypes.RepeatingBitmap:
				case FillStyleTypes.ClippedBitmap:
				case FillStyleTypes.NonSmoothedRepeatingBitmap:
				case FillStyleTypes.NonSmoothedClippedBitmap:
					MemberSpriteBitmap mb = (MemberSpriteBitmap)EH.Instance.CastLib.GetByName("Flash_"+this.CharacterId.ToString());
					Bitmap bmp = mb.Bitmap;

					if (this.MorphPosition > 0)
					{
						//TODO: this.BitmapFillMatrixEnd
					}
					//TODO: BitmapFillMatrix 
					ImageAttributes attr = new ImageAttributes();
					if (this.FillStyleType == FillStyleTypes.RepeatingBitmap ||
						this.FillStyleType == FillStyleTypes.NonSmoothedRepeatingBitmap)
						attr.SetWrapMode(WrapMode.Tile);
					else
						attr.SetWrapMode(WrapMode.Clamp);

					TextureBrush txBrush = new TextureBrush(bmp, new RectangleF(0,0,1,1), attr);
					if (this.Matrix!=null)
						txBrush.Transform = this.Matrix.GetDotNet();
					break;
			}
			return brush;
		}

		public FillStyle GetMorphed(float ratio)
		{
			FillStyle style = new FillStyle(this.FillStyleType);
			switch (this.FillStyleType)
			{
				case FillStyleTypes.Solid:
					style.Color = ColorEx.ColorHsb.InterpolateRgbInHsbSpace(this.Color, this.ColorEnd, ratio);
					break;

				case FillStyleTypes.RadialGradient:
				case FillStyleTypes.LinearGradient:
					style.Gradient = this.Gradient; //TODO: morph
					style.Matrix = this.Matrix; //TODO: morph
					break;

				case FillStyleTypes.RepeatingBitmap:
				case FillStyleTypes.ClippedBitmap:
				case FillStyleTypes.NonSmoothedRepeatingBitmap:
				case FillStyleTypes.NonSmoothedClippedBitmap:
					style.CharacterId = this.CharacterId; //TODO: morph
					style.BitmapFillMatrix = this.BitmapFillMatrix; //TODO: morph
					break;
			}
			return style;
		}
	}
}
