using System;
using Endogine;

namespace MusicGame.Midi.HitFeedback
{
	/// <summary>
	/// Summary description for Test.
	/// </summary>
	public class Test : Base
	{

		public Test()
		{
			this.Velocity = new EPointF(0,0);
			SpriteEffect.Wiggle fx = new MusicGame.Midi.SpriteEffect.Wiggle();
			fx.Parent = this;

			this._lbl = new Label.Default();
		}


		public override void EnterFrame()
		{
			this.Blend-=5;
			this.Velocity+=new EPointF(0,-0.2f);
			for (int i=this.ChildCount-1;i>=0;i--)
				this.GetChildByIndex(i).Blend = this.Blend;

			base.EnterFrame();
			
			if (this.Blend<=0)
				this.Dispose();
		}
	}
}
