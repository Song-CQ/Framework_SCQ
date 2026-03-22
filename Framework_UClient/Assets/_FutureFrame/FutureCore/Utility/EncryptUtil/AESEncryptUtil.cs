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
        /// 十六进制字符串转字节数组
        /// </summary>
        private static byte[] HexStringToByteArray(string hex)
        {
            // 移除空格
            hex = hex.Trim();

            // 检查长度是否为偶数
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException($"十六进制字符串长度必须是偶数！当前长度: {hex.Length}");
            }

            int len = hex.Length;
            byte[] bytes = new byte[len / 2];
            for (int i = 0; i < len; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="val">值</param>
        /// <param name="keyHex">十六进制格式的密钥</param>
        /// <param name="ivHex">十六进制格式的向量</param>
        /// <returns></returns>
        public static byte[] Encrypt(string val, string keyHex, string ivHex)
        {
            int totalLen = val.Length;
            int maxLength = (int)Math.Ceiling((double)(totalLen / byteMatrixSize)) * byteMatrixSize;
            for (int i = totalLen; i < maxLength; i++)
            {
                val += zeroString;
            }

            byte[] toEncryptArray = Encoding.UTF8.GetBytes(val);

            // ✅ 修正：将十六进制字符串转换为字节数组
            byte[] keyArray = HexStringToByteArray(keyHex);
            byte[] ivArray = HexStringToByteArray(ivHex);

            // 验证长度
            if (ivArray.Length != 16)
            {
                throw new CryptographicException($"IV长度错误: {ivArray.Length} 字节，需要 16 字节");
            }

            if (keyArray.Length != 16 && keyArray.Length != 32)
            {
                throw new CryptographicException($"Key长度错误: {keyArray.Length} 字节，需要 16 或 32 字节");
            }

            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Key = keyArray;
                aes.IV = ivArray;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.Zeros;

                ICryptoTransform cryptoTransform = aes.CreateEncryptor();
                return cryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="bytes">加密的字节数组</param>
        /// <param name="keyHex">十六进制格式的密钥</param>
        /// <param name="ivHex">十六进制格式的向量</param>
        /// <returns></returns>
        public static string Decrypt(byte[] bytes, string keyHex, string ivHex)
        {
            // ✅ 修正：将十六进制字符串转换为字节数组
            byte[] keyArray = HexStringToByteArray(keyHex);
            byte[] ivArray = HexStringToByteArray(ivHex);

            // 验证长度
            if (ivArray.Length != 16)
            {
                throw new CryptographicException($"IV长度错误: {ivArray.Length} 字节，需要 16 字节");
            }

            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Key = keyArray;
                aes.IV = ivArray;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.Zeros;

                ICryptoTransform cryptoTransform = aes.CreateDecryptor();
                byte[] resultArray = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                string val = Encoding.UTF8.GetString(resultArray);

                // 移除填充的零字符
                int zeroIndex = val.IndexOf(zeroString);
                if (zeroIndex >= 0)
                {
                    val = val.Substring(0, zeroIndex);
                }

                return val;
            }
        }

        /// <summary>
        /// AES加密（使用默认配置）
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte[] Encrypt(string val)
        {
            return Encrypt(val, EncryptConst.AES_Key, EncryptConst.AES_IVector);
        }

        /// <summary>
        /// AES解密（使用默认配置）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Decrypt(byte[] bytes)
        {
            return Decrypt(bytes, EncryptConst.AES_Key, EncryptConst.AES_IVector);
        }
    }
}


