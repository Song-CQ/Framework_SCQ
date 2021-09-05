using System;
using Endogine;

namespace Tests
{
	/// <summary>
	/// Summary description for Meter.
	/// </summary>
	public class Meter : Endogine.Forms.Frame
	{
		//private Endogine.Forms.Frame _bar;
		private int _cnt;

		public Meter()
		{
			this.Parent = EH.Instance.Stage.RootSprite;
			this.CuttingRect = new ERectangleF(0,0.4f,0,0.2f);
			this.MemberName = "MeterBar";
		}

		public float Position
		{
			get {return 0;}
			set
			{
				if (value < 0.25)
					return;
				float fHeight = this.Rect.Height*(1f-value);
				//this._bar.Rect = ERectangleF.FromLTRB(this.Rect.X,this.Rect.Y+fHeight,this.Rect.Right,this.Rect.Bottom);
				this.Rect = ERectangleF.FromLTRB(0,0,31,value*100);
			}
		}

		public override void EnterFrame()
		{
			_cnt++;
			this.Position = 1f-(float)_cnt/3000;
			base.EnterFrame ();
		}

	}
}