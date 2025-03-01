using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GitUtils
{   


    [MenuItem("[FC Tool]/GitTools/Open GitHub Desktop",false,100)] 
    public static void OpenGitHubDesktop()
    {
        string githubDesktopPath = GetGitHubDesktopPath();

        if (!string.IsNullOrEmpty(githubDesktopPath))
        {
            try
            {
                Process.Start(githubDesktopPath);
            
            }
            catch (Exception ex)
            {
                LogUtil.LogError("启动 GitHub Desktop 时出错: " + ex.Message);
            }
        }
        else
        {
            bool result = EditorUtility.DisplayDialog(
            "未找到GitHub Desktop", // 窗口标题
            "未找到 GitHub Desktop，是否要下载并安装\n下载链接: https://desktop.github.com/", // 窗口内容
            "确定", // 确认按钮
            "取消" // 取消按钮
             );

            // 根据用户选择执行操作
            if (result)
            {
                // 打开浏览器下载页面
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://desktop.github.com/",
                    UseShellExecute = true
                });
            }
            else
            {
                
            }

           

           
        }
    }

    private static string GetGitHubDesktopPath()
    {
        // 根据操作系统返回 GitHub Desktop 的路径
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // Windows 路径
            string appDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            return System.IO.Path.Combine(appDataPath, "GitHubDesktop", "GitHubDesktop.exe");
        }
        else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            // macOS 路径
            return "/Applications/GitHub Desktop.app/Contents/MacOS/GitHub Desktop";
        }
        else
        {
            // 其他平台不支持
            LogUtil.LogWarning("当前平台不支持打开 GitHub Desktop。");
            return null;
        }
    }

}
