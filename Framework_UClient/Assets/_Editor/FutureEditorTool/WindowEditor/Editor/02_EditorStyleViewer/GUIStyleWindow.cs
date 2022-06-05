using UnityEditor.AnimatedValues;
using UnityEditor.EditorTools;
using UnityEditor.Tilemaps;
using FutureCore;

namespace FutureEditor
{
    using UnityEngine;
    using UnityEditor;

    public class GUIStyleWindow:Singleton<GUIStyleWindow>
    {
        private Vector2 scrollPosition = Vector2.zero;
        private string search = string.Empty;

        #region 测试Demo

        string hintString = "";
        private bool isShowText = true;
        private bool isShowProgress = true;
        private bool isShowLayout = true;
        private bool isShowChoose = true;
        private bool isShowEditor = true;
        private bool isShowTileMapEditor = true;
        private bool isShowHelpBox = true;

        float knob = 1;
        private bool isFoldout = true;
        private int popupIndex = 0;
        private float sliderValue = 0;
        private bool isToggle;
        private Bounds Bounds;
        private Color color;
        private AnimationCurve animationCurve;
        private double doubleValue;
        private string m_itemString = "123";
        private BoxTool _boxTool;
        private int tabIndexDemo;

        bool showFoldout = true;
        bool posGroupEnabled = true;
        bool[] pos = { true, true, true };
        string t = "这 是 一 个 测 试 Scroll view 的 文 本 ！\n";
        string m_content = "";
        Vector2 scrollPos;
        int m_Number = 0;
        Color m_Color = Color.white;
        string m_String;
        AnimBool m_ShowExtraFields;

        private bool isShowAnimation;
        private bool isShowInspectorTitlebar;


        private bool isChecked;

        #region EnumMaskField

        public enum Example
        {
            Option_One = 1, //bits: 0000 0001
            Option_Two = 2, //bits: 0000 0010
            Option_Three = 4 //bits: 0000 0100
        }

        Example staticFlagMask = 0;

        #endregion

        #region EnumPopup

        public enum OPTIONS
        {
            CUBE = 0,
            SPHERE = 1,
            PLANE = 2
        }

        public OPTIONS op;

        #endregion

        #region IntPopup

        int selectedSize = 1;
        string[] names = new string[] { "Normal", "Double", "Quadruple" };
        int[] sizes = new int[] { 1, 2, 4 };

        #endregion

        #region Popup

        public string[] options = new string[] { "Cube", "Sphere", "Plane" };
        public int index = 0;

        #endregion

        #region EnumMaskPopup

        public enum Options
        {
            CUBE = 0,
            SPHERE = 1,
            PLANE = 2
        }

        public Options m_options;

        #endregion

        #region InspectorTitlebar

        bool fold = true;
        bool fold2 = true;
        bool fold3 = true;
        Transform selectedTransform;
        GameObject selectedGameObject;

        #endregion

        #region IntSlider

        int m_intSlider = 1;

        #endregion

        #region IntSlider

        float scale = 0.0f;

        #endregion

        #region MinMaxSlider

        float minVal = -10;
        float maxVal = 10;

        float minLimit = -20;
        float maxLimit = 20;

        #endregion

        #region PasswordField

        string m_passwordField = "";

        #endregion

        string m_textArea = "";
        Vector2 m_vector2;
        Vector3 m_vector3;
        Vector4 m_vector4;
        Vector4 rotationComponents;
        public Object source;

        #endregion

        private EditorWindow editorWindow;

        public void InitEvent(EditorWindow editor)
        {
            editorWindow = editor;
            //创建一个AnimBool对象，true是默认显示。
            m_ShowExtraFields = new AnimBool(true);
            //监听重绘
            m_ShowExtraFields.valueChanged.AddListener(editorWindow.Repaint);
        }

        public void InitEditorDemo()
        {

            tabIndexDemo = GUILayout.Toolbar(tabIndexDemo, new[] { "编辑器测试1", "编辑器测试2", "编辑器样式示例" });

            switch (tabIndexDemo)
            {
                case 0:
                    InitDemo1();
                    break;
                case 1:
                    InitDemo2();
                    break;
                case 2:
                    ShowGUIStyleWindow();
                    break;
            }
        }

        private void InitDemo1()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUIWindowKit.InitExpendBox("文本模块", ref isShowText, () =>
            {
                // 文本
                EditorGUILayout.LabelField("EditorGUILayout.LabelField: 文本");

                EditorGUILayout.BeginHorizontal(new GUIStyle());
                hintString = EditorGUILayout.TextField("这里是提示");
                if (GUILayout.Button("显示消息"))
                {
                    editorWindow.ShowNotification(new GUIContent(hintString));
                }

                EditorGUILayout.EndHorizontal();

                #region PasswordField

                m_passwordField = EditorGUILayout.PasswordField("PasswordField:", m_passwordField);
                EditorGUILayout.LabelField("输入的文本:", m_passwordField);

                #endregion

                doubleValue = EditorGUILayout.DoubleField("双精度浮点型数值", doubleValue);

                #region SelectableLabel

                //可以选择，复制粘贴
                EditorGUILayout.SelectableLabel("SelectableLabel");

                #endregion
            });

            GUIWindowKit.InitExpendBox("进度模块", ref isShowProgress, () =>
            {
                //环形进度条
                knob = EditorGUILayout.Knob(new Vector2(50, 50), knob, 5, 10, "斤", Color.black, Color.blue, true);
                //进度条
                sliderValue = EditorGUILayout.Slider(sliderValue, 1, 100);
                scale = EditorGUILayout.Slider("Slider:", scale, 1, 100);

                #region IntSlider

                //包括最大最小值
                m_intSlider = EditorGUILayout.IntSlider("IntSlider:", m_intSlider, 1, 10);

                #endregion

                #region MinMaxSlider

                EditorGUILayout.BeginHorizontal();
                //取值范围
                EditorGUILayout.LabelField(minVal.ToString());
                EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, minLimit, maxLimit);
                EditorGUILayout.LabelField(maxVal.ToString());
                EditorGUILayout.EndHorizontal();

                #endregion
            });

            GUIWindowKit.InitExpendBox("布局相关模块", ref isShowLayout, () =>
            {
                //收缩栏
                isFoldout = EditorGUILayout.Foldout(isFoldout, "展开收起", true);
                if (isFoldout)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("EditorGUILayout.Separator");
                    //空行
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("EditorGUILayout.Space");
                    EditorGUILayout.Space(50);
                    //文本
                    EditorGUILayout.LabelField("EditorGUILayout.Knob");
                    EditorGUI.indentLevel--;
                }

                InitBeginHorizontal();
                EditorGUILayout.Separator();

                InitBeginVertical();
            });


            GUIWindowKit.InitExpendBox("单选与多选", ref isShowChoose, () =>
            {
                EditorGUILayout.LabelField("EditorGUILayout.Toggle");
                isToggle = EditorGUILayout.Toggle("勾选", isToggle);
                EditorGUILayout.LabelField("Toggle Result: " + isToggle);


                EditorGUILayout.LabelField("EditorGUILayout.Popup");


                #region EnumMaskField

                //可以多选
                staticFlagMask = (Example)EditorGUILayout.EnumMaskField("EnumMaskField:", staticFlagMask);

                #endregion

                //下拉列表

                #region Popup

                popupIndex = EditorGUILayout.Popup("Popup:", popupIndex, options);

                #endregion

                #region IntPopup

                selectedSize = EditorGUILayout.IntPopup("IntPopup: ", selectedSize, names, sizes);

                #endregion

                #region EnumPopup

                op = (OPTIONS)EditorGUILayout.EnumPopup("EnumPopup:", op);

                #endregion

                #region EnumMaskPopup

                m_options = (Options)EditorGUILayout.EnumMaskPopup("EnumMaskPopup:", m_options);

                #endregion

                InitDropdownButton();
            });

            GUIWindowKit.InitExpendBox("数值编辑模块", ref isShowEditor, () =>
            {
                Bounds = EditorGUILayout.BoundsField("边界输入框", Bounds);
                EditorGUILayout.Separator();
                color = EditorGUILayout.ColorField("颜色输入框", color);
                EditorGUILayout.Separator();
                animationCurve = EditorGUILayout.CurveField("动画曲线框", animationCurve);
                EditorGUILayout.Separator();

                //自适应高，不能自适应宽
                m_textArea = EditorGUILayout.TextArea(m_textArea);

                m_vector2 = EditorGUILayout.Vector2Field("Vector2:", m_vector2);
                m_vector3 = EditorGUILayout.Vector3Field("Vector3:", m_vector3);
                m_vector4 = EditorGUILayout.Vector4Field("Vector4:", m_vector4);
            });

            GUIWindowKit.InitExpendBox("TileMap编辑工具", ref isShowTileMapEditor, () => { InitTilemapsTool(); });

            GUIWindowKit.InitExpendBox("HelpBox", ref isShowHelpBox, () =>
            {
                #region HelpBox

                EditorGUILayout.HelpBox("HelpBox Error:", MessageType.Error);
                EditorGUILayout.HelpBox("HelpBox Info:", MessageType.Info);
                EditorGUILayout.HelpBox("HelpBox None:", MessageType.None);
                EditorGUILayout.HelpBox("HelpBox Warning:", MessageType.Warning);

                #endregion
            });

            GUIWindowKit.InitExpendBox("InspectorTitlebar", ref isShowInspectorTitlebar, () =>
            {
                #region InspectorTitlebar

                var gameObject = Selection.activeGameObject;
                if (gameObject)
                {
                    selectedTransform = gameObject.transform;
                    selectedGameObject = gameObject;
                    fold = EditorGUILayout.InspectorTitlebar(fold, selectedTransform);
                    fold2 = EditorGUILayout.InspectorTitlebar(fold2, selectedGameObject);

                    fold3 = EditorGUILayout.InspectorTitlebar(fold3, selectedTransform);
                    if (fold3)
                    {
                        selectedTransform.position =
                            EditorGUILayout.Vector3Field("Position", selectedTransform.position);
                        EditorGUILayout.Space();
                        rotationComponents =
                            EditorGUILayout.Vector4Field("Detailed Rotation",
                                QuaternionToVector4(selectedTransform.localRotation));
                        EditorGUILayout.Space();
                        selectedTransform.localScale =
                            EditorGUILayout.Vector3Field("Scale", selectedTransform.localScale);
                    }

                    selectedTransform.localRotation = ConvertToQuaternion(rotationComponents);
                    EditorGUILayout.Space();
                }

                EditorGUILayout.BeginHorizontal();
                source = EditorGUILayout.ObjectField(source, typeof(Object), true);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Search!"))
                {
                    if (source == null)
                    {
                        editorWindow.ShowNotification(new GUIContent("No object selected for searching"));
                    }
                    else if (Help.HasHelpForObject(source))
                        Help.ShowHelpForObject(source);
                    else
                        Help.BrowseURL("https://forum.unity3d.com/search.php");
                }

                #endregion
            });

            GUILayout.EndScrollView();

            if (GUILayout.Button("关闭"))
            {
                editorWindow.Close();
            }
        }

        Quaternion ConvertToQuaternion(Vector4 v4)
        {
            return new Quaternion(v4.x, v4.y, v4.z, v4.w);
        }

        Vector4 QuaternionToVector4(Quaternion q)
        {
            return new Vector4(q.x, q.y, q.z, q.w);
        }

        /// <summary>
        /// 显示编辑器样式示例
        /// </summary>
        public void ShowGUIStyleWindow()
        {
            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.Label("单击示例可复制其名到剪贴板", "label");
            GUILayout.FlexibleSpace();
            GUILayout.Label("查找:");
            search = EditorGUILayout.TextField(search);
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (GUIStyle style in GUI.skin)
            {
                if (style.name.ToLower().Contains(search.ToLower()))
                {
                    GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                    GUILayout.Space(7);
                    if (GUILayout.Button(style.name, style))
                    {
                        EditorGUIUtility.systemCopyBuffer = "\"" + style.name + "\"";
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(11);
                }
            }

            GUILayout.EndScrollView();
        }

        private void InitTilemapsTool()
        {
            _boxTool = ScriptableObject.CreateInstance<BoxTool>();
            EditorGUILayout.EditorToolbar(new EditorTool[]
            {
                _boxTool,
                ScriptableObject.CreateInstance<EraseTool>(),
                ScriptableObject.CreateInstance<FillTool>(),
                ScriptableObject.CreateInstance<MoveTool>(),
                ScriptableObject.CreateInstance<PaintTool>(),
                ScriptableObject.CreateInstance<PickingTool>(),
                ScriptableObject.CreateInstance<SelectTool>(), _boxTool
            });
        }

        private void InitDemo2()
        {
            /*if (GUI.Button(new Rect(300, 50, 100, 100), isChecked ? checkBoxChecked : checkBoxUnchecked, GUIStyle.none))
            {
                isChecked = !isChecked;
                // QSettings.getInstance().set(setting, !isChecked);
            }*/
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            #region BeginFadeGroup

            m_ShowExtraFields.target = EditorGUILayout.ToggleLeft("显示折叠内容", m_ShowExtraFields.target); //选择框在左边的开关

            m_ShowExtraFields.target = EditorGUILayout.Toggle("显示折叠内容", m_ShowExtraFields.target); //选择框在右边的开关

            //创建带渐显动画的折叠块 返回值bool，参数float

            if (EditorGUILayout.BeginFadeGroup(m_ShowExtraFields.faded))
            {
                EditorGUI.indentLevel++; //缩进深度增加，以下的GUI会增加缩进


                EditorGUILayout.LabelField("ColorColorColorColorColorColorColorColorColorColor"); //标签栏


                EditorGUILayout.PrefixLabel("ColorColorColorColorColorColorColorColorColorColor"); //前缀标签

                m_Color = EditorGUILayout.ColorField(m_Color);


                EditorGUILayout.PrefixLabel("Text");

                m_String = EditorGUILayout.TextField(m_String); //文本框


                EditorGUILayout.PrefixLabel("Number");

                m_Number = EditorGUILayout.IntSlider(m_Number, 0, 10); //Int滑动条

                EditorGUI.indentLevel--; //缩进深度减少，以下的GUI会减少缩进
            }

            EditorGUILayout.EndFadeGroup();

            #endregion


            #region Foldout

            showFoldout = EditorGUILayout.Foldout(showFoldout, "折叠子物体：");

            if (showFoldout)

            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("折叠块内容1");

                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("折叠块内容2");

                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("折叠块内容3");
            }

            #endregion


            #region BeginHorizontal 水平布局

            Rect r = EditorGUILayout.BeginHorizontal("Button");

            if (GUI.Button(r, GUIContent.none))

                Debug.Log("Go here");

            GUILayout.Label("I'm inside the button");

            GUILayout.Label("So am I");

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("第一个内容");

            GUILayout.Label("第二个内容");

            if (GUILayout.Button("第三个按钮"))

            {
                Debug.Log("GUILayout的按钮");
            }

            EditorGUILayout.EndHorizontal();

            #endregion


            #region BeginVertical 垂直布局

            EditorGUILayout.BeginVertical();

            GUILayout.Label("第一个内容");

            GUILayout.Label("第二个内容");

            if (GUILayout.Button("第三个按钮"))

            {
                Debug.Log("GUILayout的按钮");
            }

            EditorGUILayout.EndVertical();

            #endregion


            #region BeginScrollView

            //需要将返回值赋值到临时变量，不然拖不动

            //可以添加GUILayoutOption参数控制大小

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(200), GUILayout.Height(100));

            GUILayout.Label(m_content);

            EditorGUILayout.EndScrollView();


            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("添加内容"))

                m_content += t;

            if (GUILayout.Button("清空内容"))

                m_content = "";

            EditorGUILayout.EndHorizontal();

            #endregion


            #region BeginToggleGroup

            posGroupEnabled = EditorGUILayout.BeginToggleGroup("ToggleGroup", posGroupEnabled);

            EditorGUILayout.BeginVertical();

            pos[0] = EditorGUILayout.Toggle("Toggle1", pos[0]);

            pos[1] = EditorGUILayout.Toggle("Toggle2", pos[1]);

            pos[2] = EditorGUILayout.Toggle("Toggle3", pos[2]);

            if (GUILayout.Button("添加内容"))

                m_content += t;

            m_String = EditorGUILayout.TextField(m_String);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndToggleGroup();

            #endregion


            //画一个居中的分割线

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label("-----------------分割线-----------------");

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        private void InitDropdownButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("DropdownButton:");
            if (EditorGUILayout.DropdownButton(new GUIContent(m_itemString), FocusType.Keyboard))
            {
                var alls = new string[4] { "A", "B", "C", "D" };
                GenericMenu _menu = new GenericMenu();
                foreach (var item in alls)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }

                    //添加菜单
                    _menu.AddItem(new GUIContent(item), m_itemString.Equals(item), OnValueSelected, item);
                }

                _menu.ShowAsContext(); //显示菜单
            }

            EditorGUILayout.EndHorizontal();
        }

        void OnValueSelected(object value)
        {
            m_itemString = value.ToString();
        }

        private void InitBeginHorizontal()
        {
            Rect r = EditorGUILayout.BeginHorizontal("Button");
            if (GUI.Button(r, GUIContent.none))
                Debug.Log("Go here");
            GUILayout.Label("I'm inside the button");
            GUILayout.Label("So am I");
            EditorGUILayout.EndHorizontal();
        }

        private void InitBeginVertical()
        {
            Rect r = (Rect)EditorGUILayout.BeginVertical("Button");
            if (GUI.Button(r, GUIContent.none))
                Debug.Log("Go here");
            GUILayout.Label("I'm inside the button");
            GUILayout.Label("So am I");
            EditorGUILayout.EndVertical();
        }
    }
}