/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A000_common
{
    public partial class btn_green_vid : GButton
    {
        public Controller cont_button;
        public const string URL = "ui://pmf3wbjia9pgfg";

        public static btn_green_vid CreateInstance()
        {
            return (btn_green_vid)UIPackage.CreateObject("A000_common", "btn_green_vid");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cont_button = GetControllerAt(0);
        }
    }
}