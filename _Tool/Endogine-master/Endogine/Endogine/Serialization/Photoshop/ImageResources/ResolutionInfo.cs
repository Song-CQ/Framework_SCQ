using System;

namespace Endogine.Serialization.Photoshop.ImageResources
{
	/// <summary>
	/// Summary description for ResolutionInfo.
	/// </summary>
	public class ResolutionInfo : ImageResource
	{
		public short hRes;
		public int hResUnit;
		public short widthUnit;

		public short vRes;
		public int vResUnit;
		public short heightUnit;

		public ResolutionInfo(ImageResource imgRes) : base(imgRes)
		{
			//m_bResolutionInfoFilled = true;
			BinaryReverseReader reader = imgRes.GetDataReader();

			this.hRes = reader.ReadInt16();
			this.hResUnit = reader.ReadInt32();
			this.widthUnit = reader.ReadInt16();

			this.vRes = reader.ReadInt16();
			this.vResUnit = reader.ReadInt32();
			this.heightUnit = reader.ReadInt16();

			reader.Close();

            //int ppm_x = 3780;	// 96 dpi
            //int ppm_y = 3780;	// 96 dpi

            //if (psd.ResolutionInfo != null)
            //{
            //    int nHorzResolution = (int)psd.ResolutionInfo.hRes;
            //    int nVertResolution = (int)psd.ResolutionInfo.vRes;

            //    ppm_x = (nHorzResolution * 10000) / 254;
            //    ppm_y = (nVertResolution * 10000) / 254;
            //}

		}
	}
}
