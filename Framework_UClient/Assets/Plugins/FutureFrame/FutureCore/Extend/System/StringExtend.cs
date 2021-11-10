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
        
        public static string AddColor(this string str, ColorType color)
        {
            return AddColor(str,ColorUtil.ColorTypeToHtml(color));
        }

        public static string AddColor(this string str, string Hex)
        {
            if (Hex.Length < 6)
            {
                LogUtil.LogError("Html不正确");
                return str;
            }
            else if (Hex.Length > 6)
            {
                Hex = Hex.Substring(0, 6);
            }
            string _str = str;
            _str = "<Color=#" + Hex + ">" + _str + "</Color>";
            return _str;
        }
        public static string AddColor(this string str, Color color)
        {
            return AddColor(str, ColorUtility.ToHtmlStringRGB(color));
        }

        public static string AddSize(this string str, int size)
        {
           return  "<Size=" +size+">"+str + "</Size>";
        }
        #endregion
    }
}

