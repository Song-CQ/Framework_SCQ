//From Marc Clifton's article on CodeProject
//http://www.codeproject.com/csharp/csquicksort.asp


using System;
using System.Collections;
using System.IO;

namespace Endogine
{

	// heavily modified from: http://www.msdnaa.net/Resources/Display.aspx?ResID=952

	// my own interface, allowing control over the swap process
	public interface ISwap
	{
		void Swap(ArrayList array, int left, int right);
	}

	// implements a default swapper and comparer
	public class Sort : ISwap, IComparer
	{
		public static IComparer comparer;
		public static ISwap swapper;

		/// <summary>
		/// The basic constructor does a quicksort using the built in comparer and swapper
		/// </summary>
		/// <param name="array">The array to sort.</param>
		public static void QuickSort(ArrayList array)
		{
			Sort s=new Sort();
			Sort.comparer=s;
			Sort.swapper=s;
			QuickSort(array, 0, array.Count-1);
		}

		/// <summary>
		/// Specifies my own swapper, but the default comparer
		/// </summary>
		/// <param name="array">The array to sort.</param>
		/// <param name="swapper">The custom swapper.</param>
		public static void QuickSort(ArrayList array, ISwap swapper)
		{
			Sort.comparer=new Sort();
			Sort.swapper=swapper;
			QuickSort(array, 0, array.Count-1);
		}

		/// <summary>
		/// Specifies my own comparer, but the default swapper
		/// </summary>
		/// <param name="array">The array to sort.</param>
		/// <param name="comparer">The customer comparer.</param>
		public static void QuickSort(ArrayList array, IComparer comparer)
		{
			Sort.comparer=comparer;
			Sort.swapper=new Sort();
			QuickSort(array, 0, array.Count-1);
		}

		/// <summary>
		/// Specifies both my comparer and my swapper
		/// </summary>
		/// <param name="array">The array to sort.</param>
		/// <param name="comparer">The custom comparer.</param>
		/// <param name="swapper">The custom swapper.</param>
		public static void QuickSort(ArrayList array, IComparer comparer, ISwap swapper)
		{
			Sort.comparer=comparer;
			Sort.swapper=swapper;
			QuickSort(array, 0, array.Count-1);
		}

		private static void QuickSort(ArrayList array, int lower, int upper)
		{
			// Check for non-base case
			if (lower < upper)
			{
				// Split and sort partitions
				int split=Pivot(array, lower, upper);
				QuickSort(array, lower, split-1);
				QuickSort(array, split+1, upper);
			}
		}

		private static int Pivot(ArrayList array, int lower, int upper)
		{
			// Pivot with first element
			int left=lower+1;
			object pivot=array[lower];
			int right=upper;

			// Partition array elements
			while (left <= right)
			{
				// Find item out of place
				while ( (left <= right) && (comparer.Compare(array[left], pivot) <= 0) )
				{
					++left;
				}

				while ( (left <= right) && (comparer.Compare(array[right], pivot) > 0) )
				{
					--right;
				}

				// Swap values if necessary
				if (left < right)
				{
					swapper.Swap(array, left, right);
					++left;
					--right;
				}
			}

			// Move pivot element
			swapper.Swap(array, lower, right);
			return right;
		}

		public void Swap(ArrayList array, int left, int right)
		{
			object swap=array[left];
			array[left]=array[right];
			array[right]=swap;
		}

		public int Compare(object a, object b)
		{
			return a.ToString().CompareTo(b.ToString());
		}
	}
}
