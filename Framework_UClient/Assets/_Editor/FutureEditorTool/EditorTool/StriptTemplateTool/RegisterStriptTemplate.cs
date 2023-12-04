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
        }[MenuItem("Assets/[FC] Create/NewCoreClass", priority = 3)]
        private static void CreateMMSctr()
        {
            CreateScript(CreateScriptType.NewCoreClass);
        }

        private static void CreateScript(CreateScriptType File)
        {
#if false
                * 参数1：instanceId       已编辑资源的实例 ID。
                * 参数2：endAction        监听编辑名称的类的实例化
                * 参数3：pathName         创建的文件路径（包括文件名）
                * 参数4：icon             图标  
                * 参数5：resourceFile     模板路径

                endAction 直接使用 new NameByEnterOrUnfocus() 出现以下警告：
                    NameByEnterOrUnfocus must be instantiated using the ScriptableObject.CreateInstance method instead of new NameByEnterOrUnfocus.
                    必须使用ScriptableObject实例化NameByEnterOrUnfocus。CreateInstance方法，而不是新的NameByEnterOrUnfocus。
#endif
          


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

   

    

}