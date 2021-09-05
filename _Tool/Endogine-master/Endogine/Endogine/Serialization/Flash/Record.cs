using System;

namespace Endogine.Serialization.Flash
{
	/// <summary>
	/// Summary description for Record.
	/// </summary>
	public class Record
	{
		//public int TagCode;
		public Flash.Tags Tag;
		public uint TagLength;
		public byte[] loadedData;
		public Flash Owner;

		public Record(BinaryFlashReader reader, Flash owner)
		{
			ushort nVal = reader.ReadUInt16();
			this.TagLength = (uint)((int)nVal & 63);
			//this.TagCode = nVal>>6;
			this.Tag = (Flash.Tags)(nVal>>6);

			if (this.TagLength == 63)
				this.TagLength = reader.ReadUInt32();

			this.loadedData = reader.ReadBytes((int)this.TagLength);

			this.Owner = owner;
		}

		public Record()
		{}
//		public Record(Record record)
//		{
//			//this.TagCode = record.TagCode;
//			this.Tag = record.Tag;
//			this.TagLength = record.TagLength;
//		}

		public virtual void Init(Record record)
		{
			this.CopyRecord(record);
		}
		protected void CopyRecord(Record record)
		{
			this.Tag = record.Tag;
			this.TagLength = record.TagLength;
			this.Owner = record.Owner;
		}
		public bool Inited
		{
			get {return this.TagLength > 0;}
		}

		public BinaryFlashReader GetDataReader()
		{
			System.IO.MemoryStream stream = new System.IO.MemoryStream(this.loadedData);
			return new BinaryFlashReader(stream);
		}
	}
}
