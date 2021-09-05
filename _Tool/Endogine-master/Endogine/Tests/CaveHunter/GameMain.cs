using System;
using Endogine;

namespace CaveHunter
{
	/// <summary>
	/// Summary description for GameMain.
	/// </summary>
	public class GameMain
	{
		private Player m_player;
		private Obstacles m_obstacles;
		private CaveWalls m_caveWalls;

		private static GameMain m_instance;

		public GameMain()
		{
			m_instance = this;

			Start();

			EndogineHub.Instance.EnterFrameEvent+=new EnterFrame(GameMain_EnterFrameEvent);
		}

		public static GameMain Instance
		{
			get {return m_instance;}
		}

		public CaveWalls CaveWalls
		{
			get {return m_caveWalls;}
		}
		public Obstacles Obstacles
		{
			get {return m_obstacles;}
		}


		private void GameMain_EnterFrameEvent()
		{
			if (m_player == null)
				return;

			Camera cam = EndogineHub.Instance.Stage.Camera;
			cam.Loc = new EPointF(m_player.LocX-90, cam.Loc.Y);
		}

		public void Start()
		{
			Cleanup();
			m_caveWalls = new CaveWalls();
			m_player = new Player();
			m_obstacles = new Obstacles();
		}

		private void Cleanup()
		{
			if (m_caveWalls != null)
				m_caveWalls.Dispose();

			if (m_player != null)
				m_player.Dispose();
			
			if (m_obstacles != null)
				m_obstacles.Dispose();
		}

		public void Dispose()
		{
			Cleanup();
		}
	}
}
