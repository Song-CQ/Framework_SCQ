/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.G004_main
{
    public partial class btn_minus : GButton
    {
        public Controller cont_button;
        public const string URL = "ui://qw9x6rf3t4cu8";

        public static btn_minus CreateInstance()
        {
            return (btn_minus)UIPackage.CreateObject("G004_main", "btn_minus");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cont_button = GetControllerAt(0);
        }
    }
}