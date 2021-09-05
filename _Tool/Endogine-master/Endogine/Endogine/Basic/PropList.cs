using System;
using System.Collections;

namespace Endogine
{
	/// <summary>
	/// Collection of "non-unique keys"/values. Can be sorted.
	/// When there are several identical keys, the first one is used for [], IndexOfKey etc
	/// </summary>
    public class PropList : IEnumerable
	{
		private bool m_bSorted;
		private ArrayList m_aKeys;
		private ArrayList m_aValues;

		public PropList()
		{
			this.m_aKeys = new ArrayList();
			this.m_aValues = new ArrayList();
		}

		public void Clear()
		{
			this.m_aKeys.Clear();
			this.m_aValues.Clear();
		}
		public void Merge(PropList pl)
		{
			for (int i = 0; i < pl.Count; i++)
				this.Add(pl.GetKey(i), pl.GetByIndex(i));
		}

		public void Add(object oKey, object oValue)
		{
			this.m_aKeys.Add(oKey);
			this.m_aValues.Add(oValue);
			//TODO: this is a slow way to do it. Instead, check where key should be inserted!
			if (this.m_bSorted)
				this.Sort();
		}
		public void Insert(int nIndex, object oKey, object oValue)
		{
			this.m_bSorted = false;
			this.m_aValues.Insert(nIndex, oValue);
			this.m_aKeys.Insert(nIndex, oKey);
		}
		public void Remove(object oKey)
		{
			int nIndex = this.IndexOfKey(oKey);
			this.RemoveAt(nIndex);
		}
		public void RemoveAt(int nIndex)
		{
			this.m_aKeys.RemoveAt(nIndex);
			this.m_aValues.RemoveAt(nIndex);
		}
		public void RemoveValue(object oValue)
		{
			int nIndex = this.IndexOfValue(oValue);
			this.RemoveAt(nIndex);
		}

		private void Sort()
		{
			//sort the keys:
			ArrayList orgKeys = this.m_aKeys.GetRange(0,this.m_aKeys.Count);
			Endogine.Sort.QuickSort(this.m_aKeys);

			//rearrange values:
			ArrayList orgValues = this.m_aValues.GetRange(0,this.m_aValues.Count);
			for (int i = 0; i < this.m_aValues.Count; i++)
			{
				object oKey = orgKeys[i];
				int nNewIndex = this.m_aKeys.IndexOf(oKey);
				//TODO: check if there are several identical keys
				//If so, 
				this.m_aValues[nNewIndex] = orgValues[i];
			}
			this.m_bSorted = true;
		}

		public bool Sorted
		{
			get {return this.m_bSorted;}
			set {if (!this.m_bSorted) this.Sort();}
		}

		/// <summary>
		/// Returns the *first* value in the list that has this key
		/// </summary>
		public object this[object oKey]
		{
			get 
			{
				int nIndex = this.IndexOfKey(oKey);
				if (nIndex < 0 || nIndex >= this.Count)
					return null;
				return this.GetByIndex(nIndex);
			}
			set 
			{
				int nIndex = this.IndexOfKey(oKey);
				if (nIndex < 0)
					this.Add(oKey, value);
				else
					this.m_aValues[nIndex] = value;
			}
		}


		public int IndexOfKey(object oKey)
		{
			return this.m_aKeys.IndexOf(oKey);
		}
		public int IndexOfValue(object oValue)
		{
			return  this.m_aValues.IndexOf(oValue);
		}

		public int Count
		{
			get{return this.m_aValues.Count;}
		}
		public bool ContainsKey(object oKey)
		{
			return this.IndexOfKey(oKey)>=0;
		}
		public bool ContainsValue(object oValue)
		{
			return this.IndexOfValue(oValue)>=0;
		}

		public object GetKeyOfValue(object oValue)
		{
			int nIndex = this.IndexOfValue(oValue);
			return this.GetKey(nIndex);
		}

		public object GetKey(int nIndex)
		{
			return this.m_aKeys[nIndex];
		}
		public object GetByIndex(int nIndex)
		{
			return this.m_aValues[nIndex];
		}
		/// <summary>
		/// Gets all values that have a certain key.
		/// </summary>
		/// <param name="oKey"></param>
		/// <returns></returns>
		public ArrayList GetAllWithKey(object oKey)
		{
			int n = this.IndexOfKey(oKey);
			if (n < 0)
				return null;
			ArrayList lst = new ArrayList();
            lst.Add(this.GetByIndex(n));
			for (;;)
			{
                n++;
                if (n >= this.Count)
                    break;
                if (this.GetKey(n) == oKey)
                    lst.Add(this.GetByIndex(n));
			}
			return lst;
		}


		public void SetByIndex(int nIndex, object oValue)
		{
			this.m_aValues[nIndex] = oValue;
		}
		public void SetKeyByIndex(int nIndex, object oKey)
		{
			this.m_bSorted = false;
			this.m_aKeys[nIndex] = oKey;
		}

        public ArrayList Keys
        {
            get { return (ArrayList)this.m_aKeys.Clone(); }
        }
        public ArrayList Values
        {
            get { return (ArrayList)this.m_aValues.Clone(); }
        }

        public IEnumerator GetEnumerator()
        {
            return new PropListEnumerator(this.m_aKeys, this.m_aValues);
        }

        private class PropListEnumerator : IEnumerator
        {
            private ArrayList m_aKeys;
            private ArrayList m_aValues;
            private int _index;

            public PropListEnumerator(ArrayList keys, ArrayList values)
            {
                m_aKeys = keys;
                m_aValues = values;
                _index = -1;
            }

            public void Reset()
            {
                _index = -1;
            }

            public DictionaryEntry Current
            {
                get { return new DictionaryEntry(m_aKeys[_index], m_aValues[_index]); }
            }
            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            { m_aKeys = null; m_aValues = null; }

            public bool MoveNext()
            {
                _index++;
                if (_index >= m_aKeys.Count)
                    return false;
                else
                    return true;
            }
        }
	}
}
