using System;

namespace Endogine.Serialization.Flash.Shape.ShapeCommand
{
	/// <summary>
	/// Summary description for Curve.
	/// </summary>
	public class Curve : Draw
	{
		EPoint _ptControl;
		EPoint _ptAnchor;
		public Curve(EPoint ptControl, EPoint ptAnchor)
		{
			this._ptControl = ptControl;
			this._ptAnchor = ptAnchor;
		}

		public EPoint Control
		{
			get {return this._ptControl;}
		}
		public EPoint Anchor
		{
			get {return this._ptAnchor;}
		}


		public override EPoint GetNewLoc(EPoint ptStart)
		{
			return ptStart + this._ptControl + this._ptAnchor;
		}

		public override void AddToPath(System.Drawing.Drawing2D.GraphicsPath path, EPoint ptStart, float scale)
		{
			System.Collections.ArrayList pts = this.GeneratePoints(ptStart);
			path.AddBezier(
				((EPointF)pts[0]).X*scale, ((EPointF)pts[0]).Y*scale,
				((EPointF)pts[1]).X*scale, ((EPointF)pts[1]).Y*scale,
				((EPointF)pts[2]).X*scale, ((EPointF)pts[2]).Y*scale,
				((EPointF)pts[3]).X*scale, ((EPointF)pts[3]).Y*scale);
		}

		public override System.Collections.ArrayList GeneratePoints(EPoint ptStart)
		{
			//Calculate curve's control and end anchor points:
			//Control point is the start point + offset to control
			EPoint ptControl = ptStart + this._ptControl;
			//End point is control point + offset to anchor:
			EPoint ptAnchor = ptControl + this._ptAnchor;
		
			//now calculate two bezier handles for the curve (Flash uses quadratic beziers, GDI+ uses cubic).
			//The two handles are 2/3rds from each endpoint to the control point.
			EPointF diff = (ptControl-ptStart).ToEPointF();
			EPointF ctrl1 = EPointF.FromLengthAndAngle(diff.Length*2/3, diff.Angle) + ptStart.ToEPointF();
			//EPointF ctrl1 = ptStart.ToEPointF() + diff/2f;
			//EPointF ctrl1 = new EPointF(ptStart.X + (1f * (ptControl.X - ptStart.X) / 2f), ptStart.Y + (1f * (ptControl.Y - ptStart.Y) / 2f));

			diff = (ptControl-ptAnchor).ToEPointF();
			EPointF ctrl2 = EPointF.FromLengthAndAngle(diff.Length*2/3, diff.Angle) + ptAnchor.ToEPointF();
			//diff = (ptAnchor-ptControl).ToEPointF();
			//EPointF ctrl2 = ptControl.ToEPointF() + diff/2f;
			//ctrl2 = new EPointF(ptControl.X + (1f * (ptAnchor.X - ptControl.X) / 2f), ptControl.Y + (1f * (ptAnchor.Y - ptControl.Y) / 2f));

			System.Collections.ArrayList pts = new System.Collections.ArrayList();
			pts.Add(ptStart.ToEPointF());
			pts.Add(ctrl1);
			pts.Add(ctrl2);
			pts.Add(ptAnchor.ToEPointF());
			return pts;
		}

		public override string ToString()
		{
			return this._ptControl.ToStringSimple() +" "+ this._ptAnchor.ToStringSimple();
		}

	}
}
