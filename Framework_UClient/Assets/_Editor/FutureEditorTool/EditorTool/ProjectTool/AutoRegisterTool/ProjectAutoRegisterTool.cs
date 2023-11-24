/****************************************************
    文件：ProjectAutoRegisterTool.cs
	作者：Clear
    日期：2022/2/7 15:39:39
    类型: 框架核心脚本(请勿修改)
	功能：主动注册项目工具
*****************************************************/
using FutureCore;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class ProjectAutoRegisterTool 
    {



        [MenuItem("[FC Tool]/AutoRegister Tool/自动注册项目", false, 31)]
        public static void AutoRegisterAll()
        {
            AutoRegisterAll(null);
        }
        public static void AutoRegisterAll(Action cb)
        {
            if (EditorUtility.DisplayDialog("自动注册项目", "是否进行自动注册项目", "确认", "取消"))
            {
                cb?.Invoke();   
                Debug.Log("[ProjectAutoRegisterTool]自动注册项目开始");
                Debug.Log("------------------------------------------自注册开始----------------------------------------------------------------");
                RegisterAutoAssetInAllDirectory();
                Debug.Log("------------------------------------------自注册完毕----------------------------------------------------------------");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }      
        }
        [MenuItem("[FC Tool]/AutoRegister Tool/自动注册(热更)", false, 32)]
        public static void AutoRegisterAll_HotFix()
        {
            AutoRegisterAll_HotFix(null);
        }
        public static void AutoRegisterAll_HotFix(Action cb)
        {
            if (EditorUtility.DisplayDialog("自动注册项目(热更)", "是否进行自动注册项目", "确认", "取消"))
            {
                cb?.Invoke();
                Debug.Log("[ProjectAutoRegisterTool]自动注册项目开始(热更)");
                Debug.Log("------------------------------------------自注册开始----------------------------------------------------------------");
                RegisterAutoAssetInAllDirectory_HotFix();
                Debug.Log("------------------------------------------自注册完毕----------------------------------------------------------------");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }      
        }


        private static void RegisterAutoAssetInAllDirectory()
        {
            UI_AutoCreator.AutoRegister();
            ModuleMgr_AutoCreator.AutoRegister();
        }  
        private static void RegisterAutoAssetInAllDirectory_HotFix()
        {
            ILRuntimeMgr_AutoCreator.CreateAdapter();
            ILRuntimeMgr_AutoCreator.CreateVersionData();
            UI_AutoCreator.AutoRegister_HotFix();
            ModuleMgr_AutoCreator.AutoRegister_HotFix();
            
        }  
        
        


        

    }
}