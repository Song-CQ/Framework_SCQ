using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FutureEditor
{
    public static class ConfigBatTool
    {
 
        private static string ToolDir = Path.GetFullPath(Application.dataPath + @"\..\..\_Tool\");
        
        [MenuItem("[Framework Tool]/ExcelConfig/自动化打表 Dll")]
        public static void SyncConfig2Dll()
        {
            if (EditorUtility.DisplayDialog("【自动化】Dll自动打表", "是否进行Dll自动化打表！", "确认", "取消"))
            {
                Debug.Log("打表");
                string cmd = @"ExcelTool\1.配置表生成Dll.bat";
                string cmdFile = Path.Combine(ToolDir, cmd);
                if (File.Exists(cmdFile))
                {
                    Process process = Process.Start(cmdFile);
                    process.WaitForExit();
                    process.Close();
                    process.Dispose();

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UnityEngine.Debug.Log("[ConfigBatTool]Dll自动打表完成");
                }
                else
                {
                    UnityEngine.Debug.LogError("没有Bat打表文件");
                }   
                     
            }
        }
        [MenuItem("[Framework Tool]/ExcelConfig/自动化打表 CS")]
        public static void SyncConfig2CS()
        {
            if (EditorUtility.DisplayDialog("【自动化】CS文件自动打表", "是否进行CS文件自动化打表！", "确认", "取消"))
            {
                string cmd = @"ExcelTool\2.配置表生成Cs.bat";
                string cmdFile = Path.Combine(ToolDir, cmd);
                if (File.Exists(cmdFile))
                {
                    Process process = Process.Start(cmdFile);
                    process.WaitForExit();
                    process.Close();
                    process.Dispose();

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UnityEngine.Debug.Log("[ConfigBatTool]CS文件自动打表完成");
                }
                else
                {
                    UnityEngine.Debug.LogError("没有Bat打表文件");
                }
            }
        }
    }
    
}

