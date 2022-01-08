using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class ConfigBatTool
    {
 
        private static string ToolDir = Path.GetFullPath(Application.dataPath + @"\..\..\_Tool\");
        
        [MenuItem("[FC Tool]/ExcelConfig/自动化打表 Dll")]
        public static void SyncConfig2Dll()
        {
            if (EditorUtility.DisplayDialog("【自动化】Dll自动打表", "是否进行Dll自动化打表！", "确认", "取消"))
            {
                //Debug.Log("打表");
                string cmd = @"ExcelTool\1.自动化打表生成_Dll.bat";
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
        [MenuItem("[FC Tool]/ExcelConfig/自动化打表 Dll(表数据加密)")]
        public static void SyncConfig2Dll_EncryptData()
        {
            if (EditorUtility.DisplayDialog("【自动化】Dll自动打表 数据加密", "是否进行Dll自动化打表!(表数据加密)", "确认", "取消"))
            {
                //Debug.Log("打表");
                string cmd = @"ExcelTool\2.自动化打表生成_Dll_表数据加密.bat";
                string cmdFile = Path.Combine(ToolDir, cmd);
                if (File.Exists(cmdFile))
                {
                    Process process = Process.Start(cmdFile);
                    process.WaitForExit();
                    process.Close();
                    process.Dispose();

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UnityEngine.Debug.Log("[ConfigBatTool]Dll自动打表完成(表数据加密)");
                }
                else
                {
                    UnityEngine.Debug.LogError("没有Bat打表文件");
                }   
                     
            }
        }
        [MenuItem("[FC Tool]/ExcelConfig/自动化打表 CS")]
        public static void SyncConfig2CS()
        {
            if (EditorUtility.DisplayDialog("【自动化】CS文件自动打表", "是否进行CS文件自动化打表！", "确认", "取消"))
            {
                string cmd = @"ExcelTool\3.自动化打表生成_CS.bat";
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
        [MenuItem("[FC Tool]/ExcelConfig/自动化打表 CS(表数据加密)")]
        public static void SyncConfig2CS_EncryptData()
        {
            if (EditorUtility.DisplayDialog("【自动化】CS文件自动打表", "是否进行CS文件自动化打表！(表数据加密)", "确认", "取消"))
            {
                string cmd = @"ExcelTool\4.自动化打表生成_CS_表数据加密.bat";
                string cmdFile = Path.Combine(ToolDir, cmd);
                if (File.Exists(cmdFile))
                {
                    Process process = Process.Start(cmdFile);
                    process.WaitForExit();
                    process.Close();
                    process.Dispose();

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UnityEngine.Debug.Log("[ConfigBatTool]CS文件自动打表完成(表数据加密)");
                }
                else
                {
                    UnityEngine.Debug.LogError("没有Bat打表文件");
                }
            }
        }
    }
    
}

