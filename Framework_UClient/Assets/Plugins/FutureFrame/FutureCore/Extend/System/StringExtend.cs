using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FutureCore
{
    
    public static class StringExtend
    {
        #region ColorText
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
        public static string AddColor(this string str, ColorType color)
        {
            string _str = str;
            _str = "<Color=" + TypeToString(color) + ">" + _str + "</Color>";
            return _str;
        }

        public static string AddColor(this string str, int colorid)
        {
            return AddColor(str, (ColorType) colorid);
        }
        
        public static string AddSize(this string str, int size)
        {
           return  "<Size=" +size+">"+str + "</Size>";
        }
        public static string TypeToString(ColorType Type)
        {
            string _str = string.Empty;
            switch (Type)
            {
                case ColorType.White:
                    _str = "#FFFFFF";
                    break;
                case ColorType.Yellow:
                    _str = "#F5FF09";
                    break;
                case ColorType.Green:
                    _str = "#2EED5F";
                    break;
                case ColorType.Red:
                    _str = "#E30A0A";
                    break;
                case ColorType.Blue:
                    _str = "#19B3FF";
                    break;
                case ColorType.深粉:
                    _str = "#ff4690";
                    break;
                case ColorType.浅粉:
                    _str = "#ffb2c4";
                    break;
                case ColorType.亮绿:
                    _str = "#33ff38";
                    break;
                case ColorType.浅绿:
                    _str = "#9fffa8";
                    break;
                case ColorType.洋红:
                    _str = "#fd26fa";
                    break;
                case ColorType.红色:
                    _str = "#ff2850";
                    break;
                case ColorType.橙色:
                    _str = "#ff701a";
                    break;
                case ColorType.橙红:
                    _str = "#fb3711";
                    break;
                case ColorType.亮黄色:
                    _str = "#fae910";
                    break;
                case ColorType.浅黄:
                    _str = "#fff89c";
                    break;
                case ColorType.紫色:
                    _str = "#c954ff";
                    break;
                case ColorType.淡蓝:
                    _str = "#a5adff";
                    break;
                case ColorType.青色:
                    _str = "#2df3ff";
                    break;
                case ColorType.淡青:
                    _str = "#c2fff0";
                    break;
                case ColorType.深天蓝:
                    _str = "#0888ff";
                    break;
                case ColorType.荧光绿:
                    _str = "#08ffc2";
                    break;
                case ColorType.绿黄:
                    _str = "#c1ff19";
                    break;
                case ColorType.灰色:
                    _str = "#bdbdbd";
                    break;
            }

            return _str;
        }

        #endregion
    }
}

