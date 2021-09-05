using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Editors
{
    public interface IMessageWindow : IEditorForm
    {
        void Put(string s);
    }
}
