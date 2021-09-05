using System;
using System.Collections;
using System.Drawing;
using Endogine;

namespace PuzzleBobble
{
	/// <summary>
	/// Summary description for Ball.
	/// </summary>
	public class Ball : Sprite
	{
		private EPointF m_pntVel;
		private PlayArea m_playArea;

		public int BallType = 0;
		public int ChainNum = 0; //for storing while calculating chains and connections in grid
		private EPoint m_pntGridLoc;
		public ArrayList m_aNeighbourLocs;

		private int m_nBurstCnt = 0;
		private int m_nFallCount = 0;
		
		public Ball(int a_nType, PlayArea a_playArea)
		{
			Parent = a_playArea;
			m_playArea = a_playArea;

			string sClr = "";
			switch (a_nType)
			{
				case 0:
					sClr = "Red";
					break;
				case 1:
					sClr = "Green";
					break;
				case 2:
					sClr = "Blue";
					break;
				case 3:
					sClr = "Yellow";
					break;
				case 4:
					sClr = "Purple";
					break;
				case 5:
					sClr = "White";
					break;
				case 6:
					sClr = "Black";
					break;
				case 7:
					sClr = "Orange";
					break;
			}
			BallType = a_nType;

			MemberName = "Ball"+sClr;
			this.CenterRegPoint();

			m_pntVel = new EPointF(0,0);
		}

		public void Shoot(int a_nAngle)
		{
			float fSpeed = 8;
			double dAngle = (double)a_nAngle*Math.PI/180;
			m_pntVel = new EPointF(-fSpeed*(float)Math.Sin(dAngle), -fSpeed*(float)Math.Cos(dAngle));
		}


		public override void EnterFrame()
		{
			base.EnterFrame();

			if (m_nBurstCnt > 0)
			{
				int nMax = 10;
				float fFact = 1+(float)m_nBurstCnt/nMax;
				Scaling = new EPointF(fFact, fFact);
				Blend = 100-100*m_nBurstCnt/nMax;
				m_nBurstCnt++;
				if (m_nBurstCnt > nMax)
				{
					m_nBurstCnt = 0;
					Dispose();
					return;
				}
			}
			if (m_nFallCount > 0)
			{
				m_pntVel.Y+=0.7f;
				LocX+=m_pntVel.X;
				LocY+=m_pntVel.Y;
				if (LocY > 400)
					Dispose();
				m_nFallCount++;
				return;
			}

			if (m_pntVel.X == 0 && m_pntVel.Y == 0)
				return;

			EPoint pntStick;
			EPointF pntBounce;
			if (m_playArea.m_pathCalc.GetFirstStickOrBounce(ref m_pntLoc, ref m_pntVel, out pntStick, out pntBounce, true))
			{
				m_playArea.Grid.SetBallOnLoc(pntStick, this);
			}
			else
				Loc = m_pntLoc;
		}

		public void PushToNextWaitingPos(int a_nPos)
		{

		}

		public void Burst()
		{
			m_nBurstCnt = 1;
		}

		public void Fall()
		{
			m_nFallCount = 1;
			Random rnd = new Random(GridLoc.X*100+GridLoc.Y);
			m_pntVel = new EPointF((float)(rnd.NextDouble()*6-3),(float)(-rnd.NextDouble()*5-2));
		}

		public EPoint GridLoc
		{
			set
			{
				m_pntGridLoc = value;

				//calc cached neighbour list for faster calculations of chains
				m_aNeighbourLocs = new ArrayList();
				if (value.Y > 0)
				{
					m_aNeighbourLocs.Add(new EPoint(value.X+1, value.Y-1));
					m_aNeighbourLocs.Add(new EPoint(value.X-1, value.Y-1));
				}
				if (value.X >= 2)
					m_aNeighbourLocs.Add(new EPoint(value.X-2, value.Y));
				if (value.X <= m_playArea.Grid.GridSize.Width - 2)
					m_aNeighbourLocs.Add(new EPoint(value.X+2, value.Y));

				m_aNeighbourLocs.Add(new EPoint(value.X-1, value.Y+1));
				m_aNeighbourLocs.Add(new EPoint(value.X+1, value.Y+1));
			}
			get {return m_pntGridLoc;}
		}

		public ArrayList GetNeighbourLocs()
		{
			return m_aNeighbourLocs;
		}
	}
}
