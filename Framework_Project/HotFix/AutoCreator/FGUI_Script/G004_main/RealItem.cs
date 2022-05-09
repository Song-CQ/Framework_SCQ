/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.G004_main
{
    public partial class RealItem : GButton
    {
        public GLoader loader_switch;
        public Transition fx_Show;
        public const string URL = "ui://qw9x6rf3pbyeu";

        public static RealItem CreateInstance()
        {
            return (RealItem)UIPackage.CreateObject("G004_main", "RealItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            loader_switch = (GLoader)GetChildAt(1);
            fx_Show = GetTransitionAt(0);
        }
    }
}