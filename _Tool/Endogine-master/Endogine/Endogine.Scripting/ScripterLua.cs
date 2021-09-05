//using System;
//
//namespace Endogine.Scripting
//{
//	/// <summary>
//	/// Summary description for ScripterLua.
//	/// </summary>
//	public class ScripterLua
//	{
//		private string _path;
//		private Hashtable _htScripts;
//
//		static Lua _lua;
//
//		public ScripterLua()
//		{
//			if (_lua==null)
//			{
//				_lua = new Lua();
//				_lua.OpenMathLib();
//				_lua.OpenBaseLib();
//				_lua.OpenStringLib();
//				_lua.OpenIOLib();
//				_lua.OpenLoadLib();
//			}
//
//			//			lua.RegisterFunction("test", this, this.GetType().GetMethod("CalledByLua"));
//
//			this._path = Endogine.AppSettings.Instance.GetNode("Scripting").FirstChild.Name;;
//
//			string sInitScript = PreprocessLuaFile(this._path, "YE.lua");
//			_lua.DoString(sInitScript);
//
//			this._htScripts = new Hashtable();
//		}
//
//		public static string PreprocessLuaFile(string path, string file)
//		{
//			string sScript = Endogine.Files.FileReadWrite.Read(path + file);
//			if (true)
//			{
//				while (true)
//				{
//					System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(sScript, "include \"");
//					if (m.Success)
//					{
//						int nStart = m.Index + m.Length;
//						int nEnd = sScript.IndexOf("\"", nStart);
//						string sIncludeFile = sScript.Substring(nStart, nEnd-nStart);
//
//						string s = Endogine.Files.FileReadWrite.Read(path + sIncludeFile);
//						sScript = sScript.Remove(m.Index, nEnd-m.Index+1);
//						sScript = sScript.Insert(m.Index, s);
//					}
//					else
//						break;
//				}
//			}
//
//			return sScript;
//		}
//
//		public static string PreprocessLuaFileToFile(string path, string file)
//		{
//			string sScript = PreprocessLuaFile(path, file);
//			string outputFile = path + "_m_" + file;
//			Endogine.Files.FileReadWrite.Write(outputFile, sScript);
//			return outputFile;
//		}
//
//		public object Execute(string code)
//		{
//			return _lua.DoString(code);
//		}
//		public object ExecuteFile(string name)
//		{
//			string sFile = (string)this._htScripts[name];
//			return _lua.DoFile(sFile);
//		}
//
//
//		public void Load(string name)
//		{
//			string sFile = PreprocessLuaFileToFile(this._path, name+".lua");
//			this._htScripts.Add(name, sFile);
//		}
//	}
//}
