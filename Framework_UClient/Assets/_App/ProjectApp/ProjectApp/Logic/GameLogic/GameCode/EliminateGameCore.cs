using FutureCore;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectApp
{
    public enum ElementType
    {
        Baihe,    // 红色基础元素
        fengjiao, // 黄色基础元素
        shuihu,   // 蓝色基础元素
        xihongshui,  // 绿色基础元素
        zhiwu,  // 紫色基础元素
        Special // 特殊元素（可切换类型）
    }

    public enum GameMode
    {
        BuildHive,      // 建造蜂巢模式
        WorkerBeeChallenge // 工蜂挑战模式
    }

    public class EliminateGameCore : MonoBehaviour
    {
        [Header("游戏配置")]
        [SerializeField] private int boardWidth = 10;    // 棋盘宽度
        [SerializeField] private int boardHeight = 14;   // 棋盘高度
        [SerializeField] private Vector3 startVector3;   // 棋盘高度
        [SerializeField] private GameMode currentMode = GameMode.BuildHive;

        [Header("元素预设")]
        [SerializeField] private GameObject[] elementPrefabs; // 0-3为基础元素，4为特殊元素

        [Header("道具预设")]
        [SerializeField] private GameObject horizontalProp;   // 横向消除道具
        [SerializeField] private GameObject verticalProp;     // 竖向消除道具
        [SerializeField] private GameObject bombProp;         // 炸弹道具
        [SerializeField] private GameObject wildProp;         // Wild道具

        // 棋盘数据
        private ElementType[,] board;
        private GameObject[,] elementObjects;

        // 当前分数
        private int currentScore = 0;
        private int targetScore = 100000; // 目标分数

        // 当前选中的元素
        private Vector2Int selectedElement = new Vector2Int(-1, -1);

        public Transform itemsTrf;

        void Start()
        {
            InitializeBoard();
            GenerateInitialElements();
        }

        /// <summary>
        /// 初始化棋盘
        /// </summary>
        void InitializeBoard()
        {
            board = new ElementType[boardWidth, boardHeight];
            elementObjects = new GameObject[boardWidth, boardHeight];
        }

        [Button("生成")]
        /// <summary>
        /// 生成初始元素（简化的随机生成）
        /// </summary>
        void GenerateInitialElements()
        {
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    // 根据配置表比例生成元素（这里简化为随机）
                    float rand = Random.value;
                    ElementType type;

                    if (rand < 0.7f) // 70%为基础元素
                    {
                        type = (ElementType)Random.Range(0, 5);
                    }
                    else if (rand < 0.9f) // 20%为特殊元素
                    {
                        type = ElementType.Special;
                    }
                    else // 10%随机生成道具（简化处理）
                    {
                        type = (ElementType)Random.Range(0, 4);
                    }

                    CreateElement(x, y, type);
                }
            }

            // 检查并消除初始匹配
            CheckInitialMatches();
        }

        /// <summary>
        /// 创建元素
        /// </summary>
        void CreateElement(int x, int y, ElementType type)
        {
            board[x, y] = type;

            if (elementPrefabs.Length > (int)type)
            {
                Vector3 position = startVector3 + new Vector3(x, y, 0);
                elementObjects[x, y] = Instantiate(elementPrefabs[(int)type], position, Quaternion.identity);

                // 添加元素脚本
                Element elementScript = elementObjects[x, y].AddComponent<Element>();
                elementScript.Initialize(x, y, type);
                elementScript.OnElementClicked += OnElementClicked;
                elementObjects[x, y].SetParent(itemsTrf);
            }
        }

        /// <summary>
        /// 元素点击事件
        /// </summary>
        void OnElementClicked(int x, int y)
        {
            if (selectedElement.x < 0 || selectedElement.y < 0)
            {
                // 第一次点击，选中元素
                selectedElement = new Vector2Int(x, y);
                HighlightElement(x, y, true);
            }
            else
            {
                // 第二次点击，判断是否相邻
                if (IsAdjacent(selectedElement.x, selectedElement.y, x, y))
                {
                    // 交换元素
                    SwapElements(selectedElement.x, selectedElement.y, x, y);

                    // 检查匹配
                    List<Vector2Int> matches = CheckMatchesAfterSwap(selectedElement.x, selectedElement.y, x, y);

                    if (matches.Count > 0)
                    {
                        // 有匹配，进行消除
                        ProcessMatches(matches);
                        StartCoroutine(FillEmptySpaces());
                    }
                    else
                    {
                        // 无匹配，交换回来
                        SwapElements(selectedElement.x, selectedElement.y, x, y);
                    }
                }

                // 清除选中状态
                HighlightElement(selectedElement.x, selectedElement.y, false);
                selectedElement = new Vector2Int(-1, -1);
            }
        }

        /// <summary>
        /// 高亮元素
        /// </summary>
        void HighlightElement(int x, int y, bool highlight)
        {
            if (elementObjects[x, y] != null)
            {
                SpriteRenderer renderer = elementObjects[x, y].GetComponentInChildren<SpriteRenderer>();
                renderer.color = highlight ? Color.yellow : Color.white;
            }
        }

        /// <summary>
        /// 判断两个元素是否相邻
        /// </summary>
        bool IsAdjacent(int x1, int y1, int x2, int y2)
        {
            int dx = Mathf.Abs(x1 - x2);
            int dy = Mathf.Abs(y1 - y2);
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        /// <summary>
        /// 交换两个元素
        /// </summary>
        void SwapElements(int x1, int y1, int x2, int y2)
        {
            // 交换棋盘数据
            ElementType tempType = board[x1, y1];
            board[x1, y1] = board[x2, y2];
            board[x2, y2] = tempType;

            // 交换游戏对象
            GameObject tempObj = elementObjects[x1, y1];
            elementObjects[x1, y1] = elementObjects[x2, y2];
            elementObjects[x2, y2] = tempObj;

            // 更新元素坐标
            if (elementObjects[x1, y1] != null)
            {
                elementObjects[x1, y1].GetComponent<Element>().UpdatePosition(x1, y1);
                elementObjects[x1, y1].transform.position = new Vector3(x1, y1, 0);
            }

            if (elementObjects[x2, y2] != null)
            {
                elementObjects[x2, y2].GetComponent<Element>().UpdatePosition(x2, y2);
                elementObjects[x2, y2].transform.position = new Vector3(x2, y2, 0);
            }
        }

        /// <summary>
        /// 检查交换后的匹配
        /// </summary>
        List<Vector2Int> CheckMatchesAfterSwap(int x1, int y1, int x2, int y2)
        {
            List<Vector2Int> matches = new List<Vector2Int>();

            // 检查交换的两个位置及其相关行列
            matches.AddRange(FindMatchesAt(x1, y1));
            matches.AddRange(FindMatchesAt(x2, y2));

            return matches;
        }

        /// <summary>
        /// 查找指定位置的匹配
        /// </summary>
        List<Vector2Int> FindMatchesAt(int x, int y)
        {
            List<Vector2Int> matches = new List<Vector2Int>();
            ElementType type = board[x, y];

            if (type == ElementType.Special) return matches; // 特殊元素单独处理

            // 横向匹配
            List<Vector2Int> horizontalMatches = new List<Vector2Int>();
            horizontalMatches.Add(new Vector2Int(x, y));

            // 向左检查
            for (int i = x - 1; i >= 0; i--)
            {
                if (board[i, y] == type)
                    horizontalMatches.Add(new Vector2Int(i, y));
                else
                    break;
            }

            // 向右检查
            for (int i = x + 1; i < boardWidth; i++)
            {
                if (board[i, y] == type)
                    horizontalMatches.Add(new Vector2Int(i, y));
                else
                    break;
            }

            // 竖向匹配
            List<Vector2Int> verticalMatches = new List<Vector2Int>();
            verticalMatches.Add(new Vector2Int(x, y));

            // 向下检查
            for (int j = y - 1; j >= 0; j--)
            {
                if (board[x, j] == type)
                    verticalMatches.Add(new Vector2Int(x, j));
                else
                    break;
            }

            // 向上检查
            for (int j = y + 1; j < boardHeight; j++)
            {
                if (board[x, j] == type)
                    verticalMatches.Add(new Vector2Int(x, j));
                else
                    break;
            }

            // 四消规则：需要4个或更多相同元素
            if (horizontalMatches.Count >= 4)
            {
                matches.AddRange(horizontalMatches);

                // 生成道具（根据消除数量）
                if (horizontalMatches.Count == 5)
                {
                    GeneratePropAt(x, y, PropType.Horizontal);
                }
                else if (horizontalMatches.Count == 6)
                {
                    GeneratePropAt(x, y, PropType.Bomb);
                }
                else if (horizontalMatches.Count >= 7)
                {
                    GeneratePropAt(x, y, PropType.Wild);
                }
            }

            if (verticalMatches.Count >= 4)
            {
                matches.AddRange(verticalMatches);

                // 生成道具
                if (verticalMatches.Count == 5)
                {
                    GeneratePropAt(x, y, PropType.Vertical);
                }
                else if (verticalMatches.Count == 6)
                {
                    GeneratePropAt(x, y, PropType.Bomb);
                }
                else if (verticalMatches.Count >= 7)
                {
                    GeneratePropAt(x, y, PropType.Wild);
                }
            }

            return matches;
        }

        /// <summary>
        /// 处理匹配消除
        /// </summary>
        void ProcessMatches(List<Vector2Int> matches)
        {
            // 计算分数
            int matchCount = matches.Count;
            int scoreToAdd = CalculateScore(matchCount);
            AddScore(scoreToAdd);

            // 消除元素
            foreach (Vector2Int match in matches)
            {
                if (elementObjects[match.x, match.y] != null)
                {
                    Destroy(elementObjects[match.x, match.y]);
                    elementObjects[match.x, match.y] = null;
                    board[match.x, match.y] = ElementType.Special; // 临时标记为空
                }
            }
        }

        /// <summary>
        /// 计算分数
        /// </summary>
        int CalculateScore(int matchCount)
        {
            switch (matchCount)
            {
                case 4: return 100;    // 四消基础分
                case 5: return 300;    // 五消得分
                case 6: return 600;    // 六消得分
                case 7: return 1000;   // 七消得分
                default: return matchCount * 200; // 更多消除
            }
        }

        /// <summary>
        /// 添加分数
        /// </summary>
        void AddScore(int score)
        {
            currentScore += score;
            Debug.Log($"当前分数: {currentScore}");

            // 检查是否达到目标
            if (currentScore >= targetScore)
            {
                GameWin();
            }
        }

        /// <summary>
        /// 填充空位
        /// </summary>
        IEnumerator FillEmptySpaces()
        {
            yield return new WaitForSeconds(0.5f);

            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (board[x, y] == ElementType.Special) // 空位标记
                    {
                        // 生成新元素
                        ElementType newType = (ElementType)Random.Range(0, 4);
                        CreateElement(x, y, newType);

                        // 从上方落下动画
                        Vector3 startPos = new Vector3(x, y + 5, 0);
                        elementObjects[x, y].transform.position = startPos;

                        StartCoroutine(MoveElementToPosition(elementObjects[x, y], new Vector3(x, y, 0), 0.3f));
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);

            // 检查新的匹配
            CheckAllMatches();
        }

        /// <summary>
        /// 元素移动动画
        /// </summary>
        IEnumerator MoveElementToPosition(GameObject element, Vector3 targetPos, float duration)
        {
            float elapsed = 0f;
            Vector3 startPos = element.transform.position;

            while (elapsed < duration)
            {
                element.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            element.transform.position = targetPos;
        }

        /// <summary>
        /// 检查所有匹配
        /// </summary>
        void CheckAllMatches()
        {
            List<Vector2Int> allMatches = new List<Vector2Int>();

            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    allMatches.AddRange(FindMatchesAt(x, y));
                }
            }

            if (allMatches.Count > 0)
            {
                ProcessMatches(allMatches);
                StartCoroutine(FillEmptySpaces());
            }
        }

        /// <summary>
        /// 检查初始匹配（避免开局就有匹配）
        /// </summary>
        void CheckInitialMatches()
        {
            bool hasMatches = true;
            int attempts = 0;

            while (hasMatches && attempts < 10)
            {
                hasMatches = false;
                List<Vector2Int> initialMatches = new List<Vector2Int>();

                for (int x = 0; x < boardWidth; x++)
                {
                    for (int y = 0; y < boardHeight; y++)
                    {
                        initialMatches.AddRange(FindMatchesAt(x, y));
                    }
                }

                if (initialMatches.Count > 0)
                {
                    hasMatches = true;
                    // 重新生成有匹配的位置
                    foreach (Vector2Int match in initialMatches)
                    {
                        Destroy(elementObjects[match.x, match.y]);
                        ElementType newType = (ElementType)Random.Range(0, 4);
                        CreateElement(match.x, match.y, newType);
                    }
                }

                attempts++;
            }
        }

        /// <summary>
        /// 生成道具
        /// </summary>
        void GeneratePropAt(int x, int y, PropType propType)
        {
            GameObject propPrefab = null;

            switch (propType)
            {
                case PropType.Horizontal:
                    propPrefab = horizontalProp;
                    break;
                case PropType.Vertical:
                    propPrefab = verticalProp;
                    break;
                case PropType.Bomb:
                    propPrefab = bombProp;
                    break;
                case PropType.Wild:
                    propPrefab = wildProp;
                    break;
            }

            if (propPrefab != null && elementObjects[x, y] != null)
            {
                // 在元素位置生成道具
                Destroy(elementObjects[x, y]);
                GameObject prop = Instantiate(propPrefab, new Vector3(x, y, 0), Quaternion.identity);
                elementObjects[x, y] = prop;

                // 添加道具脚本
                GameProp propScript = prop.AddComponent<GameProp>();
                propScript.Initialize(propType, x, y);
                propScript.OnPropClicked += OnPropClicked;
            }
        }

        /// <summary>
        /// 道具点击事件
        /// </summary>
        void OnPropClicked(PropType propType, int x, int y)
        {
            switch (propType)
            {
                case PropType.Horizontal:
                    ActivateHorizontalProp(x, y);
                    break;
                case PropType.Vertical:
                    ActivateVerticalProp(x, y);
                    break;
                case PropType.Bomb:
                    ActivateBombProp(x, y);
                    break;
                case PropType.Wild:
                    ActivateWildProp(x, y);
                    break;
            }

            // 清除道具
            Destroy(elementObjects[x, y]);
            elementObjects[x, y] = null;
            board[x, y] = ElementType.Special;

            StartCoroutine(FillEmptySpaces());
        }

        /// <summary>
        /// 激活横向道具
        /// </summary>
        void ActivateHorizontalProp(int x, int y)
        {
            for (int i = 0; i < boardWidth; i++)
            {
                if (elementObjects[i, y] != null)
                {
                    Destroy(elementObjects[i, y]);
                    elementObjects[i, y] = null;
                    board[i, y] = ElementType.Special;
                }
            }
        }

        /// <summary>
        /// 激活竖向道具
        /// </summary>
        void ActivateVerticalProp(int x, int y)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (elementObjects[x, j] != null)
                {
                    Destroy(elementObjects[x, j]);
                    elementObjects[x, j] = null;
                    board[x, j] = ElementType.Special;
                }
            }
        }

        /// <summary>
        /// 激活炸弹道具
        /// </summary>
        void ActivateBombProp(int x, int y)
        {
            // 3x3范围消除
            for (int i = Mathf.Max(0, x - 1); i <= Mathf.Min(boardWidth - 1, x + 1); i++)
            {
                for (int j = Mathf.Max(0, y - 1); j <= Mathf.Min(boardHeight - 1, y + 1); j++)
                {
                    if (elementObjects[i, j] != null)
                    {
                        Destroy(elementObjects[i, j]);
                        elementObjects[i, j] = null;
                        board[i, j] = ElementType.Special;
                    }
                }
            }
        }

        /// <summary>
        /// 激活Wild道具
        /// </summary>
        void ActivateWildProp(int x, int y)
        {
            // 随机选择一种颜色消除
            ElementType randomType = (ElementType)Random.Range(0, 4);

            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (board[i, j] == randomType && elementObjects[i, j] != null)
                    {
                        Destroy(elementObjects[i, j]);
                        elementObjects[i, j] = null;
                        board[i, j] = ElementType.Special;
                    }
                }
            }
        }

        /// <summary>
        /// 游戏胜利
        /// </summary>
        void GameWin()
        {
            Debug.Log("游戏胜利！达到目标分数！");
            // 这里可以触发胜利界面、奖励发放等
        }
    }
}


