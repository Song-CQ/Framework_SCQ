using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Editors
{
    public interface ILocScaleRotEdit : IEditorForm
    {
        Sprite EditSprite
        { get; set; }

        bool AutoswitchToSprite
        { get; set; }
    }
}
