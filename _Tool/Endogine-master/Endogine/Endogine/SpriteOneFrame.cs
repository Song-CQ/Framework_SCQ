using System;

namespace Endogine
{
	/// <summary>
	/// a sprite that automatically disposes after being rendered one time
	/// Simplifies debugging, when you want to have sprite markers - but want to avoid creating member variables to
	/// keep track of the instances. Just create the sprite each frame instead.
	/// </summary>
	public class SpriteOneFrame : Sprite
	{
		public SpriteOneFrame(EndogineHub a_endogine):base(a_endogine)
		{
		}

		/*public override void Draw()
		{
			base.Draw();
			Dispose();
		}*/
		public override void EnterFrame()
		{
			base.EnterFrame();
			Dispose();
		}
	}
}
