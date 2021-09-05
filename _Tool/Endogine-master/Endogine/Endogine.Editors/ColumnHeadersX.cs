using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Endogine.Editors
{
	public class VersatileDataGridColumnStyle
	{
		private int width = 30;
		private string headerText;
		private string columnName = "Default";
		private System.Type displayType;
		private int displayOrdinal;

		public delegate void ChangedSizeDelegate(object sender, int oldSize);
		public event ChangedSizeDelegate ChangedWidth;

		public VersatileDataGridColumnStyle()
		{
			displayType = typeof(System.Windows.Forms.TextBox);
			//displayType = typeof(System.Windows.Forms.PictureBox);
		}
		public int Width
		{
			get {return this.width;}
			set
			{
				int old = this.width;
				this.width = value;
				if (ChangedWidth!=null)
					ChangedWidth(this, old);
			}
		}
		public string HeaderText
		{
			get {return this.headerText;}
			set {this.headerText = value;}
		}
		public string ColumnName
		{
			get {return this.columnName;}
			set {this.columnName = value;}
		}
		public int DisplayOrdinal
		{
			get {return this.displayOrdinal;}
			set {this.displayOrdinal = value;}
		}
		public System.Type DisplayType
		{
			get {return this.displayType;}
			set {this.displayType = value;}
		}
	}


	/// <summary>
	/// Summary description for ColumnHeadersX.
	/// </summary>
	public class ColumnHeadersX : System.Windows.Forms.UserControl
	{
		private System.ComponentModel.Container components = null;

		public delegate void ChangedSizeDelegate(object sender, string columnName, int newSize);
		public event ChangedSizeDelegate ChangedColumnWidth;

		public delegate void ChangedSortingDelegate(object sender, string columnName, bool isAscending);
		public event ChangedSortingDelegate ChangedColumnSorting;

		private SortedList slColumnStyles = null;
		private Hashtable htCachedColumnLeftRights;
		public int SplitterWidth = 2;
		private bool bNoRefresh = false;

		public ColumnHeadersX()
		{
			InitializeComponent();
		}

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

		public void SetupColumns()
		{
//			for (int i = this.Controls.Count-1; i >= 0; i--)
//			{
//				Control ctrl = this.Controls[i];
//				if (ctrl != this.listView1)
//					this.Controls.Remove(ctrl);
//			}
//			this.listView1.Columns.Clear();
//			this.listView1.Visible = false;

			this.Controls.Clear();

			for (int x = 0; x < this.slColumnStyles.Count; x++)
			{
				VersatileDataGridColumnStyle columnStyle = this.GetColumnStyle(x);
				//					TextBox headerCtrl = new TextBox();
				//					headerCtrl.BorderStyle = BorderStyle.None;
				//					headerCtrl.Enabled = false;
				//					headerCtrl.BackColor = Color.White;


				ColumnHeader colHeader = new ColumnHeader();
				colHeader.Text = columnStyle.ColumnName;
				colHeader.Width = columnStyle.Width;
//				this.listView1.Columns.Add(colHeader);


				System.Windows.Forms.Button headerCtrl = new System.Windows.Forms.Button();
				headerCtrl.Text = columnStyle.ColumnName;
				headerCtrl.FlatStyle = FlatStyle.Popup;
				headerCtrl.Font = new Font("Verdana", 6.75f);
				headerCtrl.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);

				headerCtrl.Tag = columnStyle;
				headerCtrl.Click+=new EventHandler(headerCtrl_Click);
				this.Controls.Add(headerCtrl); //this.panelSubHeaders
				//headerCtrl.Visible = false;

				CustomSplitter splitter = new CustomSplitter();
				splitter.Width = this.SplitterWidth;
				splitter.Tag = headerCtrl;
				//					splitter.SplitterMoving+=new Endogine.Editors.CustomSplitter.SplitterEventHandler(splitter_SplitterMoving);
				splitter.SplitterMoved+=new Endogine.Editors.CustomSplitter.SplitterEventHandler(splitter_SplitterMoved);
				this.Controls.Add(splitter); //this.panelSubHeaders
			}
			this.RecalcColumnLeftRights();
		}

		public void RefreshLayout()
		{
			EPoint pntHeaderBottomRight = new EPoint();

			for (int x = 0; x < this.slColumnStyles.Count; x++)
			{
				VersatileDataGridColumnStyle columnStyle = GetColumnStyle(x);
				EPoint pntLeftRight = this.GetColumnLeftRight(columnStyle.ColumnName);

				//rct.Height = this.panelHeaders.Height; //panelSubHeaders
				Control ctrl = this.Controls[x*2]; //panelSubHeaders
				ctrl.Width = pntLeftRight.Y-pntLeftRight.X;
				ctrl.Left = pntLeftRight.X;

				//				this.listView1.Columns[x].Width = pntLeftRight.Y-pntLeftRight.X;


				Control splitter = this.Controls[x*2+1]; //panelSubHeaders
				splitter.Left = pntLeftRight.Y;
				//splitter.Height = rct.Height;

				pntHeaderBottomRight.X = pntLeftRight.Y;
				//pntHeaderBottomRight.Y = rct.Bottom;
			}
//			this.listView1.Width = pntHeaderBottomRight.X;
			this.Width = pntHeaderBottomRight.X;

			//panelSubHeaders.Width = pntHeaderBottomRight.X;
			//panelSubHeaders.Height = pntHeaderBottomRight.Y;
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ColumnHeadersX
			// 
			this.Name = "ColumnHeadersX";
			this.Size = new System.Drawing.Size(150, 16);

		}
		#endregion

		public bool NoRefresh
		{
			set {this.bNoRefresh = value;}
		}
		public ArrayList ColumnStyles
		{
			set
			{
				if (slColumnStyles!=null)
				{
					for (int i = 0; i < slColumnStyles.Count; i++)
					{
						VersatileDataGridColumnStyle colStyle = (VersatileDataGridColumnStyle)slColumnStyles.GetByIndex(i);
						colStyle.ChangedWidth-=new VersatileDataGridColumnStyle.ChangedSizeDelegate(colStyle_ChangedWidth);
					}
				}
				slColumnStyles = new SortedList();
				for (int i = 0; i < value.Count; i++)
				{
					VersatileDataGridColumnStyle colStyle = (VersatileDataGridColumnStyle)value[i];
					colStyle.DisplayOrdinal = i;
					colStyle.ChangedWidth+=new VersatileDataGridColumnStyle.ChangedSizeDelegate(colStyle_ChangedWidth);
					slColumnStyles.Add(colStyle.ColumnName, colStyle);
				}
//				if (!this.bNoRecalcing)
					this.RecalcColumnLeftRights();
			}

			get
			{
				ArrayList aStyles = new ArrayList();
				if (slColumnStyles==null)
					slColumnStyles = new SortedList();
				for (int i = 0; i < slColumnStyles.Count; i++)
				{
					VersatileDataGridColumnStyle colStyle = (VersatileDataGridColumnStyle)slColumnStyles.GetByIndex(i);
					aStyles.Add(colStyle);
				}
				return aStyles;
			}
		}

		public VersatileDataGridColumnStyle GetColumnStyle(int displayOrdinal)
		{
			for (int i = 0; i < slColumnStyles.Count; i++)
			{
				VersatileDataGridColumnStyle style = (VersatileDataGridColumnStyle)slColumnStyles.GetByIndex(i);
				if (style.DisplayOrdinal == displayOrdinal)
					return GetColumnStyle(style.ColumnName);
			}
			return null;
		}

		public VersatileDataGridColumnStyle GetColumnStyle(string columnName)
		{
//			if (this.slColumnStyles.Count == 0)
//			{
//				ArrayList aStyles = new ArrayList();
//				aStyles.Add(new VersatileDataGridColumnStyle());
//				this.ColumnStyles = aStyles;
//			}

			VersatileDataGridColumnStyle columnStyle = (VersatileDataGridColumnStyle)
				slColumnStyles[columnName];
            //if (false && columnStyle == null)
            //{
            //    ArrayList aStyles = this.ColumnStyles;
            //    VersatileDataGridColumnStyle style = new VersatileDataGridColumnStyle();
            //    style.ColumnName = columnName;
            //    aStyles.Add(style);
            //    this.ColumnStyles = aStyles;

            //    columnStyle = style; //(VersatileDataGridColumnStyle)slColumnStyles.GetByIndex(0);
            //}
			return columnStyle;
		}


		public EPoint GetColumnLeftRight(string columnName)
		{
			return (EPoint)this.htCachedColumnLeftRights[columnName];
		}

		public void RecalcColumnLeftRights()
		{
			this.htCachedColumnLeftRights = new Hashtable();

			int nTotalWidth = 0;
			for (int x = 0; x < this.slColumnStyles.Count; x++)
			{
				VersatileDataGridColumnStyle columnStyle = GetColumnStyle(x);

				EPoint pntRange = new EPoint(nTotalWidth, nTotalWidth+columnStyle.Width);
				//EH.Put(col.ColumnName + " " + pntRange.ToString());
				this.htCachedColumnLeftRights.Add(columnStyle.ColumnName, pntRange);

				nTotalWidth+=columnStyle.Width+this.SplitterWidth;
			}
		}



		private void colStyle_ChangedWidth(object sender, int newWidth)
		{
			if (bNoRefresh)
				return;

			this.RecalcColumnLeftRights();
			this.RefreshLayout();
			if (ChangedColumnWidth!=null)
				ChangedColumnWidth(this, ((VersatileDataGridColumnStyle)sender).ColumnName, newWidth);
		}

		private void headerCtrl_Click(object sender, EventArgs e)
		{
			Control ctrl = (Control)sender;
			VersatileDataGridColumnStyle columnStyle = (VersatileDataGridColumnStyle)ctrl.Tag;

			if (ChangedColumnSorting!=null)
				ChangedColumnSorting(this, columnStyle.ColumnName, true);
		}

		private void splitter_SplitterMoved(object sender, int newLoc)
		{
			CustomSplitter splitter = (CustomSplitter)sender;
			Control ctrl = (Control)splitter.Tag;
			VersatileDataGridColumnStyle columnStyle = (VersatileDataGridColumnStyle)ctrl.Tag;
			columnStyle.Width += splitter.DraggedTotal;
		}
	}
}
