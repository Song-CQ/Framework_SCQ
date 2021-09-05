using System;
using System.Drawing;
using System.ComponentModel;

namespace Endogine
{
	[Serializable]

	/// <summary>
	/// Summary description for EPointF.
	/// </summary>
	public class EPointF
	{
		private float x = 0;
		private float y = 0;

		#region Constructors
		public EPointF()
		{
		}
		public EPointF(float X, float Y)
		{
			x = X;
			y = Y;
		}
		public EPointF(int X, int Y)
		{
			x = X;
			y = Y;
		}
		public EPointF(PointF pnt)
		{
			x = pnt.X;
			y = pnt.Y;
		}
		public EPointF(Point pnt)
		{
			x = pnt.X;
			y = pnt.Y;
		}
		public EPointF(string s)
		{
			//TODO: add to EPointF and Rectangles as well
			string[] ss = s.Split(';');
			if (s.IndexOf("=")>0)
			{
				x = Convert.ToSingle(ss[0].Split('=')[1]);
				y = Convert.ToSingle(ss[1].Split('=')[1]);
			}
			else
			{
				x = Convert.ToSingle(ss[0]);
				y = Convert.ToSingle(ss[1]);
			}
		}

		public EPointF Copy()
		{
			return new EPointF(x,y);
		}

		public static EPointF FromLengthAndAngle(float a_fLength, float a_fAngle)
		{
			EPointF pnt = new EPointF(0,-1);
			pnt.Angle = a_fAngle;
			pnt.Length = a_fLength;
			return pnt;
		}
		#endregion

		#region Converts
		public EPoint ToEPoint()
		{
			return new EPoint((int)x,(int)y);
		}
		public PointF ToPointF()
		{
			return new PointF(x,y);
		}
		public Point ToPoint()
		{
			return new Point((int)x,(int)y);
		}
		public SizeF ToSizeF()
		{
			return new SizeF(x,y);
		}
		public Size ToSize()
		{
			return new Size((int)x,(int)y);
		}
		#endregion


		#region Operators
		public static EPointF operator -(EPointF p1, EPointF p2)
		{
//			if (p1==null || p2 == null)
//			{
//				throw new Exception("Not null, please");
//			}
			return new EPointF(p1.X-p2.X, p1.Y-p2.Y);
		}
		public static EPointF operator +(EPointF p1, EPointF p2)
		{
			return new EPointF(p1.X+p2.X, p1.Y+p2.Y);
		}
		public static EPointF operator *(EPointF p1, EPointF p2)
		{
			return new EPointF(p1.X*p2.X, p1.Y*p2.Y);
		}
		public static EPointF operator *(EPointF p1, EPoint p2)
		{
			return new EPointF(p1.X*p2.X, p1.Y*p2.Y);
		}
		public static EPointF operator *(EPointF p1, float f)
		{
			return new EPointF(p1.X*f, p1.Y*f);
		}
		public static EPointF operator /(EPointF p1, EPointF p2)
		{
			return new EPointF(p1.X/p2.X, p1.Y/p2.Y);
		}
		public static EPointF operator /(EPointF p1, float f)
		{
			return new EPointF(p1.X/f, p1.Y/f);
		}
		#endregion

		public float X
		{
			get {return x;}
			set {x = value;}
		}
		public float Y
		{
			get {return y;}
			set {y = value;}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Length
		{
			get {return (float)Math.Sqrt(x*x+y*y);}
			set
			{
				float fAngle = Angle;
				x = (float)Math.Sin(fAngle)*value; y=-(float)Math.Cos(fAngle)*value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float Angle
		{
			get {return (float)Math.Atan2(x,-y);}
			set
			{
				float fLength = Length;
				x = (float)Math.Sin(value)*fLength; y=-(float)Math.Cos(value)*fLength;
			}
		}

		public override bool Equals(object obj)
		{
			EPointF pnt = (EPointF)obj;
			return (pnt.X == x && pnt.Y == y);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		public override string ToString()
		{
			return "x="+x.ToString()+";y="+y.ToString();
		}
	}
}
