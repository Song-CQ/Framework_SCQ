/****************************************************
    文件: VerifyData.cs
    作者: Clear
    日期: 2022/5/31 17:44:52
    类型: 逻辑脚本
    功能: AB包自动打包
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore.Data
{
    /// <summary>
    /// 文件校验
    /// </summary>
    public class AssetBundleVerify
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
    
}