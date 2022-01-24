/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A000_common
{
    public partial class btn_crazy : GButton
    {
        public Controller cont_button;
        public GGraph fx_lizi;
        public GProgressBar pb_num;
        public GTextField text_time;
        public const string URL = "ui://pmf3wbjiboowu";

        public static btn_crazy CreateInstance()
        {
            return (btn_crazy)UIPackage.CreateObject("A000_common", "btn_crazy");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cont_button = GetControllerAt(0);
            fx_lizi = (GGraph)GetChildAt(2);
            pb_num = (GProgressBar)GetChildAt(3);
            text_time = (GTextField)GetChildAt(4);
        }
    }
}