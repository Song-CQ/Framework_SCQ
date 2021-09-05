using System;
using System.Drawing;
using System.ComponentModel;

namespace Endogine
{
	[Serializable]

	/// <summary>
	/// Summary description for ERectangle.
	/// </summary>
	public class ERectangleF
	{
		private float x = 0;
		private float y = 0;
		private float width = 0;
		private float height = 0;

		public ERectangleF()
		{
		}
        public ERectangleF(float X, float Y, float Width, float Height)
		{
            x = X; y = Y; width = Width; height = Height;
		}
		public ERectangleF(EPointF loc, EPointF size)
		{
			x = loc.X; y = loc.Y; width = size.X; height = size.Y;
		}
		public ERectangleF(RectangleF rct)
		{
			x = rct.X; y = rct.Y; width = rct.Width; height = rct.Height;
		}


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
		public float Width
		{
			get {return width;}
			set {width = value;}
		}
		public float Height
		{
			get {return height;}
			set {height = value;}
		}


		public static ERectangleF FromLTRB(float l, float t, float r, float b)
		{
			return new ERectangleF(l,t, r-l, b-t);
		}
        public static ERectangleF FromLTRB(EPointF leftTop, EPointF rightBottom)
        {
            return new ERectangleF(leftTop.X, leftTop.Y, rightBottom.X - leftTop.X, rightBottom.Y - leftTop.Y);
        }

		public void Offset(EPointF pnt)
		{
			x+=pnt.X;
			y+=pnt.Y;
		}
		public void Offset(float X, float Y)
		{
			x+=X;
			y+=Y;
		}

        [Browsable(false)] //TODO: should be browsable, but not serializable
        public EPointF Location
		{
			get {return new EPointF(x,y);}
			set {x = value.X; y = value.Y;}
		}

        /// <summary>
		/// Corner opposite of Location. If width and height are positive, this is the bottom right corner
		/// If they are both negative, it's the top left corner etc.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EPointF OppositeCorner
		{
			get {return new EPointF(x+width,y+height);}
			set {width = value.X-x; y = value.Y-y;}
		}
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EPointF TopLeft
		{
			get {return new EPointF(this.Left,this.Top);}
			set {this.Left = value.X; this.Top = value.Y;}
		}
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EPointF BottomRight
		{
			get {return new EPointF(this.Right,this.Bottom);}
			set {this.Right = value.X; this.Bottom = value.Y;}
		}
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EPointF Middle
		{
			get {return new EPointF(x+width/2,y+height/2);}
			set {this.Left = value.X-width/2; this.Top = value.Y-width/2;}
		}
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EPointF Size
		{
			get {return new EPointF(width, height);}
			set {width = value.X; height = value.Y;}
		}

		/// <summary>
		/// Moves border and resizes rect. If width is negative, only the width is changed.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Left
		{
			get {return this.width>=0?this.x:this.x+this.width;} //return x;
			set
			{
				if (this.width>0)
				{
					//this.width+=value-this.x;
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
        public float Top
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
        public float Right
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
        public float Bottom
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
        public float OppositeX
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
        public float OppositeY
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

		/// <summary>
		/// adds pnt to size (this is what Rectangle does, it seems from the manual)
		/// </summary>
		/// <param name="pnt"></param>
		public void Inflate(EPointF pnt)
		{
			this.height+=pnt.X; //Math.Max(height, pnt.X);
			this.width+=pnt.Y; //Math.Max(width, pnt.Y);
		}

        /// <summary>
        /// Will expand this rectangle to enclose the provided rct.  Actually, a more natural name would be Union I think.
        /// </summary>
        /// <param name="rct"></param>
		public void Expand(ERectangleF rct)
		{
			float oX = this.OppositeX;
			float oY = this.OppositeY;

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
		public void ExpandSpecial(ERectangleF rct)
		{
			this.Left = rct.Left<this.Left?rct.Left:this.Left;
			this.Top = rct.Top<this.Top?rct.Top:this.Top;
			this.Right = rct.Right>this.Right?rct.Right:this.Right;
			this.Bottom = rct.Bottom>this.Bottom?rct.Bottom:this.Bottom;
		}
		public void ExpandSpecial(EPointF pnt)
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
		public float GetArea()
		{
			return width*height;
		}
		public double GetHypo()
		{
			return Math.Sqrt(width*width + height*height);
		}


		public ERectangle ToERectangle()
		{
			return new ERectangle((int)x,(int)y,(int)width,(int)height);
		}
		public Rectangle ToRectangle()
		{
			return new Rectangle((int)x,(int)y,(int)width,(int)height);
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

		public ERectangleF Copy()
		{
			return new ERectangleF(x,y,width,height);
		}
		
		public void Intersect(ERectangleF rct)
		{
//			if (rct.Left>this.Left)
//				this.Left = rct.Left;
//			if (rct.Top>this.Top)
//				this.Top = rct.Top;
//			if (rct.Right<this.Right)
//				this.Right = rct.Right;
//			if (rct.Bottom<this.Bottom)
//				this.Bottom = rct.Bottom;
			float oY = this.OppositeY;
			float oX = this.OppositeX;
			x = Math.Max(rct.X, x);
			y = Math.Max(rct.Y, y);
			this.OppositeY = Math.Min(rct.OppositeY, oY);
			this.OppositeX = Math.Min(rct.OppositeX, oX);
		}
		public bool IntersectsWith(ERectangleF rct)
		{
			ERectangleF rctNew = this.Copy();
			rctNew.Intersect(rct);
			return !(rctNew.IsNegative || rctNew.IsEmpty);
		}
		public bool Contains(EPointF pnt)
		{
			return (pnt.X >= Left && pnt.X <= Right && pnt.Y >= Top && pnt.Y <= Bottom);
		}

		public void MakePointInside(EPointF pnt)
		{
			if (pnt.X<this.Left)
				pnt.X = this.Left;
			else if (pnt.X>this.Right)
				pnt.X = this.Right;
			if (pnt.Y<this.Top)
				pnt.Y = this.Top;
			else if (pnt.Y>this.Bottom)
				pnt.Y = this.Bottom;
			//pnt.X = Math.Max(pnt.X, x);
			//pnt.X = Math.Min(pnt.X, Right);
			//pnt.Y = Math.Max(pnt.Y, y);
			//pnt.Y = Math.Min(pnt.Y, Bottom);
		}
		public void WrapPointInside(EPointF pnt)
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
		/// <summary>
		/// Makes top left coordinate at 0,0. The width and/or height can still be negative 
		/// </summary>
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


		public static ERectangleF operator +(ERectangleF r1, ERectangleF r2)
		{
			return new ERectangleF(r1.X+r2.X, r1.Y+r2.Y, r1.width+r2.Width, r1.Height+r2.Height);
		}
        public static ERectangleF operator +(ERectangleF r1, EPointF p1)
        {
            return new ERectangleF(r1.X + p1.X, r1.Y + p1.Y, r1.width, r1.Height);
        }

		public static ERectangleF operator *(ERectangleF r1, float f)
		{
			return new ERectangleF(r1.X*f, r1.Y*f, r1.Width*f, r1.Height*f);
		}
		public static ERectangleF operator /(ERectangleF r1, float f)
		{
			return new ERectangleF(r1.X/f, r1.Y/f, r1.Width/f, r1.Height/f);
		}

        public static ERectangleF operator *(ERectangleF r1, EPointF pnt)
        {
            return new ERectangleF(r1.X * pnt.X, r1.Y * pnt.Y, r1.Width * pnt.X, r1.Height * pnt.Y);
        }
        public static ERectangleF operator /(ERectangleF r1, EPointF pnt)
        {
            return new ERectangleF(r1.X / pnt.X, r1.Y / pnt.Y, r1.Width / pnt.X, r1.Height / pnt.Y);
        }

        //public static bool operator ==(ERectangleF r1, ERectangleF r2)
        //{
        //    if (r1.Equals(null))
        //        return (r2.Equals(null));
        //    else if (r2.Equals(null))
        //        return false;
        //    return (r1.X == r2.X && r1.Y == r2.Y && r1.Width == r2.Width && r1.Height == r2.Height);
        //}
        //public static bool operator !=(ERectangleF r1, ERectangleF r2)
        //{
        //    if (r1.Equals(null))
        //        return !(r2.Equals(null));
        //    else if (r2.Equals(null))
        //        return true;
        //    return !(r1.X == r2.X && r1.Y == r2.Y && r1.Width == r2.Width && r1.Height == r2.Height);
        //}

        //TODO: how to override ==? I must be able to check r == null... and that leads to infinite loop!
        public bool IsEqualTo(ERectangleF r)
        {
            if (r == null)
                return false;
            return (r.X == this.X && r.Y == this.Y && r.Width == this.Width && r.Height == this.Height);
        }

		public void Multiply(EPointF pnt)
		{
			x*=pnt.X;
			y*=pnt.Y;
			width*=pnt.X;
			height*=pnt.Y;
		}
		public void Multiply(float val)
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
	}
}
