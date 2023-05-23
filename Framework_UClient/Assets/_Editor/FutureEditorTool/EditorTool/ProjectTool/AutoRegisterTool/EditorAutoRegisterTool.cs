/****************************************************
    文件：AutoRegisterTool.cs
	作者：Clear
    日期：2022/1/8 17:36:22
	功能：Nothing
*****************************************************/

using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class EditorAutoRegisterTool_Editor
    {
        [MenuItem("[FC Project]/AutoRegister/注册编辑器环境", false, -3)]
        public static void AutoRegisterAll()
        {
            if (EditorUtility.DisplayDialog("注册编辑器环境", "是否注册编辑器环境", "确认", "取消"))
            {
                Debug.Log("[EditorEnvironmentAutoRegisterTool]自动注册环境开始");
                Debug.Log("------------------------------------------自注册开始----------------------------------------------------------------");
                RegisterStriptTemplate.StartRegisterTemplate();



                Debug.Log("------------------------------------------自注册完毕----------------------------------------------------------------");

                UnityEditorTool.StartRest("注册环境完成!\n需重启Unity编辑器");
            }
        }

    }
}