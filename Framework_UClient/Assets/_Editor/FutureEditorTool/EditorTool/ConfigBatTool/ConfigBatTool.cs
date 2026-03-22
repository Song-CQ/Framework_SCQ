using Codice.Utils;
using ProjectApp;
using ProjectApp.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
            SyncConfigData(ConfigVOVersion.InternalVersion, BuildOutType.Dll, false, true, AppFacade.AESIVector, AppFacade.AESKey);
          
        }

        [MenuItem("[FC Tool]/ExcelConfig Tool/自动化打表 CS")]
        public static void SyncConfig2CS()
        {
            SyncConfigData(ConfigVOVersion.InternalVersion, BuildOutType.CS, false, true, AppFacade.AESIVector, AppFacade.AESKey);

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
        public static async void SyncConfigData(int Version, BuildOutType type = BuildOutType.Dll, bool isOutMultipleDatas = false, bool isEnciphermentData = false, string AES_IVector = "", string AES_Key = "")
        {
            // 添加验证
            if (!ValidateAESParameters(isEnciphermentData, AES_IVector, AES_Key))
            {
                return; // 验证失败，直接返回
            }

            if (EditorUtility.DisplayDialog("【自动化】自动打表", "是否进行自动化打表！", "确认", "取消"))
            {
                string cmd = @"ExcelTool\自动化打表生成.bat";
                string cmdFile = Path.Combine(ToolDir, cmd);

                if (File.Exists(cmdFile))
                {
                    // 显示进度条
                    EditorUtility.DisplayProgressBar("自动打表", "正在准备打表...", 0.5f);

                    try
                    {
                        // 创建临时参数文件
                        string tempParamFile = Path.Combine(Path.GetTempPath(), $"ExcelToolParams_{DateTime.Now.Ticks}.txt");

                        // 将参数写入临时文件（每行一个参数）
                        StringBuilder paramContent = new StringBuilder();
                        paramContent.AppendLine(Version.ToString());                    // 第1行: Version
                        paramContent.AppendLine((type).ToString());               // 第2行: Type
                        paramContent.AppendLine(isOutMultipleDatas.ToString());        // 第3行: isOutMultipleDatas
                        paramContent.AppendLine(isEnciphermentData.ToString());        // 第4行: isEnciphermentData
                        paramContent.AppendLine(AES_IVector);                          // 第5行: AES_IVector
                        paramContent.AppendLine(AES_Key);                              // 第6行: AES_Key

                        // 使用 ANSI 编码避免中文乱码
                        File.WriteAllText(tempParamFile, paramContent.ToString(), Encoding.GetEncoding("GB2312"));

                        //UnityEngine.Debug.LogError($"参数文件已创建: {tempParamFile}");

                        // 更新进度条
                        EditorUtility.DisplayProgressBar("自动打表", "正在执行打表...", 0.7f);

                        // 异步执行打表，只传递参数文件路径
                        await Task.Run(() =>
                        {
                            ProcessStartInfo processInfo = new ProcessStartInfo
                            {
                                FileName = cmdFile,
                                Arguments = $"\"{tempParamFile}\"",  // 只传递参数文件路径
                                WorkingDirectory = ToolDir,
                                UseShellExecute = true,      // 显示窗口
                                CreateNoWindow = false,      // 创建窗口
                                WindowStyle = ProcessWindowStyle.Normal
                            };

                            using (Process process = new Process())
                            {
                                process.StartInfo = processInfo;
                                process.Start();
                                process.WaitForExit();
                            }

                            //清理临时文件
                            try
                            {
                                if (File.Exists(tempParamFile))
                                    File.Delete(tempParamFile);
                                UnityEngine.Debug.Log($"临时文件已删除: {tempParamFile}");
                            }
                            catch (Exception ex)
                            {
                                UnityEngine.Debug.LogWarning($"删除临时文件失败: {ex.Message}");
                            }
                        });

                        // 回到主线程刷新资源
                        EditorApplication.delayCall += () =>
                        {
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            EditorUtility.ClearProgressBar();
                            LogUtil.Log("[ConfigBatTool]自动打表完成");
                            EditorUtility.DisplayDialog("成功", "自动打表完成！", "确定");
                        };
                    }
                    catch (System.Exception e)
                    {
                        EditorUtility.ClearProgressBar();
                        UnityEngine.Debug.LogError($"打表失败: {e.Message}");
                        EditorUtility.DisplayDialog("错误", $"打表失败: {e.Message}", "确定");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"没有找到打表文件: {cmdFile}");
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("错误", $"没有找到打表文件:\n{cmdFile}", "确定");
                }
            }
        }

        /// <summary>
        /// 验证 AES 加密参数
        /// </summary>
        private static bool ValidateAESParameters(bool isEnciphermentData, string aesIVector, string aesKey)
        {
            // 如果不加密，直接返回 true
            if (!isEnciphermentData)
            {
                return true;
            }

            // 检查是否为空
            if (string.IsNullOrEmpty(aesIVector))
            {
                EditorUtility.DisplayDialog("错误", "AES IVector 不能为空！\n请在配置中设置 AES IVector。", "确定");
                return false;
            }

            if (string.IsNullOrEmpty(aesKey))
            {
                EditorUtility.DisplayDialog("错误", "AES Key 不能为空！\n请在配置中设置 AES Key。", "确定");
                return false;
            }

            // 检查长度（十六进制字符串）
            if (aesIVector.Length != 32)
            {
                EditorUtility.DisplayDialog("错误",
                    $"AES IVector 长度错误！\n\n当前长度: {aesIVector.Length} 个字符\n需要长度: 32 个字符 (16字节)\n\n当前值: {aesIVector}",
                    "确定");
                return false;
            }

            if (aesKey.Length != 32 && aesKey.Length != 64)
            {
                EditorUtility.DisplayDialog("错误",
                    $"AES Key 长度错误！\n\n当前长度: {aesKey.Length} 个字符\n需要长度: 32 或 64 个字符 (16或32字节)\n\n当前值: {aesKey}",
                    "确定");
                return false;
            }

            // 检查是否为有效的十六进制字符串
            System.Text.RegularExpressions.Regex hexRegex = new System.Text.RegularExpressions.Regex(@"\A\b[0-9A-Fa-f]+\b\Z");

            if (!hexRegex.IsMatch(aesIVector))
            {
                EditorUtility.DisplayDialog("错误",
                    $"AES IVector 包含无效字符！\n\n只允许十六进制字符 (0-9, A-F)\n\n当前值: {aesIVector}",
                    "确定");
                return false;
            }

            if (!hexRegex.IsMatch(aesKey))
            {
                EditorUtility.DisplayDialog("错误",
                    $"AES Key 包含无效字符！\n\n只允许十六进制字符 (0-9, A-F)\n\n当前值: {aesKey}",
                    "确定");
                return false;
            }

            // 尝试转换为字节数组，验证是否能成功转换
            try
            {
                byte[] ivBytes = HexStringToByteArray(aesIVector);
                byte[] keyBytes = HexStringToByteArray(aesKey);

                if (ivBytes.Length != 16)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"AES IVector 字节长度错误！\n\n转换后长度: {ivBytes.Length} 字节\n需要长度: 16 字节",
                        "确定");
                    return false;
                }

                if (keyBytes.Length != 16 && keyBytes.Length != 32)
                {
                    EditorUtility.DisplayDialog("错误",
                        $"AES Key 字节长度错误！\n\n转换后长度: {keyBytes.Length} 字节\n需要长度: 16 或 32 字节",
                        "确定");
                    return false;
                }

                UnityEngine.Debug.Log($"✅ AES 参数验证通过 - IV长度: {ivBytes.Length}字节, Key长度: {keyBytes.Length}字节");
                return true;
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("错误",
                    $"AES 参数转换失败！\n\n错误信息: {ex.Message}\n\n请检查 IVector 和 Key 是否为有效的十六进制字符串。",
                    "确定");
                return false;
            }
        }

        /// <summary>
        /// 十六进制字符串转字节数组
        /// </summary>
        private static byte[] HexStringToByteArray(string hex)
        {
            int len = hex.Length;
            byte[] bytes = new byte[len / 2];
            for (int i = 0; i < len; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

    }
}

