using System;
using Endogine;

namespace MusicGame.Midi.LocSetter
{
	/// <summary>
	/// Summary description for LocSetterBase.
	/// </summary>
	public class Base : Sprite
	{
		protected Interactor.Base _interactor;

		public Base() : base(false)
		{
			this.Name = "LocSetter";
		}

		public Interactor.Base Interactor
		{
			get {return this._interactor;}
			set {this.Parent = value;}
		}

		public override Sprite Parent
		{
			get { return base.Parent; }
			set
			{
				this._interactor = (Interactor.Base)value;
				base.Parent = value;
			}
		}

		public override void EnterFrame()
		{
			base.EnterFrame ();
		}

		public virtual void Start()
		{
		}
	}
}
