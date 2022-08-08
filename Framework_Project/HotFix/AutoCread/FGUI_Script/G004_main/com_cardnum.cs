/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.G004_main
{
    public partial class com_cardnum : GComponent
    {
        public Controller cont_state;
        public btn_add btn_add;
        public btn_minus btn_minus;
        public const string URL = "ui://qw9x6rf3t4cu9";

        public static com_cardnum CreateInstance()
        {
            return (com_cardnum)UIPackage.CreateObject("G004_main", "com_cardnum");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cont_state = GetControllerAt(0);
            btn_add = (btn_add)GetChildAt(1);
            btn_minus = (btn_minus)GetChildAt(2);
        }
    }
}