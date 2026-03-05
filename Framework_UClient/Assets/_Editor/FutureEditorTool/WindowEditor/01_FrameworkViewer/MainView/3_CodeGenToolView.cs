/****************************************************
    文件: CodeGenToolView.cs
    作者: Clear
    日期: 2026/3/6
    类型: 工具脚本
    功能: 代码生成工具视图
*****************************************************/
using UnityEngine;
using UnityEditor;
using FutureCore;

namespace FutureEditor
{
    public static class CodeGenToolView
    {
        public static void InitData()
        {
            // 初始化数据（如果需要）
        }

        public static void OnGUI(float contentHeight, float windowWidth)
        {
            GUILayout.BeginArea(new Rect(5, 25, windowWidth - 10, contentHeight), new GUIStyle("grey_border"));
            GUILayout.BeginArea(new Rect(10, 5, windowWidth - 40, contentHeight - 10));
            GUILayout.BeginVertical();

            GUILayout.Label("📝 MVC代码生成工具", EditorStyles.boldLabel, GUILayout.Height(30));
            GUILayout.Space(15);

            // 单列：MVC工具
            float colWidth = 300;

            GUILayout.BeginVertical(GUILayout.Width(colWidth));
            GUILayout.BeginVertical("box");
            GUILayout.Label("MVC代码生成器", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("UI驱动类型:", GUILayout.Width(100));
            GUILayout.Label(FutureCore.AppConst.UIDriver.ToString(), GUILayout.Width(colWidth - 120));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("创建GUI_MVC代码模版", GUILayout.Height(40), GUILayout.Width(colWidth - 20)))
            {
                MVC_CreadTool.OpenGUICread();
                Close();
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();

            GUILayout.EndVertical();
            GUILayout.EndArea();
            GUILayout.EndArea();
        }

        private static void Close()
        {
            // 这个方法会被按钮调用，需要获取窗口实例
            // 实际使用时会通过参数传递
        }
    }
}