using System;
using System.Collections;
using Endogine;

namespace Snooker
{
	/// <summary>
	/// Summary description for Table.
	/// </summary>
	public class Table : Sprite
	{
		private ArrayList collisionLines;
		private ArrayList balls;
		private ArrayList topoObjects;

		private float fTimeWithinFrame;
		private int nNumIterations;
		private Endogine.Forms.MeterBar meterBar;

		public Table()
		{
			EH.Instance.Stage.Color = System.Drawing.Color.ForestGreen;

			this.Ink = RasterOps.ROPs.BgTransparent;

			this.MemberName = "PlayArea";
			this.ChildListChangedBefore+=new ChildListChangedDelegate(Table_ChildListChangedBefore);

			this.balls = new ArrayList();

			this.topoObjects = new ArrayList();
//			TopologyObject topo;
//			
//			topo = new TopologyObject();
//			topo.Loc = new EPointF(30,30);
//			topo.MaxRange = 100;
//			topo.MinRange = 0;
//			topo.Depth = -10;
//			topo.Parent = this;
//			this.topoObjects.Add(topo);
//
//			topo = new TopologyObject();
//			topo.Loc = new EPointF(200,300);
//			topo.Depth = 10;
//			topo.MaxRange = 150;
//			topo.MinRange = 10;
//			topo.Parent = this;
//			this.topoObjects.Add(topo);


			this.meterBar = new Endogine.Forms.MeterBar();
			this.meterBar.Rect = new ERectangleF(0,0,100,20);
			this.meterBar.LocZ = 100;

			try
			{
				//TODO: an extra sprite seems to be created here
				Endogine.Serialization.EndogineXML.Load(
					EH.Instance.CastLib.DirectoryPath + "Snooker\\level1.sgr", this);
			}
			catch
			{}
		}

		public override void Dispose()
		{
			if (this.meterBar!=null)
				this.meterBar.Dispose();
			this.meterBar = null;

			foreach (TopologyObject topo in this.topoObjects)
				topo.Dispose();
			this.topoObjects.Clear();

			if (this.balls!=null)
			{
				for (int i = this.balls.Count-1; i >= 0; i--)
				{
					Ball ball = (Ball)this.balls[i];
					ball.Dispose();
				}
				this.balls.Clear();
			}

			base.Dispose ();
		}

		public override void CalcInParent()
		{
			base.CalcInParent ();
			
			this.collisionLines = new ArrayList();
			this.collisionLines.Add(ERectangleF.FromLTRB(this.Rect.Left, this.Rect.Top, this.Rect.Right, this.Rect.Top));
			this.collisionLines.Add(ERectangleF.FromLTRB(this.Rect.Right, this.Rect.Top, this.Rect.Right, this.Rect.Bottom));
			this.collisionLines.Add(ERectangleF.FromLTRB(this.Rect.Left, this.Rect.Bottom, this.Rect.Right, this.Rect.Bottom));
			this.collisionLines.Add(ERectangleF.FromLTRB(this.Rect.Left, this.Rect.Top, this.Rect.Left, this.Rect.Bottom));
		}

		public ArrayList Lines
		{
			get {return this.collisionLines;}
		}
		public ArrayList Balls
		{
			get {return balls;}
		}


		/// <summary>
		/// this event is raised when a sprite is added as a child to the table.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="child"></param>
		/// <param name="wasAdded"></param>
		/// <returns></returns>
		private bool Table_ChildListChangedBefore(object sender, Sprite child, bool wasAdded)
		{
			if (wasAdded)
			{
				if (child.GetType() == typeof(Endogine.Sprite))
				{
					Ball ball = null;
					if (child.Member != null && child.Member.Name.IndexOf("BallWhite") >= 0)
						ball = new PlayerBall();
					else
						ball = new Ball();

					ball.Parent = this;
					ball.LocZ = 10;

					//TODO: use serialization to copy props to new object!
					ball.Member = child.Member;
					ball.Loc = child.Loc.Copy();

					ball.RegPoint = ball.Member.Size/2;

					child.Dispose();

					return false;
				}
				else if (child.GetType() == typeof(Ball) || child.GetType() == typeof(PlayerBall))
				{
					Ball ball = (Ball)child;
					if (ball.Member != null)
						ball.RegPoint = child.Member.Size/2;
					this.balls.Add(ball);

					foreach (TopologyObject topo in this.topoObjects)
						topo.AddSprite(ball);
				}
				else if (child.GetType() == typeof(TopologyObject))
				{
					this.topoObjects.Add(child);
					((TopologyObject)child).DoneConstructing();
				}
			}
			else
			{
				if (child.GetType() == typeof(Ball) || child.GetType() == typeof(PlayerBall))
				{
					this.balls.Remove(child);
				}
			}
			return true;
		}

		public override void EnterFrame()
		{
			fTimeWithinFrame = 0;
			nNumIterations = 0;
			CheckCollisions();

			float fTotalEnergy = 0;
			EPointF pntTotalVelocity = new EPointF();
			foreach (Ball ball in this.balls)
			{
				pntTotalVelocity+=ball.Velocity;
				fTotalEnergy+=ball.Velocity.Length;
			}
			meterBar.Value = fTotalEnergy/100; //pntTotalVelocity.Length

			base.EnterFrame ();
			//must be done after collision checks, because otherwise
			//the balls will already have moved before the checks!
		}

		private void CheckCollisions()
		{
			float fTimeForFirstCollision = 99;
			Ball.PropsAtCollision propsAtCollisionFirst1 = null;
			Ball.PropsAtCollision propsAtCollisionFirst2 = null;

			Ball.PropsAtCollision[] propsAtCollisionList = new Snooker.Ball.PropsAtCollision[2];

			float fCollisionTime = 0;
			//no changes in Loc or Vel until which the first collision is determined. Then execute that collision and start over from that time
			for (int i = 0; i < this.balls.Count; i++)
			{
				Ball ball1 = (Ball)this.balls[i];

				//Check with other balls:
				for (int j = i+1; j < this.balls.Count; j++)
				{
					Ball ball2 = (Ball)this.balls[j];

					if (ball1.Velocity.Angle == 0 && ball2.Velocity.Angle == 0)
						continue;

					propsAtCollisionList[0] = new Snooker.Ball.PropsAtCollision();
					propsAtCollisionList[1] = new Snooker.Ball.PropsAtCollision();

					fCollisionTime = ball1.CheckCollisionsWithBallFromTime(ball2, fTimeWithinFrame, ref propsAtCollisionList);
					if (fCollisionTime >= 0 && fCollisionTime < fTimeForFirstCollision)
					{
						fTimeForFirstCollision = fCollisionTime;
						propsAtCollisionFirst1 = propsAtCollisionList[0];
						propsAtCollisionFirst2 = propsAtCollisionList[1];
					}
				}

				//Check with lines:
				Ball.PropsAtCollision propsAtCollision = new Snooker.Ball.PropsAtCollision();
				fCollisionTime = ball1.CheckCollisionsWithLinesFromTime(fTimeWithinFrame, ref propsAtCollision);
				if (fCollisionTime >= 0 && fCollisionTime < fTimeForFirstCollision)
				{
					fTimeForFirstCollision = fCollisionTime;
					propsAtCollisionFirst1 = propsAtCollision;
					propsAtCollisionFirst2 = null;
				}

			}
			if (fTimeForFirstCollision < 99)
			{
				fTimeWithinFrame = fTimeForFirstCollision;
				propsAtCollisionFirst1.CopyToBall();
				if (propsAtCollisionFirst2 != null)
					propsAtCollisionFirst2.CopyToBall();

				nNumIterations++;
				if (nNumIterations < 20)
					this.CheckCollisions();
				else
					nNumIterations = 99;
			}
		}
	}
}
