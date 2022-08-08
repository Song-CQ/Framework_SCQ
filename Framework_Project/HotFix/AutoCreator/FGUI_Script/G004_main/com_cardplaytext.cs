/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.G004_main
{
    public partial class com_cardplaytext : GComponent
    {
        public Controller cont_text;
        public GTextField text_cardplay;
        public const string URL = "ui://qw9x6rf3rps6c";

        public static com_cardplaytext CreateInstance()
        {
            return (com_cardplaytext)UIPackage.CreateObject("G004_main", "com_cardplaytext");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cont_text = GetControllerAt(0);
            text_cardplay = (GTextField)GetChildAt(0);
        }
    }
}