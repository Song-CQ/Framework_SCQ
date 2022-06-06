/****************************************************
    文件: ABConfig.cs
    作者: Clear
    日期: 2022/6/1 12:2:51
    类型: 逻辑脚本
    功能: 打包设置
*****************************************************/
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public class ABConfig : ScriptableObject
    {
        /// <summary>
        /// 输出路径
        /// </summary>
        public string outputPath;  //最后是主包名字
        /// <summary>
        /// 项目AB包文件夹
        /// </summary>
        public string abRoot;
        /// <summary>
        /// 校验文件路径
        /// </summary>
        public string verifyPath;
        /// <summary>
        /// 全被依赖文件路径
        /// </summary>
        public string allBeDependPath;
        /// <summary>
        /// 全依赖文件路径
        /// </summary>
        public string allDependPath;
        /// <summary>
        /// 打包压缩设置
        /// </summary>
        public BuildAssetBundleOptions abOptions;
        /// <summary>
        /// 打包平台
        /// </summary>
        public BuildTarget abPlatform;

        /// <summary>
        /// 是否拷贝至StreamingAssets
        /// </summary>
        public bool isCopyStreamingAssets;
        /// <summary>
        /// 是否导出version文件
        /// </summary>
        public bool isImputVersion;
        /// <summary>
        /// 是否增量打包
        /// </summary>
        public bool isIncrementalBulie;
        /// <summary>
        /// 是否自动增加版本号
        /// </summary>
        public bool isAutoAddVersion;
    }
}