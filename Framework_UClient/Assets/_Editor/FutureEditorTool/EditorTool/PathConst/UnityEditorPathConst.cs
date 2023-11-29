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

    [InitializeOnLoad]
    public static class UnityEditorPathConst 
    {
        /// <summary>
        /// ModuleUI路劲
        /// </summary>      
        public readonly static string ModuleUIPath = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.ModuleUIPath);

        /// <summary>
        /// ModuleUI路劲(热更)
        /// </summary>
        public readonly static string ModuleUIPath_HotFix = Application.dataPath + "../../Framework_Project/HotFix/Model";
        /// <summary>
        /// CommModuleUI路劲
        /// </summary>      
        public readonly static string CommonModuleUIPath = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.CommonModuleUIPath);

        /// <summary>
        /// 自动注册目录
        /// </summary>   
        public readonly static string AutoRegisterPath = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.AutoRegisterPath);

        /// <summary>
        /// 自动注册目录(热更)
        /// </summary>
        public readonly static string AutoRegisterPath_HotFix = $"{Application.dataPath}/../../Framework_Project/HotFix/AutoRegister";
        /// <summary>
        /// Unity 存放FGUI代码目录
        /// </summary>   
        public readonly static string FGUIClassPath = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.FGUIClassPath);         
        /// <summary>
        /// 热更 存放FGUI代码目录
        /// </summary>   
        public readonly static string FGUIClassPath_HotFix = $"{Application.dataPath}/../../Framework_Project/HotFix/AutoCreator/FGUI_Script";

        /// <summary>
        /// 存放FGUI的目录
        /// </summary>   
        public readonly static string ResFGUIPath = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.ResFGUIPath);
        /// <summary>
        /// 存放UGUI预制体的目录
        /// </summary>  
        public readonly static string ResUGUIPath = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.ResUGUIPath);
        
        /// <summary>
        /// 存放ABConfig(打包Ab包设置)的路劲 
        /// </summary>
        public readonly static string ABConfigPatn_Assest = AssetDatabase.GetAssetPath(UnityEditorPath.Instance.ABConfigPath);


        /// <summary>
        /// 项目解决方案路劲
        /// </summary>
        public readonly static string Project_Sin_Path = $"{Application.dataPath}/../{ProjectApp.AppFacade.AppName}_UClient.sln";


        /// <summary>
        /// Fgui编辑器脚本的生成路劲
        /// </summary>
        public readonly static string Fgui_Stript_Patn = $"{Application.dataPath}/../../_Resources/FGUI/FGUI_Script";

        


    }
}