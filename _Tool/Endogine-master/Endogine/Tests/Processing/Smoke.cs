using System;
using System.Drawing;
using Endogine;

namespace Tests.Processing
{
	//adapted from Processing example
	//Smoke
	//by Glen Murphy <http://www.bodytag.org>

	/// <summary>
	/// Summary description for Smoke.
	/// </summary>
	public class Smoke : Canvas
	{
		public int res = 1;
		public int penSize = 30;
		public int lwidth;
		public int lheight;
		int pnum = 60000;
		public vsquare[][] v;
		public vbuffer[][] vbuf;
		public particle[] p;
		public Color CurrentColor;

		int mouseXvel = 0;
		int mouseYvel = 0;

		public Gust RandomGust;

		public Smoke()
		{
			this.MouseActive = true;
			this.TextureFilter = Sprite.TextureFilters.High;
			this.Scaling = new EPointF(5,5);
			this.Ink = RasterOps.ROPs.AddPin;

			this.Create(100,100);

			lwidth = this.Width/res;
			lheight = this.Height/res;
			
			v = new vsquare[lwidth+1][];
			for (int i=0; i<v.Length;i++)
				v[i] = new vsquare[lheight+1];
			
			vbuf = new vbuffer[lwidth+1][];
			for (int i=0; i<vbuf.Length;i++)
				vbuf[i] = new vbuffer[lheight+1];

			p = new particle[pnum];

			CurrentColor = Color.FromArgb(2,255,0,0);

			ERectangle emitter = ERectangle.FromLTRB(this.Width/2-20,this.Height-20,this.Width/2+20,this.Height);
			for(int i = 0; i < pnum; i++) 
			{
				p[i] = new particle(emitter, this);
				p[i].Init();
			}
			for(int i = 0; i <= lwidth; i++) 
			{
				for(int u = 0; u <= lheight; u++) 
				{
					v[i][u] = new vsquare(i*res,u*res, this);
					vbuf[i][u] = new vbuffer(i*res,u*res, this);
				}
			}

			RandomGust = new Gust();
		}

        public override void Dispose()
        {
            //TODO:
            base.Dispose();
        }

		public override void UpdateCanvas()
		{
			Endogine.ColorEx.ColorHsb clr = new Endogine.ColorEx.ColorHsb(CurrentColor);
			clr.H+=1f;
			if (clr.H > 360)
				clr.H-=360;
			CurrentColor = clr.ColorRGBA;

            this.Clear(Color.Black);
			int axvel = this.MouseLoc.X-this.MouseLastLoc.X; 
			int ayvel = this.MouseLoc.Y-this.MouseLastLoc.Y;

			mouseXvel = (axvel != mouseXvel) ? axvel : 0;
			mouseYvel = (ayvel != mouseYvel) ? ayvel : 0;

			if(RandomGust.Alive) 
				RandomGust.Update();
			else
			{
				if(RandomEx.Random(0,10)==0) 
				{
					RandomGust.SetLife(RandomEx.Random(1,20));
					RandomGust.Loc = new EPointF(RandomEx.Random(0f,(float)this.Width),
						RandomEx.Random(0f,this.Height-10)); //TODO above emitter, not fixed value!

					float fact = 0.3f*this.Width/200;
					EPointF vel = new EPointF(RandomEx.Random(0f,8f)*fact, RandomEx.Random(-2f,1f)*fact);
					if(RandomGust.Loc.X > this.Width/2) 
						vel.X*=-1;
					RandomGust.Vel = vel;

					RandomGust.Size = RandomEx.Random(0f,50f)*this.Width/200;
				}
				//randomGust--;
			}

			for(int i = 0; i < lwidth; i++) 
			{
				for(int u = 0; u < lheight; u++) 
				{
					vbuf[i][u].updatebuf(i,u);
					v[i][u].Refresh();
				}
			}
			for(int i = 0; i < pnum-1; i++) 
			{
				p[i].updatepos();
			}
			for(int i = 0; i < lwidth; i++) 
			{
				for(int u = 0; u < lheight; u++) 
				{
					v[i][u].addbuffer(i, u);
					v[i][u].updatevels(mouseXvel, mouseYvel);
					v[i][u].display(i, u);
				}
			}
			//randomGust = 0;
		}

		public class Gust
		{
			public EPointF Loc;
			//EPointF Vel;
			EPointF _vel;
			EPointF _orgVel;
			public float Size;
			int _cnt;
			int _life;

			public Gust()
			{
			}

			public void SetLife(int time)
			{
				_life = time;
				_cnt = 0;
			}
			public bool Alive
			{
				get {return this._cnt < this._life;}
			}

			public EPointF Vel
			{
				get {return this._vel;}
				set {this._vel = value; this._orgVel = value.Copy();}
			}

			public void Update()
			{
				_cnt++;
				if (!this.Alive)
					return;
				float speed = (float)Math.Sin((float)_cnt/_life);
				if (speed == 0)
					speed = 0.0001f;
				this._vel.Length=speed*this._orgVel.Length;
			}
		}

		public class particle 
		{
			public float x;
			public float y;
			public float xvel;
			public float yvel;
			public float temp;
			public int pos;

			public Color color;
			Smoke smoke;
			ERectangle emitter;

			public particle(ERectangle emitterIn, Smoke smokeIn) 
			{
				emitter = emitterIn;
				smoke = smokeIn;
			}

			public void Init() 
			{
				//x = smoke.Width/2+RandomEx.Random(-20f,20f);
				//y = RandomEx.Random((float)smoke.Height-10f,(float)smoke.Height);
				x = RandomEx.Random((float)emitter.Left, (float)emitter.Right);
				y = RandomEx.Random((float)emitter.Top, (float)emitter.Bottom);
				
				color = smoke.CurrentColor; //new Endogine.ColorEx.ColorHsb(2, 0,1,1).ToColor();

				xvel = RandomEx.Random(-1f,1f);
				yvel = RandomEx.Random(-1f,1f);
			}

			public void updatepos() 
			{
				float res = smoke.res;
				int vi = (int)(x/smoke.res);
				int vu = (int)(y/smoke.res);

				if(vi > 0 && vi < smoke.lwidth && vu > 0 && vu < smoke.lheight) 
				{
					smoke.v[vi][vu].addcolour(this.color);

					float ax = (x%smoke.res)/smoke.res;
					float ay = (y%smoke.res)/smoke.res;

					xvel += (1-ax)*smoke.v[vi][vu].xvel*0.05f;
					yvel += (1-ay)*smoke.v[vi][vu].yvel*0.05f;

					xvel += ax*smoke.v[vi+1][vu].xvel*0.05f;
					yvel += ax*smoke.v[vi+1][vu].yvel*0.05f;

					xvel += ay*smoke.v[vi][vu+1].xvel*0.05f;
					yvel += ay*smoke.v[vi][vu+1].yvel*0.05f;

					smoke.v[vi][vu].yvel -= (1-ay)*0.003f;
					smoke.v[vi+1][vu].yvel -= ax*0.003f;

					if(smoke.v[vi][vu].yvel < 0) smoke.v[vi][vu].yvel *= 1.00025f;

					x += xvel;
					y += yvel;
				} 
				else
					this.Init();

				if(RandomEx.Random(0,400) < 1) 
					this.Init();
				
				xvel *= 0.6f;
				yvel *= 0.6f;
			}
		}

		public class vbuffer
		{
			public int x;
			public int y;
			public float xvel;
			public float yvel;
			public float pressurex = 0;
			public float pressurey = 0;
			public float pressure = 0;
			Smoke smoke;
	
			public vbuffer(int xIn,int yIn, Smoke smokeIn)
			{
				x = xIn;
				y = yIn;
				pressurex = 0;
				pressurey = 0;
				smoke = smokeIn;
			}

			public void updatebuf(int i, int u) 
			{
				if(i>0 && i<smoke.lwidth && u>0 && u<smoke.lheight) 
				{
					pressurex = (smoke.v[i-1][u-1].xvel*0.5f + smoke.v[i-1][u].xvel + smoke.v[i-1][u+1].xvel*0.5f - smoke.v[i+1][u-1].xvel*0.5f - smoke.v[i+1][u].xvel - smoke.v[i+1][u+1].xvel*0.5f);
					pressurey = (smoke.v[i-1][u-1].yvel*0.5f + smoke.v[i][u-1].yvel + smoke.v[i+1][u-1].yvel*0.5f - smoke.v[i-1][u+1].yvel*0.5f - smoke.v[i][u+1].yvel - smoke.v[i+1][u+1].yvel*0.5f);
					pressure = (pressurex + pressurey)*0.25f;
				}
			}
		}
	
		public class vsquare 
		{
			public int x;
			public int y;
			public float xvel;
			public float yvel;
			float r,g,b,a;
			public Color col;
			int numColsAdded;
			Smoke smoke;
	
			public vsquare(int xIn,int yIn, Smoke smokeIn) 
			{
				x = xIn;
				y = yIn;
				smoke = smokeIn;
				this.Refresh();
			}

			public void Refresh()
			{
				col = Color.FromArgb(0,0,0,0);
				r = g = b = a = 0;
				numColsAdded = 0;
			}

			public void addbuffer(int i, int u) 
			{
				if(i>0 && i<smoke.lwidth && u>0 && u<smoke.lheight) 
				{
					xvel += (smoke.vbuf[i-1][u-1].pressure*0.5f
						+smoke.vbuf[i-1][u].pressure
						+smoke.vbuf[i-1][u+1].pressure*0.5f
						-smoke.vbuf[i+1][u-1].pressure*0.5f
						-smoke.vbuf[i+1][u].pressure
						-smoke.vbuf[i+1][u+1].pressure*0.5f
						)*0.49f;
					yvel += (smoke.vbuf[i-1][u-1].pressure*0.5f
						+smoke.vbuf[i][u-1].pressure
						+smoke.vbuf[i+1][u-1].pressure*0.5f
						-smoke.vbuf[i-1][u+1].pressure*0.5f
						-smoke.vbuf[i][u+1].pressure
						-smoke.vbuf[i+1][u+1].pressure*0.5f
						)*0.49f;
				}
			}

			public void updatevels(int mvelX, int mvelY) 
			{
                //float adj;
                //float opp;
				float dist;
				float mod;

                bool bMousePressed = false; //TODO:
                if (bMousePressed)
				{
					EPointF diff = new EPointF(x,y)-smoke.MouseLoc.ToEPointF();
					dist = (float)diff.Length;
					if(dist < smoke.penSize) 
					{
						if(dist < 4) dist = smoke.penSize;
						mod = smoke.penSize/dist;
						xvel += mvelX*mod;
						yvel += mvelY*mod;
					}
				}
				if(smoke.RandomGust.Alive) 
				{
					EPointF diff = new EPointF(x,y)-smoke.RandomGust.Loc;
					dist = diff.Length;
					if(dist < smoke.RandomGust.Size) 
					{
						if(dist < smoke.res*2)
							dist = smoke.RandomGust.Size;
						mod = smoke.RandomGust.Size/dist;

						xvel += smoke.RandomGust.Vel.X*mod; //(smoke.randomGustMax-smoke.randomGust)*smoke.randomGustXvel*mod;
						yvel += smoke.RandomGust.Vel.Y*mod; //(smoke.randomGustMax-smoke.randomGust)*smoke.randomGustYvel*mod;
					}
				}
				xvel *= 0.99f;
				yvel *= 0.98f;
			}

			public void addcolour(Color c) 
			{
				numColsAdded++;
				r+=(float)c.R*c.A/255;
				g+=(float)c.G*c.A/255;
				b+=(float)c.B*c.A/255;
				a+=c.A;
				//col = Endogine.ColorEx.ColorFunctions.InterpolateBetweenRGB(c, col, 0.5f);
				//col += amt;
				//if(col > 196) col = 196;
			}

			public void display(int i, int u) 
			{
//				float tcol = 0;
//				if(i>0 && i<smoke.lwidth-1 && u>0 && u<smoke.lheight-1) 
//				{
//
//					tcol = (+ smoke.v[i][u+1].col
//						+ smoke.v[i+1][u].col
//						+ smoke.v[i+1][u+1].col*0.5f
//						)*0.3f;
//					tcol = (int)(tcol+col*0.5f);
//				}

				r = r>255?255:r;
				g = g>255?255:g;
				b = b>255?255:b;
				Color clr = Color.FromArgb((int)r, (int)g, (int)b);

//				int gray = (int)tcol; //255-(int)tcol;
//				Color clr = Color.FromArgb(gray,gray,gray);

				smoke.FillRectangle(x,y,smoke.res,smoke.res, clr.ToArgb());
			}
		}
	}
}
