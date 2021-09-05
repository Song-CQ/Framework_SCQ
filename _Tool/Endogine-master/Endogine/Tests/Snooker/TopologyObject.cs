using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;

using Endogine;
using Endogine.GameHelpers;

namespace Snooker
{
	/// <summary>
	/// Summary description for GravityPoint.
	/// </summary>
	public class TopologyObject : Sprite
	{
		private ArrayList affectSprites;

		private float fMaxRange = 1;
		private float fMinRange = 0;
		private float fDepth = -10;
		public enum Functions
		{SinHalf, CosHalf, Sin, Cos, Linear};
		private Functions function;

		private Bitmap bmpTopology;
		private Bitmap bmpInclination;
		private bool bConstructing = true;

		public TopologyObject()
		{
			this.affectSprites = new ArrayList();
			this.Ink = RasterOps.ROPs.AddPin;
		}

		public void DoneConstructing()
		{
			//TODO: this is needed in many places - tell an object that the code calling it's contructor
			//is done with modifying settings.
			this.bConstructing = false;
			this.CreateVisualization();
		}

		private void CreateVisualization()
		{
			if (this.bConstructing)
				return;

			this.bmpTopology = this.CreateBitmap();

			Bitmap bmp = (Bitmap)this.bmpTopology.Clone();
			Endogine.BitmapHelpers.Filters.EmbossFrom24BitGrayscale(bmp);

			this.bmpInclination = this.CreateInclinationBitmap(this.bmpTopology);
			bmp = this.bmpInclination;
//			Bitmap bmp2 = this.SplitInvert(bmp);

			MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
			//mb.CenterRegPoint();
			this.Member = mb;
			this.Ink = RasterOps.ROPs.AddPin;

			for (int i = this.ChildCount-1; i >= 0; i--)
				this.GetChildByIndex(i).Dispose();

//			Sprite sp = new Sprite();
//			sp.Parent = this;
//			MemberSpriteBitmap mb2 = new MemberSpriteBitmap(bmp2);
//			sp.Member = mb2;
//			sp.Ink = RasterOps.ROPs.SubtractPin;
		}

		[Category("Topology"),
		Description("Outer radius of the created circle")]
		public float MaxRange
		{
			get {return this.fMaxRange;}
			set
			{
				this.fMaxRange = value;
				this.CreateVisualization();
			}
		}
		[Category("Topology"),
		Description("Inner radius of the created circle")]
		public float MinRange
		{
			get {return this.fMinRange;}
			set
			{
				this.fMinRange = value;
				this.CreateVisualization();
			}
		}
		[Category("Topology"),
		Description("Max depth of the topology. A negative value means it's a bump, not a hole")]
		public float Depth
		{
			get {return this.fDepth;}
			set
			{
				this.fDepth = value;
				this.CreateVisualization();
			}
		}
		[Category("Topology"),
		Description("The topology shape")]
		public Functions Function
		{
			get {return this.function;}
			set
			{
				this.function = value;
				this.CreateVisualization();
			}
		}

		

		public void AddSprite(GameSprite sp)
		{
			this.affectSprites.Add(sp);
		}
		public void RemoveSprite(GameSprite sp)
		{
			this.affectSprites.Remove(sp);
		}

		private float GetAltitudeOnDistance(float fDistance)
		{
			float fWhereInRange = (fDistance-this.fMinRange)/(this.fMaxRange-this.fMinRange);
			fWhereInRange = Math.Min(1f, Math.Max(0f, fWhereInRange));
			
			float fVal = 0;
			
			if (this.function == Functions.CosHalf)
				fVal = (float)Math.Cos(fWhereInRange*Math.PI);
			else if (this.function == Functions.Cos)
				fVal = (float)Math.Cos(fWhereInRange*Math.PI*2);
			else if (this.function == Functions.Linear)
				fVal = 1.0f-fWhereInRange;
			else if (this.function == Functions.Sin)
				fVal = (float)Math.Sin(fWhereInRange*Math.PI*2);
			else if (this.function == Functions.SinHalf)
				fVal = (float)Math.Sin(fWhereInRange*Math.PI);

//			fVal = (float)Math.Cos(fWhereInRange*Math.PI*2);
//			fVal = (float)Math.Cos(fWhereInRange*Math.PI*4);
			return fVal*fDepth;
//			return fWhereInRange*fDepth;
		}

		private EPointF GetForceOnPoint(EPointF pnt)
		{
			//because of error in inclination bitmap edges:
			ERectangleF rct = this.Rect + new ERectangleF(2,2,-4,-4);
			if (!rct.Contains(pnt))
				return new EPointF();

			EPointF pntDiff = pnt-this.Loc;
			EPoint pntInside = pntDiff.ToEPoint();

			Color clr = this.bmpInclination.GetPixel(pntInside.X, pntInside.Y);
			//EPointF pntForce = EPointF.FromLengthAndAngle((float)clr.B/1000, (float)Math.Atan2((clr.R-128), -(clr.G-128)));
			EPointF pntForce = new EPointF(-(clr.R-128), -(clr.G-128))/600;
			
			return pntForce;
		}

		private Bitmap CreateBitmap()
		{
			//creates a 24-bit topology bitmap
			int radius = (int)this.fMaxRange;
		
			Bitmap bmpQuarter = new Bitmap(radius,radius, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			//Render 1/4 of the circle and copy/paste the others
			for (int y = 0; y < radius; y++)
			{
				for (int x = 0; x < radius; x++)
				{
					EPointF pnt = new EPointF(radius-x,radius-y);
					float altitude = this.GetAltitudeOnDistance(pnt.Length);

					float fMaxAltitude = 20;
					int nClr = 8388608+(int)(altitude/fMaxAltitude*8388608);
					Color clr = Color.FromArgb(nClr);
					bmpQuarter.SetPixel(x,y,clr);
				}
			}

			Bitmap bmp = Endogine.BitmapHelpers.Filters.FoldOut(bmpQuarter, (float)Math.PI*2, 3);

			return bmp;
		}

		/// <summary>
		/// Creates a 24-bit bitmap from a 24-bit grayscale bitmap where the first 8 bits is
		/// inclination on x axis and the next 8 bits are inclination on y axis
		/// </summary>
		/// <param name="bmpTopology"></param>
		/// <returns></returns>
		private Bitmap CreateInclinationBitmap(Bitmap bmpTopology)
		{
			Bitmap bmpInc = new Bitmap(bmpTopology.Width, bmpTopology.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

			BitmapData bmpIncData = Endogine.BitmapHelpers.BitmapHelper.GetBitmapData(bmpInc);
			BitmapData bmpTopoData = Endogine.BitmapHelpers.BitmapHelper.GetBitmapData(bmpTopology);

			int bppInc = Endogine.BitmapHelpers.BitmapHelper.GetBytesPerPixel(bmpInc);
			int bppTopo = Endogine.BitmapHelpers.BitmapHelper.GetBytesPerPixel(bmpTopology);
			
			unsafe
			{
				for (int y = 1; y < bmpTopology.Height; y++)
				{
					byte* ptrInc = Endogine.BitmapHelpers.BitmapHelper.GetPointerToLineStart(bmpIncData, y);
					byte* ptrTopo = Endogine.BitmapHelpers.BitmapHelper.GetPointerToLineStart(bmpTopoData, y);

					for (int x = 1; x < bmpTopology.Width; x++)
					{
						ptrTopo+=bppTopo;
						ptrInc+=bppInc;

						int nClr = Endogine.BitmapHelpers.BitmapHelper.GetNBits(ptrTopo, bppTopo);
						int nClrSide = Endogine.BitmapHelpers.BitmapHelper.GetNBits(ptrTopo-1*bppTopo, bppTopo);
						int nClrAbove = Endogine.BitmapHelpers.BitmapHelper.GetNBits(ptrTopo-bmpTopoData.Stride, bppTopo);

						EPointF pntAngles = new EPointF(nClrSide-nClr, nClrAbove-nClr);
							pntAngles/=10;
							*ptrInc = 0;
							*(ptrInc+1) = (byte)Math.Min(255,Math.Max(0,128-(int)pntAngles.Y/100));
							*(ptrInc+2) =  (byte)Math.Min(255,Math.Max(0,128-(int)pntAngles.X/100));
					}
				}
			}
			bmpInc.UnlockBits(bmpIncData);
			bmpTopology.UnlockBits(bmpTopoData);

			return bmpInc;
		}

		public override void EnterFrame()
		{
			base.EnterFrame ();
			foreach (GameSprite sp in this.affectSprites)
			{
				//EPointF pnt = this.GetInclinationAndAngleOnPoint(sp.Loc);
				EPointF pnt = this.GetForceOnPoint(sp.Loc);
				sp.Velocity+= pnt;

//				EPointF pntDiff = this.Loc - sp.Loc;
//				float fForce = this.GetForceOnDistance(pntDiff.Length);
//				sp.Velocity+=EPointF.FromLengthAndAngle(fForce, pntDiff.Angle);
			}
		}



		private Bitmap SplitInvert(Bitmap bmp)
		{
			//split into two bitmaps, one with values > 127 and the other with values <= 127
			//The first to be used with AddPin, the other with SubtractPin
			Bitmap bmp2 = (Bitmap)bmp.Clone();

			int nBpp = Endogine.BitmapHelpers.BitmapHelper.GetBytesPerPixel(bmp);
			//TODO: writeonly
			System.Drawing.Imaging.BitmapData bmp2Data = Endogine.BitmapHelpers.BitmapHelper.GetBitmapData(bmp2);
			System.Drawing.Imaging.BitmapData bmpData = Endogine.BitmapHelpers.BitmapHelper.GetBitmapData(bmp);

			unsafe
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					byte* ptr = (byte*)bmpData.Scan0 + bmpData.Stride*y;
					byte* ptr2 = (byte*)bmp2Data.Scan0 + bmp2Data.Stride*y;

					for (int x = 0; x < bmp.Width; x++)
					{
						int nClr = 0;
						for (int nByte = 0; nByte < nBpp; nByte++)
							nClr += *(ptr+nByte);

						nClr/=3;

						int nClr1 = 0;
						int nClr2 = 0;
						if (nClr > 127)
						{
							nClr1 = Math.Min(255,(nClr-127)*2);
							nClr2 = 0;
						}
						else
						{
							nClr1 = 0;
							nClr2 = Math.Min(255, (128-nClr)*2);
						}
						for (int nByte = 0; nByte < nBpp; nByte++)
							*(ptr+nByte) = (byte)nClr1;
						for (int nByte = 0; nByte < nBpp; nByte++)
							*(ptr2+nByte) = (byte)nClr2;

						ptr+=nBpp;
						ptr2+=nBpp;
					}
				}
			}
			bmp.UnlockBits(bmpData);
			bmp2.UnlockBits(bmp2Data);

			return bmp2;
		}
	}
}
