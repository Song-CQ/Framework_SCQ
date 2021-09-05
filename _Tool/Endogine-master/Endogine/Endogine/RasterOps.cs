using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

using Endogine.ResourceManagement;

namespace Endogine
{
	/// <summary>
	/// Summary description for RasterOps.
	/// </summary>
	public class RasterOps
	{
		private static SortedList m_slPreCalced = null;
		public enum ROPs
		{
			Copy = 0,
			AddPin = 33,
			Add = 34,
			SubtractPin = 35,
			BgTransparent = 36,
			Lightest = 37,
			Subtract = 38,
			Darkest = 39,
			Multiply = 101,
			Screen = 103,
			Overlay = 106,
			ColorDodge = 107,
			ColorBurn = 108,
			Difference = 109,
			IgnoreAlpha = 110,
			D3DTest1 = 200,
			D3DTest2 = 201,
			D3DTest3 = 202,
			D3DTest4 = 203
		}

		public RasterOps()
		{
			m_slPreCalced = new SortedList();
		}

		public void PreCalcBlendMode(ROPs a_nMode)
		{
			PreCalcBlendMode((int)a_nMode);
		}

		/// <summary>
		/// Precalcing raster operations (by creating look-up tables) can speed up some ops significantly
		/// However, each table takes 256*256 bytes = 65kB, so avoid doing it too much!
		/// </summary>
		/// <param name="a_nMode"></param>
		public void PreCalcBlendMode(int a_nMode)
		{
			byte[,] aVals = new byte[256,256];
			for (int nSrc = 0; nSrc < 256; nSrc++)
			{
				for (int nDst = 0; nDst < 256; nDst++)
				{
					switch (a_nMode)
					{
						case 33: //Add pin
							aVals[nDst,nSrc] = (byte)Math.Min(nDst+nSrc, 255);
							break;
						case 34: //Add
							aVals[nDst,nSrc] = (byte)(nDst+nSrc);
							break;
						case 35: //Subtract pin
							aVals[nDst,nSrc] = (byte)Math.Max(nDst-nSrc, 0);
							break;
						case 37: //Lightest
							aVals[nDst,nSrc] = (byte)((nSrc>nDst)?nSrc:nDst);
							break;
						case 38: //Subtract
							aVals[nDst,nSrc] = (byte)(nDst-nSrc);
							break;
						case 39: //Darkest
							aVals[nDst,nSrc] = (byte)((nSrc<nDst)?nSrc:nDst);
							break;
						default:
							aVals[nDst,nSrc] = (byte)nSrc;
							break;

							//http://www.pegtop.net/delphi/blendmodes/
							//Photoshop-like ops:
						case 101: //multiply
							aVals[nDst,nSrc] = (byte)(nSrc*nDst / 255);
							break;

						case 103://screen
							aVals[nDst,nSrc] = (byte)(255- ((255-nSrc)*(255-nDst) >> 8));
							//pPDst[ulDP] = 255 - (int(255-pPSrc1[ulS1P]) * (255-pPSrc2[ulS2P]) >> 8);
							break;

						case 106: //Overlay
							if (nSrc<128) aVals[nDst,nSrc] = (byte)(nSrc*nSrc >> 7);
							else aVals[nDst,nSrc] = (byte)(255 - ((255-nSrc) * (255-nDst) >> 7));
							break;

						case 107: //Color dodge
							if (nDst==255) aVals[nDst,nSrc] = 255;
							else 
							{
								int nTempVal = (nSrc<<8) / (256 - nDst);
								if (nTempVal > 255) aVals[nDst,nSrc] = 255;
								else aVals[nDst,nSrc] = (byte)nTempVal; }
							break;

						case 108: //Color burn
							if (nDst==0) aVals[nDst,nSrc] = 0;
							else 
							{
								int nTempVal = (int)255 - (((255-nSrc) << 8) / nDst);
								if (nTempVal < 0) aVals[nDst,nSrc] = 0;
								else aVals[nDst,nSrc] = (byte)nTempVal; }
							break;

						case 109: //Difference
							aVals[nDst,nSrc] = (byte)Math.Abs(nDst - nSrc);
							break;
					}
				}
			}
			m_slPreCalced.Add(a_nMode, aVals);
		}

		/// <summary>
		/// Clips the rects so that the destination's bitmap's clipping boundaries aren't exceeded.
		/// If the destination rect needs to be clipped, the source rect must be clipped accordingly.
		/// </summary>
		/// <param name="a_rctSrc">The rect to blit</param>
		/// <param name="a_rctDst">The requested destination rect</param>
		/// <param name="a_rctDstClip">The destination clipping rect</param>
		/// <param name="a_rctNewSrc">The resulting source rect</param>
		/// <param name="a_rctNewDst">The resulting destination rect</param>
		static private void CalcClippedRects(
			ERectangle a_rctSrc, ERectangleF a_rctDst, ERectangle a_rctDstClip,
			out ERectangle a_rctNewSrc, out ERectangle a_rctNewDst)
		{
			ERectangleF rctNewDstTmp = a_rctDst.Copy();
			//a_rctDstClip - the srcClip of the dest sprite

			ERectangleF rctDstClipTmp = new ERectangleF((float)a_rctDstClip.X, (float)a_rctDstClip.Y, (float)a_rctDstClip.Width, (float)a_rctDstClip.Height);
			rctNewDstTmp.Intersect(rctDstClipTmp);
			a_rctNewDst = new ERectangle((int)rctNewDstTmp.X, (int)rctNewDstTmp.Y, (int)rctNewDstTmp.Width, (int)rctNewDstTmp.Height);
			//and if so, the src clip must be adjusted accordingly:
			ERectangleF rctDiff = new ERectangleF(
				rctNewDstTmp.Left-a_rctDst.Left,
				rctNewDstTmp.Top-a_rctDst.Top,
				rctNewDstTmp.Width-a_rctDst.Width,
				rctNewDstTmp.Height-a_rctDst.Height);

			PointF pntScale = new PointF(a_rctSrc.Width/a_rctDst.Width, a_rctSrc.Height/a_rctDst.Height);
			ERectangle rctTmpSrcClip = a_rctSrc.Copy();
			rctTmpSrcClip.X = a_rctSrc.X+(int)(pntScale.X*rctDiff.X);
			rctTmpSrcClip.Width = a_rctSrc.Width+(int)(pntScale.X*rctDiff.Width);
			rctTmpSrcClip.Y = a_rctSrc.Y+(int)(pntScale.Y*rctDiff.Y);
			rctTmpSrcClip.Height = a_rctSrc.Height+(int)(pntScale.Y*rctDiff.Height);

			if (rctTmpSrcClip.X < 0)
				rctTmpSrcClip.X = 0;

			a_rctNewSrc = rctTmpSrcClip;
		}

		static public void CopyPixels(Bitmap a_bmpDst, Bitmap a_bmpSrc, int a_nInk, int a_nBlend)
		{
			CopyPixels(a_bmpDst, a_bmpSrc,
				new ERectangleF(0,0, a_bmpDst.Width,a_bmpDst.Height),
				new ERectangle(0,0, a_bmpSrc.Width,a_bmpSrc.Height),
				new ERectangle(0,0, a_bmpDst.Width,a_bmpDst.Height),
				a_nInk, a_nBlend);
		}

		static public void CopyPixels(Bitmap a_bmpDst, Bitmap a_bmpSrc,
			ERectangleF a_rctDst, ERectangle a_rctSrc, ERectangle a_rctDstClip, int a_nInk, int a_nBlend)
		{
			ERectangle rctNewSrc, rctNewDst;

			CalcClippedRects(a_rctSrc, a_rctDst, a_rctDstClip, out rctNewSrc, out rctNewDst);
			if (rctNewDst.Width <= 0 || rctNewDst.Height <= 0 || rctNewSrc.Width <= 0 || rctNewSrc.Height <= 0)
				return;

            BitmapData bmpdtDst = null, bmpdtSrc = null;
			try
			{
				bmpdtDst = a_bmpDst.LockBits(new Rectangle(0,0,a_bmpDst.Width,a_bmpDst.Height), //rctNewDst,
					System.Drawing.Imaging.ImageLockMode.ReadWrite,
					a_bmpDst.PixelFormat);
				bmpdtSrc = a_bmpSrc.LockBits(new Rectangle(0,0,a_bmpSrc.Width,a_bmpSrc.Height), //rctNewSrc,
					System.Drawing.Imaging.ImageLockMode.ReadOnly,
					a_bmpSrc.PixelFormat);
			}
			catch
			{
				if (bmpdtDst != null)
					a_bmpDst.UnlockBits(bmpdtDst);
				if (bmpdtSrc != null)
					a_bmpSrc.UnlockBits(bmpdtSrc);

				return;
			}

			int nBytesPerPixelDst = (a_bmpDst.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)?3:
				((a_bmpDst.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)?1:4);
			int nBytesPerPixelSrc = (a_bmpSrc.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)?3:
				((a_bmpSrc.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)?1:4);
//				((a_bmpSrc.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)?8:32);
			unsafe
			{
				BlitWithOps((byte*)bmpdtSrc.Scan0, (byte*)bmpdtDst.Scan0,
					rctNewSrc, rctNewDst, bmpdtSrc.Stride, bmpdtDst.Stride,
					nBytesPerPixelSrc, nBytesPerPixelDst, a_nInk, a_nBlend);
			}

			a_bmpDst.UnlockBits(bmpdtDst);
			a_bmpSrc.UnlockBits(bmpdtDst);
		}


		unsafe static private void BlitWithOps(byte* a_ptrSrc, byte* a_ptrDst,
			ERectangle a_rctSrc, ERectangle a_rctDst,
			int a_nStrideSrc, int a_nStrideDst,
			int a_nBppSrc, int a_nBppDst,
			int a_nInk, int a_nBlend)
		{
			int a_nBppSrc2 = a_nBppDst;
			//http://www.codeproject.com/cs/media/KVImageProcess.asp

			double dDstSizeDiffX = (double)a_rctDst.Width/a_rctSrc.Width;
			int nStepDiff;
			if (dDstSizeDiffX < 1)
				nStepDiff = (int)(-1.0/dDstSizeDiffX)+1;
			else
				nStepDiff = (int)dDstSizeDiffX-1;
			nStepDiff*=a_nBppDst;

			byte[,] aVals = null;
            if (m_slPreCalced!=null && m_slPreCalced.Contains(a_nInk))
				aVals = (byte[,])m_slPreCalced[a_nInk];

			unsafe
			{
				

				for (int nLineNum = 0; nLineNum < a_rctSrc.Height; nLineNum++)
				{
					//int nLineOffsetDst = ((int)(dDstSizeDiffX*nLineNum)+(int)a_rctDst.Top)*a_nStrideDst;
					int nLineOffsetDst = (nLineNum+(int)a_rctDst.Top)*a_nStrideDst;
					int nLineOffsetSrc = (nLineNum+(int)a_rctSrc.Top)*a_nStrideSrc;

					byte* pDstPix = a_ptrDst+nLineOffsetDst+(int)a_rctDst.Left*a_nBppDst;
					byte* pSrc1Pix = a_ptrSrc+nLineOffsetSrc+(int)a_rctSrc.Left*a_nBppSrc;
					byte* pSrc2Pix = a_ptrDst+nLineOffsetDst+(int)a_rctDst.Left*a_nBppSrc2;
					//EndogineHub.Put(a_rctSrc.ToString());
					//per-pixel operations

					if (aVals!=null) //used precalced data if it exists
					{
						for (int x = a_rctSrc.Left; x < a_rctSrc.Right; x++)
						{
							for (int n = 0; n < a_nBppSrc; n++)
							{
								*pDstPix = aVals[*pSrc2Pix, *pSrc1Pix];
								pDstPix++;
								pSrc1Pix++;
								pSrc2Pix++;
							}
							pDstPix+=a_nBppDst-a_nBppSrc; //+ nStepDiff;
							pSrc2Pix+=a_nBppSrc2-a_nBppSrc; //+ nStepDiff;
						}
						continue;
					}
					//per-bit operations
					for (int x = a_rctSrc.Left; x < a_rctSrc.Right; x++)
					{
						for (int n = 0; n < a_nBppSrc; n++)
						{
							//byte btDstBefore = *pDstPix;
							switch (a_nInk)
							{
									/*0   COPY            d = s
											1   TRANS           d = d AND s
											2   REVERSE         d = d XOR s
											3   GHOST           d = d OR (NOT s)
											4   NOT_COPY        d = NOT s
											5   NOT_TRANS       d = d AND (NOT s)
											6   NOT_REVERSE     d = d XOR (NOT s)
											7   NOT_GHOST       d = d OR s
											32  BLEND           d = d * blend + s * (100% - blend)
											36  BKGND_TRANS     d = s if s != bg
											40  LIGHTEN         d = s * scaleFactor (bg) + addFactor (fg)
											41  DARKEN          d = s * scaleFactor (bg) + addFactor ((256 - bg) + fg)*/
								case 0: //Copy
									//*pDstPix = 255;
									if (a_nBlend != 255)
										*pDstPix = (byte)((255-a_nBlend)*(*pSrc2Pix)/255 + a_nBlend*(*pSrc1Pix)/255);
									else
										*pDstPix = *pSrc1Pix;
									break;
								case 1: //Transparent
									*pDstPix = (byte)(*pSrc2Pix & *pSrc1Pix);
									break;
								case 2: //Reverse
									*pDstPix = (byte)(*pSrc2Pix ^ *pSrc1Pix);
									break;
								case 3: //Ghost
									*pDstPix = (byte)(*pSrc2Pix | (~*pSrc1Pix)); //or (not src)
									break;


								case 33: //Add pin
									if (a_nBlend != 255)
										*pDstPix = (byte)Math.Min((int)*pSrc2Pix+(int)a_nBlend*(*pSrc1Pix)/255, 255);
									else
										*pDstPix = (byte)Math.Min((int)*pSrc2Pix+(int)*pSrc1Pix, 255);
									break;
								case 34: //Add
									if (a_nBlend != 255)
										*pDstPix += (byte)(a_nBlend*(*pSrc1Pix)/255);
									else
										*pDstPix += *pSrc1Pix;
									break;
								case 35: //Subtract pin
									if (a_nBlend != 255)
										*pDstPix = (byte)Math.Max((int)*pSrc2Pix-(int)a_nBlend*(*pSrc1Pix)/255, 0);
									else
										*pDstPix = (byte)Math.Max((int)*pSrc2Pix-(int)*pSrc1Pix, 0);
									break;
								case 37: //Lightest
									*pDstPix = (*pSrc1Pix>*pSrc2Pix)?*pSrc1Pix:*pSrc2Pix;
									break;
								case 38: //Subtract
									if (a_nBlend != 255)
										*pDstPix -= (byte)(a_nBlend*(*pSrc1Pix)/255);
									else
										*pDstPix -= *pSrc1Pix;
									break;
								case 39: //Darkest
									*pDstPix = (*pSrc1Pix<*pSrc2Pix)?*pSrc1Pix:*pSrc2Pix;
									break;
								default:
									*pDstPix = *pSrc1Pix;
									break;


									//http://www.pegtop.net/delphi/blendmodes/
									//Photoshop-like ops:

								case 101: //multiply
									*pDstPix = (byte)((int)(*pSrc1Pix)*(*pSrc2Pix) / 255);
									break;

								case 103://screen
									*pDstPix = (byte)(255- ((int)(255-*pSrc1Pix)*(255-*pSrc2Pix) >> 8));
									//pPDst[ulDP] = 255 - (int(255-pPSrc1[ulS1P]) * (255-pPSrc2[ulS2P]) >> 8);
									break;

								case 106: //Overlay

									if (*pSrc1Pix<128) *pDstPix = (byte)((int)(*pSrc1Pix)* *pSrc1Pix >> 7);
									else *pDstPix = (byte)(255 - ((255-*pSrc1Pix) * (255-*pSrc2Pix) >> 7));
									break;

								case 107: //Color dodge
									if (*pSrc2Pix==255) *pDstPix = 255;
									else 
									{
										int nTempVal = ((int)*pSrc1Pix<<8) / (256 - *pSrc2Pix);
										if (nTempVal > 255) *pDstPix = 255;
										else *pDstPix = (byte)nTempVal; }
									break;

								case 108: //Color burn
									if (*pSrc2Pix==0) *pDstPix = 0;
									else 
									{
										int nTempVal = (int)255 - (((255-*pSrc1Pix) << 8) / *pSrc2Pix);
										if (nTempVal < 0) *pDstPix = 0;
										else *pDstPix = (byte)nTempVal; }
									break;

								case 109: //Difference
									*pDstPix = (byte)Math.Abs((int)*pSrc1Pix - *pSrc2Pix);
									break;			
							}
							pDstPix++;
							pSrc1Pix++;
							pSrc2Pix++;
						}
						pDstPix+=a_nBppDst-a_nBppSrc; //+ nStepDiff;
						pSrc2Pix+=a_nBppSrc2-a_nBppSrc; //+ nStepDiff;
						
						
							/*
							 * ulDP+=4;
		if (ulDP > cjbiiDst.ulNextLineByteNum-1) {
			cjbiiDst.ulCurrYInROI++;
			if (cjbiiDst.ulCurrYInROI > cjbiiDst.ulROIHeight-1) break;
			ulDP = cjbiiDst.lROIStartXOffsetBytes + (cjbiiDst.ulCurrYInROI+cjbiiDst.lROIStartYOffsetLines)*cjbiiDst.pmmImageInfo->iRowBytes;
			cjbiiDst.ulNextLineByteNum = ulDP + cjbiiDst.ulROIWidthNumBytes;
		}

		cjbiiSrc1.ulX++;
		ulS1P+=4;
		if (ulS1P > cjbiiSrc1.ulNextLineByteNum-1) {
			cjbiiSrc1.ulCurrYInROI++;
			if (cjbiiSrc1.ulCurrYInROI > cjbiiSrc1.ulROIHeight-1) break;
			ulS1P = cjbiiSrc1.lROIStartXOffsetBytes + (cjbiiSrc1.ulCurrYInROI+cjbiiSrc1.lROIStartYOffsetLines)*cjbiiSrc1.pmmImageInfo->iRowBytes;
			cjbiiSrc1.ulNextLineByteNum = ulS1P + cjbiiSrc1.ulROIWidthNumBytes;
			cjbiiSrc1.ulY++;
			cjbiiSrc1.ulX = cjbiiSrc1.mrctROI.left;

		}

		ulS2P+=4;
		if (ulS2P > cjbiiSrc2.ulNextLineByteNum-1) {
			cjbiiSrc2.ulCurrYInROI++;
			if (cjbiiSrc2.ulCurrYInROI > cjbiiSrc2.ulROIHeight-1) break;
			ulS2P = cjbiiSrc2.lROIStartXOffsetBytes + (cjbiiSrc2.ulCurrYInROI+cjbiiSrc2.lROIStartYOffsetLines)*cjbiiSrc2.pmmImageInfo->iRowBytes;
			cjbiiSrc2.ulNextLineByteNum = ulS2P + cjbiiSrc2.ulROIWidthNumBytes;
		}
		*/




							//TODO: per-pixel ops
							/*
							 *
							 case 114: //Displacement
			//lTemp = 4*(char)(pPSrc2[ulS2P+2]) + (long)(cjbiiSrc1.pmmImageInfo->iRowBytes)*(char)(pPSrc2[ulS2P+1]) + ulS1P;
			//TODO: use modulo to wrap more than once around image!
			//TODO: optimize when dDisplacementFact == 1.0
			lX = (long)cjbiiSrc1.ulX + (long)(dDisplacementFact*(char)(pPSrc2[ulS2P+2]));
			lY = (long)cjbiiSrc1.ulY + (long)(dDisplacementFact*(char)(pPSrc2[ulS2P+1]));
			if (lY < cjbiiSrc1.mrctROI.top) {
				lY = cjbiiSrc1.mrctROI.bottom - (cjbiiSrc1.mrctROI.top - lY);
			}
			else if (lY > cjbiiSrc1.mrctROI.bottom) {
				lY = cjbiiSrc1.mrctROI.top + (lY - cjbiiSrc1.mrctROI.bottom);
			}
			if (lX < cjbiiSrc1.mrctROI.left) {
				lX = cjbiiSrc1.mrctROI.right - (cjbiiSrc1.mrctROI.left - lX);
			}
			else if (lX > cjbiiSrc1.mrctROI.right) {
				lX = cjbiiSrc1.mrctROI.left + (lX - cjbiiSrc1.mrctROI.right);
			}
			lTemp = 4*lX + lY*(cjbiiSrc1.pmmImageInfo->iRowBytes);
			//if (lTemp < 0 || lTemp > (cjbiiDst.ulROIHeight-1)*cjbiiDst.pmmImageInfo->iRowBytes) {
			//	break;
			//}
			ulTemp = (unsigned long)lTemp;
			pPDst[ulDP+2] = pPSrc1[ulTemp+2];
			pPDst[ulDP+1] = pPSrc1[ulTemp+1];
			pPDst[ulDP+0] = pPSrc1[ulTemp+0];
			break;
			
																case 112: //Actual composite
			if (pPSrc1[ulS1P+3] + pPSrc2[ulS2P+3] == 0) {
				pPDst[ulDP+2] = 0;
				pPDst[ulDP+1] = 0;
				pPDst[ulDP+0] = 0;
			}
			else {
				ucTemp = (unsigned char)((unsigned int)pPSrc1[ulS1P+3]*(255-pPSrc2[ulS2P+3]) >> 8);
				pPDst[ulDP+3] = ucTemp + pPSrc2[ulS2P+3];
				
				pPDst[ulDP+2] = (unsigned char)((((unsigned int)ucTemp*pPSrc1[ulS1P+2] + (unsigned int)pPSrc2[ulS2P+2]*pPSrc2[ulS2P+3])/pPSrc1[ulS1P+3])); // >> 8);
				pPDst[ulDP+1] = (unsigned char)((((unsigned int)ucTemp*pPSrc1[ulS1P+1] + (unsigned int)pPSrc2[ulS2P+1]*pPSrc2[ulS2P+3])/pPSrc1[ulS1P+3])); // >> 8);
				pPDst[ulDP+0] = (unsigned char)((((unsigned int)ucTemp*pPSrc1[ulS1P+0] + (unsigned int)pPSrc2[ulS2P+0]*pPSrc2[ulS2P+3])/pPSrc1[ulS1P+3])); // >> 8);
			}

									case 111: //Hue
			convRGB2HSB(pPSrc2[ulS2P+2], pPSrc2[ulS2P+1], pPSrc2[ulS2P+0], hsbH,ucTemp,ucTemp);
			convRGB2HSB(pPSrc1[ulS1P+2], pPSrc1[ulS1P+1], pPSrc1[ulS1P+0], unTemp,hsbS,hsbB);
			convHSB2RGB(hsbH,hsbS,hsbB, pPDst[ulDP+2], pPDst[ulDP+1], pPDst[ulDP+0]);
			break;
 
																case 110: //Color
			convRGB2HSB(pPSrc2[ulS2P+2], pPSrc2[ulS2P+1], pPSrc2[ulS2P+0], hsbH,hsbS,ucTemp);
			convRGB2HSB(pPSrc1[ulS1P+2], pPSrc1[ulS1P+1], pPSrc1[ulS1P+0], unTemp,ucTemp,hsbB);
			convHSB2RGB(hsbH,hsbS,hsbB, pPDst[ulDP+2], pPDst[ulDP+1], pPDst[ulDP+0]);
			break;

							 * case 102: //HSB shift
			convRGB2HSB(pPSrc1[ulS1P+2], pPSrc1[ulS1P+1], pPSrc1[ulS1P+0], hsbH,hsbS,hsbB);
			
			nHsbH = (int)hsbH + (int)(pPSrc2[ulS2P+2]) * 360 / 255;
			if (nHsbH >= 360) nHsbH-=360;
			else if (nHsbH < 0) nHsbH+=360;
			nHsbS = (int)hsbS + (int)(char)(pPSrc2[ulS2P+1]);
			if (nHsbS > 100) nHsbS=100;
			else if (nHsbS < 0) nHsbS=0;
			nHsbB = (int)hsbB + (int)(char)(pPSrc2[ulS2P+0]);
			if (nHsbB > 100) nHsbB=100;
			else if (nHsbB < 0) nHsbB=0;
			
			convHSB2RGB((unsigned int)nHsbH,(unsigned char)nHsbS,(unsigned char)nHsbB, pPDst[ulDP+2], pPDst[ulDP+1], pPDst[ulDP+0]);
			break;*/
					}
				}
			}
		}
	}
}
