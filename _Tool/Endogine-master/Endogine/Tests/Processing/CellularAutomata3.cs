//C# adaption of Cellular Automata 3 by Mike Davis <http://www.lightcycle.org>

using System;
using System.Drawing;
using Endogine;

namespace Tests.Processing
{
	/// <summary>
	/// Summary description for CellularAutomata3.
	/// </summary>
	public class CellularAutomata3 : Canvas
	{

		class Cell
		{
			int x, y;
			CellularAutomata3 cv;
			public Cell(int xin, int yin, CellularAutomata3 canvas)
			{
				x = xin;
				y = yin;
				cv = canvas;
			}
			// Will move the cell (dx, dy) units if that space is empty
			void Move(int dx, int dy) 
			{
				if (cv.GetPixel(x + dx, y + dy) == cv.BgColor) 
				{
					cv.SetPixel(x + dx, y + dy, cv.GetPixel(x, y));
					cv.SetPixel(x, y, cv.BgColor);
					x += dx;
					y += dy;
				}
			}

			// Perform action based on surroundings
			public void Run()
			{
				cv.AssertPoint(ref x,ref y);

				// Cell instructions
				int myColor = cv.GetPixel(x, y);
				if (myColor == cv.Spore1) 
				{
					if (cv.GetPixel(x - 1, y + 1) == cv.BgColor && cv.GetPixel(x + 1, y + 1) == cv.BgColor && cv.GetPixel(x, y + 1) == cv.BgColor) Move(0, 1);
					else if (cv.GetPixel(x - 1, y) == cv.Spore2 && cv.GetPixel(x - 1, y - 1) != cv.BgColor) Move(0, -1);
					else if (cv.GetPixel(x - 1, y) == cv.Spore2 && cv.GetPixel(x - 1, y - 1) == cv.BgColor) Move(-1, -1);
					else if (cv.GetPixel(x + 1, y) == cv.Spore1 && cv.GetPixel(x + 1, y - 1) != cv.BgColor) Move(0, -1);
					else if (cv.GetPixel(x + 1, y) == cv.Spore1 && cv.GetPixel(x + 1, y - 1) == cv.BgColor) Move(1, -1);
					else Move(RandomEx.Random(3) - 1, 0);
				} 
				else if (myColor == cv.Spore2) 
				{
					if (cv.GetPixel(x - 1, y + 1) == cv.BgColor && cv.GetPixel(x + 1, y + 1) == cv.BgColor && cv.GetPixel(x, y + 1) == cv.BgColor) Move(0, 1);
					else if (cv.GetPixel(x + 1, y) == cv.Spore1 && cv.GetPixel(x + 1, y - 1) != cv.BgColor) Move(0, -1);
					else if (cv.GetPixel(x + 1, y) == cv.Spore1 && cv.GetPixel(x + 1, y - 1) == cv.BgColor) Move(1, -1);
					else if (cv.GetPixel(x - 1, y) == cv.Spore2 && cv.GetPixel(x - 1, y - 1) != cv.BgColor) Move(0, -1);
					else if (cv.GetPixel(x - 1, y) == cv.Spore2 && cv.GetPixel(x - 1, y - 1) == cv.BgColor) Move(-1, -1);
					else Move(RandomEx.Random(3) - 1, 0);
				}
				else if (myColor == cv.Spore3)
				{
					if (cv.GetPixel(x - 1, y - 1) == cv.BgColor && cv.GetPixel(x + 1, y - 1) == cv.BgColor && cv.GetPixel(x, y - 1) == cv.BgColor) Move(0, -1);
					else if (cv.GetPixel(x - 1, y) == cv.Spore4 && cv.GetPixel(x - 1, y + 1) != cv.BgColor) Move(0, 1);
					else if (cv.GetPixel(x - 1, y) == cv.Spore4 && cv.GetPixel(x - 1, y + 1) == cv.BgColor) Move(-1, 1);
					else if (cv.GetPixel(x + 1, y) == cv.Spore3 && cv.GetPixel(x + 1, y + 1) != cv.BgColor) Move(0, 1);
					else if (cv.GetPixel(x + 1, y) == cv.Spore3 && cv.GetPixel(x + 1, y + 1) == cv.BgColor) Move(1, 1);
					else Move(RandomEx.Random(3) - 1, 0);
				}
				else if (myColor == cv.Spore4)
				{
					if (cv.GetPixel(x - 1, y - 1) == cv.BgColor && cv.GetPixel(x + 1, y - 1) == cv.BgColor && cv.GetPixel(x, y - 1) == cv.BgColor) Move(0, -1);
					else if (cv.GetPixel(x + 1, y) == cv.Spore3 && cv.GetPixel(x + 1, y + 1) != cv.BgColor) Move(0, 1);
					else if (cv.GetPixel(x + 1, y) == cv.Spore3 && cv.GetPixel(x + 1, y + 1) == cv.BgColor) Move(1, 1);
					else if (cv.GetPixel(x - 1, y) == cv.Spore4 && cv.GetPixel(x - 1, y + 1) != cv.BgColor) Move(0, 1);
					else if (cv.GetPixel(x - 1, y) == cv.Spore4 && cv.GetPixel(x - 1, y + 1) == cv.BgColor) Move(-1, 1);
					else Move(RandomEx.Random(3) - 1, 0);
				}
			}
		}

		int maxcells = 4000;
		int numcells;
		Cell[] cells;
		public int Spore1, Spore2, Spore3, Spore4;
		public int BgColor;
		// set lower for smoother animation, higher for faster simulation
		int runs_per_loop = 10000;

		public CellularAutomata3()
		{
			this.TextureFilter = Sprite.TextureFilters.High;
			this.Scaling = new EPointF(2,2);

			this.Create(200,200);
			this.BgColor = Color.FromArgb(255,0,0,0).ToArgb();
			this.Setup();
		}

        public override void Dispose()
        {
            //TODO:
            base.Dispose();
        }

		public void Setup()
		{
			this.Locked = true;
			this.Clear(this.BgColor);
			cells = new Cell[maxcells];
			Spore1 = Color.FromArgb(128, 172, 255).ToArgb();
			Spore2 = Color.FromArgb(64, 128, 255).ToArgb();
			Spore3 = Color.FromArgb(255, 128, 172).ToArgb();
			Spore4 = Color.FromArgb(255, 64, 128).ToArgb();
			numcells = 0;
			Seed();
			this.Locked = false;
		}

		void Seed()
		{
			// Add cells at random places
			for (int i = 0; i < maxcells; i++)
			{
				int cX = RandomEx.Random(this.Width);
				int cY = RandomEx.Random(this.Height);

				int clr;
				float r = RandomEx.Random();
				if (r < 0.25) clr = Spore1;
				else if (r < 0.5) clr = Spore2;
				else if (r < 0.75) clr = Spore3;
				else clr = Spore4;

				if (this.GetPixelInt(cX, cY) == this.BgColor)
				{
					this.SetPixelInt(cX, cY, clr);
					cells[numcells] = new Cell(cX, cY, this);
					numcells++;
				}
			}
		}

		public override void UpdateCanvas()
		{
			// Run cells in random order
			for (int i = 0; i < runs_per_loop; i++) 
			{
				int selected = RandomEx.Random(numcells); //, numcells-1);
				cells[selected].Run();
			}
		}

		public void AssertPoint(ref int x, ref int y)
		{
			while(x < 0) x+=this.Width;
			while(x > this.Width - 1) x-=this.Width;
			while(y < 0) y+=this.Height;
			while(y > this.Height - 1) y-=this.Height;
		}
		public void SetPixel(int x, int y, int c) 
		{
			this.AssertPoint(ref x,ref y);
			base.SetPixelInt(x, y, c);
		}
  
		public new int GetPixel(int x, int y) 
		{
			this.AssertPoint(ref x,ref y);
			return base.GetPixelInt(x, y);
		}
	}
}
