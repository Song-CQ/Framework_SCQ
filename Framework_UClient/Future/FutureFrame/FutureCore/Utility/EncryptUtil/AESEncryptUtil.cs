using System;
using System.Security.Cryptography;
using System.Text;

namespace FutureCore
{
    public static class AESEncryptUtil
    {
        /// <summary>
        /// 运算字节矩阵
        /// </summary>
        private const int byteMatrixSize = 4 * 4;
        /// <summary>
        /// 补零
        /// </summary>
        private const string zeroString = "\0";

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="val">值</param>
        /// <param name="key">键</param>
        /// <param name="iVector">向量</param>
        /// <returns></returns>
        public static byte[] Encrypt(string val,string key,string iVector)
        {
            int totalLen = val.Length;
            int maxLength = (int)Math.Ceiling((double)(totalLen / byteMatrixSize)) * byteMatrixSize;
            for (int i = totalLen; i < maxLength; i++)
            {
                val += zeroString;
            }
            
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(val);
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] ivArray = Encoding.UTF8.GetBytes(iVector);
            
            RijndaelManaged aes = new RijndaelManaged();
            aes.Key = keyArray;
            aes.IV = ivArray;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.Zeros;

            ICryptoTransform cryptoTransform = aes.CreateEncryptor();
            byte[] resultArray = cryptoTransform.TransformFinalBlock(toEncryptArray,0,toEncryptArray.Length);
            return resultArray;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="val">值</param>
        /// <param name="key">键</param>
        /// <param name="iVector">向量</param>
        /// <returns></returns>
        public static string Decrypt(byte[] bytes,string key,string iVector)
        {
            byte[] toEncryptArray = bytes;
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] ivArray = Encoding.UTF8.GetBytes(iVector);
            
            RijndaelManaged aes = new RijndaelManaged();
            aes.Key = keyArray;
            aes.IV = ivArray;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.Zeros;

            ICryptoTransform cryptoTransform = aes.CreateDecryptor();
            byte[] resultArray = cryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            string val = Encoding.UTF8.GetString(resultArray);

            int zeroIndex = val.IndexOf(zeroString);
            if (zeroIndex > 0)
            {
                val = val.Substring(0,zeroIndex);
            }
            return val;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte[] Encrypt(string val)
        {
            return Encrypt(val,EncryptConst.AES_Key, EncryptConst.AES_IVector);
        }
        
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Decrypt(byte[] bytes)
        {
            return Decrypt(bytes,EncryptConst.AES_Key, EncryptConst.AES_IVector);
        }


    }
    
}


