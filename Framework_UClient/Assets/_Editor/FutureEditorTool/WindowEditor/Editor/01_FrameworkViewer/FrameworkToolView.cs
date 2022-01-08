using UnityEditor;
using UnityEngine;

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
            minSize = new Vector2(900, 300);

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
                case (int)ShowType.ExcelTool:
                    RefreshUI_ExcelTool();
                    break;
                case (int)ShowType.AutoRegisterTool:
                    RefreshUI_EditorTool();
                    break;
            }
        }

        private enum ShowType
        {
            ExcelTool = 0,
            GameTool = 1,
            FeilTool = 2,
            AutoRegisterTool = 3,
        }


        private void RefreshUI_ExcelTool()
        {
            GUILayout.BeginArea(new Rect(10, 30, 200, 200));

            GUILayout.BeginVertical(GUILayout.Height(50), GUILayout.Width(100));

            if (GUILayout.Button("Dll自动打表", GUILayout.Height(40), GUILayout.Width(100)))
            {
                ConfigBatTool.SyncConfig2Dll();

            }
            if (GUILayout.Button("CS文件自动打表", GUILayout.Height(40), GUILayout.Width(100)))
            {
                ConfigBatTool.SyncConfig2CS();

            }
            if (GUILayout.Button("Dll自动打表(表数据加密)", GUILayout.Height(40), GUILayout.Width(150)))
            {
                ConfigBatTool.SyncConfig2Dll_EncryptData();
            }
            if (GUILayout.Button("CS文件自动打表(表数据加密)", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ConfigBatTool.SyncConfig2CS_EncryptData();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void RefreshUI_EditorTool()
        {
            GUILayout.BeginArea(new Rect(10, 30, 200, 200));
            if (GUILayout.Button("自动注册编辑器环境", GUILayout.Height(40), GUILayout.Width(180)))
            {
                EditorEnvironmentAutoRegisterTool.AutoRegisterAll();
            }
            GUILayout.EndArea();
        }


    }
}

