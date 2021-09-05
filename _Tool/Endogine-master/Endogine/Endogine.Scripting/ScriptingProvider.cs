using System;
using System.Collections;

namespace Endogine.Scripting
{
	//TODO: this is all static. Should it be part of ScripterBase instead? Not as clear, but more compact and I like the principle.
	/// <summary>
	/// Summary description for ScriptingProvider.
	/// </summary>
	public class ScriptingProvider
	{
		static Hashtable _scripters;

		public ScriptingProvider()
		{
		}

		public static ScripterBase CreateScripter(string languageSuffix)
		{
			if (_scripters == null)
				_scripters = new Hashtable();

			ScripterBase scripter = (ScripterBase)_scripters[languageSuffix];

			//can only have one instance of each scripter
			//TODO: any reason to allow multiple instances?
			if (scripter!=null)
				return scripter;

			//TODO: use reflection to find available scripters
			if (languageSuffix == "boo")
				scripter = new ScripterBoo();
			else if (languageSuffix == "cs")
				scripter = new ScripterCSharp();

			scripter.FileExtension = languageSuffix;
			return scripter;
		}

		public static ArrayList GetAvailableLanguages()
		{
			System.Collections.ArrayList lst = new ArrayList();
			lst.Add("boo");
			lst.Add("cs");
			return lst;
		}

		public static ScripterBase GetScripter(string languageSuffix)
		{
			if (_scripters == null)
				return null;

			return (ScripterBase)_scripters[languageSuffix];
		}
	}
}
