using System;
using UnityEditor;
using UnityEngine;

namespace Vision.Editor
{
    public class VisionGUIWindowKit
    {
        public static void InitExpendBox(string smallTabTitle, ref bool isShow, Action action)
        {
            EditorGUILayout.Separator();
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
            EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.colorField));
            isShow = EditorGUILayout.Foldout(isShow, smallTabTitle, true, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }
        

   

    }
}