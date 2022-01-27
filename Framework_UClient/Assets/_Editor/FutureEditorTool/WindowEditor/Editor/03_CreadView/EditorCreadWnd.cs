/****************************************************
    文件：EditorCreadWnd.cs
	作者：Clear
    日期：2022/1/26 17:53:9
    类型: 框架核心脚本(请勿修改)
	功能：编辑器创建窗口
*****************************************************/
using System;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public class EditorCreadWnd: EditorWindow
    {
        private string creadName;

        private string tipsStr;

        private Func<string,string> complete;

        public static void ShowWindow(string title,Func<string, string> onFunc)
        {
            EditorCreadWnd window = GetWindow<EditorCreadWnd>(true, title);

            window.Show(onFunc);
        }

        private void Show(Func<string, string> onFunc)
        {
            complete = onFunc;
            Show();
        }

        private void OnEnable()
        {
            minSize = new Vector2(280, 140);
            maxSize = minSize;
            name = string.Empty;
            tipsStr = string.Empty;
            

        }

        public void OnGUI()
        {
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("请输入要生成的模块名:");
            GUILayout.Space(5);
            EditorGUILayout.LabelField(tipsStr);
            GUILayout.Space(5);
            creadName = EditorGUILayout.TextField(creadName, GUILayout.Height(20));               
            GUILayout.Space(5);
            if (GUILayout.Button("创建", GUILayout.Height(40)))
            {
                Create(creadName);
            }           
            EditorGUILayout.EndVertical();


        }

        private void Create(string val)
        {
            
            if (val==null|| val.Trim() == string.Empty)
            {
                tipsStr = "创建名为空";
                return;
            }
            if (complete == null)
            {
                tipsStr = "回调为空";
                return;
            }
            tipsStr = complete.Invoke(val.Trim());
            if (tipsStr== "End")
            {
                Close();
            }
            
        }




    }
}