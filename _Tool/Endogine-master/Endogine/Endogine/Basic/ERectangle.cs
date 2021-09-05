using System;
using System.Drawing;
using System.ComponentModel;

namespace Endogine
{
	[Serializable]

	/// <summary>
	/// Summary description for ERectangle.
	/// </summary>
	public class ERectangle
	{
		private int x = 0;
		private int y = 0;
		private int width = 0;
		private int height = 0;

		public ERectangle()
		{
		}
		public ERectangle(int X, int Y, int Width, int Height)
		{
			x = X; y = Y; width = Width; height = Height;
		}
		public ERectangle(EPoint loc, EPoint size)
		{
			x = loc.X; y = loc.Y; width = size.X; height = size.Y;
		}
		public ERectangle(Rectangle rct)
		{
			x = rct.X; y = rct.Y; width = rct.Width; height = rct.Height;
		}
		public ERectangle(string s)
		{
			//TODO: add to EPointF and Rectangles as well
			string[] ss = s.Split(';');
			if (s.IndexOf("=")>0)
			{
				x = Convert.ToInt32(ss[0].Split('=')[1]);
				y = Convert.ToInt32(ss[1].Split('=')[1]);
				width = Convert.ToInt32(ss[2].Split('=')[1]);
				height = Convert.ToInt32(ss[3].Split('=')[1]);
			}
			else
			{
				x = Convert.ToInt32(ss[0]);
				y = Convert.ToInt32(ss[1]);
				width = Convert.ToInt32(ss[2]);
				height = Convert.ToInt32(ss[3]);
			}
		}


		public static ERectangle FromLTRB(int l, int t, int r, int b)
		{
			return new ERectangle(l,t, r-l, b-t);
		}

		public void Offset(EPoint pnt)
		{
			x+=pnt.X;
			y+=pnt.Y;
		}
		public void Offset(int X, int Y)
		{
			x+=X;
			y+=Y;
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
		public int Width
		{
			get {return width;}
			set {width = value;}
		}
		public int Height
		{
			get {return height;}
			set {height = value;}
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EPoint Location
		{
			get {return new EPoint(x,y);}
			set {x = value.X; y = value.Y;}
		}
		/// <summary>
		/// Corner opposite of Location. If width and height are positive, this is the bottom right corner
		/// If they are both negative, it's the top left corner etc.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EPoint OppositeCorner
		{
			get {return new EPoint(x+width,y+height);}
			set {width = value.X-x; y = value.Y-y;}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EPoint TopLeft
		{
			get {return new EPoint(this.Left,this.Top);}
			set {this.Left = value.X; this.Top = value.Y;}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EPoint BottomRight
		{
			get {return new EPoint(this.Right,this.Bottom);}
			set {this.Right = value.X; this.Bottom = value.Y;}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EPoint Middle
		{
			get {return new EPoint(x+width/2,y+height/2);}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EPoint Size
		{
			get {return new EPoint(width, height);}
			set {width = value.X; height = value.Y;}
		}

		/// <summary>
		/// Moves border and resizes rect.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Left
		{
			get {return this.width>=0?this.x:this.x+this.width;} //return x;
			set
			{
				if (this.width>0)
				{
					//JB 051014: this.width+=value-this.x;
					this.width-=value-this.x;
					this.x = value;
				}
				else
					this.width=value-this.x;
			}
		}
		/// <summary>
		/// Moves border and resizes rect.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Top
		{
			get {return this.height>=0?this.y:this.y+this.height;}
			set
			{
				if (this.height>0)
				{
					this.height-=value-this.y;
					this.y = value;
				}
				else
					this.height=value-this.y;
			}
		}
		/// <summary>
		/// Moves border and resizes rect.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Right
		{
			get {return this.width>0?this.x+this.width:this.x;}
			set
			{
				if (this.width>0)
					this.width = value-this.x;
				else
				{
					this.width-=value-this.x;
					this.x = value;
				}
			}
		}
		/// <summary>
		/// Moves border and resizes rect.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Bottom
		{
			get {return this.height>0?this.y+this.height:this.y;}
			set
			{
				if (this.height>0)
					this.height = value-this.y;
				else
				{
					this.height-=value-this.y;
					this.y = value;
				}
			}
		}
		/// <summary>
		/// If width is positive, it's the same as Right. If negative, it's the same as Left
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int OppositeX
		{
			get {return this.width>0?this.Right:this.Left;}
			set
			{
				if (this.width>0)
					this.Right = value;
				else
					this.Left = value;
			}
		}
		/// <summary>
		/// If height is positive, it's the same as Bottom. If negative, it's the same as Top
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int OppositeY
		{
			get {return this.height>0?this.Bottom:this.Top;}
			set
			{
				if (this.height>0)
					this.Bottom = value;
				else
					this.Top = value;
			}
		}



        public void Inflate(ERectangle rct)
        {
            this.height += rct.Width;
            this.width += rct.Height;
            this.x += rct.X;
            this.y += rct.Y;
        }
        /// <summary>
        /// adds pnt to size (this is what Rectangle does, it seems from the manual)
        /// </summary>
        /// <param name="pnt"></param>
        public void Inflate(EPoint pnt)
		{
			this.height+=pnt.X; //Math.Max(height, pnt.X);
			this.width+=pnt.Y; //Math.Max(width, pnt.Y);
		}
        /// <summary>
        /// Will expand this rectangle to enclose the provided rct.  Actually, a more natural name would be Union I think.
        /// </summary>
        /// <param name="rct"></param>
		public void Expand(ERectangle rct)
		{
			int oX = this.OppositeX;
			int oY = this.OppositeY;

			x = Math.Min(x, rct.X);
			y = Math.Min(y, rct.Y);

			this.OppositeX = Math.Max(oX, rct.OppositeX);
			this.OppositeY = Math.Max(oY, rct.OppositeY);
		}
		/// <summary>
		/// Expands the rectangle if needed so that it completely encloses rct
		/// If it had negative height or width, it stays that way.
		/// </summary>
		/// <param name="rct"></param>
		public void ExpandSpecial(ERectangle rct)
		{
			this.Left = rct.Left<this.Left?rct.Left:this.Left;
			this.Top = rct.Top<this.Top?rct.Top:this.Top;
			this.Right = rct.Right>this.Right?rct.Right:this.Right;
			this.Bottom = rct.Bottom>this.Bottom?rct.Bottom:this.Bottom;
		}
		public void ExpandSpecial(EPoint pnt)
		{
			this.Left = pnt.X<this.Left?pnt.X:this.Left;
			this.Top = pnt.Y<this.Top?pnt.Y:this.Top;
			this.Right = pnt.X>this.Right?pnt.X:this.Right;
			this.Bottom = pnt.Y>this.Bottom?pnt.Y:this.Bottom;
		}



		public void Normalize()
		{
			if (width < 0)
			{
				width=-width;
				x = x-width;
			}
			if (height < 0)
			{
				height=-height;
				y = y-height;
			}
		}
		public int GetArea()
		{
			return width*height;
		}
		public double GetHypo()
		{
			return Math.Sqrt(width*width + height*height);
		}


		public ERectangleF ToERectangleF()
		{
			return new ERectangleF(x,y,width,height);
		}
		public Rectangle ToRectangle()
		{
			return new Rectangle(x,y,width,height);
		}
		public RectangleF ToRectangleF()
		{
			return new RectangleF(x,y,width,height);
		}

		public bool IsEmpty
		{
			get {return (width==0 && height == 0);}
		}
		public bool IsNegative
		{
			get {return (width<0 || height < 0);}
		}

		public ERectangle Copy()
		{
			return new ERectangle(x,y,width,height);
		}
		public void Intersect(ERectangle rct)
		{
			int oY = this.OppositeY;
			int oX = this.OppositeX;
			x = Math.Max(rct.X, x);
			y = Math.Max(rct.Y, y);
			this.OppositeY = Math.Min(rct.OppositeY, oY);
			this.OppositeX = Math.Min(rct.OppositeX, oX);
//			if (rct.Left>this.Left)
//				this.Left = rct.Left;
//			if (rct.Top>this.Top)
//				this.Top = rct.Top;
//			if (rct.Right<this.Right)
//				this.Right = rct.Right;
//			if (rct.Bottom<this.Bottom)
//				this.Bottom = rct.Bottom;
		}
		public bool IntersectsWith(ERectangle rct)
		{
			ERectangle rctNew = this.Copy();
			rctNew.Intersect(rct);
			return !(rctNew.IsNegative || rctNew.IsEmpty);
		}
		public bool Contains(EPoint pnt)
		{
			return (pnt.X >= Left && pnt.X <= Right && pnt.Y >= Top && pnt.Y <= Bottom);
		}

		public void MakePointInside(EPoint pnt)
		{
			if (pnt.X<this.Left)
				pnt.X = this.Left;
			else if (pnt.X>this.Right)
				pnt.X = this.Right;
			if (pnt.Y<this.Top)
				pnt.Y = this.Top;
			else if (pnt.Y>this.Bottom)
				pnt.Y = this.Bottom;
		}
		public void WrapPointInside(EPoint pnt)
		{
			if (pnt.X < this.Left)
				pnt.X = this.Right - (this.Left - pnt.X);
			else if (pnt.X > this.Right)
				pnt.X = this.Left + (pnt.X - this.Right);
			if (pnt.Y < this.Top)
				pnt.Y = this.Bottom - (this.Top - pnt.Y);
			else if (pnt.Y > this.Bottom)
				pnt.Y = this.Top + (pnt.Y - this.Bottom);
		}
		public void MakeTopLeftAtOrigo()
		{
			if (width < 0)
				x = -width;
			else
				x = 0;
			if (height < 0)
				y = -height;
			else
				y = 0;
		}



		public static ERectangle operator +(ERectangle r1, ERectangle r2)
		{
			return new ERectangle(r1.X+r2.X, r1.Y+r2.Y, r1.Width+r2.Width, r1.Height+r2.Height);
		}
        public static ERectangle operator +(ERectangle r1, EPoint p1)
        {
            return new ERectangle(r1.X + p1.X, r1.Y + p1.Y, r1.width, r1.Height);
        }

		public static ERectangle operator *(ERectangle r1, int i)
		{
			return new ERectangle(r1.X*i, r1.Y*i, r1.Width*i, r1.Height*i);
		}
		public static ERectangle operator /(ERectangle r1, int i)
		{
			return new ERectangle(r1.X/i, r1.Y/i, r1.Width/i, r1.Height/i);
		}

        public static ERectangle operator *(ERectangle r1, EPoint pnt)
        {
            return new ERectangle(r1.X * pnt.X, r1.Y * pnt.Y, r1.Width * pnt.X, r1.Height * pnt.Y);
        }
        public static ERectangle operator /(ERectangle r1, EPoint pnt)
        {
            return new ERectangle(r1.X / pnt.X, r1.Y / pnt.Y, r1.Width / pnt.X, r1.Height / pnt.Y);
        }

        //public static bool operator ==(ERectangle r1, ERectangle r2)
        //{
        //    if (r1.Equals(null))
        //        return (r2.Equals(null));
        //    else if (r2.Equals(null))
        //        return false;
        //    return (r1.X == r2.X && r1.Y == r2.Y && r1.Width == r2.Width && r1.Height == r2.Height);
        //}
        //public static bool operator !=(ERectangle r1, ERectangle r2)
        //{
        //    if (r1.Equals(null))
        //        return !(r2.Equals(null));
        //    else if (r2.Equals(null))
        //        return true;
        //    return !(r1.X == r2.X && r1.Y == r2.Y && r1.Width == r2.Width && r1.Height == r2.Height);
        //}

        //TODO: how to override ==? I must be able to check r == null... and that leads to infinite loop!
        public bool IsEqualTo(ERectangle r)
        {
            if (r == null)
                return false;
            return (r.X == this.X && r.Y == this.Y && r.Width == this.Width && r.Height == this.Height);
        }


		public void Multiply(EPoint pnt)
		{
			x*=pnt.X;
			y*=pnt.Y;
			width*=pnt.X;
			height*=pnt.Y;
		}
		public void Multiply(int val)
		{
			x*=val;
			y*=val;
			width*=val;
			height*=val;
		}

		public override string ToString()
		{
			return "x="+x.ToString()+";y="+y.ToString()+";width="+width.ToString()+";height="+height.ToString();
		}
		public string ToStringSimple()
		{
			return x.ToString()+";"+y.ToString()+";"+width.ToString()+";"+height.ToString();
		}

	}
}
