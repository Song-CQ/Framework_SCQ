/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.A002_loading
{
    public partial class com_loading : GComponent
    {
        public GProgressBar pb_loading;
        public GTextField text_severStatus;
        public const string URL = "ui://pc0wa25bt268a";

        public static com_loading CreateInstance()
        {
            return (com_loading)UIPackage.CreateObject("A002_loading", "com_loading");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            pb_loading = (GProgressBar)GetChildAt(1);
            text_severStatus = (GTextField)GetChildAt(2);
        }
    }
}