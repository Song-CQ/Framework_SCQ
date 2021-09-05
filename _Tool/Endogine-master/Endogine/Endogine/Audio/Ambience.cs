using System;

namespace Endogine.Audio
{
	/// <summary>
	/// Used to play background sounds with variation.
	/// Instead of having one looong bg sound which is so long the repetition isn't noticeable
	/// (or a short where it IS noticeable)
	/// use this player which has a little library of effects that are randomized;
	/// e.g. a looping bg wind sound with pitch/volume which variates over time
	/// a number of bird chirps which come at irregular intervals,
	/// a few car randomized drive-by sounds etc
	/// 
	/// Possibly it should have an intensity setting, like DirectMusic has (had?)
	/// </summary>
	public class Ambience
	{
		public Ambience()
		{
			//
		}
	}
}
