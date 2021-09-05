using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Endogine
{

	/// <summary>
	/// Summary description for ValueEdit.
	/// </summary>
	public class ValueEdit : System.Windows.Forms.UserControl
	{
		public delegate void ValueChangedDelegate(object sender, EPointF pnt);
		public event ValueChangedDelegate ValueChanged = null;

		private Endogine.JogShuttle jsJog;
		private Endogine.JogShuttle jsShuttle;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbCoordinate;
		private System.Windows.Forms.Button btnGoCoordinate;
		private Endogine.StepXY stepXY1;
		private System.Windows.Forms.Label lblCurrent;
		private System.Windows.Forms.Button btnDefault;
		private System.Windows.Forms.Button btnRestore;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private EPointF m_pntDefault;
		private EPointF m_pntStart;
		private EPointF m_pntCurrent;

		public ValueEdit()
		{
			InitializeComponent();

			m_pntDefault = new EPointF();
			m_pntStart = new EPointF();
			m_pntCurrent = new EPointF();

			jsShuttle.StartTimer();
			jsShuttle.Factor = new EPointF(0.1f,0.1f);
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
			this.jsJog = new Endogine.JogShuttle();
			this.jsShuttle = new Endogine.JogShuttle();
			this.btnDefault = new System.Windows.Forms.Button();
			this.btnRestore = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tbCoordinate = new System.Windows.Forms.TextBox();
			this.btnGoCoordinate = new System.Windows.Forms.Button();
			this.stepXY1 = new Endogine.StepXY();
			this.lblCurrent = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// jsJog
			// 
			this.jsJog.Location = new System.Drawing.Point(0, 32);
			this.jsJog.Name = "jsJog";
			this.jsJog.Size = new System.Drawing.Size(40, 40);
			this.jsJog.TabIndex = 0;
			this.jsJog.TextInside = "Jog ";
			this.jsJog.DraggingEvent += new Endogine.DragDelegate(this.jsJog_DraggingEvent);
			// 
			// jsShuttle
			// 
			this.jsShuttle.Location = new System.Drawing.Point(48, 32);
			this.jsShuttle.Name = "jsShuttle";
			this.jsShuttle.Size = new System.Drawing.Size(40, 40);
			this.jsShuttle.TabIndex = 1;
			this.jsShuttle.TextInside = "Shuttle";
			this.jsShuttle.DraggingEvent += new Endogine.DragDelegate(this.jsShuttle_DraggingEvent);
			// 
			// btnDefault
			// 
			this.btnDefault.Location = new System.Drawing.Point(0, 0);
			this.btnDefault.Name = "btnDefault";
			this.btnDefault.Size = new System.Drawing.Size(32, 23);
			this.btnDefault.TabIndex = 2;
			this.btnDefault.Text = "0;0";
			this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
			// 
			// btnRestore
			// 
			this.btnRestore.Location = new System.Drawing.Point(32, 0);
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.Size = new System.Drawing.Size(56, 23);
			this.btnRestore.TabIndex = 3;
			this.btnRestore.Text = "Restore";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(96, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "Key move:";
			// 
			// tbCoordinate
			// 
			this.tbCoordinate.Location = new System.Drawing.Point(96, 48);
			this.tbCoordinate.Name = "tbCoordinate";
			this.tbCoordinate.Size = new System.Drawing.Size(56, 20);
			this.tbCoordinate.TabIndex = 5;
			this.tbCoordinate.Text = "0;0";
			this.tbCoordinate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// btnGoCoordinate
			// 
			this.btnGoCoordinate.Location = new System.Drawing.Point(152, 48);
			this.btnGoCoordinate.Name = "btnGoCoordinate";
			this.btnGoCoordinate.Size = new System.Drawing.Size(32, 23);
			this.btnGoCoordinate.TabIndex = 6;
			this.btnGoCoordinate.Text = "Go";
			this.btnGoCoordinate.Click += new System.EventHandler(this.btnGoCoordinate_Click);
			// 
			// stepXY1
			// 
			this.stepXY1.Location = new System.Drawing.Point(152, 24);
			this.stepXY1.Name = "stepXY1";
			this.stepXY1.Size = new System.Drawing.Size(24, 24);
			this.stepXY1.TabIndex = 7;
			this.stepXY1.KeyMoveEvent += new Endogine.KeyMoveDelegate(this.stepXY1_KeyMoveEvent);
			// 
			// lblCurrent
			// 
			this.lblCurrent.Location = new System.Drawing.Point(96, 0);
			this.lblCurrent.Name = "lblCurrent";
			this.lblCurrent.Size = new System.Drawing.Size(88, 16);
			this.lblCurrent.TabIndex = 8;
			this.lblCurrent.Text = "0;0";
			this.lblCurrent.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ValueEdit
			// 
			this.Controls.Add(this.lblCurrent);
			this.Controls.Add(this.stepXY1);
			this.Controls.Add(this.tbCoordinate);
			this.Controls.Add(this.btnGoCoordinate);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnRestore);
			this.Controls.Add(this.btnDefault);
			this.Controls.Add(this.jsShuttle);
			this.Controls.Add(this.jsJog);
			this.Name = "ValueEdit";
			this.Size = new System.Drawing.Size(184, 72);
			this.ResumeLayout(false);

		}
		#endregion

		private void jsShuttle_DraggingEvent(object sender, Endogine.EPointF pntDelta)
		{
			EPointF pnt = jsShuttle.GetDelta();
			Send(m_pntCurrent+pnt);
		}

		private void jsJog_DraggingEvent(object sender, Endogine.EPointF pntDelta)
		{
			Send(m_pntCurrent+pntDelta);
		}

		private void stepXY1_KeyMoveEvent(object sender, Endogine.EPointF pntMove)
		{
			Send(m_pntCurrent+pntMove);
		}

		private void btnDefault_Click(object sender, System.EventArgs e)
		{
			Send(m_pntDefault.Copy());
		}

		private void btnGoCoordinate_Click(object sender, System.EventArgs e)
		{
			MoveToCoordinate();
		}

		private void MoveToCoordinate()
		{
			string s = tbCoordinate.Text;
			s = s.Trim();
			s = System.Text.RegularExpressions.Regex.Replace(s, @"[^0-9^;^.^-]", "");
			string[] ss = s.Split(";".ToCharArray(), 2);

			//System.Globalization.NumberFormatInfo fi = new System.Globalization.NumberFormatInfo();
			//fi.CurrencyDecimalSeparator = ".";
			EPointF pnt = new EPointF();
			if (ss.GetLength(0) > 0)
				pnt.X = Convert.ToSingle(ss[0]);
			if (ss.GetLength(0) > 1)
				pnt.Y = Convert.ToSingle(ss[1]);

			tbCoordinate.Text = pnt.X.ToString() + ";" + pnt.Y.ToString();

			Send(pnt);
		}

		public void InitWithValue(EPointF pnt)
		{
			DefaultValue = pnt;
			StartValue = pnt;
			CurrentValue = pnt;
		}

		//[DefaultValue(new EPointF())]
		public EPointF DefaultValue
		{
			get {return m_pntDefault;}
			set
			{
				if (value!=null)
				{
					m_pntDefault = value;
					tbCoordinate.Text = value.X.ToString() + ";" + value.Y.ToString();
					this.btnDefault.Text = tbCoordinate.Text;
				}
			}
		}
		public EPointF StartValue
		{
			get {return m_pntStart;}
			set {m_pntStart = value;}
		}
		public EPointF CurrentValue
		{
			get {return m_pntCurrent;}
			set {m_pntCurrent = value;}
		}

		public JogShuttle Jog
		{
			get {return this.jsJog;}
		}
		public JogShuttle Shuttle
		{
			get {return this.jsShuttle;}
		}
		
		private void Send(EPointF pnt)
		{
			m_pntCurrent = pnt;

			lblCurrent.Text = pnt.X.ToString("#0.0") + ";  " + pnt.Y.ToString("#0.0");

			if (ValueChanged!=null)
				ValueChanged(this, pnt);
		}
	}
}
