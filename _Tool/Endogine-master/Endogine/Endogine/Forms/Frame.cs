using System;
using System.Drawing;
using System.Collections;
using Endogine.ResourceManagement;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for Frame.
	/// </summary>
	public class Frame : Sprite
	{
		protected ArrayList m_aSprites;
		protected Sprite[,] _spriteArray;
		protected int m_nRemoveForInterpolation;

		protected ERectangleF _cuttingRectFract;
		protected ERectangle _cuttingRectPixels;
		protected ERectangle _midSection;

		public Frame()
		{
			this.m_bMeInvisibleButNotChildren = true;
			this.m_bNoScalingOnSetRect = true;
			Name = "Frame";

			m_aSprites = new ArrayList();

//			for (int i = 0; i < 9; i++)
//				m_aSprites.Add(new Sprite());

			this._spriteArray = new Sprite[3,3];

			this._cuttingRectFract = new ERectangleF(0.5f,0.5f,0.01f,0.01f);
		}

		private void CalcSourceRects()
		{
			MemberSpriteBitmap mb = this.Member;

			this._cuttingRectPixels = ERectangle.FromLTRB(
				(int)(this._cuttingRectFract.Left*mb.Size.X),
				(int)(this._cuttingRectFract.Top*mb.Size.Y),
				(int)(this._cuttingRectFract.Right*mb.Size.X),
				(int)(this._cuttingRectFract.Bottom*mb.Size.Y));

			if (this._cuttingRectFract.Width > 0.001f && this._cuttingRectPixels.Width == 0)
				this._cuttingRectPixels.Width++;
			if (this._cuttingRectFract.Height > 0.001f && this._cuttingRectPixels.Height == 0)
				this._cuttingRectPixels.Height++;


			ERectangleF[,] rects = this.CreateRectanglesFromCuttingRect(this._cuttingRectPixels.ToERectangleF(), this.Member.Size.ToEPointF());
			this._midSection = rects[1,1].ToERectangle();

			for (int x=0; x<3; x++)
			{
				for (int y=0; y<3; y++)
				{
					ERectangleF rct = rects[x,y];
					if (rct.Width == 0 || rct.Height == 0)
					{
						if (this._spriteArray[x,y] != null)
						{
							this._spriteArray[x,y].Dispose();
							this._spriteArray[x,y] = null;
						}
					}
					else
					{
						Sprite sp = this._spriteArray[x,y];
						if (sp == null)
						{
							sp = new Sprite();
							sp.Parent = this;
							sp.Name = x.ToString()+";"+y.ToString();
							this._spriteArray[x,y] = sp;
						}
						sp.Member = this.Member;
						sp.SourceRect = rct.ToERectangle();
						sp.RegPoint = sp.SourceRect.Location;
					}
				}
			}

			this.m_aSprites.Clear();
			for (int x=0; x<3; x++)
				for (int y=0; y<3; y++)
					if (this._spriteArray[x,y]!=null)
						this.m_aSprites.Add(this._spriteArray[x,y]);
		}

		public override MemberSpriteBitmap Member
		{
			get {	return base.Member;}
			set
			{
				base.Member = value;

				MemberSpriteBitmap mb = value;

				this.CalcSourceRects();

//				//unfortunately, GDI+ always interpolates when stretching, so remove last pixels on right/down:
//				m_nRemoveForInterpolation = 1;
//				int nRemove = m_nRemoveForInterpolation;
			}
		}

		/// <summary>
		/// How to cut up the source bitmap for stretching. Default is to stretch the middle pixel.
		/// </summary>
		public ERectangleF CuttingRect
		{
			get {return this._cuttingRectFract;}
			set {this._cuttingRectFract = value;}
		}

		private ERectangleF[,] CreateRectanglesFromCuttingRect(ERectangleF rct, EPointF fullSize)
		{
			ERectangleF[,] rects = new ERectangleF[3,3];

			float left = 0;
			float right = 0;
			for (int x=0; x<3; x++)
			{
				if (x==0) right = rct.Left;
				else if (x==1) right = rct.Right;
				else right = fullSize.X;

				float top = 0;
				float bottom = 0;
				for (int y=0; y<3; y++)
				{
					if (y==0) bottom = rct.Top;
					else if (y==1) bottom = rct.Bottom;
					else bottom = fullSize.Y;

					//if (!(bottom-top == 0 || right-left == 0)) //right-left: could be optimized outside this loop
					rects[x,y] = ERectangleF.FromLTRB(left,top,right,bottom);

					top = bottom;
				}
				left = right;
			}

			return rects;
		}

		public override RasterOps.ROPs Ink
		{
			get {return base.Ink;}
			set 
			{
				base.Ink = value;
				for (int i = 0; i < m_aSprites.Count; i++)
				{
					((Sprite)m_aSprites[i]).Ink = value;
				}
			}
		}

		public override Color Color
		{
			get {	return base.Color;	}
			set
			{
				for (int i = 0; i < m_aSprites.Count; i++)
				{
					((Sprite)m_aSprites[i]).Color = value;
				}
				base.Color = value;
			}
		}


		public override ERectangleF Rect
		{
			get { return base.Rect;}
			set
			{
				base.Rect = value;

				m_nRemoveForInterpolation = 1;
				int nAdd= m_nRemoveForInterpolation;

				//we don't want this to be scaled (that would make the 9 sprite scale as well) so set SourceRect to same size
				//set it directly, we don't want to recalc the output rect (as SourceRect would do)
				//this.m_rctSrcClip = new Rectangle(0,0,(int)Rect.Width, (int)Rect.Height);

				//the corners just need locs, the sides need rect (stretching on one axis), the middle tile needs rect (on both axes)

				ERectangle rctMid = this._midSection;

				EPoint pntUnstretchedSize = this.Member.Size - rctMid.Size;
				EPointF pntSizeOfStretchedInNewRect = value.Size-pntUnstretchedSize.ToEPointF();

				//make sure the mid rect isn't stretched if there shouldn't be a mid section on that axis:
				if (rctMid.Width == 0)
					pntSizeOfStretchedInNewRect.X = 0;
				if (rctMid.Height == 0)
					pntSizeOfStretchedInNewRect.Y = 0;

				ERectangleF rctStretchedMid = new ERectangleF(rctMid.TopLeft.ToEPointF(), pntSizeOfStretchedInNewRect);

				//strange, bizarre:
//				float fBizarre = rctStretchedMid.Height - (int)rctStretchedMid.Height;
//				rctStretchedMid.Height-=fBizarre*2;
				
				//when it's smaller than original bitmap, just make it equally smaller in all sections
				if (value.Width < this.Member.Size.X)
				{
					float f = value.Width / this.Member.Size.X;
					rctStretchedMid.X=(f*rctMid.X);
					rctStretchedMid.Right=(f*rctMid.Right);
				}
				if (value.Height < this.Member.Size.Y)
				{
					float f = value.Height / this.Member.Size.Y;
					rctStretchedMid.Y=(f*rctMid.Y);
					rctStretchedMid.Bottom=(f*rctMid.Bottom);
				}

				ERectangleF[,] rects = this.CreateRectanglesFromCuttingRect(rctStretchedMid, value.Size);
				//EH.Put(value.ToString() + "  " + rects[2,2].Bottom.ToString() + "  " + value.Size.Y.ToString());

				
				//does it fill width and height properly? not too big or small?
				float fWidth = rects[0,0].Width+rects[1,0].Width+rects[2,0].Width;
				float fScaleX = value.Size.X/fWidth;
				float fHeight = rects[0,0].Height+rects[0,1].Height+rects[0,2].Height;
				float fScaleY = value.Size.Y/fHeight;


				for (int x=0; x<3; x++)
					for (int y=0; y<3; y++)
						if (this._spriteArray[x,y]!=null)
						{
							Sprite spX = this._spriteArray[x,y];
							if (rects[x,y].IsEmpty)
								spX.Visible = false;
							else
							{
								spX.Visible = true;
								spX.Rect = rects[x,y];
								//spX.Rect.Size*=new EPointF(fScaleX,fScaleY);
							}
						}
			}
		}
	}
}
