using System;
using System.Drawing;
using System.ComponentModel;

namespace Endogine
{
	[Serializable]

	/// <summary>
	/// Summary description for EPointF.
	/// </summary>
	public class EPoint
	{
		private int x = 0;
		private int y = 0;

		public EPoint()
		{
		}
		public EPoint(float X, float Y)
		{
			x = (int)X;
			y = (int)Y;
		}
		public EPoint(int X, int Y)
		{
			x = X;
			y = Y;
		}
		public EPoint(PointF pnt)
		{
			x = (int)pnt.X;
			y = (int)pnt.Y;
		}
		public EPoint(Point pnt)
		{
			x = pnt.X;
			y = pnt.Y;
		}
		public EPoint(string s)
		{
			//TODO: add to Rectangles as well
			string[] ss = s.Split(';');
			if (s.IndexOf("=")>0)
			{
				x = Convert.ToInt32(ss[0].Split('=')[1]);
				y = Convert.ToInt32(ss[1].Split('=')[1]);
			}
			else
			{
				x = Convert.ToInt32(ss[0]);
				y = Convert.ToInt32(ss[1]);
			}
		}
		public EPoint Copy()
		{
			return new EPoint(x,y);
		}


		public EPointF ToEPointF()
		{
			return new EPointF(x,y);
		}
		public PointF ToPointF()
		{
			return new PointF(x,y);
		}
		public Point ToPoint()
		{
			return new Point(x,y);
		}
		public SizeF ToSizeF()
		{
			return new SizeF(x,y);
		}
		public Size ToSize()
		{
			return new Size(x,y);
		}


		public static EPoint operator -(EPoint p1, EPoint p2)
		{
			return new EPoint(p1.X-p2.X, p1.Y-p2.Y);
		}
		public static EPoint operator +(EPoint p1, EPoint p2)
		{
			return new EPoint(p1.X+p2.X, p1.Y+p2.Y);
		}
		public static EPoint operator *(EPoint p1, EPoint p2)
		{
			return new EPoint(p1.X*p2.X, p1.Y*p2.Y);
		}
		public static EPoint operator *(EPoint p1, int i)
		{
			return new EPoint(p1.X*i, p1.Y*i);
		}
		public static EPoint operator /(EPoint p1, EPoint p2)
		{
			return new EPoint(p1.X/p2.X, p1.Y/p2.Y);
		}
		public static EPoint operator /(EPoint p1, int i)
		{
			return new EPoint(p1.X/i, p1.Y/i);
		}

		public int X
		{
			get {return x;}
			set {x = value;}
		}
		public int Y
		{
			get {return y;}
			set {y = value;}
		}


		public override bool Equals(object obj)
		{
			EPoint pnt = (EPoint)obj;
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
		public string ToStringSimple()
		{
			return x.ToString()+";"+y.ToString();
		}

	}
}
