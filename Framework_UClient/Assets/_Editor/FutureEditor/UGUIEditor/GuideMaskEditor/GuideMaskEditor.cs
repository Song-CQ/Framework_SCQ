/****************************************************
    文件: GuideMaskEditor.cs
    作者: Clear
    日期: 2026/2/13 3:52:31
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using UnityEngine;
using ProjectApp;
using UnityEditor;
using UnityEditor.UI;


namespace FutureEditor
{
    

    [CustomEditor(typeof(GuideMask))]
    public class GuideMaskEditor : ImageEditor
    {
        private SerializedProperty shape;
        private SerializedProperty targetRect;
        private SerializedProperty polygonPoints;
        private SerializedProperty maskColor;
        private SerializedProperty featherWidth;
        private SerializedProperty circleRadiusScale;

        // 需要隐藏的Image属性
        private string[] hiddenProperties = new string[]
        {
        "m_Sprite",
        "m_Type",
        "m_UseSpriteMesh",
        "m_PreserveAspect",
        "m_FillCenter",
        "m_FillMethod",
        "m_FillAmount",
        "m_FillClockwise",
        "m_FillOrigin",
        "m_AlphaHitTestMinimumThreshold",
        "m_ImageType",
        "m_PixelsPerUnitMultiplier",
        "sprite"
        };

        protected override void OnEnable()
        {
            base.OnEnable();

            shape = serializedObject.FindProperty("shape");
            targetRect = serializedObject.FindProperty("targetRect");
            polygonPoints = serializedObject.FindProperty("polygonPoints");
            maskColor = serializedObject.FindProperty("maskColor");
            featherWidth = serializedObject.FindProperty("featherWidth");
            circleRadiusScale = serializedObject.FindProperty("circleRadiusScale");
        }

        public override void OnInspectorGUI()
        {


            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((GuideMask)target), typeof(GuideMask), false);
            GUI.enabled = true;

            // 标题
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("✨ 新手引导遮罩设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // 分割线
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(5);

            // 形状选择
            EditorGUILayout.PropertyField(shape, new GUIContent("洞的形状"));
            EditorGUILayout.Space(5);

            GuideMask.ShapeType currentShape = (GuideMask.ShapeType)shape.enumValueIndex;

            // 根据形状显示不同字段
            switch (currentShape)
            {
                case GuideMask.ShapeType.Rectangle:
                    EditorGUILayout.PropertyField(targetRect, new GUIContent("目标矩形"));
                    EditorGUILayout.HelpBox("矩形洞将挖在目标RectTransform范围内", MessageType.Info);
                    break;

                case GuideMask.ShapeType.Circle:
                    EditorGUILayout.PropertyField(targetRect, new GUIContent("目标矩形"));
                    EditorGUILayout.PropertyField(circleRadiusScale, new GUIContent("半径缩放"));
                    EditorGUILayout.HelpBox("圆形洞将以目标矩形中心为圆心，对角线为直径", MessageType.Info);
                    break;

                case GuideMask.ShapeType.Polygon:
                    EditorGUILayout.PropertyField(polygonPoints, new GUIContent("多边形顶点"), true);
                    EditorGUILayout.HelpBox("顶点坐标使用屏幕像素坐标", MessageType.Info);

                    if (polygonPoints.arraySize < 3)
                    {
                        EditorGUILayout.HelpBox("至少需要3个顶点", MessageType.Warning);
                    }
                    break;
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(5);

            // 外观设置
            EditorGUILayout.LabelField("🎨 外观设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            EditorGUILayout.PropertyField(maskColor, new GUIContent("遮罩颜色"));
            EditorGUILayout.PropertyField(featherWidth, new GUIContent("羽化宽度"));

            if (featherWidth.floatValue < 0)
                featherWidth.floatValue = 0;

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(5);

            // 基础Image设置（只显示必要的）
            EditorGUILayout.LabelField("⚙️ 基础设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            // 显示必要的Image属性
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"), new GUIContent("Image颜色"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RaycastTarget"), new GUIContent("射线检测"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Maskable"), new GUIContent("可被遮罩"));

            // 显示材质（如果需要）
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Material"), new GUIContent("材质"));

            EditorGUILayout.Space(10);

            // 应用修改
            serializedObject.ApplyModifiedProperties();

            // 预览按钮
            //if (GUILayout.Button("强制刷新预览", GUILayout.Height(30)))
            //{
            //    var guideMask = target as GuideMask;
            //    guideMask.SetVerticesDirty();
            //    SceneView.RepaintAll();
            //}
        }

        // 重写ImageEditor的OnInspectorGUI，隐藏不需要的属性
        public override bool HasPreviewGUI()
        {
            return false;
        }
    }

}