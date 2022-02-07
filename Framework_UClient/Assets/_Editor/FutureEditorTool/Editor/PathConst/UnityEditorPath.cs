/****************************************************
    文件：UnityEditorPath.cs
	作者：Clear
    日期：2022/2/7 16:5:28
    类型: 框架核心脚本(请勿修改)
	功能：编辑器路劲常量
*****************************************************/
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public class UnityEditorPath : ScriptableObject
    {

        public const string AsssetPath = @"Assets\Editor\UnityEditorPath";

        private static UnityEditorPath m_instance;
        public static UnityEditorPath Instance
        {
            get
            {
                if (m_instance == null)
                {
                    string path = AsssetPath + @"\UnityEditorPath.asset";
                    m_instance = AssetDatabase.LoadAssetAtPath<UnityEditorPath>(path);
                    if (m_instance == null)
                    {
                        Debug.LogError(path + "文件不存在");
                    }
                }  
                return m_instance;
            }
        }

        

        [Header("存放自动生成文件的目录")]
        public DefaultAsset AutoCreadPath;
        [Header("存放MVC模块的目录")]
        public DefaultAsset ModuleUIPath;   
        [Header("存放FGUI生成代码的目录")]
        public DefaultAsset FGUIClassPath;
        [Header("存放FGUI的目录")]
        public DefaultAsset ResFGUIPath;

        



    }
}