using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine.Serialization.Palettes
{
    public abstract class Base
    {
        public abstract ColorEx.Palette Load(string filename);
        public abstract void Save(string filename, ColorEx.Palette palette);
    }
}
