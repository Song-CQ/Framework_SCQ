using System;
using System.Collections;

namespace Endogine
{
	/// <summary>
	/// Summary description for SortedFloatList.
	/// </summary>
	public class SortedFloatList : SortedList
	{
		public SortedFloatList()
		{
		}

		public override void Add(object key, object value)
		{
			double fKey = (double)(float)key;
			fKey*=10000;
			int index = this.IndexOfKey(fKey);
			if (index >= 0)
			{
				double lastVal = fKey;
				while (++index < this.Count)
				{
					double val = (double)this.GetKey(index);
					if (val - lastVal > 1.1)
						break;
					lastVal = val;
				}
				fKey = lastVal+1;
			}
			base.Add (fKey, value);
		}
	}
}
