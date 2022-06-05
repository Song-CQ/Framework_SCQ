using System;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public class GUIWindowKit
    {
        public static void InitExpendBox(string smallTabTitle, ref bool isShow, Action action,bool IsNeedSeparator=true)
        {
            if (IsNeedSeparator)
            {
                EditorGUILayout.Separator();
            }         
            EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
        
            InitExpendTitle(smallTabTitle, ref isShow);

            if (isShow)
            {
                EditorGUI.indentLevel++;
                action?.Invoke();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        public static void InitExpendTitle(string smallTabTitle, ref bool isShow)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            isShow = EditorGUILayout.Foldout(isShow,smallTabTitle,true);
            EditorGUILayout.EndHorizontal();
        }
        

   

    }
}