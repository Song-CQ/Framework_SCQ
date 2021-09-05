using System;
using System.Drawing;
using System.Collections;
using Endogine;

namespace CaveHunter
{
	/// <summary>
	/// Summary description for CaveWalls.
	/// </summary>
	public class CaveWalls
	{
		/// <summary>
		/// Used internally for representing the lines in the top and bottom wall tiles during construction of their bitmaps
		/// </summary>
		private class TileLines
		{
			public PointF[] Points;
			public float MinY;
			public float MaxY;
			public TileLines(PointF[] a_aPoints)
			{
				MinY = 9999;
				MaxY = -9999;
				Points = a_aPoints;
				foreach (PointF pnt in Points)
				{
					MinY = Math.Min(MinY, pnt.Y);
					MaxY = Math.Max(MaxY, pnt.Y);
				}
				for (int x = 0; x < Points.GetLength(0); x++)
					Points[x] = new PointF(x,Points[x].Y-MinY);
			}
			public PointF[] GetPolygon(bool bFillDownside)
			{
				PointF[] polygon = new PointF[Points.Length+3];
				int i = 0;
				for (; i < Points.Length; i++)
					polygon[i] = Points[i];

				float y = 0;
				if (bFillDownside)
					y = MaxY-MinY;

				polygon[i++] = new PointF(Points[Points.Length-1].X, y);
				polygon[i++] = new PointF(0, y);
				polygon[i++] = Points[0];

				return polygon;
			}
		}

		private float m_fMidY = 0;
		private int m_nTileWidth = 32;
		private float m_fCaveHeight = 0;
		private Random m_rnd;
		private Endogine.Procedural.Noise m_noise;

		private ERectangle m_rctPlayArea;

		private ArrayList m_wallSprites;

		private SortedList m_locYPairs;

		public CaveWalls()
		{
			m_noise = new Endogine.Procedural.Noise();
			m_noise.Frequency = 0.1f;
			m_noise.Octaves = 4;
			m_noise.Decay = 0.5f;

			m_wallSprites = new ArrayList();
			m_locYPairs = new SortedList();

			m_rctPlayArea = new ERectangle(0,0,640,480);
			m_fCaveHeight = m_rctPlayArea.Height*0.8f;

			m_rnd = new Random();
			for (int i = 0; i < m_rctPlayArea.Width/m_nTileWidth+1; i++)
			{
				this.CreateNewTiles(i*m_nTileWidth);
			}
		}

		private void CreateNewTiles(int a_nLocX)
		{
			//use perlin noise to generate wall structure
			ArrayList aNoise = new ArrayList();
			for (int x = 0; x < m_nTileWidth; x++)
				aNoise.Add(0f);
			m_noise.Offset = new EPointF(a_nLocX, 0);
			m_noise.FillArray(aNoise);

			PointF[] aPoints1 = new PointF[m_nTileWidth];
			PointF[] aPoints2 = new PointF[m_nTileWidth];
			for (int x = 0; x < m_nTileWidth; x++)
			{
				m_fCaveHeight*=0.9997f;
				float fAllowedY = m_rctPlayArea.Height - m_fCaveHeight;
				m_fMidY=((float)aNoise[x])*fAllowedY;

				float[] yPair = new float[]{m_fMidY, m_fMidY+m_fCaveHeight};

				aPoints1[x] = new PointF(x,yPair[0]);
				aPoints2[x] = new PointF(x,yPair[1]);

				m_locYPairs.Add(x+a_nLocX, yPair);
			}

			ArrayList aBothTileLines = new ArrayList();
			aBothTileLines.Add(new TileLines(aPoints1));
			aBothTileLines.Add(new TileLines(aPoints2));
			

			for (int i = 0; i < 2; i++)
			{
				TileLines tileLines = (TileLines)aBothTileLines[i];
				Bitmap bmp;
				Graphics g;
				float fYDiff = tileLines.MaxY-tileLines.MinY+1;
				bmp = new Bitmap(m_nTileWidth,(int)fYDiff);
				g = Graphics.FromImage(bmp);
				g.FillRectangle(new SolidBrush(Color.Black), 0,0,bmp.Width,bmp.Height);
				//g.DrawLines(new Pen(Color.White, 2), tileLines.Points);
				//create polygon from lines:
				PointF[] polygon = tileLines.GetPolygon(i == 1);
				g.FillPolygon(new SolidBrush(Color.White), polygon);
				g.Dispose();

				MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
				Sprite sp = new Sprite();
				sp.Member = mb;
				sp.LocX = a_nLocX;
				sp.LocY = tileLines.MinY;
				if (i == 0)
					sp.Name = "Ceiling";
				else
					sp.Name = "Floor";

				BhReportWhenOutside bh = new BhReportWhenOutside();
				sp.AddBehavior(bh);
				bh.Outside+=new CaveHunter.BhReportWhenOutside.OutsideDelegate(bh_Outside);

				m_wallSprites.Add(sp);


				Sprite spFill = new Sprite();
				spFill.MemberName = "BG1";
				if (i == 0)
					spFill.Rect = new ERectangleF(sp.LocX, m_rctPlayArea.Top, this.m_nTileWidth, sp.Rect.Top);
				else
					spFill.Rect = new ERectangleF(sp.LocX, sp.Rect.Bottom, this.m_nTileWidth, m_rctPlayArea.Bottom-sp.Rect.Bottom);
				bh = new BhReportWhenOutside();
				spFill.AddBehavior(bh);
				bh.Outside+=new CaveHunter.BhReportWhenOutside.OutsideDelegate(bh_Outside);

				m_wallSprites.Add(spFill);
			}
		}

		private void bh_Outside(Sprite sp)
		{
			m_wallSprites.Remove(sp);
			sp.Dispose();

			if (sp.Name == "Ceiling")
			{
				for (int i = 0; i < m_nTileWidth; i++)
					m_locYPairs.RemoveAt(0);

				CreateNewTiles((int)((Sprite)m_wallSprites[m_wallSprites.Count-1]).LocX + this.m_nTileWidth);
			}
		}

		public float[] GetWallsYOnX(int a_nX)
		{
			return (float[])m_locYPairs[a_nX];
		}

		public int GetMaxX()
		{
			return (int)m_locYPairs.GetKey(m_locYPairs.Count-1);
		}

		public bool CheckCollision(Sprite sp)
		{
			foreach (Sprite wall in m_wallSprites)
			{
				if (wall.GetCollisionPoint(sp) != null)
					return true;
			}
			return false;
		}

		public void Dispose()
		{
			foreach (Sprite wall in m_wallSprites)
				wall.Dispose();
			m_wallSprites.Clear();
		}

	}
}
