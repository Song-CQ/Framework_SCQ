using System;
using System.Collections;

namespace Endogine.ResourceManagement
{
	/// <summary>
	/// Summary description for Animations.
	/// </summary>
	public class Animations
	{
		private Hashtable _animations;
		public Animations()
		{
			_animations = new Hashtable();
		}

		/// <summary>
		/// All animations must have unique names Walk for two different characters
		/// </summary>
		/// <param name="name"></param>
		/// <param name="animation"></param>
		public void AddAnimation(string name, ArrayList animation)
		{
			_animations.Add(name, animation);
		}
	}
}
