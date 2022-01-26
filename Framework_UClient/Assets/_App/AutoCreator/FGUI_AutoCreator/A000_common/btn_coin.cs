/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A000_common
{
    public partial class btn_coin : GButton
    {
        public GImage bg;
        public GImage load_icon;
        public GGraph gp_hand;
        public const string URL = "ui://pmf3wbjilbaaeu";

        public static btn_coin CreateInstance()
        {
            return (btn_coin)UIPackage.CreateObject("A000_common", "btn_coin");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (GImage)GetChildAt(0);
            load_icon = (GImage)GetChildAt(1);
            gp_hand = (GGraph)GetChildAt(3);
        }
    }
}