using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using FutureCore;

namespace FutureEditor
{
    public class FrameworkToolView : EditorWindow
    {
        [MenuItem("GameObject/[Open Tool Window]", false, -1000)]
        private static void GoOpenFrameworkWin()
        {
            OpenFrameworkWin();
        }
        [MenuItem("Assets/[Open Tool Window]", false, -1000)]
        private static void AssOpenFrameworkWin()
        {
            OpenFrameworkWin();
        }
        [MenuItem("[FC Window]/框架工具窗口", false, -100)]
        public static void OpenFrameworkWin()
        {
            CreateWindow<FrameworkToolView>("Framework Tool");
        }

        private void OnEnable()
        {
      
            minSize = new Vector2(900, 500);
            maxSize = minSize;
            InitData();
            Show();
            Focus();
        }

        private void InitData()
        {
            toolbatVal = new string[System.Enum.GetValues(typeof(ShowType)).Length];
            System.Collections.IList list = System.Enum.GetValues(typeof(ShowType));
            for (int i = 0; i < list.Count; i++)
            {
                ShowType t = (ShowType)list[i];
                toolbatVal[i] = t.ToString();
            }
        }


        private string[] toolbatVal;
        private int toolbatIndex = 0;
        private void OnGUI()
        {
          
            toolbatIndex = GUILayout.Toolbar(toolbatIndex, toolbatVal);
            switch (toolbatIndex)
            {
            
                case (int)ShowType.UnityTool:
                    RefreshUI_UnityTool();
                    break;
                case (int)ShowType.ExcelTool:
                    RefreshUI_ExcelTool();
                    break;
                case (int)ShowType.CreateTool:
                    RefreshUI_CreateTool();
                    break;
                case (int)ShowType.AutoRegisterTool:
                    RefreshUI_AutoRegisterTool();
                    break; 
                case (int)ShowType.OpenWnidow:
                    RefreshUI_OpenWnidow();
                    break;
            }
        }

       

        private enum ShowType
        {
            UnityTool=0,
            ExcelTool,
            GameTool,
            CreateTool,
            AutoRegisterTool,
            OpenWnidow,
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
            GUILayout.BeginVertical();
            GUILayout.Label("Visual Studio Editor", GUILayout.Height(20));
            if (GUILayout.Button("删除VS解决方案", GUILayout.Height(40), GUILayout.Width(100)))
            {
                DeleteSin();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();


            GUILayout.BeginArea(new Rect(550, 235, 200, 800));
            GUILayout.Label("  Open Path", GUI.skin.GetStyle("IN TitleText"), GUILayout.Height(20));
            GUILayout.BeginVertical(GUI.skin.GetStyle("FrameBox"));

            if (GUILayout.Button("Open AssetDataPath"))
            {
                Application.OpenURL(Application.dataPath);
            }

            if (GUILayout.Button("Open PersistentDataPath"))
            {
                Application.OpenURL(Application.persistentDataPath);
            }
            GUILayout.Space(3);
            if (GUILayout.Button("Open TemporaryCachePath"))
            {
                Application.OpenURL(Application.temporaryCachePath);
            }

            GUILayout.EndVertical();
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
            GUILayout.Label("MVC (根据UI驱动类型)");
            GUILayout.BeginVertical();

            if (GUILayout.Button("创建GUI_MVC代码模版", GUILayout.Height(40), GUILayout.Width(160)))
            {
                MVC_CreadTool.OpenFGUICread();
                Close();
            } 
            if (GUILayout.Button("创建GUI_MVC代码模版(热更)", GUILayout.Height(40), GUILayout.Width(200)))
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

        #region OpenWnidowTool
        private void RefreshUI_OpenWnidow()
        {
            GUILayout.BeginArea(new Rect(10, 35, 200, 200));
            if (GUILayout.Button("AssetBundle窗口", GUILayout.Height(40), GUILayout.Width(180)))
            {
                BuildAssetBundleWnd.OpenWnd();
                Close();
            }   
            GUILayout.EndArea();
        }
        #endregion

    }
}

