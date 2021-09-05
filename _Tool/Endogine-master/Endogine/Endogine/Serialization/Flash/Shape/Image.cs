using System;
using System.Drawing;
using System.IO;

namespace Endogine.Serialization.Flash.Shape
{
	/// <summary>
	/// Summary description for Image.
	/// </summary>
	public class Image : Base
	{
		public Bitmap Bitmap;
		private byte[] _jpegTables;

		public Image()
		{
		}
		public Image(byte[] jpegTables)
		{
			this._jpegTables = jpegTables;
		}

		public override void Init(Record record)
		{
			base.Init (record);

			BinaryFlashReader reader = record.GetDataReader();
			this.Id = reader.ReadUInt16();

			Bitmap bmp = null;
			byte[] alphaData = null;

			if (record.Tag == Flash.Tags.DefineBits) //Jpeg with separate jpeg table
			{
				ushort start = reader.ReadUInt16();
				if (start != 0xd8ff)
					throw new Exception("JPEG start error");
				
				
				MemoryStream stream = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(stream);
				writer.Write(this._jpegTables);
				writer.Write(reader.ReadBytes((int)record.TagLength));
				this._jpegTables = null;
				writer.Close();
				stream.Close();
				byte[] data = stream.ToArray();

				stream = new MemoryStream(data);
				bmp = new Bitmap(stream);
				stream.Close();
			}
			else if (record.Tag == Flash.Tags.DefineBitsJPEG2) //Jpeg with included jpeg table
			{
				byte[] data = reader.ReadBytes((int)record.TagLength);
				MemoryStream stream = new MemoryStream(data);
				bmp = new Bitmap(stream);
				stream.Close();
			}
			else if (record.Tag == Flash.Tags.DefineBitsJPEG3) //Jpeg with alpha
			{
				uint alphaDataOffset = reader.ReadUInt32();
				byte[] data = reader.ReadBytes((int)alphaDataOffset);
				MemoryStream stream = new MemoryStream(data);
				bmp = new Bitmap(stream);
				stream.Close();
				byte[] alphaDataZCompressed = reader.ReadBytes((int)reader.BytesToEnd);
				//TODO: unpack zlib compressed data
			}
			else
			{
				//ZLib-compressed
				bool UseAlpha = (record.Tag==Flash.Tags.DefineBitsLossless2?true:false);
				byte format = reader.ReadByte();
				ushort width = reader.ReadUInt16();
				ushort height = reader.ReadUInt16();
				int stride = ((width+3)/4)*4;

				int numPaletteEntries = 0;
				long size = 0;
				int bpp = 1;
				int bytesPerPaletteEntry = 3;

				if (format==3)
				{
					numPaletteEntries = reader.ReadByte();
					if (UseAlpha)
						bytesPerPaletteEntry = 4;
					size = stride*height + bytesPerPaletteEntry*numPaletteEntries;
				}
				else
				{
					if (format==4)
						bpp = 2;
					else if (format==5)
						bpp = 3;
					size = bpp*width*height;
				}

				size+=248+1024; //Don't know why...

				byte[] compressed = reader.ReadBytes((int)reader.BytesToEnd);
				byte[] decompressed = null; //zlib.decompress(compressed);

				if (format==3)
				{
					Color[] palette = new Color[numPaletteEntries];
					for (int i=0; i<numPaletteEntries;i++)
					{
						Color clr = Color.White;
						int ptr = i*bytesPerPaletteEntry;
						if (bytesPerPaletteEntry==3)
							clr = Color.FromArgb(decompressed[ptr],decompressed[ptr+1],decompressed[ptr+2]);
						else
							clr = Color.FromArgb(decompressed[ptr],decompressed[ptr+1],decompressed[ptr+2],decompressed[ptr+3]);
						palette[i] = clr;
					}
					int pixelPtr = numPaletteEntries*bytesPerPaletteEntry;
					bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					//TODO:
				}
				else
				{
					//TODO:
				}

			}

			if (bmp!=null)
			{
				if (alphaData!=null)
				{
					//TODO: write to bmp's alpha channel
				}
				MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
				mb.Name = "Flash_"+this.Id.ToString();
			}

			this.InitDone();
		}
	}
}
