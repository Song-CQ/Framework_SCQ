using System;

namespace Endogine.Serialization.Photoshop
{
	/// <summary>
	/// Summary description for ImageResource.
	/// </summary>
	public class ImageResource
	{
		public ushort ID;
		public string Name;
		public byte[] Data;

		public ImageResource()
		{
		}

		public ImageResource(ImageResource imgRes)
		{
			this.ID = imgRes.ID;
			this.Name = imgRes.Name;
		}

		public ImageResource(BinaryReverseReader reader)
		{
			this.ID = reader.ReadUInt16();
			this.Name = reader.ReadPascalString();
			uint settingLength = reader.ReadUInt32();
			this.Data = reader.ReadBytes((int)settingLength);
			if (reader.BaseStream.Position % 2 == 1)
				reader.ReadByte();
		}

		public BinaryReverseReader GetDataReader()
		{
			System.IO.MemoryStream stream = new System.IO.MemoryStream(this.Data);
			return new BinaryReverseReader(stream);
		}
	}
}
