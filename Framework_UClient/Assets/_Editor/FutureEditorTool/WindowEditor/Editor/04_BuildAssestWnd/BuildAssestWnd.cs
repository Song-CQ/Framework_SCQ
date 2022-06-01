using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using FutureCore;

namespace FutureEditor
{
    public class BuildAssetBundleWnd : EditorWindow
    {
        [MenuItem("GameObject/[Window]/BuildAssetBundle", false, -100)]
        private static void GoOpenFrameworkWin()
        {
            OpenWnd();
        }

        [MenuItem("Assets/[Window]/BuildAssetBundle", false, -100),MenuItem("Assets/[Window]",false,-100)]
        private static void AssOpenFrameworkWin()
        {
            OpenWnd();
        }
        [MenuItem("[FC Window]/打包AssetBundle窗口", false, -100)]
        public static void OpenWnd()
        {
            CreateWindow<BuildAssetBundleWnd>("Build AssetBundle Window");
        }

        private void OnEnable()
        {
      
            minSize = new Vector2(900, 500);
            maxSize = minSize;
            InitData();
            Show();
            Focus();
        }

        private ABConfig abConfig;

        private string versionPath;

        private void InitData()
        {
            string abConfigPath = UnityEditorPathConst.ABConfigPatn_Assest + "/ABConfig.asset";
            abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
            if (abConfig == null)
            {
                ScriptableObjectTool.CreadScriptableObject<ABConfig>(UnityEditorPathConst.ABConfigPatn_Assest);
                abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
                abConfig.outputPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "AssetBundle";
                abConfig.abRoot = Application.dataPath+ "/_AssetBundleRes";
                abConfig.verifyPath = UnityEditorPathConst.ABConfigPatn_Assest + "/version.json";
                abConfig.allBeDependPath = UnityEditorPathConst.ABConfigPatn_Assest + "/allBeDependData.json";
                abConfig.allDependPath = UnityEditorPathConst.ABConfigPatn_Assest + "/allDependData.json";
                abConfig.abOptions = BuildAssetBundleOptions.None;
                abConfig.abPlatform = BuildTarget.StandaloneWindows;
            }
            versionPath = abConfig.verifyPath.Replace("/version.json",string.Empty);
        }

        private void OnGUI()
        {
            RefreshABConfig();




        }

        private void RefreshABConfig()
        {
            GUILayout.BeginArea(new Rect(450, 0 , 450, 500), GUI.skin.GetStyle("ShurikenModuleBg"));
            GUILayout.BeginVertical();

            GUILayout.Space(10);

            GUIStyle gUIStyle = new GUIStyle();

            GUILayout.BeginHorizontal();

            gUIStyle.fontStyle = FontStyle.BoldAndItalic;
            gUIStyle.fontSize = 16;

            GUILayout.Label("AssetBundle Config", gUIStyle);
            GUILayout.EndHorizontal();

            

            GUILayout.Label("资源文件夹(Asset):", GUILayout.Height(20));

            GUILayout.BeginHorizontal();
            abConfig.abRoot = GUILayout.TextField(abConfig.abRoot, GUILayout.Width(300));
            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                string path = EditorUtility.OpenFolderPanel("选择资源文件夹", Application.dataPath, "").Replace(@"\","/").Replace(@"\\","/");
                if (path.Contains(Application.dataPath))
                {
                    abConfig.abRoot = path;
                }
            }
            GUILayout.EndHorizontal();
          
         
       
            GUILayout.Label("校验缓存文件夹:", GUILayout.Height(20));

            versionPath = GUILayout.TextField(versionPath, GUILayout.Height(20),GUILayout.Width(450));
            abConfig.verifyPath = versionPath + "/version.json";
            abConfig.allBeDependPath = versionPath + "/allBeDependData.json";
            abConfig.allDependPath = versionPath + "/allDependData.json";

  

            GUILayout.EndVertical();
            GUILayout.EndArea();


            GUILayout.BeginScrollView(new Vector2(0,0), GUILayout.Height(400), GUILayout.Width(450));

            
            GUILayout.EndScrollView();

        }


     
        #region UnityTool
        private void RefreshUI_UnityTool()
        {
            
            GUILayout.BeginArea(new Rect(10, 35, 200, 800));
            GUILayout.Label("Unity Editor",  GUILayout.Height(20),GUILayout.Width(80));
            if (GUILayout.Button("重启Unity", GUILayout.Height(40), GUILayout.Width(100)))
            {
                UnityEditorTool.StartRest();
            }     
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(550, 60, 300, 500));
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            GUILayout.Label("AppName:", GUILayout.Width(80));
            GUILayout.Space(30);
            GUILayout.TextField(ProjectApp.AppFacade.AppName);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("AppDesc:", GUILayout.Width(80));
            GUILayout.Space(30);
            GUILayout.TextField(ProjectApp.AppFacade.AppDesc);

            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 235, 200, 800));
            GUILayout.Label("Visual Studio Editor", GUILayout.Height(20));
            if (GUILayout.Button("删除VS解决方案", GUILayout.Height(40), GUILayout.Width(100)))
            {
                DeleteSin();
            }
            GUILayout.EndArea();

        }

        private void DeleteSin()
        {
            if (FutureCore.FileUtil.DeleteFileOrDirectory(UnityEditorPathConst.Project_Sin_Path))
            {
                EditorUtility.DisplayDialog("删除完成!", "请重新打开VS,生成解决方案!", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("删除失败!", "请确认是否解决方案和项目名一致或文件不存在!", "确定");
            }


        }
        #endregion

        #region ExcelTool
        private void RefreshUI_ExcelTool()
        {

            GUILayout.BeginArea(new Rect(10, 35, 200, 200));

            GUILayout.BeginVertical(GUILayout.Height(50), GUILayout.Width(100));

            if (GUILayout.Button("Dll自动打表", GUILayout.Height(40), GUILayout.Width(100)))
            {
                Close();
                ConfigBatTool.SyncConfig2Dll();

            }
            if (GUILayout.Button("CS文件自动打表", GUILayout.Height(40), GUILayout.Width(100)))
            {
                Close();
                ConfigBatTool.SyncConfig2CS();

            }
            if (GUILayout.Button("Dll自动打表(表数据加密)", GUILayout.Height(40), GUILayout.Width(150)))
            {
                Close();
                ConfigBatTool.SyncConfig2Dll_EncryptData();
            }
            if (GUILayout.Button("CS文件自动打表(表数据加密)", GUILayout.Height(40), GUILayout.Width(180)))
            {
                Close();
                ConfigBatTool.SyncConfig2CS_EncryptData();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        #endregion

        #region CreateTool

        private void RefreshUI_CreateTool()
        {
            GUILayout.BeginArea(new Rect(10, 35, 200, 200));
            GUILayout.Label("MVC");
            GUILayout.BeginVertical();

            if (GUILayout.Button("创建FGUI_MVC代码模版", GUILayout.Height(40), GUILayout.Width(160)))
            {
                MVC_CreadTool.OpenFGUICread();
                Close();
            } 
            if (GUILayout.Button("创建FGUI_MVC代码模版(热更)", GUILayout.Height(40), GUILayout.Width(200)))
            {
                MVC_CreadTool.OpenFGUICread_HotFix();
                Close();
            }
        
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        #endregion

        #region AutoRegisterTool
        private void RefreshUI_AutoRegisterTool()
        {
            GUILayout.BeginArea(new Rect(10, 35, 200, 200));
            if (GUILayout.Button("注册编辑器环境", GUILayout.Height(40), GUILayout.Width(180)))
            {
                EditorAutoRegisterTool_Editor.AutoRegisterAll();
                Close();
            }          
            if (GUILayout.Button("自动注册项目数据", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ProjectAutoRegisterTool.AutoRegisterAll();
                Close();
            }
            if (GUILayout.Button("自动注册项目模块(热更)", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ProjectAutoRegisterTool.AutoRegisterAll_HotFix();
                Close();
            }
            GUILayout.EndArea();
        }

        #endregion
    }
}

/*
增加删除解决方案 

增加编辑该脚本功能




 
 
 */