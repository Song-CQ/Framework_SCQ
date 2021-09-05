using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Endogine.Editors
{
	//should use this instead? http://www.codeproject.com/cs/miscctrl/XPTable.asp
	/// <summary>
	/// Summary description for VersatileDataGrid.
	/// CustomUITypeEditor .NET Samples – Windows Forms: Control Authoring.
	/// </summary>
	public class VersatileDataGrid : System.Windows.Forms.UserControl
	{
		public delegate void ChangedSizeDelegate(object sender, int oldSize);
		public delegate void ScrolledDelegate(object sender, int newLoc);

		public class VersatileDataGridRowStyle
		{
			private int height = 16;
			public event ChangedSizeDelegate ChangedHeight;
			public VersatileDataGridRowStyle()
			{
			}
			public int Height
			{
				get {return this.height;}
				set
				{
					int old = this.height;
					this.height = value;
					if (ChangedHeight!=null)
						ChangedHeight(this, old);
				}
			}
		}

		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private object dataSource = null;
		private SortedList slColumnStyles = null;
		private ArrayList aRowStyles = null;
		private System.Windows.Forms.Panel panelGrid;
		private System.Windows.Forms.Panel panelHeaders;
		private bool columnHeadersVisible = true;

		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.Panel panelSubGrid;

		private EPoint pntCachedBottomRightLoc = null;
		private Endogine.Editors.ColumnHeadersX columnHeaders1;

		public event ScrolledDelegate ScrolledVertical;
		private bool bNoRecalcing;

		public VersatileDataGrid()
		{
			InitializeComponent();
			this.slColumnStyles = new SortedList();
			this.aRowStyles = new ArrayList();
			this.aRowStyles.Add(new VersatileDataGridRowStyle());
			this.pntCachedBottomRightLoc = new EPoint();

			this.columnHeaders1.Height = this.panelHeaders.Height;
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
			this.panelGrid = new System.Windows.Forms.Panel();
			this.panelSubGrid = new System.Windows.Forms.Panel();
			this.panelHeaders = new System.Windows.Forms.Panel();
			this.columnHeaders1 = new Endogine.Editors.ColumnHeadersX();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.panelGrid.SuspendLayout();
			this.panelHeaders.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelGrid
			// 
			this.panelGrid.Controls.Add(this.panelSubGrid);
			this.panelGrid.Location = new System.Drawing.Point(0, 17);
			this.panelGrid.Name = "panelGrid";
			this.panelGrid.Size = new System.Drawing.Size(120, 72);
			this.panelGrid.TabIndex = 0;
			// 
			// panelSubGrid
			// 
			this.panelSubGrid.Location = new System.Drawing.Point(0, 0);
			this.panelSubGrid.Name = "panelSubGrid";
			this.panelSubGrid.Size = new System.Drawing.Size(104, 48);
			this.panelSubGrid.TabIndex = 0;
			// 
			// panelHeaders
			// 
			this.panelHeaders.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelHeaders.Controls.Add(this.columnHeaders1);
			this.panelHeaders.Location = new System.Drawing.Point(0, 0);
			this.panelHeaders.Name = "panelHeaders";
			this.panelHeaders.Size = new System.Drawing.Size(120, 18);
			this.panelHeaders.TabIndex = 1;
			// 
			// columnHeaders1
			// 
			this.columnHeaders1.Location = new System.Drawing.Point(0, 0);
			this.columnHeaders1.Name = "columnHeaders1";
			this.columnHeaders1.Size = new System.Drawing.Size(96, 16);
			this.columnHeaders1.TabIndex = 4;
			this.columnHeaders1.ChangedColumnSorting += new Endogine.Editors.ColumnHeadersX.ChangedSortingDelegate(this.columnHeaders1_ChangedColumnSorting);
			this.columnHeaders1.ChangedColumnWidth += new Endogine.Editors.ColumnHeadersX.ChangedSizeDelegate(this.columnHeaders1_ChangedColumnWidth);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Location = new System.Drawing.Point(120, 17);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(16, 72);
			this.vScrollBar1.TabIndex = 3;
			this.vScrollBar1.ValueChanged += new System.EventHandler(this.vScrollBar1_ValueChanged);
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Location = new System.Drawing.Point(0, 89);
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(120, 16);
			this.hScrollBar1.TabIndex = 2;
			this.hScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
			// 
			// VersatileDataGrid
			// 
			this.Controls.Add(this.vScrollBar1);
			this.Controls.Add(this.hScrollBar1);
			this.Controls.Add(this.panelHeaders);
			this.Controls.Add(this.panelGrid);
			this.Name = "VersatileDataGrid";
			this.Size = new System.Drawing.Size(136, 128);
			this.Resize += new System.EventHandler(this.VersatileDataGrid_Resize);
			this.Load += new System.EventHandler(this.VersatileDataGrid_Load);
			this.panelGrid.ResumeLayout(false);
			this.panelHeaders.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion



		public object DataSource
		{
			set
			{
				if (value.GetType() == typeof(DataTable)
					|| value.GetType() == typeof(DataSet)
					|| value.GetType() == typeof(DataView))
				{
					dataSource = value;
					this.DataBind();
				}
			}
		}

		public void SetColumnStyle()
		{
		}
		public void SetColumnSpecificStyle(VersatileDataGridColumnStyle colStyle)
		{
			VersatileDataGridColumnStyle style = this.columnHeaders1.GetColumnStyle(colStyle.ColumnName);
			
			ArrayList aStyles = this.columnHeaders1.ColumnStyles;
			if (style!=null)
				aStyles.Remove(style);
			aStyles.Add(colStyle);
			this.columnHeaders1.ColumnStyles = aStyles;
		}


		private void DataBind()
		{
			DataView dv = this.GetDataView();

			ArrayList aColumnStyles = this.columnHeaders1.ColumnStyles;
			for (int x = 0; x < dv.Table.Columns.Count; x++)
			{
				DataColumn col = dv.Table.Columns[x];
				VersatileDataGridColumnStyle columnStyle = this.columnHeaders1.GetColumnStyle(col.ColumnName);
				if (columnStyle == null)
				{
					columnStyle =new Endogine.Editors.VersatileDataGridColumnStyle();
					columnStyle.ColumnName = col.ColumnName;
					columnStyle.DisplayOrdinal = x;
					aColumnStyles.Add(columnStyle);
				}
			}
			this.columnHeaders1.ColumnStyles = aColumnStyles;

			this.panelSubGrid.Controls.Clear();
			this.panelSubGrid.SuspendLayout();

			this.columnHeaders1.SetupColumns();
			this.columnHeaders1.Visible = this.columnHeadersVisible;

			this.bNoRecalcing = true;


			for (int y = 0; y < dv.Count; y++)
			{
				DataRowView rowView = dv[y];
				for (int x = 0; x < dv.Table.Columns.Count; x++)
				{
					DataColumn col = dv.Table.Columns[x];
					VersatileDataGridColumnStyle columnStyle = this.GetColumnStyle(col.ColumnName);

					Control ctrl = null;
					if (columnStyle.DisplayType == typeof(TextBox))
					{
						TextBox tb = new TextBox();
						tb = new TextBox();
						tb.BorderStyle = BorderStyle.None;
						tb.Enabled = false;
                        tb.BackColor = System.Drawing.Color.White;
						tb.Text = rowView[col.ColumnName].ToString();
						ctrl = tb;
					}
					else if (columnStyle.DisplayType == typeof(System.Windows.Forms.PictureBox))
					{
						PictureBox pb = new System.Windows.Forms.PictureBox();
						//Endogine.ResourceManagement.MemberBitmapBase
						//	mb = new Endogine.ResourceManagement.MemberBitmapBase();
						object obj = rowView[col.ColumnName];
						if (obj.GetType() == typeof(Endogine.MemberSpriteBitmap))
						{
						}
						Endogine.MemberSpriteBitmap mb =
							(Endogine.MemberSpriteBitmap)EH.Instance.CastLib.GetOrCreate("Button2Down");
						Bitmap bmp = mb.Thumbnail;
//						Bitmap bmp = new Bitmap(20,20,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
//						Graphics g = Graphics.FromImage(bmp);
//						g.FillRectangle(new SolidBrush(Color.Red), 0,0,20,20);
						pb.Image = bmp;
						ctrl = pb;
					}

					if (ctrl!=null)
						this.panelSubGrid.Controls.Add(ctrl);
				}
			}
			this.bNoRecalcing = false;

			this.panelSubGrid.ResumeLayout();
			//this.columnHeaders1.ResumeLayout();

			RefreshLayout();
		}

		private void RefreshLayout()
		{
			this.columnHeaders1.RefreshLayout();
			this.columnHeaders1.Height = this.panelHeaders.Height;

			this.panelSubGrid.SuspendLayout();
			//this.columnHeaders1.SuspendLayout();

			DataView dv = this.GetDataView();
			pntCachedBottomRightLoc = new EPoint();
			for (int y = 0; y < dv.Count; y++)
			{
				//DataRowView rowView = dv[y];
				for (int x = 0; x < dv.Table.Columns.Count; x++)
				{
					DataColumn col = dv.Table.Columns[x];
					VersatileDataGridColumnStyle columnStyle = this.GetColumnStyle(col.ColumnName);

					Control ctrl = this.panelSubGrid.Controls[x + y*dv.Table.Columns.Count];
					Rectangle rct = this.GetBoundingRect(y,x);
					//EH.Put(x.ToString() + ";"+y.ToString()+" " + rct.ToString());
					ctrl.Location = rct.Location;
					ctrl.Size = rct.Size;
					pntCachedBottomRightLoc.X = ctrl.Right;
					pntCachedBottomRightLoc.Y = ctrl.Bottom;
				}
			}
			this.panelSubGrid.Width = pntCachedBottomRightLoc.X;
			this.panelSubGrid.Height = pntCachedBottomRightLoc.Y;

			this.panelSubGrid.ResumeLayout();
			//this.panelSubHeaders.ResumeLayout();

			this.UpdateSize();
		}

		private DataView GetDataView()
		{
			DataView dv = null;
			if (dataSource.GetType() == typeof(DataTable))
				dv = (DataView)((DataTable)dataSource).DefaultView;
			else if (dataSource.GetType() == typeof(DataSet))
				dv = (DataView)((DataTable)((DataSet)dataSource).Tables[0]).DefaultView;
			else if (dataSource.GetType() == typeof(DataView))
				dv = (DataView)dataSource;
			return dv;
		}

		public Rectangle GetBoundingRect(int rowIndex, int colIndex)
		{
			Rectangle rct = new Rectangle(0,0,0,0);
			DataView dv = this.GetDataView();

			DataColumn col = dv.Table.Columns[colIndex];
			EPoint pntRange = this.columnHeaders1.GetColumnLeftRight(col.ColumnName);
			rct.X = pntRange.X;
			rct.Width = pntRange.Y-pntRange.X;


			int nTotalHeight = 0;
			for (int y = 0; y <= rowIndex; y++)
			{
				VersatileDataGridRowStyle rowStyle = this.GetRowStyle(y);
				if (y == rowIndex)
				{
					rct.Y = nTotalHeight;
					rct.Height = rowStyle.Height;
				}
				nTotalHeight+=rowStyle.Height;
			}
			return rct;
		}

		private VersatileDataGridRowStyle GetRowStyle(int rowIndex)
		{
			//TODO: support alternating styles (pattern: 0,0,1,1 or 0,1 etc)
			VersatileDataGridRowStyle rowStyle = (VersatileDataGridRowStyle)this.aRowStyles[0];
			return rowStyle;
		}

		private void colStyle_ChangedWidth(object sender, int newSize)
		{
			if (bNoRecalcing)
				return;
			this.RefreshLayout();
		}


		private void VersatileDataGrid_Resize(object sender, System.EventArgs e)
		{
			this.UpdateSize();
		}

		private void UpdateSize()
		{
			int nNewPanelHeight = this.Height - this.panelGrid.Top;
			int nNewPanelWidth = this.Width - this.panelGrid.Left;

			this.vScrollBar1.Visible = (this.pntCachedBottomRightLoc.Y > nNewPanelHeight)?true:false;
			this.hScrollBar1.Visible = (this.pntCachedBottomRightLoc.X > nNewPanelWidth)?true:false;

			if (this.hScrollBar1.Visible)
				this.panelGrid.Height = nNewPanelHeight-this.hScrollBar1.Height;
			else
				this.panelGrid.Height = nNewPanelHeight;

			if (this.vScrollBar1.Visible)
				this.panelGrid.Width = nNewPanelWidth - this.vScrollBar1.Width;
			else
				this.panelGrid.Width = nNewPanelWidth;

			if (this.hScrollBar1.Visible)
			{
				//EH.Put(this.Height.ToString() + " " + this.panelGrid.Top.ToString());
				this.hScrollBar1.Top = this.panelGrid.Bottom;
				this.hScrollBar1.Width = this.Width - this.hScrollBar1.Left;
				if (this.vScrollBar1.Visible)
					this.hScrollBar1.Width-=this.vScrollBar1.Width;
				this.hScrollBar1.Maximum = this.pntCachedBottomRightLoc.X;
				this.hScrollBar1.LargeChange = this.hScrollBar1.Width;
			}
			if (this.vScrollBar1.Visible)
			{
				this.vScrollBar1.Left = this.panelGrid.Right;
				this.vScrollBar1.Height = this.Height - this.vScrollBar1.Top;
				if (this.hScrollBar1.Visible)
					this.vScrollBar1.Height-=this.hScrollBar1.Height;
				this.vScrollBar1.Maximum = this.pntCachedBottomRightLoc.Y;
				this.vScrollBar1.LargeChange = this.vScrollBar1.Height;
			}

			this.panelHeaders.Width = nNewPanelWidth;
		}

		private void vScrollBar1_ValueChanged(object sender, System.EventArgs e)
		{
			this.panelSubGrid.Top = -vScrollBar1.Value;
			if (ScrolledVertical!=null)
				ScrolledVertical(this, vScrollBar1.Value);
		}

		private void hScrollBar1_ValueChanged(object sender, System.EventArgs e)
		{
			this.panelSubGrid.Left = -hScrollBar1.Value;
			this.columnHeaders1.Left = -hScrollBar1.Value;
		}

		private void VersatileDataGrid_Load(object sender, System.EventArgs e)
		{
		}

		public int GridTotalHeight
		{
			get {return this.pntCachedBottomRightLoc.Y;}
		}

		public VersatileDataGridColumnStyle GetColumnStyle(string columnName)
		{
			return this.columnHeaders1.GetColumnStyle(columnName);
		}

		public void AutoAdjustColumns(int nMinWidth, int nMaxWidth, bool bOnlyLookHeaders)
		{
			bNoRecalcing = true;
			this.columnHeaders1.NoRefresh = true;

			DataView dv = this.GetDataView();

			bool bIncludeHeaders = true;
			foreach (DataColumn col in dv.Table.Columns)
			{
				int nFoundMaxWidth = 0;

				int rowStart;
				if (bIncludeHeaders)
					rowStart = -1;
				else
					rowStart = 0;
				for (int y = rowStart; y < dv.Count; y++)
				{
					int nWidth = this.GetWantedWidth(y, col.Ordinal);
					nFoundMaxWidth = Math.Max(nWidth, nFoundMaxWidth);
				}
				VersatileDataGridColumnStyle style = GetColumnStyle(col.ColumnName);

				style.Width = Math.Max(nMinWidth,Math.Min(nMaxWidth, nFoundMaxWidth));
			}

			bNoRecalcing = false;
			this.columnHeaders1.NoRefresh = false;
			this.columnHeaders1.RecalcColumnLeftRights();

			this.RefreshLayout();
		}

		private int GetWantedWidth(int rowIndex, int colIndex)
		{
			DataView dv = this.GetDataView();
			DataColumn col = dv.Table.Columns[colIndex];
			string s;
			if (rowIndex < 0)
				s = col.ColumnName;
			else
				s = dv[rowIndex][col.ColumnName].ToString();
			//TODO: how to find out the width of this string in the specific font used??
			//Until then, use an average...
			int nWidth = (int)(s.Length * 5.6)+1;

			return nWidth;
//			VersatileDataGridColumnStyle style = GetColumnStyle(col);
		}

		private void columnHeaders1_ChangedColumnWidth(object sender, string columnName, int newSize)
		{
			this.RefreshLayout();
		}

		private void columnHeaders1_ChangedColumnSorting(object sender, string columnName, bool isAscending)
		{
			//TODO: create a new dataView sorted by this column
			DataView dv = this.GetDataView();
			string sort = dv.Sort;
			//TODO: fire event so e.g. treeView can be changed to reflect the new sorting
		}

//		public ColumnHeadersX.VersatileDataGridColumnStyle 
	}
}
