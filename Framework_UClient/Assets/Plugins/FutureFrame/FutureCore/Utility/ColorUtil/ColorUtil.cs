/****************************************************************************
* ScriptType: 主框架
* 请勿修改!!!
****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    public enum ColorType
    {
        White = 0,
        Yellow = 1,
        Green = 2,
        Red = 3,
        Blue = 4,
        深粉,
        浅粉,
        亮绿,
        浅绿,
        洋红,
        红色,
        橙色,
        橙红,
        亮黄色,
        浅黄,
        紫色,
        淡蓝,
        青色,
        淡青,
        深天蓝,
        荧光绿,
        绿黄,
        灰色,
    }

    public static class ColorUtil
    {
        private static Dictionary<ColorType, string> thisColorTypeDic;
        private static Dictionary<ColorType, string> ColorTypeDic {
            get
            {
                if (thisColorTypeDic==null) 
                {
                    thisColorTypeDic= new Dictionary<ColorType, string>();
                    Init();
                };
                return thisColorTypeDic;
            }
        }
        private static void Init()
        {
            thisColorTypeDic.Add(ColorType.White, "FFFFFF");
            thisColorTypeDic.Add(ColorType.Yellow, "F5FF09");
            thisColorTypeDic.Add(ColorType.Green, "2EED5F");
            thisColorTypeDic.Add(ColorType.Red, "E30A0A");
            thisColorTypeDic.Add(ColorType.Blue, "19B3FF");
            thisColorTypeDic.Add(ColorType.深粉, "ff4690");
            thisColorTypeDic.Add(ColorType.浅粉, "ffb2c4");
            thisColorTypeDic.Add(ColorType.亮绿, "33ff38");
            thisColorTypeDic.Add(ColorType.浅绿, "9fffa8");
            thisColorTypeDic.Add(ColorType.洋红, "fd26fa");
            thisColorTypeDic.Add(ColorType.红色, "ff2850");
            thisColorTypeDic.Add(ColorType.橙色, "ff701a");
            thisColorTypeDic.Add(ColorType.橙红, "fb3711");
            thisColorTypeDic.Add(ColorType.亮黄色, "fae910");
            thisColorTypeDic.Add(ColorType.浅黄, "fff89c");
            thisColorTypeDic.Add(ColorType.紫色, "c954ff");
            thisColorTypeDic.Add(ColorType.淡蓝, "a5adff");
            thisColorTypeDic.Add(ColorType.青色, "2df3ff");
            thisColorTypeDic.Add(ColorType.淡青, "c2fff0");
            thisColorTypeDic.Add(ColorType.深天蓝, "0888ff");
            thisColorTypeDic.Add(ColorType.荧光绿, "08ffc2");
            thisColorTypeDic.Add(ColorType.绿黄, "c1ff19");
            thisColorTypeDic.Add(ColorType.灰色, "bdbdbd");
        }

        public static void HtmlParseColor(string htmlStr, out Color color)
        {
            bool result = ColorUtility.TryParseHtmlString(htmlStr, out color);
            if (!result)
            {
                LogUtil.LogError($"[ColorConst]Parse Html String Fairly: {htmlStr}");
            }
        }

        public static string ColorTypeToHtml(ColorType colorType)
        {
            
            if (!ColorTypeDic.TryGetValue(colorType, out string val))
            {
                val = "FFFFFF";
            }
            return val;
        }

        /// <summary>
        /// Color转换Hex
        /// </summary>
        public static string ColorToHex(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255.0f);
            int g = Mathf.RoundToInt(color.g * 255.0f);
            int b = Mathf.RoundToInt(color.b * 255.0f);
            int a = Mathf.RoundToInt(color.a * 255.0f);
            string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
            return hex;
        }

        /// <summary>
        /// Hex转换到Color
        /// </summary>
        public static Color HexToColor(string hex)
        {
            byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            float a = cc / 255f;
            return new Color(r, g, b, a);
        }



    }
    

}