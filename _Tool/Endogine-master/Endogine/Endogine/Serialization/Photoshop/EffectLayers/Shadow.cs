using System;
using System.Drawing;

namespace Endogine.Serialization.Photoshop.EffectLayers
{
	/// <summary>
	/// Summary description for Shadow.
	/// </summary>
	public class Shadow : Effect
	{
		public uint Blur;
		public uint Angle;
		public uint Distance;
		public Color Color;
		public uint BlendModeSignature;
		public uint BlendModeKey;
		public bool Enabled;
		public bool UseGlobalAngle;
		public byte Opacity;

		public Shadow(Effect effect) : base(effect)
		{
			BinaryReverseReader reader = base.GetDataReader();

			this.Blur = reader.ReadUInt32();
			this.Angle = reader.ReadUInt32();
			this.Distance = reader.ReadUInt32();

			this.Color = this.ReadColor(reader);

			this.BlendModeSignature = reader.ReadUInt32();
			this.BlendModeKey = reader.ReadUInt32();
			this.Enabled = reader.ReadBoolean();
			this.UseGlobalAngle = reader.ReadBoolean();
			this.Opacity = reader.ReadByte();

			reader.Close();
		}
	}
}
