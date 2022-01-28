/****************************************************
    文件：PathConst.cs
	作者：Clear
    日期：2022/1/28 14:31:1
    类型: 框架核心脚本(请勿修改)
	功能：编辑器路劲常量
*****************************************************/
using UnityEngine;

namespace FutureCore
{
    public static class UnityEditorPathConst 
    {
        /// <summary>
        /// ModuleUI路劲
        /// </summary>
        public readonly static string ModuleUIPath =  @"_App\ProjectApp\Logic\ModuleUI";      
        public readonly static string ModuleUIPath_Assets = Application.dataPath +@"\"+ ModuleUIPath;

        /// <summary>
        /// 自动注册目录
        /// </summary>   
        public readonly static string AudioCreadPath =  @"_App\AutoCreator\AutoRegister";
        public readonly static string AudioCreadPath_Assets = Application.dataPath + @"\" + AudioCreadPath;



    }
}