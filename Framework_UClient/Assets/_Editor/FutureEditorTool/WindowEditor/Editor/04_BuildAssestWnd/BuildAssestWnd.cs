using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using FutureCore;
using System.IO;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using FutureEditor;

namespace FutureEditor
{
    public class BuildAssetBundleWnd : EditorWindow
    {
        [MenuItem("GameObject/[Window]/BuildAssetBundle", false, -100)]
        private static void GoOpenFrameworkWin()
        {
            OpenWnd();
        }

        [MenuItem("Assets/[Window]/BuildAssetBundle", false, -100), MenuItem("Assets/[Window]", false, -100)]
        private static void AssOpenFrameworkWin()
        {
            OpenWnd();
        }
        [MenuItem("[FC Window]/打包AssetBundle窗口", false, -100)]
        public static void OpenWnd()
        {
            CreateWindow<BuildAssetBundleWnd>("Build AssetBundle Window");
        }
        private static BuildAssetBundleWnd Instance;
        public static void ShowNotificationTips(string hintString)
        {
            if (Instance != null)
            {
                Instance.ShowNotification(new GUIContent(hintString));
            }
        }

        private void OnEnable()
        {
            Instance = this;
            minSize = new Vector2(900, 510);
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
        private bool showFoldout = false;

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
                abConfig.isCopyStreamingAssets = true;
                abConfig.isImputVersion = true;
                abConfig.isIncrementalBulie = true;
                abConfig.isAutoAddVersion = true;
            }
            versionPath = abConfig.verifyPath.Replace("/version.json", string.Empty);


            //m_ShowExtraFields = new AnimBool(false);
            //m_ShowExtraFields.speed = 10;
            ////监听重绘
            //m_ShowExtraFields.valueChanged.AddListener(Repaint);

            if (abConfig.abRoot != null)
            {
                abRoot = AssetDatabase.LoadAssetAtPath<DefaultAsset>(abConfig.abRoot);

            }

            SlideVal = EditorPrefs.GetFloat(SlideValKey, 0.5f);

            RestAbRoodData();
        }
        private string SlideValKey = "BuildAssetBundleWndSlideVal";


        private enum ShowType
        {
            AllPag = 0,
            BuildSetting

        }

        private void OnGUI()
        {

            toolbatIndex = GUILayout.Toolbar(toolbatIndex, toolbatVal);
            switch (toolbatIndex)
            {

                case (int)ShowType.AllPag:
                    Refresh_AllPag();
                    break;
                case (int)ShowType.BuildSetting:
                    RefreshABConfig();
                    break;


            }


        }



        private void GetPackName(DirectoryInfo directory, Dictionary<string, DirectoryData> AllDic, bool isAdd = false)
        {
            bool isNeedFile = false;
            foreach (var item in directory.GetDirectories())
            {
                isNeedFile = true;
                GetPackName(item, AllDic, true);
            }
            if (!isAdd)
            {
                return;
            }
            if (isNeedFile)
            {
                return;
            }
            FileInfo[] fileInfos = directory.GetFiles();
            if (fileInfos.Length == 0)
            {
                return;
            }
            List<FileInfo> fileInfoLst = new List<FileInfo>();
            long size = 0;
            foreach (var file in fileInfos)
            {
                if (file.FullName.EndsWith(".meta"))
                {
                    continue;
                }
                fileInfoLst.Add(file);
                size += file.Length;
            }

            string path = directory.FullName.Replace("\\", "/").Replace(@"\", "/");
            int index = path.IndexOf(abConfig.abRoot) + abConfig.abRoot.Length + 1;
            path = path.Substring(index, path.Length - index);
            DirectoryData directoryData = new DirectoryData();
            directoryData.path = path;
            directoryData.directory = directory;
            directoryData.allFiles = fileInfoLst;
            directoryData.size = size;

            allDicInfoLst.Add(path, directoryData);
        }
        private Vector2 scPot;
        private float SlideVal = 0.5f;


        #region data
        DirectoryInfo abRootInfo;
        private class DirectoryData
        {
            public bool isShow;
            public string path;
            public long size;
            public DirectoryInfo directory;
            public List<FileInfo> allFiles;
        }

        private Dictionary<string, DirectoryData> allDicInfoLst = new Dictionary<string, DirectoryData>();

        private DirectoryData currSelectDirectoryData;
        #endregion

        #region com
        private float maxVal;
        #endregion

        private void Refresh_AllPag()
        {


            GUILayout.BeginArea(new Rect(10, 480, 880, 30), GUI.skin.GetStyle("FrameBox"));
            SlideVal = EditorGUILayout.Slider(SlideVal, 0.3f, 0.7f, GUILayout.Height(20));

            GUILayout.EndArea();

            //右上
            GUILayout.BeginArea(new Rect(minSize.x * SlideVal, 30, minSize.x * (1 - SlideVal) - 10, 300), GUI.skin.GetStyle("FrameBox"));

            GUILayout.BeginVertical();


            GUILayout.BeginHorizontal();
            DefaultAsset pathObj = abRoot as DefaultAsset;
            if (pathObj != null)
            {
                string path = AssetDatabase.GetAssetPath(pathObj);
                if (abConfig.abRoot != path)
                {
                    abConfig.abRoot = path;
                    RestAbRoodData();
                }
                showFoldout = EditorGUILayout.Foldout(showFoldout, "资源文件夹(Asset):");
            }
            else
            {
                GUILayout.Label(" 资源文件夹(Asset):", GUI.skin.GetStyle("IN TitleText"), GUILayout.Width(155));
                showFoldout = false;
                abConfig.abRoot = string.Empty;
            }

            abRoot = EditorGUILayout.ObjectField(abRoot, typeof(DefaultAsset), true);
            GUILayout.EndHorizontal();

            if (showFoldout)
            {
                EditorGUI.indentLevel++; //缩进深度增加，以下的GUI会增加缩进

                EditorGUILayout.TextField(abConfig.abRoot, GUILayout.Height(20));

                EditorGUI.indentLevel--; //缩进深度减少，以下的GUI会减少缩进
            }
            EditorGUILayout.HelpBox("   以文件夹为标准资源包打包,不允许文件夹和文件共存,如有共存会忽略文件!", MessageType.Info);

            if (GUILayout.Button("刷新路劲"))
            {
                RestAbRoodData();
            }
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打开资源目录"))
            {
                Application.OpenURL(Application.dataPath + "/../" + abConfig.abRoot);
            }
            if (GUILayout.Button("打开导出目录"))
            {
                Application.OpenURL(abConfig.outputPath);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("删除打包记录"))
            {

                if (File.Exists(abConfig.verifyPath))
                {
                    File.Delete(abConfig.verifyPath);
                }

                if (File.Exists(abConfig.allBeDependPath))
                {
                    File.Delete(abConfig.allBeDependPath);
                }

                if (File.Exists(abConfig.allDependPath))
                {
                    File.Delete(abConfig.allDependPath);
                }
                AssetDatabase.Refresh();

            }
            GUILayout.EndHorizontal();
            GUILayout.Space(60);
            if (GUILayout.Button("一键打包AB", GUILayout.Height(100)))
            {
                Build();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();


            //右下
            GUILayout.BeginArea(new Rect(minSize.x * SlideVal + 1, 335, minSize.x * (1 - SlideVal) - 12, 145), GUI.skin.GetStyle("grey_border"));
            if (currSelectDirectoryData != null)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(currSelectDirectoryData.path, GUI.skin.GetStyle("IN TitleText"));
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Size:    " + FutureCore.FileUtil.ConvertFileSize(currSelectDirectoryData.size), GUI.skin.GetStyle("IN TitleText"));
                EditorGUILayout.LabelField("File Cont:    " + currSelectDirectoryData.allFiles.Count, GUI.skin.GetStyle("IN TitleText"));

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(18);
                GUILayout.Label("AB Path:", GUI.skin.GetStyle("IN TitleText"));
                EditorGUILayout.TextField(currSelectDirectoryData.directory.FullName, GUILayout.Width(300));
                GUILayout.Space(5);
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

            }

            GUILayout.EndArea();



            //左
            GUILayout.BeginArea(new Rect(10, 30, minSize.x * SlideVal - 20, 450), GUI.skin.GetStyle("GameViewBackground"));


            if (abConfig.abRoot == null || !Directory.Exists(abConfig.abRoot))
            {
                GUILayout.BeginArea(new Rect(80, 130, 300, 400));
                GUILayout.Box("请选择资源文件夹", GUI.skin.GetStyle("NotificationBackground"), GUILayout.Height(110));
                GUILayout.EndArea();
            }
            else
            {
                scPot = GUILayout.BeginScrollView(scPot);
                GUILayout.BeginVertical();

                foreach (var item in allDicInfoLst)
                {
                    bool isShow = item.Value.isShow;

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    bool isSelect = false;
                    if (!isShow)
                    {
                        isSelect = true;
                    }
                    isShow = EditorGUILayout.Foldout(isShow, item.Key, true, EditorStyles.foldout);
                    if (isShow && isSelect)
                    {
                        currSelectDirectoryData = item.Value;
                    }

                    int val = item.Key.Length - 55;
                    if (val < 0)
                    {
                        val = 0;
                    }
                    else
                    {
                        val = val * 8;
                    }
                    GUILayout.Space(290 + val);
                    GUILayout.Label(FutureCore.FileUtil.ConvertFileSize(item.Value.size));
                    EditorGUILayout.EndHorizontal();
                    if (isShow)
                    {
                        foreach (var file in item.Value.allFiles)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(25);
                            GUILayout.Label(file.Name);
                            EditorGUILayout.LabelField(FutureCore.FileUtil.ConvertFileSize(file.Length));
                            GUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                    item.Value.isShow = isShow;
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }




            GUILayout.EndArea();

        }

        private void Build()
        {
            EditorUtility.SetDirty(abConfig);
            AssetDatabase.SaveAssets();
            ExportABTool.Export(abConfig);
        }

        private void RestAbRoodData()
        {
            abRootInfo = new DirectoryInfo(abConfig.abRoot);
            allDicInfoLst.Clear();
            currSelectDirectoryData = null;
            GetPackName(abRootInfo, allDicInfoLst);
        }

        private void RefreshABConfig()
        {

            GUILayout.BeginArea(new Rect(10, 30, 880, 450), GUI.skin.GetStyle("sv_iconselector_back"));
            GUILayout.BeginVertical();


            GUILayout.Label(" AB包输出目录:", GUI.skin.GetStyle("IN TitleText"));
            GUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel(abConfig.outputPath, EditorStyles.textField, GUILayout.Height(20));

            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                string path = EditorUtility.OpenFolderPanel("选择AB包输出目录", Application.dataPath + @"\..", "").Replace(@"\", "/").Replace(@"\\", "/");
                if (!path.IsNullOrEmpty())
                {
                    abConfig.outputPath = path;
                    Repaint();
                }

            }
            GUILayout.EndHorizontal();



            GUILayout.Label(" 导出AB信息文件夹:", GUI.skin.GetStyle("IN TitleText"));
            GUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel(versionPath, EditorStyles.textField, GUILayout.Height(20));

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
            abConfig.abOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("AB包压缩方式:", abConfig.abOptions, GUILayout.Width(400));

            GUILayout.Space(3);
            abConfig.abPlatform = (BuildTarget)EditorGUILayout.EnumPopup("AB包打包平台:", abConfig.abPlatform, GUILayout.Width(400));

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Imput Version");
            GUILayout.Space(3);
            abConfig.isImputVersion = EditorGUILayout.Toggle(abConfig.isImputVersion);
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Copy to StreamingAssets");
            GUILayout.Space(3);
            abConfig.isCopyStreamingAssets = EditorGUILayout.Toggle(abConfig.isCopyStreamingAssets);
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("增量打包");
            GUILayout.Space(3);
            abConfig.isIncrementalBulie = EditorGUILayout.Toggle(abConfig.isIncrementalBulie);
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("自动追加版本号");
            GUILayout.Space(3);
            abConfig.isAutoAddVersion = EditorGUILayout.Toggle(abConfig.isAutoAddVersion);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();




        }


        private void OnDisable()
        {
            Instance = null;
            //标记目标已被改变数值
            EditorUtility.SetDirty(abConfig);
            AssetDatabase.SaveAssets();
            EditorPrefs.SetFloat(SlideValKey, SlideVal);
        }


    }
}

