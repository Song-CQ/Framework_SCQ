/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A001_common
{
    public partial class btn_collect : GButton
    {
        public Controller cont_state;
        public const string URL = "ui://pmf3wbjib9hhfh";

        public static btn_collect CreateInstance()
        {
            return (btn_collect)UIPackage.CreateObject("A001_common", "btn_collect");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cont_state = GetControllerAt(0);
        }
    }
}