/****************************************************
    文件：FileUtil.cs
	作者：Clear
    日期：2022/2/3 18:34:13
    类型: 框架核心脚本(请勿修改)
	功能：文件Util
*****************************************************/
using System.IO;
using System.Text;

namespace FutureCore
{
    public static class FileUtil 
    {

        public static void WriteFile(string targetPath, string classStr)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
            if (!directoryInfo.Parent.Exists)
            {
                Directory.CreateDirectory(directoryInfo.Parent.FullName);
            }
            File.WriteAllText(targetPath, classStr, new UTF8Encoding(false));
        }

        public static bool DeleteFileOrDirectory(string path)
        {  
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
                return true;
            }
            return false;
        }

    }
}