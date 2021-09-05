using System;
using System.Collections.Generic;
using Endogine;
//using Endogine.Audio;

namespace Tests.SpaceInvaders
{
	/// <summary>
	/// Summary description for InvadersGrid.
	/// </summary>
	public class InvadersGrid : Sprite
	{
		public List<Invader> _invaders;
		private SortedList<int, SortedList<int, Invader>> _columnSortedInvaders;

		private EPointF m_pntMovement;
		private int m_nNumWaitFrames;

//		private Sound m_sndStep1;
//		private Sound m_sndStep2;

		public InvadersGrid()
		{
			this.SourceRect = new ERectangle(0,0,1,1);

            this._invaders = new List<Invader>();

			int nColumns = 11;
            int nRows = 5;

            //nColumns = 0;
            //nRows = 0;

            this._columnSortedInvaders = new SortedList<int, SortedList<int, Invader>>();
			for (int i = 0; i < nColumns; i++)
                this._columnSortedInvaders.Add(i, new SortedList<int, Invader>());

            for (int y = 0; y < nRows; y++)
			{
                string anim = "Invader03";
				if (y >= 3)
                    anim = "Invader01";
				else if (y>=1)
                    anim = "Invader02";

				for (int x = 0; x<nColumns; x++)
				{
					Invader invader = new Invader();
                    invader.SetGraphics(anim);
                    invader.Loc = new EPointF((x-1)*32,(y-1)*32) + new EPointF(185,124);
					invader.Parent = this;

                    this._invaders.Add(invader);
					this._columnSortedInvaders[x].Add(y, invader);
				}
			}

			this.m_pntMovement = new EPointF(4,0);
		}

		public override void Dispose()
		{
            for (int i = this._invaders.Count - 1; i >= 0; i--)
                this._invaders[i].Dispose();
            this._invaders = null;

            this._columnSortedInvaders = null;

			base.Dispose ();
		}

		public void Step()
		{
            if (this._invaders.Count == 0)
                return;

			//change direction?
			bool bChange = false;
			if (this.m_pntMovement.X < 0)
			{
				Invader inv = this._columnSortedInvaders.Values[0].Values[0];
				if (inv.Loc.X < 135)
					bChange = true;
			}
			else
			{
                Invader inv = this._columnSortedInvaders.Values[this._columnSortedInvaders.Count-1].Values[0];
				if (inv.Loc.X > 520)
					bChange = true;
			}
			float fXTmp = this.m_pntMovement.X;
			if (bChange)
			{
				fXTmp=-fXTmp;
				this.m_pntMovement.X = 0;
				this.m_pntMovement.Y = 32;
			}

			EPointF pntMove = new EPointF(-5,0);
			for (int n = this._invaders.Count-1; n >= 0; n--)
				this._invaders[n].Move(this.m_pntMovement);
			this.m_pntMovement.X = fXTmp;
			this.m_pntMovement.Y = 0;

			this.m_nNumWaitFrames = (int)(0.8*this._invaders.Count);
			//this.m_nNumWaitFrames = 10;
		}

		public bool CheckCollision(Sprite sp)
		{
            for (int i = this._invaders.Count - 1; i >= 0; i--)
			{
				Invader inv = this._invaders[i];
				if (inv.GetCollisionPoint(sp) != null)
				{
					inv.Explode();
					return true;
				}
			}
			return false;
		}


		public override void RemoveChild(Sprite a_sp)
		{
            Invader inv = (Invader)a_sp;
            if (this._invaders.Contains(inv))
			{
                this._invaders.Remove(inv);

				//also remove from m_columnSortedInvaders:
				for (int n = this._columnSortedInvaders.Count-1; n >= 0; n--)
				{
					SortedList<int, Invader> slCol = this._columnSortedInvaders.Values[n];
                    int nIndex = slCol.IndexOfValue(inv);
					if (nIndex>=0)
					{
						slCol.RemoveAt(nIndex);
						if (slCol.Count == 0)
							this._columnSortedInvaders.RemoveAt(n);
						break;
					}
				}
			}
			base.RemoveChild (a_sp);
		}

		public override void EnterFrame()
		{
			//Anyone to fire?
            if (this._invaders.Count == 0)
                return;

			Random rnd = new Random();
			if (rnd.Next(30) == 0) //yup, fire a shot!
			{
				Invader inv = null;
				if (rnd.Next(4) == 0) //sometimes, let that someone be close to player to make it harder:
				{
					Invader invClosest = null;
					float fXClosest = 9999;

					for (int nColIndex = 0; nColIndex < this._columnSortedInvaders.Count; nColIndex++)
					{
						SortedList<int, Invader> slCol = this._columnSortedInvaders.Values[nColIndex];
						inv = slCol.Values[slCol.Count-1];
						
						float fXDiff = Math.Abs(inv.LocX - GameMain.Instance.m_player.LocX);

						if (fXDiff < fXClosest)
						{
							invClosest = inv;
							fXClosest = fXDiff;
						}
					}
					inv = invClosest;
				}
				else
				{
					SortedList<int, Invader> slCol = this._columnSortedInvaders.Values[rnd.Next(this._columnSortedInvaders.Count)];
					inv = slCol.Values[slCol.Count-1];
				}
				inv.Fire();
			}


			if (m_nNumWaitFrames-- > 0)
				return;

			this.Step();
		}
	}
}
