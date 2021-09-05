using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Editors
{
    public interface IColorPickerForm : IEditorForm
    {
        ColorEx.ColorBase ColorObject
        { get; set; }
    }
}
