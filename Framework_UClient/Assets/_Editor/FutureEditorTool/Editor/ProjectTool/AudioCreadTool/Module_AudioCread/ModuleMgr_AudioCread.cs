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
        private static string ModuleUIPath = Application.dataPath+@"\" + UnityEditorPathConst.ModuleUIPath;

        private const string ConstClassTemplate = @"/****************************************************
    文件: #ClassName#Const.cs
    作者: Clear
    日期: #CreateTime#
    类型: 框架自动创建(请勿修改)
    功能: #Introduce#
*****************************************************/
namespace ProjectApp
{
    public static class #ClassName#Const
    {
#Val#
    }   
}";
        private static string FieldTemplate = @"        public const string #FieldName# = ""#FieldName#"";";


        private const string ModuleMgrRegisterTemplate = @"/****************************************************
    文件: ModuleMgrRegister.cs
    作者: Clear
    日期: #CreateTime#
    类型: 自动创建
    功能: 模块数据注册
*****************************************************/
using FutureCore;
using UnityEngine;

namespace ProjectApp
{
    public static class ModuleMgrRegister 
    {
        public static void AutoRegisterModel()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
#ModelVal#
        }

        public static void AutoRegisterUIType()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
#UIVal#
        }

        public static void AutoRegisterCtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
#CtrlVal#
        }

        public static void AutoRegisterUICtrl()
        {
            ModuleMgr moduleMgr = ModuleMgr.Instance;
#UICtrlVal#
        }

    }
}";

        private static string UIFieldTemplate = @"            moduleMgr.AddUIType(UIConst.#FieldName#UI,typeof(#FieldName#UI));";
        private static string UICtrlFieldTemplate = @"            moduleMgr.AddUICtrl(UICtrlConst.#FieldName#UICtrl,new #FieldName#UICtrl());";
        private static string CtrlFieldTemplate = @"            moduleMgr.AddCtrl(CtrlConst.#FieldName#Ctrl,new #FieldName#Ctrl());";
        private static string ModelFieldTemplate = @"            moduleMgr.AddModel(ModelConst.#FieldName#Model,new #FieldName#Model());";

        private static string UICtrlMsgClassTemplate = @"/****************************************************
    文件: UICtrlMsg_OpenClose.cs
	作者: Clear
    日期: #CreateTime#
    类型: 框架自动创建(请勿修改)
	功能: UI打开关闭消息
*****************************************************/
namespace FutureCore
{
    public static partial class UICtrlMsg
    {
        private static uint cursor_UIOpenClose = 110000;

#Val#

    }
}";
        private static string UICtrlMsgFieldTemplate = "        public static uint #FieldName#UI_Open = ++cursor_UIOpenClose;" + "\r\n"+
        "        public static uint #FieldName#UI_Close = ++cursor_UIOpenClose;";


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
            CreadModuleMgrRegister(nameLst);
            CreadUICtrlMsgConst(nameLst);
            Debug.Log("[ModuleMgr_AudioCread]模块自动注册完成");
        }       
        private static void CreadModelConst(List<string> nameLst)
        {
            string classStr = ConstClassTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            classStr = classStr.Replace("#ClassName#", "Model");
            classStr = classStr.Replace("#Introduce#", "模块常量");
            string val= string.Empty;
            foreach (var item in nameLst)
            {
                val += FieldTemplate.Replace("#FieldName#",item+ "Model") + "\r\n";
            }
            classStr = classStr.Replace("#Val#",val);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/ModelConst_AudioCread.cs", classStr, Encoding.UTF8);
            Debug.Log("[ModuleMgr_AudioCread]注册模块常量完成");
        }

        private static void CreadUICtrlConst(List<string> nameLst)
        {
            string classStr = ConstClassTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            classStr = classStr.Replace("#ClassName#", "UICtrl");
            classStr = classStr.Replace("#Introduce#", "UI控制器常量");
            string val = string.Empty;
            foreach (var item in nameLst)
            {
                val += FieldTemplate.Replace("#FieldName#", item + "UICtrl") + "\r\n";
            }
            classStr = classStr.Replace("#Val#", val);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/UICtrlConst_AudioCread.cs", classStr, Encoding.UTF8);
            Debug.Log("[ModuleMgr_AudioCread]注册UI控制器常量完成");
        }

        private static void CreadCtrlConst(List<string> nameLst)
        {
            string classStr = ConstClassTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            classStr = classStr.Replace("#ClassName#", "Ctrl");
            classStr = classStr.Replace("#Introduce#", "控制器常量");
            string val = string.Empty;
            foreach (var item in nameLst)
            {
                val += FieldTemplate.Replace("#FieldName#", item + "Ctrl") + "\r\n";
            }
            classStr = classStr.Replace("#Val#", val);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/CtrlConst_AudioCread.cs", classStr, Encoding.UTF8);
            Debug.Log("[ModuleMgr_AudioCread]注册控制器常量完成");
        }

        private static void CreadUIConst(List<string> nameLst)
        {
            string classStr = ConstClassTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            classStr = classStr.Replace("#ClassName#", "UI");
            classStr = classStr.Replace("#Introduce#", "UI常量");
            string val = string.Empty;
            foreach (var item in nameLst)
            {
                val += FieldTemplate.Replace("#FieldName#", item + "UI") + "\r\n";
            }
            classStr = classStr.Replace("#Val#", val);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/UIConst_AudioCread.cs", classStr, Encoding.UTF8);
            Debug.Log("[ModuleMgr_AudioCread]注册UI常量完成");
        }
        private static void CreadModuleMgrRegister(List<string> nameLst)
        {
            string classStr = ModuleMgrRegisterTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            string UIval = string.Empty;
            string UICtrlval = string.Empty;
            string Ctrlval = string.Empty;
            string Modelval = string.Empty;
            foreach (var item in nameLst)
            {
                UIval += UIFieldTemplate.Replace("#FieldName#", item) + "\r\n";
                UICtrlval += UICtrlFieldTemplate.Replace("#FieldName#", item) + "\r\n";
                Ctrlval += CtrlFieldTemplate.Replace("#FieldName#", item) + "\r\n";
                Modelval += ModelFieldTemplate.Replace("#FieldName#", item) + "\r\n";
            }
            classStr = classStr.Replace("#UIVal#", UIval);
            classStr = classStr.Replace("#UICtrlVal#", UICtrlval);
            classStr = classStr.Replace("#CtrlVal#", Ctrlval);
            classStr = classStr.Replace("#ModelVal#", Modelval);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/ModuleMgrRegister_AudioCread.cs", classStr, Encoding.UTF8);
        }

        private static void CreadUICtrlMsgConst(List<string> nameLst)
        {
            string classStr = UICtrlMsgClassTemplate.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            string val = string.Empty;
            foreach (var item in nameLst)
            {
                val += UICtrlMsgFieldTemplate.Replace("#FieldName#", item) + "\r\n";
            }
            classStr = classStr.Replace("#Val#",val);
            File.WriteAllText(UnityEditorPathConst.AudioCreadPath_Assets + @"/ModuleMgr/UICtrlMsg_OpenClose_AudioCread.cs", classStr, new UTF8Encoding(false));
            Debug.Log("[ModuleMgr_AudioCread]UI消息注册完成");
        }

    }
}