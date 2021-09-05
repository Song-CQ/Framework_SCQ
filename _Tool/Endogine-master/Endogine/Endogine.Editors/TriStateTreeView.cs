// Adapted from the VB.NET control by Carlos J. Quintero:
// http://www.codeproject.com/vb/net/vbnettristatechkbox.asp

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Endogine.Editors
{
    public partial class TriStateTreeView : TreeView
    {
        public enum CheckBoxState
        {
            None = 0,
            Unchecked,
            Checked,
            Indeterminate
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wparam, IntPtr lparam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wparam, ref TVITEM lparam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wparam, ref TVHITTESTINFO lparam);

        [StructLayout(LayoutKind.Sequential)]
        public struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            public int pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TVHITTESTINFO
        {
            public POINTAPI pt;
            public int flags;
            public IntPtr hItem;
        }

        //Messages
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETIMAGELIST = TV_FIRST + 9;
        private const int TVM_GETITEM = TV_FIRST + 12;
        private const int TVM_SETITEM = TV_FIRST + 13;
        private const int TVM_HITTEST = TV_FIRST + 17;

        // TVM_SETIMAGELIST image list kind
        private const int TVSIL_STATE = 2;

        //TVITEM.mask flags
        private const int TVIF_STATE = 0x8;
        private const int TVIF_HANDLE = 0x10;

        //TVITEM.state flags
        private const int TVIS_STATEIMAGEMASK = 0xF000;

        //TVHITTESTINFO.flags flags
        private const int TVHT_ONITEMSTATEICON = 0x40;




        public TriStateTreeView()
        {
            InitializeComponent();

            this.StateImageList = this.stateImageList;
            this.ImageList = this.iconsImageList;
        }

        public new ImageList StateImageList
        {
            set
            {
                //Set the state image list
                SendMessage(this.Handle, TVM_SETIMAGELIST, TVSIL_STATE, value.Handle);
            }
        }

        private void SetTreeNodeAndChildrenStateRecursively(TriStateTreeNode node, CheckBoxState state)
        {
            if (node == null)
                return;

            this.SetTreeNodeState(node, state);
            foreach (TriStateTreeNode childNode in node.Nodes)
                this.SetTreeNodeAndChildrenStateRecursively(childNode, state);
        }

        private void SetParentTreeNodeStateRecursively(TriStateTreeNode parentNode)
        {
            if (parentNode == null)
                return;
            if (this.GetTreeNodeState(parentNode) == CheckBoxState.None)
                return;


            bool allChildrenChecked = true;
            bool allChildrenUnchecked = true;

            foreach (TriStateTreeNode node in parentNode.Nodes)
            {
                switch (this.GetTreeNodeState(node))
                {
                    case CheckBoxState.Checked:
                        allChildrenUnchecked = false;
                        break;
                    case CheckBoxState.Indeterminate:
                        allChildrenChecked = false;
                        allChildrenUnchecked = false;
                        break;
                    case CheckBoxState.Unchecked:
                        allChildrenChecked = false;
                        break;
                }
                if (allChildrenChecked == false && allChildrenUnchecked == false)
                    break; //This is an optimization
            }

            if (allChildrenChecked)
                this.SetTreeNodeState(parentNode, CheckBoxState.Checked);
            else if (allChildrenUnchecked)
                this.SetTreeNodeState(parentNode, CheckBoxState.Unchecked);
            else
                this.SetTreeNodeState(parentNode, CheckBoxState.Indeterminate);

            if (parentNode.Parent != null)
                this.SetParentTreeNodeStateRecursively((TriStateTreeNode)parentNode.Parent);
        }

        public CheckBoxState GetTreeNodeState(TriStateTreeNode node)
        {
            TVITEM tvItem = new TVITEM();
            tvItem.mask = TVIF_HANDLE | TVIF_STATE;
            tvItem.hItem = node.Handle;
            tvItem.stateMask = TVIS_STATEIMAGEMASK;
            tvItem.state = 0;

            IntPtr ptr = SendMessage(this.Handle, TVM_GETITEM, 0, ref tvItem);
            if (ptr != null)
            {
                //State image index in bits 12..15
                int iState = tvItem.state >> 12;

                if (iState == (int)CheckBoxState.None)
                    return CheckBoxState.None;
                if (iState == (int)CheckBoxState.Unchecked)
                    return CheckBoxState.Unchecked;
                if (iState == (int)CheckBoxState.Checked)
                    return CheckBoxState.Checked;
                if (iState == (int)CheckBoxState.Indeterminate)
                    return CheckBoxState.Indeterminate;
            }
            return CheckBoxState.None;
        }


        public void SetTreeNodeState(TriStateTreeNode node, CheckBoxState state)
        {
            if (node == null)
                return;

            TVITEM tvItem = new TVITEM();
            tvItem.mask = TVIF_HANDLE | TVIF_STATE;
            tvItem.hItem = node.Handle;
            tvItem.stateMask = TVIS_STATEIMAGEMASK;
            //State image index in bits 12..15
            tvItem.state = ((int)state) << 12;

            SendMessage(this.Handle, TVM_SETITEM, 0, ref tvItem);
        }

        public void ToggleTreeNodeState(TriStateTreeNode node)
        {
            this.BeginUpdate();
            switch (this.GetTreeNodeState(node))
            {
                case CheckBoxState.Unchecked:
                    this.SetTreeNodeAndChildrenStateRecursively(node, CheckBoxState.Checked);
                    this.SetParentTreeNodeStateRecursively((TriStateTreeNode)node.Parent);
                    break;

                case CheckBoxState.Checked:
                case CheckBoxState.Indeterminate:
                    this.SetTreeNodeAndChildrenStateRecursively(node, CheckBoxState.Unchecked);
                    this.SetParentTreeNodeStateRecursively((TriStateTreeNode)node.Parent);
                    break;
            }
            this.EndUpdate();
        }

        private TriStateTreeNode GetTreeNodeHitAtCheckBoxByScreenPosition(int iXScreenPos, int iYScreenPos)
        {
            Point ptClient = this.PointToClient(new Point(iXScreenPos, iYScreenPos));
            return this.GetTreeNodeHitAtCheckBoxByClientPosition(ptClient.X, ptClient.Y);
        }

        private TriStateTreeNode GetTreeNodeHitAtCheckBoxByClientPosition(int iXClientPos, int iYClientPos)
        {
            //Get the hit info
            TVHITTESTINFO tvHitTestInfo = new TVHITTESTINFO();
            tvHitTestInfo.pt.x = iXClientPos;
            tvHitTestInfo.pt.y = iYClientPos;

            IntPtr ptrTreeNode = SendMessage(this.Handle, TVM_HITTEST, 0, ref tvHitTestInfo);

            //Check if it has clicked on an item
            if (ptrTreeNode != null)
            {
                //Check if it has clicked on the state image of the item
                if ((tvHitTestInfo.flags & TVHT_ONITEMSTATEICON) != 0)
                    return (TriStateTreeNode)TriStateTreeNode.FromHandle(this, ptrTreeNode);
            }

            return null;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            TriStateTreeNode node = this.GetTreeNodeHitAtCheckBoxByClientPosition(e.X, e.Y);
            if (node != null)
                this.ToggleTreeNodeState(node);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Space)
            {
                if (this.SelectedNode != null)
                    this.ToggleTreeNodeState((TriStateTreeNode)this.SelectedNode);
            }
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            //PATCH: if the node is being expanded by a double click at the state image, cancel it
            if (this.GetTreeNodeHitAtCheckBoxByScreenPosition(Form.MousePosition.X, Form.MousePosition.Y) != null)
                e.Cancel = true;
        }

        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            //PATCH: if the node is being expanded by a double click at the state image, cancel it
            if (this.GetTreeNodeHitAtCheckBoxByScreenPosition(Form.MousePosition.X, Form.MousePosition.Y) != null)
                e.Cancel = true;
        }

        public TriStateTreeNode AddTreeNode(TreeNodeCollection nodes, string text, int imageIndex, CheckBoxState state)
        {
            TriStateTreeNode node = new TriStateTreeNode(text);
            node.ImageIndex = imageIndex;
            node.SelectedImageIndex = imageIndex;
            nodes.Add(node);
            this.SetTreeNodeState(node, state);
            return node;
        }
    }
}