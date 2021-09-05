using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Endogine.Serialization.Palettes
{
    [Category("pal")]
    public class Pal : Base
    {
        public Pal()
        { }
        public override Endogine.ColorEx.Palette Load(string filename)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override void Save(string filename, Endogine.ColorEx.Palette palette)
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }
}
