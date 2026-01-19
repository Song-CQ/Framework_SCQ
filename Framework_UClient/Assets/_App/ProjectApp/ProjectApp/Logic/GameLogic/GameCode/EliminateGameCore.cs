using FutureCore;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectApp
{
   

    public enum GameMode
    {
        BuildHive,      // 建造蜂巢模式
        WorkerBeeChallenge // 工蜂挑战模式
    }

    public class EliminateGameData
    {
        #region 棋盘数据
        public ElementData[,] boardData;
        // 当前选中的元素
        public Vector2Int selectedElement = new Vector2Int(-1, -1);

        public Vector2Int boardSize;

        #endregion

        // 当前分数
        public int currentScore = 0;
        public int targetScore = 100000; // 目标分数

    }


    public class EliminateGameCore : MonoBehaviour
    {
        [Header("游戏配置")]
        [SerializeField] private int _boardWidth = 10;    // 棋盘宽度
        [SerializeField] private int _boardHeight = 14;   // 棋盘高度
        [SerializeField] private Vector3 startVector3;   // 棋盘高度
        [SerializeField] private GameMode currentMode = GameMode.BuildHive;

        [Header("元素预设")]
        [SerializeField] private Sprite[] elementIocns; // 0-3为基础元素，4为特殊元素

        [Header("道具预设")]
        [SerializeField] private GameObject horizontalProp;   // 横向消除道具
        [SerializeField] private GameObject verticalProp;     // 竖向消除道具
        [SerializeField] private GameObject bombProp;         // 炸弹道具
        [SerializeField] private GameObject wildProp;         // Wild道具



        #region 棋盘属性


        public ElementData[,] BoardData => Data.boardData;
        // 当前选中的元素
        public Vector2Int SelectedElement => Data.selectedElement;

        public Vector2Int BoardSize => Data.boardSize;


        public ElementItem[,] ElementItems => visualEffectsModule.elementItems;

        public int CurrentScore => Data.currentScore;

        #endregion







        /// <summary>
        /// 能否操作
        /// </summary>
        public bool EndedCtr { get => _endedCtrSum > 0; set { if (value) _endedCtrSum++; else _endedCtrSum--; } }
        /// <summary>
        /// 操作计数器
        /// </summary>
        private int _endedCtrSum = 0;

        private void Awake()
        { 
            Dispatcher = new Dispatcher<uint>();
        }

        

        #region 消息派发

        public Dispatcher<uint> Dispatcher { get; private set; }

        public void Dispatch(uint msg, params object[] param)
        {
            Dispatcher.Dispatch(msg,param);
        }

        public void AddListener(uint msg, Action<object> paramCB)
        {
            Dispatcher.AddListener(msg, paramCB);
        }

        public void RemoveListener(uint msg, Action<object> paramCB)
        {
            Dispatcher.RemoveListener(msg, paramCB);
        }

        #endregion

        public EliminateGameData Data { get; private set; }
        private GameRules_Module gameRuleMode;
        private VisualEffects_Module visualEffectsModule;
        private List<IGameModule> gameModules = new List<IGameModule>();

        void Start()
        {
            GameTool.GameCore = this;

            Data = new EliminateGameData();

            gameRuleMode = new GameRules_Module();
            visualEffectsModule = new VisualEffects_Module();


            gameModules.Add(gameRuleMode);
            gameModules.Add(visualEffectsModule);


            foreach (var item in gameModules)
            {
                item.FillCore(this);
            }

            RegisterEvent();

            //设置种子
            GameTool.SetRandomSeed(132131231);
           
            InitializeBoard(_boardWidth,_boardHeight);
            
            GenerateInitialElements();

            //允许操作
            EndedCtr = true;

        }

        private void RegisterEvent()
        {
            foreach (var item in gameModules)
            {
                item.AddListener();
            }
            
            
        }
        private void UnregisterEvent()
        {
            foreach (var item in gameModules)
            {
                item.RemoveListener();
            }
        }

        /// <summary>
        /// 初始化棋盘
        /// </summary>
        void InitializeBoard(int boardWidth,int boardHeight)
        {
            Data.boardData = new ElementData[boardWidth, boardHeight];
            Data.boardSize = new Vector2Int(boardWidth,boardHeight);
            foreach (var item in gameModules)
            {
                item.InitializeBoard( boardWidth, boardHeight);
            }

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
                    ElementType type = GetRandomElementType();
                    

                    CreadElement(x, y, type);
                }
            }

            // 检查并消除初始匹配
            CheckInitialMatches();
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
                        CreadElement(match.x, match.y, newType);
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

        public void Disposed()
        {
            UnregisterEvent();
            foreach (var item in gameModules)
            {
                item.Disposed();
            }
        }

        public ElementType GetRandomElementType()
        {
            ElementType elementType =  ElementType.Baihe;
            // 根据配置表比例生成元素（这里简化为随机）
            int rand = GameTool.RandomToInt(0,5);

            elementType = (ElementType)rand;
            return elementType;
            //if (rand < 0.7f) // 70%为基础元素
            //{
            //    type = (ElementType)Random.Range(0, 5);
            //}
            //else if (rand < 0.9f) // 20%为特殊元素
            //{
            //    type = ElementType.Special;
            //}
            //else // 10%随机生成道具（简化处理）
            //{
            //    type = (ElementType)Random.Range(0, 4);
            //}

        }
    }
}


