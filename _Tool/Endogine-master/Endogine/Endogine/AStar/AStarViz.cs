using System;
using System.Drawing;
using Endogine;

namespace Endogine.AStar
{
	/// <summary>
	/// Visualization class for debugging and testing of the AStarSearch class
	/// </summary>
	public class AStarViz
	{
		public AStarSearch m_astar;
		public Bitmap m_bmp;
		public int SquareSide = 1;

		public Color StartColor = Color.Green;
		public Color GoalColor = Color.Red;

		public bool UpdateWhileSearching;

		private int m_nNumFramesLeftToWaitAfterUpdate;

		public Sprite m_sp;

		public AStarViz(AStarSearch astar)
		{
			this.m_astar = astar;
			this.m_astar.SearchedCoordinate+=new Endogine.AStar.AStarSearch.SearchDelegate(m_astar_SearchedCoordinate);
			this.m_astar.ChangedStartCoordinate+=new Endogine.AStar.AStarSearch.SearchDelegate(m_astar_ChangedStartCoordinate);
			this.m_astar.ChangedGoalCoordinate+=new Endogine.AStar.AStarSearch.SearchDelegate(m_astar_ChangedGoalCoordinate);
			this.m_astar.SearchFinished+=new Endogine.AStar.AStarSearch.SearchDelegate(m_astar_SearchFinished);
			this.m_astar.SearchStarted+=new Endogine.AStar.AStarSearch.SearchDelegate(m_astar_SearchStarted);
			this.m_astar.ChangedAcceptableGoals+=new Endogine.AStar.AStarSearch.SearchDelegate(m_astar_ChangedAcceptableGoals);

			this.m_sp = new Sprite();
			this.m_sp.Name = "Map";
			this.m_sp.Parent = EH.Instance.Stage.RootSprite;
			this.m_sp.Scaling = new EPointF(2,2);
			this.m_sp.LocZ = 5001;
			this.m_sp.Ink = RasterOps.ROPs.AddPin;

			EH.Instance.EnterFrameEvent+=new EnterFrame(Instance_EnterFrameEvent);
		}

		public void Dispose()
		{
			EH.Instance.EnterFrameEvent-=new EnterFrame(Instance_EnterFrameEvent);
			this.m_sp.Dispose();
			this.m_astar = null;
		}

		public void UpdateGrid()
		{
			this.m_bmp = new Bitmap((this.m_astar.Size.X+1)*this.SquareSide, (this.m_astar.Size.Y+1)*this.SquareSide);
			this.ClearGrid();
		}

		public void ClearGrid()
		{
			Graphics g = Graphics.FromImage(this.m_bmp);
			g.FillRectangle(new SolidBrush(Color.Black), 0,0,this.m_bmp.Width,this.m_bmp.Height);

			for (int y = 0; y<this.m_astar.Size.Y; y++)
			{
				for (int x = 0; x<this.m_astar.Size.X; x++)
				{
					if (this.m_astar.IsOutBoundOrNotPassable(x,y))
						this.SetPosColor(new EPoint(x,y), Color.Gray);
				}
			}
			this.m_sp.Member = new MemberSpriteBitmap(this.m_bmp);
		}

		public void SetPosColor(EPoint pos, Color clr)
		{
			Graphics g = Graphics.FromImage(this.m_bmp);
			ERectangle rct = new ERectangle(pos.X, pos.Y, 1,1)*this.SquareSide;
			g.FillRectangle(new SolidBrush(clr), rct.ToRectangle());
		}

		private void m_astar_SearchedCoordinate(object sender, EPoint coordinate)
		{
			this.SetPosColor(coordinate, Color.Blue);
			if (this.UpdateWhileSearching)
			{
				this.m_sp.Member.Bitmap = this.m_bmp;
				this.m_astar.Paused = true;
				m_nNumFramesLeftToWaitAfterUpdate = 1;
			}
		}

		private void Instance_EnterFrameEvent()
		{
			if (m_nNumFramesLeftToWaitAfterUpdate > 0)
				m_nNumFramesLeftToWaitAfterUpdate--;
			else
			{
				if (this.m_astar.Paused)
					this.m_astar.Paused = false;
			}
		}

		private void m_astar_ChangedStartCoordinate(object sender, EPoint coordinate)
		{
			this.SetPosColor(coordinate, Color.Green);
			this.m_sp.Member.Bitmap = this.m_bmp;
		}

		private void m_astar_ChangedGoalCoordinate(object sender, EPoint coordinate)
		{
			this.SetPosColor(coordinate, Color.Red);
			this.m_sp.Member.Bitmap = this.m_bmp;
		}

		private void m_astar_SearchFinished(object sender, EPoint coordinate)
		{
			for (int i = 1; i < this.m_astar.Solution.Count-1; i++)
				this.SetPosColor((EPoint)this.m_astar.Solution[i], Color.Yellow);
			this.m_sp.Member.Bitmap = this.m_bmp;
		}

		private void m_astar_SearchStarted(object sender, EPoint coordinate)
		{
			this.ClearGrid();
			this.SetPosColor(this.m_astar.StartPos, this.StartColor);
			this.SetPosColor(this.m_astar.GoalPos, this.GoalColor);
			this.m_sp.Member.Bitmap = this.m_bmp;
		}

		private void m_astar_ChangedAcceptableGoals(object sender, EPoint coordinate)
		{
			foreach (EPoint pnt in this.m_astar.AcceptableGoals)
				this.SetPosColor(pnt, Color.FromArgb(0,200,200));
			this.m_sp.Member.Bitmap = this.m_bmp;
		}
	}
}
