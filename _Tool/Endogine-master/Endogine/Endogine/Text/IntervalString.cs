using System;

namespace Endogine.Text
{
	/// <summary>
	/// Summary description for RangeString.
	/// </summary>
	public class IntervalString
	{
		public IntervalString()
		{
		}

		public static string CreateIntervalStringFromArray(ref System.Collections.ArrayList a_array)
		{
			bool bAllowExceptClause = false;

			string sResult = "";
			string sExclude = "";
			int nCount = a_array.Count;
			if (nCount == 0)
				return sResult;

			a_array.Sort();
			int nLast = Convert.ToInt32(a_array[0]);
			sResult = nLast.ToString();
			int nThis;
			int nFirstInCurrentRange = nLast;

			for (int i = 1; i < nCount; i++)
			{
				nThis = Convert.ToInt32(a_array[i]);
				if (nThis == nLast) //ignore if a number occurs more than once //TODO: build another list for those?
					continue;

				if (nThis != nLast+1)
				{
					if (bAllowExceptClause && nThis == nLast+2)
					{
						//TODO: if it's just one that's skipped, continue this range and plce the skipped in the exclude list
						//ex: 1,2,3,4,5,7,8,9,10,12,13,14,16 can be 1-16 EXCEPT 6,11,15 instead of 1-5,7-10,12-14,16
						sExclude+=(nLast+1).ToString() + ",";
					}
					else
					{
						if (nLast != nFirstInCurrentRange) //it was a range (more than one number)
							sResult+= "-" + nLast.ToString();
						sResult+=","+nThis.ToString();
						nFirstInCurrentRange = nThis;
					}
				}

				nLast = nThis;
			}
			//remove the last ","
			//sResult = sResult.Remove(sResult.Length-1,1);

			//TODO: misses last range end, no..?

			if (bAllowExceptClause && sExclude.Length > 0)
				sResult+=" EXCEPT "+sExclude.Remove(sExclude.Length-1, 1);
			return sResult;
		}

		//-3--1,3,5-7,30,31 becomes -3,-2,-1,3,5,6,7,30,31
		public static string CreateCommaStringFromIntervalString(string a_sInterval, string a_sRemoveIntervals)
		{
			System.Collections.ArrayList a_aReturnList = a_aReturnList = CreateArrayWithRemovalFromIntervalString(a_sInterval, a_sRemoveIntervals);
			string sReturn = "";
			for (int i = 0; i < a_aReturnList.Count; i++)
				sReturn += a_aReturnList[i].ToString() + ",";

			return sReturn.Remove(sReturn.Length-1,1);
		}

		public static System.Collections.ArrayList CreateArrayWithRemovalFromIntervalString(string a_sInterval, string a_sRemoveIntervals)
		{
			int nOff = a_sInterval.IndexOf("EXCEPT");
			if (nOff >= 0)
			{
				a_sRemoveIntervals = a_sInterval.Substring(nOff+7);
				a_sInterval = a_sInterval.Substring(0, nOff-1);
			}

			System.Collections.ArrayList a_aReturnList = CreateArrayFromIntervalString(a_sInterval);
			System.Collections.ArrayList aRemoveList = CreateArrayFromIntervalString(a_sRemoveIntervals);

			for (int i = 0; i < aRemoveList.Count; i++)
				a_aReturnList.Remove(aRemoveList[i]);

			return a_aReturnList;
		}

		public static System.Collections.ArrayList CreateArrayFromIntervalString(string a_sInterval)
		{
			int nLastCharType = -4711;  //1 = operation, 2 = val
			int nIntervalStartVal = 0;
			bool bGotIntervalStart = false;
			int nCharNum = 0;
			string sChar, sSinceLastOp = "";

			a_sInterval = a_sInterval.Trim();
			//a_sInterval = Regex.Replace(a_sInterval, "[^0-9]", "");
			
			System.Collections.ArrayList a_aReturnList = new System.Collections.ArrayList();
			do
			{
				if (a_sInterval.Length == 0)
					return a_aReturnList;

				sChar = a_sInterval.Substring(nCharNum,1);
				if (sChar == "," || nCharNum == a_sInterval.Length-1)
				{
					int nLast = nCharNum-1;
					if (nCharNum == a_sInterval.Length-1)
						nLast = nCharNum;

					nLastCharType = 1;
					if (nLast > -1)
					{
						int nVal = 0;

						sSinceLastOp = a_sInterval.Substring(0,nLast+1);
						if (sSinceLastOp.Length == 0)
							throw new Exception("Wrong format");
					
						nVal = Convert.ToInt32(sSinceLastOp);

						if (bGotIntervalStart)
						{
							if (nIntervalStartVal > nVal) 
							{
								int nTmp = nVal;
								nVal = nIntervalStartVal;
								nIntervalStartVal = nVal;
							}

							for (int n = nIntervalStartVal; n <= nVal; n++)
							{
								a_aReturnList.Add(n);
							}
							bGotIntervalStart = false;
						}
						else
						{
							a_aReturnList.Add(nVal);
						}

						if (nCharNum == a_sInterval.Length-1)
							break;
					}
					a_sInterval = a_sInterval.Remove(0,nCharNum+1);
					nCharNum = 0;
				}

				else if (sChar == "-")
				{
					if (nLastCharType == -4711 || nLastCharType == 1)
					{
						nCharNum++;
					}
					else
					{
						nLastCharType = 1;
						string s = a_sInterval.Substring(0,nCharNum);
						//return "#" + a_sInterval + "#" + nCharNum.ToString();
						nIntervalStartVal = Convert.ToInt32(s);
						bGotIntervalStart = true;
						a_sInterval = a_sInterval.Remove(0,nCharNum+1);
						nCharNum = 0;
					}
				}
				else
				{
					//TODO: check that it's an integer, and no other char!
					nLastCharType = 2;
					nCharNum++;
				}
			}
			while (nCharNum < a_sInterval.Length);

			return a_aReturnList;
		}
	}
}
