using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Endogine
{
	public delegate void DragDelegate(object sender, EPointF pntDelta);

	/// <summary>
	/// Summary description for JogShuttle.
	/// </summary>
	public class JogShuttle : System.Windows.Forms.UserControl
	{
		public event DragDelegate DraggingEvent;

		private bool m_bMouseMoveDown = false;
		private EPoint m_pntMouseLastLoc;
		private EPoint m_pntMouseDownLoc;

		private EPoint m_pntWrappedAdd;
		private EPointF m_pntFactor;

		private EPoint m_pntActiveAxes;

		private System.Windows.Forms.Label label1;
		private System.Timers.Timer timer1;
		private System.Windows.Forms.Panel panel1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public JogShuttle()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			m_pntFactor = new EPointF(1,1);
			m_pntWrappedAdd = new EPoint();
			m_pntActiveAxes = new EPoint(1,1);
			FixLayout();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.timer1 = new System.Timers.Timer();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(1, 1);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label1_MouseUp);
			this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label1_MouseMove);
			this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
			// 
			// timer1
			// 
			this.timer1.Interval = 20;
			this.timer1.SynchronizingObject = this;
			this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(32, 24);
			this.panel1.TabIndex = 1;
			// 
			// JogShuttle
			// 
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel1);
			this.Name = "JogShuttle";
			this.Size = new System.Drawing.Size(40, 40);
			this.Resize += new System.EventHandler(this.JogShuttle_Resize);
			((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void label1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_bMouseMoveDown = true;
			m_pntMouseLastLoc = new EPoint(e.X, e.Y);
			m_pntMouseDownLoc = new EPoint(e.X, e.Y);
		}

		private void label1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (m_pntMouseDownLoc==null)
				return;
			m_bMouseMoveDown = false;
			System.Windows.Forms.Cursor.Position = label1.PointToScreen(m_pntMouseDownLoc.ToPoint());
		}

		private void label1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!m_bMouseMoveDown)
			{
//				string sText = this.label1.Text.Split(" ".ToCharArray())[0];
				EPoint pntMid = new EPoint(this.label1.Left, this.label1.Top) + new EPoint(this.label1.Width, this.label1.Height)/2;
				EPoint pntMouse = new EPoint(e.X, e.Y); //this.label1.PointToClient(new Point(e.X, e.Y))
				EPointF pntDiff = (pntMouse-pntMid).ToEPointF();
				if (pntDiff.Length < 0.8f*this.label1.Width/2)
					m_pntActiveAxes = new EPoint(1,1);
				else
				{
					if ((pntDiff.Angle > Math.PI/4 && pntDiff.Angle < 3*Math.PI/4) ||
						(pntDiff.Angle < -Math.PI/4 && pntDiff.Angle > -3*Math.PI/4))
						m_pntActiveAxes = new EPoint(1,0);
					else
						m_pntActiveAxes = new EPoint(0,1);
				}


				if (m_pntActiveAxes.X == 1 && m_pntActiveAxes.Y == 1)
				{
//					sText+=" XY";
					this.Cursor = System.Windows.Forms.Cursors.SizeAll;
				}
				else if (m_pntActiveAxes.X == 1)
				{
//					sText+=" X";
					this.Cursor = System.Windows.Forms.Cursors.SizeWE;
				}
				else if (m_pntActiveAxes.Y == 1)
				{
//					sText+=" Y";
					this.Cursor = System.Windows.Forms.Cursors.SizeNS;
				}

//				this.label1.Text = sText;
			}
			else
			{
				EPoint pntNow = new EPoint(e.X, e.Y);
				EPoint pntDiff = pntNow-m_pntMouseLastLoc;
				//CameraMove(pntDiff.ToEPointF());

				//If mouse moves outside screen, wrap it around the edges:
			
				EPoint pntScreen = new EPoint(label1.PointToScreen(m_pntMouseLastLoc.ToPoint()));
				EPoint pntOrg = pntScreen.Copy();

				ERectangle rct = new ERectangle(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea);
				rct = rct + new ERectangle(5,5,-10,-10);
				rct.WrapPointInside(pntScreen);

				if (!pntOrg.Equals(pntScreen))
				{
					System.Windows.Forms.Cursor.Position = pntScreen.ToPoint();
					pntNow = new EPoint(label1.PointToClient(pntScreen.ToPoint()));
					m_pntWrappedAdd+=pntOrg-pntScreen;
				}

				//when using timer - don't send on mouse move
				if (timer1.Enabled == false)
					if (DraggingEvent!=null) DraggingEvent(this, pntDiff.ToEPointF()*m_pntFactor*m_pntActiveAxes);

				m_pntMouseLastLoc = pntNow;
			}
		}

		public EPointF Factor
		{
			get {return m_pntFactor;}
			set {m_pntFactor = value;}
		}

		public EPointF GetDelta()
		{
			if (!m_bMouseMoveDown)
				return new EPointF();
			return ((m_pntMouseLastLoc+m_pntWrappedAdd)-m_pntMouseDownLoc).ToEPointF()*m_pntFactor*m_pntActiveAxes;
		}

		private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!m_bMouseMoveDown)
				return;
			if (DraggingEvent!=null) DraggingEvent(this, ((m_pntMouseLastLoc+m_pntWrappedAdd)-m_pntMouseDownLoc).ToEPointF()*m_pntFactor*m_pntActiveAxes);
		}

		public void StartTimer()
		{
			timer1.Enabled = true;
			timer1.Start();
		}
		public void StopTimer()
		{
			timer1.Enabled = false;
			timer1.Stop();
		}

		private void JogShuttle_Resize(object sender, System.EventArgs e)
		{
			FixLayout();
		}

		private void FixLayout()
		{
			panel1.Width = this.Width;
			panel1.Height = this.Height;

			label1.Width = this.Width-2;
			label1.Height = this.Height-2;
		}

		/// <summary>
		/// The text shown inside the control
		/// </summary>
		[Bindable(true), Category("Appearance"), 
		DefaultValue("JogShu"),
		Description("Text shown inside the control.")]
		public string TextInside
		{
			get {return label1.Text;}
			set {label1.Text = value;}
		}
	}
}
