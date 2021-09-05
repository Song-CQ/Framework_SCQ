using System;

namespace Endogine.Serialization.Flash.Shape
{
	/// <summary>
	/// Summary description for Base.
	/// </summary>
	public class Base : Record
	{
		public ushort Id;

		public Base()
		{
		}

		protected void InitDone()
		{
			this.Owner.Characters.Add(this.Id, this);
		}

		public void Dispose()
		{
			this.Owner.Characters.Remove(this.Id);
		}
	}
}
