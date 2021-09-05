using System;
using System.Collections;
using Endogine;

namespace SideScroller
{
	/// <summary>
	/// Summary description for GameMain.
	/// </summary>
	public class GameMain
	{
		private StarField m_starField;
		private Player m_player;
		private ArrayList m_aAsteroids;

		public GameMain()
		{
			m_starField = new StarField();
			m_player = new Player(this);
			m_aAsteroids = new ArrayList();

			//The aOKLocs is generated so no asteroids will appear close to the ship
			EPoint pntStageSize = EndogineHub.Instance.Stage.Size;
			ArrayList aOKLocs = new ArrayList();
			EPoint pntNumPositions = new EPoint(6,6);
			ERectangle rctFreePositions = new ERectangle(2,2,2,2);
			for (int y = 0; y < pntNumPositions.Y; y++)
			{
				if (y >= rctFreePositions.Y && y < rctFreePositions.Bottom)
					y+=rctFreePositions.Height;
				for (int x = 0; x < pntNumPositions.X; x++)
				{
					if (x >= rctFreePositions.X && x < rctFreePositions.Right)
						x+=rctFreePositions.Width;
					EPoint pnt = new EPoint(x,y) * pntStageSize/(pntNumPositions-new EPoint(1,1)) - pntStageSize/2;;
					aOKLocs.Add(pnt);
				}
			}

			Random rnd = new Random();
			for (int i = 0; i < 4; i++)
			{
				Asteroid asteroid = new Asteroid(this, 3);
				asteroid.Velocity = new EPointF((float)rnd.NextDouble()-0.5f, (float)rnd.NextDouble()-0.5f);

				int nRndPos = rnd.Next(aOKLocs.Count);
				EPoint pntLoc = (EPoint)aOKLocs[nRndPos];
				aOKLocs.RemoveAt(nRndPos);
				asteroid.Loc = pntLoc.ToEPointF();
			}
		}

		public ArrayList Asteroids
		{
			get {return m_aAsteroids;}
		}
		public void Dispose()
		{
			m_starField.Dispose();
			m_starField = null;

			m_player.Dispose();
			m_player = null;

			for (int i = m_aAsteroids.Count-1; i>=0; i--)
			{
				((Asteroid)m_aAsteroids[i]).Dispose();
			}
		}
	}
}
