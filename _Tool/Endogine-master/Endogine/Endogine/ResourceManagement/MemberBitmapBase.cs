using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Generic;

namespace Endogine.ResourceManagement
{
	/// <summary>
	/// Summary description for MemberBitmapBase.
	/// </summary>
	public abstract class MemberBitmapBase : MemberBase
	{
		protected EPoint m_pntRegPoint;

		protected EPoint m_sizeTotal; //The full size of the bitmap (if it's a bitmap with many frames, it's the size of the whole bitmap, not just a frame)
        //protected int m_nAnimationFrame = 0;
        protected bool m_bGotAnimation = false;
        //protected bool m_bAutoAnimate = false;

		protected bool m_bGotAlpha;

		//TODO: these only apply to GDI rendering, should move to GDIStrategy - since for D3D textures, it's decided at load time
		protected bool m_bGotOwnClrKey = false;
		protected Color m_clrKey;
		private static Color m_clrDefaultClrKey = Color.FromArgb(255,255,255);

		private Bitmap m_bmp;

		//static string DummyFilename; //this file is to be used when resource isn't found

		//One frame inside a bitmap/texture
		public class Frame
		{
			public string ID;
			public ERectangle Source; //TODO: use in future when texture anims don't have to be grids
			public EPoint Offset;
			public Frame(string id)
			{
				this.ID = id;
				this.Offset = new EPoint();
				this.Source = new ERectangle();
			}
		}

		public MemberBitmapBase()
		{
			//m_clrKey = Color.FromArgb(255,255,255);
			m_pntRegPoint = new EPoint(0,0);
            //m_sizeAnimateWithin = new EPoint(0,0);
			m_sizeTotal = new EPoint(0,0);
		}


		public virtual EPoint Size
		{
			get
			{
                return m_sizeTotal;
                //if (GotAnimation)
                //    return this.AnimateWithinSize;
                //return this.TotalSize;
			}
		}


		public virtual Bitmap Bitmap
		{
			get {return this.m_bmp;}
			set {this.m_bmp = value;} 
		}

		public Bitmap LoadIntoBitmap(string a_sFilename) //, ref string actualFilename)
		{
			//string sOrg = a_sFilename; //for debugging
			//a_sFilename = m_endogine.CastLib.FindFile(a_sFilename);
            a_sFilename = AppSettings.Instance.FindFile(a_sFilename);
			if (a_sFilename.Length == 0)
			{
				if (true) // || m_endogine.CastLib.UseDummiesForFilesNotFound)
				{
					//TODO: generalized dummy function
					a_sFilename = m_endogine.CastLib.DirectoryPath + "Cross.bmp";
				}
			}

			System.IO.FileInfo finfo = new System.IO.FileInfo(a_sFilename);

			Name = finfo.Name.Replace(finfo.Extension, "");
			FileFullName = a_sFilename;

			
			if (!finfo.Exists)
			{
				throw new System.IO.FileNotFoundException("Member file not found: "+a_sFilename);
			}


			//TODO: only allow >= 24 bpp PixelFormat.Format8bppIndexed - otherwise convert!

			Bitmap bmpDst = null;
			bool bStandardLoad = true;
			if (a_sFilename.IndexOf(".gif") > 0)
			{
				Image img = System.Drawing.Image.FromFile(a_sFilename);
				System.Drawing.Imaging.FrameDimension oDimension = new System.Drawing.Imaging.FrameDimension(img.FrameDimensionsList[0]);
				int nNumFrames = img.GetFrameCount(oDimension);

				//calc how big the destination bitmap must be (make it as square as possible)
				//TODO: it's OK if there's a few uninhabited tiles at end?
				int nNumFramesOnX = (int)Math.Sqrt(nNumFrames);
				for (; nNumFramesOnX > 0; nNumFramesOnX--)
				{
					if (nNumFrames/nNumFramesOnX*nNumFramesOnX == nNumFrames)
						break;
				}
				
				bmpDst = new Bitmap(
					img.Size.Width*nNumFramesOnX, img.Size.Height*nNumFrames/nNumFramesOnX,
					PixelFormat.Format24bppRgb);
				Graphics g = Graphics.FromImage(bmpDst);
				
				if (nNumFrames > 1)
				{
					bStandardLoad = false;
                    //TODO: let picRefs handle the generation animation...
					for(int i = 0; i < nNumFrames; i++)
					{
						int x = i%nNumFramesOnX;
						int y = i/nNumFramesOnX;
						img.SelectActiveFrame(oDimension, i);
                        ERectangle rctDst = new ERectangle(x*img.Size.Width, y*img.Size.Height, img.Size.Width, img.Size.Height);
                        g.DrawImageUnscaled(img, rctDst.X, rctDst.Y, rctDst.Width, rctDst.Height);
                    }
					g.Dispose();
                    this.m_sizeTotal = new EPoint(bmpDst.Width, bmpDst.Height);

                    this.m_bGotAnimation = true;
                    PicRef.CreatePicRefs(this, nNumFramesOnX, nNumFrames);
                    //AnimateWithinSize = new EPoint(img.Size.Width, img.Size.Height);
				}
				img.Dispose();
			}

			if (!bStandardLoad)
			{
			}
			else
			{
				bmpDst = (Bitmap)System.Drawing.Bitmap.FromFile(a_sFilename);
			}

			m_sizeTotal = new EPoint(bmpDst.Width, bmpDst.Height);

			m_bmpThumbnail = new Bitmap(32,32, PixelFormat.Format24bppRgb);
			Graphics g2 = Graphics.FromImage(m_bmpThumbnail);
			g2.DrawImage(bmpDst, 0,0, m_bmpThumbnail.Width, m_bmpThumbnail.Height);
			g2.Dispose();

			this.LoadResourceFile(a_sFilename);

			return bmpDst;
		}



		public EPoint RegPoint
		{
			get {return m_pntRegPoint;}
			set
			{
				m_pntRegPoint = value;
				//TODO: tell sprites to invalidate!
			}
		}

		public void CenterRegPoint()
		{
			m_pntRegPoint = new EPoint(Size.X/2, Size.Y/2);
		}

		public virtual void Fill()
		{}

		
		private void LoadResourceFile(string sPicFile)
		{
			System.IO.FileInfo finfo = new System.IO.FileInfo(sPicFile);
			string sResInfo = finfo.FullName.Replace(finfo.Extension,"")+".xml";
			//TODO: switch to XmlSerializer!

			if (!System.IO.File.Exists(sResInfo))
				return;

			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			doc.Load(sResInfo);
			System.Xml.XmlNode node = doc.FirstChild.SelectSingleNode("RegPoint");
			if (node!=null)
			{
                this.m_pntRegPoint = this.ParseRegPoint(Serialization.XmlHelper.GetValueOrInnerText(node), this.Size);
				//this.m_pntRegPoint = new EPoint(Serialization.XmlHelper.GetValueOrInnerText(node));
			}


            node = doc.FirstChild.SelectSingleNode("NumFramesTotal");
            int numFramesTotal = 0;
            if (node != null)
                numFramesTotal = Convert.ToInt32(Serialization.XmlHelper.GetValueOrInnerText(node));

            int numFramesOnX = 0;
            node = doc.FirstChild.SelectSingleNode("NumFramesOnX");
            if (node != null)
                numFramesOnX = Convert.ToInt32(Serialization.XmlHelper.GetValueOrInnerText(node)); //GetValueOrInnerText


            List<PicRef> picRefs = null;
            if (numFramesTotal > 0)
            {
                this.m_bGotAnimation = true;
                picRefs = PicRef.CreatePicRefs(sPicFile, numFramesOnX, numFramesTotal);
            }
            else
            {
                picRefs = new List<PicRef>();
                PicRef pr = PicRef.Create((MemberSpriteBitmap)this, this.Name);
                pr.SourceRectangle = new ERectangle(0, 0, this.Size.X, this.Size.Y);
                picRefs.Add(pr);
            }

            node = doc.FirstChild.SelectSingleNode("RegPoint");
            if (node != null)
            {
                if (picRefs != null)
                {
                    EPoint ptReg = this.ParseRegPoint(Serialization.XmlHelper.GetValueOrInnerText(node), picRefs[0].SourceRectangle.Size);
                    foreach (PicRef pr in picRefs)
                        pr.Offset = ptReg;
                }
            }

			node = doc.FirstChild.SelectSingleNode("Animations");
            //TODO: add to animations library
            //if (node!=null)
            //    this._animations = Endogine.Animation.AnimationHelpers.ParseAnimations(node);
			//TODO: optionally load animations from other files.
		}

        private EPoint ParseRegPoint(string sRegPoint, EPoint size)
        {
            if (sRegPoint == "center")
                return size / 2;
            else
                return new EPoint(sRegPoint);
        }
        //private void CreatePicRefs(string fileName, int numFrames, int numOnX, EPoint totalSize)
        //{
        //    this.m_bGotAnimation = true;
        //    PicRef.CreatePicRefs(fileName, numFrames, numOnX);
            
            //if (fileName.IndexOf("\\") > 0)
            //    fileName = fileName.Remove(0, fileName.LastIndexOf("\\"));

            //List<string> animRefs = new List<string>();

            //int numOnY = numFrames/numOnX;
            //EPoint frameSize = new EPoint(totalSize.X / numOnX, totalSize.Y / numOnY);
            //for (int i = 0; i < numFrames; i++)
            //{
            //    string picRefName = fileName + i;
            //    int x = i % numOnX;
            //    int y = i / numOnX;
            //    PicRef picture = new PicRef(picRefName, (MemberSpriteBitmap)this);
            //    picture.SourceRectangle = new ERectangle(x * frameSize.X, y * frameSize.Y, frameSize.X, frameSize.Y);
            //    picture.Offset = new EPoint();
            //    //EH.Instance.CastLib.Pictures.AddPicture(picture);
            //    animRefs.Add(picRefName);
            //}
            //EH.Instance.CastLib.FrameSets.AddFrameSet(fileName, animRefs);
        //}
        //public bool AutoAnimate
        //{
        //    get { return m_bAutoAnimate; }
        //    set { m_bAutoAnimate = value; }
        //}
        public bool GotAnimation
        { get { return m_bGotAnimation; } }


		public Color ColorKey
		{
			get
			{
				if (m_bGotOwnClrKey) return m_clrKey;
				return m_clrDefaultClrKey;
			}
			set {m_clrKey = value; m_bGotOwnClrKey = true;}
		}

		public EPoint GetUpperPowerTextureSize(EPoint a_size)
		{
			EPoint sizeReturn = new EPoint();
			SortedList aAllowedSizes = new SortedList();
			int n = 4;
			for (int i = 0; i < 10; i++)
			{
				aAllowedSizes.Add(n,i);
				n*=2;
			}

			int nVal = a_size.X;
			for (int i = 0; i < 2; i++)
			{
				aAllowedSizes.Add(nVal, -1);
				int nIndex = aAllowedSizes.IndexOfValue(-1);
				aAllowedSizes.RemoveAt(nIndex);
				if (nIndex > aAllowedSizes.Count)
					nIndex--;
				nVal = Convert.ToInt32(aAllowedSizes.GetKey(nIndex));
				if (i == 0)
					sizeReturn.X = nVal;
				else
					sizeReturn.Y = nVal;

				nVal = a_size.Y;
			}
			return sizeReturn;
		}

		/// <summary>
		/// If the loaded bitmap had alpha (i.e. colorKeyed alpha doesn't count)
		/// </summary>
		public bool GotAlpha
		{
			get {return m_bGotAlpha;}
			set {m_bGotAlpha = value;}
		}
	}
}
