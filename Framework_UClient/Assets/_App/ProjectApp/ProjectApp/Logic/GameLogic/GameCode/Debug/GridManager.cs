/****************************************************
    文件: GridManager.cs
    作者: Clear
    日期: 2026/1/23 21:38:56
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace ProjectApp
{

    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private GridDisplay grid = new GridDisplay();

        private EliminateGameCore core;

        public GridDisplay Grid => grid;
        [SerializeField]


        [Header("网格设置")]
        [Range(1, 20)]
        public int gridWidth = 9;
        [Range(1, 20)]
        public int gridHeight = 14;

        [Header("调试信息")]
        [SerializeField]
        private bool showDebugInfo = true;
        [SerializeField, TextArea(3, 10)]
        private string gridInfo;

        public static GridManager instance;

        private void Awake()
        {
            instance = this;
        }
        
        private void Start()
        {
            core = GetComponent<EliminateGameCore>();
            ResizeGrid();
        }

        void OnValidate()
        {
            // 同步网格尺寸
            if (grid.Width != gridWidth || grid.Height != gridHeight)
            {
                ResizeGrid();
            }

            UpdateDebugInfo();
        }

        void ResizeGrid()
        {
            // 这里可以实现网格重设大小逻辑
            grid._width = gridWidth;
            grid._height = gridHeight;
            grid.OnValidate();
            // 注意：GridDisplay内部已经处理了尺寸变化
        }
        private void Update()
        {
            if (core == null) return;
            if(core.Data!=null)
            foreach (var item in core.BoardData)
            {
                grid[item.X, item.Y] = item;
            }

        }

        void UpdateDebugInfo()
        {
            if (!showDebugInfo) return;

            gridInfo = $"网格统计:\n";
            gridInfo += $"尺寸: {grid.Width} × {grid.Height}\n";
            gridInfo += $"总单元格数: {grid.Width * grid.Height}\n\n";

            // 统计各类型数量
            System.Collections.Generic.Dictionary<ElementType, int> typeCount =
                new System.Collections.Generic.Dictionary<ElementType, int>();

            foreach (ElementType type in System.Enum.GetValues(typeof(ElementType)))
            {
                typeCount[type] = 0;
            }

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    typeCount[grid[x, y].Type]++;
                }
            }

            foreach (var kvp in typeCount)
            {
                gridInfo += $"{kvp.Key}: {kvp.Value}\n";
            }
        }

#if UNITY_EDITOR
        [ContextMenu("打印网格数据")]
        void PrintGridData()
        {
            Debug.Log("=== 网格数据 ===");
            for (int y = 0; y < grid.Height; y++)
            {
                string row = "";
                for (int x = 0; x < grid.Width; x++)
                {
                    row += $"[{grid[x, y].Type.ToString()[0]}] ";
                }
                Debug.Log(row);
            }
        }

        [ContextMenu("导出为CSV")]
        void ExportToCSV()
        {
            string csv = "X,Y,Type\n";

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    csv += $"{x},{y},{grid[x, y].Type}\n";
                }
            }

            string path = EditorUtility.SaveFilePanel(
                "导出为CSV",
                "",
                "GridData.csv",
                "csv");

            if (!string.IsNullOrEmpty(path))
            {
                System.IO.File.WriteAllText(path, csv);
                Debug.Log($"CSV文件已保存到: {path}");
            }
        }
#endif
    }
  
    #if UNITY_EDITOR


[CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor
    {
        private ElementType brushType = ElementType.Fixed_None;
        private bool showGrid = true;
        private Vector2 scrollPosition;


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GridManager gridManager = (GridManager)target;

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("网格编辑器", EditorStyles.boldLabel);

            // 控制面板
            DrawControlPanel(gridManager);

            if (showGrid)
            {
                EditorGUILayout.Space(10);
                DrawGridVisual(gridManager);
            }
        }

        private void DrawControlPanel(GridManager gridManager)
        {
            EditorGUILayout.BeginVertical("box");

            showGrid = EditorGUILayout.Toggle("显示网格", showGrid);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("画笔设置", EditorStyles.miniBoldLabel);
            brushType = (ElementType)EditorGUILayout.EnumPopup("元素类型", brushType);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("操作", EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("清空网格", GUILayout.Height(25)))
            {
                ClearGrid(gridManager.Grid);
            }

            if (GUILayout.Button("随机填充", GUILayout.Height(25)))
            {
                RandomFillGrid(gridManager.Grid);
            }

            if (GUILayout.Button("保存预设", GUILayout.Height(25)))
            {
                SaveGridAsPreset(gridManager);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawGridVisual(GridManager gridManager)
        {
            var grid = gridManager.Grid;
            if (grid == null) return;

            EditorGUILayout.LabelField($"网格尺寸: {grid.Width} × {grid.Height}");

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));

            // 计算单元格大小
            float cellSize = Mathf.Clamp(400f / Mathf.Max(grid.Width, grid.Height), 30f, 60f);

            // 绘制X轴坐标
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(30)); // Y轴标签占位

            for (int x = 0; x < grid.Width; x++)
            {
                GUILayout.Label(x.ToString(),
                    new GUIStyle(EditorStyles.miniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter
                    },
                    GUILayout.Width(cellSize));
            }
            EditorGUILayout.EndHorizontal();

            // 绘制网格内容
            for (int y = 0; y < grid.Height; y++)
            {
                EditorGUILayout.BeginHorizontal();

                // Y轴坐标
                GUILayout.Label(y.ToString(),
                    new GUIStyle(EditorStyles.miniLabel)
                    {
                        alignment = TextAnchor.MiddleRight
                    },
                    GUILayout.Width(25));
                //var _colorMap = GridManager.instance != null ? GridManager.instance.colorMap : colorMap;
                for (int x = 0; x < grid.Width; x++)
                {
                    ElementData cell = grid[x, y];
                    Color cellColor = Color.white;

                    // 创建样式
                    GUIStyle cellStyle = new GUIStyle(GUI.skin.box)
                    {
                        fixedWidth = cellSize,
                        fixedHeight = cellSize,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 10
                    };

                    // 创建背景纹理
                    Texture2D bgTexture = new Texture2D(1, 1);
                    bgTexture.SetPixel(0, 0, cellColor);
                    bgTexture.Apply();

                    cellStyle.normal.background = bgTexture;
                    cellStyle.normal.textColor = Color.white;

                    // 显示单元格内容
                    string cellText = $"{cell.Type.ToString()[0]}";

                    if (GUILayout.Button(cellText, cellStyle))
                    {
                        // 点击设置类型
                        Undo.RecordObject(gridManager, "Change Grid Cell");
                      
                        grid[x, y] = new ElementData().SetPot(x, y).SetType(brushType);

                        EditorUtility.SetDirty(gridManager);
                    }

                    // 显示坐标提示
                    if (Event.current.type == EventType.Repaint)
                    {
                        Rect lastRect = GUILayoutUtility.GetLastRect();
                        EditorGUI.LabelField(
                            new Rect(lastRect.x, lastRect.y, lastRect.width, 15),
                            $"({x},{y})",
                            new GUIStyle(EditorStyles.miniLabel)
                            {
                                alignment = TextAnchor.UpperCenter,
                                normal = { textColor = new Color(1, 1, 1, 0.7f) }
                            });
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // 绘制图例
            DrawLegend();
        }

        private void DrawLegend()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("图例", EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            //var _colorMap = GridManager.instance != null ? GridManager.instance.colorMap : colorMap;

            //foreach (var kvp in _colorMap)
            //{
            //    EditorGUILayout.BeginVertical(GUILayout.Width(70));

            //    // 颜色方块
            //    Rect colorRect = GUILayoutUtility.GetRect(20, 20);
            //    EditorGUI.DrawRect(colorRect, kvp.Value);

            //    // 类型名称
            //    EditorGUILayout.LabelField(kvp.Key.ToString(),
            //        new GUIStyle(EditorStyles.miniLabel)
            //        {
            //            alignment = TextAnchor.MiddleCenter
            //        });

            //    EditorGUILayout.EndVertical();
            //}

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void ClearGrid(GridDisplay grid)
        {
            Undo.RecordObject(target, "Clear Grid");

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    grid[x, y] = new ElementData().SetPot(x, y).SetType(ElementType.Fixed_None); 
                }
            }

            EditorUtility.SetDirty(target);
        }

        private void RandomFillGrid(GridDisplay grid)
        {
            Undo.RecordObject(target, "Random Fill Grid");

            Array types = Enum.GetValues(typeof(ElementType));

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    ElementType randomType = (ElementType)types.GetValue(UnityEngine.Random.Range(1, types.Length));

                    grid[x, y] = new ElementData().SetPot(x, y).SetType(randomType);
                }
            }

            EditorUtility.SetDirty(target);
        }

        private void SaveGridAsPreset(GridManager gridManager)
        {
            //string path = EditorUtility.SaveFilePanelInProject(
            //    "保存网格预设",
            //    "GridPreset",
            //    "asset",
            //    "选择保存位置");

            //if (!string.IsNullOrEmpty(path))
            //{
            //    GridPreset preset = ScriptableObject.CreateInstance<GridPreset>();
            //    preset.SaveFromGrid(gridManager.Grid);

            //    AssetDatabase.CreateAsset(preset, path);
            //    AssetDatabase.SaveAssets();

            //    Debug.Log($"网格预设已保存到: {path}");
            //}
        }
    }
#endif



    [Serializable]
    public class GridDisplay
    {
        [SerializeField, Range(1, 20)]
        public int _width = 8;
        [SerializeField, Range(1, 20)]
        public int _height = 8;

        [SerializeField]
        private ElementData[] _cells;

        public int Width => _width;
        public int Height => _height;

        public ElementData this[int x, int y]
        {
            get 
            {
                
                return _cells[y * _width + x];
            } 
            set
            {
                value.X = x;
                value.Y = y;

                _cells[y * _width + x] = value;
            }
        }

        public GridDisplay()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            _cells = new ElementData[_width * _height];
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _cells[y * _width + x] = new ElementData().SetPot(x, y).SetType( ElementType.Fixed_Special);
                }
            }
        }

        // Unity序列化回调
        public void OnValidate()
        {
            if (_cells == null || _cells.Length != _width * _height)
            {
                InitializeGrid();
            }
        }
    }




[CustomPropertyDrawer(typeof(GridDisplay))]
    public class GridDisplayDrawer : PropertyDrawer
    {
        private const float CELL_SIZE = 40f;
        private const float MARGIN = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty widthProp = property.FindPropertyRelative("_width");
            SerializedProperty heightProp = property.FindPropertyRelative("_height");
            SerializedProperty cellsProp = property.FindPropertyRelative("_cells");

            // 绘制折叠标题
            Rect foldoutRect = new Rect(position.x, position.y, position.width, 20);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

            if (property.isExpanded)
            {
                // 网格视图
                float startY = position.y + 25;
                DrawGrid(position.x + 20, startY, cellsProp, widthProp.intValue, heightProp.intValue);
            }

            EditorGUI.EndProperty();
        }

        private void DrawGrid(float startX, float startY, SerializedProperty cellsProp, int width, int height)
        {
            if (cellsProp == null || !cellsProp.isArray) return;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    if (index >= cellsProp.arraySize) break;

                    SerializedProperty cellProp = cellsProp.GetArrayElementAtIndex(index);
                    SerializedProperty typeProp = cellProp.FindPropertyRelative("Type");

                    Rect cellRect = new Rect(
                        startX + x * (CELL_SIZE + MARGIN),
                        startY + y * (CELL_SIZE + MARGIN),
                        CELL_SIZE,
                        CELL_SIZE);

                    // 根据类型绘制不同颜色
                    ElementType type = (ElementType)Enum.Parse(typeof(ElementType), typeProp.enumNames[typeProp.enumValueIndex]);
                    Color cellColor = GetColorForType(type);

                    EditorGUI.DrawRect(cellRect, cellColor);

                    // 显示类型首字母
                    //string typeChar = type.ToString().Substring(0, 1);
                    string typeChar = type.ToString();
                    EditorGUI.LabelField(cellRect, typeChar,
                        new GUIStyle(EditorStyles.boldLabel)
                        {
                            alignment = TextAnchor.MiddleCenter
                        });
                }
            }
        }

     

        private Color GetColorForType(ElementType type)
        {
            return Color.white;
            if (GridManager.instance == null)
            {
                return Color.white;
            }
           //return GridManager.instance.colorMap[type];
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return 20;

            SerializedProperty heightProp = property.FindPropertyRelative("_height");
            int height = heightProp.intValue;

            return 25 + height * (CELL_SIZE + MARGIN);
        }
    }

}