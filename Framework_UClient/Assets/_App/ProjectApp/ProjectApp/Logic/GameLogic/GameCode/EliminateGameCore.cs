using FutureCore;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

        public int BoardWidth => boardSize.x;
        public int BoardHeight => boardSize.y;

        #endregion

        // 当前分数
        public int currentScore = 0;
        public int targetScore = 100000; // 目标分数

        public ElementData GetElementData(Vector2Int pot)
        {
            if (pot.x >= BoardWidth || pot.y >= BoardHeight)
            {
                return default;
            }
            return boardData[pot.x, pot.y];
        }

        public void SetElementData(ElementData elementData)
        {
            if (elementData.X >= BoardWidth || elementData.Y >= BoardHeight)
            {
                return;
            }
            boardData[elementData.X, elementData.Y] = elementData;
        }
    }


    public class EliminateGameCore : MonoBehaviour
    {
        [Header("游戏配置")]
        [SerializeField] private int _boardWidth = 10;    // 棋盘宽度
        [SerializeField] private int _boardHeight = 14;   // 棋盘高度
        [SerializeField] public Vector3 startVector3;   // 棋盘高度
        [SerializeField] private GameMode currentMode = GameMode.BuildHive;

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
        private GameInitial_Module gameInitialModule;
        private GameRule_Module gameRuleModule;
        private VisualEffects_Module visualEffectsModule;
        private List<IGameModule> gameModules = new List<IGameModule>();
        
        /// <summary>
        /// 能否操作 Controller
        /// </summary>
        public bool Enabled_PlayerCtr { get => _enabledCtrSum > 0; set { if (value) _enabledCtrSum++; else _enabledCtrSum--; } }
        /// <summary>
        /// 操作计数器
        /// </summary>
        private int _enabledCtrSum = 0;

        private void Awake()
        { 
            Dispatcher = new Dispatcher<uint>();
        }

        void Start()
        {
            GameTool.GameCore = this;

            Data = new EliminateGameData();
            

            gameInitialModule = new GameInitial_Module();

            gameRuleModule = new GameRule_Module();
            visualEffectsModule = new VisualEffects_Module();


            gameModules.Add(gameInitialModule);
            gameModules.Add(gameRuleModule);
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
            Enabled_PlayerCtr = true;

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
            foreach (var item in gameModules)
            {
                item.GenerateInitialElements();
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
            ElementType elementType =  ElementType.Fixed_None;
            // 根据配置表比例生成元素（这里简化为随机）
            int rand = GameTool.RandomToInt(1,6);

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

        public List<Vector2Int> FindAllMatches(List<Vector2Int> allMatches = null)
        {
            return gameRuleModule.FindAllMatches(allMatches);
        }
    }
}


