/****************************************************
    文件：PathConst.cs
	作者：Clear
    日期：2022/1/28 14:31:1
    类型: 框架核心脚本(请勿修改)
	功能：编辑器路劲常量
*****************************************************/
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class UnityEditorPathConst 
    {
        /// <summary>
        /// ModuleUI路劲
        /// </summary>      
        public readonly static string ModuleUIPath_Assets = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.ModuleUIPath);
        public readonly static string ModuleUIPath = ModuleUIPath_Assets.Replace(Application.dataPath,string.Empty);
        /// <summary>
        /// CommModuleUI路劲
        /// </summary>      
        public readonly static string CommonModuleUIPath_Assets = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.CommonModuleUIPath);
        public readonly static string CommonModuleUIPath = CommonModuleUIPath_Assets.Replace(Application.dataPath,string.Empty);

        /// <summary>
        /// 自动注册目录
        /// </summary>   
        public readonly static string AutoCreadPath_Assets = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.AutoCreadPath);
        public readonly static string AutoCreadPath = AutoCreadPath_Assets.Replace(Application.dataPath, string.Empty); 
        
        /// <summary>
        /// FGUI代码目录
        /// </summary>   
        public readonly static string FGUIClassPath_Assets = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.FGUIClassPath);
        public readonly static string FGUIClassPath = FGUIClassPath_Assets.Replace(Application.dataPath, string.Empty);
        /// <summary>
        /// 存放UI的目录
        /// </summary>   
        public readonly static string ResFGUIPath_Assets = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.ResFGUIPath);
        public readonly static string ResFGUIPath = ResFGUIPath_Assets.Replace(Application.dataPath, string.Empty);


        /// <summary>
        /// 项目解决方案路劲
        /// </summary>
        public readonly static string Project_Sin_Path = $"{Application.dataPath}/../{ProjectApp.AppFacade.AppName}_UClient.sln";


    }
}