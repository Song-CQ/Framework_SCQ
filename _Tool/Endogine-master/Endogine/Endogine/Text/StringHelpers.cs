using System;

namespace Endogine.Text
{
	/// <summary>
	/// Summary description for StringHelpers.
	/// </summary>
	public class StringHelpers
	{
		public StringHelpers()
		{
		}

		/// <summary>
		/// Replaces a number (start, length) of characters in a string with the specified string.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="startIndexInInput"></param>
		/// <param name="lengthInInput"></param>
		/// <param name="replace"></param>
		/// <returns></returns>
		public static string Replace(string input, int startIndexInInput, int lengthInInput, string replace)
		{
			string s = input.Substring(0, startIndexInInput);
			s+=replace;
			s+=input.Substring(startIndexInInput+lengthInInput);
			return s;
		}
		public static string Replace(string input, string find, string replace, int maxReplacements)
		{
			do
			{
				int index = input.IndexOf(find);
				input = input.Substring(0,index) + replace + input.Remove(0,index+find.Length);
				maxReplacements--;
			} while (maxReplacements>0);

			return input;
		}
	}
}
