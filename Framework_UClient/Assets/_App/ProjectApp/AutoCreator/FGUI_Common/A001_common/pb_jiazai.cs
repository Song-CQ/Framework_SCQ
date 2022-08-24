/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A001_common
{
    public partial class pb_jiazai : GProgressBar
    {
        public GTextField text_val;
        public const string URL = "ui://pmf3wbjimgiifk";

        public static pb_jiazai CreateInstance()
        {
            return (pb_jiazai)UIPackage.CreateObject("A001_common", "pb_jiazai");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            text_val = (GTextField)GetChildAt(2);
        }
    }
}