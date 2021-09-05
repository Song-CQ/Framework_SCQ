using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Endogine.Serialization.Palettes
{
    [Category("txt")]
    public class Painter : Base
    {
        public override Endogine.ColorEx.Palette Load(string filename)
        {
//              --readPainterTxt("C:\Program\Corel\Corel Painter IX\Color Sets\_test.txt")
//  --p-ut readPainterTxt("C:\Program\Corel\Corel Painter IX\Color Sets\Pantone\PANTONE(R) pastel coated.txt")
            string contents = Files.FileReadWrite.Read(filename);

            Endogine.ColorEx.Palette palette = new Endogine.ColorEx.Palette();

            //remove header:
            int indexR = contents.IndexOf("R:");
            contents = contents.Remove(0, indexR);

            int colorIndex = 0;
            //  nNumColors = szContents.lines.count
            string[] lines = contents.Split("\r\n".ToCharArray());
            foreach (string line in lines)
        	{
                ColorEx.ColorRgb rgb = new Endogine.ColorEx.ColorRgb();
                string[] items = line.Split(':');
                rgb.R = this.GetPainterVal(items[0]);
                rgb.G = this.GetPainterVal(items[1]);
                rgb.B = this.GetPainterVal(items[2]);
   
                //TODO: they're not always present!!
                for (int i = 3; i < 6; i++)
    			{
                    //aUnknowns[n] = me.cleanPainterVal(szLine.item[n+1])
	    		}

                string name = colorIndex.ToString();
                int index = line.IndexOf("VV");
                if (index >= 0)
                {
                    string vv = line.Remove(0, index);
                    index = vv.IndexOf("  ");
                    if (index >= 0)
                    {
                        name = vv.Remove(0, index + 2);
                        name = vv.Remove(vv.Length-1);
                    }
                }
                palette.Add(name, rgb);
                colorIndex++;
            }
            return palette;
        }

        private int GetPainterVal(string val)
        {
            int index = val.IndexOf(",");
            if (index >= 0)
                val = val.Remove(index);
            return Convert.ToInt32(val);
        }

        public override void Save(string filename, Endogine.ColorEx.Palette palette)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
