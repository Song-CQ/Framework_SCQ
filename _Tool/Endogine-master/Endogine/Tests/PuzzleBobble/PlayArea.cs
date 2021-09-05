using System;
using System.Drawing;
using System.Collections;
using Endogine;

namespace PuzzleBobble
{
	/// <summary>
	/// Summary description for PlayArea.
	/// </summary>
	public class PlayArea : Sprite
	{
		private ArrayList m_aPlayers; //normally, there's only one player per playArea, but why not allow more?
		public Grid Grid;
		public PathCalc m_pathCalc;
		private Sprite m_spBg;
		private Sprite m_spBgFill;
		public ArrayList m_aCollisionLines;

		private LevelManager m_level;

		public PlayArea()
		{
			//this.m_bNoScalingOnSetRect = true;
			m_spBg = new Sprite();
			m_spBg.Name = "Bg";
			m_spBg.MemberName = "PlayArea";
			m_spBg.Parent = this;
			m_spBg.RegPoint = new EPoint(30,30);

			m_spBgFill = new Sprite();
			m_spBgFill.Parent = this;
			m_spBgFill.Name = "BgFill";

			m_pathCalc = new PathCalc(this);
			Grid = new Grid(this);


			m_aCollisionLines = new ArrayList();
			EPointF pntTopLeft = Grid.GetGfxLocFromGridLoc(new EPointF(-1f,-0.0f));
			EPointF pntBottomRight = Grid.GetGfxLocFromGridLoc(new EPointF(Grid.GridSize.Width+0.5f,Grid.GridSize.Height));

			//first line is ceiling (where balls stick)
			m_aCollisionLines.Add(ERectangleF.FromLTRB(pntTopLeft.X,pntTopLeft.Y,pntBottomRight.X,pntTopLeft.Y));
			m_aCollisionLines.Add(ERectangleF.FromLTRB(pntTopLeft.X,pntTopLeft.Y,pntTopLeft.X,pntBottomRight.Y));
			m_aCollisionLines.Add(ERectangleF.FromLTRB(pntBottomRight.X,pntTopLeft.Y,pntBottomRight.X,pntBottomRight.Y));

			m_aPlayers = new ArrayList();
			AddPlayer(new Hashtable());

			Hashtable htKeysForPlayer2 = new Hashtable();
			htKeysForPlayer2.Add("left", System.Windows.Forms.Keys.A);
			htKeysForPlayer2.Add("right", System.Windows.Forms.Keys.D);
			htKeysForPlayer2.Add("up", System.Windows.Forms.Keys.W);
			htKeysForPlayer2.Add("shoot", System.Windows.Forms.Keys.S);
			AddPlayer(htKeysForPlayer2);
			//TODO: A fun cooperative mode would be to force players to take turns.


			//TODO: write a main game class which manages levels and multiple playAreas.
			m_level = new LevelManager(this);
			NextLevel();
		}

		public void AddPlayer(Hashtable a_htDefaultKeys)
		{
			Player player = new Player(this, a_htDefaultKeys);
			player.Parent = this;
			if (m_aPlayers.Count == 0)
				player.Loc = Grid.GetGfxLocFromGridLoc(new EPoint(Grid.GridSize.Width/2, Grid.GridSize.Height));
			else
				player.Loc = Grid.GetGfxLocFromGridLoc(new EPoint(Grid.GridSize.Width/2-4, Grid.GridSize.Height));
			m_aPlayers.Add(player);
		}

		public void RemovedBalls(int a_nNumInColorChain, int a_nNumFallen)
		{
			//TODO: need to know whose ball it was!
			int nPoints = (int)Math.Pow(2,a_nNumFallen-1)*20 + a_nNumFallen*10;

			if (Grid.GetAllBalls().Count == 0)
			{
				NextLevel();
			}
		}

		private void NextLevel()
		{
			m_level.Level++;
			ArrayList balls = m_level.GetLevelBalls(m_level.Level);
			AddBallsToGrid(balls);

			foreach (Player player in m_aPlayers)
			{
				player.NextLevel();
			}
		}

		public void AddBallsToGrid(ArrayList a_balls)
		{
			foreach (Ball ball in a_balls)
			{
				ball.Loc = Grid.GetGfxLocFromGridLoc(ball.GridLoc);
				Grid.NoRemoveSetBallOnLoc(ball.GridLoc, ball);
			}
		}
	}
}
