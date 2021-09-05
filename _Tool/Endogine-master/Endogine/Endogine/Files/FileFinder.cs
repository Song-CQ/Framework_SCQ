using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Endogine.Files
{
	/// <summary>
	/// Summary description for FileFinder.
	/// </summary>
	public class FileFinder
	{
		public static string RegexIdentifier = "@@";

		public FileFinder()
		{
		}

//		public static string[] GetMultipass(string[] searchpatterns, string basePath)
//		{
//			foreach (string searchpattern in searchpatterns)
//			{
//			}
//			return null;
//		}

		public static bool IsFullyQualified(string filename)
		{
			FileInfo finfo = new FileInfo(filename);
			return filename.IndexOf(finfo.Directory.Root.FullName) == 0;
		}

        public static bool ContainsRoot(string path)
        {
            if (path[1] == ':')
                return true;
            if (path.StartsWith(@"\\"))
                return true;
            return false;
        }

		public static string GetDirectory(string ID_XPath)
		{
			string[] paths = AppSettings.Instance[ID_XPath];
			return paths[0];
		}

//		public static FileInfo[] GetFileInfos(string searchpattern)
//		{
//			string[] dirAndFile = GetDirectoryAndFile(searchpattern);
//			DirectoryInfo di = new DirectoryInfo(dirAndFile[0]);
//			return di.GetFiles(dirAndFile[1]);
//		}

		public static string[] GetNamesFromFiles(FileInfo[] files)
		{
			string[] names = new string[files.Length];
			for (int i=0; i<files.Length;i++)
				names[i] = files[i].FullName;
			return names;
		}

		/// <summary>
		/// Pattern for finding [1-4] [1-45,56-80] [12-14pad:3] etc
		/// </summary>
		/// <returns></returns>
		public static string GetRegexPatternForRanges()
		{
			return @"\[[0-9]+[-,]+[0-9]+[-,0-9]*[pad:]*[0-9]*\]";
			//\[[0-9]+[-,]+[0-9]+[-,0-9]*\]
			//\[[0-9]+[-,0-9]*[0-9]\]"); 
			//\[[0-9]+[-,0-9]*[0-9]{2,}\]]");
		}

		/// <summary>
		/// If filename starts with @@, it's interpreted as being a regex, eg: C:\test\@@^File[0-9][\w+][.]txt$
        /// * for wildcards only works without @@.
		/// </summary>
		/// <param name="searchpattern"></param>
		/// <returns></returns>
		public static FileInfo[] GetFiles(string searchpattern)
		{
            //TODO: wanted ¤¤ instead of @@, but StreamReader ignores them!?!?
            string[] dirAndFile = GetDirectoryAndFile(searchpattern);
            DirectoryInfo di = new DirectoryInfo(dirAndFile[0]);
            searchpattern = dirAndFile[1];


            if (!searchpattern.StartsWith(RegexIdentifier))
            {
                if (searchpattern.IndexOf("*")>=0)
                {
                    //might be a regular wildcard?
                    //TODO: how to determine properly?
                    if (searchpattern.IndexOfAny("[]+".ToCharArray()) < 0)
                    {
                        //if so just make it a regex.
                        searchpattern = RegexIdentifier + searchpattern.Replace(".", "[.]");
                        searchpattern = searchpattern.Replace("*", ".*");
                    }
                }
            }

			FileInfo[] files = null;
			if (searchpattern.StartsWith(RegexIdentifier))
			{
				searchpattern = searchpattern.Remove(0,2);

				//check if a range of numbers is defined (not allowed in regular expressions, do a manual thing)
				//allow more matches than final result, then do a new search for each found file
				string orgSearchpattern = searchpattern;

				MatchCollection ms = Regex.Matches(searchpattern, GetRegexPatternForRanges());
				ArrayList newPositions = new ArrayList();
				ArrayList allowedNumbers = new ArrayList();
				ArrayList newPatterns = new ArrayList();

				foreach (Match rangeMatch in ms)
				{
					int index;

					//check if padding is specified. 
					string sMatch = rangeMatch.Value;
					index = sMatch.IndexOf("pad:");
					int padding = 0;
					if (index > 0)
					{
						string sPadding = sMatch.Substring(index+4, sMatch.Length-index-5);
						padding = Convert.ToInt32(sPadding);
						sMatch = sMatch.Remove(index, sMatch.Length-index-1);
					}

					string s = sMatch.Substring(1,sMatch.Length-2);
					ArrayList nums = Endogine.Text.IntervalString.CreateArrayFromIntervalString(s);
					allowedNumbers.Add(nums);

					int nLowest = ((int)nums[0]).ToString().Length;
					int nHighest = ((int)nums[nums.Count-1]).ToString().Length;
					if (padding > 0)
					{
						nLowest = padding;
						nHighest = padding;
					}
					string newSubPattern = "[0-9]{"+nLowest.ToString()+","+nHighest.ToString()+"}";
					newPatterns.Add(newSubPattern);

					index = searchpattern.IndexOf(rangeMatch.Value);
					searchpattern = searchpattern.Substring(0,index) + newSubPattern + searchpattern.Remove(0,index+rangeMatch.Length);
					newPositions.Add(index);
				}

				FileInfo[] allFiles = di.GetFiles("*");
				ArrayList lst = new ArrayList();
				foreach (FileInfo fi in allFiles)
				{
                    //if (searchpattern.IndexOf("[1]") > 0 && fi.Name.IndexOf("143") > 0)
                    //    dirAndFile[1] = "ms";

					Match m = Regex.Match(fi.Name, searchpattern);
					if (m.Success && m.Index == 0) //only OK if pattern starts at 0 (e.g. abc.* -> abcde but not eabcd)
					{
						bool ok = true;
						if (ms.Count > 0)
						{
							for (int i=0; i<ms.Count;i++)
							{
								//TODO: redesign this!
								//redesign: extract pattern up until next rangepattern starts (or to the end)

								//first: find part of the pattern up until the rangepattern start
								string firstPattern = searchpattern.Substring(0,(int)newPositions[i]);
								m = Regex.Match(fi.Name, firstPattern);
								int firstLength = m.Index+m.Length;

								string nextPattern = firstPattern + (string)newPatterns[i];
								m = Regex.Match(fi.Name, nextPattern);
								int nextLength = m.Index+m.Length;

								string substring = fi.Name.Substring(firstLength, nextLength-firstLength);
								int val = Convert.ToInt32(substring);

								if (!((ArrayList)allowedNumbers[i]).Contains(val))
								{
									ok = false;
									break;
								}
							}
						}

						if (ok)
							lst.Add(fi);
					}
				}
				files = new FileInfo[lst.Count];
				for (int i=0; i<lst.Count;i++)
					files[i] = (FileInfo)lst[i];
			}
			else
				files = di.GetFiles(searchpattern);

			return files;
		}

        /// <summary>
        /// Ensures there's a \ between path and file, also handles ..\ in the filename.
        /// Removes
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string JoinPathAndFile(string folder, string filename)
        {
            if (folder == null)
                folder = "";
            //if (folder.Length == 0)
                //folder = System.Com
            if (folder.Length > 0)
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                while (filename.StartsWith(@"..\"))
                {
                    filename = filename.Remove(0, 3);
                    di = di.Parent;
                }
                folder = di.FullName + "\\";
            }
            int index = filename.LastIndexOf("\\");
            if (index >= 0)
                filename = filename.Remove(0, index + 1);

            return folder + filename;
        }

		public static string[] GetDirectoryAndFile(string fileName)
		{
			int nIndex = fileName.LastIndexOf(RegexIdentifier);
			if (nIndex >= 0)
				nIndex = fileName.LastIndexOf("\\", nIndex);
			else
				nIndex = fileName.LastIndexOf("\\");
			string sDir = "";
			if (nIndex >= 0)
			{
				sDir = fileName.Substring(0,nIndex);
				fileName = fileName.Remove(0,nIndex+1);
			}
			return new string[]{sDir, fileName};
		}

		/// <summary>
		/// Find the first file that matches the wildcard
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string GetFirstMatchingFile(string fileName)
		{
			//if it's already got an extension, don't continue:
			//(?!\.)[a-z]{1,4}$  [.][\w][\w][\w]\z
			//System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(fileName, @"(?!\.)[a-z]{1,4}$");
			System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(fileName, @"[.][a-z]{1,4}$");
			if (m.Success)
			{
				if (File.Exists(fileName))
					return fileName;
				return null;
			}

			int nIndex = fileName.LastIndexOf("\\");
			if (nIndex >= 0)
			{
				string sDir = fileName.Substring(0,nIndex);
				if (!sDir.EndsWith("\\"))
					sDir+="\\";

				DirectoryInfo dirInfo = new DirectoryInfo(sDir);
				if (!dirInfo.Exists)
					return null;

				fileName = fileName.Remove(0,nIndex+1);
				FileInfo[] files = dirInfo.GetFiles(fileName+".*");
				if (files!=null && files.Length > 0)
					return files[0].FullName;
			}
			return null;
		}

		public static string GetHighestFilename(string searchpattern)
		{
			FileInfo[] files = FileFinder.GetFiles(searchpattern);
			if (files.Length == 0)
				return null;
			SortedList sl = new SortedList();
			foreach (FileInfo existingFile in files)
				sl.Add(existingFile.Name, existingFile);
			return ((FileInfo)sl.GetByIndex(sl.Count-1)).FullName;
		}
		
		public static string GetHighestFilenamePlus(string searchpattern, int plusWhat)
		{
			if (!(searchpattern.IndexOf("*") > 0))
				throw new Exception("Only works with * wildcards: "+searchpattern);

			//make sure the full path is in the search pattern:
			string xxx = searchpattern.Replace("*", "");
			FileInfo file = new FileInfo(xxx);
			searchpattern = file.FullName.Replace(xxx, searchpattern);

			int index = searchpattern.IndexOf("*");

			string originallyAfterStar = searchpattern.Remove(0,index+1);
			int whatIsStar = -1;
			//find the file with the highest number matching this file:
			FileInfo[] files = FileFinder.GetFiles(searchpattern);
			//System.IO.FileInfo[] files = Endogine.Files.FileFinder.GetFiles(searchpattern);
			if (files.Length > 0)
			{
				SortedList sl = new SortedList();
				foreach (FileInfo existingFile in files)
					sl.Add(existingFile.Name, existingFile);
				FileInfo fileHighest = (FileInfo)sl.GetByIndex(sl.Count-1);
				string fromStar = fileHighest.FullName.Remove(0,index);
				whatIsStar = Convert.ToInt32(fromStar.Replace(originallyAfterStar, ""));
				//whatIsStar = afterStar.Replace(searchpattern.Remove(0,index+1), "");
			}
			whatIsStar+=plusWhat;
			string filename = searchpattern.Substring(0,index)+whatIsStar.ToString().PadLeft(3,'0')+originallyAfterStar;
			return filename;
		}



        public static string[] GenerateFilenamesFromRegexList(string[] regexList)
        {
            List<string> allFiles = new List<string>();
            bool exclude = false;
            foreach (string s in regexList)
            {
                if (s.ToLower() == "except")
                    exclude = true;
                else
                {
                    string[] dirAndFile = Endogine.Files.FileFinder.GetDirectoryAndFile(s);
                    string[] files = Endogine.Files.FileFinder.GetNamesFromFiles(
                        Endogine.Files.FileFinder.GetFiles(dirAndFile[0] + "\\" + dirAndFile[1]));
                    foreach (string file in files)
                    {
                        if (exclude)
                            allFiles.Remove(file);
                        else
                            allFiles.Add(file);
                    }
                }
            }

            string[] returnList = new string[allFiles.Count];
            for (int i = 0; i < allFiles.Count; i++)
                returnList[i] = allFiles[i];
            return returnList;
        }

        /// <summary>
        /// When encountering a lines with only a directory name, it will be used as the current directory
        /// Lines after that are treated as files/regexes in that directory.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] HumanReadableToStrings(string text)
        {
            string[] input = text.Split("\r\n".ToCharArray());
            string currentDir = null;
            List<string> strings = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                string s = input[i];
                if (s.EndsWith(":"))
                {
                    currentDir = s.Remove(s.Length - 1);
                    if (!currentDir.EndsWith("\\"))
                        currentDir += "\\";
                }
                else if (s.ToLower() == "except")
                    strings.Add("except");
                else if (s.Length > 0)
                {
                    if (s.IndexOf(":\\") == 1 || s.StartsWith("\\\\"))
                        //if (Endogine.Files.FileFinder.IsFullyQualified(s))
                        strings.Add(s);
                    else
                        strings.Add(currentDir + s);
                }
            }

            string[] output = new string[strings.Count];
            for (int i = 0; i < strings.Count; i++)
                output[i] = strings[i];

            return output;
        }
        public static string StringsToHumanReadable(string[] strings)
        {
            List<string> sorted = new List<string>();
            foreach (string s in strings)
                sorted.Add(s);
            sorted.Sort();

            string currentDir = null;
            string output = "";
            for (int i = 0; i < sorted.Count; i++)
            {
                string s = sorted[i];

                if (s.Trim().ToLower() == "except")
                    output += "except\r\n";
                else
                {
                    int index = s.LastIndexOf("\\");
                    string dir = s.Substring(0, index);
                    string file = s.Remove(0, index + 1);
                    if (dir != currentDir)
                    {
                        output += dir + ":\r\n";
                        currentDir = dir;
                    }
                    output += file + "\r\n";
                }
            }
            return output;
        }
	}
}
