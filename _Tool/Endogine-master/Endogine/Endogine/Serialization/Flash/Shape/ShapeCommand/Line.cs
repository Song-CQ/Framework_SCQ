using System;

namespace Endogine.Serialization.Flash.Shape.ShapeCommand
{
	/// <summary>
	/// Summary description for Line.
	/// </summary>
	public class Line : Draw
	{
		EPoint _ptTarget;
		public Line(EPoint ptTarget)
		{
			this._ptTarget = ptTarget;
		}

		public override EPoint GetNewLoc(EPoint ptStart)
		{
			return ptStart + this._ptTarget;
		}

		public override void AddToPath(System.Drawing.Drawing2D.GraphicsPath path, EPoint ptStart, float scale)
		{
			EPoint ptEnd = ptStart + this._ptTarget;
			path.AddLine(ptStart.X*scale, ptStart.Y*scale, ptEnd.X*scale, ptEnd.Y*scale);
		}
		public override System.Collections.ArrayList GeneratePoints(EPoint ptStart)
		{
			System.Collections.ArrayList pts = new System.Collections.ArrayList();
			EPoint ptEnd = ptStart + this._ptTarget;
			pts.Add(ptStart);
			pts.Add(ptEnd);
			return pts;
		}

		public override string ToString()
		{
			return this._ptTarget.ToStringSimple();
		}

		public EPoint Offset
		{
			get {return this._ptTarget;}
		}
	}
}
