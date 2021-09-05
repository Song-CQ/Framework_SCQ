using System;

namespace Endogine.Serialization.Flash.Basic
{
	/// <summary>
	/// Summary description for Matrix.
	/// </summary>
	public class Matrix
	{
		public int A;
		public int B;
		public int C;
		public int D;
		public int Tx;
		public int Ty;

		public Matrix (BinaryFlashReader reader)
		{
			bool bHasScale = reader.ReadBoolean();
			if (bHasScale)
			{
				int nNumBits = (int)reader.ReadBits(5);
				long[] vals = reader.ReadBitArray(2, nNumBits, true);
				//TODO: scale is 16.16 fixed!?! Why numbits in that case? MM doc error as usual...
				//matrix.Scale((float)vals[0]/65536, (float)vals[1]/65536);
				this.A = (int)vals[0];
				this.D = (int)vals[1];
			}
			else
				this.A = this.D = 0x00010000;

			bool bHasRotateSkew = reader.ReadBoolean();
			if (bHasRotateSkew)
			{
				int nNumBits = (int)reader.ReadBits(5);
				long[] vals = reader.ReadBitArray(2, nNumBits, true);
				//matrix.Shear((float)vals[0]/65536, (float)vals[1]/65536);
				this.B = (int)vals[0];
				this.C = (int)vals[1];
			}

			//Translate info is always included:
			if (true)
			{
				int nNumBits = (int)reader.ReadBits(5);
				long[] vals = reader.ReadBitArray(2, nNumBits, true);
				//matrix.Translate((float)vals[0]/20,(float)vals[1]/20);
				this.Tx = (int)vals[0];
				this.Ty = (int)vals[1];
			}

			reader.JumpToNextByteStart();
		}

		public System.Drawing.Drawing2D.Matrix GetDotNet()
		{
			return new System.Drawing.Drawing2D.Matrix(
				(float)this.A/65536,
				(float)this.B/65536,
				(float)this.C/65536,
				(float)this.D/65536,
				(float)this.Tx/20,
				(float)this.Ty/20);
		}
	}
}
