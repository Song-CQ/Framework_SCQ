using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for CustomSplitter.
	/// </summary>
	public class CustomSplitter : System.Windows.Forms.UserControl
	{
		private System.ComponentModel.Container components = null;
		private bool isDragging = false;
		private EPoint mouseDownLoc;
		private EPoint mouseLastLoc;

		public delegate void SplitterEventHandler(object sender, int newLoc);
		public event SplitterEventHandler SplitterMoving;
		public event SplitterEventHandler SplitterMoved;


		public CustomSplitter()
		{
			InitializeComponent();
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
			// 
			// CustomSplitter
			// 
			this.BackColor = System.Drawing.Color.Black;
			this.Cursor = System.Windows.Forms.Cursors.VSplit;
			this.Name = "CustomSplitter";
			this.Size = new System.Drawing.Size(2, 150);
			this.Load += new System.EventHandler(this.CustomSplitter_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CustomSplitter_MouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CustomSplitter_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomSplitter_MouseDown);

		}
		#endregion

		private void CustomSplitter_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.isDragging = true;
			this.mouseDownLoc = new EPoint(e.X, e.Y);
			this.mouseLastLoc = new EPoint(e.X, e.Y);
		}

		private void CustomSplitter_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.isDragging)
			{
				EPoint pntDiff = new EPoint(e.X, e.Y) - this.mouseLastLoc;
				this.mouseLastLoc = new EPoint(e.X, e.Y);
				//TODO:why doesn't it work? this.Left+= pntDiff.X;

				if (SplitterMoving!=null)
					SplitterMoving(this, pntDiff.X);
			}
		}

		private void CustomSplitter_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.isDragging = false;
			EPoint pntDiff = new EPoint(e.X, e.Y) - this.mouseLastLoc; //mouseDownLoc;
			this.mouseLastLoc = new EPoint(e.X, e.Y);
			this.Left+= (this.mouseLastLoc-this.mouseDownLoc).X;
			
			if (SplitterMoved!=null)
				SplitterMoved(this, pntDiff.X);
		}

		public int DraggedTotal
		{
			get {return (this.mouseLastLoc-this.mouseDownLoc).X;}
		}

		private void Parent_SizeChanged(object sender, EventArgs e)
		{
			if (this.Parent != null)
				this.Height = this.Parent.Height;
		}

		private void CustomSplitter_Load(object sender, System.EventArgs e)
		{
			this.Parent.SizeChanged+=new EventHandler(Parent_SizeChanged);
			this.Height = this.Parent.Height;
		}
	}
}
