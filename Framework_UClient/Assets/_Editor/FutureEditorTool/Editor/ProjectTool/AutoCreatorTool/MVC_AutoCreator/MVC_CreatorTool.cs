/****************************************************
    文件:MVC_CreadTool.cs
    作者:Clear
    日期:2022/1/26 17:22:11
    类型:框架核心脚本(请勿修改)
    功能:MVC创建
*****************************************************/
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using FutureCore;

namespace FutureEditor
{
    public static class MVC_CreadTool
    {
        public static string outPath = Application.dataPath +"/../"+ UnityEditorPathConst.ModuleUIPath;

        public static string commOutPath = Application.dataPath + "/../" + UnityEditorPathConst.CommonModuleUIPath;

        public static string hotFixOutPath = UnityEditorPathConst.ModuleUIPath_HotFix;

        public static string templatePath = Application.dataPath + @"\_Editor\FutureEditorTool\Editor\ProjectTool\AutoCreatorTool\MVC_AutoCreator\Template";


        public static void OpenFGUICread()
        {
            EditorCreadWnd.ShowWindow("创建 FGUI MVC 代码模版", CreadFGUIMVC);
        }
        public static void OpenFGUICread_HotFix()
        {
            EditorCreadWnd.ShowWindow("创建 FGUI MVC 代码模版(热更)", CreadFGUIMVC_HotFix);
        }

        private static string CreadFGUIMVC_HotFix(string name)
        {
            string[] names = Directory.GetDirectories(hotFixOutPath);
            foreach (var item in names)
            {
                if (item.Replace(hotFixOutPath + @"\", string.Empty) == name)
                {

                    if (Directory.GetFiles(item).Length != 0)
                        return "已有同名模块";
                }
            }
            names = Directory.GetDirectories(commOutPath);
            foreach (var item in names)
            {
                if (item.Replace(commOutPath + @"\", string.Empty) == name)
                {

                    if (Directory.GetFiles(item).Length != 0)
                        return "通用模块已有同名模块";
                }
            }
            DirectoryInfo directoryInfo = Directory.CreateDirectory(hotFixOutPath + @"\" + name);
            Debug.LogFormat("[MVC_AudioCread]开始生成{0}MVC 热更代码".AddColor(ColorType.淡青), name);
            CreadUI_FGUI(directoryInfo, name,true);
            CreadUICtr(directoryInfo, name, true);
            CreadCtr(directoryInfo, name, true);
            CreadModel(directoryInfo, name, true);
            ModuleMgr_AutoCreator.AutoRegister_HotFix();

            Debug.Log("[MVC_AudioCread]MVC 热更代码生成完成".AddColor(ColorType.浅黄));
            return "End";


        }



        private static string CreadFGUIMVC(string name)
        {
          
            string[] names = Directory.GetDirectories(outPath);
            foreach (var item in names)
            {
                if (item.Replace(outPath+@"\", string.Empty) == name)
                {
            
                    if (Directory.GetFiles(item).Length!=0)
                    return "已有同名模块";
                }
            }
            names = Directory.GetDirectories(commOutPath);
            foreach (var item in names)
            {
                if (item.Replace(commOutPath + @"\", string.Empty) == name)
                {

                    if (Directory.GetFiles(item).Length != 0)
                        return "通用模块已有同名模块";
                }
            }

            DirectoryInfo directoryInfo = Directory.CreateDirectory(outPath + @"\" + name);
            Debug.LogFormat("[MVC_AudioCread]开始生成{0}MVC代码".AddColor(ColorType.淡青), name);
            CreadUI_FGUI(directoryInfo, name);
            CreadUICtr(directoryInfo, name);
            CreadCtr(directoryInfo, name);
            CreadModel(directoryInfo, name);
            ModuleMgr_AutoCreator.AutoRegister();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UnityEditorTool.SelectObject_Assets(UnityEditorPathConst.ModuleUIPath + @"\" + name);
            Debug.Log("[MVC_AudioCread]MVC生成完成".AddColor(ColorType.浅黄));
            return "End";
        }

        private static void CreadUI_FGUI(DirectoryInfo directoryInfo, string name,bool isHotFix=false)
        {   
            string uiClassStr = File.ReadAllText(templatePath + @"\UI_FGUITemplate.txt");
            uiClassStr = uiClassStr.Replace("#ClassName#",name).Replace(
            "#CreateTime#",TimerUtil.GetLacalTimeYMD_HHMMSS());
            string targetPath = directoryInfo.FullName + @"\" + name + "UI.cs";
            if (isHotFix)
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp.HotFix");
            }
            else
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp");
            }
            File.WriteAllText(targetPath, uiClassStr, Encoding.UTF8);
            Debug.Log($"[MVC_AudioCread]{name}UI.cs生成完成".AddColor(ColorType.淡蓝));
        } 
        
        private static void CreadCtr(DirectoryInfo directoryInfo, string name, bool isHotFix = false)
        {   
            string uiClassStr = File.ReadAllText(templatePath + @"\CtrlTemplate.txt");
            uiClassStr = uiClassStr.Replace("#ClassName#",name).Replace(
            "#CreateTime#",TimerUtil.GetLacalTimeYMD_HHMMSS());
            string targetPath = directoryInfo.FullName + @"\" + name + "Ctrl.cs";
            if (isHotFix)
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp.HotFix");
            }
            else
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp");
            }
            File.WriteAllText(targetPath, uiClassStr, Encoding.UTF8);
            Debug.Log($"[MVC_AudioCread]{name}Ctrl.cs生成完成".AddColor(ColorType.淡蓝));
        }
        
        private static void CreadUICtr(DirectoryInfo directoryInfo, string name, bool isHotFix = false)
        {   
            string uiClassStr = File.ReadAllText(templatePath + @"\UICtrlTemplate.txt");
            uiClassStr = uiClassStr.Replace("#ClassName#",name).Replace(
            "#CreateTime#",TimerUtil.GetLacalTimeYMD_HHMMSS());
            string targetPath = directoryInfo.FullName + @"\" + name + "UICtrl.cs";
            if (isHotFix)
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp.HotFix");
            }
            else
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp");
            }
            File.WriteAllText(targetPath, uiClassStr, Encoding.UTF8);
            Debug.Log($"[MVC_AudioCread]{name}UICtrl.cs生成完成".AddColor(ColorType.淡蓝));
        }
        
        private static void CreadModel(DirectoryInfo directoryInfo, string name, bool isHotFix = false)
        {   
            string uiClassStr = File.ReadAllText(templatePath + @"\ModelTemplate.txt");
            uiClassStr = uiClassStr.Replace("#ClassName#",name).Replace(
            "#CreateTime#",TimerUtil.GetLacalTimeYMD_HHMMSS());
            string targetPath = directoryInfo.FullName + @"\" + name + "Model.cs";
            if (isHotFix)
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp.HotFix");
            }
            else
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp");
            }
            File.WriteAllText(targetPath, uiClassStr, Encoding.UTF8);
            Debug.Log($"[MVC_AudioCread]{name}Model.cs生成完成".AddColor(ColorType.淡蓝));
        }


    }
}