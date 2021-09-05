using System;
using System.Drawing;

using Endogine;

namespace Tests
{
	/// <summary>
	/// Summary description for Bouncer.
	/// </summary>
	public class Bouncer : Endogine.GameHelpers.GameSprite
	{
        EPointF _acceleration = new EPointF(0,.1f);

		public Bouncer()
		{
			Name = "Bouncer";
            this.Velocity.Length = 1;
            this.Velocity.Angle = (float)Math.PI / 4;
		}

		public override void EnterFrame()
		{
            this.Velocity += this._acceleration;
			base.EnterFrame();
            if (this.Rect.Left < 0 || this.Rect.Right > EH.Instance.Stage.Size.X)
                this.Velocity.X *= -1;
            if (this.Rect.Bottom > EH.Instance.Stage.Size.Y)
            {
                this.Velocity.Y *= -1;
                this.Velocity -= this._acceleration; //make up for "lost" velocity
            }

            if (this.Animator != null)
                this.Animator.Animator.StepSize = Loc.X / 200;
		}
	}
}
