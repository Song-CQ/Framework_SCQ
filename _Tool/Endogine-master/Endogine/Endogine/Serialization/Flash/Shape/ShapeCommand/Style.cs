using System;

namespace Endogine.Serialization.Flash.Shape.ShapeCommand
{
	/// <summary>
	/// Summary description for Style.
	/// </summary>
	public class Style : Base
	{
		private int _style;
		public Style(int style)
		{
			this._style = style;
		}

		public int StyleId
		{
			get {return this._style;}
		}
		public override bool IsStyle
		{
			get {return true;}
		}

		public override string ToString()
		{
			return this.StyleId.ToString();
		}
	}
}
