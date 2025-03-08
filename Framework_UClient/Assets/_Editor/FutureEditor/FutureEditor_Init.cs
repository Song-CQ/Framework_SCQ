/****************************************************
    文件: FutureEditor_Init.cs
    作者: Clear
    日期: 2023/5/23 16:33:34
    类型: 编辑器脚本
    功能: 编辑器数据初始化
*****************************************************/
using FutureCore;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class FutureEditor_Init
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            InitAB();
        }

        private static void InitAB()
        {
            string abConfigPath = UnityEditorPathConst.ABConfigPatn_Assest + "/ABConfig.asset";
            ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
            if (abConfig != null)
            {
                PathConst.EditorAssetBundlesPath = Path.GetFullPath(abConfig.outputPath.Replace(@"Assets\..\..\",Application.dataPath+ @"\..\..\"))  + @"\StandaloneWindows";
                if (!Directory.Exists(PathConst.EditorAssetBundlesPath))
                {
                    LogUtil.LogError("AB包输出目录不存在! ----  "+ PathConst.EditorAssetBundlesPath);
                    LogUtil.LogError("请重新选择AB包输出目录!");
                }
            }

        }



    }
}