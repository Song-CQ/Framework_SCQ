namespace XL.EditorTool
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEditor;
    using UnityEditor.UI;

    public class UITool
    {
        [MenuItem("GameObject/UI工具/添加按钮组件", false, 0)]
        static void AddButton()
        {         
            foreach (var item in Selection.gameObjects)
            {
                if (item.GetComponent<Image>() != null && item.GetComponent<Button>() == null)
                {
                    item.AddComponent<Button>();
                }
            }
        }
        [MenuItem("GameObject/UI工具/添加按钮组件", true)]
        static bool CheckAddButton()
        {
            if (Selection.activeObject != null)
            {
                foreach (var item in Selection.gameObjects)
                {
                    if (item.GetComponent<Image>() != null&&item.GetComponent<Button>()==null)
                    {
                        return true;
                    }
                }
                    
            }
            return false;
        }
    }
    [CustomEditor(typeof(Image_XL))]
    public class Image_XLEditor : ImageEditor
    {
        public new Image_XL target;

        private SerializedProperty _spFlipHor;
        private SerializedProperty _spFlipVer;
        private GUIContent _gcFlipHor;
        private GUIContent _gcFlipVer;

        protected override void OnEnable()
        {
            base.OnEnable();

            target = base.target as Image_XL;

            _spFlipHor = serializedObject.FindProperty("flipHor");
            _spFlipVer = serializedObject.FindProperty("flipVer");
            _gcFlipHor = EditorGUIUtility.TrTextContent("水平翻转", null);
            _gcFlipVer = EditorGUIUtility.TrTextContent("垂直翻转", null);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(_spFlipHor, _gcFlipHor);
            EditorGUILayout.PropertyField(_spFlipVer, _gcFlipVer);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}