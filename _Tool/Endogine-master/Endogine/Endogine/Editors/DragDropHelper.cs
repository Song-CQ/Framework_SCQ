using System;
using System.Drawing;
using System.Windows.Forms;

namespace Endogine.Editors
{
	/// <summary>
	/// Summary description for DragDropHelper.
	/// </summary>
	public class DragDropHelper
	{
		private Sprite draggedSprite;
		private Control dragToControl;
		private bool isControlStage = false;
		private bool isControlTreeView = false;

		public event DragEventHandler DragDrop;

		public DragDropHelper(Control a_control)
		{
			this.dragToControl = a_control;

			this.isControlStage = (this.dragToControl.GetType().BaseType == typeof(Endogine.MainBase));
			this.isControlTreeView = (this.dragToControl.GetType() == typeof(System.Windows.Forms.TreeView));

			this.dragToControl.DragEnter+=new DragEventHandler(dragToControl_DragEnter);
			this.dragToControl.DragOver+=new DragEventHandler(dragToControl_DragOver);
			this.dragToControl.DragLeave+=new EventHandler(dragToControl_DragLeave);
			this.dragToControl.DragDrop+=new DragEventHandler(dragToControl_DragDrop);
		}

		private string[] GetDraggedInfo(System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(string)))
			{
				string sData = (string)e.Data.GetData(typeof(string));
				string[] aInfo = sData.Split(";".ToCharArray(), 2);
				if (aInfo[0] == "Member" || aInfo[0] == "File")
					return aInfo;
			}
			return null;
		}

		private void CreateDragDropSprite(System.Windows.Forms.DragEventArgs e)
		{
			string[] aInfo = GetDraggedInfo(e);
			if (aInfo==null)
				return;

			if (aInfo[0] == "Member" || aInfo[0] == "File")
			{
				//Endogine.ResourceManagement.MemberBase mb = EH.Instance.CastLib.GetByName(aInfo[1]);
				Endogine.ResourceManagement.MemberBase mb = 
					EH.Instance.CastLib.GetOrCreate(aInfo[1]);

				MemberSpriteBitmap mbSp = (MemberSpriteBitmap)mb;

				this.draggedSprite = new Sprite();
				this.draggedSprite.Member = mbSp;
				Point pnt = dragToControl.PointToClient(new Point(e.X, e.Y));
				//Point pnt = EH.Instance.Stage.RenderControl.PointToClient(new Point(e.X, e.Y));
				this.draggedSprite.Loc = new EPointF(pnt);
				EH.Instance.LatestCreatedSprites = null;
				this.draggedSprite.Parent = EH.Instance.SpriteToAddChildrenTo;
				if (this.draggedSprite.Disposing)
				{
					//after setting parent, the sprite may have been disposed,
					//and transformed into another sprite-derived type.
					//this.draggedSprite = null;
					this.draggedSprite = EH.Instance.LatestCreatedSprites[0];
				}

				//System.Runtime.Remoting.ObjectHandle obj = System.Activator.CreateInstance("EndoTest01", "Endogine.ResourceManagement.MemberBase");
				//object o = obj.Unwrap();
				//Endogine.ResourceManagement.MemberBase mb = (Endogine.ResourceManagement.MemberBase)o;	
			}
		}

		private void dragToControl_DragEnter(object sender, DragEventArgs e)
		{
			string[] aInfo = GetDraggedInfo(e);
			if (aInfo==null)
				e.Effect = DragDropEffects.None;

			e.Effect = DragDropEffects.Copy;
			this.dragToControl.Parent.Focus();
			this.dragToControl.Focus();

			if (this.isControlStage)
				this.CreateDragDropSprite(e);
		}

		private void dragToControl_DragOver(object sender, DragEventArgs e)
		{
			Point pnt = dragToControl.PointToClient(new Point(e.X, e.Y));
			if (this.draggedSprite!=null)
			{
				this.draggedSprite.Loc = new EPointF(pnt);
			}
			else if (this.isControlTreeView)
			{
				TreeNode node = ((TreeView)dragToControl).GetNodeAt(pnt.X, pnt.Y);
				if (node!=null)
				{
					if (node.GetNodeCount(false) > 0 && node.IsExpanded == false)
						node.Expand();
					node.TreeView.SelectedNode = node;
				}
			}
		}

		private void dragToControl_DragLeave(object sender, EventArgs e)
		{
			if (this.draggedSprite!=null)
			{
				this.draggedSprite.Dispose();
				this.draggedSprite = null;
			}
		}

		private void dragToControl_DragDrop(object sender, DragEventArgs e)
		{
			if (this.draggedSprite!=null)
			{
				this.draggedSprite = null;
			}
			else
			{
				this.CreateDragDropSprite(e);
				//set the loc to middle of parent sprite:
				this.draggedSprite.Loc = this.draggedSprite.Parent.Rect.Size/2;
				this.draggedSprite = null;

				if (DragDrop!=null)
					DragDrop(sender, e);

				if (this.isControlTreeView)
					dragToControl.Refresh();
			}
		}
	}
}