using System;
using System.Collections.Generic;

namespace Endogine.Serialization.Photoshop
{
    //public class Page
    //{
    //    public ERectangle Rect;
    //    public Page(BinaryReverseReader reader)
    //    {
    //        this.Rect = new ERectangle();
    //        this.Rect.Y = reader.ReadInt32();
    //        this.Rect.X = reader.ReadInt32();
    //        this.Rect.Height = reader.ReadInt32() - Rect.Y;
    //        this.Rect.Width = reader.ReadInt32() - Rect.X;
    //    }
    //}

	/// <summary>
	/// Summary description for LayerInfo.
	/// </summary>
	public class Layer
	{
        Document _document;
        public Document Document
        {
            get { return _document; }
        }

        ERectangle _rect;
        public ERectangle Rectangle
        {
            get { return _rect; }
        }
        public int Width
        {
            get { return this._rect.Width; }
        }
        public int Height
        {
            get { return this._rect.Height; }
        }
        public int BitsPerPixel
        {
            get { return this.Document.BitsPerPixel; }
        }

		//public Page page;
		public ushort NumChannels;

        Dictionary<int, Channel> _channels;
        Mask _mask;
        public List<EffectLayers.Effect> Effects = new List<EffectLayers.Effect>();


        /// <summary>
        /// Name that makes sense
        /// </summary>
		public string BlendKey;
        public enum BlendKeys
        {
            Normal, Darken, Lighten, Hue, Saturation, Color, Luminosity, Multiply, Screen, Dissolve, Overlay, HardLight, SoftLight, Difference, Exclusion, ColorDodge, ColorBurn
        }
        private enum _blendKeysPsd
        {
            norm, dark, lite, hue, sat, colr, lum, mul, scrn, diss, over, hLit, sLit, diff, smud, div, idiv
        }

		public byte Opacity;
		public byte Clipping;
		public byte Flags;


		public string Name;

		public int LayerID;
		public EPointF ReferencePoint;
		public bool BlendClipping;
		public bool Blend;
		public bool Knockout;
		public uint NameSourceSetting;
		public System.Drawing.Color SheetColor;
		public string UnicodeName;

		public Layer(BinaryReverseReader reader, Document document)
		{
            this._document = document;

            this._rect = new ERectangle();
            this._rect.Y = reader.ReadInt32();
            this._rect.X = reader.ReadInt32();
            this._rect.Height = reader.ReadInt32() - this._rect.Y;
            this._rect.Width = reader.ReadInt32() - this._rect.X;


			this.NumChannels = reader.ReadUInt16();
            this._channels = new Dictionary<int, Channel>();
            for (int channelNum = 0; channelNum < this.NumChannels; channelNum++)
			{
                Channel ch = new Channel(reader, this);
                this._channels.Add(ch.Usage, ch);
			}

			string sHeader = new string(reader.ReadChars(4));
			if (sHeader != "8BIM")
				throw(new Exception("Layer Channelheader error!"));

			this.BlendKey = new string(reader.ReadChars(4));
            int nBlend = -1;
            try
            {
                nBlend = (int)Enum.Parse(typeof(_blendKeysPsd), this.BlendKey);
            }
            catch { }
            if (nBlend >= 0)
            {
                BlendKeys key = (BlendKeys)nBlend;
                this.BlendKey = Enum.GetName(typeof(BlendKeys), key);
            }

            this.Opacity = reader.ReadByte(); //(byte)(255 - (int)reader.ReadByte());
			//paLayerInfo[#Opacity] = 256 - m_oReader.readUC() --256-ScaleCharToQuantum(ReadBlobByte(image))
			this.Clipping = reader.ReadByte();
			this.Flags = reader.ReadByte();

	        reader.ReadByte(); //padding


			uint nSize = reader.ReadUInt32();
			long nChannelEndPos = reader.BaseStream.Position + (long)nSize;
			if (nSize > 0)
			{
				uint nLength;
				//uint nCombinedlength = 0;

                this._mask = new Mask(reader, this);
                if (this._mask.Rectangle == null)
                    this._mask = null;

                //reader.BaseStream.Position+=nLength-16;

				//nCombinedlength+= nLength + 4;

				//blending ranges
				nLength = reader.ReadUInt32();
				for (uint i = 0; i < nLength/8; i++)
				{
					uint color1 = reader.ReadUInt32();
                    uint color2 = reader.ReadUInt32();
				}
				//nCombinedlength+= nLength + 4;

				//Name
				nLength = (uint)reader.ReadByte();
				reader.BaseStream.Position-=1;
				this.Name = reader.ReadPascalString();
				//nCombinedlength+= nLength + 4;


				#region Adjustment etc layers
				//TODO: there's probably a 2-byte padding here
				sHeader = new string(reader.ReadChars(4));
				if (sHeader != "8BIM")
				{
					reader.BaseStream.Position-=2;
					sHeader = new string(reader.ReadChars(4));
				}
				reader.BaseStream.Position-=4;

				do
				{
					try
					{
						this.ReadPSDChannelTag(reader);
					}
					catch
					{
						//dunno what the last bytes are for, just skip them:
						reader.BaseStream.Position = nChannelEndPos;
					}
				}
				while(reader.BaseStream.Position < nChannelEndPos);

				#endregion
			}
		}

        #region Read adjustment layers
		private void ReadPSDChannelTag(BinaryReverseReader reader)
		{
			string sHeader = new string(reader.ReadChars(4));
			if (sHeader != "8BIM")
			{
				reader.BaseStream.Position-=4; //back it up before throwing exception
				throw(new Exception("Effect header incorrect"));
			}

			string sKey = new string(reader.ReadChars(4));
			uint nLength = reader.ReadUInt32();
			long nPosStart = reader.BaseStream.Position;

			switch (sKey)
			{
				case "lyid":
					this.LayerID = (int)reader.ReadUInt32();
					break;

				case "fxrp":
					this.ReferencePoint = new EPointF();
					this.ReferencePoint.X = reader.ReadPSD8BitSingle();
					this.ReferencePoint.Y = reader.ReadPSD8BitSingle();
					break;

				case "clbl":
					//blend clipping
					this.BlendClipping = reader.ReadBoolean();
					reader.BaseStream.Position+=3; //padding
					break;

				case "infx":
					//blend interior elements
					this.Blend = reader.ReadBoolean();
					reader.BaseStream.Position+=3; //padding
					break;

				case "knko":
					//Knockout setting
					this.Knockout = reader.ReadBoolean();
					reader.BaseStream.Position+=3; //padding
					break;

				case "lspf":
					//Protected settings
					//TODO:
					reader.ReadBytes(4); //nLength?
					//bits 0-2 = Transparency, composite and position
					break;

				case "lclr":
					//Sheet Color setting
					this.SheetColor = System.Drawing.Color.FromArgb(
						reader.ReadByte(),
						reader.ReadByte(),
						reader.ReadByte(),
						reader.ReadByte());
					reader.BaseStream.Position+=2; //padding
					break;

				case "lnsr":
					//Layer Name Source setting
					string sWhatIsThis = new string(reader.ReadChars((int)nLength));
					//this.NameSourceSetting = reader.ReadUInt32();
					break;

				case "luni":
					//Unicode Layer name
					uint nUnicodeLength = reader.ReadUInt32();
					this.UnicodeName = new string(reader.ReadChars((int)nUnicodeLength*2));
					break;

				case "lrFX":
					//Effects Layer info
					reader.BaseStream.Position+=2; //unused
					ushort nNumEffects = reader.ReadUInt16();
					//      aEffectsInfo = []
					//      paInfo[#EffectsInfo] = aEffectsInfo
					for (int nEffectNum = 0; nEffectNum < nNumEffects; nEffectNum++)
					{
						sHeader = new string(reader.ReadChars(4));
						if (sHeader != "8BIM")
							throw(new Exception("Effect header incorrect"));

						EffectLayers.Effect effectForReading = new Endogine.Serialization.Photoshop.EffectLayers.Effect(reader);
						//reader.JumpToEvenNthByte(2);
						EffectLayers.Effect effect = null;
						//long nEffectEndPos = reader.BaseStream.Position + effect.Size;
						switch (effectForReading.Name)
						{
							case "cmnS": //common state
								BinaryReverseReader subreader = effectForReading.GetDataReader();
								bool bVisible = subreader.ReadBoolean();
								//reader.BaseStream.Position+=2; //unused
								break;

							case "dsdw":
							case "isdw":
								//drop/inner shadow
								if (effectForReading.Version == 0)
									effect = new Endogine.Serialization.Photoshop.EffectLayers.Shadow(effectForReading);
								else
								{
									//TODO:
								}
								break;

							case "oglw":
							case "iglw":
								//outer/inner glow
								if (effectForReading.Version == 0)
									effect = new Endogine.Serialization.Photoshop.EffectLayers.Glow(effectForReading);
								else
								{
									//TODO:
								}

								break;
							case "bevl": //bevel
								if (effectForReading.Version == 0)
									effect = new Endogine.Serialization.Photoshop.EffectLayers.Bevel(effectForReading);
								else
								{
									//TODO:
								}
								break;

							case "sofi": //unknown
								break;
						}
						this.Effects.Add(effect);
						//reader.BaseStream.Position = nEffectEndPos;
					}
					break;

				case "lsct":
					//TODO: what is this?
					reader.BaseStream.Position+=4;
					break;

				case "TySh":
				case "lfx2":
					//TODO: what are these?
					break;

				default:
					string sMsg = "Unknown layer setting: " + sKey + " Length:" + nLength.ToString() + " Pos: "+reader.BaseStream.Position.ToString();
					//EH.Put(sMsg);
					break;
			}
			//add to nLength so it's padded to 4
			int nLengthMod = (int)(nLength % (long)4);
			if (nLengthMod > 0)
				nLength+= 4-(uint)nLengthMod;

			reader.BaseStream.Position = nPosStart + nLength;
			reader.JumpToEvenNthByte(2);
        }
#endregion

        public void ReadPixels(BinaryReverseReader reader)
        {
            foreach (Channel ch in this._channels.Values)
            {
                if (ch.Usage != -2)
                    ch.ReadPixels(reader);
            }

            if (this._mask != null)
                this._mask.ReadPixels(reader);
        }



        public Sprite CreateSprite()
        {
            //TODO: check if we've already created a member!
            System.Drawing.Bitmap bmp = this.Bitmap;
            if (bmp == null)
                return null;
            Sprite sp = new Sprite();
            sp.Name = this.Name;
            sp.LocZ = this.LayerID;
            sp.Blend = this.Opacity;
            //TODO:
            //sp.Loc = layerInfo.ReferencePoint;
            //sp.Ink = layerInfo.BlendKey;
            sp.Color = this.SheetColor;
            MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
            sp.Member = mb;
            foreach (EffectLayers.Effect effect in this.Effects)
            {
            }
            return sp;
        }

        public System.Drawing.Bitmap Bitmap
        {
            get
            {
                if (this.Width == 0 || this.Height == 0)
                    return null;

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Endogine.BitmapHelpers.Canvas canvas = Endogine.BitmapHelpers.Canvas.Create(bmp);
                canvas.Locked = true;

                Endogine.ColorEx.ColorBase clr = new Endogine.ColorEx.ColorRgbFloat();

                List<Channel> channelsToUse = new List<Channel>();
                foreach (KeyValuePair<int, Channel> kv in this._channels)
                {
                    if (kv.Key >= 0)
                        channelsToUse.Add(kv.Value);
                }
                if (this._channels.ContainsKey(-1))
                    channelsToUse.Add(this._channels[-1]); //Alpha must always come last!

                float[] channelValues = new float[channelsToUse.Count];
                for (int y = this.Height-1; y >= 0; y--)
                {
                    for (int x = this.Width - 1; x >= 0; x--)
                    {
                        for (int channelNum = 0; channelNum < channelsToUse.Count; channelNum++)
                        {
                            channelValues[channelNum] = channelsToUse[channelNum].GetPixel(x, y);
                        }

                        clr.Array = channelValues;
                        canvas.SetPixel(x, y, clr.ColorRGBA);
                    }
                }

                //TODO: read alpha from mask!

                canvas.Locked = false;
                return bmp;
            }
            set
            {

            }
        }
	}
}
