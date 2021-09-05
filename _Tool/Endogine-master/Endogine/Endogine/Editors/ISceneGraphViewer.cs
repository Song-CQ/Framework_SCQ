using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Editors
{
    public interface ISceneGraphViewer : IEditorForm
    {
        Sprite SelectedSprite
        { get; set;}

        void MarkSprite(Sprite sprite);
    }
}
