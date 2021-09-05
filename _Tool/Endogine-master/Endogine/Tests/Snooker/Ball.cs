using System;
using Endogine;

namespace Snooker
{
	/// <summary>
	/// Summary description for Ball.
	/// </summary>
	public class Ball : Endogine.GameHelpers.GameSprite
	{
		public class PropsAtCollision
		{
			public EPointF Loc;
			public EPointF Velocity;
			public float Time = -1;
			public Ball Ball;
			public PropsAtCollision()
			{}
			public void CopyToBall()
			{
				Ball.Loc = Loc;
				Ball.Velocity = Velocity;
			}
		}

		private float radius = 16;
		private float friction = 0.99f;

		public Ball()
		{
		}

		public float Radius
		{
			get {return radius;}
		}

		public override void EnterFrame()
		{
			base.EnterFrame ();

			//Enterframe (move) must be done before speed change - otherwise the collision
			//detection (that has already been done) will have been using old velocity values.
			this.Velocity*=friction;
			if (this.Velocity.Length < 0.05f)
				this.Velocity.Length = 0;

		}

		public float CheckCollisionsWithLinesFromTime(float a_fTime, ref PropsAtCollision propsAtCollision)
		{
			float fCollisionTime;
			EPointF pntCircleAtCollisionLoc;
			EPointF pntTouchLoc;

			EPointF locThis = this.Loc + this.Velocity*a_fTime;
			EPointF velThis = this.Velocity*(1.0f-a_fTime);

			float fFirstHitTime = 99;
			foreach (ERectangleF rctLine in GameMain.Instance.Table.Lines)
			{
				if (Endogine.Collision.Collision.CalcCircleLineCollision(locThis, this.Radius, velThis, rctLine, out pntTouchLoc, out pntCircleAtCollisionLoc))
				{
					fCollisionTime = (pntCircleAtCollisionLoc-locThis).Length / velThis.Length;
					if (fCollisionTime < fFirstHitTime)
					{
						fFirstHitTime = fCollisionTime;
						float fNormal = (float)Endogine.Collision.Collision.GetNormalAngle(rctLine)+(float)Math.PI/2;
						//the line's direction, it shouldn't matter if it's up or down, or left or right - same bounce regardless
						fNormal = Endogine.Collision.Collision.GetOrientationAngle(fNormal);

						float fVelAngle = velThis.Angle;
						float fAngleDiff = fVelAngle-fNormal;
						float fAfterBounceAngle = fVelAngle-fAngleDiff*2;

						propsAtCollision.Loc = pntCircleAtCollisionLoc;
						propsAtCollision.Velocity = EPointF.FromLengthAndAngle(velThis.Length, fAfterBounceAngle);
					}
				}
			}
			
			propsAtCollision.Ball = this;

			if (fFirstHitTime <= 1)
				propsAtCollision.Time =  fFirstHitTime * (1.0f-a_fTime) + a_fTime;
			return propsAtCollision.Time;
		}


		public float CheckCollisionsWithBallFromTime(Ball a_ball, float a_fTime, ref PropsAtCollision[] propsAtCollisionList)
		{
			propsAtCollisionList[0].Ball = this;
			propsAtCollisionList[1].Ball = a_ball;

			float fCollisionTime;
			EPointF pntCircleAtCollisionLoc;
			EPointF pntTouchLoc;

			EPointF locThis = this.Loc + this.Velocity*a_fTime;
			EPointF velThis = this.Velocity*(1.0f-a_fTime);

			EPointF locOther = a_ball.Loc + a_ball.Velocity*a_fTime;
			EPointF velOther = a_ball.Velocity*(1.0f-a_fTime);

			if (Endogine.Collision.Collision.CalcFirstCircleCircleCollisions(locThis, locOther, velThis, velOther, this.Radius, a_ball.Radius, out pntCircleAtCollisionLoc, out fCollisionTime))
			{
				propsAtCollisionList[0].Loc = locThis+velThis*fCollisionTime;
				propsAtCollisionList[1].Loc = locOther + velOther*fCollisionTime;

				propsAtCollisionList[0].Velocity = this.Velocity.Copy();
				propsAtCollisionList[1].Velocity = a_ball.Velocity.Copy();

				EPointF relativeVel = this.Velocity-a_ball.Velocity;

				EPointF pntTmp = EPointF.FromLengthAndAngle(this.Radius, (propsAtCollisionList[1].Loc-propsAtCollisionList[0].Loc).Angle);
				pntTouchLoc = propsAtCollisionList[0].Loc+pntTmp;

				EPointF debugLoc = propsAtCollisionList[1].Loc-propsAtCollisionList[0].Loc;

				//the amount of energy transferred to the other depends on the collision angle vs the C:\Documents and Settings\Jonas\Mina dokument\Visual Studio Projects\EndoTest01\Endogine\Sprite.csrelative velocity's angle
				float velAngle = relativeVel.Angle;
				EPointF pntX = propsAtCollisionList[0].Loc - pntTouchLoc;
				float angleOrientation = Endogine.Collision.Collision.GetOrientationAngle(pntX.Angle - velAngle);
				//if "wall" is same angle as relative move angle: no energy transferred.
				//if "wall" is 90 degrees off, all energy is transferred.
				float fTransferredEnergyFactor = Math.Abs((float)((angleOrientation-Math.PI/2)/(Math.PI/2)));
				//float fTransferredEnergyFactor = (float)(Math.Cos(angleOrientation*2));

				EPointF pntDiff = propsAtCollisionList[1].Loc - pntTouchLoc;
				EPointF pntAddedVel = EPointF.FromLengthAndAngle(relativeVel.Length*fTransferredEnergyFactor, pntDiff.Angle);

				propsAtCollisionList[1].Velocity+=pntAddedVel;

				//total energy and angle must be the same, so adjust the other's velocity (must be an thought error somewhere?)
				propsAtCollisionList[0].Velocity = (this.Velocity+a_ball.Velocity) - propsAtCollisionList[1].Velocity;
				
//				propsAtCollisionList[0].Velocity = EPointF.FromLengthAndAngle(
//					(this.Velocity+a_ball.Velocity).Length - propsAtCollisionList[1].Velocity.Length,
//					(this.Velocity+a_ball.Velocity).Angle - propsAtCollisionList[1].Velocity.Angle);
				propsAtCollisionList[0].Velocity.Length = this.Velocity.Length+a_ball.Velocity.Length - propsAtCollisionList[1].Velocity.Length;

				return fCollisionTime * (1.0f-a_fTime) + a_fTime;
			}
			else
				return -1;
		}

	}
}
