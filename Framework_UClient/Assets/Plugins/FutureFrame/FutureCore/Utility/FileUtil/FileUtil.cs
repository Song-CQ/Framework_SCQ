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

        public static void WriteAllText(string path, string val)
        {
            File.WriteAllText(path, val + "", Encoding.UTF8);


        }



    }
}