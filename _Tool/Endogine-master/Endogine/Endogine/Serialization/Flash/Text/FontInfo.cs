using System;

namespace Endogine.Serialization.Flash.Text
{
	/// <summary>
	/// Summary description for FontInfo.
	/// </summary>
	public class FontInfo : Record
	{
		public FontInfo()
		{
		}

		public override void Init(Record record)
		{
			base.Init (record);

			BinaryFlashReader reader = this.GetDataReader();
			ushort id = reader.ReadUInt16();
			((Font)record.Owner.Characters[id]).ReadFontInfo(this);
		}
	}
}
