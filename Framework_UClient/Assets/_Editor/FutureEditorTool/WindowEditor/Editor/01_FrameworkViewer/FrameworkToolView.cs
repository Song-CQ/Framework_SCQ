using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
        private static void OpenFrameworkWin()
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
            }
        }

        

        private enum ShowType
        {
            UnityTool=0,
            ExcelTool,
            GameTool,
            CreateTool,
            AutoRegisterTool,
        }
        private void RefreshUI_UnityTool()
        {
            GUILayout.BeginArea(new Rect(10,35, 200, 200));
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
            GUILayout.Label("AppDesc", GUILayout.Width(80));
            GUILayout.Space(30);
            GUILayout.TextField(ProjectApp.AppFacade.AppDesc);

            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

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


        #region CreateTool

        private void RefreshUI_CreateTool()
        {
            GUILayout.BeginArea(new Rect(10, 35, 200, 200));
            GUILayout.Label("MVC");
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("创建FGUI_MVC代码模版", GUILayout.Height(40), GUILayout.Width(160)))
            {
                MVC_CreadTool.OpenFGUICread();
                Close();
            }
        
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        #endregion
        private void RefreshUI_AutoRegisterTool()
        {
            GUILayout.BeginArea(new Rect(10, 35, 200, 200));
            if (GUILayout.Button("自动注册编辑器环境", GUILayout.Height(40), GUILayout.Width(180)))
            {
                EditorEnvironmentAutoRegisterTool.AutoRegisterAll();
                Close();
            }
            GUILayout.EndArea();
        }


    }
}

