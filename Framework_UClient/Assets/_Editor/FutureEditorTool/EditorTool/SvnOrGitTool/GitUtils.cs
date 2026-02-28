using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GitUtils
{


    [MenuItem("[FC Tool]/GitTools/Open Git Client", false, 100)]
    public static void OpenGitClient()
    {
        // 1. 先尝试打开 GitKraken
        string gitkrakenPath = GetGitKrakenPath();

        if (!string.IsNullOrEmpty(gitkrakenPath) && System.IO.File.Exists(gitkrakenPath))
        {
            try
            {
                Process.Start(gitkrakenPath);
                LogUtil.Log("正在启动 GitKraken");
                return; // 启动成功，直接返回
            }
            catch (Exception ex)
            {
                LogUtil.LogError("启动 GitKraken 时出错: " + ex.Message);
                // 继续尝试打开 GitHub Desktop
            }
        }
        else
        {
            LogUtil.Log("未找到 GitKraken，尝试打开 GitHub Desktop");
        }

        // 2. GitKraken 失败，尝试打开 GitHub Desktop
        string githubDesktopPath = GetGitHubDesktopPath();

        if (!string.IsNullOrEmpty(githubDesktopPath) && System.IO.File.Exists(githubDesktopPath))
        {
            try
            {
                Process.Start(githubDesktopPath);
                LogUtil.Log("成功启动 GitHub Desktop");
                return;
            }
            catch (Exception ex)
            {
                LogUtil.LogError("启动 GitHub Desktop 时出错: " + ex.Message);
            }
        }

        // 3. 两个都没找到，弹出下载提示
        bool result = EditorUtility.DisplayDialog(
            "未找到 Git 客户端", // 窗口标题
            "未找到 GitKraken 或 GitHub Desktop，是否要下载安装？\n\n" +
            "GitKraken: https://www.gitkraken.com/download\n" + 
            "破解 https://santisify.top/blog/gitkraken-crack/\n"+
            "GitHub Desktop: https://desktop.github.com/ ", // 窗口内容
           
            "确定", // 确认按钮
            "取消" // 取消按钮
        );

        if (result)
        {
            // 让用户选择下载哪个
            int choice = EditorUtility.DisplayDialogComplex(
                "选择下载",
                "请选择要下载的 Git 客户端：",
                "GitKraken",
                
                "GitHub Desktop",
                "取消"
            );

            string url = choice == 0 ? "https://www.gitkraken.com/download" :
                        choice == 1 ? "https://desktop.github.com/" : null;

            if (url != null)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
        }
    }

    private static string GetGitKrakenPath()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // Windows 常见安装路径
            string[] possiblePaths = new[]
            {
            // // 标准安装路径 
            System.IO.Path.Combine(System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.LocalApplicationData),
                "gitkraken", "gitkraken.exe"),
                
            // 用户安装路径
            System.IO.Path.Combine(System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.ApplicationData),
                "..", "Local", "gitkraken", "app-*", "gitkraken.exe"),
                
            // Program Files 路径
            System.IO.Path.Combine(System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.ProgramFiles),
                "GitKraken", "gitkraken.exe"),

            System.IO.Path.Combine(System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.ProgramFilesX86),
                "GitKraken", "gitkraken.exe")
        };

            // 查找存在的路径（支持通配符）
            foreach (string pattern in possiblePaths)
            {
                if (pattern.Contains("*"))
                {
                    // 处理通配符路径
                    string directory = System.IO.Path.GetDirectoryName(pattern);
                    if (System.IO.Directory.Exists(directory))
                    {
                        var files = System.IO.Directory.GetFiles(directory,
                            System.IO.Path.GetFileName(pattern),
                            System.IO.SearchOption.TopDirectoryOnly);
                        if (files.Length > 0)
                        {
                            return files[0];
                        }
                    }
                }
                else if (System.IO.File.Exists(pattern))
                {
                    return pattern;
                }
            }

            // 尝试从注册表查找
            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\gitkraken.exe"))
                {
                    if (key != null)
                    {
                        string path = key.GetValue("") as string;
                        if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                        {
                            return path;
                        }
                    }
                }
            }
            catch { }
        }
        else if (Application.platform == RuntimePlatform.OSXEditor ||
                 Application.platform == RuntimePlatform.OSXPlayer)
        {
            // macOS 路径
            string[] macPaths = new[]
            {
            "/Applications/GitKraken.app/Contents/MacOS/GitKraken",
            System.IO.Path.Combine(System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.UserProfile),
                "Applications", "GitKraken.app", "Contents", "MacOS", "GitKraken")
        };

            foreach (string path in macPaths)
            {
                if (System.IO.File.Exists(path))
                {
                    return path;
                }
            }
        }

        return null;
    }

    private static string GetGitHubDesktopPath()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // Windows 路径
            string appDataPath = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.LocalApplicationData);
            return System.IO.Path.Combine(appDataPath, "GitHubDesktop", "GitHubDesktop.exe");
        }
        else if (Application.platform == RuntimePlatform.OSXEditor ||
                 Application.platform == RuntimePlatform.OSXPlayer)
        {
            // macOS 路径
            return "/Applications/GitHub Desktop.app/Contents/MacOS/GitHub Desktop";
        }

        return null;
    }

}
