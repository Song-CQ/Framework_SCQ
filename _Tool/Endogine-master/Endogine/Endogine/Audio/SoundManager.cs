using System;
using System.Collections;
using System.Windows.Forms;

namespace Endogine.Audio
{
	/// <summary>
	/// Summary description for SoundManager.
	/// </summary>
	public class SoundManager
	{
		private ArrayList _sounds;
		protected string _defaultPath;
		protected string _soundFilenameIfNotFound;

		private static string _systemPrefix = "Endogine.Audio.";
		private static SoundManager _defaultSoundManager;
		protected ArrayList _supportedExtensions;
		protected string _defaultExtension;

		//protected Control _owner;

		public SoundManager(Control owner)
		{
			//this._owner = owner;
			this._sounds = new ArrayList();
			this._supportedExtensions = new ArrayList();
		}

		public virtual SoundStrategy CreateSoundStrategy()
		{
			return null;
		}

		public virtual void Dispose()
		{
			for (int i=this._sounds.Count-1; i>=0; i--)
				((Sound)this._sounds[i]).Dispose();
			this._sounds = null;
		}

		public void SoundCreated(Sound snd)
		{
			this._sounds.Add(snd);
		}
		public void SoundDisposed(Sound snd)
		{
			this._sounds.Remove(snd);
		}

		public ArrayList Sounds
		{
			get
			{
				ArrayList snds = new ArrayList();
				foreach (Sound snd in this._sounds)
					snds.Add(snd);
				return snds;
			}
		}

		public string DefaultSoundPath
		{
			get { return this._defaultPath;}
			set
			{
				this._defaultPath = value;
				if (value == null)
					return;
				if (!this._defaultPath.EndsWith("\\"))
					this._defaultPath+="\\"; //this._defaultPath.Remove(this._defaultPath.Length-1,1);
			}
		}
		/// <summary>
		/// If this is not set to null, the sound file specified will be used to play all sounds that weren't found
		/// </summary>
		public string SoundToUseIfNotFound
		{
			get {return this._soundFilenameIfNotFound;}
			set {this._soundFilenameIfNotFound = value;}
		}

		public virtual ArrayList SupportedExtensions
		{
			get {return null;}
		}

		public string FindSound(string sound)
		{
			//TODO: if no extension, check what formats the system supports (mp3, wav, ogg)
			//first check if fully qualified name
			string s = sound;
			if (sound.StartsWith("."))
			{
			}
			else
			{
				if (!Files.FileFinder.IsFullyQualified(sound))
				{
					if (sound.StartsWith("\\"))
						sound = sound.Remove(0,1);
					s = this._defaultPath + sound;
					if (!System.IO.File.Exists(s))
					{
						if (AppSettings.Instance!=null)
							s = AppSettings.Instance.FindFile(sound);
					}
				}
				if (s==null)
					s="";
			}

			if (System.IO.File.Exists(s))
				return s;

			if (this.SoundToUseIfNotFound!=null)
				return this.SoundToUseIfNotFound;
			throw new Exception("Sound file not found: "+ sound);
		}

		public static string[] GetAvailableSystems(string path)
		{
			string search = GetDllFilename(path, "*");

			System.IO.FileInfo[] files = Endogine.Files.FileFinder.GetFiles(search);
			string[] names = new string[files.Length];
			for (int i=0; i<files.Length; i++)
			{
				string name = files[i].Name;
				name = name.Remove(0,name.IndexOf(_systemPrefix)+_systemPrefix.Length);
				name = name.Remove(name.Length-4,4); //remove .dll
				names[i] = name;
			}
			return names;
		}

		private static string GetDllFilename(string path, string name)
		{
			if (path==null)
				path = System.IO.Directory.GetCurrentDirectory();
			if (!path.EndsWith("\\"))
				path+="\\";
			return path+_systemPrefix+name+".dll";
		}

		public static System.Reflection.Assembly GetAssembly(string name, string path)
		{
			string[] systems = GetAvailableSystems(null);
            if (systems.Length == 0)
                return null;
				//throw new Exception("No audio systems found!");

			if (name == null)
				name = (string)systems[0];

			System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFile(GetDllFilename(path, name));
			return ass;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="renderControl"></param>
		/// <returns></returns>
		public static SoundManager CreateSystem(string name, Control renderControl)
		{
			System.Reflection.Assembly ass = GetAssembly(name, null);
            if (ass == null)
                return null;

			string sFullname = ass.FullName.Substring(0,ass.FullName.IndexOf(","));
			Type scriptClass = ass.GetType(sFullname+".SoundManager");
            if (scriptClass == null)
                throw new Exception("SoundManager class not found in renderer " + name);

			System.Reflection.ConstructorInfo cons = scriptClass.GetConstructor(new Type[]{typeof(Control)});

            object o = null;
            try
            {
                o = cons.Invoke(new object[] { renderControl });
            }
            catch (Exception e)
            {
                if (sFullname.ToLower().Contains(".bass"))
                    throw new Exception("BASS audio needs some external .dlls. Download from http://codeproject.com/csharp/Endogine.asp");

                throw new Exception("Sound system failed to load: " + scriptClass.FullName + "\r\n(Inner: " + e.Message + ")");
            }
			SoundManager._defaultSoundManager = (SoundManager)o;
			SoundManager._defaultSoundManager.DefaultSoundPath = System.IO.Directory.GetCurrentDirectory();
			
			return (SoundManager)o;
		}

		public static SoundManager DefaultSoundManager
		{
			get
            {
                if (SoundManager._defaultSoundManager == null)
                    Endogine.Audio.SoundManager.CreateSystem(null, EH.Instance.Stage.RenderControl);
                return SoundManager._defaultSoundManager;
            }
			set {SoundManager._defaultSoundManager = value;}
		}
	}
}
