/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A000_common
{
    public partial class btn_balloon : GButton
    {
        public Transition fx_fly;
        public const string URL = "ui://pmf3wbjifczjfq";

        public static btn_balloon CreateInstance()
        {
            return (btn_balloon)UIPackage.CreateObject("A000_common", "btn_balloon");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            fx_fly = GetTransitionAt(0);
        }
    }
}