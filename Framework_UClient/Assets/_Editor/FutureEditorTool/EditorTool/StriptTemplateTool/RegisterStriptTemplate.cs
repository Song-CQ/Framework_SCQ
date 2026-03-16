/************************************************************
    文件: ScriptsInfoRecoder.cs
	作者: 承清
    邮箱: 2728285639@qq.com
    日期: 2018/10/13 12:01
	功能: 记录脚本信息
*************************************************************/

using FutureCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FutureEditor
{
    public class RegisterStriptTemplate : UnityEditor.AssetModificationProcessor
    {
        
        private static void OnWillCreateAsset(string path)
        { 
            path = path.Replace(".meta", "");
            if (!Directory.Exists(path))
            {
                return;
            }
            if (path.EndsWith(".cs"))
            {
                string str = File.ReadAllText(path);
                str = str.Replace("#CreateAuthor#", Environment.UserName).Replace(
                                  "#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
                File.WriteAllText(path, str);
            }
        }
        /// <summary>
        /// 复制模板文件到Unity安装目录
        /// </summary>
        public static void StartRegisterTemplate()
        {
            string prth = EditorApplication.applicationContentsPath + "/Resources/ScriptTemplates/";
            string srcPath = Application.dataPath + "/_Editor/FutureEditorTool/EditorTool/StriptTemplateTool/Template";
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i.Name.EndsWith(".meta")) continue;
                    if (!(i is DirectoryInfo))     //判断是否文件夹
                    {
                        File.Copy(i.FullName, prth + i.Name, true);
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.LogError("注册C#模版出错");
                throw e;
            }
            LogUtil.Log("[FC Register]注册C#脚本模版完成");
            AssetDatabase.Refresh();
        }

        private class NameByEnterOrUnfocus : EndNameEditAction
        {
            /// <summary>
            /// 当用户通过按下 Enter 键或失去键盘输入焦点接受编辑后的名称时，Unity 调用此函数
            /// </summary>
            /// <param name="instanceId">已编辑资源的实例 ID。</param>
            /// <param name="pathName">资源的路径。</param>
            /// <param name="resourceFile">传递给ProjectWindowUtil.StartNameEditingIfProjectWindowExists的资源文件字符串参数</param>
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var obj = CreateScript(pathName, resourceFile);
                // 创建并展示
                ProjectWindowUtil.ShowCreatedAsset(obj);
            }

            private static Object CreateScript(string pathName, string resourceFile)
            {
                // 读取模板文件内容
                var streamReader = new StreamReader(resourceFile);
                var templateText = streamReader.ReadToEnd();
                streamReader.Close();

                // 获取创建的脚本文件名称
                var fileName = Path.GetFileNameWithoutExtension(pathName);

                // 正则替换文本内自定义的变量 
                var scriptText = templateText.Replace("#SCRIPTNAME#", fileName).
                    Replace("#CreateAuthor#", Environment.UserName)
                    .Replace("#CreateTime#", TimerUtil.GetLacalTimeYMD_HHMMSS());
                // 写入脚本
                var streamWriter = new StreamWriter(pathName);
                streamWriter.Write(scriptText);
                streamWriter.Close();

                // 在路径导入资源
                AssetDatabase.ImportAsset(pathName);
                // 返回给定路径assetPath类型类型的第一个资源对象
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            }
        }

        // 创建文件夹的EndNameEditAction类
        private class CreateFolderEndNameEdit : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                // 创建文件夹
                Directory.CreateDirectory(pathName);
                // 刷新AssetDatabase
                AssetDatabase.Refresh();
                
                // 选中创建的文件夹
                var obj = AssetDatabase.LoadAssetAtPath<Object>(pathName);
                if (obj != null)
                {
                    ProjectWindowUtil.ShowCreatedAsset(obj);
                    Selection.activeObject = obj;
                }
            }
        }

        private enum CreateScriptType
        {
            //101-200 .cs
            NewClass =101,
            NewScript ,
            NewCoreClass
      
        }

        private static Dictionary<CreateScriptType, string> createScriptTypeDic = new Dictionary<CreateScriptType, string> {
            {CreateScriptType.NewClass,"Assets/_Editor/FutureEditorTool/EditorTool/StriptTemplateTool/Template/31-[FC]C# Class-NewClass.cs.txt"},
            {CreateScriptType.NewScript,"Assets/_Editor/FutureEditorTool/EditorTool/StriptTemplateTool/Template/32-[FC]C# Script-NewScript.cs.txt"},
            {CreateScriptType.NewCoreClass,"Assets/_Editor/FutureEditorTool/EditorTool/StriptTemplateTool/Template/33-[FC]C# Core-NewClass.cs.txt"},
        };

        [MenuItem("Assets/[FC] Create/NewClass",priority = 1)]
        private static void CreateNewClass()
        {
            CreateScript(CreateScriptType.NewClass);
        }
        [MenuItem("Assets/[FC] Create/NewScript", priority = 2)]
        private static void CreateNewScript()
        {
            CreateScript(CreateScriptType.NewScript);
        }
        [MenuItem("Assets/[FC] Create/NewCoreClass", priority = 3)]
        private static void CreateMMSctr()
        {
            CreateScript(CreateScriptType.NewCoreClass);
        }

        // ==================== 新增：创建文件夹功能 ====================
        [MenuItem("Assets/[FC] Create/NewFolder", priority = 4)]
        private static void CreateNewFolder()
        {
            const int instanceId = 0;
            var endAction = ScriptableObject.CreateInstance<CreateFolderEndNameEdit>();

            // 获取当前选中的路径
            string selectedPath = GetSelectedPath();
            string folderName = "NewFolder";
            string pathName = Path.Combine(selectedPath, folderName);

            // 如果路径已存在，添加数字后缀
            int counter = 1;
            while (Directory.Exists(pathName) || File.Exists(pathName))
            {
                pathName = Path.Combine(selectedPath, folderName + " " + counter);
                counter++;
            }

            // 获取文件夹图标
            Texture2D folderIcon = EditorGUIUtility.FindTexture("Folder Icon");

            // 开始编辑名称
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                instanceId, 
                endAction, 
                pathName, 
                folderIcon,
                null);
        }

        // ==================== 新增：创建多级文件夹功能 ====================
        [MenuItem("Assets/[FC] Create/Create Folders From String", priority = 5)]
        private static void CreateFoldersFromString()
        {
            // 显示输入对话框
            CreateFolderWindow.ShowWindow();
        }

        // ==================== 新增：快速创建常用文件夹结构 ====================
        [MenuItem("Assets/[FC] Create/Create Default Folder Structure", priority = 6)]
        private static void CreateDefaultFolderStructure()
        {
            string selectedPath = GetSelectedPath();
            
            // 创建默认的文件夹结构
            string[] defaultFolders = new string[]
            {
                "Scripts",
                "Scenes",
                "Prefabs",
                "Textures",
                "Materials",
                "Audio",
                "Animations",
                "Fonts",
                "Resources",
                "Plugins",
                "Editor"
            };

            foreach (string folder in defaultFolders)
            {
                string folderPath = Path.Combine(selectedPath, folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[FC] 默认文件夹结构创建完成在: {selectedPath}");
        }

        // ==================== 辅助方法 ====================
        private static string GetSelectedPath()
        {
            // 获取当前选中的路径
            string selectedPath = "Assets";
            
            Object[] selections = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if (selections.Length > 0)
            {
                string path = AssetDatabase.GetAssetPath(selections[0]);
                if (Directory.Exists(path))
                {
                    selectedPath = path;
                }
                else
                {
                    selectedPath = Path.GetDirectoryName(path);
                }
            }
            
            return selectedPath;
        }

        // 检查路径是否有效
        private static bool IsValidFolderPath(string path)
        {
            try
            {
                // 检查是否包含非法字符
                char[] invalidChars = Path.GetInvalidPathChars();
                if (path.IndexOfAny(invalidChars) >= 0)
                    return false;

                // 检查是否以点开头
                string folderName = Path.GetFileName(path);
                if (folderName.StartsWith("."))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void CreateScript(CreateScriptType File)
        {
            if (!createScriptTypeDic.ContainsKey(File))
            {
                return;
            }
            string resourceFilePath = createScriptTypeDic[File];

            const int instanceId = 0;

            var endAction = ScriptableObject.CreateInstance<NameByEnterOrUnfocus>();     

            string extension = Path.GetExtension(resourceFilePath.Replace(".txt",string.Empty));
            string FileName = File.ToString() + extension;

            Object[] objects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            var pathName = "Assets/" + FileName;
            if (objects.Length != 0)
            {
                pathName = AssetDatabase.GetAssetPath(objects[0]) + "/" + FileName;               
            }
            Texture2D texture2D = null;
            
            switch (extension)
            {
                case ".cs":
                    texture2D = EditorGUIUtility.FindTexture("cs Script Icon");
                    break;
                case ".shader":
                    texture2D = EditorGUIUtility.FindTexture("Shader Script Icon");
                    break;
                default:
                    texture2D = EditorGUIUtility.FindTexture("TextAsset Icon");
                    break;
            }

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(instanceId, endAction, pathName, texture2D,resourceFilePath);
        }
    }

    // ==================== 新增：创建文件夹的窗口类 ====================
    public class CreateFolderWindow : EditorWindow
    {
        private string folderPath = "";
        private string folderStructure = "";

        public static void ShowWindow()
        {
            CreateFolderWindow window = GetWindow<CreateFolderWindow>(true, "创建文件夹结构");
            window.minSize = new Vector2(400, 200);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            
            // 当前选中的路径
            EditorGUILayout.LabelField("当前路径:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(GetCurrentSelectedPath());
            
            GUILayout.Space(10);
            
            // 输入文件夹路径
            EditorGUILayout.LabelField("输入文件夹路径 (用/或\\分隔):", EditorStyles.boldLabel);
            folderStructure = EditorGUILayout.TextField(folderStructure);
            
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox("例如: Scripts/UI/Panels\n或者: Textures/Characters/Player", MessageType.Info);
            
            GUILayout.Space(20);

            // 按钮区域
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = !string.IsNullOrEmpty(folderStructure);
            if (GUILayout.Button("创建", GUILayout.Height(30)))
            {
                CreateFolders();
                Close();
            }
            GUI.enabled = true;
            
            if (GUILayout.Button("取消", GUILayout.Height(30)))
            {
                Close();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private string GetCurrentSelectedPath()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (Directory.Exists(path))
                {
                    break;
                }
                else
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }

        private void CreateFolders()
        {
            string basePath = GetCurrentSelectedPath();
            string[] folders = folderStructure.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            
            string currentPath = basePath;
            foreach (string folder in folders)
            {
                currentPath = Path.Combine(currentPath, folder);
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[FC] 文件夹结构创建完成: {Path.Combine(basePath, folderStructure)}");
        }
    }
}