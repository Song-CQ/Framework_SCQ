using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using FutureCore;
using System.IO;
using UnityEditor.AnimatedValues;

namespace FutureEditor
{
    public class BuildAssetBundleWnd : EditorWindow
    {
        [MenuItem("GameObject/[Window]/BuildAssetBundle", false, -100)]
        private static void GoOpenFrameworkWin()
        {
            OpenWnd();
        }

        [MenuItem("Assets/[Window]/BuildAssetBundle", false, -100),MenuItem("Assets/[Window]",false,-100)]
        private static void AssOpenFrameworkWin()
        {
            OpenWnd();
        }
        [MenuItem("[FC Window]/打包AssetBundle窗口", false, -100)]
        public static void OpenWnd()
        {
            CreateWindow<BuildAssetBundleWnd>("Build AssetBundle Window");
        }

        private void OnEnable()
        {
      
            minSize = new Vector2(900, 500);
            maxSize = minSize;
            InitData();
            Show();
            Focus();
        }



        private string abConfigPath;
        private ABConfig abConfig;

        private string versionPath;


        private AnimBool m_ShowExtraFields;
        UnityEngine.Object abRoot = null;
        private string[] toolbatVal;
        private int toolbatIndex = 0;
        private bool showFoldout=false;

        private void InitData()
        {
            toolbatVal = new string[System.Enum.GetValues(typeof(ShowType)).Length];
            System.Collections.IList list = System.Enum.GetValues(typeof(ShowType));
            for (int i = 0; i < list.Count; i++)
            {
                ShowType t = (ShowType)list[i];
                toolbatVal[i] = t.ToString();
            }

            abConfigPath = UnityEditorPathConst.ABConfigPatn_Assest + "/ABConfig.asset";
            abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
            if (abConfig == null)
            {
                ScriptableObjectTool.CreadScriptableObject<ABConfig>(UnityEditorPathConst.ABConfigPatn_Assest);
                abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
                abConfig.outputPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "AssetBundle";
                abConfig.abRoot = "_AssetBundleRes";
                abConfig.verifyPath = UnityEditorPathConst.ABConfigPatn_Assest + "/version.json";
                abConfig.allBeDependPath = UnityEditorPathConst.ABConfigPatn_Assest + "/allBeDependData.json";
                abConfig.allDependPath = UnityEditorPathConst.ABConfigPatn_Assest + "/allDependData.json";
                abConfig.abOptions = BuildAssetBundleOptions.None;
                abConfig.abPlatform = BuildTarget.StandaloneWindows;
            }
            versionPath = abConfig.verifyPath.Replace("/version.json",string.Empty);


            m_ShowExtraFields = new AnimBool(false);
            m_ShowExtraFields.speed = 10;
            //监听重绘
            m_ShowExtraFields.valueChanged.AddListener(Repaint);

            if (abConfig.abRoot!=null)
            {
                abRoot = AssetDatabase.LoadAssetAtPath<DefaultAsset>(abConfig.abRoot);

            }

        }

        

        private enum ShowType
        {
            AllPag=0,
            Build 

        }

        private void OnGUI()
        {           

            toolbatIndex = GUILayout.Toolbar(toolbatIndex, toolbatVal);
            switch (toolbatIndex)
            {

                case (int)ShowType.AllPag:
                    Refresh_AllPag();
                    break;
                case (int)ShowType.Build:
                    RefreshABConfig();
                    break;
            
             
            }


        }
       
        private void Refresh_AllPag()
        {
            //左
            GUILayout.BeginArea(new Rect(10, 30, 420, 450), GUI.skin.GetStyle("GameViewBackground"));



            

            GUILayout.BeginScrollView( Vector2.zero);

           

            GUILayout.EndScrollView();


            GUILayout.EndArea();
        

            //右
            GUILayout.BeginArea(new Rect(450,30,440,450),GUI.skin.GetStyle("FrameBox"));
            
            GUILayout.BeginVertical();


           
            GUILayout.BeginHorizontal();
       
            
            DefaultAsset pathObj = abRoot as DefaultAsset;
            if (pathObj != null)
            {
                string path = AssetDatabase.GetAssetPath(pathObj);
                abConfig.abRoot = path;
                showFoldout = EditorGUILayout.Foldout(showFoldout, "资源文件夹(Asset):");
                m_ShowExtraFields.target = showFoldout;
            }
            else
            {
                GUILayout.Label(" 资源文件夹(Asset):", GUI.skin.GetStyle("IN TitleText"), GUILayout.Width(155));
                m_ShowExtraFields.target = false;
            }

            abRoot = EditorGUILayout.ObjectField(abRoot, typeof(DefaultAsset), true);

            GUILayout.EndHorizontal();

            if (showFoldout)
            {
                EditorGUI.indentLevel++; //缩进深度增加，以下的GUI会增加缩进

                EditorGUILayout.TextField(abConfig.abRoot, GUILayout.Height(20));

                EditorGUI.indentLevel--; //缩进深度减少，以下的GUI会减少缩进
            }







            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void RefreshABConfig()
        {
            
            GUILayout.BeginArea(new Rect(10, 30, 880, 450),GUI.skin.GetStyle("sv_iconselector_back"));
            GUILayout.BeginVertical();


            GUILayout.Label(" AB包输出目录:", GUI.skin.GetStyle("IN TitleText"));
            GUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel(abConfig.outputPath,EditorStyles.textField,GUILayout.Height(20));

            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                string path = EditorUtility.OpenFolderPanel("选择AB包输出目录", Application.dataPath+@"\..", "").Replace(@"\", "/").Replace(@"\\", "/");
                if (!path.IsNullOrEmpty())
                {
                    abConfig.outputPath = path;
                    Repaint();
                }
            
            }
            GUILayout.EndHorizontal();



            GUILayout.Label(" 导出AB信息文件夹:",  GUI.skin.GetStyle("IN TitleText"));
            GUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel(versionPath,EditorStyles.textField, GUILayout.Height(20));

            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                string path = EditorUtility.OpenFolderPanel("选择AB信息文件夹", Application.dataPath, "").Replace(@"\", "/").Replace(@"\\", "/");
                if (path.Contains(Application.dataPath))
                {
                    versionPath = path;
                    Repaint();
                }
            }
            abConfig.verifyPath = versionPath + "/version.json";
            abConfig.allBeDependPath = versionPath + "/allBeDependData.json";
            abConfig.allDependPath = versionPath + "/allDependData.json";
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            abConfig.abOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("AB包压缩方式:", abConfig.abOptions,GUILayout.Width(400));
            
            GUILayout.Space(3);
            abConfig.abPlatform = (BuildTarget)EditorGUILayout.EnumPopup("AB包打包平台:", abConfig.abPlatform, GUILayout.Width(400));

            GUILayout.EndVertical(); 
            GUILayout.EndArea();




        }

        private void OnDisable()
        {
            //标记目标已被改变数值
            EditorUtility.SetDirty(abConfig);
            AssetDatabase.SaveAssets();
        }


    }
}

