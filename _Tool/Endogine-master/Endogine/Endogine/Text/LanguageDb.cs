using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Endogine.Text
{
	/// <summary>
	/// Summary description for LanguageDb.
	/// </summary>
	public class LanguageDb
	{
		private Hashtable _htStrings;
		public static LanguageDb Instance;

		public LanguageDb()
		{
			Instance = this;
		}

		public void Load(string filename)
		{
			string languageColumnName = "String";

			if (filename == null)
				filename = Endogine.AppSettings.Instance.FindFile("Language.xls");

			System.Data.DataTable dt = null;
			if (filename.EndsWith(".xls"))
				dt = Endogine.Tools.DbAccess.ExcelToDataTable(filename);
			else
				dt = Endogine.Tools.DbAccess.CSVToDataTable(filename);

			System.Data.DataColumn col = dt.Columns[languageColumnName];
			if (col == null)
				col = dt.Columns[1];

			this._htStrings = new Hashtable();
			foreach (System.Data.DataRow row in dt.Rows)
			{
				if (row[0] == DBNull.Value)
					continue;
				string sKey = (string)row[0];
				sKey = sKey.ToLower();
				string s = "";
				if (row[col]!=DBNull.Value)
					s = (string)row[col];
				if (this._htStrings.Contains(sKey))
					throw new Exception("Same ID: "+sKey + " "+(string)this._htStrings[sKey] + " trying "+s);
				this._htStrings.Add(sKey, s);
			}
		}

		/// <summary>
		/// By using uppercase in the ID, the returned string will change accordingly.
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public static string GetString(string ID)
		{
			if (ID==null)
				return null;
			string lowercase = ID.ToLower();
			if (!Instance._htStrings.ContainsKey(lowercase))
				return ID+"N/A ";
			string s = (string)Instance._htStrings[lowercase];
			if (s == null)
				s = "Missing: "+ID;
			string firstLetter = ID.Substring(0,1);
			if (firstLetter.ToUpper() == firstLetter)
			{
				string lastLetter = ID.Substring(s.Length-1,1);
				if (lastLetter.ToUpper() == lastLetter)
					s = s.ToUpper();
				else
					s = firstLetter.ToUpper()+s.Substring(1,s.Length-1);
			}
			return s;
		}

		public static string FindString(string pattern)
		{
			string[] ss = FindStrings(pattern);
			if (ss.Length > 0)
				return ss[0];
			return null;
		}
		/// <summary>
		/// Wildcard or regex seach
		/// </summary>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static string[] FindStrings(string pattern)
		{
			ArrayList found = new ArrayList();
			if (pattern.IndexOf("*") >= 0)
			{
				//it's an oldschool wildcard search. Translate to regex:
				pattern = pattern.Replace(".", "[.]");
				pattern = pattern.Replace("*", ".*");
			}
			foreach (DictionaryEntry de in Instance._htStrings)
			{
				string sKey = (string)de.Key;
				Match m = Regex.Match(sKey, pattern);
				if (m.Success)
					found.Add((string)de.Value);
			}
			string[] result = new string[found.Count];
			for (int i=0; i<found.Count;i++)
				result[i] = (string)found[i];

			return result;
		}

		public static string Process(string s)
		{
			System.Text.RegularExpressions.MatchCollection ms =
				System.Text.RegularExpressions.Regex.Matches(s, @"[#]\w+[#]");

			for (int i = ms.Count-1; i >=0; i--)
			{
				System.Text.RegularExpressions.Match m = ms[i];
				string ID = m.Value.Remove(0,1);
				s = s.Replace(m.Value, GetString(ID));
			}
			return s;
		}
	}
}
