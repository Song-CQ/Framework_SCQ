using System;

namespace Endogine.Serialization.Flash.Shape.ShapeCommand
{
	/// <summary>
	/// Summary description for Base.
	/// </summary>
	public class Base
	{
		public Base()
		{
		}

		public virtual bool MovesTurtle
		{
			get {return false;}
		}
		public virtual bool Draws
		{
			get {return false;}
		}
//		public virtual bool IsFillStyle
//		{
//			get {return false;}
//		}
		public virtual bool IsStyle
		{
			get {return false;}
		}


		public virtual EPoint GetNewLoc(EPoint ptStart)
		{
			return ptStart;
		}

	}
}
