using System;
using System.Drawing;

namespace Endogine.Serialization.Photoshop.EffectLayers
{
	/// <summary>
	/// Summary description for Effect.
	/// </summary>
	public class Effect
	{
		public string Name;
		public uint Version;

		public uint Size;

		protected byte[] loadedData;

		public Effect(BinaryReverseReader reader)
		{
			this.Name = new string(reader.ReadChars(4));
			this.Size = reader.ReadUInt32();
			this.Version = reader.ReadUInt32();

			this.loadedData = reader.ReadBytes((int)this.Size-4); //4 bytes for version
		}

		public Effect(Effect effect)
		{
			this.Name = effect.Name;
			this.Version = effect.Version;
			this.Size = effect.Size;
			this.loadedData = effect.loadedData;
		}

		public BinaryReverseReader GetDataReader()
		{
			System.IO.MemoryStream stream = new System.IO.MemoryStream(this.loadedData);
			return new BinaryReverseReader(stream);
		}


		public Color ReadColor(BinaryReverseReader reader)
		{
			reader.BaseStream.Position+=2; //padding
			ushort r = reader.ReadUInt16();
			ushort g = reader.ReadUInt16();
			ushort b = reader.ReadUInt16();
			return Color.FromArgb((int)r,(int)g,(int)b);
		}

		public Color ReadColorWithAlpha(BinaryReverseReader reader)
		{
			reader.BaseStream.Position+=2; //padding
			ushort a = reader.ReadUInt16();
			ushort r = reader.ReadUInt16();
			ushort g = reader.ReadUInt16();
			ushort b = reader.ReadUInt16();
			return Color.FromArgb((int)a,(int)r,(int)g,(int)b);
		}
	}
}
