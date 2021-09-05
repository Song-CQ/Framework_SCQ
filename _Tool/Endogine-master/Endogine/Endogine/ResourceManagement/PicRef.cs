using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Endogine
{
	/// <summary>
	/// A wrapper for image information. No bitmap is loaded until the member is asked for.
	/// </summary>
	public class PicRef
	{
		public delegate void ChangedDelegate();
		public event ChangedDelegate Changed;

		private MemberSpriteBitmap _mb;
		private ERectangle _sourceRect;
		private string _originalName;
		private EPoint _offset;


        #region Static stuff, management
        
        static Dictionary<string, string> _originalsToMerged; //file names of original files (before merging/tiling), matched to the merged (tiled) file names
        static Dictionary<string, PicRef> _pictures;
        static int _numCreated;

        private static void Prepare()
        {
            if (_originalsToMerged == null)
                _originalsToMerged = new Dictionary<string, string>();
            if (_pictures == null)
                _pictures = new Dictionary<string, PicRef>();
        }

        public static void ScanMediaDirectories()
        {
            //Look for files ending with .xml in all Media paths
            string[] paths = Endogine.AppSettings.Instance["Paths.Media"];

            if (paths != null)
            {
                foreach (string path in paths)
                    PicRef.ScanDirectory(path);
            }
        }

		public static void ScanDirectory(string path)
		{
            PicRef.Prepare();

            if (!Endogine.Files.FileFinder.ContainsRoot(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (!di.Exists)
                {
                    di = new DirectoryInfo(Endogine.AppSettings.BaseDirectory + "\\" + path);
                    if (!di.Exists)
                        throw new Exception("ScanDirectory: "+path+" doesn't exists");
                }
                path = di.FullName;
            }
            if (!path.EndsWith("\\"))
                path += "\\";

			FileInfo[] files = Endogine.Files.FileFinder.GetFiles(path+"*.xml");
			foreach (FileInfo file in files)
			{
				//Find files with the same name, but a different file extension - those are probably media files.
				//TODO: Not only bitmaps - this goes for all types of media, they come in media+resource pairs.
				string mediaFile = file.FullName.Remove(file.FullName.Length-3,3) + "*";
				FileInfo[] probableMediaFiles = Endogine.Files.FileFinder.GetFiles(mediaFile);

				foreach (FileInfo probableFile in probableMediaFiles)
				{
					if (probableFile.Name.EndsWith("xml"))
						continue;
					PicRef.LoadPicRef(probableFile.FullName);
					break;
				}
			}

            FileInfo[] filesGif = Endogine.Files.FileFinder.GetFiles(path+"*.gif");
            foreach (FileInfo file in filesGif)
            {
                //TODO: check if already loaded (had xml fork)
                PicRef.LoadPicRef(file.FullName);
            }
		}

		public static void LoadPicRef(string bitmapFilename)
		{
			FileInfo bitmapFile = new FileInfo(bitmapFilename);

            PicRef.Prepare();

			if (_originalsToMerged.ContainsValue(bitmapFile.FullName))
				return;
//				throw new Exception("This tiled bitmap has already been loaded: "+bitmapFile.FullName);

			//For now, load the bitmap immediately. The retrieval process for each PicRef seems to be very slow.
			MemberSpriteBitmap mb = new MemberSpriteBitmap(bitmapFilename);

			XmlDocument doc = new XmlDocument();
			string xmlFile = bitmapFile.FullName.Substring(0, bitmapFile.FullName.Length-bitmapFile.Extension.Length) + ".xml";
            if (File.Exists(xmlFile))
            {
                doc.Load(xmlFile);
                if (doc["root"]["Files"] != null)
                {
                    List<string> fileOrder = new List<string>();
                    foreach (XmlNode node in doc["root"]["Files"])
                    {
                        string originalFile = node.Attributes["value"].InnerXml;
                        fileOrder.Add(originalFile);
                        //remove file extension:
                        int index = originalFile.LastIndexOf(".");
                        if (index > 0)
                            originalFile = originalFile.Remove(index, originalFile.Length - index);
                        //							if (_originalsToMerged.Contains(originalFile)
                        //if (originalFile == "object01_0000")
                        //	EH.Put("Jks");
                        //_originalsToMerged.Add(originalFile, bitmapFile.FullName);

                        PicRef picture = new PicRef(originalFile, mb);
                        picture.SourceRectangle = new ERectangle(node["Rect"].Attributes["value"].InnerXml);
                        picture.Offset = new EPoint(node["Offset"].Attributes["value"].InnerXml);
                    }

                    //TODO: ATM, nothing is done with the animation information in here.
                    //Should be stored in a global animation collection -
                    //accessed by [
                    if (doc["root"]["Animations"] != null)
                    {
                        foreach (XmlNode node in doc["root"]["Animations"])
                        {
                            System.Collections.ArrayList anim = Endogine.Animation.AnimationHelpers.ParseAnimationString(node.Attributes["value"].InnerXml);
                            List<string> animPics = new List<string>();
                            foreach (int frame in anim)
                                animPics.Add(fileOrder[frame]);
                            EH.Instance.CastLib.FrameSets.AddFrameSet(node.Name, animPics);
                        }
                    }
                }
            }
		}

		public static PicRef Get(string originalName)
		{
            if (PicRef._pictures.ContainsKey(originalName))
    			return PicRef._pictures[originalName];
            return null;
		}

		public static MemberSpriteBitmap GetMemberForFile(string file)
		{
            if (!PicRef._originalsToMerged.ContainsKey(file))
                return null;
			string mergedFile = PicRef._originalsToMerged[file];
			return (MemberSpriteBitmap)EH.Instance.CastLib.GetOrCreate(mergedFile);
		}

		public static void AddPicture(PicRef p)
		{
            PicRef.Prepare();
			PicRef._pictures.Add(p.OriginalName, p);
            PicRef._originalsToMerged.Add(p.OriginalName, p.Member.Name);
		}

        public static List<PicRef> CreatePicRefs(string fileName, int numFramesOnX, int numFramesTotal)
        {
            if (fileName.IndexOf("\\") > 0)
                fileName = fileName.Remove(0, fileName.LastIndexOf("\\")+1);
            int index = fileName.LastIndexOf(".");
            if (index > 0)
                fileName = fileName.Remove(index);

            MemberSpriteBitmap mb = (MemberSpriteBitmap)EH.Instance.CastLib.GetOrCreate(fileName);
            return PicRef.CreatePicRefs(mb, numFramesOnX, numFramesTotal);
        }
        public static List<PicRef> CreatePicRefs(Endogine.ResourceManagement.MemberBitmapBase mb, int numFramesOnX, int numFramesTotal)
        {
            List<string> animRefs = new List<string>();
            List<PicRef> picRefs = new List<PicRef>();
            int numFramesOnY = numFramesTotal / numFramesOnX;
            EPoint frameSize = new EPoint(mb.Size.X / numFramesOnX, mb.Size.Y / numFramesOnY);
            for (int i = 0; i < numFramesTotal; i++)
            {
                string picRefName = mb.Name + "_" + i;
                int x = i % numFramesOnX;
                int y = i / numFramesOnX;
                PicRef pr = PicRef.Create((MemberSpriteBitmap)mb, picRefName);
                pr.SourceRectangle = new ERectangle(x * frameSize.X, y * frameSize.Y, frameSize.X, frameSize.Y);
                picRefs.Add(pr);
                animRefs.Add(picRefName);
            }

            EH.Instance.CastLib.FrameSets.AddFrameSet(mb.Name, animRefs);

            return picRefs;
        }

        public static PicRef Create(string fileName)
        {
            MemberSpriteBitmap mb = (MemberSpriteBitmap)EH.Instance.CastLib.GetOrCreate(fileName);
            return PicRef.Create(mb, fileName);
        }
        public static PicRef Create(MemberSpriteBitmap mb, string name)
        {
            PicRef pr = new PicRef(name, (MemberSpriteBitmap)mb);
            pr.Offset = new EPoint();
            return pr;
        }

        public static void SetRegPoints(List<PicRef> picRefs, EPoint pnt)
        {
            foreach (PicRef p in picRefs)
                p.Offset = pnt;
        }
        public static void CenterRegPoints(List<PicRef> picRefs)
        {
            foreach (PicRef p in picRefs)
                p.Offset = p.SourceRectangle.Size/2;
        }

        public static PicRef GetOrCreate(string name)
        {
            PicRef pr = PicRef.Get(name);
            if (pr != null)
                return pr;
            return PicRef.Create(name);
        }
        #endregion



        //public PicRef(string fileName)
        //{
        //    PicRef.LoadPicRef(fileName);
        //}

        public PicRef(System.Drawing.Bitmap bmp, string name)
        {
            MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
            this.PostConstructor(name);
            mb.Name = this._originalName;
            this._mb = mb;
        }

        public PicRef(string originalName, MemberSpriteBitmap mb)
		{
            this._mb = mb;
            this.PostConstructor(originalName);
		}

		private void PostConstructor(string originalName)
		{
            PicRef._numCreated++;
            if (originalName == null)
                originalName = "Unnamed" + PicRef._numCreated;
			this._originalName = originalName;
			this._offset = new EPoint();
			this._sourceRect = new ERectangle();
            PicRef.AddPicture(this);
		}

        //public static PicRef FromOriginalName(string originalName)
        //{
        //    return PicRef.Get(originalName);
        //}

		public MemberSpriteBitmap Member
		{
			get
			{
				if (_mb == null)
					_mb = PicRef.GetMemberForFile(_originalName);
				return _mb;
			}
			set
			{
				_mb = value;
                _sourceRect = new ERectangle(0, 0, _mb.Size.X, _mb.Size.Y); //TotalSize
				if (Changed!=null)
					Changed();
			}
		}

		public string OriginalName
		{
			get {return _originalName;}
		}

		public EPoint Offset
		{
			get {return this._offset;}
			set
			{
				this._offset = value;
				if (Changed!=null)
					Changed();
			}
		}

		public ERectangle SourceRectangle
		{
			get {return _sourceRect;}
			set {_sourceRect = value;}
		}

	}
}
