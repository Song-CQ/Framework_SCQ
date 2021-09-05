using System;
using System.Xml;
using System.Collections;

namespace Endogine.Animation
{
	/// <summary>
	/// Summary description for AnimationHelpers.
	/// </summary>
	public class AnimationHelpers
	{
		public AnimationHelpers()
		{
		}

		public static SortedList ParseAnimations(string xmlFileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFileName);
			return ParseAnimations(doc.FirstChild);
		}

		public static ArrayList ParseAnimationString(string sAnim)
		{
			//TODO: "<FrameSet name>"
			string[] sFirstSplit = sAnim.Split(" ".ToCharArray(), 2);
			int nFirstFrame = 0;
			if (sFirstSplit.Length > 1)
			{
				nFirstFrame = Convert.ToInt32(sFirstSplit[0]);
				sAnim = sFirstSplit[1];
			}

			ArrayList anim = Endogine.Text.IntervalString.CreateArrayFromIntervalString(sAnim);
			for (int i=0; i<anim.Count; i++)
			{
				int frame = (int)anim[i];
				anim[i] = frame + nFirstFrame;
			}
			return anim;
		}

		public static SortedList ParseAnimations(XmlNode node)
		{
			SortedList animations = new SortedList();

			foreach (XmlNode animNode in node.ChildNodes)
			{
				//<Stand_Angry>10  0-10,13,15,9</Stand_Angry>
				
				string sAnim = Serialization.XmlHelper.GetValueOrInnerText(animNode).Trim();
				ArrayList anim = ParseAnimationString(sAnim);

				SortedList aKeys = new SortedList();
				animations.Add(animNode.Name, aKeys);
				
				int nTime = 0;
				foreach (int val in anim)
				{
					AnimationKey key = new AnimationKey(nTime, val); //+nFirstFrame);
					aKeys.Add(key.Time, key);
					nTime++;
				}
			}

			return animations;
		}
	}
}
