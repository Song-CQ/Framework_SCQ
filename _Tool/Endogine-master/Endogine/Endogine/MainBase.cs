using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Endogine
{
	/// <summary>
	/// Summary description for MainBase.
	/// </summary>
	public class MainBase : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		protected EndogineHub m_endogine;
		//private Sprite draggedSprite = null;
		private Endogine.Editors.DragDropHelper dragDropHelper;

		public MainBase()
		{
			m_endogine = EndogineHub.Instance;
			InitializeComponent();

			if (m_endogine!=null)
			{
				this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque, true);
//				if (m_endogine.CurrentRenderStrategy == EndogineHub.RenderStrategy.GDI)
//					this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque, true);
//				else
//					this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			}
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// MainBase
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Name = "MainBase";
			this.Text = "MainBase";
			this.Closed += new System.EventHandler(this.MainBase_Closed);

		}
		#endregion

		
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e) 
		{
			if (e.KeyCode == Keys.Escape) 
			{
				this.Close();
				return;
			}
			if (m_endogine!=null) m_endogine.OnKeyDown(e);
		}
		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e) 
		{
			if (m_endogine!=null) m_endogine.OnKeyUp(e);
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) 
		{
			if (m_endogine!=null) m_endogine.OnPaint(e);
			else base.OnPaint(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (m_endogine!=null) m_endogine.OnMouseDown(this, e);
			base.OnMouseDown (e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (m_endogine!=null) m_endogine.OnMouseUp(this, e);
			base.OnMouseUp (e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (m_endogine!=null) m_endogine.OnMouseMove(this, e);
			base.OnMouseMove (e);
		}

		public virtual void EndogineInitDone()
		{
			dragDropHelper = new Endogine.Editors.DragDropHelper(this);
		}

		private void MainBase_Closed(object sender, System.EventArgs e)
		{
			EH.Instance.ShutDown();
		}
	}
}
