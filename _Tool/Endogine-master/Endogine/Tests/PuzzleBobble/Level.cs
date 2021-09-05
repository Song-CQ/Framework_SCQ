using System;
using System.Drawing;
using System.Collections;
using Endogine;

namespace PuzzleBobble
{
	/// <summary>
	/// Summary description for LevelManager
	/// </summary>
	public class LevelManager
	{
		PlayArea m_playArea;
		private int m_nLevel = 0;
		public LevelManager(PlayArea a_playArea)
		{
			m_playArea = a_playArea;
		}

		private int[,] GetLevel(int a_nLevel)
		{
			//"Red"	"Green";	"Blue";	"Yellow";"Purple";"White";7:"Black";

			int[,] aLevel;
			switch (a_nLevel)
			{
				case 1:
					aLevel = new int[,]{
			{1,1,2,2,3,3,4,4},
			 {1,1,2,2,3,3,4,0},
			{3,3,4,4,1,1,2,2},
			 {3,4,4,1,1,2,2,0}};
					break;

				case 2:
					aLevel = new int[,]{
			{0,0,0,7,7,0,0,0},
			 {0,0,0,3,0,0,0,0},
			{0,0,0,2,0,0,0,0},
			 {0,0,0,3,0,0,0,0},
			{0,0,0,5,0,0,0,0},
			 {0,0,2,2,0,0,0,0},
			{0,0,0,3,0,0,0,0},
			 {0,0,0,6,0,0,0,0}
					};
					break;
				case 3:
					aLevel = new int[,]{
			{0,7,7,0,5,3,3,0},
			 {0,6,0,0,0,5,0,0},
			{0,3,0,0,0,2,0,0},
			 {0,6,0,0,0,2,0,0},
			{0,6,0,0,0,2,0,0},
			 {0,1,0,0,0,6,0,0},
			{0,6,0,0,0,3,0,0},
			 {0,1,0,0,0,2,0,0}
									   };
					break;
				default:
					aLevel = new int[,]{
			{0,7,7,0,5,3,3,0},
			 {0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0},
			 {0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0},
			 {0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0},
			 {0,0,0,0,0,0,0,0}
					};
					break;
			}
			return aLevel;
		}

		public ArrayList GetLevelBalls(int a_nLevel)
		{
			int[,] aLevel = GetLevel(a_nLevel);
			ArrayList balls = new ArrayList();
			Ball ball;
			for (int y = 0; y < aLevel.GetLength(0); y++)
			{
				for (int x = 0; x < aLevel.GetLength(1); x++)
				{
					int nType = aLevel[y,x]-1;
					if (nType < 0)
						continue;
					ball = new Ball(nType, m_playArea);
					ball.GridLoc = new EPoint(x*2+y%2,y);
					balls.Add(ball);
				}
			}
			return balls;
		}

		public int Level
		{
			get {return m_nLevel;}
			set {m_nLevel = value;}
		}
	}
}
