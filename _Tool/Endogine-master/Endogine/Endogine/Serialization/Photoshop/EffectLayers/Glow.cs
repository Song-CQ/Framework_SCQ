using System;
using System.Drawing;

namespace Endogine.Serialization.Photoshop.EffectLayers
{
	/// <summary>
	/// Summary description for Shadow.
	/// </summary>
	public class Glow : Effect
	{
		public uint Blur;
		public uint Intensity;
		public Color Color;
		public uint BlendModeSignature;
		public uint BlendModeKey;
		public bool Enabled;
		public bool UseGlobalAngle;
		public byte Opacity;

		public Glow(Effect effect) : base(effect)
		{
			BinaryReverseReader reader = base.GetDataReader();

			uint version = reader.ReadUInt32(); //two version specifications?!?

			this.Blur = reader.ReadUInt32();
			this.Intensity = reader.ReadUInt32();
			this.Color = this.ReadColorWithAlpha(reader);

			this.BlendModeSignature = reader.ReadUInt32();
			this.BlendModeKey = reader.ReadUInt32();
			this.Enabled = reader.ReadBoolean();
			this.UseGlobalAngle = reader.ReadBoolean();
			this.Opacity = reader.ReadByte();

			reader.Close();
		}
	}
}
