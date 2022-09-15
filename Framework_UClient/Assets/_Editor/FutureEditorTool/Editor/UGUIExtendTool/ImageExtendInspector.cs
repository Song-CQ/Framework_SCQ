/****************************************************
    文件: ImageExtendInspector.cs
    作者: Clear
    日期: 2022/9/14 20:2:15
    类型: 逻辑脚本
    功能: ImageExtend面板显示工具
*****************************************************/
using ProjectApp.UGUI;
using UnityEditor;
using UnityEngine;

namespace FutureEditor.UGUIExtend
{
    [CustomEditor(typeof(ImageExtend),true)]
    public class ImageExtendInspector: Editor
    {
     
        SerializedProperty _flipHor;
        SerializedProperty _flipVer;
    
        public override void OnInspectorGUI()
        {

            _flipHor = serializedObject.FindProperty("flipHor");
            _flipVer = serializedObject.FindProperty("flipVer");

            serializedObject.Update();

            //绘制变量
            EditorGUILayout.PropertyField(_flipHor,new GUIContent("水平翻转图片"));
            EditorGUILayout.PropertyField(_flipVer, new GUIContent("垂直翻转图片"));

            //值改变应用到字段上
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }


    }
}