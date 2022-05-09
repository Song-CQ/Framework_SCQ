/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.G004_main
{
    public partial class btn_add : GButton
    {
        public Controller cont_button;
        public const string URL = "ui://qw9x6rf3t4cu7";

        public static btn_add CreateInstance()
        {
            return (btn_add)UIPackage.CreateObject("G004_main", "btn_add");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cont_button = GetControllerAt(0);
        }
    }
}