/****************************************************
    文件：ReStartUnityTool.cs
	作者：Clear
    日期：2022/1/8 15:43:15
	功能：重启Unity
*****************************************************/

using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FutureEditor
{
    public static class RestStartUnityTool
    {
        /// <summary>
        /// 重启Unity 
        /// </summary>
        [MenuItem("[FC Editor]/重启Unity", false, -1000)]
        private static void StartRestMenu()
        {
            StartRest();
        }

        public static void StartRest(string val="")
        {
            val = val == "" ? "是否重启Unity!" : val;
            if (EditorUtility.DisplayDialog("重启Unity", val, "确认", "取消"))
            {
                EditorApplication.isPlaying = false;
                //保存
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", saveAsCopy: false);
                //开始
                Start();
            }
           
        }


        private static void Start()
        {
            string m_UnityPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string m_AssetPath = Application.dataPath + "/..";
            string m_BatPath = Path.GetFullPath(Application.dataPath + @"\..\..\_Tool\UnityTool\StartUnity_启动项目.bat");

            CreateProcess(m_BatPath, string.Format("\"{0}\" \"{1}\" ", m_UnityPath, m_AssetPath));
            Process.GetCurrentProcess().Kill();
        }

        //创建cmd
        public static Process CreateProcess(string cmd, string args, string workingDir = "")
        {
            var pStartInfo = new System.Diagnostics.ProcessStartInfo(cmd);
            pStartInfo.Arguments = args;
            pStartInfo.CreateNoWindow = false;
            pStartInfo.UseShellExecute = false;
            pStartInfo.RedirectStandardError = false;
            pStartInfo.RedirectStandardInput = false;
            pStartInfo.RedirectStandardOutput = false;
            if (!string.IsNullOrEmpty(workingDir))
            {
                pStartInfo.WorkingDirectory = workingDir;
            }
            return System.Diagnostics.Process.Start(pStartInfo);
        }

    }
}