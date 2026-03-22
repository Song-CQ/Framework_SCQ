using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExcelTool.Tool
{
    public static class MD5Util
    {

        /// <summary>
        /// 计算字节数组的 MD5
        /// </summary>
        public static string ComputeMD5(byte[] data)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(data);
                return BytesToHex(hashBytes);
            }
        }

        /// <summary>
        /// 计算文件的 MD5
        /// </summary>
        public static string ComputeFileMD5(string filePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    return BytesToHex(hashBytes);
                }
            }
        }

        /// <summary>
        /// 字节数组转十六进制字符串
        /// </summary>
        private static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
