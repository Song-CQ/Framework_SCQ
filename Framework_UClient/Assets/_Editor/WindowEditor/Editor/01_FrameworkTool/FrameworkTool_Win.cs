using System;
using System.Collections;
using System.Collections.Generic;
using FutureCore;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public class FrameworkTool_Win:EditorWindow
    {
        [MenuItem("GameObject/打开框架功能窗口",false,-1000)]
        private static void GoOpenFrameworkWin()
        {
            OpenFrameworkWin();
        }
        [MenuItem("Assets/Open Window/框架功能窗口",false,-100)]
        private static void AssOpenFrameworkWin()
        {
            OpenFrameworkWin();
        }
        [MenuItem("[Framework Tool]/Open Window/框架功能窗口",false,-100)]
        private static void OpenFrameworkWin()
        { 
            CreateWindow<FrameworkTool_Win>("Framework Tool");
        }

        private void OnEnable()
        {
            minSize = new Vector2(900, 300);
            Show();
            Focus();
        }
        
        private string[] toolbatVal=new []{"ExcelTool","GameTool","FeilTool"};
        private int toolbatIndex = 0;
        private void OnGUI()
        {
            toolbatIndex = GUILayout.Toolbar(toolbatIndex, toolbatVal);
            //GUILayoutOption val = new GUILayoutOption(,);
            
            if (toolbatIndex==0)
            {
                GUILayout.BeginArea(new Rect(10,30,120,100));
                GUILayout.BeginVertical(GUILayout.Height(50),GUILayout.Width(100));
                
                if (GUILayout.Button("Dll自动打表",GUILayout.Height(40),GUILayout.Width(100)))
                {
                    ConfigBatTool.SyncConfig2Dll();
                    
                }
                if (GUILayout.Button("CS文件自动打表",GUILayout.Height(40),GUILayout.Width(100)))
                {
                    ConfigBatTool.SyncConfig2CS();
                    
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
    }
}

