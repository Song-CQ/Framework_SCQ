using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using FutureCore;

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

        private ABConfig abConfig;

        private string versionPath;

        private string[] toolbatVal;
        private int toolbatIndex = 0;
        private void InitData()
        {
            toolbatVal = new string[System.Enum.GetValues(typeof(ShowType)).Length];
            System.Collections.IList list = System.Enum.GetValues(typeof(ShowType));
            for (int i = 0; i < list.Count; i++)
            {
                ShowType t = (ShowType)list[i];
                toolbatVal[i] = t.ToString();
            }

            string abConfigPath = UnityEditorPathConst.ABConfigPatn_Assest + "/ABConfig.asset";
            abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
            if (abConfig == null)
            {
                ScriptableObjectTool.CreadScriptableObject<ABConfig>(UnityEditorPathConst.ABConfigPatn_Assest);
                abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
                abConfig.outputPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "AssetBundle";
                abConfig.abRoot = Application.dataPath+ "/_AssetBundleRes";
                abConfig.verifyPath = UnityEditorPathConst.ABConfigPatn_Assest + "/version.json";
                abConfig.allBeDependPath = UnityEditorPathConst.ABConfigPatn_Assest + "/allBeDependData.json";
                abConfig.allDependPath = UnityEditorPathConst.ABConfigPatn_Assest + "/allDependData.json";
                abConfig.abOptions = BuildAssetBundleOptions.None;
                abConfig.abPlatform = BuildTarget.StandaloneWindows;
            }
            versionPath = abConfig.verifyPath.Replace("/version.json",string.Empty);
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
            abConfig.abRoot = EditorGUILayout.TextField("资源文件夹(Asset)", abConfig.abRoot);
            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                string path = EditorUtility.OpenFolderPanel("选择资源文件夹", Application.dataPath, "").Replace(@"\", "/").Replace(@"\\", "/");
                if (path.Contains(Application.dataPath))
                {
                    abConfig.abRoot = path;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void RefreshABConfig()
        {
            
            GUILayout.BeginArea(new Rect(10, 30, 880, 450),GUI.skin.GetStyle("sv_iconselector_back"));
            GUILayout.BeginVertical();





            GUILayout.Label(" 导出AB信息文件夹:",  GUI.skin.GetStyle("IN TitleText"));

            GUILayout.BeginHorizontal();
            versionPath = GUILayout.TextField(versionPath, GUILayout.Height(20),GUILayout.MinWidth(300));

            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                string path = EditorUtility.OpenFolderPanel("选择AB信息文件夹", Application.dataPath, "").Replace(@"\", "/").Replace(@"\\", "/");
                if (path.Contains(Application.dataPath))
                {
                    versionPath = path;
                }
            }
            abConfig.verifyPath = versionPath + "/version.json";
            abConfig.allBeDependPath = versionPath + "/allBeDependData.json";
            abConfig.allDependPath = versionPath + "/allDependData.json";
            GUILayout.EndHorizontal();



            GUILayout.EndVertical();
            GUILayout.EndArea();




        }

       


    }
}

