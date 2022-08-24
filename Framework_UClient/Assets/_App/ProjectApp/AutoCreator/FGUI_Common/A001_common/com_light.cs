/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A001_common
{
    public partial class com_light : GComponent
    {
        public Transition fx_light;
        public const string URL = "ui://pmf3wbjicjp2l";

        public static com_light CreateInstance()
        {
            return (com_light)UIPackage.CreateObject("A001_common", "com_light");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            fx_light = GetTransitionAt(0);
        }
    }
}