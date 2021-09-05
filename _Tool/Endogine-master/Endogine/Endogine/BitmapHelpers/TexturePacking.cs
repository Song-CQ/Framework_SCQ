using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Endogine.BitmapHelpers
{

	class PackNode
	{
		//Adapted from Jim Scotts' http://www.blackpawn.com/texts/lightmaps/default.html
		PackNode[] _children;
		public ERectangle _rct;
		public int _imgId = -1;

		public PackNode()
		{
		}
		public void GetUnusedLeaves(ArrayList leaves)
		{
			if (this.IsLeaf)
			{
				if (this._imgId==-1)
					leaves.Add(this);
			}
			else
			{
				_children[0].GetUnusedLeaves(leaves);
				_children[1].GetUnusedLeaves(leaves);
			}
		}
		public PackNode Insert(Image img)
		{
			if (!this.IsLeaf) //we're not a leaf so try inserting into a child
			{
				PackNode newNode = _children[0].Insert(img);
				if (newNode != null)
					return newNode;
				// no room in first, insert into second
				return _children[1].Insert(img);
			}
			//if there's already a lightmap here, return
			if (_imgId >= 0)
				return null;
			
			//if we're too small, return
			if (_rct.Width+1 < img.Size.Width || _rct.Height+1 < img.Size.Height)
				return null;

			//if we're just right, accept
			if (_rct.Width+1 == img.Size.Width && _rct.Height+1 == img.Size.Height)
				return this;

			//otherwise, gotta split this node and create some kids
			_children = new PackNode[2];
			_children[0] = new PackNode();
			_children[1] = new PackNode();
			
			//decide which way to split
			int dw = this._rct.Width - img.Width;
			int dh = this._rct.Height - img.Height;
			
			if (dw > dh)
			{
				_children[0]._rct = ERectangle.FromLTRB(_rct.Left, _rct.Top, _rct.Left+img.Width-1, _rct.Bottom);
				_children[1]._rct = ERectangle.FromLTRB(_rct.Left+img.Width, _rct.Top, _rct.Right, _rct.Bottom);
			}
			else
			{
				_children[0]._rct = ERectangle.FromLTRB(_rct.Left, _rct.Top, _rct.Right, _rct.Top+img.Height-1);
				_children[1]._rct = ERectangle.FromLTRB(_rct.Left, _rct.Top+img.Height, _rct.Right, _rct.Bottom);
			}
			
			//insert into first child we created
			return this._children[0].Insert(img);
		}

		public bool IsLeaf
		{
			get
			{
				return this._children==null || (this._children[0] == null && this._children[1] == null);
			}
		}
	}
	/// <summary>
	/// Summary description for TexturePacking.
	/// </summary>
	public class TexturePacking
	{
		public TexturePacking()
		{
		}

		public static Image[] SortImagesBySize(Image[] images)
		{
			SortedList sl = new SortedList();
			//start by sorting by area:
			for (int i=0; i<images.Length; i++)
			{
				Image img = images[i];
				sl.Add(img.Width*img.Height*1000 + i, img);
			}
			Image[] ret = new Image[images.Length];
			for (int i=0; i<images.Length; i++)
				ret[i] = (Image)sl.GetByIndex(i); //images[i];
			
			return ret;
		}

		public static int GetTotalArea(Image[] images)
		{
			int nTotalArea = 0;
			for (int i=0; i<images.Length; i++)
				nTotalArea+=images[i].Width*images[i].Height;
			return nTotalArea;
		}

		public static void GetImagesStatistics(Image[] images, out int totalArea, out Size maxWidthAndHeight, out Size minWidthAndHeight)
		{
			totalArea = 0;
			maxWidthAndHeight = new Size(0,0);
			minWidthAndHeight = new Size(999999,999999);
			for (int i=0; i<images.Length; i++)
			{
				Image img = images[i];
				totalArea+=img.Width*img.Height;
				maxWidthAndHeight.Width = img.Width>maxWidthAndHeight.Width?img.Width:maxWidthAndHeight.Width;
				maxWidthAndHeight.Height = img.Height>maxWidthAndHeight.Height?img.Height:maxWidthAndHeight.Height;

				minWidthAndHeight.Width = img.Width<minWidthAndHeight.Width?img.Width:minWidthAndHeight.Width;
				minWidthAndHeight.Height = img.Height<minWidthAndHeight.Height?img.Height:minWidthAndHeight.Height;
			}
		}

		public static Bitmap TreePack(Size textureSize, Image[] images, out Image[] unpackedImages, out EPoint[] locs)
		{
			Bitmap bmpTexture = new Bitmap(textureSize.Width,textureSize.Height, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bmpTexture);

			PackNode root = new PackNode();
			root._rct = new ERectangle(0,0,textureSize.Width,textureSize.Height);

			locs = new EPoint[images.Length];

			ArrayList unhandled = new ArrayList();
			for (int i=images.Length-1; i>=0;i--)
			{
				Image img = images[i];

				PackNode node = root.Insert(img);
				if (node == null)
				{
					unhandled.Add(img);
					continue;
				}
				locs[i] = new EPoint(node._rct.X, node._rct.Y);
				node._imgId = i;
				g.DrawImageUnscaled(img, node._rct.Left, node._rct.Top);
			}
			if (true)
			{
				ArrayList leaves = new ArrayList();
				root.GetUnusedLeaves(leaves);
				Random rnd = new Random();
				foreach (PackNode leaf in leaves)
				{
					g.FillRectangle(new SolidBrush(Color.FromArgb(rnd.Next(254), rnd.Next(254), rnd.Next(254))), leaf._rct.ToRectangleF());
				}
			}

			unpackedImages = new Image[unhandled.Count];
			for (int i=0; i<unpackedImages.Length; i++)
				unpackedImages[i] = (Image)unhandled[i];

			return bmpTexture;
		}

		public static void TreePack(Size textureSize, string findFiles, string outputFilename)
		{
			System.IO.FileInfo[] files = Endogine.Files.FileFinder.GetFiles(findFiles);
			if (files.Length == 0)
				throw new Exception("No files found for '"+findFiles+"'");

			string[] filenames = Endogine.Files.FileFinder.GetNamesFromFiles(files);
			TreePack(textureSize, filenames, outputFilename);
		}

		public static System.Xml.XmlDocument CreateDocFromRectsAndOffsets(ERectangle[] rects, EPoint[] offsets, string[] optionalNames)
		{
			Node infoNode = new Node();
			infoNode = infoNode.CreateChild("Files");

			for (int i = 0; i<rects.Length; i++)
			{
				Node frameNode = infoNode.CreateChild("Bitmap");
				if (optionalNames!=null)
					frameNode.Value = optionalNames[i];
				else
					frameNode.Value = i.ToString();
				
				Node subNode = frameNode.CreateChild("Rect");
				subNode.Value = rects[i].ToString();
				subNode = frameNode.CreateChild("Offset");
				subNode.Value = offsets[i].ToString();
			}
			return infoNode.RootNode.CreateXmlDocument();
		}

		public static Bitmap TreePack(Image[] images, out ERectangle[] rects, out EPoint[] offsets)
		{
			//EPoint[] offsetsOrg = new EPoint[images.Length];
			offsets = new EPoint[images.Length];
			ArrayList originalOrder = new ArrayList();
			for (int i=0; i<images.Length; i++)
			{
				EPoint pntMid = new EPoint(images[i].Width, images[i].Height)/2;
				EPoint pnt = new EPoint();
				images[i] = Endogine.BitmapHelpers.BitmapHelper.TrimWhitespace((Bitmap)images[i], out pnt);
				offsets[i] = pntMid - pnt + new EPoint(1,1);

				originalOrder.Add(images[i]);
			}

			int numPixelsMinimum;
			Size maxWnH, minWnH;
			GetImagesStatistics(images, out numPixelsMinimum, out maxWnH, out minWnH);

			int side = (int)Math.Sqrt(numPixelsMinimum);
			int exponent = (int)Math.Ceiling(Math.Log(side, 2));
			side = (int)Math.Pow(2, exponent);
			Size textureSize = new Size(side,side);
			if (side*side/2 > numPixelsMinimum)
				textureSize.Height/=2;

			images = Endogine.BitmapHelpers.TexturePacking.SortImagesBySize(images);

			while (true)
			{
				EPoint[] locsInTexture = null;
				Image[] unpacked = null;
				Bitmap bmp = Endogine.BitmapHelpers.TexturePacking.TreePack(textureSize, images, out unpacked, out locsInTexture);
				if (unpacked.Length == 0)
				{
					//offsets = new EPoint[images.Length];
					rects = new ERectangle[images.Length];
					for (int i=0; i<images.Length; i++)
					{
						Image img = images[i];
						int orgIndex = originalOrder.IndexOf(img);
						rects[orgIndex] = new ERectangle(locsInTexture[i], new EPoint(img.Width, img.Height));
						//offsets[orgIndex] = offsetsOrg[orgIndex]; //don't need it, correct order to begin with...
					}
					return bmp;
				}

				if (textureSize.Width > textureSize.Height)
					textureSize.Height*=2;
				else
					textureSize.Width*=2;
			}
			
		}

		public static void TreePack(Size textureSize, string[] filenames, string outputFilename)
		{
			System.Drawing.Image[] images = new Image[filenames.Length];
//			for (int i=0; i<filenames.Length; i++)
//			{
//				files[i] = new System.IO.FileInfo(filenames[i]);
//				images[i] = System.Drawing.Bitmap.FromFile(filenames[i]);
//			}
//			ERectangle[] rects;
//			EPoint[] offsets;
//			Bitmap bmp = TreePack(images, out rects, out offsets);
//			bmp.Save(outputFilename);

			EPoint[] offsets = new EPoint[filenames.Length];
			System.IO.FileInfo[] files = new System.IO.FileInfo[filenames.Length];
			PropList originalOrder = new PropList();
			for (int i=0; i<filenames.Length; i++)
			{
				files[i] = new System.IO.FileInfo(filenames[i]);
				images[i] = System.Drawing.Bitmap.FromFile(filenames[i]);

				EPoint pntMid = new EPoint(images[i].Width, images[i].Height)/2;
				EPoint pnt = new EPoint();
				images[i] = Endogine.BitmapHelpers.BitmapHelper.TrimWhitespace((Bitmap)images[i], out pnt);
				offsets[i] = pntMid - pnt + new EPoint(1,1);

				originalOrder.Add(filenames[i], images[i]);
			}

			Image[] unpacked = null;
			
			int numPixelsMinimum;
			Size maxWnH, minWnH;
			GetImagesStatistics(images, out numPixelsMinimum, out maxWnH, out minWnH);

			if (textureSize.Width == 0 || textureSize.Height == 0)
			{
				int side = (int)Math.Sqrt(numPixelsMinimum);
				int asdas = (int)Math.Ceiling(Math.Log(side, 2));
				textureSize = new Size(side,side);
			}

			if (numPixelsMinimum >= textureSize.Width*textureSize.Height)
				throw new Exception("Not enough space to pack in");

			images = Endogine.BitmapHelpers.TexturePacking.SortImagesBySize(images);


			Random rnd = new Random();

			int nNumTries = 1;
			for (int i=0; i<nNumTries; i++)
			{
				//Irritating, but since we can't find index of an item in a standard array...:
				ArrayList aImagesNewOrder = new ArrayList();
				foreach (Image image in images)
					aImagesNewOrder.Add(image);

				EPoint[] locsInTexture = null;
				Bitmap bmp = Endogine.BitmapHelpers.TexturePacking.TreePack(textureSize, images, out unpacked, out locsInTexture);
				int areaExcluded = Endogine.BitmapHelpers.TexturePacking.GetTotalArea(unpacked);

				if (unpacked.Length > 0)
					throw new Exception("Couldn't fit all images. Total pixels excluded = "+areaExcluded.ToString() + " (side = "+((int)Math.Sqrt(areaExcluded)).ToString()+")");

				ArrayList aUnpacked = new ArrayList();
				foreach (Image image in unpacked)
					aUnpacked.Add(image);
				int areaUnused = 0;
				foreach (Image image in images)
				{
					if (!aUnpacked.Contains(image))
						areaUnused+=image.Size.Width*image.Size.Height;
				}

				if (areaUnused > textureSize.Width*textureSize.Height/2)
				{
					//TODO: we could maybe make the texture at least half as big
				}

				Node infoNode = new Node();
				infoNode = infoNode.CreateChild("Files");

				//TODO: go by the list of images that was actually used!
				for (int orgIndex = 0; orgIndex<originalOrder.Count; orgIndex++)
				{
					Image image = (Image)originalOrder.GetByIndex(orgIndex);
					if (aUnpacked.Contains(image))
						continue;

					int newIndex = aImagesNewOrder.IndexOf(image);

					//create the file entry (File, Rect, Offset)
					ERectangle rctInTexture = new ERectangle(
						locsInTexture[newIndex].X, locsInTexture[newIndex].Y, image.Width, image.Height);
					Node frameNode = infoNode.CreateChild("File");
					//remove extension from filename:
					frameNode.Value = files[orgIndex].Name.Substring(0,files[orgIndex].Name.LastIndexOf(files[orgIndex].Extension));
					//frameNode.Value = files[orgIndex].Name;
					Node subNode = frameNode.CreateChild("Rect");
					subNode.Value = rctInTexture.ToString();
					subNode = frameNode.CreateChild("Offset");
					subNode.Value = offsets[orgIndex].ToString();
				}

				System.Xml.XmlDocument doc = infoNode.RootNode.CreateXmlDocument();

				string sOut = null;
				if (nNumTries > 1)
					sOut = outputFilename+areaExcluded.ToString()+"-"+i.ToString();
				else
					sOut = outputFilename;

				doc.Save(sOut+".xml");
				bmp.Save(sOut+".png");
				
				//randomize order to see if we get better results:
				ArrayList aImages = new ArrayList();
				for (int j=0; j<images.Length;j++)
					aImages.Add(images[j]);
				
				Image[] randomOrder = new Image[images.Length];
				for (int j=images.Length-1;j>=0;j--)
				{
					int pos = 0;
					if (j > 0)
						pos = Math.Min(rnd.Next(j), j-1);
					randomOrder[j] = (Image)aImages[pos];
					aImages.RemoveAt(pos);
				}
				images = randomOrder;
			}
		}



		public static void PackBitmapFiles(string[] files, string outputFile)
		{
			ArrayList bmps = new ArrayList();
			foreach (string file in files)
			{
				Bitmap bmp = (Bitmap)Bitmap.FromFile(file);
				bmps.Add(bmp);
			}
			Node infoNode;
			Bitmap large = PackBitmapsIntoOneLarge(bmps, null, out infoNode);
			//string sFormat = outputFile.Remove(0,outputFile.LastIndexOf(".")+1).ToUpper();
			large.Save(outputFile); //, GetEncoderInfo(sFormat), null);

			int nIndex = outputFile.LastIndexOf(".");
			outputFile = outputFile.Substring(0,nIndex)+".xml";

			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			infoNode.AddToXml(doc);
			doc.Save(outputFile);
		}
		public static void PackBitmapFiles(string sSearchPath, string outputFile)
		{
			int nIndex = sSearchPath.LastIndexOf("\\");
			string[] files = System.IO.Directory.GetFiles(sSearchPath.Substring(0,nIndex), sSearchPath.Remove(0,nIndex+1));
			PackBitmapFiles(files, outputFile);
		}

		public static Bitmap PackBitmapsIntoOneLarge(ArrayList bmps, EPoint pntPreferredLayout, out Node infoRoot)
		{
			//If no specification of number of tiles on X and Y, make a guess:
			if (pntPreferredLayout == null)
			{
				int nNumOnX = (int)Math.Sqrt(bmps.Count);
				if (nNumOnX*nNumOnX < bmps.Count)
					nNumOnX++;
				int nNumOnY = bmps.Count/nNumOnX;
				if (nNumOnX*nNumOnY < bmps.Count)
					nNumOnY++;

				pntPreferredLayout = new EPoint(nNumOnX, nNumOnY);
			}

			bool bTrimWhiteSpace = true;
			
			//when packing the bitmap tightly, must store the offsets of bitmap frames so they don't wiggle on playback:
			bool bUseIndividualOffsets = true; 

			infoRoot = new Node();
			Node node = infoRoot.AppendChild("root");
			node.AppendChild("NumFramesTotal").Value = bmps.Count;
			node.AppendChild("NumFramesOnX").Value = pntPreferredLayout.X;

			Node subNode;
			subNode = node.AppendChild("Animations");
			//subNode.AppendChild("Default").Value = "0 0-4";

			EPoint[] offsets = new EPoint[bmps.Count];

			//this is the smallest rectangle that can encompass all frames:
			ERectangle rctBounds = ERectangle.FromLTRB(9999,9999,-9999,-9999);

			//Trim white space from all bitmaps
			subNode = node.AppendChild("Frames");
			for (int i = 0; i < bmps.Count; i++)
			{
				Bitmap bmp = (Bitmap)bmps[i];

				if (bTrimWhiteSpace)
				{
					EPoint pntMid = new EPoint(bmp.Size.Width, bmp.Size.Height)/2;
					EPoint pntTopLeftCorner;
					bmp = BitmapHelper.TrimWhitespace(bmp, out pntTopLeftCorner);
					bmps[i] = bmp;

					offsets[i] = pntTopLeftCorner;

					if (bUseIndividualOffsets) //make more compact (but offset values are needed):
					{
						rctBounds.Expand(new ERectangle(0,0,bmp.Width, bmp.Height));
						//make offset to middle of input bitmap
						offsets[i] = pntMid - pntTopLeftCorner;
					}
					else //Expand bounds so no offset values are needed:
						rctBounds.Expand(new ERectangle(pntTopLeftCorner.X, pntTopLeftCorner.Y, bmp.Width, bmp.Height));
				}
				else
				{
					rctBounds.Expand(new ERectangle(0, 0, bmp.Width, bmp.Height));
				}
			}

			//Create the merged bitmap:
			EPoint totalSize = rctBounds.Size*pntPreferredLayout;
			Bitmap largeBmp = new Bitmap(totalSize.X, totalSize.Y);
			Graphics g = Graphics.FromImage(largeBmp);
			for (int i = 0; i < bmps.Count; i++)
			{
				Bitmap bmp = (Bitmap)bmps[i];
				EPoint pntDst = new EPoint((i%pntPreferredLayout.X)*rctBounds.Size.X, (i/pntPreferredLayout.X)*rctBounds.Size.Y);

				Node subsub = subNode.AppendChild("Frame");
				subsub.Value = i.ToString();

				if (bUseIndividualOffsets)
				{
					//subsub.AppendChild("Offset").Value = (offsets[i]+rctBounds.TopLeft).ToString();
					subsub.AppendChild("Offset").Value = offsets[i].ToString();
				}
				else
				{
					//pntDst is upper left corner of destination rectangle.
					//Since we don't use individual offset, we want to move it according to offset:
					pntDst = pntDst-rctBounds.TopLeft+offsets[i];
				}
				g.DrawImage(bmp, new RectangleF(pntDst.ToPoint(), new Size(bmp.Width,bmp.Height)));
			}

			if (bUseIndividualOffsets)
				node.AppendChild("RegPoint").Value = new EPoint().ToString();
			else
				node.AppendChild("RegPoint").Value = rctBounds.TopLeft.ToString();

			return largeBmp;
		}

	}
}
