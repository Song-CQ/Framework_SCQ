using UnityEngine;
using UnityEditor;
using FutureCore;

namespace FutureEditor
{
    public static class ExcelToolView
    {
        private static int buildOutType;
        private static bool isEnciphermentData = false;
        private static bool isOutMultipleDatas = false;

        public static void InitData()
        {
            buildOutType = EditorPrefs.GetInt("ExcelTool_BuildOutType", 0);
            isEnciphermentData = EditorPrefs.GetBool("ExcelTool_IsEnciphermentData", false);
            isOutMultipleDatas = EditorPrefs.GetBool("ExcelTool_IsOutMultipleDatas", false);
        }

        public static void OnGUI(System.Action closeAction)
        {
            GUILayout.Space(5);

            ConfigBatTool.BuildOutType type = (ConfigBatTool.BuildOutType)EditorGUILayout.EnumPopup("[生成Class类型]:", (ConfigBatTool.BuildOutType)buildOutType, GUILayout.Width(400));
            buildOutType = (int)type;
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("[加密表数据]");
            GUILayout.Space(5);
            isEnciphermentData = EditorGUILayout.Toggle(isEnciphermentData);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("[每张表单独生成数据文件]");
            GUILayout.Space(5);
            isOutMultipleDatas = EditorGUILayout.Toggle(isOutMultipleDatas);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("自动打表", GUILayout.Height(40), GUILayout.Width(100)))
            {
                EditorPrefs.SetInt("ExcelTool_BuildOutType", buildOutType);
                EditorPrefs.SetBool("ExcelTool_IsEnciphermentData", isEnciphermentData);
                EditorPrefs.SetBool("ExcelTool_IsOutMultipleDatas", isOutMultipleDatas);

                closeAction?.Invoke();
                ConfigBatTool.SyncConfigData(type, isEnciphermentData, isOutMultipleDatas);
            }

            if (GUILayout.Button("打开表格目录", GUILayout.Height(40), GUILayout.Width(100)))
            {
                ConfigBatTool.OpenExcelPath();
            }
        }
    }
}