using System;
using System.Collections.Generic;

namespace Endogine.ResourceManagement
{
	/// <summary>
	/// A FrameSet is a collection of PicRefs - e.g. a bitmap animation sequence
	/// </summary>
	public class FrameSets
	{
		private Dictionary<string,List<string>> _frameSets;
		public FrameSets()
		{
            this._frameSets = new Dictionary<string, List<string>>();
		}

		/// <summary>
		/// All frameSets must have unique names Walk for two different characters
		/// </summary>
		/// <param name="name"></param>
		/// <param name="pictureNames"></param>
		public void AddFrameSet(string name, List<string> pictureNames)
		{
			if (this._frameSets.ContainsKey(name))
			{
				//EH.Put("Non-unique frameSet name: "+name);
				return;
			}
			this._frameSets.Add(name, pictureNames);
		}

        public bool Exists(string frameSetName)
        {
            return this._frameSets.ContainsKey(frameSetName);
        }

        public List<string> this[string frameSetName]
		{
			get
            {
                if (!this._frameSets.ContainsKey(frameSetName))
                    return null;
                return this._frameSets[frameSetName];
            }
		}
	}
}
