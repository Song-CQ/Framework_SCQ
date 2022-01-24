/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A000_common
{
    public partial class btn_cash : GButton
    {
        public GImage load_icon;
        public GGraph gp_hand;
        public const string URL = "ui://pmf3wbjilbaa8";

        public static btn_cash CreateInstance()
        {
            return (btn_cash)UIPackage.CreateObject("A000_common", "btn_cash");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            load_icon = (GImage)GetChildAt(1);
            gp_hand = (GGraph)GetChildAt(3);
        }
    }
}