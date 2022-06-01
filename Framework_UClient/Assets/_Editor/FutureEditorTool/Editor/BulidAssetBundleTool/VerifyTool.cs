/****************************************************
    文件: VerifyTool.cs
    作者: Clear
    日期: 2022/5/31 17:47:29
    类型: 逻辑脚本
    功能: AB包自动打包 校验工具
*****************************************************/
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace FutureEditor
{
    public static class VerifyTool 
    {
        private static MD5 md5 = new MD5CryptoServiceProvider();
        private static StringBuilder sb = new StringBuilder();

        /// <summary>
        /// 获取文件的MD5码
        /// </summary>
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>
        /// <returns></returns>
        public static string GetFileMD5(string fileName)
        {
            try
            {
                sb.Clear();
                FileStream file = new FileStream(fileName, FileMode.Open);
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));  //x表示16进制 2表示显示两位，即 0x1A
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("获取MD5出错:" + ex.Message);
            }
        }
        /// <summary>
        /// 比较指定文件的MD5值与目标值是否相等,并返回文件当前md5
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="Template"></param>
        /// <param name="curmd5">文件当前的md5</param>
        /// <returns>true 没修改  false 修改</returns>
        public static bool Compare(string fileName, StringBuilder Template)
        {
            string newmd5 = GetFileMD5(fileName);
            if (newmd5.Equals(Template.ToString()))
            {
                Template.Clear();
                Template.Append(newmd5);
                return true;
            }
            Template.Clear();
            Template.Append(newmd5);
            return false;
        }
        /// <summary>
        /// 比较指定文件的MD5值与目标值是否相等
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="Template"></param>
        /// <returns></returns>
        public static bool Compare(string fileName, string Template)
        {
            string newmd5 = GetFileMD5(fileName);
            if (newmd5.Equals(Template))
            {
                return true;
            }
            return false;
        }
    
    }
}