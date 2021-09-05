using System;
using Endogine;

using System.ComponentModel;

namespace Tests
{
	/// <summary>
	/// Summary description for SwingBehavior.
	/// </summary>
	public class BhSwing : Behavior
	{
		private int m_nCnt = 0;

		private EPointF pntLocAmount = new EPointF(0,5);
		private EPointF pntLocSpeed = new EPointF(0,0.05f);
		private float fRotationAmount = 0.1f;
		private float fRotationSpeed = 0.05f;

		private EPointF pntOffset = new EPointF();

		public BhSwing()
		{
		}

		public BhSwing(int nCntOffset)
		{
			m_nCnt = nCntOffset;
		}

		/// <summary>
		/// How much it swings on Y axis
		/// </summary>
		[
		Category("Y movement"),
		DefaultValue(0.0f),
		Description("How much it swings on Y axis")
		]
		public float LocYAmount
		{
			get {return this.pntLocAmount.Y;}
			set {this.pntLocAmount.Y = value;}
		}
		/// <summary>
		/// How fast it swings on Y axis
		/// </summary>
		[
		Category("Y movement"),
		DefaultValue(0.0f),
		Description("How fast it swings on Y axis")
		]
		public float LocYSpeed
		{
			get {return this.pntLocSpeed.Y;}
			set {this.pntLocSpeed.Y = value;}
		}

		/// <summary>
		/// How much it swings on X axis
		/// </summary>
		[
		Category("X movement"),
		DefaultValue(0.0f),
		Description("How much it swings on X axis")
		]
		public float LocXAmount
		{
			get {return this.pntLocAmount.X;}
			set {this.pntLocAmount.X = value;}
		}
		/// <summary>
		/// How fast it swings on X axis
		/// </summary>
		[
		Category("X movement"),
		DefaultValue(0.0f),
		Description("How fast it swings on X axis")
		]
		public float LocXSpeed
		{
			get {return this.pntLocSpeed.X;}
			set {this.pntLocSpeed.X = value;}
		}


		/// <summary>
		/// How much it rotates
		/// </summary>
		[
		Category("Rotation"),
		DefaultValue(0.0f),
		Description("How much it rotates")
		]
		public float RotationAmount
		{
			get {return this.fRotationAmount;}
			set {this.fRotationAmount = value;}
		}
		/// <summary>
		/// How fast it rotates
		/// </summary>
		//Read about the following in Attributes and Design-Time Support  
		[
		Category("Rotation"),
		DefaultValue(0.0f),
		//RefreshProperties(RefreshProperties.Repaint),
		Description("How fast it rotates")
		//DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		//Editor(typeof(FlashTrackBarDarkenByEditor), typeof(UITypeEditor)),
		]
		public float RotationSpeed
		{
			get {return this.fRotationSpeed;}
			set {this.fRotationSpeed = value;}
		}


		protected override void EnterFrame()
		{
			EPointF pnt = new EPointF(
				(float)Math.Sin(this.pntLocSpeed.X*m_nCnt),
				(float)Math.Sin(this.pntLocSpeed.Y*m_nCnt)) * this.pntLocAmount;
			m_sp.Loc+=pnt-this.pntOffset;
			this.pntOffset = pnt;
			
			m_sp.Rotation = (float)Math.Sin(fRotationSpeed*m_nCnt)*fRotationAmount;
			m_nCnt++;

			base.EnterFrame();
		}
	}
}
