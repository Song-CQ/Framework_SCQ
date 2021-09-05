using System;
using System.Collections;
using System.Drawing;
using Endogine;

namespace PuzzleBobble
{
	/// <summary>
	/// Summary description for Grid.
	/// </summary>
	public class Grid
	{
		public float BallDiameter = 32;
		public float HeightProportion;
		public int NumBallsWidth = 7;
		public Size GridSize;
		public int CeilingNumStepsDown = 0;

		private class ChainInfo
		{
			public int State;
			public ArrayList Balls;
			public ChainInfo()
			{
				State = 0;
				Balls = new ArrayList();
			}
		}

		public Ball[,] m_aGrid;

		private ChainInfo m_aCurrentChainInfo;
		private int m_nCurrentChainNum = 0;
		private PlayArea m_playArea;

		public Grid(PlayArea a_playArea)
		{
			m_playArea = a_playArea;
			HeightProportion = (float)Math.Sqrt(0.75)*2;
			//grid is counted so width is twice the number of balls that fit (since every second line is in 0.5 phase)
			GridSize = new Size(NumBallsWidth*2+1,12);
			m_aGrid = new Ball[GridSize.Width, GridSize.Height];
		}

		public EPointF GetGridLocFromGfxLoc(EPointF a_pnt)
		{
			return new EPointF(a_pnt.X*2/BallDiameter, a_pnt.Y/BallDiameter*2/HeightProportion);
			//return new Point((int)(a_pnt.X*2/BallDiameter), (int)Math.Round((a_pnt.Y/BallDiameter*2/HeightProportion)));
		}

		public ArrayList GetBallTypes()
		{
			ArrayList aTypes = new ArrayList();
			ArrayList aBalls = GetAllBalls();
			foreach (Ball ball in aBalls)
			{
				if (!aTypes.Contains(ball.BallType))
					aTypes.Add(ball.BallType);
			}
			return aTypes;
		}

		public ArrayList GetAllBalls()
		{
			ArrayList aBalls = new ArrayList();
			for (int y = 0; y < GridSize.Height; y++)
			{
				for (int x = 0; x < GridSize.Width; x++)
				{
					if (m_aGrid[x,y]!=null)
						aBalls.Add(m_aGrid[x,y]);
				}
			}
			return aBalls;
		}

		public EPointF GetGridStartLoc()
		{
			return new EPointF(GridSize.Width/2, GridSize.Height);
		}
		public EPointF GetGfxLocFromGridLoc(EPointF a_pnt)
		{
			return new EPointF(
				(a_pnt.X)*BallDiameter/2,
				(a_pnt.Y)*BallDiameter/2*HeightProportion);
		}
		public EPointF GetGfxLocFromGridLoc(EPoint a_pnt)
		{
			return GetGfxLocFromGridLoc(new EPointF(a_pnt.X, a_pnt.Y));
		}

		public EPoint RoundToClosestGridLoc(EPointF a_pntLoc)
		{
			int y = (int)Math.Round(a_pntLoc.Y);
			float x = a_pntLoc.X/2 + (y%(2+CeilingNumStepsDown))*0.5f;
			x = (int)Math.Round(x)*2 - (y % (2+CeilingNumStepsDown));
			if (x < 0)
				x = 0;
			else if (x >= GridSize.Width)
				x = GridSize.Width-1-y%2;
			return new EPoint((int)x, y);
		}

		public Ball GetBallOnLoc(EPoint a_pnt)
		{
			if (a_pnt.X < 0 || a_pnt.Y < 0 || a_pnt.X >= GridSize.Width || a_pnt.Y >= GridSize.Height)
				return (Ball)null;
			return (Ball)m_aGrid[a_pnt.X, a_pnt.Y];
		}

		public void NoRemoveSetBallOnLoc(EPoint a_pnt, Ball a_ball)
		{
			m_aGrid[a_pnt.X, a_pnt.Y] = a_ball;
		}


		public void SetBallOnLoc(EPoint a_pnt, Ball a_ball)
		{
			if (a_pnt.X < 0 || a_pnt.X >= GridSize.Width || a_pnt.Y < 0)
				return;
			m_aGrid[a_pnt.X, a_pnt.Y] = a_ball;
			a_ball.GridLoc = a_pnt;
			a_ball.Loc = GetGfxLocFromGridLoc(a_pnt);

			ResetChainNums();
			m_nCurrentChainNum = 0;
			ArrayList aSameColor = CalcColorChain(a_pnt);

			if (aSameColor.Count > 2)
			{
				ArrayList aAllRemove = new ArrayList();
				//remove balls in color chain, and then all balls that are no longer connected to ceiling!

				//first color chain, and also find all neighbours to the color chain
				ArrayList aAllNeighbours = new ArrayList();
					//make a list of all neighbours:
					foreach (Ball removeball in aSameColor)
					{
						ArrayList aNeighbours = GetNeighbours(removeball);
						foreach (Ball neighbour in aNeighbours)
						{
							if (aAllNeighbours.Contains(neighbour) == false && aSameColor.Contains(neighbour) == false)
								aAllNeighbours.Add(neighbour);
						}
						RemoveBallAtLoc(removeball.GridLoc);
						removeball.Burst();
						//aAllRemove.Add(removeball);
					}

				ResetChainNums();
				m_nCurrentChainNum = 0;
				//now for each neighbour, add to remove list if not connected to ceiling:
				foreach (Ball ball in aAllNeighbours)
				{
					//if one of these balls is connected to another, the recursive function will already have added it to its remove list
					if (ball.ChainNum < 0)
					{
						EndogineHub.Put("New chain");
						m_aCurrentChainInfo = new ChainInfo();
						if (RecursiveCalcConnection(ball) == false) //the chain is not connected to ceiling
						{
							foreach (Ball removeball in m_aCurrentChainInfo.Balls)
							{
								aAllRemove.Add(removeball);
								RemoveBallAtLoc(removeball.GridLoc);
							}
						}
					}
				}

				foreach (Ball removeball in aAllRemove)
				{
					removeball.Fall(); //Dispose(); //.Color = Color.FromArgb(100,100,100);
				}

				m_playArea.RemovedBalls(aSameColor.Count, aAllRemove.Count);
			}
		}

		private void ResetChainNums()
		{
			ArrayList balls = GetAllBalls();
			foreach (Ball ball in balls)
				ball.ChainNum = -1;
		}
		private void RemoveBallAtLoc(EPoint a_pnt)
		{
			m_aGrid[a_pnt.X, a_pnt.Y] = (Ball)null;
		}



		#region Remove reaction methods

		public ArrayList CalcColorChain(EPoint a_pntLoc)
		{
			m_aCurrentChainInfo = new ChainInfo();

			Ball ball = GetBallOnLoc(a_pntLoc);
			RecursiveCalcColorChain(ball);
			return m_aCurrentChainInfo.Balls; //		return duplicate(currentChain)
		}
		private void RecursiveCalcColorChain(Ball a_ball)
		{
			m_aCurrentChainInfo.Balls.Add(a_ball);
			a_ball.ChainNum = m_nCurrentChainNum;
  
			ArrayList aNeighbours = GetNeighbours(a_ball);
			foreach (Ball ball in aNeighbours)
			{
				if (ball != null) 
				{
					if (ball.BallType == a_ball.BallType) 
					{
						if (ball.ChainNum < m_nCurrentChainNum) //m_nStartChainNum) 
						{ //not checked before
							RecursiveCalcColorChain(ball);
						}
					}
				}
			}
		}

		private ArrayList GetNeighbours(Ball a_ball)
		{
			ArrayList aBalls = new ArrayList();
			ArrayList aLocs = a_ball.GetNeighbourLocs();
			foreach (EPoint loc in aLocs)
			{
				Ball ball = this.GetBallOnLoc(loc);
				if (ball!=null)
					aBalls.Add(ball);
			}
			return aBalls;
		}

		private bool RecursiveCalcConnection(Ball a_ball)
		{
			m_aCurrentChainInfo.Balls.Add(a_ball);
			a_ball.ChainNum = 1;
			if (a_ball.GridLoc.Y == 0)
				m_aCurrentChainInfo.State = 1; //it's connected to ceiling

			ArrayList aNeighbours = GetNeighbours(a_ball);
			foreach (Ball ball in aNeighbours)
			{
				//m_playArea.EndogineHub.Put(ball.BallType.ToString() + "  " + ball.GridLoc.ToString());

				if (ball==null)
				{
					//m_playArea.EndogineHub.Put("Null");
					continue;
				}
				if (ball.ChainNum > 0)
				{
					//m_playArea.EndogineHub.Put("Is in chain:"+ball.ChainNum.ToString());
					continue; //Connected to the chain
				}
				RecursiveCalcConnection(ball);
				//if (RecursiveCalcConnection(ball))
				//	return true;
			}

			if (m_aCurrentChainInfo.State == 1)
				return true;

			return false;
		}
		#endregion  
	}
}
