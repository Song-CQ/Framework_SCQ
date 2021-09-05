using System;

namespace Endogine.Serialization.Photoshop.ImageResources
{
	/// <summary>
	/// Summary description for Thumbnail.
	/// </summary>
	public class Thumbnail : ImageResource
	{
		public int		nFormat;
		public int		nWidth;
		public int		nHeight;
		public int		nWidthBytes;
		public int		nSize;
		public int		nCompressedSize;
		public short	nBitPerPixel;
		public short	nPlanes;

		public Thumbnail(ImageResource imgRes)
		{
			BinaryReverseReader reader = imgRes.GetDataReader();
			
			//m_bThumbnailFilled = true;

			this.nFormat = reader.ReadInt32();
			this.nWidth = reader.ReadInt32();
			this.nHeight = reader.ReadInt32();
			this.nWidthBytes = reader.ReadInt32();
			this.nSize = reader.ReadInt32();
			this.nCompressedSize = reader.ReadInt32();
			this.nBitPerPixel = reader.ReadInt16();
			this.nPlanes = reader.ReadInt16();
									
			int nTotalData = this.nSize - 28; // header

			byte [] buffer = new byte[nTotalData];
			if (this.ID == 1033)
			{
				// BGR
				for(int n=0; n<nTotalData; n=n+3)
				{
					buffer[n+2] = reader.ReadByte();
					buffer[n+1] = reader.ReadByte();
					buffer[n+0] = reader.ReadByte();
				}
			}
			else if (this.ID == 1036)
			{
				// RGB										
				for (int n=0; n<nTotalData; ++n)
					buffer[n] = reader.ReadByte();
			}
			reader.Close();			
		}
	}
}
