using System;

namespace Endogine.Serialization.Photoshop.ImageResources
{
	/// <summary>
	/// Summary description for CopyRightInfo.
	/// </summary>
	public class CopyrightInfo : ImageResource
	{
		public bool Copyrighted;

		public CopyrightInfo(ImageResource imgRes)
		{
			BinaryReverseReader reader = imgRes.GetDataReader();
			this.Copyrighted = reader.ReadByte()==0?false:true;
			reader.Close();			
		}
	}
}
