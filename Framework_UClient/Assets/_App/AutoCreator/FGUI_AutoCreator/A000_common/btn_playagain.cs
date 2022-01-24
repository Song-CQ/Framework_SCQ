/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A000_common
{
    public partial class btn_playagain : GButton
    {
        public GTextField text_cardnum;
        public const string URL = "ui://pmf3wbjikmj6f4";

        public static btn_playagain CreateInstance()
        {
            return (btn_playagain)UIPackage.CreateObject("A000_common", "btn_playagain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            text_cardnum = (GTextField)GetChildAt(2);
        }
    }
}