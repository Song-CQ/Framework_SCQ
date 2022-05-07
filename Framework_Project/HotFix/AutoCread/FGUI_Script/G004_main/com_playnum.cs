/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.G004_main
{
    public partial class com_playnum : GComponent
    {
        public Controller cont_cardleft;
        public GTextField text_num;
        public GTextField text_fx;
        public GTextField text_time;
        public GTextField text_free;
        public Transition fx_num;
        public const string URL = "ui://qw9x6rf3rbhoa";

        public static com_playnum CreateInstance()
        {
            return (com_playnum)UIPackage.CreateObject("G004_main", "com_playnum");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cont_cardleft = GetControllerAt(0);
            text_num = (GTextField)GetChildAt(1);
            text_fx = (GTextField)GetChildAt(2);
            text_time = (GTextField)GetChildAt(3);
            text_free = (GTextField)GetChildAt(4);
            fx_num = GetTransitionAt(0);
        }
    }
}