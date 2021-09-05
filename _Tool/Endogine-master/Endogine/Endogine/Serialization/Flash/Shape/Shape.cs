using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;

namespace Endogine.Serialization.Flash.Shape
{
	/// <summary>
	/// Summary description for Shape.
	/// </summary>
	public class Shape : Base
	{
		class PathInfo
		{
			public GraphicsPath Path;
			public Pen Pen;
			public Brush Brush0;
			public Brush Brush1;

			public PathInfo()
			{
				this.Path = new GraphicsPath(); //FillMode.Alternate
			}
			public ERectangleF GetBounds()
			{
				return new ERectangleF(this.Path.GetBounds(new Matrix(), this.Pen));
			}
		}

		public ERectangle Bounds;
		public ArrayList FillStyles;
		public ArrayList LineStyles;
		public ArrayList CommandList;

		public bool UseAlpha;
		private bool _extended;
		private bool _hasX; //unknown

		public static bool Debug;

		public Shape()
		{
		}

		public override void Init(Record record)
		{
			base.Init (record);

			BinaryFlashReader reader = record.GetDataReader();
			//TODO: Is this a correct assumption about DefineShape5?
			this.UseAlpha = (this.Tag==Flash.Tags.DefineShape3 || this.Tag==Flash.Tags.DefineShape5)?true:false;
			this._extended = (this.Tag==Flash.Tags.DefineShape)?false:true;
			this._hasX = (this.Tag==Flash.Tags.DefineShape4 || this.Tag==Flash.Tags.DefineShape5)?true:false;
			this.Id = reader.ReadUInt16();
			this.Bounds = reader.ReadRect();

			if (this.Tag==Flash.Tags.DefineShape4)
				reader.ReadRect(); //TODO: what's this rect for?

			this.ReadShapeWithStyle(reader);

			this.InitDone();
		}

		private void ReadShapeWithStyle(BinaryFlashReader reader)
		{
			this.FillStyles = reader.ReadFillStyleArray(this.UseAlpha, this._extended, false);
			this.LineStyles = reader.ReadLineStyleArray(this.UseAlpha, this._extended, false, this._hasX);
  
			this.CommandList = ReadShapeCommands(reader, this.FillStyles, this.LineStyles, this.UseAlpha, this._extended, false, this._hasX);

			reader.JumpToNextByteStart();
		}


		public static ArrayList ReadShapeCommands(BinaryFlashReader reader, ArrayList fillStyles, ArrayList lineStyles, bool useAlpha, bool extended, bool morph, bool hasX)
		{
			reader.JumpToNextByteStart();
			ArrayList commands = new ArrayList();

			Byte val = reader.ReadByte();
			int numFillBits = val >> 4; //this.NumFillBits
			int numLineBits = val & 15; //this.NumLineBits

			while (true)
			{
				//Debugging
//				ArrayList types = new ArrayList();
//				foreach (ShapeCommand.Base cmd in commands)
//				{
//					string name = cmd.GetType().ToString();
//					types.Add(name.Remove(0,name.LastIndexOf(".")+1) + ":"+cmd.ToString());
//				}

				bool bIsEdge = reader.ReadBoolean();
  
				if (!bIsEdge)
				{
					int nFlags = (int)reader.ReadBits(5);
					if (nFlags == 0)
						break;

					bool bStateNewStyles = (nFlags & 16) > 0;
					bool bStateLineStyle = (nFlags & 8) > 0;
					bool bStateFillStyle1 = (nFlags & 4) > 0;
					bool bStateFillStyle0 = (nFlags & 2) > 0;
					bool bStateMoveTo = (nFlags & 1) > 0;

					if (bStateMoveTo)
					{
						int nMoveBits = (int)reader.ReadBits(5);
						EPoint pntMove = new EPoint(
							(int)reader.ReadBits(nMoveBits,true),
							(int)reader.ReadBits(nMoveBits,true));
						commands.Add(new ShapeCommand.Move(pntMove));
					}
					if (bStateFillStyle0)
						commands.Add(new ShapeCommand.FillStyle((int)reader.ReadBits(numFillBits), 0));
					if (bStateFillStyle1)
						commands.Add(new ShapeCommand.FillStyle((int)reader.ReadBits(numFillBits), 1));
					if (bStateLineStyle)
						commands.Add(new ShapeCommand.LineStyle((int)reader.ReadBits(numLineBits)));
					if (bStateNewStyles)
					{
						fillStyles.AddRange(reader.ReadFillStyleArray(useAlpha, extended, morph));
						lineStyles.AddRange(reader.ReadLineStyleArray(useAlpha, extended, morph, hasX));
						numFillBits = (int)reader.ReadBits(4);
						numLineBits = (int)reader.ReadBits(4);
					}
				}
				else
				{
					bool bIsStraight = reader.ReadBoolean();
					if (bIsStraight)
					{
						int nNumBits = (int)reader.ReadBits(4)+2;
						bool bGeneralLineFlag = reader.ReadBoolean();
						//0 = horizontal/vertical, 1 = general (ie both X and Y)
						EPoint pntMove = new EPoint();
						if (bGeneralLineFlag)
						{
							pntMove.X = (int)reader.ReadBits(nNumBits,true);
							pntMove.Y = (int)reader.ReadBits(nNumBits,true);
						}
						else
						{
							//error in MMs documentation: either X or Y in here, not both!
							bool bVertical = reader.ReadBoolean();
							int nVal = (int)reader.ReadBits(nNumBits, true);
							if (bVertical)
								pntMove.Y = nVal;
							else
								pntMove.X = nVal;
						}
						commands.Add(new ShapeCommand.Line(pntMove));
					}
					else
					{
						//it's a curve
						int nNumBits = (int)reader.ReadBits(4)+2;
						long[] vals = reader.ReadBitArray(4, nNumBits, true);
						commands.Add(new ShapeCommand.Curve(new EPoint(vals[0], vals[1]), new EPoint(vals[2], vals[3])));
					}
				}
			}
			return commands;
		}

		public MemberSpriteBitmap CreateAsMember(int nTwipSize)
		{
			Bitmap bmp = this.RenderToBitmap(nTwipSize);
			if (bmp==null)
				return null;
			MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
			mb.Name = "Flash_"+this.Id.ToString();
			mb.RegPoint = this.Bounds.Location*-1; //.ToEPoint()
			return mb;
		}

		public Bitmap RenderToBitmap(int nTwipSize)
		{
			EPoint ptOffset;
			Bitmap bmp = Shape.RenderToBitmap(nTwipSize, this.CommandList, this.FillStyles, this.LineStyles, out ptOffset);
            if (bmp == null)
                return null;
			this.Bounds = new ERectangle(0,0,bmp.Width,bmp.Height);
			this.Bounds.Offset(ptOffset);
			//this.Bounds*=nTwipSize;
			return bmp;
		}

		public static Bitmap RenderToBitmap(int nTwipSize, ArrayList commandList, ArrayList fillStyles, ArrayList lineStyles, out EPoint ptOffset)
		{
			EPoint pntCurrentLoc = new EPoint();

			ArrayList pathInfos = new ArrayList();
			PathInfo pathInfoCurrent = new PathInfo();
			pathInfos.Add(pathInfoCurrent);

			bool hasDrawnSinceLastStyleChange = false;

			EPointF ptScale = new EPointF(1,1)/nTwipSize;

			string sDebug = "";

			foreach (ShapeCommand.Base cmd in commandList)
			{
				if (cmd.MovesTurtle)
				{
					if (cmd.Draws)
					{
						((ShapeCommand.Draw)cmd).AddToPath(pathInfoCurrent.Path, pntCurrentLoc, ptScale.X);
						if (Shape.Debug)
						{
							ArrayList pts = ((ShapeCommand.Draw)cmd).GeneratePoints(pntCurrentLoc);
							if (cmd is ShapeCommand.Curve)
								sDebug+="Curve";
							else
								sDebug+="Line";
							sDebug+="\r\n";

							foreach (EPointF pt in pts)
								sDebug+=pt.ToString()+"\r\n";
						}
						hasDrawnSinceLastStyleChange = true;
					}
					else
					{
						pathInfoCurrent.Path.CloseAllFigures();
						pathInfoCurrent.Path.StartFigure();
					}
					pntCurrentLoc = cmd.GetNewLoc(pntCurrentLoc);
				}
				else
				{
					if (hasDrawnSinceLastStyleChange)
					{
						pathInfoCurrent = new PathInfo();
						pathInfos.Add(pathInfoCurrent);
					}

					if (cmd is ShapeCommand.FillStyle)
					{
						ShapeCommand.FillStyle fs = (ShapeCommand.FillStyle)cmd;
						Brush brush = null;
						if (fs.StyleId > 0)
							brush = ((Style.FillStyle)fillStyles[fs.StyleId-1]).GetBrush();
						if (fs.Side == 0)
							pathInfoCurrent.Brush0 = brush;
						else
							pathInfoCurrent.Brush1 = brush;
					}
					else if (cmd is ShapeCommand.LineStyle)
					{
						ShapeCommand.LineStyle ls = (ShapeCommand.LineStyle)cmd;
						Pen pen = null;
						if (ls.StyleId > 0)
							pen = ((Style.LineStyle)lineStyles[ls.StyleId-1]).GetPen();
						pathInfoCurrent.Pen = pen;

						if (pen!=null)
							pen.Width*= ptScale.X;
					}

					hasDrawnSinceLastStyleChange = false;
				}
			}

			Matrix transform = new Matrix();
			transform.Scale(ptScale.X,ptScale.Y);

			ERectangle bounds = ERectangle.FromLTRB(99999,99999,-99999,-99999);
			foreach (PathInfo pathInfo in pathInfos)
			{
				//pathInfo.Path.Transform(transform);
				bounds.Expand(pathInfo.GetBounds().ToERectangle());
			}
			ptOffset = bounds.TopLeft;

			if (bounds.Width == 0 || bounds.Height == 0)
				return null;
			//this.Bounds = bounds;

			if (Shape.Debug)
				Endogine.Files.FileReadWrite.Write("__s.txt", sDebug);


			transform = new Matrix();
			transform.Translate(-bounds.X, -bounds.Y);
			foreach (PathInfo pathInfo in pathInfos)
				pathInfo.Path.Transform(transform);

			Bitmap bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            Endogine.BitmapHelpers.Canvas canvas = Endogine.BitmapHelpers.Canvas.Create(bmp);
            canvas.Locked = true;
            canvas.Fill(Color.FromArgb(0, 255, 255, 255));
            canvas.Dispose();
			Graphics g = Graphics.FromImage(bmp);
			g.SmoothingMode = SmoothingMode.HighQuality;
			foreach (PathInfo pathInfo in pathInfos)
			{
				if (pathInfo.Pen!=null)
					g.DrawPath(pathInfo.Pen, pathInfo.Path);
				if (pathInfo.Brush0!=null)
					g.FillPath(pathInfo.Brush0, pathInfo.Path);
				if (pathInfo.Brush1!=null) //TODO: can GDI+ handle Flash's two different fills (0 and 1)?
					g.FillPath(pathInfo.Brush1, pathInfo.Path);
			}

			return bmp;
		}

		public string WriteCommands(ArrayList cmds)
		{
			string s = "";
			foreach (ShapeCommand.Base cmd in cmds)
			{
				string name = cmd.GetType().ToString();
				name = name.Remove(0,name.LastIndexOf(".")+1) + ":"+cmd.ToString();
				s+=name+" ";
				s+="\r\n";
			}
			return s;
		}
	}
}
