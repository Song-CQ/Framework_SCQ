/****************************************************
    文件：AutoRegisterTool.cs
	作者：Clear
    日期：2022/1/8 17:36:22
	功能：注册编辑器工具
*****************************************************/

using System;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class EditorAutoRegisterTool_Editor
    {
        [MenuItem("[FC Tool]/AutoRegister Tool/注册编辑器环境", false, 30)]
        public static void AutoRegisterAll()
        {
            AutoRegisterAll(null);
        }
        public static void AutoRegisterAll(Action cb)
        {
            if (EditorUtility.DisplayDialog("注册编辑器环境", "是否注册编辑器环境", "确认", "取消"))
            {

                cb?.Invoke();

                Debug.Log("[EditorEnvironmentAutoRegisterTool]自动注册环境开始");
                Debug.Log("------------------------------------------自注册开始----------------------------------------------------------------");
                RegisterStriptTemplate.StartRegisterTemplate();
                

                Debug.Log("------------------------------------------自注册完毕----------------------------------------------------------------");

                
                if (EditorUtility.DisplayDialog("修改项目文件夹", "是否要重新生成解决方案?", "确认", "取消"))
                {
                    EditorApplication.isPlaying = false;
                    //删除解决方案
                    UnityEditorTool.DeleteSin();
                    //设置项目解决方案名字和项目文件夹名字
                    UnityEditorTool.SetSinNameAndDirName();
                }
                else
                {
                    UnityEditorTool.StartRest("注册环境完成!\n需重启Unity编辑器");
                }

                
            }
        }

    }
}