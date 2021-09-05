using System;
using System.Collections;
using System.IO;

using System.Xml;

namespace Endogine.Serialization.Flash
{
	//Also check http://www.codeproject.com/useritems/flashexternalapi.asp

	/// <summary>
	/// Summary description for Flash.
	/// </summary>
	public class Flash
	{
		public enum Tags
		{
			End=0,
			ShowFrame=1,
			DefineShape=2,
			PlaceObject=4,
			RemoveObject=5,
			DefineBits=6,
			DefineButton=7,
			JPEGTables=8,
			SetBackgroundColor=9,
			DefineFont=10,
			DefineText=11,
			DoAction=12,
			DefineFontInfo=13,
			DefineSound=14,
			StartSound=15,
			DefineButtonSound=17,
			SoundStreamHead=18,
			SoundStreamBlock=19,
			DefineBitsLossless=20,
			DefineBitsJPEG2=21,
			DefineShape2=22,
			DefineButtonCxform=23,
			Protect=24,
			PlaceObject2=26,
			RemoveObject2=28,
			DefineShape3=32,
			DefineText2=33,
			DefineButton2=34,
			DefineBitsJPEG3=35,
			DefineBitsLossless2=36,
			DefineEditText=37,
			DefineSprite=39,
			FrameLabel=43,
			SoundStreamHead2=45,
			DefineMorphShape=46,
			DefineFont2=48,
			ExportAssets=56,
			ImportAssets=57,
			EnableDebugger=58,
			DoInitAction=59,
			DefineVideoStream=60,
			VideoFrame=61,
			DefineFontInfo2=62,
			EnableDebugger2=64,
			ScriptLimits=65,
			SetTabIndex=66,
			DefineShape4=67,
			FileAttributes=69, //http://wiki.jswiff.com/Wiki.jsp?page=FileAttributes
			PlaceObject3=70,
			ImportAssets2=71,
			DefineFontInfo3=73,  //http://wiki.jswiff.com/Wiki.jsp?page=DefineFontInfo3
			DefineTextInfo=74,
			DefineFont3=75, //http://wiki.jswiff.com/Wiki.jsp?page=DefineFont3
			Metadata=77, //http://wiki.jswiff.com/Wiki.jsp?page=Metadata
			ScalingGrid=78, //http://wiki.jswiff.com/Wiki.jsp?page=ScalingGrid
			DefineShape5=83,
			DefineMorphShape2=84
		}

		private BinaryFlashReader reader;
		// http://www.amanith.org/blog/index.php
		// http://sswf.sourceforge.net/SWFalexref.html
		//private Hashtable _shapesAsMembers;
		public Hashtable Members;
		private SortedList _objectByDepth;
		public Hashtable Characters;
		public Hashtable CharacterIdsByDepth;
		public ArrayList AllReadObjects;
		public int TwipSize = 20;
		private int _executedRecordIndex; //
		private byte[] _jpegTables;
		private long _nextUpdateTime;
		public float FrameRate;

		public Flash(string a_sFilename)
		{
			this.Read(a_sFilename);
			this.Render();
		}

		public void Read(string a_sFilename)
		{
			FileStream stream = new FileStream(a_sFilename,
				FileMode.Open, FileAccess.Read);
			this.reader = new BinaryFlashReader(stream);

			//Header
			//Compressed or uncompressed?
			bool bCompressed = (this.reader.ReadChar().ToString() == "C")?true:false;

			//char 2-3 is always WS:
			string sWS = new string(this.reader.ReadChars(2));
			if (sWS != "WS")
				throw(new Exception("Not a Flash file"));

			//Testing: I'm writing some data to an XML doc in order to see what the parser's results are
			//TODO: instead, the whole Flash data structure (after parsing) should be serializable to XML
			//I.e. a Flash->XML->Flash converter is a must later on. Or preferably XAML.
//			XmlDocument xdoc = new XmlDocument();
//			XmlNode xnode = xdoc.CreateElement("root");
//			xdoc.AppendChild(xnode);

			int Version = this.reader.ReadByte();

//			this.AddNodeAndValue(xnode, "Version", Version.ToString());

			long nFileLength = this.reader.ReadUInt32();

			//rect for the bounding rect of the whole Flash file:
			ERectangle boundingRect = this.reader.ReadRect();

//			this.AddNodeAndValue(xnode, "BoundingRect", boundingRect.ToString());

			this.FrameRate = this.reader.ReadFixed16();
			int FrameCount = this.reader.ReadUInt16();

//			this.AddNodeAndValue(xnode, "FrameRate", FrameRate.ToString());
//			this.AddNodeAndValue(xnode, "FrameCount", FrameCount.ToString());


			this.AllReadObjects = new ArrayList();
			this.Characters = new Hashtable();
			this.Members = new Hashtable();

			//read tag by tag until end of file:
			while (true)
			{
				Record record = new Record(reader, this);
				Record newRecord = null;
				switch (record.Tag)
				{
					case Tags.SetBackgroundColor:
						EH.Instance.Stage.Color = record.GetDataReader().ReadRGB();
						break;

					case Tags.ShowFrame:
						newRecord = record;
						break;
					
					case Tags.DefineBits:
					case Tags.DefineBitsJPEG2:
					case Tags.DefineBitsJPEG3:
					case Tags.DefineBitsLossless:
					case Tags.DefineBitsLossless2:
						if (record.Tag == Tags.DefineBits)
							newRecord = new Shape.Image(this._jpegTables);
						else
							newRecord = new Shape.Image();
						break;
					case Tags.JPEGTables:
						if (this._jpegTables == null)
						{
							BinaryFlashReader subReader = record.GetDataReader();
							this._jpegTables = subReader.ReadBytes((int)record.TagLength - 2);
							if (subReader.ReadUInt16() != 0xd9ff)
								throw new Exception("JPEG error");
						}
						break;

				
					case Tags.DefineFont:
						newRecord = new Text.Font();//record
						break;
					case Tags.DefineFontInfo:
						break;
					case Tags.DefineText2:
						newRecord = new Text.Text();//record
						break;

					case Tags.DefineShape:
					case Tags.DefineShape2:
					case Tags.DefineShape3:
					case Tags.DefineShape4:
					case Tags.DefineShape5:
						newRecord = new Shape.Shape();
						break;

					case Tags.DefineMorphShape:
						newRecord = new Shape.MorphShape();
						break;

					case Tags.PlaceObject:
					case Tags.PlaceObject2:
						newRecord = new Placement.Placement();
						break;

					case Tags.RemoveObject:
					case Tags.RemoveObject2:
						newRecord = new Placement.Remove();
						break;

					case Tags.DoAction:
					case Tags.DoInitAction:
						//newRecord = new Action.DoAnAction();
						break;
				}

				if (newRecord!=null)
				{
					if (!newRecord.Inited)
						newRecord.Init(record);
					this.AllReadObjects.Add(newRecord);
				}

				if (record.Tag == Tags.End)
					break;
				if (this.reader.BaseStream.Position >= nFileLength-1)
					break;
			}

			//xdoc.Save(a_sFilename+".xml");
			//zoom in:
			//this.TwipSize = 1; //20
			this._executedRecordIndex = -1;
			this._objectByDepth = new SortedList();
			this.CharacterIdsByDepth = new Hashtable();
			EH.Instance.EnterFrameEvent+=new EnterFrame(Instance_EnterFrameEvent);
		}

		public void Render()
		{
			if (this._executedRecordIndex >= this.AllReadObjects.Count-1)
			{
				return;
                //For looping:
                //foreach (DictionaryEntry de in this._objectByDepth)
                //    ((Sprite)de.Value).Dispose();
                //this._objectByDepth.Clear();
                //this._executedRecordIndex = 0; //loop from start
			}

			//EH.Put("New frame!" + this._executedRecordIndex.ToString() + "/"+this.AllReadObjects.Count.ToString());

			ArrayList newSprites = new ArrayList();
			ArrayList removedSprites = new ArrayList();
			//loop through the parsed flash file:
			while (this._executedRecordIndex < this.AllReadObjects.Count-1)
			{
				this._executedRecordIndex++;
				Record record = (Record)this.AllReadObjects[this._executedRecordIndex];
				//EH.Put("Tag: "+record.Tag.ToString());

				if (record is Placement.Placement)
				{
					// like "put member in sprite on stage"
					Placement.Placement placement = (Placement.Placement)record;
					Sprite spExisting = (Sprite)this._objectByDepth[placement.Depth];
					Sprite sp = placement.Execute(this, spExisting);
					if (spExisting==null)
					{
						this._objectByDepth.Add(placement.Depth, sp);
						newSprites.Add(spExisting);
					}
				}
				else if (record is Placement.Remove)
				{
					Placement.Remove remove = (Placement.Remove)record;
					if (!this._objectByDepth.Contains(remove.Depth))
						EH.Put("Removing nonexistent sprite?");
					else
					{
						Sprite sp = (Sprite)this._objectByDepth[remove.Depth];
						this._objectByDepth.Remove(remove.Depth);
						removedSprites.Add(sp);
					}
				}
				else if (record.Tag == Tags.SetBackgroundColor)
				{}
				else if (record.Tag == Tags.ShowFrame)
					break;
			}
//			foreach (Sprite sp in newSprites)
//				sp.Visible = true;
			foreach (Sprite sp in removedSprites)
				sp.Dispose();

			float msecsPerFrame = 1000f/this.FrameRate;
			//float msecsPerFrame = 300; //;
			this._nextUpdateTime = DateTime.Now.Ticks + (int)(msecsPerFrame*1000*10);
		}

		private XmlNode AddNodeAndValue(XmlNode parentNode, string nodeName, string nodeValue)
		{
			XmlNode newNode = parentNode.OwnerDocument.CreateElement(nodeName);
			parentNode.AppendChild(newNode);
			newNode.AppendChild(parentNode.OwnerDocument.CreateTextNode(nodeValue));
			return newNode;
		}

		private void Instance_EnterFrameEvent()
		{
			if (DateTime.Now.Ticks > this._nextUpdateTime)
				this.Render();
		}
	}
}
