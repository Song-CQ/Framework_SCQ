using System;

namespace Endogine.Serialization.Flash.Placement
{
	/// <summary>
	/// Summary description for Remove.
	/// </summary>
	public class Remove : Record
	{
		public int Depth;
		public Remove()
		{
		}

		public override void Init(Record record)
		{
			base.Init (record);

			BinaryFlashReader reader = record.GetDataReader();
			if (record.Tag == Flash.Tags.RemoveObject2)
				this.Depth = (int)reader.ReadUInt16();
			else
				throw new Exception("Placement version not implemented");
		}
	}
}
