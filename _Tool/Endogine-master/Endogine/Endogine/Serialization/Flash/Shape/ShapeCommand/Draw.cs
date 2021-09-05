using System;

namespace Endogine.Serialization.Flash.Shape.ShapeCommand
{
	/// <summary>
	/// Summary description for Draw.
	/// </summary>
	public abstract class Draw : Base
	{
		public Draw()
		{
		}

		public virtual System.Collections.ArrayList GeneratePoints(EPoint ptStart)
		{
			return null;
		}
		public virtual void AddToPath(System.Drawing.Drawing2D.GraphicsPath path, EPoint ptStart, float scale)
		{
		}

		public override bool MovesTurtle
		{
			get {return true;}
		}
		public override bool Draws
		{
			get {return true;}
		}

		public Curve GetAsCurve()
		{
			if (this is Curve)
				return (Curve)this;

			EPoint pt = ((Line)this).Offset;
			EPoint ptNew = pt/2;
			return new Curve(ptNew.Copy(), ptNew);
		}
	}
}
