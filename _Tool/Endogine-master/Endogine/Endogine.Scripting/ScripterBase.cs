using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Endogine.Scripting
{
	/// <summary>
	/// Summary description for ScripterBase.
	/// </summary>
	abstract public class ScripterBase
	{
		protected ArrayList _referencedAssemblies;
		protected string _defaultContext = null;
		protected Hashtable _classToAssembly;
		protected Hashtable _scriptTemplates;
		protected string _fileExtension = "";

		protected Hashtable _instanceIdToObject;
		protected Hashtable _instanceIdToClass;

		public ScripterBase()
		{
			this._referencedAssemblies = new ArrayList();
			this._classToAssembly = new Hashtable();
			this._scriptTemplates = new Hashtable();
		}

		public string FileExtension
		{
			get {return this._fileExtension;}
			set {this._fileExtension = value;}
		}

		public string PreprocessingMarker
		{
			set {}
			get {return "##";}
		}

		public string Preprocess(string code, Hashtable injections)
		{
			code = this.Inject(code, injections);
			code = this.MakeReplacements(code);
			code = this.ProcessMacros(code);
			return code;
		}

		public void SetTemplate(string name, string code)
		{
			if (this._scriptTemplates.Contains(name))
				this._scriptTemplates.Remove(name);
			this._scriptTemplates.Add(name, code);
		}
		public string GetTemplate(string name)
		{
			return (string)this._scriptTemplates[name];
		}

		public string Inject(string code, Hashtable injections)
		{
			MatchCollection ms = Regex.Matches(code, @"\#\#INJECT.+[^#]");
			for (int matchNum=ms.Count-1; matchNum>=0; matchNum--)
			{
				Match m = Regex.Match(ms[matchNum].Value, @"\s\w+[^#]");
				string key = m.Value.Remove(0,1);
				
				string val = (string)injections[key];
				if (val == null)
					val = "";
				
				code = this.InsertWithSameIndentation(code, ms[matchNum].Index, ms[matchNum].Value, val);
//				int index = code.LastIndexOf("\n", ms[matchNum].Index);
//				string startOfLine = code.Substring(index+1, ms[matchNum].Index-index-1);
//				string wholeLine = startOfLine + ms[matchNum].Value;
//
//				//if injection is  the first thing on that line, match the number of tabs before it
//				Match mNonWhitespaces = Regex.Match(startOfLine, @"\w");
//				if (mNonWhitespaces.Success == true) // && false)
//				{
//					//Fuck, why doesn't this work...
//					code = code.Replace(ms[matchNum].Value, val); //+"\r\n");
//				}
//				else
//				{
//					//Find how many tabs are used:
//					MatchCollection msTabs = Regex.Matches(
//						startOfLine, @"\t");
//					int numTabs = msTabs.Count;
//					if (numTabs > 0)
//					{
//						string[] lines = val.Split("\r\n".ToCharArray());
//						//only add so many tabs that are really needed. If there are already tabs at the beginning of first line,
//						//remove that number from the number to add.
//						msTabs = Regex.Matches(lines[0], @"\t");
//						numTabs-=msTabs.Count;
//						string tabs = "";
//						for (int i=0; i<numTabs; i++)
//							tabs+="\t";
//
//						val = "";
//						foreach (string line in lines)
//						{
//							if (line.Trim().Length == 0)
//								continue;
//							val+=tabs+line+"\r\n";
//						}
//					}
//					code = code.Replace(wholeLine, val+"\r\n");
//				}

			}
			if (code.EndsWith("##"))
				code = code.Substring(0,code.Length-2);

			return code;
		}

		private string InsertWithSameIndentation(string code, int snippetStart, string snippetString, string insertString)
		{
			//		ScriptName = ##INJECT ClassName##

			//		##MACRO whileActiveYield
			//		#while Active and <args>:
			
			//find beginning of line where it starts:
			int lineStartIndex = code.LastIndexOf("\n", snippetStart);
			string startOfLine = code.Substring(lineStartIndex+1, snippetStart-lineStartIndex-1);

			int lineEndIndex = code.IndexOf("\n", lineStartIndex+1);
			if (lineEndIndex == -1)
				lineEndIndex = code.Length-1;
			string wholeLine = code.Substring(lineStartIndex+1, lineEndIndex-lineStartIndex);

			//Find where first word starts:
			Match mNonWhitespaces = Regex.Match(startOfLine, @"\w");
			string whitespacesAtStart = startOfLine;
			if (mNonWhitespaces.Success)
			{
				int firstWordStart = mNonWhitespaces.Index;
				whitespacesAtStart = code.Substring(lineStartIndex+1, firstWordStart);
			}

			//Find how many tabs are used:
			MatchCollection msTabs = Regex.Matches(whitespacesAtStart, @"\t");
			int numTabs = msTabs.Count;

			string[] lines = insertString.Split("\r\n".ToCharArray());

			//only add so many tabs that are really needed. If there are already tabs at the beginning of first line,
			//remove that number from the number to add.
			msTabs = Regex.Matches(lines[0], @"\t");
			numTabs-=msTabs.Count;
			string tabs = "";
			for (int i=0; i<numTabs; i++)
				tabs+="\t";

			insertString = "";
	
			int nLineIndex = 0;
			foreach (string line in lines)
			{
				if (nLineIndex++ == 0)
					insertString+=startOfLine + line.Trim()+"\r\n";
				else
				{
					if (line.Trim().Length == 0)
						continue;
					insertString+=tabs+line+"\r\n";
				}
			}
			
			//string xxx = code.Substring(lineStartIndex+1, wholeLine.Length);
			code = code.Remove(lineStartIndex+1, wholeLine.Length);
			code = code.Insert(lineStartIndex+1, insertString+"\r\n");

			//code = code.Replace(startOfLine + snippetString, insertString+"\r\n"); //wholeLine
			
			return code;
		}

		public string MakeReplacements(string code)
		{
			//Search and replace ##REPLACE ...## stuff
			MatchCollection ms = Regex.Matches(code, @"\#\#REPLACE.+[^#]");
			for (int matchNum=ms.Count-1; matchNum>=0; matchNum--)
			{
				Match m = Regex.Match(ms[matchNum].Value, @"\s.+[^#]");
				string item = m.Value.Remove(0,1);
				int index = item.IndexOf("##");
				item = item.Substring(0, index);

				code = code.Replace(ms[matchNum].Value, "");

				index = item.IndexOf("=");
				string oldString = item.Substring(0,index);
				string newString = item.Remove(0,index+1);
				code = code.Replace(oldString, newString);
			}

			if (code.EndsWith("##"))
				code = code.Substring(0,code.Length-2);

			return code;
		}

		public string ProcessMacros(string code)
		{
			MatchCollection ms = Regex.Matches(code, @"\#\#MACRO([\s\S\r\n])+\#\#");
			for (int matchNum=ms.Count-1; matchNum>=0; matchNum--)
			{
				Match m = Regex.Match(ms[matchNum].Value, @"(?<=MACRO\s+)\w+");
				string macroName = m.Value;
				string macroImplementation = ms[matchNum].Value.Remove(0, ms[matchNum].Value.IndexOf("\r\n")+2);
				macroImplementation = macroImplementation.Replace("#", "");

				//remove definition from code:
				code = code.Replace(ms[matchNum].Value, "");

				//find <args> usage:
				MatchCollection msArgs = Regex.Matches(macroImplementation, "<args>");

				//find macro occurrences:
				MatchCollection msCalls = Regex.Matches(code, macroName+@".+\)");
				for (int callNum=msCalls.Count-1; callNum>=0; callNum--)
				{
					string sCall = msCalls[callNum].Value;
					Match mArgsInCall = Regex.Match(sCall, @"(?<="+macroName+@"\().+(?=\))");
					string implementationWithArgs = macroImplementation;
					if (mArgsInCall.Success)
					{
						implementationWithArgs = implementationWithArgs.Replace("<args>", mArgsInCall.Value);
					}

					code = this.InsertWithSameIndentation(code, msCalls[callNum].Index, sCall, implementationWithArgs);
				}
			}
			return code;
		}

		/// <summary>
		/// Generates the text in the beginning with the referenced assemblies (e.g. C# "using System;")
		/// </summary>
		/// <returns></returns>
		public virtual string GenerateReferenceString()
		{
			return null;
		}

		public void AddReferencedAssemblies(string[] assemblies)
		{
			foreach (string s in assemblies)
				if (!_referencedAssemblies.Contains(s))
					_referencedAssemblies.Add(s);
		}

		public string DefaultContext
		{
			get {return this._defaultContext;}
			set {this._defaultContext = value;}
		}

		public ArrayList GetScriptNames()
		{
			ArrayList lst = new ArrayList();
			foreach (DictionaryEntry de in _classToAssembly)
			{
				lst.Add((string)de.Key);
			}
			return lst;
		}

		public virtual object Construct(string className, string instanceId)
		{
			return null;
		}
		public virtual object Construct(string className)
		{
			return this.Construct(className, className);
		}

		public virtual object GetInstance(string instanceId)
		{
			return this._instanceIdToObject[instanceId];
		}

		public virtual System.Reflection.Assembly CompileMultiple(Hashtable scriptNamesAndCode)
		{
			return null;
		}
		public virtual void Compile(string code, string nameID, Hashtable injects)
		{}
		public virtual void Compile(string code, string nameID)
		{}

		public virtual void Load(string filename, string optionalNameID)
		{
			if (optionalNameID == null)
				optionalNameID = filename;
			this._classToAssembly.Add(optionalNameID, Endogine.Files.FileReadWrite.Read(filename));
		}
		public virtual object Execute(string code)
		{
			return null;
		}
		public virtual object ExecuteFile(string scriptName, string methodName)
		{
			string code =	(string)this._classToAssembly[scriptName];
			if (code!=null)
				return this.Execute(code);
			return null;
		}
		public virtual object Invoke(string instanceId, string methodName)
		{
			return null;
		}
		public virtual void Remove(string instanceId)
		{
		}


		protected string InsertCode(string code)
		{
			return _defaultContext.Replace("##INJECT##", code);
		}

		public static string GetAssemblyInfo(Assembly ass)
		{
			string sAll = "";
			Type[] types = ass.GetTypes();
			foreach (Type type in types)
			{
				sAll+="class: "+type.Name+"\r\n";
				MethodInfo[] methods = type.GetMethods();
				foreach (MethodInfo method in methods)
				{
					if (method.Name != "GetType"
						&& method.Name != "GetHashCode"
						&& method.Name != "Equals"
						&& method.Name != "ToString")
					{
						sAll+=method.Name+"\r\n";
					}
				}
			}
			return sAll;
		}

	}
}
