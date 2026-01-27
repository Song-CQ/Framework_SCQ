using ConsoleE;
using FutureCore;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ProjectApp
{


    public enum GameMode
    {
        BuildHive,      // 建造蜂巢模式
        WorkerBeeChallenge // 工蜂挑战模式
    }
    [Serializable]
    public class ElementGameData
    {
        #region 棋盘数据


        //上一次操作的数据快照
        private List<ElementData[,]> lastBoardDataList = new List<ElementData[,]>(GameTool.maxUndoSum);
        [SerializeField]
        public ElementData[,] boardData;
        // 当前选中的元素
        public Vector2Int selectedElement = new Vector2Int(-1, -1);

        public Vector2Int boardSize;

        public int BoardWidth => boardSize.x;
        public int BoardHeight => boardSize.y;
        
        //连接的棋盘位置 [5,0]和[5,1] 连接
        public Vector2Int[,] linkBoardPot;

        public int linkBoardPotLength ;

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

        public void Dispose()
        {
            currentScore  = 0;
            targetScore  = 0;
            boardData = null;
            lastBoardDataList.Clear();
            selectedElement = new Vector2Int(-1, -1);
            boardSize = new Vector2Int(0, 0);
        }

        /// <summary>
        /// 记录快照
        /// </summary>
        public void TakeMemorySnapshotBoardData()
        {
            var lastBoardData = (ElementData[,])boardData.Clone();
            if (lastBoardDataList.Count >= GameTool.maxUndoSum)
            {
                lastBoardDataList.RemoveAt(0);
            }
            lastBoardDataList.Add(lastBoardData);
        }

        public void SetBoardData(ElementData[,] newBoardData)
        {
            boardData = (ElementData[,])newBoardData.Clone();
        }

        public bool UndoStepBoardData()
        {
            if(!CanUndo())return false;
            var data = lastBoardDataList[lastBoardDataList.Count-1];
            lastBoardDataList.RemoveAt(lastBoardDataList.Count -1);
            SetBoardData(data);
            return true;
        }

        public bool CanUndo()
        {
            if(lastBoardDataList.Count <=0)
            {
                return false;
            }
            return true;

        }

    }


    public class EliminateGameCore : MonoBehaviour
    {
        [Header("游戏配置")]
        [SerializeField] private int _boardWidth = 10;    // 棋盘宽度
        [SerializeField] private int _boardHeight = 14;   // 棋盘高度
        [SerializeField] public Vector3 startVector3;   // 棋盘高度
        [SerializeField] private GameMode currentMode = GameMode.BuildHive;



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
            if (param == null || param.Length == 0)
            {
                Dispatcher.Dispatch(msg);
            }
            else
                if (param.Length == 1)
            {
                Dispatcher.Dispatch(msg, param[0]);
            }
            else
            {
                Dispatcher.Dispatch(msg, (object)param);
            }
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

        public ElementGameData Data { private set; get; }
        private GameInitial_Module gameInitialModule;
        private GameRule_Module gameRuleModule;
        private VisualEffects_Module visualEffectsModule;
        private List<IGameModule> gameModules;

        private bool isInit = false;

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
            Init();
        }


        public void Init()
        {
            InputMgr.Instance.Init();
            InputMgr.Instance.StartUp();

            GameTool.GameCore = this;
            GameTool.SetRandomSeed(132131231);//设置种子
            GameTool.AllBaseElements = new ElementType[] { ElementType.Item_A, ElementType.Item_B, ElementType.Item_C, ElementType.Item_D };

            Data = new ElementGameData();
            Dispatcher = new Dispatcher<uint>();

            gameInitialModule = new GameInitial_Module();

            gameRuleModule = new GameRule_Module();
            visualEffectsModule = new VisualEffects_Module();

            gameModules = new List<IGameModule>();
            gameModules.Add(gameInitialModule);
            gameModules.Add(gameRuleModule);
            gameModules.Add(visualEffectsModule);


            //填充核心
            FillCore();
            //注册事件
            RegisterEvent();
            //初始化棋盘
            InitializeBoard(_boardWidth, _boardHeight);
            //生成元素
            GenerateInitialElements();

            //允许操作
            _enabledCtrSum = 0;
            Enabled_PlayerCtr = true;


            isInit = true;
        }

        private void FillCore()
        {
            foreach (var item in gameModules)
            {
                item.FillCore(this);
            }
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
        private void InitializeBoard(int boardWidth, int boardHeight)
        {
            foreach (var item in gameModules)
            {
                item.InitializeBoard(boardWidth, boardHeight);
            }

        }


        /// <summary>
        /// 生成初始元素（简化的随机生成）
        /// </summary>
        private void GenerateInitialElements()
        {
            foreach (var item in gameModules)
            {
                item.GenerateInitialElements();
            }

        }

        public Vector2Int temp1 = new Vector2Int(0, 13);
        public Vector2Int temp2 = new Vector2Int(0, 12);
        [Button("交换元素")]
        public void Test1()
        {
            Dispatcher.Dispatch(GameMsg.ClickElement, new ElementData().SetPot(temp1.x, temp1.y));
            Dispatcher.Dispatch(GameMsg.ClickElement, new ElementData().SetPot(temp2.x, temp2.y));

        }

        [Button("重新开始")]
        public void ResetGame()
        {
            Dispose();


            Invoke("Init", 1);


        }
        [Button("重新随机")]
        public void Ranan()
        {
            gameRuleModule.Player_RananAllElement();
        }


        /// <summary>
        /// 游戏胜利
        /// </summary>
        private void GameWin()
        {
            Debug.Log("游戏胜利！达到目标分数！");
            // 这里可以触发胜利界面、奖励发放等
        }


        private void Update()
        {
            if (!isInit) return;

            visualEffectsModule.Update();

        }

        public ElementData GetRandomElementData()
        {
            ElementType elementType = ElementType.Fixed_None;
            // 根据配置表比例生成元素（这里简化为随机）
            int rand = GameTool.RandomToInt(1, 6);

            elementType = (ElementType)rand;

            ElementData data = new ElementData(elementType);

            ///特殊元素
            if (data.Type == ElementType.Item_Change)
            {
                GameTool.YatesElements();
                var values = GameTool.AllBaseElements;

                data.data1 = (int)values[0];
                data.data2 = (int)values[1];
                data.data3 = (int)values[2];

            }

            return data;
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

        public void Dispose()
        {
            UnregisterEvent();

            foreach (var item in gameModules)
            {
                item.Dispose();
            }
            gameInitialModule = null;
            gameRuleModule = null;
            visualEffectsModule = null;

            gameModules.Clear();
            gameModules = null;

            Data.Dispose();
            Data = null;

            Dispatcher.Dispose();
            Dispatcher = null;


            isInit = false;
        }
        
        #region 玩家的操作 点击元素 拖动元素 
        public void OnClickElementItem(ElementItem elementItem)
        {
            if(!Enabled_PlayerCtr) return;
            Dispatch(GameMsg.ClickElement,elementItem);
        }

        public void OnSwipeElementItem(ElementItem startItem, ElementItem endItem)
        {
            if(!Enabled_PlayerCtr) return;
            if(startItem!=null&&endItem!=null)
            {
                Dispatch(GameMsg.SwipeElement,startItem.Data,endItem.Data);
            }

        }

        #endregion
    }
}


