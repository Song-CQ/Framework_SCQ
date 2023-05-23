using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class SVNUtils
{
    // ���ݵ��Ծ�����̷�������
    private static List<string> drives = new List<string>() { "C:", "D:", "E:", "F:", "G:" };
    private static string svnPath = @"\Program Files\TortoiseSVN\bin\";
    private static string svnProc = @"TortoiseProc.exe";
    private static string svnProcPath = "";
    // ��Ҫ���µ�Ŀ¼(���롢Э�顢���á�UI��),�����Լ�����Ŀ��������ú�
    private static string Path_SVNProject = "null";


    [MenuItem("[FC Tool]/SVNTools/����Assets")]
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

    [MenuItem("[FC Tool]/SVNTools/�ύAssets")]
    public static void UpdateCommitProject()
    {
        CommitToSVN(Path_SVNProject, "�ύAssetsĿ¼_");
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