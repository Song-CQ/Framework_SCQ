using System;

namespace Endogine.Serialization.Flash.Shape.ShapeCommand
{
	/// <summary>
	/// Summary description for Move.
	/// </summary>
	public class Move : Base
	{
		EPoint _ptTarget;

		public Move(EPoint ptTarget)
		{
			this._ptTarget = ptTarget;
		}

		public override bool MovesTurtle
		{
			get {return true;}
		}

		public EPoint Target
		{
			get {return this._ptTarget;}
		}
		public override EPoint GetNewLoc(EPoint ptStart)
		{
			return this._ptTarget;
		}

		public override string ToString()
		{
			return this._ptTarget.ToStringSimple();
		}

	}
}
