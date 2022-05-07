/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.G004_main
{
    public partial class com_rolling : GComponent
    {
        public GTextField text_rolling;
        public const string URL = "ui://qw9x6rf3t4tdr";

        public static com_rolling CreateInstance()
        {
            return (com_rolling)UIPackage.CreateObject("G004_main", "com_rolling");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            text_rolling = (GTextField)GetChildAt(0);
        }
    }
}