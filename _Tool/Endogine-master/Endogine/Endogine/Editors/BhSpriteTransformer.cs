using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for BhSpriteChangeLoc.
	/// </summary>
	public class BhSpriteTransformer : Behavior
	{
		protected ArrayList m_aSprites;

		protected ArrayList resizeSquares;
		protected Sprite UIParentSprite;

		protected Sprite moveSprite;
		protected EPointF pntStartMove;

		protected Sprite pivotSprite;
		protected Sprite rotateSprite;
		protected EPointF pntMouseStartRotate;

        private static EPoint _gridOffset = new EPoint(0, 0);
        public static EPoint GridOffset
        {
            get { return _gridOffset; }
            set { _gridOffset = value; }
        }
        private static EPoint _gridSpacing = new EPoint(30, 30);
        public static EPoint GridSpacing
        {
            get { return _gridSpacing; }
            set { _gridSpacing = value; }
        }


		public BhSpriteTransformer()
		{
			m_aSprites = new ArrayList();
		}

		public void Init()
		{
			this.UIParentSprite = new Sprite();
			this.UIParentSprite.Parent = this.m_sp.Parent;
			this.UIParentSprite.Name = "Transformer";

			this.moveSprite = new Sprite();
			this.moveSprite.Parent = this.UIParentSprite;
			this.moveSprite.LocZ = 999;
			this.moveSprite.Name = "Move";
			this.moveSprite.MouseActive = true;
			this.moveSprite.MouseEvent+=new Endogine.Sprite.MouseEventDelegate(moveSprite_MouseEvent);
			this.moveSprite.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.m_aSprites.Add(this.moveSprite);

			this.rotateSprite = new Sprite();
			this.rotateSprite.Parent = this.UIParentSprite;
			this.rotateSprite.LocZ = 998;
			this.rotateSprite.Name = "Rotate";
			this.rotateSprite.MouseActive = true;
			this.rotateSprite.MouseEvent+=new Endogine.Sprite.MouseEventDelegate(rotateSprite_MouseEvent);
			this.rotateSprite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.m_aSprites.Add(this.rotateSprite);


			Bitmap bmpCross = new Bitmap(8,8,PixelFormat.Format24bppRgb);
			Graphics g = Graphics.FromImage(bmpCross);
			Pen pen = new Pen(System.Drawing.Color.Gray);
			g.DrawRectangle(pen, 0,0,bmpCross.Width, bmpCross.Height);
			g.DrawLine(pen, bmpCross.Width/2,0,bmpCross.Width/2,bmpCross.Height);
			g.DrawLine(pen, 0,bmpCross.Height/2,bmpCross.Width,bmpCross.Height/2);
			MemberSpriteBitmap mb = new MemberSpriteBitmap(bmpCross);
			mb.CenterRegPoint();

			this.pivotSprite = new Sprite();
			this.pivotSprite.Parent = this.UIParentSprite;
			this.pivotSprite.Name = "Pivot";
			this.pivotSprite.Member = mb;
			this.pivotSprite.LocZ = 1002;
			this.pivotSprite.MouseActive = true;
			this.pivotSprite.MouseEvent+=new Endogine.Sprite.MouseEventDelegate(pivotSprite_MouseEvent);
			this.pivotSprite.Cursor = System.Windows.Forms.Cursors.NoMove2D;
            this.m_aSprites.Add(this.pivotSprite);


			//8 squares for resizing:
			Bitmap bmpSquare = new Bitmap(8,8,PixelFormat.Format24bppRgb);
			g = Graphics.FromImage(bmpSquare);
			//g.DrawRectangle(new Pen(Color.Gray), 0,0,bmpSquare.Width, bmpSquare.Height);
            g.FillRectangle(new SolidBrush(System.Drawing.Color.Gray), 0, 0, bmpSquare.Width, bmpSquare.Height);
			mb = new MemberSpriteBitmap(bmpSquare);
			mb.CenterRegPoint();

			resizeSquares = new ArrayList();

			for (int y = 0; y < 3; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					if (x == 1 && y == 1)
						continue;
					Sprite spSquare = new Sprite();
					//spSquare.Parent = this.m_sp;
					spSquare.Parent = this.UIParentSprite;
					spSquare.Name = "Size";
					spSquare.Member = mb;
					resizeSquares.Add(spSquare);
					spSquare.Tag = new EPoint(x,y);
					spSquare.LocZ = 1000;
					spSquare.Ink = RasterOps.ROPs.BgTransparent;
					spSquare.MouseActive = true;
					spSquare.MouseEvent+=new Endogine.Sprite.MouseEventDelegate(spSquare_MouseEvent);
                    this.m_aSprites.Add(spSquare);
				}
			}
			((Sprite)resizeSquares[0]).Cursor = System.Windows.Forms.Cursors.SizeNWSE;
			((Sprite)resizeSquares[7]).Cursor = System.Windows.Forms.Cursors.SizeNWSE;
			((Sprite)resizeSquares[2]).Cursor = System.Windows.Forms.Cursors.SizeNESW;
			((Sprite)resizeSquares[5]).Cursor = System.Windows.Forms.Cursors.SizeNESW;
			((Sprite)resizeSquares[1]).Cursor = System.Windows.Forms.Cursors.SizeNS;
			((Sprite)resizeSquares[6]).Cursor = System.Windows.Forms.Cursors.SizeNS;
			((Sprite)resizeSquares[3]).Cursor = System.Windows.Forms.Cursors.SizeWE;
			((Sprite)resizeSquares[4]).Cursor = System.Windows.Forms.Cursors.SizeWE;

			this.Update();
		}

        public override void Dispose()
        {
            foreach (Sprite sp in m_aSprites)
                sp.Dispose();
            base.Dispose();
        }

		private void Update()
		{
			this.UIParentSprite.Loc = this.m_sp.Loc - this.m_sp.RegPoint.ToEPointF();
			this.moveSprite.Rect = new ERectangleF(new EPointF(), this.m_sp.Rect.Size);
			this.rotateSprite.Rect = new ERectangleF(new EPointF(-40,-40), this.m_sp.Rect.Size + new EPointF(80,80));
			this.pivotSprite.Loc = this.m_sp.RegPoint.ToEPointF(); //this.m_sp.Rect.Size/2;

			int i = 0;
			for (int y = 0; y < 3; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					if (x == 1 && y == 1)
						continue;
					Sprite sp = (Sprite)resizeSquares[i];
					sp.Loc = this.m_sp.Rect.Size*((EPoint)sp.Tag)/2;
					i++;
				}
			}
		}

		private Sprite CreateSpriteWithMemberAndAxis(MemberSpriteBitmap mb, string axis)
		{
			Sprite sp = new Sprite();
			sp.Name = axis;
			sp.Parent = this.m_sp;
			sp.Member = mb;
			sp.LocZ = 1000;
			m_aSprites.Add(sp);
			sp.MouseActive = true;
			sp.MouseEvent+=new Endogine.Sprite.MouseEventDelegate(sp_MouseEvent);
			return sp;
		}

		private void DrawLineWithCaps(Graphics g, Pen pen, EPointF start, EPointF end, float capLength)
		{
			//pen caps aren't flexible enough...
			//			pen.StartCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
			//			pen.EndCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
			EPointF pntDiff = end-start;

			g.DrawLine(pen, start.ToPointF(), end.ToPointF());
			g.DrawLine(pen, start.ToPointF(), (start+EPointF.FromLengthAndAngle(capLength, pntDiff.Angle+(float)Math.PI/4)).ToPointF());
			g.DrawLine(pen, start.ToPointF(), (start+EPointF.FromLengthAndAngle(capLength, pntDiff.Angle-(float)Math.PI/4)).ToPointF());

			g.DrawLine(pen, end.ToPointF(), (end+EPointF.FromLengthAndAngle(capLength, pntDiff.Angle+(float)Math.PI+(float)Math.PI/4)).ToPointF());
			g.DrawLine(pen, end.ToPointF(), (end+EPointF.FromLengthAndAngle(capLength, pntDiff.Angle+(float)Math.PI-(float)Math.PI/4)).ToPointF());
		}

		protected override void EnterFrame()
		{
			base.EnterFrame();

			if (m_sp.Rect == null)
				return;

			ERectangleF rct = new ERectangleF(
				m_sp.ConvParentLocToRootLoc(m_sp.Rect.Location),
				m_sp.ConvParentLocToRootLoc(m_sp.Rect.Size));
		}

		private void sp_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
		{
			if (t == Endogine.Sprite.MouseEventType.Down)
			{

			}
			else if (t == Endogine.Sprite.MouseEventType.StillDown)
			{
				EPoint pntAxis = (EPoint)sender.Tag;
				EPointF pntDiff = new EPointF(e.X, e.Y) - sender.MouseLastLoc.ToEPointF();
				this.m_sp.Move(pntDiff*pntAxis);
			}
		}

        /// <summary>
        /// Stretching sprite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="t"></param>
		private void spSquare_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
		{
			if (t == Endogine.Sprite.MouseEventType.StillDown)
			{
				EPoint pntWhichSquare = (EPoint)sender.Tag;
				EPointF pntDiff = new EPointF(e.X, e.Y) - sender.MouseLastLoc.ToEPointF();

				EPointF pntRestrictTo = new EPointF();
				if (pntWhichSquare.Y == 1)
					pntRestrictTo.X = 1;
				else if (pntWhichSquare.X == 1)
					pntRestrictTo.Y = 1;
				else
					pntRestrictTo = new EPointF(1,1);

				EPointF pntMove = pntDiff*pntRestrictTo;
				ERectangleF rct = this.m_sp.Rect.Copy();

				if (pntWhichSquare.X == 0 || pntWhichSquare.Y == 0)
				{
					if (pntWhichSquare.X == 0 && pntWhichSquare.Y == 2)
					{
						rct.X+=pntMove.X;
						rct.Width-=pntMove.X;
						rct.Height+=pntMove.Y;
					}
					else if (pntWhichSquare.X == 2 && pntWhichSquare.Y == 0)
					{
						rct.Y+=pntMove.Y;
						rct.Height-=pntMove.Y;
						rct.Width+=pntMove.X;
					}					
					else
					{
						rct.Location+=pntMove;
						rct.Size-=pntMove;
					}
				}
				else
					rct.Size+=pntMove;

				this.m_sp.Rect = rct;

				this.Update();
			}
		}

		private void moveSprite_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
		{
			if (t == Endogine.Sprite.MouseEventType.StillDown)
			{
				EPointF pntNow = new EPointF(e.X, e.Y);
                EPointF gridSpacing = new EPointF(30, 30);
                EPointF gridOffset = new EPointF(0, 0);
                if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control) //Snap to grid
                {
                    pntNow -= _gridOffset.ToEPointF();
                    pntNow /= _gridSpacing.ToEPointF();
                    pntNow = new EPointF((float)Math.Round(pntNow.X), (float)Math.Round(pntNow.Y)) * _gridSpacing.ToEPointF();
                    pntNow += _gridOffset.ToEPointF();
                }
				EPointF pntDiff = pntNow - sender.MouseDownLoc.ToEPointF();

				if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift) //Force horizontal/vertical movement
				{
					if ((pntDiff.Angle > (float)Math.PI/4 && pntDiff.Angle < 3*(float)Math.PI/4) ||
						(pntDiff.Angle < -(float)Math.PI/4 && pntDiff.Angle > -3*(float)Math.PI/4))
						pntDiff*=new EPointF(1,0);
					else
						pntDiff*=new EPointF(0,1);
				}
				this.m_sp.Loc = pntDiff + this.pntStartMove;
				this.Update();
			}
			else if (t == Endogine.Sprite.MouseEventType.Down)
				this.pntStartMove = this.m_sp.Loc.Copy();
		}

		private void rotateSprite_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
		{
			EPointF pivotLoc = this.m_sp.Loc;
			if (t == Endogine.Sprite.MouseEventType.StillDown)
			{
				EPointF pntNow = new EPointF(e.X, e.Y) - pivotLoc;
				float fAngleDiff = pntNow.Angle - this.pntMouseStartRotate.Angle;
				if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift)
				{

				}
				this.m_sp.Rotation = fAngleDiff;
			}
			else if (t == Endogine.Sprite.MouseEventType.Down)
				this.pntMouseStartRotate = new EPointF(e.X, e.Y) - pivotLoc;
		}

		private void pivotSprite_MouseEvent(Sprite sender, System.Windows.Forms.MouseEventArgs e, Endogine.Sprite.MouseEventType t)
		{
			if (t == Endogine.Sprite.MouseEventType.StillDown)
			{
				EH.Put(e.X.ToString() + " " + e.Y.ToString() + this.m_sp.Loc.ToString());
				sender.Loc+= new EPointF(e.X, e.Y) - sender.MouseLastLoc.ToEPointF();

				//TODO: setting the regpoint is not working when scaled.
				if (false)
				{
					EPoint pntLastRegPoint = this.m_sp.RegPoint.Copy();
					this.m_sp.RegPoint = (sender.Loc/this.m_sp.Scaling).ToEPoint();
					this.m_sp.Loc += (this.m_sp.RegPoint-pntLastRegPoint).ToEPointF();
					this.Update();
				}
			}
		}
	}
}
