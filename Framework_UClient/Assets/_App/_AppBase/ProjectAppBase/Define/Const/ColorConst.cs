using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public static partial class ColorConst
    {
        public static Color AllWhite;
        public static Color HalfWhite;
        public static Color AllBlack;

        public static Color Red;
        public static Color Green;
        public static Color Blue;

        public const string AllWhiteStr = "#FFFFFFFF";
        public const string HalfWhiteStr = "#FFFFFF80";
        public const string AllBlackStr = "#000000FF";

        public const string RedStr = "#ff4348";
        public const string GreenStr = "#7dfc46";
        public const string BlueStr = "#27bbe5FF";

        public static void Init()
        {
            ColorUtil.HtmlParseColor(AllWhiteStr, out AllWhite);
            ColorUtil.HtmlParseColor(HalfWhiteStr, out HalfWhite);
            ColorUtil.HtmlParseColor(AllBlackStr, out AllBlack);

            ColorUtil.HtmlParseColor(RedStr, out Red);
            ColorUtil.HtmlParseColor(GreenStr, out Green);
            ColorUtil.HtmlParseColor(BlueStr, out Blue);
        }
    }
}

