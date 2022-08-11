using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class SVNUtils
{
    // 根据电脑具体的盘符来定义
    private static List<string> drives = new List<string>() { "C:", "D:", "E:", "F:", "G:" };
    private static string svnPath = @"\Program Files\TortoiseSVN\bin\";
    private static string svnProc = @"TortoiseProc.exe";
    private static string svnProcPath = "";
    // 需要更新的目录(代码、协议、配置、UI等),根据自己的项目情况来配置好
    private static string Path_SVNProject = "null";


    [MenuItem("SVNTools/更新Assets")]
    public static void UpdateSVNProject()
    {
        UpdateFromSVN(Path_SVNProject);
    }


    private static void UpdateFromSVN(string localFolderPath)
    {
        if (string.IsNullOrEmpty(svnProcPath)) svnProcPath = GetSvnProcPath();
        string updatePath = localFolderPath.Replace("/", "\\");
        string updateCall = "/command:update /path:" + "\"" + updatePath + "\"" + "/closeonend:0";
        System.Diagnostics.Process.Start(svnProcPath, updateCall);
    }

    [MenuItem("SVNTools/提交Assets")]
    public static void UpdateCommitProject()
    {
        CommitToSVN(Path_SVNProject, "提交Assets目录_");
    }


    private static void CommitToSVN(string localFolderPath, string commitMsgTip)
    {
        if (string.IsNullOrEmpty(svnProcPath)) svnProcPath = GetSvnProcPath();
        string updatePath = localFolderPath.Replace("/", "\\");
        string updateCall = "/command:commit /path:" + "\"" + updatePath + "\"" + "/logmsg:" + commitMsgTip + " /closeonend:0";
        System.Diagnostics.Process.Start(svnProcPath, updateCall);
    }

    private static string GetSvnProcPath()
    {
        foreach (string item in drives)
        {
            string path = string.Concat(item, svnPath, svnProc);
            if (File.Exists(path))
            {
                return path;
            }
        }
        return EditorUtility.OpenFilePanel("Select TortoiseProc.exe", "C:\\", "exe");
    }
}
