using UnityEngine;
using UnityEditor;

namespace FutureEditor
{
    public static class PathToolView
    {
        public static void InitData()
        {
            // 初始化数据（如果需要）
        }

        public static void OnGUI(System.Action closeAction)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("📁 路径工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Unity 安装路径
            GUILayout.BeginVertical("box");
            GUILayout.Label("Unity 安装路径", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorApplication.applicationContentsPath, GUILayout.Width(400));
            if (GUILayout.Button("复制", GUILayout.Width(50), GUILayout.Height(30)))
            {
                EditorGUIUtility.systemCopyBuffer = EditorApplication.applicationContentsPath;
                Debug.Log("路径已复制到剪贴板");
            }
            if (GUILayout.Button("打开", GUILayout.Width(50), GUILayout.Height(30)))
            {
                Application.OpenURL(EditorApplication.applicationContentsPath);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            // 项目资源路径
            GUILayout.BeginVertical("box");
            GUILayout.Label("项目资源路径", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(Application.dataPath, GUILayout.Width(400));
            if (GUILayout.Button("复制", GUILayout.Width(50), GUILayout.Height(30)))
            {
                EditorGUIUtility.systemCopyBuffer = Application.dataPath;
                Debug.Log("路径已复制到剪贴板");
            }
            if (GUILayout.Button("打开", GUILayout.Width(50), GUILayout.Height(30)))
            {
                Application.OpenURL(Application.dataPath);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            // 持久化数据路径
            GUILayout.BeginVertical("box");
            GUILayout.Label("持久化数据路径", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(Application.persistentDataPath, GUILayout.Width(400));
            if (GUILayout.Button("复制", GUILayout.Width(50), GUILayout.Height(30)))
            {
                EditorGUIUtility.systemCopyBuffer = Application.persistentDataPath;
                Debug.Log("路径已复制到剪贴板");
            }
            if (GUILayout.Button("打开", GUILayout.Width(50), GUILayout.Height(30)))
            {
                Application.OpenURL(Application.persistentDataPath);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            // 临时缓存路径
            GUILayout.BeginVertical("box");
            GUILayout.Label("临时缓存路径", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(Application.temporaryCachePath, GUILayout.Width(400));
            if (GUILayout.Button("复制", GUILayout.Width(50), GUILayout.Height(30)))
            {
                EditorGUIUtility.systemCopyBuffer = Application.temporaryCachePath;
                Debug.Log("路径已复制到剪贴板");
            }
            if (GUILayout.Button("打开", GUILayout.Width(50), GUILayout.Height(30)))
            {
                Application.OpenURL(Application.temporaryCachePath);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            // 控制台输出路径
            GUILayout.BeginVertical("box");
            GUILayout.Label("控制台输出路径", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            string consolePath = System.IO.Path.Combine(Application.dataPath, "../Logs");
            GUILayout.BeginHorizontal();
            GUILayout.Label(consolePath, GUILayout.Width(400));
            if (GUILayout.Button("复制", GUILayout.Width(50), GUILayout.Height(30)))
            {
                EditorGUIUtility.systemCopyBuffer = consolePath;
                Debug.Log("路径已复制到剪贴板");
            }
            if (GUILayout.Button("打开", GUILayout.Width(50), GUILayout.Height(30)))
            {
                if (System.IO.Directory.Exists(consolePath))
                {
                    Application.OpenURL(consolePath);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "日志文件夹不存在", "确定");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            // 工程根目录
            GUILayout.BeginVertical("box");
            GUILayout.Label("工程根目录", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            string projectPath = System.IO.Path.GetDirectoryName(Application.dataPath);
            GUILayout.BeginHorizontal();
            GUILayout.Label(projectPath, GUILayout.Width(400));
            if (GUILayout.Button("复制", GUILayout.Width(50), GUILayout.Height(30)))
            {
                EditorGUIUtility.systemCopyBuffer = projectPath;
                Debug.Log("路径已复制到剪贴板");
            }
            if (GUILayout.Button("打开", GUILayout.Width(50), GUILayout.Height(30)))
            {
                Application.OpenURL(projectPath);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.EndVertical();
        }
    }
}