using System;
using System.IO;

namespace Endogine
{
	/// <summary>
	/// Keeps track of paths and stuff, accessed by IDs.
	/// There's one setup for dev, one for prod, and one for common settings
	/// Common is merged into both dev and prod.
	/// </summary>
	public class AppSettings
	{
		private static AppSettings _instance;
		private Node _root;

		public AppSettings()
		{
			_instance = this;

			string config;
			try
			{
				System.IO.StreamReader rd = new System.IO.StreamReader(AppSettings.BaseDirectory+"\\App.cfg");
				config = rd.ReadToEnd();
				rd.Close();
			}
			catch
			{
				config = @"
Dev
	Defines
		proj=..\..\#exe#
Prod
	Defines
		proj=#exe#
All
	Media
		#exe#Media";
			}

			this._root = this.Load(AppSettings.RunMode, config);

			string[] paths = this["Media"];
		}

		/// <summary>
		/// The project path when running from IDE (when exe is in either Debug or Release) - otherwise the exe path
		/// </summary>
		public static string BaseDirectory
		{
			get
			{
				System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(EH.Instance.ApplicationPath).Parent;
				if (dirInfo.Name == "Debug" || dirInfo.Name == "Release")
					return dirInfo.Parent.Parent.FullName;
				return dirInfo.FullName;
			}
		}

		public static string RunMode
		{
			get
			{
				System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(EH.Instance.ApplicationPath).Parent;
				if (dirInfo.Name == "Debug" || dirInfo.Name == "Release")
					return "Dev";
				return "Prod";
			}
		}
		public static AppSettings Instance
		{
			get {return _instance;}
		}

		public Node Load(string runmode, string config)
		{
			//a table for replacing #something# with a stored value (eg #exe# -> C:\docs\)
			System.Collections.Hashtable replacements = new System.Collections.Hashtable();
			replacements.Add("exe", EH.Instance.ApplicationDirectory);

			Node node = Node.FromTabbed(config);

			Node allNode = node["All"];
			node = node[runmode];
			if (node == null)
				node = new Node();
			
			if (allNode!=null)
				node.Merge(allNode);

			Node n = node;
			while(true)
			{
				n = n.NextSpecial;
				if (n==null)
					break;
				if (n.Name.IndexOf("#") >= 0)
				{
					System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(n.Name, @"\#\w+\#");
					string sKey = m.Value.Substring(1,m.Value.Length-2);
					string sValue = (string)replacements[sKey];
					if (sValue!=null)
					{
						string sNewName = System.Text.RegularExpressions.Regex.Replace(n.Name, @"\#\w+\#", sValue);
						n.Name = sNewName;
					}
				}
				if (n.Name.IndexOf(@"..\") >= 0)
				{
					int nFirst = n.Name.IndexOf(@"..\");
					string sPath = n.Name.Remove(0,nFirst);

					System.Text.RegularExpressions.MatchCollection ms = System.Text.RegularExpressions.Regex.Matches(
						sPath, @"[.][.]\\");

					sPath = sPath.Replace(@"..\", "");
					System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sPath);
					for (int i=0; i<ms.Count;i++)
						di = di.Parent;

					n.Name = n.Name.Substring(0,nFirst) + di.FullName;
				}

				if (n.Name.IndexOf("=") >= 0)
				{
					string[] sKV = n.Name.Split("=".ToCharArray());
					replacements[sKV[0]] = sKV[1];
				}
			}

//			string sTabbed = node.ToTabbed();
//			sTabbed = sTabbed.Replace("\n", "\r\n");
//			System.IO.StreamWriter wr = new System.IO.StreamWriter("Test.txt");
//			wr.Write(sTabbed);
//			wr.Flush();
//			wr.Close();

			return node;
		}

        public string GetPath(string XPath)
        {
            Node node = this._root["Paths."+XPath];
            if (node == null)
                return null;
            if (node.ChildNodes.Count > 0)
                return node[0].Name;
            return node.Text;
        }
        public void AddPath(string XPath, string val)
        {
            Node node = this.GetNode("Paths."+XPath);
            if (node == null)
                return;
            if (!val.EndsWith("\\"))
                val += "\\";
            node.AppendChild(val);
        }

		public Node GetNode(string XPath)
		{
			return this._root[XPath];
		}

		/// <summary>
		/// Returns same as GetNode().Text if it exists.
		/// </summary>
		/// <param name="XPath"></param>
		/// <returns></returns>
		public string GetNodeText(string XPath)
		{
			Node node = this._root[XPath];
			if (node==null)
				return null;
			return node.Text;
		}


		/// <summary>
		/// Gets all the child node names of the requested XPath node
		/// </summary>
		public string[] this [string XPath]
		{
			get
			{
				Node node = this._root[XPath];
				if (node == null)
					return null;

				if (!node.HasChildNodes)
					return null;

				string[] entries = new string[node.ChildNodes.Count];
				for (int i = 0; i < node.ChildNodes.Count; i++)
					entries[i] = ((Node)node.ChildNodes.GetByIndex(i)).Name;

				return entries;
			}
		}

		public string Resolve(string input)
		{
			if (input.IndexOf("#") >= 0)
			{
				System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(input, @"\#\w+\#");
				string sKey = m.Value.Substring(1,m.Value.Length-2);
				string[] values = this[sKey];

				if (values!=null)
					input = System.Text.RegularExpressions.Regex.Replace(input, @"\#\w+\#", values[0]);
			}
			if (input.IndexOf(@"..\") >= 0)
			{
				int nFirst = input.IndexOf(@"..\");
				string sPath = input.Remove(0,nFirst);

				System.Text.RegularExpressions.MatchCollection ms = System.Text.RegularExpressions.Regex.Matches(
					sPath, @"[.][.]\\");

				sPath = sPath.Replace(@"..\", "");
				System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sPath);
				for (int i=0; i<ms.Count;i++)
					di = di.Parent;

				input = input.Substring(0,nFirst) + di.FullName;
			}
			return input;
		}

		/// <summary>
		/// Finds a file. If a fully qualified path is supplied, it does nothing.
		/// Looks first in default castlib folder, then in application folder.
		/// If no extension is provided, the first found file which matches will be returned.
		/// TODO: could add several alternative paths to look in
		/// </summary>
		/// <param name="a_sFile">File name or path</param>
		public string FindFile(string a_sFile)
		{
			if (Files.FileFinder.IsFullyQualified(a_sFile))
			{
				a_sFile = Files.FileFinder.GetFirstMatchingFile(a_sFile);
				if (a_sFile!=null)
					return a_sFile;
			}
			else if (a_sFile.IndexOf("#")>=0)
			{
				a_sFile = AppSettings.Instance.Resolve(a_sFile);
				a_sFile = Files.FileFinder.GetFirstMatchingFile(a_sFile);
				if (a_sFile!=null)
					return a_sFile;
			}
			else
			{
				string[] paths = AppSettings.Instance["Paths.Media"];
				
				foreach (string path in paths)
				{
					string fileName = Files.FileFinder.GetFirstMatchingFile(path + a_sFile);
					if (fileName!=null)
						return fileName;
				}
			}

			return "";
		}


//Dev
//	Defines
//		proj=..\..\#exe#
//	Sound
//		#exe#Media
//Prod
//	Defines
//		proj=#exe#
//	Sound
//		#proj#\dd
//All
//	Media
//		#exe#Media
	}
}
