using System;
using System.Collections;
using Endogine;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Summary description for GameMain.
	/// </summary>
	public class GameMain
	{
		public Player m_player;
		public InvadersGrid m_invadersGrid;
		public static GameMain Instance;
		public System.Drawing.Color m_clrOffwhite;
		public ArrayList m_covers;
		public Score m_score;
		public LivesLeft m_livesLeft;

		private ArrayList m_interfaceSprites;

		public GameMain()
		{
			Instance = this;

            string path = AppSettings.Instance.GetPath("Media") + "SpaceInv";
            AppSettings.Instance.AddPath("Media", path);
            PicRef.ScanDirectory(path);

			this.m_clrOffwhite = System.Drawing.Color.FromArgb(214,181,140);

			this.m_interfaceSprites = new ArrayList();

			this.m_score = new Score();

			this.m_livesLeft = new LivesLeft();

			Sprite sp = new Sprite();
			sp.SetGraphics("Screen");
			sp.LocZ = 500;
			sp.Ink = RasterOps.ROPs.AddPin;
			this.m_interfaceSprites.Add(sp);
			
			this.AddColorizers();


			this.m_covers = new ArrayList();
			Cover cover;
			for (int nCoverNum = 0; nCoverNum < 4; nCoverNum++)
			{
				cover = new Cover();
				cover.Loc = new EPointF(170 + 90 + (nCoverNum-1)*90,369);
				this.m_covers.Add(cover);
			}

			ERectangleF rctPlayArea = ERectangleF.FromLTRB(155,20,550,400); //new ERectangleF(145,10,350,200); //155 550
			this.m_invadersGrid = new InvadersGrid();

			//ERectangleF rctPlayerConstraints = rctPlayArea+ERectangleF.FromLTRB(-10,0,-55,0);
			this.m_player = new Player();
		}

		public void Dispose()
		{
			this.m_player.Dispose();
			this.m_invadersGrid.Dispose();
			foreach (Cover cover in this.m_covers)
				cover.Dispose();
			this.m_score.Dispose();
			this.m_livesLeft.Dispose();
			foreach (Sprite sp in this.m_interfaceSprites)
				sp.Dispose();
		}

		private void AddColorizers()
		{
			int locZ = 490;
			Colorizer color;

			color = new Colorizer();
			color.Rect = ERectangleF.FromLTRB(98,12,549,110);
			color.Color = System.Drawing.Color.FromArgb(0,0,255);
			color.LocZ = locZ;
			this.m_interfaceSprites.Add(color);

			color = new Colorizer();
			color.Rect = ERectangleF.FromLTRB(98,110,549,202);
			color.Color = System.Drawing.Color.FromArgb(0,255,255);
			color.LocZ = locZ;
			this.m_interfaceSprites.Add(color);

			color = new Colorizer();
			color.Rect = ERectangleF.FromLTRB(98,202,549,330);
			color.Color = System.Drawing.Color.FromArgb(0,0,255);
			color.LocZ = locZ;
			this.m_interfaceSprites.Add(color);

			color = new Colorizer();
			color.Rect = ERectangleF.FromLTRB(98,202,549,330);
			color.Color = System.Drawing.Color.FromArgb(255,0,255);
			color.LocZ = locZ;
			this.m_interfaceSprites.Add(color);
		}
	}
}
