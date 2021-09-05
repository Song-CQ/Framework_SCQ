using System;
using System.Collections;
using Endogine;

namespace CaveHunter
{
	/// <summary>
	/// Summary description for Obstacles.
	/// </summary>
	public class Obstacles
	{
		private ArrayList m_obstacles;

		public Obstacles()
		{
			m_obstacles = new ArrayList();
			this.CreateNew(320);
			this.CreateNew(-1);
		}

		private void CreateNew(int a_nLocX)
		{
			Sprite sp = new Sprite();
			sp.Name = "Obstacle";
			sp.LocZ = 5;
            //TODO: new animation system
            MemberSpriteBitmap mb = (MemberSpriteBitmap)EndogineHub.Instance.CastLib.GetOrCreate("asteroid01");
            //mb.NumFramesPerAxis = new EPoint(7,6);
			sp.Member = mb;
	
			if (a_nLocX <= 0)
				a_nLocX = GameMain.Instance.CaveWalls.GetMaxX();
			float[] walls = GameMain.Instance.CaveWalls.GetWallsYOnX(a_nLocX);

			Random rnd = new Random();
			sp.Loc = new EPointF(a_nLocX, (walls[1]-walls[0])*(float)rnd.NextDouble()+walls[0]);
            //sp.AutoAnimator.StepSize = 0.1f;
			sp.Scaling = new EPointF(0.3f,0.3f);

			BhReportWhenOutside bh = new BhReportWhenOutside();
			sp.AddBehavior(bh);
			bh.Outside+=new CaveHunter.BhReportWhenOutside.OutsideDelegate(bh_Outside);

			m_obstacles.Add(sp);
		}

		private void bh_Outside(Sprite sp)
		{
			m_obstacles.Remove(sp);
			sp.Dispose();
			CreateNew(-1);
		}

		public bool CheckCollision(Sprite sp)
		{
			foreach (Sprite obstacle in m_obstacles)
			{
				if (obstacle.GetCollisionPoint(sp) != null)
					return true;
			}
			return false;
		}

		public void Dispose()
		{
			foreach (Sprite obstacle in m_obstacles)
				obstacle.Dispose();
			m_obstacles.Clear();
		}
	}
}
