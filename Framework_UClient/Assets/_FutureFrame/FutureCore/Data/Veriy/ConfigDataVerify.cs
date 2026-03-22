/****************************************************
    文件: ConfigDataVerify.cs
    作者: Clear
    日期: 2026/3/21 1:51:7
    类型: 框架核心脚本(请勿修改)
    功能: 配置表版本信息
*****************************************************/
using System;
using System.Collections.Generic;

namespace FutureCore
{

    [Serializable]
    public class ConfigDataVerify 
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public uint version;
        /// <summary>
        /// 是否加密数据
        /// </summary>
        public bool isEnciphermentData = false;
        /// <summary>
        /// 是否每张表生成单独的数据包
        /// </summary>
        public bool isOutMultipleDatas = false;

        /// <summary>
        /// 数据包
        /// </summary>
        public List<ConfigData_FileMsg> files;


        [Serializable]
        public class ConfigData_FileMsg
        {
            /// <summary>
            /// 文件路径(含尾缀)
            /// </summary>
            public string Path;
            public string MD5;
            public int Size;
        }

    }

    
}