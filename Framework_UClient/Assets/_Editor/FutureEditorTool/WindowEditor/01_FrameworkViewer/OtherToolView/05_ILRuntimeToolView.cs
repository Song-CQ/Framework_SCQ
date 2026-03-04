using UnityEngine;
using UnityEditor;
using FutureCore;
using System.IO;

namespace FutureEditor
{
    public static class ILRuntimeToolView
    {
        private static bool showFoldout_ILRuntime = true;
        private static bool showFoldout2_ILRuntime = false;
        private static bool showFoldout3_ILRuntime = true;
        private static bool isDelClient_ILRuntime = false;
        private static bool isCread_BatFile = false;
        private static ILRuntimeMgr_AutoCreator.CompileCodePlan compileCodePlan = ILRuntimeMgr_AutoCreator.CompileCodePlan.MsBuild;
        private static string MsBuildPath = string.Empty;

        public static void InitData()
        {
            MsBuildPath = EditorPrefs.GetString("MsBuildPath", string.Empty);
        }

        public static void OnGUI(System.Action closeAction)
        {
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
                compileCodePlan = (ILRuntimeMgr_AutoCreator.CompileCodePlan)EditorGUILayout.EnumPopup("编译Dll的方案:", compileCodePlan, GUILayout.Width(300));
                if (compileCodePlan != ILRuntimeMgr_AutoCreator.CompileCodePlan.CompileAssembly_UnityEditor)
                {
                    GUILayout.Space(5);
                    isCread_BatFile = GUILayout.Toggle(isCread_BatFile, "是否重新创建.bat文件");
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(5);

                if (compileCodePlan == ILRuntimeMgr_AutoCreator.CompileCodePlan.MsBuild)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("MsBuild路径:");
                    string text = MsBuildPath == string.Empty ? @"" : MsBuildPath;
                    EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(20), GUILayout.Width(465));

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
                    
                    if (MsBuildPath == string.Empty)
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
                ILRuntimeMgr_AutoCreator.AutoRegister_HotFix_ILRuntimeMgr(isDelClient_ILRuntime, isCread_BatFile, compileCodePlan, MsBuildPath, closeAction);
            }

            GUILayout.Space(10);

            showFoldout3_ILRuntime = EditorGUILayout.Foldout(showFoldout3_ILRuntime, "[热更流程] ---------------------------------------------------------------------------------------------------");

            if (showFoldout3_ILRuntime)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;

                GUILayout.BeginHorizontal();
                GUILayout.Label("(1)", GUILayout.Width(20), GUILayout.Height(40));
                if (GUILayout.Button("[Client Code To Cache] - 提取代码", GUILayout.Height(40), GUILayout.Width(215)))
                {
                    ILRuntimeMgr_AutoCreator.ClientToHotFixClass(true);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    closeAction?.Invoke();
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
                closeAction?.Invoke();
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
                closeAction?.Invoke();
            }
            if (GUILayout.Button("注册跨域继承适配器", GUILayout.Height(40), GUILayout.Width(180)))
            {
                ILRuntimeMgr_AutoCreator.CreateAdapter();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                closeAction?.Invoke();
            }
            GUILayout.EndHorizontal();
        }
    }
}