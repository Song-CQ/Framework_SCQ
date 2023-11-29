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

        [MenuItem("Assets/[FC Window]/打包AssetBundle窗口", false, -100), MenuItem("GameObject/[FC Window]/打包AssetBundle窗口", false, -100)]
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
            minSize = new Vector2(900, 520);
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


        private void SetABConfigData()
        {
            ABConfig temp = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
            abConfig = new ABConfig();
            if (temp == null)
            {
                abConfig.outputPath = Path.GetFullPath(Application.dataPath + "/../../_Resources/UpData/AssetBundles");
                abConfig.abRoot = "Assets/_Res/AssetBundleRes";
                abConfig.verifyPath = UnityEditorPathConst.ABConfigPatn_Assest + "/version.json";
                abConfig.allBeDependPath = UnityEditorPathConst.ABConfigPatn_Assest + "/allBeDependData.json";
                abConfig.allDependPath = UnityEditorPathConst.ABConfigPatn_Assest + "/allDependData.json";
                abConfig.abOptions = BuildAssetBundleOptions.None;
                abConfig.abPlatform = BuildTarget.StandaloneWindows;
                abConfig.isImputVersion = true;
                abConfig.isCopyStreamingAssets = false;
                abConfig.isIncrementalBulie = false;
                abConfig.isAutoAddVersion = true;
            }
            else
            {
                abConfig.outputPath = temp.outputPath;
                abConfig.abRoot = temp.abRoot;
                abConfig.verifyPath = temp.verifyPath;
                abConfig.allBeDependPath = temp.allBeDependPath;
                abConfig.allDependPath = temp.allDependPath;
                abConfig.abOptions = temp.abOptions;
                abConfig.abPlatform = temp.abPlatform;
                abConfig.isImputVersion = temp.isImputVersion;
                abConfig.isCopyStreamingAssets = temp.isCopyStreamingAssets;
                abConfig.isIncrementalBulie = temp.isIncrementalBulie;
                abConfig.isAutoAddVersion = temp.isAutoAddVersion;
            }

        }

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
            SetABConfigData();

            versionPath = abConfig.verifyPath.Replace("/version.json", string.Empty);


            //m_ShowExtraFields = new AnimBool(false);
            //m_ShowExtraFields.speed = 10;
            ////监听重绘
            //m_ShowExtraFields.valueChanged.AddListener(Repaint);

            if (abConfig.abRoot != null)
            {
                abRoot = AssetDatabase.LoadAssetAtPath<DefaultAsset>(abConfig.abRoot);

            }

            RestAbRoodData();
        }
        //private string SlideValKey = "BuildAssetBundleWndSlideVal";


        private enum ShowType
        {
            AllPag = 0,
            BuildSetting

        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            toolbatIndex = GUILayout.Toolbar(toolbatIndex, toolbatVal);

            GUILayout.EndHorizontal();
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
            List<Texture2D> allFileTexture2D = new List<Texture2D>();

            long size = 0;
            foreach (var file in fileInfos)
            {
                if (file.FullName.EndsWith(".meta"))
                {
                    continue;
                }
                fileInfoLst.Add(file);
                string assetsPath = Path.GetFullPath(file.FullName).Replace("\\", "/").Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                Texture2D texture2D = AssetDatabase.GetCachedIcon(assetsPath) as Texture2D;
                allFileTexture2D.Add(texture2D);
                size += file.Length;
            }

            string path = directory.FullName.Replace("\\", "/").Replace(@"\", "/").Replace(Application.dataPath, "Assets");
            int index = path.IndexOf(abConfig.abRoot) + abConfig.abRoot.Length + 1;
            path = path.Substring(index, path.Length - index);
            DirectoryData directoryData = new DirectoryData();
            directoryData.path = path;
            directoryData.directory = directory;
            directoryData.allFiles = fileInfoLst;
            directoryData.allFilesTexture2D = allFileTexture2D;
            directoryData.size = size;


            allDicInfoLst.Add(path, directoryData);
        }
        private Vector2 scPot;

        #region data
        DirectoryInfo abRootInfo;
        private class DirectoryData
        {
            public bool isShow;
            public string path;
            public long size;
            public DirectoryInfo directory;
            public List<FileInfo> allFiles;
            public List<Texture2D> allFilesTexture2D;
        }

        private Dictionary<string, DirectoryData> allDicInfoLst = new Dictionary<string, DirectoryData>();

        private DirectoryData currSelectDirectoryData;
        #endregion    

        Rect m_HorizontalSplitterRect;
        Rect m_VerticalSplitterRect;
        bool m_ResizingHorizontalSplitter;
        bool m_ResizingVerticalSplitter;
        [SerializeField]
        float m_HorizontalSplitterPercent = 0.5f;
        [SerializeField]
        float m_VerticalSplitterPercent = 0.7f;
        private void HandleHorizontalResize()
        {
            m_HorizontalSplitterRect.x = (int)(position.width * m_HorizontalSplitterPercent);
            m_HorizontalSplitterRect.y = 30;
            m_HorizontalSplitterRect.height = position.height - 40;
            m_HorizontalSplitterRect.width = 3;

            EditorGUIUtility.AddCursorRect(m_HorizontalSplitterRect, MouseCursor.ResizeHorizontal);
            if (Event.current.type == EventType.MouseDown && m_HorizontalSplitterRect.Contains(Event.current.mousePosition))
                m_ResizingHorizontalSplitter = true;

            if (m_ResizingHorizontalSplitter)
            {
                m_HorizontalSplitterPercent = Mathf.Clamp(Event.current.mousePosition.x / position.width, 0.3f, 0.7f);
                m_HorizontalSplitterRect.x = (int)(this.maxSize.x * m_HorizontalSplitterPercent);
            }

            if (Event.current.type == EventType.MouseUp)
                m_ResizingHorizontalSplitter = false;

            //EditorGUIUtility.DrawColorSwatch(m_HorizontalSplitterRect, Color.white);
        }

        private void HandleVerticalResize()
        {
            m_VerticalSplitterRect.x = m_HorizontalSplitterRect.x + 5;
            m_VerticalSplitterRect.y = (int)((position.height - 30) * m_VerticalSplitterPercent) + 30;
            m_VerticalSplitterRect.height = 3;
            m_VerticalSplitterRect.width = position.width - m_HorizontalSplitterRect.x - 10;

            EditorGUIUtility.AddCursorRect(m_VerticalSplitterRect, MouseCursor.ResizeVertical);
            if (Event.current.type == EventType.MouseDown && m_VerticalSplitterRect.Contains(Event.current.mousePosition))
                m_ResizingVerticalSplitter = true;

            if (m_ResizingVerticalSplitter)
            {
                m_VerticalSplitterPercent = Mathf.Clamp(Event.current.mousePosition.y / position.height, 0.2f, 0.8f);
                m_VerticalSplitterRect.y = (int)((this.maxSize.y - 30) * m_VerticalSplitterPercent) + 30;
            }

            if (Event.current.type == EventType.MouseUp)
                m_ResizingVerticalSplitter = false;

            //EditorGUIUtility.DrawColorSwatch(m_VerticalSplitterRect, new Color(1,1,1,0.5f));

        }

        private void Refresh_AllPag()
        {

            HandleHorizontalResize();
            HandleVerticalResize();



            //右上
            GUILayout.BeginArea(new Rect(m_HorizontalSplitterRect.x + 5, 30, position.width - m_HorizontalSplitterRect.x - 10, m_VerticalSplitterRect.y - 30), GUI.skin.GetStyle("FrameBox"));

            GUILayout.BeginVertical();


            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            abRoot = EditorGUILayout.ObjectField(abRoot, typeof(DefaultAsset), true);
            if (GUILayout.Button(EditorGUIUtility.FindTexture("Refresh"), GUILayout.Width(30)))
            {
                RestAbRoodData();
            }
            if (EditorGUI.EndChangeCheck())
            {
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
            }


            GUILayout.EndHorizontal();

            if (showFoldout)
            {
                GUILayout.Space(3);
                EditorGUI.indentLevel++; //缩进深度增加，以下的GUI会增加缩进

                EditorGUILayout.TextField(abConfig.abRoot, GUILayout.Height(20));

                EditorGUI.indentLevel--; //缩进深度减少，以下的GUI会减少缩进
            }
            GUILayout.Space(3);
            EditorGUILayout.HelpBox("   以文件夹为标准资源包打包,不允许文件夹和文件共存,如有共存会忽略文件!", MessageType.Info);

            if (GUILayout.Button("恢复初始化设置"))
            {
                DestroyImmediate(abConfig, true);
                InitData();
                Repaint();
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


            GUILayout.FlexibleSpace();
            if (GUILayout.Button("一键打包AB", GUILayout.Height(100)))
            {
                Build();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();


            //右下
            GUILayout.BeginArea(new Rect(m_HorizontalSplitterRect.x + 5, m_VerticalSplitterRect.y + 3, position.width - m_HorizontalSplitterRect.x - 12, position.height - m_VerticalSplitterRect.y - 13), GUI.skin.GetStyle("grey_border"));
            if (currSelectDirectoryData != null)
            {


                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(currSelectDirectoryData.path, GUI.skin.GetStyle("IN TitleText"));
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Size:    " + StringConvertUnit.ConvertFileSize(currSelectDirectoryData.size), GUI.skin.GetStyle("IN TitleText"));
                EditorGUILayout.LabelField("File Cont:    " + currSelectDirectoryData.allFiles.Count, GUI.skin.GetStyle("IN TitleText"));

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(18);
                GUILayout.Label("AB Path:", GUI.skin.GetStyle("IN TitleText"));

                GUILayout.Space(5);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.TextField(currSelectDirectoryData.directory.FullName.Replace("\\", "/").Replace(@"\", "/").Replace(Application.dataPath, "Assets"));

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

            }

            GUILayout.EndArea();



            //左
            GUILayout.BeginArea(new Rect(10, 30, m_HorizontalSplitterRect.x - 10, 480), GUI.skin.GetStyle("GameViewBackground"));



            if (abConfig.abRoot == null || !Directory.Exists(abConfig.abRoot))
            {
                GUILayout.BeginArea(new Rect((m_HorizontalSplitterRect.x - 10) / 2 - 150, 130, 300, 400));
                GUILayout.Box("请选择资源文件夹", GUI.skin.GetStyle("NotificationBackground"));
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
                    GUILayout.Space(300 + val);
                    GUILayout.Label(StringConvertUnit.ConvertFileSize(item.Value.size));
                    EditorGUILayout.EndHorizontal();
                    if (isShow)
                    {
                        for (int i = 0; i < item.Value.allFiles.Count; i++)
                        {

                            FileInfo file = item.Value.allFiles[i];
                            Texture2D texture2D = item.Value.allFilesTexture2D[i];
                            GUILayout.BeginHorizontal();

                            Rect rect = GUILayoutUtility.GetRect(15, 15);
                            rect.width = 15;
                            rect.height = 15;
                            rect.y += 4;
                            rect.x += 5;
                            GUI.DrawTexture(rect, texture2D);
                            GUILayout.Space(10);
                            GUILayout.Label(file.Name);
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField(StringConvertUnit.ConvertFileSize(file.Length));
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button(EditorGUIUtility.IconContent("d_CanvasRenderer Icon")))
                            {
                                string assetsPath = Path.GetFullPath(file.FullName).Replace("\\", "/").Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                                UnityEditorTool.SelectObject_Assets(assetsPath);
                            }
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

            if (m_ResizingHorizontalSplitter || m_ResizingVerticalSplitter)
            {
                Repaint();
            }


        }


        private void Build()
        {
            SaveABConfigData();
            ExportABTool.Export(abConfig);
        }

        private void RestAbRoodData()
        {
            if (abConfig.abRoot.IsNullOrEmpty()) return;

            abRootInfo = new DirectoryInfo(abConfig.abRoot);
            allDicInfoLst.Clear();
            currSelectDirectoryData = null;
            GetPackName(abRootInfo, allDicInfoLst);
        }

        private void RefreshABConfig()
        {

            GUILayout.BeginArea(new Rect(10, 30, 880, 480), GUI.skin.GetStyle("sv_iconselector_back"));
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
            EditorGUI.BeginChangeCheck();



            EditorGUILayout.SelectableLabel(versionPath, EditorStyles.textField, GUILayout.Height(20));

            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                string path = EditorUtility.OpenFolderPanel("选择AB信息文件夹", Application.dataPath, "").Replace(@"\", "/").Replace(@"\\", "/");
                if (path.Contains(Application.dataPath))
                {
                    versionPath = path;

                }
            }


            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            BuildAssetBundleOptions abOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("AB包压缩方式:", abConfig.abOptions, GUILayout.Width(400));

            GUILayout.Space(3);
            BuildTarget abPlatform = (BuildTarget)EditorGUILayout.EnumPopup("AB包打包平台:", abConfig.abPlatform, GUILayout.Width(400));

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Imput Version");
            GUILayout.Space(3);
            bool isImputVersion = EditorGUILayout.Toggle(abConfig.isImputVersion);
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Copy to StreamingAssets");
            GUILayout.Space(3);
            bool isCopyStreamingAssets = EditorGUILayout.Toggle(abConfig.isCopyStreamingAssets);
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("增量打包");
            GUILayout.Space(3);
            bool isIncrementalBulie = EditorGUILayout.Toggle(abConfig.isIncrementalBulie);
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("自动追加版本号");
            GUILayout.Space(3);
            bool isAutoAddVersion = EditorGUILayout.Toggle(abConfig.isAutoAddVersion);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();

            if (EditorGUI.EndChangeCheck())
            {
                abConfig.verifyPath = versionPath + "/version.json";
                abConfig.allBeDependPath = versionPath + "/allBeDependData.json";
                abConfig.allDependPath = versionPath + "/allDependData.json";
                abConfig.abOptions = abOptions;
                abConfig.abPlatform = abPlatform;
                abConfig.isImputVersion = isImputVersion;
                abConfig.isCopyStreamingAssets = isCopyStreamingAssets;
                abConfig.isIncrementalBulie = isIncrementalBulie;
                abConfig.isAutoAddVersion = isAutoAddVersion;
                SaveABConfigData();
                Repaint();
            }


        }

        private void SaveABConfigData()
        {
            ABConfig temp = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
            if (temp == null)
            {
                ScriptableObjectTool.CreadScriptableObject<ABConfig>(UnityEditorPathConst.ABConfigPatn_Assest);
                temp = AssetDatabase.LoadAssetAtPath<ABConfig>(abConfigPath);
            }

            temp.outputPath = abConfig.outputPath;
            temp.abRoot = abConfig.abRoot;
            temp.verifyPath = abConfig.verifyPath;
            temp.allBeDependPath = abConfig.allBeDependPath;
            temp.allDependPath = abConfig.allDependPath;
            temp.abOptions = abConfig.abOptions;
            temp.abPlatform = abConfig.abPlatform;
            temp.isImputVersion = abConfig.isImputVersion;
            temp.isCopyStreamingAssets = abConfig.isCopyStreamingAssets;
            temp.isIncrementalBulie = abConfig.isIncrementalBulie;
            temp.isAutoAddVersion = abConfig.isAutoAddVersion;
            //标记目标已被改变数值
            EditorUtility.SetDirty(temp);
            AssetDatabase.SaveAssets();
        }

        private void OnDisable()
        {
            SaveABConfigData();
            Instance = null;
        }




    }
}

