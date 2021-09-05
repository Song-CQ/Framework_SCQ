using System;
using System.Collections;

namespace Endogine.Serialization.Flash.Text
{
	/// <summary>
	/// Summary description for Font.
	/// </summary>
	public class Font : Record
	{
		public ushort Id;
		public string Name;
		public bool Bold;
		public bool Italic;
		public bool Small;

		public bool DoubleByte;
		public bool Ansi;
		public byte LanguageCode;

		public Font()
		{
		}

		public override void Init(Record record)
		{
			base.Init (record);

			BinaryFlashReader reader = this.GetDataReader();
			this.Id = reader.ReadUInt16();

			bool wideOffsets = false;
			ArrayList offsets = new ArrayList();
			int nNumChars;
			bool hasLayout = false;

			if (this.Tag == Flash.Tags.DefineFont2)
			{
				byte flags = reader.ReadByte();
				hasLayout = (flags & 128)>0;
				bool shiftJIS = (flags & 64)>0;
				this.Small = (flags & 32)>0;
				this.Ansi = (flags & 16)>0;
				wideOffsets = (flags & 8)>0;
				this.DoubleByte = (flags & 4)>0;
				this.Italic = (flags & 2)>0;
				this.Bold = (flags & 1)>0;

				this.LanguageCode = reader.ReadByte();
				this.Name = reader.ReadPascalString();
				nNumChars = reader.ReadUInt16();
			}
			else
			{
				ushort firstOffset = reader.ReadUInt16();
				offsets.Add(firstOffset);
				nNumChars = firstOffset/2;
			}

			for (int charNum=offsets.Count;charNum<nNumChars;charNum++)
			{
				if (wideOffsets)
					offsets.Add(reader.ReadUInt32());
				else
					offsets.Add((uint)reader.ReadUInt16());
			}

			foreach (uint offset in offsets)
			{
				Record recordX = new Record(reader, this.Owner);
				Shape.Shape shape = new Endogine.Serialization.Flash.Shape.Shape();
				shape.Init(recordX);
			}

			if (this.Tag == Flash.Tags.DefineFont2)
			{
				ArrayList codeTable = new ArrayList();
				for (int charNum=0;charNum<nNumChars;charNum++)
				{
					if (this.DoubleByte)
						codeTable.Add(reader.ReadUInt16());
					else
						codeTable.Add((ushort)reader.ReadByte());
				}

				if (hasLayout)
				{
					short ascent = reader.ReadInt16();
					short descent = reader.ReadInt16();
					short leading = reader.ReadInt16();
					ArrayList advanceTable = new ArrayList();
					for (int i=0; i<nNumChars; i++)
						advanceTable.Add(reader.ReadInt16());
					ArrayList boundsTable = new ArrayList();
					for (int i=0; i<nNumChars; i++)
					{
						ERectangle rect = reader.ReadRect();
						boundsTable.Add(rect);
					}
					ushort kerningCount = reader.ReadUInt16();
					for (int i=0; i<kerningCount; i++)
					{
						//Kerning record
					}
				}
			}
		}

		public void ReadFontInfo(FontInfo fi)
		{
			BinaryFlashReader reader = fi.GetDataReader();
			this.Name = reader.ReadUnicodePascalString();

			byte flags = reader.ReadByte();
			this.Small = (flags & 32)!=0;
			bool shiftJIS = (flags & 16)!=0;
			this.Ansi = (flags & 8)!=0;
			this.Italic = (flags & 4)!=0;
			this.Bold = (flags & 2)!=0;
			this.DoubleByte = (flags & 1)!=0;

			if (fi.Tag == Flash.Tags.DefineFontInfo2)
				this.LanguageCode = reader.ReadByte();

			int nNumChars = (int)reader.BaseStream.Length - (int)reader.BaseStream.Position; //number of bytes left

			if (this.DoubleByte)
				nNumChars/=2;
			ArrayList codes = new ArrayList();
			
			for (int charNum=0;charNum<nNumChars;charNum++)
			{
				if (this.DoubleByte)
					codes.Add(reader.ReadUInt16());
				else
					codes.Add((ushort)reader.ReadByte());
			}
		}
	}
}
