using System;

namespace Endogine.Serialization
{
	/// <summary>
	/// Summary description for BinaryReaderEx.
	/// </summary>
	public class BinaryReaderEx : System.IO.BinaryReader
	{
		public BinaryReaderEx(System.IO.Stream a_stream) : base(a_stream, System.Text.Encoding.UTF8)
		{
		}

		/// <summary>
		/// For padding jumps. E.g. jump to next 4-even position
		/// </summary>
		/// <param name="n"></param>
		public void JumpToEvenNthByte(int n)
		{
			int nMod = (int)((this.BaseStream.Position) % (long)n); // NOPE!: -1 since it's 0-based. E.g. 3
			if (nMod > 0)
				this.BaseStream.Position+= n-nMod;
		}

		public long BytesToEnd
		{
			get {return this.BaseStream.Length - this.BaseStream.Position;}
		}

		public string ReadPascalString()
		{
			string s = "";
			byte nLength = base.ReadByte();
			for (byte i = 0; i < nLength; i++)
				s+=base.ReadChar();
			if ((nLength % 2) == 0)
				base.ReadByte();
			return s;
		}
	}
}
