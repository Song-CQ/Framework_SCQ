using System;

namespace Tests.Processing
{
	/// <summary>
	/// Summary description for Random.
	/// </summary>
	public class RandomEx
	{
		static Random _rnd;
		public RandomEx()
		{
		}

		public static void Init()
		{
			if (RandomEx._rnd==null)
				RandomEx._rnd = new Random();
		}

		public static float Random()
		{
			return (float)RandomEx._rnd.NextDouble();
		}
		public static int Random(int i)
		{
			int val = RandomEx._rnd.Next(i);
			return val>i-1?i-1:val;
		}
		public static int Random(int from, int to)
		{
			return RandomEx.Random(to-from)+from;
		}

		public static float Random(float i)
		{
			float val = (float)RandomEx._rnd.NextDouble();
			return val*i;
		}
		public static float Random(float from, float to)
		{
			return RandomEx.Random(to-from)+from;
		}
	}
}
