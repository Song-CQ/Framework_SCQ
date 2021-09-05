using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Endogine.Serialization.Flash.Placement
{
	/// <summary>
	/// Summary description for Place.
	/// </summary>
	public class Placement : Record
	{
		public int Depth;
		public ushort CharacterId;
		public Basic.Matrix Matrix;
		public Basic.ColorMatrix ColorMatrix;
		public int Ratio;
		public string Name;
		public int ClipDepth;

		public Placement()
		{
		}

		public override void Init(Record record)
		{
			base.Init (record);

			BinaryFlashReader reader = record.GetDataReader();

			if (this.Tag == Flash.Tags.PlaceObject)
			{
				this.CharacterId = reader.ReadUInt16();
				this.Depth = reader.ReadUInt16();
				this.Matrix = new Basic.Matrix(reader);
				this.ColorMatrix = new Basic.ColorMatrix(reader, false); //reader.ReadColorMatrix();
			}
			else
			{
				int nPlaceInfo = reader.ReadByte();

				bool bPlaceFlagHasClipActions = (nPlaceInfo & 128)>0;
				bool bPlaceFlagHasClipDepth = (nPlaceInfo & 64)>0;
				bool bPlaceFlagHasName = (nPlaceInfo & 32)>0;
				bool bPlaceFlagHasRatio = (nPlaceInfo & 16)>0;
				bool bPlaceFlagHasColorTransform = (nPlaceInfo & 8)>0;
				bool bPlaceFlagHasMatrix = (nPlaceInfo & 4)>0;
				bool bPlaceFlagHasCharacter = (nPlaceInfo & 2)>0;
				bool bPlaceFlagMove = (nPlaceInfo & 1)>0;

				this.Depth = reader.ReadUInt16();
				if (bPlaceFlagHasCharacter)
					this.CharacterId = reader.ReadUInt16();
				if (bPlaceFlagHasMatrix)
					this.Matrix = new Basic.Matrix(reader);
				if (bPlaceFlagHasColorTransform)
					this.ColorMatrix = new Basic.ColorMatrix(reader, true); 
				if (bPlaceFlagHasRatio)
					this.Ratio = reader.ReadUInt16();
				if (bPlaceFlagHasName)
					this.Name = reader.ReadPascalString(); //readNullString
				if (bPlaceFlagHasClipDepth)
					this.ClipDepth = reader.ReadUInt16();
				if (bPlaceFlagHasClipActions)
				{
					//TODO:!!!
				}

				bool bFilters = false;
				bool bBlend = false;
				bool bCache = false;
				if (this.Tag == Flash.Tags.PlaceObject3)
				{
					nPlaceInfo = reader.ReadByte();
					bFilters = (nPlaceInfo&1)>0; //TODO: reverse order? 128,64,32?
					bBlend = (nPlaceInfo&2)>0; //TODO: implement blend&cache
					bCache = (nPlaceInfo&4)>0;
				}
				if (bFilters)
				{
					byte nNumFilters = reader.ReadByte();
					for (int i=0; i<nNumFilters; i++)
					{
						byte filterId = reader.ReadByte();
						Filter.Base filter = null;
						switch (filterId)
						{
							case 0:
								filter = new Filter.DropShadow();
								break;
							case 1:
								filter = new Filter.Blue();
								break;
							case 2:
								filter = new Filter.Glow();
								break;
							case 3:
								filter = new Filter.Bevel();
								break;
							case 4:
								filter = new Filter.GradientGlow();
								break;
							case 6:
								filter = new Filter.AdjustColor();
								break;
							case 7:
								filter = new Filter.GradientBevel();
								break;
						}
					}
				}
			}
		}

		public Sprite Execute(Flash player, Sprite spExistingSprite)
		{
			if (spExistingSprite == null)
			{
				Sprite sp = new Sprite();
				sp.TextureFilter = Sprite.TextureFilters.High;
				sp.LocZ = this.Depth;
				sp.Ink = RasterOps.ROPs.BgTransparent;
				//sp.Visible = false;
				spExistingSprite = sp;
				//EH.Put("New sprite");
			}
			ushort characterId = this.CharacterId;
			if (this.CharacterId > 0)
				this.Owner.CharacterIdsByDepth[this.Depth] = this.CharacterId;
			else
				characterId = (ushort)this.Owner.CharacterIdsByDepth[this.Depth];
			object character = this.Owner.Characters[characterId];

			//EH.Put("Depth: "+this.Depth.ToString());
			if (character==null)
			{
				throw new Exception("Error - no character. Id: " + characterId.ToString());
			}
			
			//EH.Put("Char: "+character.GetType().ToString() + " "+characterId.ToString());

			if (this.Matrix!=null)
			{
				spExistingSprite.Loc = new EPointF(this.Matrix.Tx, this.Matrix.Ty); ///player.TwipSize;
				spExistingSprite.Scaling = new EPointF(this.Matrix.A, this.Matrix.D)/65535;
				//EH.Put("Loc: "+spExistingSprite.Loc.ToString() + " Sc: "+spExistingSprite.Scaling.ToString());
				//TODO: rotation etc
			}

			MemberSpriteBitmap mb = null;
			if (character!=null)
			{
				if (character is Shape.MorphShape)
				{
					//EH.Put("Ratio:"+this.Ratio);
					Shape.MorphShape ms = (Shape.MorphShape)character;
					EPoint ptOffset;
					string sOut = "MS"+ms.Id.ToString()+"-"+this.Ratio.ToString().PadLeft(5,'0')+"d"+this.Depth;
//					if (this.Ratio == 0)
//					{
//						System.Collections.ArrayList morphed = ms.CreateMorphedShape((float)this.Ratio/65535);
//						string s = ms.WriteCommands(morphed);
//						Endogine.Files.FileReadWrite.Write(sOut+".txt", s);
//
//						Shape.MorphShape.Debug = false;
//						if (this.Depth == 6)
//							Shape.MorphShape.Debug = true;
//					}
					Bitmap bmp = ms.RenderToBitmap(this.Owner.TwipSize, (float)this.Ratio/65535, out ptOffset);
					if (bmp==null)
						return null;
					mb = new MemberSpriteBitmap(bmp);
					mb.RegPoint = ptOffset*-1;
					//bmp.Save(sOut+".png");
					//mb.RegPoint = this.Bounds.Location*-1; //.ToEPoint()
				}
				else if (character is Shape.Shape)
				{
					Shape.Shape shape = (Shape.Shape)character;
					mb = (MemberSpriteBitmap)this.Owner.Members[shape.Id];
					if (mb == null)
					{
//						Bitmap bmp = shape.RenderToBitmap(this.Owner.TwipSize);
//						bmp.Save("S"+shape.Id+"d"+this.Depth+".png");
//						
//						string s = shape.WriteCommands(shape.CommandList);
//						Endogine.Files.FileReadWrite.Write("S0d"+this.Depth+".txt", s);

						mb = shape.CreateAsMember(this.Owner.TwipSize);
						if (mb!=null)
							mb.Name = "Flash_"+shape.Id.ToString();
						this.Owner.Members[shape.Id] = mb;
					}
				}
			}
			if (mb==null)
				mb = (MemberSpriteBitmap)EH.Instance.CastLib.GetByName("BallGreen");
			spExistingSprite.Member = mb;

			return spExistingSprite;
		}
	}
}
