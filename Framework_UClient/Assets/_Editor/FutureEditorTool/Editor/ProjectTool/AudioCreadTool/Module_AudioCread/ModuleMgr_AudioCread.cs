/****************************************************
    文件：ModuleMgr_AudioCread.cs
	作者：Clear
    日期：2022/1/28 14:9:58
    类型: 框架核心脚本(请勿修改)
	功能：创建框架Module常量,自动注册ModuleMgr
*****************************************************/
using FutureCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FutureEditor
{
    public static class ModuleMgr_AudioCread
    {
        private static string ModuleUIPath = Application.dataPath + UnityEditorPathConst.ModuleUIPath;

        private const string ClassTemplate = @"/****************************************************
    文件：#ClassName#Const.cs
	作者：Clear
    日期：#CreateTime#
    类型: 框架自动创建(请勿修改)
    功能：#Introduce#
*****************************************************/
namespace ProjectApp
{
    public static class #ClassName#Const
    {
        #Val#
    }
}";
        private static string FieldTemplate = @"public const string #FieldName# = ""#FieldName#"";/n";

        public static void CheckModule()
        {
            string[] DirectoriePath = Directory.GetDirectories(ModuleUIPath);
            List<string> nameLst = new List<string>();
            foreach (string path in DirectoriePath)
            {
                nameLst.Add(path.Replace(ModuleUIPath + @"\", string.Empty));
            }          
            Debug.Log("[ModuleMgr_AudioCread]开始自动模块注册");
            CreadUIConst(nameLst);
            CreadCtrlConst(nameLst);
            CreadUICtrlConst(nameLst);
            CreadModelConst(nameLst);
            Debug.Log("[ModuleMgr_AudioCread]模块自动注册完成");
        }

        private static void CreadModelConst(List<string> nameLst)
        {
            string classStr = ClassTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            classStr = classStr.Replace("#ClassName#", "Model");
            classStr = classStr.Replace("#Introduce#", "模块常量");
            string val= string.Empty;
            foreach (var item in nameLst)
            {
                val += FieldTemplate.Replace("#FieldName#",item+ "Model");
            }
            classStr.Replace("#Val#",val);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/ModelConst_AudioCread", classStr, Encoding.UTF8);
            Debug.Log("[ModuleMgr_AudioCread]注册模块常量完成");
        }

        private static void CreadUICtrlConst(List<string> nameLst)
        {
            string classStr = ClassTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            classStr = classStr.Replace("#ClassName#", "UICtrl");
            classStr = classStr.Replace("#Introduce#", "UI控制器常量");
            string val = string.Empty;
            foreach (var item in nameLst)
            {
                val += FieldTemplate.Replace("#FieldName#", item + "UICtrl");
            }
            classStr.Replace("#Val#", val);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/UICtrlConst_AudioCread", classStr, Encoding.UTF8);
            Debug.Log("[ModuleMgr_AudioCread]注册UI控制器常量完成");
        }

        private static void CreadCtrlConst(List<string> nameLst)
        {
            string classStr = ClassTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            classStr = classStr.Replace("#ClassName#", "Ctrl");
            classStr = classStr.Replace("#Introduce#", "控制器常量");
            string val = string.Empty;
            foreach (var item in nameLst)
            {
                val += FieldTemplate.Replace("#FieldName#", item + "Ctrl");
            }
            classStr.Replace("#Val#", val);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/CtrlConst_AudioCread", classStr, Encoding.UTF8);
            Debug.Log("[ModuleMgr_AudioCread]注册控制器常量完成");
        }

        private static void CreadUIConst(List<string> nameLst)
        {
            string classStr = ClassTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            classStr = classStr.Replace("#ClassName#", "UI");
            classStr = classStr.Replace("#Introduce#", "UI常量");
            string val = string.Empty;
            foreach (var item in nameLst)
            {
                val += FieldTemplate.Replace("#FieldName#", item + "UI");
            }
            classStr.Replace("#Val#", val);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/UIConst_AudioCread", classStr, Encoding.UTF8);
            Debug.Log("[ModuleMgr_AudioCread]注册UI常量完成");
        }
    }
}