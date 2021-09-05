using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Editors
{
    public interface IEditorForm
    {
        event EventHandler Disposed;
        void Show();
        System.Windows.Forms.Form MdiParent
        { get; set; }
        System.Drawing.Point Location
        { get; set; }

    }
}
