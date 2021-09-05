using System;
using System.ComponentModel;

namespace Endogine.GameHelpers
{
	/// <summary>
	/// Summary description for GameSprite.
	/// </summary>
	public class GameSprite : Sprite
	{
		protected EPointF _velocity;
		protected int _numFramesAlive;

		public GameSprite()
		{
			this._velocity = new EPointF();
		}

		public override void EnterFrame()
		{
			base.EnterFrame();
			this._numFramesAlive++;
			Loc+=this._velocity;
		}

		public int FramesAlive
		{
			get {return this._numFramesAlive;}
		}

		[Category("Movement")]
		public EPointF Velocity
		{
			get {return this._velocity;}
			set 
			{this._velocity = value;}
		}
	}
}
