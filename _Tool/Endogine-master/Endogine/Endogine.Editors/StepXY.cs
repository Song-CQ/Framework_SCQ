using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Endogine
{
	public delegate void KeyMoveDelegate(object sender, EPointF pntMove);

	/// <summary>
	/// Summary description for StepXY.
	/// </summary>
	public class StepXY : System.Windows.Forms.UserControl
	{

		private System.Windows.Forms.TextBox textBox1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Timers.Timer timer1;
		private DateTime dtKeyDown;
		private bool bInitialWaitAfterDown;

		private KeysSteering keysSteering;
		public event KeyMoveDelegate KeyMoveEvent = null;


		public StepXY()
		{
			InitializeComponent();

            keysSteering = new KeysSteering();
            keysSteering.AddActionAndKey("left", Keys.Left);
            keysSteering.AddActionAndKey("right", Keys.Right);
            keysSteering.AddActionAndKey("up", Keys.Up);
            keysSteering.AddActionAndKey("down", Keys.Down);
            keysSteering.ReceiveFormsControlKeys(textBox1);
            keysSteering.NoReceiveEndogineKeys();
			keysSteering.KeyEvent+=new KeyEventHandler(keysSteering_KeyEvent);
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.timer1 = new System.Timers.Timer();
			((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(16, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 20;
			this.timer1.SynchronizingObject = this;
			this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
			// 
			// StepXY
			// 
			this.Controls.Add(this.textBox1);
			this.Name = "StepXY";
			this.Size = new System.Drawing.Size(24, 24);
			((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			textBox1.Text = "";
		}

		private void keysSteering_KeyEvent(KeyEventArgs e, bool bDown)
		{
			if (bDown)
			{
				if (!bInitialWaitAfterDown) //don't wait again when second key is pressed
				{
					dtKeyDown = DateTime.Now;	
					bInitialWaitAfterDown = true;
					CheckKeys();
				}
			}
			else
			{
				if (keysSteering.KeysPressed.Count == 0)
					bInitialWaitAfterDown = false;
			}
		}

		private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (bInitialWaitAfterDown)
			{
				TimeSpan ts = e.SignalTime.Subtract(dtKeyDown);
				if (ts.TotalMilliseconds < 200)
					return;
				bInitialWaitAfterDown = false;
			}
			CheckKeys();
		}

		private void CheckKeys()
		{
            if (this.keysSteering == null)
                return;

			EPoint pntMove = new EPoint();
			if (keysSteering.GetKeyActive("up"))
				pntMove.Y =-1;
			else if (keysSteering.GetKeyActive("down"))
				pntMove.Y = 1;

			if (keysSteering.GetKeyActive("left"))
				pntMove.X =-1;
			else if (keysSteering.GetKeyActive("right"))
				pntMove.X = 1;

			if (pntMove.X != 0 || pntMove.Y != 0)
			{
				if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
					pntMove*=10;
				if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
					pntMove*=100;

				if (KeyMoveEvent!=null)
					KeyMoveEvent(this, pntMove.ToEPointF());
			}
		}
	}
}
