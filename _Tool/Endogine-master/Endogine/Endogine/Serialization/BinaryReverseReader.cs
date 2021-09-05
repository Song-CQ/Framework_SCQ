using System;
using System.IO;

namespace Endogine.Serialization
{
	/// <summary>
	/// Summary description for BinaryReverseReader.
	/// </summary>
	public class BinaryReverseReader : BinaryReaderEx
	{
		public BinaryReverseReader(Stream a_stream) : base(a_stream)
		{
		}

		public float ReadPSD8BitSingle()
		{
			//TODO: examine PSD format!
			return base.ReadByte() + base.ReadByte();
		}


		public override short ReadInt16()
		{
			short val = base.ReadInt16();
			unsafe
			{
				this.SwapBytes((byte*)&val, 2);
			}
			return val;
		}
		public override int ReadInt32()
		{
			int val = base.ReadInt32();
			unsafe
			{
				this.SwapBytes((byte*)&val, 4);
			}
			return val;
		}
		public override long ReadInt64()
		{
			long val = base.ReadInt64();
			unsafe
			{
				this.SwapBytes((byte*)&val, 8);
			}
			return val;
		}

		public override ushort ReadUInt16()
		{
			ushort val = base.ReadUInt16();
			//IntPtr ptr = 
			unsafe
			{
				this.SwapBytes((byte*)&val, 2);
			}
			return val;
		}
		public override uint ReadUInt32()
		{
			uint val = base.ReadUInt32();
			unsafe
			{
				this.SwapBytes((byte*)&val, 4);
			}
			return val;
		}
		public override ulong ReadUInt64()
		{
			ulong val = base.ReadUInt64();
			unsafe
			{
				this.SwapBytes((byte*)&val, 8);
			}
			return val;
		}



		unsafe protected void SwapBytes(byte* ptr, int nLength)
		{
			for(long i=0; i<nLength/2; ++i) 
			{
				byte t = *(ptr+i);
				*(ptr+i) = *(ptr+nLength - i - 1);
				*(ptr+nLength - i - 1) = t;
			}
		}
	}
}
