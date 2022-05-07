/****************************************************
    文件：UI_AutoCread.cs
	作者：Clear
    日期：2022/2/7 19:2:24
    类型: 框架核心脚本(请勿修改)
	功能：UI自动创建注册
*****************************************************/
using FutureCore;
using System;
using System.IO;
using UnityEngine;

namespace FutureEditor
{
    public static class UI_AutoCread
    {
        private static string Auto_UIMgr;

        public static void AutoRegister()
        {

            if (AppConst.UIDriver == UIDriverEnem.FGUI)
            {
                Auto_UIMgr = string.Empty;
                CopyFGUIStript(UnityEditorPathConst.FGUIClassPath_Assets);
                AutoRegisterFGUI(UnityEditorPathConst.FGUIClassPath);
                FinishRegisterFGUI();
            }

        }
        public static void AutoRegister_HotFix()
        {
            if (AppConst.UIDriver == UIDriverEnem.FGUI)
            {
                Auto_UIMgr = string.Empty;
                CopyFGUIStript(UnityEditorPathConst.FGUIClassPath_HotFix);
                AutoRegisterFGUI(UnityEditorPathConst.FGUIClassPath_HotFix);
                FinishRegisterFGUI_HotFix();
            }
        }

        private static void CopyFGUIStript(string path)
        {
            if (Directory.Exists(UnityEditorPathConst.Fgui_Stript_Patn))
            {
                DirectoryInfo Fgui_StriptDicInfo = new DirectoryInfo(UnityEditorPathConst.Fgui_Stript_Patn);
                DirectoryInfo[] infos = Fgui_StriptDicInfo.GetDirectories();
                foreach (var item in infos)
                {
                    if (item.Name.StartsWith("A")) continue;
                    FileUtil.CopyFolder(item.FullName,path);
                }

            }
        }

        private static void AutoRegisterFGUI(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                if (directory.GetDirectories().Length != 0)
                {
                    DirectoryInfo[] directories = directory.GetDirectories();
                    for (int i = 0; i < directories.Length; i++)
                    {
                        AutoRegisterFGUI(directories[i].FullName);
                    }
                }               
                // 处理自动注册的逻辑
                FileInfo[] files = directory.GetFiles();
                for (int j = 0; j < files.Length; j++)
                {
                    FileInfo file = files[j];
                    if (!file.Name.EndsWith(".cs")) continue;
                    if (file.Name.StartsWith("A")&& file.Name.EndsWith("Binder.cs")) continue;

                    string dirName = file.DirectoryName;
                    string Name = file.Name.Split('.')[0];
                    if (dirName.Contains("AutoCreator") && dirName.Contains("FGUI_") && Name.EndsWith("Binder"))
                    {
                        Auto_UIMgr += "            " + "UI." + Name.Replace("Binder", "") + "." + Name + ".BindAll();" + "\r\n";
                    }
                }
            }
        }

        private static void FinishRegisterFGUI()
        {

            //string uiCommonPackageInfo = string.Empty;
            //if (Auto_UIMgr.Length > 0)
            //{
            //    Auto_UIMgr = Auto_UIMgr.Substring(0, Auto_UIMgr.Length - 2);

            //    string fguiPackage = "_fui.bytes";
            //    DirectoryInfo uiDirInfo = new DirectoryInfo(UnityEditorPathConst.ResFGUIPath_Assets);
            //    FileInfo[] files = uiDirInfo.GetFiles();
            //    foreach (FileInfo file in files)
            //    {
            //        if (file.Name.EndsWith(fguiPackage))
            //        {
            //            if (file.Name.StartsWith("A") || file.Name.StartsWith("Font_"))
            //            {
            //                string packageName = "\"" + file.Name.Replace(fguiPackage, string.Empty) + "\"";
            //                string row = "            " + "commonPackages.Add(" + packageName + ");" + "\r\n";
            //                uiCommonPackageInfo += row;
            //            }
            //        }
            //    }
            //    if (uiCommonPackageInfo.Length > 0)
            //    {
            //        uiCommonPackageInfo = uiCommonPackageInfo.Substring(0, uiCommonPackageInfo.Length - 2);
            //    }
            //}

            string classStr = @"/****************************************************
    文件: UIRegister_FGUI.cs
    作者: Clear
    日期: #CreateTime#
    类型: 框架自动创建(请勿修改)
    功能: FGUI包注册
* ****************************************************/
using System.Collections.Generic;
namespace ProjectApp
{
    public static partial class UIRegister_FGUI
    {
        public static void AutoRegisterBinder()
        {
//ReplaceBinder
        }

    }
}";
            classStr = classStr.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            string replaceBinderConst = "//ReplaceBinder";
            classStr = classStr.Replace(replaceBinderConst, Auto_UIMgr);
            //string replaceCommonPackageConst = "//ReplaceCommonPackage";
            //classStr = classStr.Replace(replaceCommonPackageConst, uiCommonPackageInfo);
            string targetPath = UnityEditorPathConst.AutoCreadPath_Assets + "/UIMgr/UIRegister_FGUI.cs";
            FutureCore.FileUtil.WriteFile(targetPath, classStr);
            Debug.Log("[UI_AutoCread]注册FGUI包完成");
        }
        private static void FinishRegisterFGUI_HotFix()
        {

            
            string classStr = @"/****************************************************
    文件: UIRegister_FGUI.cs
    作者: Clear
    日期: #CreateTime#
    类型: 框架自动创建(请勿修改)
    功能: FGUI包注册
* ****************************************************/
using System.Collections.Generic;
namespace ProjectApp.HotFix
{
    public static partial class UIRegister_FGUI
    {
        public static void AutoRegisterBinder()
        {
//ReplaceBinder
        }

    }
}";
            classStr = classStr.Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
            string replaceBinderConst = "//ReplaceBinder";
            classStr = classStr.Replace(replaceBinderConst, Auto_UIMgr);

            string targetPath = UnityEditorPathConst.AutoCreadPath_HotFix + "/UIMgr/UIRegister_FGUI.cs";
            FutureCore.FileUtil.WriteFile(targetPath, classStr);
            Debug.Log("[UI_AutoCread]注册FGUI包完成(热更)");
        }

    }
}