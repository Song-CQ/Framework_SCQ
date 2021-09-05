using System;

namespace Endogine.Serialization.Flash.Shape.ShapeCommand
{
	/// <summary>
	/// Summary description for FillStyle.
	/// </summary>
	public class FillStyle : Style
	{
		private int _side;
		public FillStyle(int style, int side) : base(style)
		{
			this._side = side;
		}

		public int Side
		{
			get {return this._side;}
		}

		public override string ToString()
		{
			return base.ToString() + " side: "+this._side;
		}

	}
}
