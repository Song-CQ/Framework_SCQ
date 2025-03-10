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
using UnityEngine.UI;

namespace FutureEditor
{
    public static class MVC_CreadTool
    {
        public static string outPath =  Application.dataPath +"/../"+ UnityEditorPathConst.ModuleUIPath;

        public static string commOutPath = Application.dataPath + "/../" + UnityEditorPathConst.CommonModuleUIPath;

        public static string templatePath = Application.dataPath + @"\_Editor\FutureEditorTool\EditorTool\ProjectTool\AutoCreatorTool\MVC_AutoCreator\Template";
        public static string uguiPrefabPath =  UnityEditorPathConst.ResUGUIPath;


        public static void OpenGUICread()
        {
            EditorCreadWnd.ShowWindow("创建 GUI MVC 代码模版", CreadGUIMVC);
        }
       

        private static string CreadGUIMVC(string name)
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
            Debug.LogFormat("[MVC_AudioCread]开始生成{0}MVC代码({1})".AddColor(ColorType.淡青),name,AppConst.UIDriver.ToString());

            string TemplateName = string.Format("UI_{0}Template",AppConst.UIDriver.ToString());
            CreadUI_GUI(directoryInfo, name,TemplateName);
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

        private static void CreadUI_GUI(DirectoryInfo directoryInfo, string name,string templateName, bool isHotFix=false)
        {       
            string uiClassStr = File.ReadAllText(string.Format(@"{0}\{1}.txt", templatePath,templateName));
            uiClassStr = uiClassStr.Replace("#ClassName#",name).Replace(
            "#CreateTime#",TimerUtil.GetLacalTimeYMD_HHMMSS()).Replace("#AssetName#", name+ "_Plane");
            string targetPath = directoryInfo.FullName + @"\" + name + "UI.cs";
            if (isHotFix)
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp.HotFix");
            }
            else
            {
                uiClassStr = uiClassStr.Replace("#namespace#", "ProjectApp");
            }
            if (AppConst.UIDriver == UIDriverEnem.UGUI)
            {
                Fill_UGUICont(name, uiClassStr);
                Cread_UGUIWnd_Prefab(name);
            }
            

            File.WriteAllText(targetPath, uiClassStr, Encoding.UTF8);
            Debug.Log($"[MVC_AudioCread]{name}UI.cs生成完成".AddColor(ColorType.淡蓝));
        }

        private static void Cread_UGUIWnd_Prefab(string name)
        {
            
            if (!Directory.Exists(uguiPrefabPath + @"\" + name + "_UIPack"))
            {
                Directory.CreateDirectory(uguiPrefabPath + @"\" + name + "_UIPack");
            }
            string prefabPath = uguiPrefabPath + @"\" + name + "_UIPack" + @"\" + name + "_Plane.prefab";
            if (!File.Exists(prefabPath))
            {
                GameObject wndPrefab  = new GameObject(name + "_Plane");
                RectTransform rectTransform = wndPrefab.AddComponent<RectTransform>();

                wndPrefab.layer = LayerMaskConst.UI;
                rectTransform.pivot = new Vector2(0.5f,0.5f);
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector2.one;

                PrefabUtility.SaveAsPrefabAsset(wndPrefab, prefabPath,out bool success);
             
                GameObject.DestroyImmediate(wndPrefab);
                if (success)
                {
                    Debug.Log("创建UI界面预制体成功");
                }
                else
                {
                    Debug.LogError("创建UI界面预制体失败");
                }

                GameObject UIRootGo = GameObject.Find("UIRoot");
                if (!UIRootGo)
                {

                    UIRootGo = new GameObject("UIRoot");
                    UIRootGo.AddComponent<RectTransform>();
                  
                    
                    var canvas = UIRootGo.AddComponent<Canvas>();
                    var UIRoot = UIRootGo.transform;

                    UIRoot.localPosition = Vector3.zero;
                    UIRoot.gameObject.layer = LayerMaskConst.UI;

                    var canvasScaler = UIRoot.gameObject.AddComponent<CanvasScaler>();
                   
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.referenceResolution = AppConst.UIResolution;

                }

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                GameObject instance = PrefabUtility.InstantiatePrefab(prefab, UIRootGo.transform) as GameObject;              
                instance.transform.localPosition = Vector3.zero;
                UnityEditorTool.SelectObject_Assets(instance);
            }

        }

        private static string Fill_UGUICont(string name,string uiClassStr)
        {         
            GameObject wnd = Resources.Load<GameObject>(string.Format("UGUI/{0}_UIPack/{0}_Plane", name));
            if (wnd == null)
            {
                wnd = Resources.Load<GameObject>(string.Format("UGUI/Common_UIPack/{0}_Plane", name));
            }
            if (wnd!=null)
            {
                string val = Fill_UGUICont(wnd);
                uiClassStr.Replace("#region 控件常量\n#endregion", "#region 控件常量\n" + val + "#endregion");
            }
            
            return uiClassStr;

        }

        public static string Fill_UGUICont(GameObject wnd) 
        {
            string val = string.Empty;

            foreach (var item in wnd.GetComponentsInChildren<Transform>())
            {
                string item_name = item.name;
                if (item_name.Trim().StartsWith("ui_"))
                {
                    val += string.Format("private const string {0}_Key = \"{0}\";\n", item_name);
                }
            } 
            return val;
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