/****************************************************
    文件: VerifyData.cs
    作者: Clear
    日期: 2022/5/31 17:44:52
    类型: 逻辑脚本
    功能: AB包自动打包
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    /// <summary>
    /// 文件校验
    /// </summary>
    public class VerifyData
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public int version;
        /// <summary>
        /// 最新的打包时间
        /// </summary>
        public string builddate;
        /// <summary>
        /// 文件总表
        /// </summary>
        public List<FileMsg> filemap = new List<FileMsg>();
        /// <summary>
        /// 包总表
        /// </summary>
        public List<BundleMsg> bagmap = new List<BundleMsg>();
    }
    [Serializable]
    public class FileMsg
    {
        /// <summary>
        /// 文件路径(含尾缀)
        /// </summary>
        public string Path;
        public string MD5;
    }

    [Serializable]
    public class BundleMsg
    {
        public string bagname;
        public int num;
    }
    /// <summary>
    /// 依赖关系信息
    /// </summary>
    public class DependData
    {
        public List<DependMsg> depsmap = new List<DependMsg>();
    }

    [Serializable]
    public class DependMsg
    {
        public string selfbag;
        public List<string> depends = new List<string>();
    }

    /// <summary>
    /// 被依赖关系信息
    /// </summary>
    public class BeDependData
    {
        public List<BeDependMsg> bedepsmap = new List<BeDependMsg>();
    }

    [Serializable]
    public class BeDependMsg
    {
        public string selfbag;
        public List<string> bedepends = new List<string>();
    }
    public class ABConfig
    {
        /// <summary>
        /// 输出路径
        /// </summary>
        public static string outputpath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "AB";  //最后是主包名字
        /// <summary>
        /// 项目AB包文件夹
        /// </summary>
        public static string abroot = "ABres";
        /// <summary>
        /// 校验文件路径
        /// </summary>
        public static string verifypath = Application.dataPath + "/Editor/version.json";
        /// <summary>
        /// 全被依赖文件路径
        /// </summary>
        public static string allbedependpath = Application.dataPath + "/Editor/allbedependdata.json";
        /// <summary>
        /// 全依赖文件路径
        /// </summary>
        public static string alldependpath = Application.dataPath + "/Editor/alldependdata.json";
        /// <summary>
        /// 打包压缩设置
        /// </summary>
        public static BuildAssetBundleOptions ABOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        /// <summary>
        /// 打包平台
        /// </summary>
        public static BuildTarget ABPlatform = BuildTarget.StandaloneWindows;
    }
}