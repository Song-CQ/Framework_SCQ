/****************************************************
    文件：ReStartUnityTool.cs
	作者：Clear
    日期：2022/1/8 15:43:15
	功能：Unity编辑器工具
*****************************************************/

using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement; 
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FutureEditor
{
    public static class UnityEditorTool
    {

        #region 重启Unity

        /// <summary>
        /// 重启Unity 
        /// </summary>
        [MenuItem("[FC Tool]/Unity Editor/重启Unity", false, -1000)]
        private static void StartRestMenu()
        {
            StartRest();
        }

        public static void StartRest(string val = "")
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
            pStartInfo.UseShellExecute = true;
            pStartInfo.RedirectStandardError = false;
            pStartInfo.RedirectStandardInput = false;
            pStartInfo.RedirectStandardOutput = false;
            if (!string.IsNullOrEmpty(workingDir))
            {
                pStartInfo.WorkingDirectory = workingDir;
            }
            return System.Diagnostics.Process.Start(pStartInfo);
        }
        /// <summary>
        /// 删除解决方案
        /// </summary>
        /// <returns></returns>
        public static bool DeleteSin()
        {

            string[] strings = Directory.GetFiles(Application.dataPath + "/../", "*.sln", SearchOption.TopDirectoryOnly);
            if (strings.Length <= 0)
            {
                EditorUtility.DisplayDialog("删除失败!", "请确认是否解决方案和项目名一致或文件不存在!", "确定");
                return false;
            }
            bool isError = false;
            foreach (var path in strings)
            {
                string Project_Sin_Path = path;
                if (!FutureCore.FileUtil.DeleteFileOrDirectory(Project_Sin_Path))
                {
                    isError = true;
                    LogUtil.LogError("删除解决方案失败：" + Project_Sin_Path);
                }

            }
            if (!isError)
            {
                EditorUtility.DisplayDialog("删除完成!", "请重新打开VS,生成解决方案!", "确定");
            }
            return !isError;

        }

        /// <summary>
        /// 设置项目解决方案名字和项目文件夹名字
        /// </summary>
        public static void SetSinNameAndDirName()
        {
            
            string m_UnityPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string m_AssetPath = Application.dataPath + "/..";
            string m_BatPath = Path.GetFullPath(Application.dataPath + @"\..\..\_Tool\UnityTool\SetProjectName_设置项目名字.bat");

            string m_newpath = Path.GetFullPath(Application.dataPath + @"\..\..\" + ProjectApp.AppFacade.AppName+ "_UClient");
            

            CreateProcess(m_BatPath, string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\" ", m_UnityPath, m_AssetPath, ProjectApp.AppFacade.AppName, m_newpath));





        }
        /// <summary>
        /// 关闭vs编辑器和Unity
        /// </summary>
        public static void CloseVSIDE_Untiy(System.Action vb)
        {
            try
            {
                string val = @"E:\SetUp\VS2022\Common7\IDE\devenv.exe";
                Process[] processes = Process.GetProcesses();
                System.Collections.Generic.List<Process> temp = new System.Collections.Generic.List<Process>();
                foreach (var item in processes)
                {
                    ProcessModule module = item.MainModule;
                    if (module.FileName.EndsWith(@"Common7\IDE\devenv.exe"))
                    {
                        
                        temp.Add(item);
                    }
                }

                vb?.Invoke();
                foreach (var item in temp)
                {
                    //本来只想关闭vs 但是不知道为啥同时关闭了untiy 
                    item.CloseMainWindow();
                }
                //本来只想关闭vs 但是不知道为啥同时关闭了untiy 
                Process.GetCurrentProcess().Kill();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("关闭VS失败:"+e);

            }
           
        }

        #endregion



        #region Assets
        /// <summary>
        /// 高亮选中路劲 Assets资源
        /// </summary>
        /// <param name="path"></param>
        public static void SelectObject_Assets(string path)
        {
            Object obj = AssetDatabase.LoadMainAssetAtPath(path);
            SelectObject_Assets(obj);
        }
        /// <summary>
        /// 高亮选中路劲
        /// </summary>
        /// <param name="path"></param>
        public static void SelectObject_Assets(Object obj)
        {            
            if (obj == null) return;

            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
        #endregion

    }
}