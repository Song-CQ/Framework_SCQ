using System;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for MeterBar.
	/// </summary>
	public class MeterBar : MeterBase
	{
		protected Sprite m_spBar;
		protected bool _isVertical;

		public MeterBar()
		{
			this.m_bMeInvisibleButNotChildren = true;
			this.Name = "MeterBar";
			this.SourceRect = new ERectangle(0,0,1,1);
			m_spBar = new Sprite();
			m_spBar.Parent = this;

			MemberSpriteBitmap mb = new MemberSpriteBitmap(
				Endogine.BitmapHelpers.BitmapHelper.CreateFilledBitmap(new EPoint(1,1), System.Drawing.Color.FromArgb(255,255,255)));
			m_spBar.Member = mb;

			Value = 0;
		}

		public bool Vertical
		{
			get {return this._isVertical;}
			set {this._isVertical = value;}
		}

		public override float Value
		{
			get { return base.Value; }
			set
			{
				base.Value = value;
                float fFract = this.GetFraction();
                if (fFract > 0)
                    m_spBar.Color = System.Drawing.Color.Green;
                else
                {
                    fFract = -fFract;
                    m_spBar.Color = System.Drawing.Color.Red;
                }
				if (this._isVertical)
					m_spBar.Rect = new ERectangleF(0, 1f-fFract, 1, fFract);
				else
                    m_spBar.Rect = new ERectangleF(m_spBar.Rect.X, m_spBar.Rect.Y, fFract, m_spBar.Rect.Height);
                //if (fFract != 0)
                //    Console.WriteLine("" + fFract * m_spBar.Rect.Width + " " + m_spBar.Rect);
            }
		}
	}
}