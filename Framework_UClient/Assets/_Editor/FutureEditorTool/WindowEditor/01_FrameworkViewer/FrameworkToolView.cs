using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using FutureCore;
using System.IO;
using System.Collections.Generic;

namespace FutureEditor
{
    public class FrameworkToolView : EditorWindow
    {
        [MenuItem("GameObject/[Open Framework View]", false, -1000)]
        private static void GoOpenFrameworkWin()
        {
            OpenFrameworkWin();
        }
        [MenuItem("Assets/[Open Framework View]", false, -1000)]
        private static void AssOpenFrameworkWin()
        {
            OpenFrameworkWin();
        }
        [MenuItem("[FC Window]/框架工具窗口", false, -100)]
        public static void OpenFrameworkWin()
        {
            CreateWindow<FrameworkToolView>("Framework Tool");
        }
        
        

        private void OnEnable()
        {
      
            minSize = new Vector2(900, 500);
            maxSize = minSize;
            InitData();
            Show();
            Focus();
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

            OnInitOtherToolData();

        }


        private string[] toolbatVal;
        private int toolbatIndex = 0;
        private void OnGUI()
        {
          
            toolbatIndex = GUILayout.Toolbar(toolbatIndex, toolbatVal);
           
            switch (toolbatIndex)
            {           
                case (int)ShowType.UnityTool:
                    RefreshUI_UnityTool();
                    break;
                case (int)ShowType.CreateTool:
                    RefreshUI_CreateTool();
                    break;
                case (int)ShowType.AutoRegisterTool:
                    RefreshUI_AutoRegisterTool();
                    break; 
                case (int)ShowType.OtherTool:
                    RefreshUI_OtherTool();
                    break;
            }
        }

       

        private enum ShowType
        {
            UnityTool=0,
            GameTool,
            CreateTool,
            AutoRegisterTool,
            OtherTool,
        }
     
        #region UnityTool
        private void RefreshUI_UnityTool()
        {
            
            GUILayout.BeginArea(new Rect(10, 35, 200, 800));
            GUILayout.Label("Unity Editor",  GUILayout.Height(20),GUILayout.Width(80));
            if (GUILayout.Button("重启Unity", GUILayout.Height(40), GUILayout.Width(100)))
            {
                UnityEditorTool.StartRest();
            }     
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(550, 60, 300, 500));
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            GUILayout.Label("AppName:", GUILayout.Width(80));
            GUILayout.Space(30);
            GUILayout.TextField(ProjectApp.AppFacade.AppName);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("AppDesc:", GUILayout.Width(80));
            GUILayout.Space(30);
            GUILayout.TextField(ProjectApp.AppFacade.AppDesc);

            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 235, 200, 800));
            GUILayout.BeginVertical();
            GUILayout.Label("Visual Studio Editor", GUILayout.Height(20));
            if (GUILayout.Button("删除VS解决方案", GUILayout.Height(40), GUILayout.Width(100)))
            {
                UnityEditorTool.DeleteSin();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();


            GUILayout.BeginArea(new Rect(550, 235, 200, 800));
            GUILayout.Label("  Open Path", GUI.skin.GetStyle("IN TitleText"), GUILayout.Height(20));
            GUILayout.BeginVertical(GUI.skin.GetStyle("FrameBox"));

            if (GUILayout.Button("Open UnityInstallPath"))
            {
                Application.OpenURL(EditorApplication.applicationContentsPath);
            }
            if (GUILayout.Button("Open AssetDataPath"))
            {
                Application.OpenURL(Application.dataPath);
            }

            if (GUILayout.Button("Open PersistentDataPath"))
            {
                Application.OpenURL(Application.persistentDataPath);
            }
            GUILayout.Space(3);
            if (GUILayout.Button("Open TemporaryCachePath"))
            {
                Application.OpenURL(Application.temporaryCachePath);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();


           

        }

        
        #endregion

        

        #region CreateTool

        private void RefreshUI_CreateTool()
        {
            GUILayout.BeginArea(new Rect(10, 35, 200, 200));
            GUILayout.Label("MVC (根据UI驱动类型)");
            GUILayout.BeginVertical();

            if (GUILayout.Button("创建GUI_MVC代码模版", GUILayout.Height(40), GUILayout.Width(160)))
            {
                MVC_CreadTool.OpenGUICread();
                Close();
            } 
            
        
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        #endregion

        #region AutoRegisterTool
        private void RefreshUI_AutoRegisterTool()
        {
            GUILayout.BeginArea(new Rect(10, 35, 200, 200));
            if (GUILayout.Button("注册编辑器环境", GUILayout.Height(40), GUILayout.Width(180)))
            {
                EditorAutoRegisterTool_Editor.AutoRegisterAll(Close);
               
            }          
            if (GUILayout.Button("自动注册项目数据", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ProjectAutoRegisterTool.AutoRegisterAll(Close);
               
            }
            
            GUILayout.EndArea();
        }

        #endregion

        #region OtherTool

        private Vector2 otherToolPos;
        private Vector2 otherToolPos2;
        private string selectOtherToolKey;
        private Color selectColor = new Color(100/255f,207/255f,255/255f);

        private Dictionary<string, Action> OtherTooDic = new Dictionary<string, Action>();
        private void RefreshUI_OtherTool()
        {
            
            GUILayout.BeginArea(new Rect(5, 25, 185, 465), new GUIStyle("grey_border"));

            GUILayout.BeginArea(new Rect(0, 2, 182, 460));
            otherToolPos = GUILayout.BeginScrollView(otherToolPos, false, true, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);//grey_border//Dopesheetkeyframe

            
            foreach (var item in OtherTooDic)
            {
                if (selectOtherToolKey==string.Empty)
                {
                    selectOtherToolKey = item.Key;
                }

                GUI.backgroundColor = Color.white;
                if (selectOtherToolKey == item.Key)
                {

                    GUI.backgroundColor = selectColor;

                }

                if (GUILayout.Button(item.Key, GUILayout.Width(160),GUILayout.Height(30)))
                {
                    selectOtherToolKey = item.Key;
                }

                GUI.backgroundColor = Color.white;
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(193, 25, 700, 465), new GUIStyle("FrameBox"));

            otherToolPos2 = GUILayout.BeginScrollView(otherToolPos2);
            GUILayout.Label(selectOtherToolKey, new GUIStyle("CN StatusWarn"));

            OtherTooDic[selectOtherToolKey]?.Invoke();
            GUILayout.EndScrollView();
            GUILayout.EndArea();


          
        }
     
        private void OnInitOtherToolData()
        {
           
            selectOtherToolKey = string.Empty;
            OtherTooDic.Clear();
            OtherTooDic.Add("[Excel Tool]", OnOtherToolView_Excel);
            OtherTooDic.Add("[AssetBundle Tool]", OnOtherToolView_AssetBundle);
            OtherTooDic.Add("[SVN Tool]", OnOtherToolView_Svn);
            OtherTooDic.Add("[ILRuntime Tool]", OnOtherToolView_ILRuntime);

            for (int i = 0; i < 30; i++)
            {

                OtherTooDic.Add("[test]--"+i, OnTest);
            }

            MsBuildPath = EditorPrefs.GetString("MsBuildPath",string.Empty);
        }

        private void OnOtherToolView_Excel()
        {
            if (GUILayout.Button("Dll自动打表", GUILayout.Height(40), GUILayout.Width(100)))
            {
                Close();
                ConfigBatTool.SyncConfig2Dll();

            }
            if (GUILayout.Button("CS文件自动打表", GUILayout.Height(40), GUILayout.Width(100)))
            {
                Close();
                ConfigBatTool.SyncConfig2CS();

            }
            if (GUILayout.Button("Dll自动打表(表数据加密)", GUILayout.Height(40), GUILayout.Width(150)))
            {
                Close();
                ConfigBatTool.SyncConfig2Dll_EncryptData();
            }
            if (GUILayout.Button("CS文件自动打表(表数据加密)", GUILayout.Height(40), GUILayout.Width(180)))
            {
                Close();
                ConfigBatTool.SyncConfig2CS_EncryptData();
            }
        }

        private void OnOtherToolView_AssetBundle()
        {
            if (GUILayout.Button("Open AssetBundles Window", GUILayout.Height(40), GUILayout.Width(180)))
            {
                BuildAssetBundleWnd.OpenWnd();
                Close();
            }
        }
        private void OnOtherToolView_Svn()
        {
            if (GUILayout.Button("更新Assets目录", GUILayout.Height(40), GUILayout.Width(150)))
            {
                Close();
                SVNUtils.UpdateSVNProject_Assets();
            }
            if (GUILayout.Button("提交Assets目录", GUILayout.Height(40), GUILayout.Width(150)))
            {
                Close();
                SVNUtils.CommitProject_Assets();
            }
        }

        private bool showFoldout_ILRuntime = true;
        private bool showFoldout2_ILRuntime = false;
        private bool showFoldout3_ILRuntime = true;

        private bool isDelClient_ILRuntime = false;

        private bool isCread_BatFile = false;

        private ILRuntimeMgr_AutoCreator.CompileCodePlan compileCodePlan = ILRuntimeMgr_AutoCreator.CompileCodePlan.MsBuild;

        public string MsBuildPath = string.Empty;
        private void OnOtherToolView_ILRuntime()
        {
            string ModuleMgrPath = UnityEditorPathConst.AutoRegisterPath + "/ModuleMgr";
            string LogicPath = Application.dataPath + "/_App/ProjectApp/ProjectApp/Logic";





            GUILayout.Space(5);

            showFoldout2_ILRuntime = EditorGUILayout.Foldout(showFoldout2_ILRuntime, "[热更相关路径]");

            if (showFoldout2_ILRuntime)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;


                EditorGUILayout.LabelField("[提取的热更代码路径]", Path.GetFullPath(ILRuntimeMgr_AutoCreator.HotFix_Class_Path));
                EditorGUILayout.LabelField("[存放HotFix.Dll  路径]", @"Assets\StreamingAssets\HotFix");
                GUILayout.Space(5);
                GUILayout.Label("    ---热更文件夹");
                EditorGUILayout.LabelField("[ModuleMgr  路径]", "Assets" + Path.GetFullPath(ILRuntimeMgr_AutoCreator.HotFix_ModuleMgrPath).Replace(Path.GetFullPath(Application.dataPath), ""));
                EditorGUILayout.LabelField("[Logic  路径]", "Assets" + Path.GetFullPath(ILRuntimeMgr_AutoCreator.HotFix_LogicPath).Replace(Path.GetFullPath(Application.dataPath), ""));



                EditorGUI.indentLevel--;

            }
            GUILayout.Space(5);


            showFoldout_ILRuntime = EditorGUILayout.Foldout(showFoldout_ILRuntime, "[热更启动设置]");

            if (showFoldout_ILRuntime)
            {
                GUILayout.Space(5);
              
                EditorGUI.indentLevel++;
                isDelClient_ILRuntime = GUILayout.Toggle(isDelClient_ILRuntime, "被提取的热更代码 是否删除");
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                compileCodePlan = (ILRuntimeMgr_AutoCreator.CompileCodePlan)EditorGUILayout.EnumPopup("编译Dll的方案:",compileCodePlan, GUILayout.Width(300));
                if (compileCodePlan != ILRuntimeMgr_AutoCreator.CompileCodePlan.CompileAssembly_UnityEditor)
                {
                    GUILayout.Space(5);
                    isCread_BatFile = GUILayout.Toggle(isCread_BatFile, "是否重新创建.bat文件");
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                
                if (compileCodePlan ==  ILRuntimeMgr_AutoCreator.CompileCodePlan.MsBuild)
                {

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("MsBuild路径:");
                    string text = MsBuildPath==string.Empty? @"" : MsBuildPath;
                    EditorGUILayout.SelectableLabel(text, EditorStyles.textField,GUILayout.Height(20), GUILayout.Width(465));

                    if (GUILayout.Button("选择", GUILayout.Width(50f)))
                    {
                        string path = EditorUtility.OpenFilePanel("选择MsBuild.exe", Application.dataPath + @"\..", "exe").Replace(@"\", "/").Replace(@"\\", "/");
                        if (!path.IsNullOrEmpty())
                        {
                            MsBuildPath = path;
                            EditorPrefs.SetString("MsBuildPath", MsBuildPath);
                        }

                    }
                    GUILayout.EndHorizontal();
                    if (MsBuildPath==string.Empty)
                    {
                        GUI.color = Color.green;
                        GUILayout.Label(@"      Tips: MsBuild.exe 一般位于vs安装目录的VS2022\Msbuild\Current\Bin\MSBuild.exe");
                        GUI.color = Color.white;
                    }
                    
                }
                
               
              
               
                EditorGUI.indentLevel--;
         
            }
            GUILayout.Space(5);


            if (GUILayout.Button("启动ILRuntime热更流程", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ILRuntimeMgr_AutoCreator.AutoRegister_HotFix_ILRuntimeMgr(isDelClient_ILRuntime, isCread_BatFile, compileCodePlan, MsBuildPath, Close);

            }

            GUILayout.Space(10);

            showFoldout3_ILRuntime = EditorGUILayout.Foldout(showFoldout3_ILRuntime, "[热更流程] ---------------------------------------------------------------------------------------------------");

            if (showFoldout3_ILRuntime)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;

                GUILayout.BeginHorizontal();
                GUILayout.Label("(1)", GUILayout.Width(20),GUILayout.Height(40));
                if (GUILayout.Button("[Client Code To Cache] - 提取代码", GUILayout.Height(40), GUILayout.Width(215)))
                {
                    ILRuntimeMgr_AutoCreator.ClientToHotFixClass(true);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Close();
                }
              
               
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("(2)", GUILayout.Width(20), GUILayout.Height(40));
                if (GUILayout.Button("[Register Code] - 注册热更代码 ", GUILayout.Height(40), GUILayout.Width(200)))
                {
                    ILRuntimeMgr_AutoCreator.RegisterCode();
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("(3)", GUILayout.Width(20), GUILayout.Height(40));

                if (GUILayout.Button("[Compile Code To Dll] - 编译Dll", GUILayout.Height(40), GUILayout.Width(200)))
                {
                    ILRuntimeMgr_AutoCreator.CompileCodeToDll(isCread_BatFile, compileCodePlan, MsBuildPath);
                }
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
               

            }
            GUILayout.Space(10);
            GUILayout.Label("[流程工具]---------------------------------------------------------------------------------------------------");
        

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("载入热更文件夹缓存代码", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ILRuntimeMgr_AutoCreator.HotFixClassToClient();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Close();
            }
        
            if (GUILayout.Button("删除热更文件夹缓存代码", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ILRuntimeMgr_AutoCreator.Delete_HotFixClass_Cache();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10); 
            GUILayout.Label("[其他工具]---------------------------------------------------------------------------------------------------");
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("分析热更DLL生成CLR绑定", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ILRuntimeMgr_AutoCreator.GenerateCLRBindingByAnalysis();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Close();
            }
            if (GUILayout.Button("注册跨域继承适配器", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ILRuntimeMgr_AutoCreator.CreateAdapter();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Close();
            }

            
            GUILayout.EndHorizontal();


        }

        #endregion

        #region Test

        private float v1;
        private float v4;
        private float v2;
        private float v3;


        private void OnTest()
        {
            GUILayout.BeginArea(new Rect(450, 200, 450, 500));
            GUILayout.Label(v1.ToString());
            GUILayout.Label(v2.ToString());
            GUILayout.Label(v3.ToString());
            GUILayout.Label(v4.ToString());
            v1 = GUILayout.HorizontalSlider(v1, 0, 1000);
            GUILayout.Space(10);
            v2 = GUILayout.HorizontalSlider(v2, 0, 1000);
            GUILayout.Space(10);
            v3 = GUILayout.HorizontalSlider(v3, 0, 1000);
            GUILayout.Space(10);
            v4 = GUILayout.HorizontalSlider(v4, 0, 1000);
            GUILayout.Space(10);
            GUILayout.EndArea();
        }

        #endregion
    }
}

