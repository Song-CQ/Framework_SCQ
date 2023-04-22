/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI.G004_main
{
    public partial class com_main : GComponent
    {
        public GGraph gp_fx;
        public GButton settings;
        public GButton btn_play;
        public GButton btn_coin;
        public GButton btn_cash;
        public GButton btn_pp;
        public com_cardnum com_cardnum;
        public com_playnum com_cards;
        public com_cardplaytext com_cardplay;
        public GButton btn_task;
        public GButton btn_wheel;
        public GButton btn_sign;
        public GButton btn_ballon;
        public GButton btn_pig;
        public GButton btn_crazy;
        public GButton btn_iwatch;
        public GGroup gp_left;
        public GButton btn_gift;
        public RealItem btn_RealItem;
        public GGroup gp_right;
        public GList list_rollingList;
        public const string URL = "ui://qw9x6rf3lbaa0";

        public static com_main CreateInstance()
        {
            return (com_main)UIPackage.CreateObject("G004_main", "com_main");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            gp_fx = (GGraph)GetChildAt(2);
            settings = (GButton)GetChildAt(3);
            btn_play = (GButton)GetChildAt(4);
            btn_coin = (GButton)GetChildAt(5);
            btn_cash = (GButton)GetChildAt(6);
            btn_pp = (GButton)GetChildAt(7);
            com_cardnum = (com_cardnum)GetChildAt(8);
            com_cards = (com_playnum)GetChildAt(9);
            com_cardplay = (com_cardplaytext)GetChildAt(10);
            btn_task = (GButton)GetChildAt(11);
            btn_wheel = (GButton)GetChildAt(12);
            btn_sign = (GButton)GetChildAt(13);
            btn_ballon = (GButton)GetChildAt(14);
            btn_pig = (GButton)GetChildAt(15);
            btn_crazy = (GButton)GetChildAt(16);
            btn_iwatch = (GButton)GetChildAt(17);
            gp_left = (GGroup)GetChildAt(18);
            btn_gift = (GButton)GetChildAt(19);
            btn_RealItem = (RealItem)GetChildAt(20);
            gp_right = (GGroup)GetChildAt(21);
            list_rollingList = (GList)GetChildAt(22);
        }
    }
}