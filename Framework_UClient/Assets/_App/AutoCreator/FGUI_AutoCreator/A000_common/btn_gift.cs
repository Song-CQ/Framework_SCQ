/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A000_common
{
    public partial class btn_gift : GButton
    {
        public GLoader loader_gift;
        public Transition fx_anui;
        public const string URL = "ui://pmf3wbjiboowv";

        public static btn_gift CreateInstance()
        {
            return (btn_gift)UIPackage.CreateObject("A000_common", "btn_gift");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            loader_gift = (GLoader)GetChildAt(1);
            fx_anui = GetTransitionAt(0);
        }
    }
}