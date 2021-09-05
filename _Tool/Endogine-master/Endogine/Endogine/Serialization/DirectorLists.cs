using System;
using System.Xml;
using System.Text.RegularExpressions;

namespace Endogine.Serialization
{
	/// <summary>
	/// Summary description for DirectorLists.
	/// </summary>
	public class DirectorLists
	{
		public DirectorLists()
		{
		}

		private static int GetLineCount(string s)
		{
			int nLineStart = 0;
			int nCurrLine = 0;
			while (true)
			{
				int nLineEnd = s.IndexOf("\n", nLineStart);
				if (nLineEnd < 0)
					return nCurrLine;
				nLineStart = nLineEnd+1;
				nCurrLine++;
			}
		}

		private static string GetLine(string s, int nLine)
		{
			int nLineStart = 0;
			int nLineEnd = 0;
			for (int nCurrLine = 0; nCurrLine <= nLine; nCurrLine++)
			{
				nLineEnd = s.IndexOf("\n", nLineStart+1);
				if (nLineEnd < 0)
					return "";
			}
			return s.Substring(nLineStart, nLineEnd);
		}

		private static string GetWord(string s, int nWord)
		{
			int nWordStart = 0;
			int nWordEnd = 0;
			for (int nCurrWord = 0; nCurrWord <= nWord; nCurrWord++)
			{
				Match match = Regex.Match(s, "^ ");
				nWordStart = match.Index;
				s = s.Remove(0,nWordStart);
				match = Regex.Match(s, " ");
				nWordEnd = match.Index;
				if (nWordEnd < 0)
					return "";
				s = s.Remove(0,nWordEnd);
			}
			return s.Substring(nWordStart, nWordEnd);
		}


		private static void Recurse()
		{
		}

		public static XmlDocument TabbedToXML(string a_sTabbed)
		{
			XmlDocument xdoc = new XmlDocument();

			XmlNode node = xdoc.CreateNode(XmlNodeType.Element, "root", null);
			xdoc.AppendChild(node);

			int nNumLines = GetLineCount(a_sTabbed);
			int nLastIndents = 0;
			for (int nLineNum = 0; nLineNum < nNumLines; nLineNum++)
			{
				string sLine = GetLine(a_sTabbed, nLineNum);
				
				Match match = Regex.Match(sLine, "^\t");
				int nIndents = match.Index;

				string sWord1 = GetWord(sLine, 0);
				if (sWord1.Substring(0,1) == "#")
					sWord1 = sWord1.Substring(1,sWord1.Length-1); //TODO: linear list

				//TODO: add node?
				if (nIndents > nLastIndents)
				{
				}
				else if (nIndents < nLastIndents)
				{
					//      tmpPropList = argPropList
					//      repeat with n = 1 to indent-1
					//        tmpPropList = tmpPropList.getAt(tmpPropList.count)
					//      end repeat
				}

				int nNumWords = 0;

				if (nNumWords == 1)
				{
						//Add new node with subnodes
//      tmpNewPropList = [:]
//      tmpPropList.addProp(prop, tmpNewPropList)
//	  tmpPropList = tmpNewPropList
				}
				
				if (nNumWords > 1)
				{
					//Add node with value
					//GetWords(sLine, 1, nNumWords)
				}
				else
				{
				}
				nLastIndents = nIndents;
			}
			return xdoc;
		}
	}
}
