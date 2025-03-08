using Codice.Utils;
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class ConfigBatTool
    {
        public enum BuildOutType
        {
            Dll = 0,
            CS = 1,
        }

        public static string ToolDir = Path.GetFullPath(Application.dataPath + @"\..\..\_Tool\");
        
        [MenuItem("[FC Tool]/ExcelConfig Tool/自动化打表 Dll")]
        public static void SyncConfig2Dll()
        {
            SyncConfigData(BuildOutType.Dll, true, false);
        }

        [MenuItem("[FC Tool]/ExcelConfig Tool/自动化打表 CS")]
        public static void SyncConfig2CS()
        {
            SyncConfigData(BuildOutType.CS, true, false);          
        }
     

        /// <summary>
        /// 自动打表
        /// </summary>
        public static void SyncConfigData(BuildOutType type = BuildOutType.Dll , bool isEnciphermentData = false, bool isOutMultipleDatas = false)
        {
            if (EditorUtility.DisplayDialog("【自动化】自动打表", "是否进行自动化打表！", "确认", "取消"))
            {
                //Debug.Log("打表");
                string cmd = @"ExcelTool\自动化打表生成.bat";
                string cmdFile = Path.Combine(ToolDir, cmd);
                if (File.Exists(cmdFile))
                {

                    // 创建 ProcessStartInfo 对象
                    ProcessStartInfo processInfo = new ProcessStartInfo(cmdFile);
                    processInfo.FileName = cmdFile; // 设置要执行的 .bat 文件
                    processInfo.Arguments = $"{type} {isEnciphermentData} {isOutMultipleDatas}"; // 传递参数
              

                    processInfo.UseShellExecute = true; // 不使用操作系统 shell 启动进程
                    processInfo.CreateNoWindow = false; // 不创建新窗口

                    // 启动进程
                    using (Process process = new Process())
                    {
                        process.StartInfo = processInfo;
                        process.Start();


                        // 等待进程结束
                        process.WaitForExit();
                        process.Close();
                        process.Dispose();
                    }
    

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UnityEngine.Debug.Log("[ConfigBatTool]自动打表完成");
                }
                else
                {
                    UnityEngine.Debug.LogError("没有Bat打表文件");
                }

            }



        }
    }
    
}

