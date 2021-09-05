using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Endogine.Editors
{
    public class TriStateTreeNode : TreeNode
    {
        TriStateTreeView.CheckBoxState _state;

        public TriStateTreeNode()
        {
        }
        public TriStateTreeNode(string text) : base(text)
        {
        }

        public TriStateTreeView.CheckBoxState CheckBoxState
        {
            get { return this._state; }
            set
            {
                this._state = value;
                TriStateTreeView v = (TriStateTreeView)this.TreeView;
                v.SetTreeNodeState(this, value);
            }
        }
    }
}
