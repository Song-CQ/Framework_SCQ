using Codice.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static FutureEditor.ConfigBatTool;

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
        public static string ExcelPath = Path.GetFullPath(Application.dataPath + @"\..\..\_Resources\配置表\游戏配置表\");

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

        public static void OpenExcelPath()
        {
            // 检查文件夹是否存在
            if (Directory.Exists(ExcelPath))
            {
                // 方法1：在文件资源管理器中打开文件夹（Windows）
                EditorUtility.RevealInFinder(ExcelPath);

                // 方法2：或者使用 System.Diagnostics.Process 打开
                // System.Diagnostics.Process.Start(ExcelPath);

                LogUtil.Log($"已打开Excel目录: {ExcelPath}");
            }


        }


        /// <summary>
        /// 自动打表
        /// </summary>

        public static async void SyncConfigData(BuildOutType type = BuildOutType.Dll, bool isEnciphermentData = false, bool isOutMultipleDatas = false)
        {
            if (EditorUtility.DisplayDialog("【自动化】自动打表", "是否进行自动化打表！", "确认", "取消"))
            {
                string cmd = @"ExcelTool\自动化打表生成.bat";
                string cmdFile = Path.Combine(ToolDir, cmd);

                if (File.Exists(cmdFile))
                {
                    // 显示进度条
                    EditorUtility.DisplayProgressBar("自动打表", "正在打表中...", 0.5f);

                    try
                    {
                        // 异步执行打表
                        await Task.Run(() =>
                        {
                            ProcessStartInfo processInfo = new ProcessStartInfo
                            {
                                FileName = "cmd.exe",
                                Arguments = $"/c \"{cmdFile}\" {type} {isEnciphermentData} {isOutMultipleDatas}",
                                WorkingDirectory = ToolDir,
                                UseShellExecute = true,
                                CreateNoWindow = false,
                                WindowStyle = ProcessWindowStyle.Normal
                            };

                            using (Process process = new Process())
                            {
                                process.StartInfo = processInfo;
                                process.Start();
                                process.WaitForExit();
                            }
                        });

                        // 回到主线程刷新资源
                        EditorApplication.delayCall += () =>
                        {
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            EditorUtility.ClearProgressBar();
                            LogUtil.Log("[ConfigBatTool]自动打表完成");
                        };
                    }
                    catch (System.Exception e)
                    {
                        EditorUtility.ClearProgressBar();
                        UnityEngine.Debug.LogError($"打表失败: {e.Message}");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("没有Bat打表文件");
                    EditorUtility.ClearProgressBar();
                }
            }
        }

    }
}

