using System;
using System.Drawing;

namespace Endogine.Serialization.Photoshop.EffectLayers
{
	/// <summary>
	/// Summary description for Shadow.
	/// </summary>
	public class Bevel : Effect
	{
		public uint Angle;
		public uint Strength;
		public uint Blur;

		public uint BlendModeSignature; //hilite
		public uint BlendModeKey; //hilite

		public uint ShadowBlendModeSignature;
		public uint ShadowBlendModeKey;

		public Color Color;  //hilite
		public Color ShadowColor;

		public byte BevelStyle;
		public byte Opacity;  //hilite
		public byte ShadowOpacity;

		public bool Enabled;
		public bool UseGlobalAngle;
		public bool Inverted;

		public Bevel(Effect effect) : base(effect)
		{
			BinaryReverseReader reader = base.GetDataReader();

			this.Angle = reader.ReadUInt32();
			this.Strength = reader.ReadUInt32();
			this.Blur = reader.ReadUInt32();

			this.BlendModeSignature = reader.ReadUInt32();
			this.BlendModeKey = reader.ReadUInt32();

			this.ShadowBlendModeSignature = reader.ReadUInt32();
			this.ShadowBlendModeKey = reader.ReadUInt32();

			this.Color = this.ReadColorWithAlpha(reader);
			this.ShadowColor = this.ReadColorWithAlpha(reader);


			this.Enabled = reader.ReadBoolean();
			this.UseGlobalAngle = reader.ReadBoolean();
			this.Inverted = reader.ReadBoolean();

			reader.Close();
		}
	}
}
