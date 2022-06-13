/****************************************************
    文件: HotFixVerifyData.cs
    作者: Clear
    日期: 2022/6/8 18:8:32
    类型: 框架核心脚本(请勿修改)
    功能: 热更版本信息
*****************************************************/
namespace FutureCore.Data
{
    public class HotFixVerify
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public int version;
        /// <summary>
        /// 最新的打包时间
        /// </summary>
        public string buildDate;
        /// <summary>
        /// MD5码
        /// </summary>
        public string MD5;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long size;
    }
}