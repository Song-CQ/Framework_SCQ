using System;
using System.Drawing;
using System.IO;
//using System.Collections;
using System.Collections.Generic;

namespace Endogine.Serialization.Photoshop
{
	public enum ColorModes
	{
		Bitmap=0, Grayscale=1, Indexed=2, RGB=3, CMYK=4, Multichannel=7, Duotone=8, Lab=9
	};

	/// <summary>
	/// Summary description for Photoshop.
	/// </summary>
	public class Document
	{
        private Dictionary<int, Layer> _layers;
        public Dictionary<int, Layer> Layers
        {
            get { return _layers; }
        }
		//public Header Header;
		public ImageResources.ResolutionInfo ResolutionInfo;
		public byte[] ColorData;
		public short NumColors;
        
        public ushort Version;
        public short Channels;
        public ulong Rows;
        public ulong Columns;
        public int BitsPerPixel;
        //public ushort Depth;
        public ColorModes ColorMode;


		public enum ResourceIDs
		{
			Undefined=0,
			MacPrintInfo=1001,
			ResolutionInfo=1005,
			AlphaChannelNames=1006,
			DisplayInfo=1007,
			Caption=1008,
			BorderInfo=1009,
			BgColor=1010,
			PrintFlags=1011,
			MultiChannelHalftoneInfo=1012,
			ColorHalftoneInfo=1013,
			DuotoneHalftoneInfo=1014,
			MultiChannelTransferFunctions=1015,
			ColorTransferFunctions=1016,
			DuotoneTransferFunctions=1017,
			DuotoneImageInfo=1018,
			BlackWhiteRange=1019,
			EPSOptions=1021,
			QuickMaskInfo=1022, //2 bytes containing Quick Mask channel ID, 1 byte boolean indicating whether the mask was initially empty.
			LayerStateInfo=1024, //2 bytes containing the index of target layer. 0=bottom layer.
			WorkingPathUnsaved=1025,
			LayersGroupInfo=1026, //2 bytes per layer containing a group ID for the dragging groups. Layers in a group have the same group ID.
			IPTC_NAA=1028,
			RawFormatImageMode=1029,
			JPEGQuality=1030,
			GridGuidesInfo=1032,
			Thumbnail1=1033,
			CopyrightInfo=1034,
			URL=1035,
			Thumbnail2=1036,
			GlobalAngle=1037,
			ColorSamplers=1038,
			ICCProfile=1039, //The raw bytes of an ICC format profile, see the ICC34.pdf and ICC34.h files from the Internation Color Consortium located in the documentation section
			Watermark=1040,
			ICCUntagged=1041, //1 byte that disables any assumed profile handling when opening the file. 1 = intentionally untagged.
			EffectsVisible=1042, //1 byte global flag to show/hide all the effects layer. Only present when they are hidden.
			SpotHalftone=1043, // 4 bytes for version, 4 bytes for length, and the variable length data.
			DocumentSpecific=1044,
			UnicodeAlphaNames=1045, // 4 bytes for length and the string as a unicode string
			IndexedColorTableCount=1046, // 2 bytes for the number of colors in table that are actually defined
			TransparentIndex=1047,
			GlobalAltitude=1049,  // 4 byte entry for altitude
			Slices=1050,
			WorkflowURL=1051, //Unicode string, 4 bytes of length followed by unicode string
			JumpToXPEP=1052, //2 bytes major version, 2 bytes minor version,
			//4 bytes count. Following is repeated for count: 4 bytes block size,
			//4 bytes key, if key = 'jtDd' then next is a Boolean for the dirty flag
			//otherwise it’s a 4 byte entry for the mod date
			AlphaIdentifiers=1053, //4 bytes of length, followed by 4 bytes each for every alpha identifier.
			URLList=1054, //4 byte count of URLs, followed by 4 byte long, 4 byte ID, and unicode string for each count.
			VersionInfo=1057, //4 byte version, 1 byte HasRealMergedData, unicode string of writer name, unicode string of reader name, 4 bytes of file version.
			Unknown4=1058, //pretty long, 302 bytes in one file. Holds creation date, maybe Photoshop license number
			XMLInfo=1060, //some kind of XML definition of file. The xpacket tag seems to hold binary data
			Unknown=1061, //seems to be common!
			Unknown2=1062, //seems to be common!
			Unknown3=1064, //seems to be common!
			PathInfo=2000, //2000-2999 actually I think?
			ClippingPathName=2999,
			PrintFlagsInfo=10000
		}

		public Document(string a_sFilename)
		{
			FileStream stream = new FileStream(a_sFilename,
				FileMode.Open, FileAccess.Read);
			//stream.
			BinaryReverseReader reader = new BinaryReverseReader(stream); //, System.Text.Encoding.UTF8); //System.Text.Encoding.BigEndianUnicode);

			string signature = new string(reader.ReadChars(4));
			if (signature != "8BPS")
				return;

            #region Header
            this.Version = reader.ReadUInt16();
            if (Version != 1)
                throw new Exception("Can not read .psd version " + Version);
            byte[] buf = new byte[256];
            reader.Read(buf, (int)reader.BaseStream.Position, 6); //6 bytes reserved
            this.Channels = reader.ReadInt16();
            this.Rows = reader.ReadUInt32();
            this.Columns = reader.ReadUInt32();
            this.BitsPerPixel = (int)reader.ReadUInt16();
            this.ColorMode = (ColorModes)reader.ReadInt16();
            #endregion

            #region Palette
            uint nPaletteLength = reader.ReadUInt32();
			if (nPaletteLength > 0)
			{
				this.ColorData = reader.ReadBytes((int)nPaletteLength);
				if (this.ColorMode == ColorModes.Duotone)
				{
				}
				else
				{
				}
            }
            #endregion

            #region ImageResource section
            uint nResLength = reader.ReadUInt32();
			ResourceIDs resID = ResourceIDs.Undefined;
			if (nResLength > 0)
			{
				//read settings
				while (true)
				{
					long nBefore = reader.BaseStream.Position;
					string settingSignature = new string(reader.ReadChars(4));
					if (settingSignature != "8BIM")
					{
						reader.BaseStream.Position = nBefore;
						//TODO: it SHOULD be 4 bytes back - but sometimes ReadChars(4) advances 5 positions. WHY?!?
//						reader.BaseStream.Position-=4;
						break;
					}

					ImageResource imgRes = new ImageResource(reader);
					resID = (ResourceIDs)imgRes.ID;
					switch (resID) //imgRes.ID)
					{
						case ResourceIDs.ResolutionInfo:
							this.ResolutionInfo = 
								new Endogine.Serialization.Photoshop.ImageResources.ResolutionInfo(imgRes);
							break;

						case ResourceIDs.DisplayInfo:
							ImageResources.DisplayInfo displayInfo = new Endogine.Serialization.Photoshop.ImageResources.DisplayInfo(imgRes);
							break;

						case ResourceIDs.CopyrightInfo:
							ImageResources.CopyrightInfo copyright = new Endogine.Serialization.Photoshop.ImageResources.CopyrightInfo(imgRes);
							break;

						case ResourceIDs.Thumbnail1:
						case ResourceIDs.Thumbnail2:
							ImageResources.Thumbnail thumbnail = new Endogine.Serialization.Photoshop.ImageResources.Thumbnail(imgRes);
							break;

						case ResourceIDs.GlobalAngle:
							//m_nGlobalAngle = reader.ReadInt32();
							break;

						case ResourceIDs.IndexedColorTableCount:
							this.NumColors = reader.ReadInt16();
							break;

						case ResourceIDs.TransparentIndex:
							//m_nTransparentIndex = reader.ReadInt16();
							break;

						case ResourceIDs.Slices://Slices. What's that..?
							//Leftlong, Botmlong etc etc
							break;

						case ResourceIDs.XMLInfo:
							break;

						case ResourceIDs.Unknown:
							//Seems to be very common...
							break;
					}
				}
			}
			#endregion

			if (resID == ResourceIDs.Unknown4)
			{
				//it seems this one is
			}
			//reader.JumpToEvenNthByte(4);
			int nTotalLayersBytes = reader.ReadInt32();
			long nAfterLayersDefinitions = reader.BaseStream.Position + nTotalLayersBytes;

			//TODO: ??
			if (nTotalLayersBytes == 8)
				stream.Position+=nTotalLayersBytes;

			uint nSize = reader.ReadUInt32();
			long nLayersEndPos = reader.BaseStream.Position + nSize;

			short nNumLayers = reader.ReadInt16();
			bool bSkipFirstAlpha = false;

			if (nNumLayers < 0)
			{
				bSkipFirstAlpha = true;
				nNumLayers = (short)-nNumLayers;
			}

            List<Layer> loadOrderLayers = new List<Layer>();
            this._layers = new Dictionary<int, Layer>();
			for (int nLayerNum = 0; nLayerNum < nNumLayers; nLayerNum++)
			{
				Layer layerInfo = new Layer(reader, this);
                if (this._layers.ContainsKey(layerInfo.LayerID))
					throw(new Exception("Duplicate layer IDs! " + layerInfo.LayerID.ToString()));
				else
                    this._layers.Add(layerInfo.LayerID, layerInfo);
                loadOrderLayers.Add(layerInfo);
			}

			//I have no idea what this is:
//			ushort nWhat = reader.ReadUInt16();
//			reader.BaseStream.Position+=(long)this.Header.Rows*2*2; //this.Header.Channels; //*bitsperpixel

            for (int layerNum = 0; layerNum < nNumLayers; layerNum++)
            {
                Layer layer = (Layer)loadOrderLayers[layerNum];
                layer.ReadPixels(reader);
            }

            reader.BaseStream.Position = nAfterLayersDefinitions;

            
            if (false)
            {
                //the big merged bitmap (which is how the photoshop document looked when it was saved)
                //TODO: read!

                //Bitmap bmp = null;
                //if (bmp != null)
                //{
                //    Sprite sp = new Sprite();
                //    MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
                //    sp.Member = mb;
                //}
            }

			reader.Close();
			stream.Close();
		}

        public void CreateSprites()
        {
            foreach (Layer layer in this._layers.Values)
            {
                layer.CreateSprite();
            }
        }
	}
}
