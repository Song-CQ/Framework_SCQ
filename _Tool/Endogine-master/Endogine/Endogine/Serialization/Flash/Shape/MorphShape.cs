using System;
using System.Collections;
using System.Drawing;

namespace Endogine.Serialization.Flash.Shape
{
	/// <summary>
	/// Summary description for MorphShape.
	/// </summary>
	public class MorphShape : Shape
	{
		private float _morphPosition;
		private ArrayList _morphCommandList;

		public MorphShape()
		{
		}

		public override void Init(Record record)
		{
			base.CopyRecord (record); //wheww, this is ugly.

			BinaryFlashReader reader = record.GetDataReader();
			//I don't need these (start/end bounds):
			this.Id = reader.ReadUInt16();
			reader.ReadRect();
			reader.ReadRect();


			// Offset to EndEdges
			uint offset = reader.ReadUInt32();
			int posForShape2 = (int) (reader.BaseStream.Position + offset);

			this.FillStyles = reader.ReadFillStyleArray(true, true, true);
			this.LineStyles = reader.ReadLineStyleArray(true, true, true, false);

			this.CommandList = ReadShapeCommands(reader, this.FillStyles, this.LineStyles, true, true, true, false);

			if (posForShape2 > reader.BaseStream.Position)
				throw new Exception("Morph target error");
			//make sure we're at the right position
			reader.BaseStream.Position = posForShape2;

			this._morphCommandList = ReadShapeCommands(reader, this.FillStyles, this.LineStyles, true, true, true, false);

			//Style changes are defined only in start shape - copy them to end shape!
			object carry = null;
			EPoint ptCurrent = new EPoint();
			ArrayList corrected = new ArrayList();
			IEnumerator enTarget = this._morphCommandList.GetEnumerator();
			foreach (ShapeCommand.Base cmd in this.CommandList)
			{
				if (cmd.IsStyle)
					continue;
				
				if (carry!=null)
				{
					corrected.Add(carry);
					carry = null;
				}
				else
				{
					if (!enTarget.MoveNext())
						throw new Exception("Morph target is missing edges");

					ShapeCommand.Base cmdTo = (ShapeCommand.Base)enTarget.Current;
					if (cmd is ShapeCommand.Move)
					{
						if (cmdTo is ShapeCommand.Move)
							corrected.Add(cmdTo);
						else
						{
							carry = cmdTo;
							corrected.Add(new ShapeCommand.Move(ptCurrent));
						}
					}
					else
						corrected.Add(cmdTo);

					if (cmdTo is ShapeCommand.Move)
						ptCurrent = ((ShapeCommand.Move)cmdTo).GetNewLoc(ptCurrent);
					else if (cmdTo is ShapeCommand.Draw)
						ptCurrent = ((ShapeCommand.Draw)cmdTo).GetNewLoc(ptCurrent);
				}
			}
			this._morphCommandList = corrected;

			string s = this.WriteCommands(this._morphCommandList);
			Endogine.Files.FileReadWrite.Write("MS"+this.Id+".txt", s);

			this.InitDone();
		}

		public float MorphPosition
		{
			set
			{
				this._morphPosition = value;
				foreach (Style.FillStyle fs in this.FillStyles)
					fs.MorphPosition = value;
				foreach (Style.LineStyle ls in this.LineStyles)
					ls.MorphPosition = value;
			}
			get {return this._morphPosition;}
		}


		public Bitmap RenderToBitmap(int twipSize, float ratio, out EPoint ptOffset)
		{
			ArrayList morphedCommands = this.CreateMorphedShape(ratio);
			return Shape.RenderToBitmap(twipSize, morphedCommands, this.FillStyles, this.LineStyles, out ptOffset);
		}

		public ArrayList CreateMorphedShape(float ratio)
		{
			ArrayList morphedCommands = new ArrayList();
			int targetIndex=-1;
			EPoint ptCurrent = new EPoint();
			foreach (ShapeCommand.Base cmd in this.CommandList)
			{
				if (!(cmd is ShapeCommand.Style))
					targetIndex++;

				ShapeCommand.Base cmdTo = (ShapeCommand.Base)this._morphCommandList[targetIndex];

				if (cmd is ShapeCommand.Move)
				{
					EPoint pt1 = ((ShapeCommand.Move)cmd).Target;
					EPoint pt2 = ((ShapeCommand.Move)cmdTo).Target;
					ptCurrent = this.GetPointBetween(pt1,pt2,ratio);
					morphedCommands.Add(new ShapeCommand.Move(ptCurrent));
				}
				else if ((cmd is ShapeCommand.Line) || (cmd is ShapeCommand.Curve))
				{
					ShapeCommand.Draw draw = (ShapeCommand.Draw)cmd;
					ShapeCommand.Curve curve = draw.GetAsCurve();

					if ((cmdTo is ShapeCommand.Line) || (cmdTo is ShapeCommand.Curve))
					{
						draw = (ShapeCommand.Draw)cmdTo;
						ShapeCommand.Curve curveTo = draw.GetAsCurve();

						EPoint ptControl = this.GetPointBetween(curve.Control,curveTo.Control,ratio);
						EPoint ptAnchor = this.GetPointBetween(curve.Anchor,curveTo.Anchor,ratio);
					
						morphedCommands.Add(new ShapeCommand.Curve(ptControl, ptAnchor));
						ptCurrent+= ptControl + ptAnchor;
					}
					else
					{
						ShapeCommand.Move move = (ShapeCommand.Move)cmdTo;

						EPoint ptFrom = ptCurrent+curve.Control+curve.Anchor;
						EPoint ptTo = move.Target;
						
						ptCurrent = this.GetPointBetween(ptFrom, ptTo, ratio);
						morphedCommands.Add(new ShapeCommand.Move(ptCurrent.Copy()));
					}
				}
				else if (cmd is ShapeCommand.LineStyle)
				{
					ShapeCommand.LineStyle ls = (ShapeCommand.LineStyle)cmd;
					if (ls.StyleId > 0)
					{
						Style.LineStyle style = (Style.LineStyle)this.LineStyles[ls.StyleId-1];
						style.MorphPosition = ratio;
						morphedCommands.Add(ls); //.GetMorphed(ratio));
					}
				}
				else if (cmd is ShapeCommand.FillStyle)
				{
					ShapeCommand.FillStyle fs = (ShapeCommand.FillStyle)cmd;
					if (fs.StyleId > 0)
					{
						Style.FillStyle style = (Style.FillStyle)this.FillStyles[fs.StyleId-1];
						style.MorphPosition = ratio;
						morphedCommands.Add(fs); //.GetMorphed(ratio));
					}
				}
			}
			return morphedCommands;
		}

		private EPoint GetPointBetween(EPoint pt1, EPoint pt2, float ratio)
		{
			EPointF ptMid = (pt2-pt1).ToEPointF()*ratio + pt1.ToEPointF();
			return ptMid.ToEPoint();
		}
	}
}
