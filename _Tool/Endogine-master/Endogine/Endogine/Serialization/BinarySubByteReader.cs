using System;

namespace Endogine.Serialization
{
	/// <summary>
	/// Summary description for BinarySubByteReader.
	/// </summary>
	public class BinarySubByteReader : BinaryReaderEx
	{
		private int unusedBitsInByte = 0;
		private byte currentByteVal = 0;

		public BinarySubByteReader(System.IO.Stream a_stream) : base(a_stream)
		{
		}

		public void JumpToNextByteStart()
		{
			this.unusedBitsInByte = 0;
			this.currentByteVal = 0;
		}

		public override bool ReadBoolean()
		{
			return (this.ReadBits(1)==1)?true:false;
		}
		public override byte ReadByte()
		{
			return (byte)this.ReadBits(8, false);
		}
		public override ushort ReadUInt16()
		{
			int val = (int)this.ReadBits(16, false);
			return (ushort)(((val&0x00ff)<<8) + (val>>8));
		}
		public override uint ReadUInt32()
		{
			int val = (int)this.ReadBits(32, false);
			return (uint)(
				((val&0x000000ff)<<24)
				+ ((val&0x0000ff00)<<8)
				+ ((val&0x00ff0000)>>8)
				+ ((val&0xff000000)>>24));
		}


		public override char ReadChar()
		{
			return (char)this.ReadBits(8, true);
		}
		public override short ReadInt16()
		{
			return (short)this.ReadBits(16, true);
		}
		public override int ReadInt32()
		{
			return (int)this.ReadBits(32, true);
		}





		public long ReadBits(int numBits)
		{
			return this.ReadBits(numBits, false);
		}
		public long ReadBits(int numBits, bool signed)
		{
			int nBitsToReadFromNextBytes = numBits - this.unusedBitsInByte;

			//do we have to read in new byte(s) or not?
			int nBytesToRead = 0;
			if (nBitsToReadFromNextBytes > 0)
			{
				nBytesToRead = nBitsToReadFromNextBytes/8;
				if (nBitsToReadFromNextBytes%8 > 0)
					nBytesToRead++;
			}
  
			int nLeftOverFromAlreadyReadByte = 0;
			if (this.unusedBitsInByte > 0)
			{
				nLeftOverFromAlreadyReadByte = this.currentByteVal & ((1<<this.unusedBitsInByte)-1);
				//we're in the middle of a byte (neither start nor end are at byte boundaries)
				if (nBitsToReadFromNextBytes < 0)
					nLeftOverFromAlreadyReadByte>>=-nBitsToReadFromNextBytes;
			}

			int nVal = nLeftOverFromAlreadyReadByte;
			for (int nByte = 0; nByte < nBytesToRead; nByte++)
			{
				this.currentByteVal = base.ReadByte();
				if (nByte < nBytesToRead-1)
				{
					nVal*=256;
					nVal+=this.currentByteVal;
				}
				else
				{
					int nNumBitsLeftToRead = nBitsToReadFromNextBytes-nByte*8;
					nVal<<=nNumBitsLeftToRead;//this.Pow(2,nNumBitsLeftToRead);
					nVal+= this.currentByteVal >> (8-nNumBitsLeftToRead);
				}
			}

			this.unusedBitsInByte = (8-(nBitsToReadFromNextBytes%8)) % 8;

			if (signed)
			{
				int nLeftmostBitVal = 1<<(numBits-1);
				if ((nVal & nLeftmostBitVal) > 0)
				{
					nVal |= (int)(-1L << numBits);
					//nVal&=(nLeftmostBitVal-1);
					//nVal=-nVal;
				}
			}
			return nVal;
		}

		public long[] ReadBitArray(int nNumVals, int nBitsPerVal, bool signed)
		{
			long[] vals = new long[nNumVals];
			for (int i = 0; i < nNumVals; i++)
				vals[i] = this.ReadBits(nBitsPerVal, signed);
			return vals;
		}
	}
}
