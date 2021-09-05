using System;
using System.IO;

namespace Endogine.Files
{
	/// <summary>
	/// Summary description for FileReadWrite.
	/// </summary>
	public class FileReadWrite
	{
		public FileReadWrite()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Simple file reader, returns contents of file.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string Read(string filename)
		{
			//TODO: use generic function to parse ## entries (predefined paths)
			filename = Files.FileFinder.GetFirstMatchingFile(filename);
			System.IO.StreamReader rd = new StreamReader(filename, System.Text.Encoding.Default);
			string contents = rd.ReadToEnd();
			rd.Close();
			return contents;
		}

		/// <summary>
		/// Simple file writer. Deletes old file first.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="contents"></param>
		public static void Write(string filename, string contents)
		{
			if (File.Exists(filename))
				File.Delete(filename);

			System.IO.StreamWriter wr = new StreamWriter(filename);
			wr.Write(contents);
			wr.Flush();
			wr.Close();
		}

        public static void Delete(string filename)
        {
            File.Delete(filename);
        }
	}
}
