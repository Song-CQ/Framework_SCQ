/****************************************************
    文件: StringCo.cs
    作者: Clear
    日期: 2022/6/8 18:31:43
    类型: 框架核心脚本(请勿修改)
    功能: 字符串转换
*****************************************************/
using System;

namespace FutureCore
{
    public class StringConvertUnit
    {

        /// <summary>
        /// 将⽂件⼤⼩(字节)转换为最适合的显⽰⽅式
        /// </summary>
        /// <param name="size">⽂件字节</param>
        /// <returns>返回转换后的字符串</returns>
        public static string ConvertFileSize(long size)
        {
            string result = "0KB";
            int filelength = size.ToString().Length;
            if (size == 0)
                return result;
            else if (filelength < 4)
                result = size + "byte";
            else if (filelength < 7)
                result = Math.Round(Convert.ToDouble(size / 1024d), 2) + "KB";
            else if (filelength < 10)
                result = Math.Round(Convert.ToDouble(size / 1024d / 1024), 2) + "MB";
            else if (filelength < 13)
                result = Math.Round(Convert.ToDouble(size / 1024d / 1024 / 1024), 2) + "GB";
            else
                result = Math.Round(Convert.ToDouble(size / 1024d / 1024 / 1024 / 1024), 2) + "TB";
            return result;
        }





    }
}