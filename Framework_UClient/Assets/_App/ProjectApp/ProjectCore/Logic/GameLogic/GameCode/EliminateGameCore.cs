using FutureCore;
using ProjectApp.Data;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;


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


        public int linkBoardPotLength;
        /// <summary>
        /// 连接的棋盘位置 long值哈希表
        /// </summary>
        // 从Key到Connection的映射
        //连接的棋盘位置 [5,0]和[5,1] 连接
        private Dictionary<long, (Vector2Int, Vector2Int)> _keyToConnection = new Dictionary<long, (Vector2Int, Vector2Int)>();


        #endregion

        // 当前分数
        public int currentScore = 0;
        public int targetScore = 100000; // 目标分数

        public int activeSum;//激活的位置数量







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

        public bool HasConnection(int x1, int y1, int x2, int y2)
        {
            return HasConnection(new Vector2Int(x1, y1), new Vector2Int(x2, y2));
        }

        // 快速检查连接是否存在
        public bool HasConnection(Vector2Int point1, Vector2Int point2)
        {
            long key = GameTool.EncodeConnection(ref point1, ref point2);
            return _keyToConnection.ContainsKey(key);
        }

        // 添加连接
        public bool AddConnection(Vector2Int point1, Vector2Int point2)
        {
            long key = GameTool.EncodeConnection(ref point1, ref point2);
            // 检查是否重复（双向检查）
            if (!_keyToConnection.ContainsKey(key))
            {
                _keyToConnection[key] = (point1, point2);
                return true;
            }
            return false;
        }

        // 移除连接
        public bool RemoveConnection(Vector2Int point1, Vector2Int point2)
        {
            long key = GameTool.EncodeConnection(ref point1, ref point2);
            if (_keyToConnection.ContainsKey(key))
            {
                _keyToConnection.Remove(key);
                return true;
            }
            return false;
        }

        public Dictionary<long, (Vector2Int, Vector2Int)> GetAllConnection()
        {
            return _keyToConnection;
        }





        public void Dispose()
        {
            currentScore = 0;
            targetScore = 0;
            activeSum = 0;
            boardData = null;
            lastBoardDataList.Clear();

            linkBoardPotLength = 0;
            _keyToConnection.Clear();

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

        public void DelLastMemorySnapshotBoardData()
        {
            if (CanUndo())
            {
                lastBoardDataList.RemoveAt(lastBoardDataList.Count - 1);
            }
        }

        public void SetBoardData(ElementData[,] newBoardData)
        {
            boardData = (ElementData[,])newBoardData.Clone();
        }

        public bool UndoStepBoardData()
        {
            if (!CanUndo()) return false;
            var data = lastBoardDataList[lastBoardDataList.Count - 1];
            lastBoardDataList.RemoveAt(lastBoardDataList.Count - 1);
            SetBoardData(data);
            return true;
        }

        public bool CanUndo()
        {
            if (lastBoardDataList.Count <= 0)
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
        
        public int TargetScore
        {
            get => Data.targetScore;
        }

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

                    Dispatcher.Dispatch(msg, param);
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
        private GameEnd_Module gameEndModule;
        private GameRule_Module gameRuleModule;
        private VisualEffects_Module visualEffectsModule;
        private ExternalProp_Module externalProp_Module;
        private Dictionary<Type, IGameModule> gameModules;

        private bool isInit = false;
        
        [SerializeField]
        private LevelVO levelData;

        [SerializeField]
        private int levelID;

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
            if (isEditor) Init();
        }

        [Button("Init")]
        public void Init(int _levelID = 0)
        {
            if (isEditor)
            {
                InputMgr.Instance.Init();
                InputMgr.Instance.StartUp();

                ConfigDataMgr.Instance.ResetData();
                ConfigDataMgr.Instance.ReadData();

                
                

                ConfigDataMgr.Instance.Init();
                ConfigDataMgr.Instance.StartUp();
                CameraMgr.Instance.mainCamera = Camera.main;
            }


            GameTool.GameCore = this;
            GameTool.SetRandomSeed(MainUI.GameRandomSeed);//设置种子
            GameTool.AllBaseElements = new ElementType[] { ElementType.Item_A, ElementType.Item_B, ElementType.Item_C, ElementType.Item_D };

            Data = new ElementGameData();

            Dispatcher = new Dispatcher<uint>();

            gameInitialModule = new GameInitial_Module();
            gameEndModule = new GameEnd_Module();
            externalProp_Module = new ExternalProp_Module();

            gameRuleModule = new GameRule_Module();
            visualEffectsModule = new VisualEffects_Module();

            gameModules = new Dictionary<Type, IGameModule>();
            gameModules.Add(gameInitialModule.GetType(), gameInitialModule);
            gameModules.Add(gameEndModule.GetType(), gameEndModule);
            gameModules.Add(gameRuleModule.GetType(), gameRuleModule);
            gameModules.Add(visualEffectsModule.GetType(), visualEffectsModule);
            gameModules.Add(externalProp_Module.GetType(), externalProp_Module);

            _enabledCtrSum = 0;
            Enabled_PlayerCtr = false;
            isInit = true;

            if (_levelID != 0)
            {
                levelID = _levelID;
            }

            levelData = LevelVOModel.Instance.GetVO(levelID);

            if (levelData == null)
            {
                LogUtil.LogError("ID : " + levelID + " 关卡数据为空");
                return;
            }

            


            //解析关卡表数据
            Parse_CfgData();

            //开始
            GameStart();



        }

        private void Parse_CfgData()
        {
            Data.targetScore = levelData.Passing_Score;
            if (isEditor)
            {
                Data.targetScore = 1000000;
            }

            allElement_Rate = 0;

            prop_Rate = levelData.Proportion_Of_Props;
            special_Rate = levelData.Special_Element;
            allElement_Rate = special_Rate + prop_Rate;
            baseType_Rate = new List<int>();
            foreach (var item in levelData.Basic_Proportion)
            {
                baseType_Rate.Add(item);
                allElement_Rate += item;
            }


            //wild 加其他道具
            wildAndBomb_Rate = GeneralStaticVO.Instance.Proportion_Wild_Bomb;
            wildAndHorizontal_Rate = GeneralStaticVO.Instance.Proportion_Wild_H_V;
            wildAndVertical_Rate = GeneralStaticVO.Instance.Proportion_Wild_H_V;

        }

        private void GameStart()
        {

            //填充核心
            FillCore();
            //注册事件
            RegisterEvent();
            //初始化棋盘
            InitializeBoard(_boardWidth, _boardHeight);
            //生成元素
            GenerateInitialElements();

            //开始
            Dispatch(GameMsg.GameStart);
            UICtrlDispatcher.Instance.Dispatch(UICtrlMsg.GameUI_Open);

            //允许操作
            _enabledCtrSum = 0;
            Enabled_PlayerCtr = true;
        }

        private void FillCore()
        {
            foreach (var item in gameModules)
            {
                item.Value.FillCore(this);
            }
        }

        private void RegisterEvent()
        {
            foreach (var item in gameModules)
            {
                item.Value.AddListener();
            }

        }
        private void UnregisterEvent()
        {
            foreach (var item in gameModules)
            {
                item.Value.RemoveListener();
            }
        }

        /// <summary>
        /// 初始化棋盘
        /// </summary>
        private void InitializeBoard(int boardWidth, int boardHeight)
        {
            foreach (var item in gameModules)
            {
                item.Value.InitializeBoard(boardWidth, boardHeight);
            }

        }

        public T GetModule<T>() where T : IGameModule
        {
            Type type = typeof(T);
            gameModules.TryGetValue(type, out IGameModule module);

            return (T)module;
        }
        /// <summary>
        /// 生成初始元素（简化的随机生成）
        /// </summary>
        private void GenerateInitialElements()
        {
            foreach (var item in gameModules)
            {
                item.Value.GenerateInitialElements();
            }

        }

        #region 测试

        public Vector2Int temp1 = new Vector2Int(0, 13);
        public Vector2Int temp2 = new Vector2Int(0, 12);

        [LabelText("是否编辑器模式")]
        public bool isEditor;
        [LabelText("是否填充")]
        public bool IsFill;
        [LabelText("是否触发组合道具 点击模式")]
        public bool IsClickProp;
        [LabelText("是否使用对象池动画")]
        public bool isPool = true;
        [LabelText("是否检查元素消除")]
        public bool isCheckAllMatches = true;
        [LabelText("无消除是否要交换回元素")]
        public bool IsBackSwap = true;


        [Button("交换元素")]
        public void Test1()
        {
            Dispatcher.Dispatch(GameMsg.Player_ClickElement, new ElementData().SetPot(temp1.x, temp1.y));
            Dispatcher.Dispatch(GameMsg.Player_ClickElement, new ElementData().SetPot(temp2.x, temp2.y));

        }

        [Button("重新开始")]
        public void ResetGame()
        {
            Dispose();


            Init();


        }
        [Button("重新随机")]
        public void Ranan()
        {
            gameRuleModule.Player_RananAllElement();
        }

        #endregion


        /// <summary>
        /// 游戏胜利
        /// </summary>
        public void GameWin()
        {
            Debug.Log("游戏胜利！达到目标分数！");
            // 这里可以触发胜利界面、奖励发放等
            Dispatch(GameMsg.GameWin);

            UICtrlDispatcher.Instance.Dispatch(UICtrlMsg.GameWinUI_Open);
        }

        public void ExitGame()
        {
            Dispatch(GameMsg.GameOver);



        }


        private void Update()
        {
            if (!isInit) return;

            visualEffectsModule.Update();

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
                item.Value.Dispose();
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

            GameTool.GameCore = null;

            baseType_Rate.Clear();
            baseType_Rate = null;
            levelData = null;

            isInit = false;
        }

        #region 生成逻辑 Tools

        private int allElement_Rate;
        private int special_Rate;
        private int prop_Rate;
        private List<int> baseType_Rate;

        public ElementData GetRandomElementData()
        {
            ElementType elementType = ElementType.Fixed_None;
            // 根据配置表比例生成元素

            int rand = GameTool.RandomToInt(0, allElement_Rate);

            while (true)
            {
                if (rand < prop_Rate) //道具
                {
                    elementType = (ElementType)GameTool.RandomToInt(101, 105);
                    break;
                }
                rand -= prop_Rate;

                if (rand < special_Rate) ///特殊元素
                {
                    elementType = ElementType.Item_Special;
                    break;
                }
                rand -= special_Rate;

                for (int i = 0; i < baseType_Rate.Count; i++)
                {
                    int item_Rate = baseType_Rate[i];

                    if (rand < item_Rate)
                    {
                        elementType = (ElementType)(i + 1);
                        break;
                    }
                    rand -= item_Rate;
                }

                if (elementType == ElementType.Fixed_None)
                {
                    elementType = (ElementType)GameTool.RandomToInt(1, 5);
                }

                break;
            }

            ElementData data = new ElementData(elementType);

            ///特殊元素
            if (data.Type == ElementType.Item_Special)
            {
                GameTool.YatesElements();
                var values = GameTool.AllBaseElements;

                data.data1 = (int)values[0];
                data.data2 = (int)values[1];
                data.data3 = (int)values[2];

            }

            return data;

        }

        public int GetActiveSum(bool isRest = false)
        {
            if (isRest)
            {
                int sum = 0;
                foreach (var item in Data.boardData)
                {
                    if (ElementTool.CheckType_CanMatches(item.Type))
                    {
                        sum++;
                    }
                }
                Data.activeSum = sum;

            }

            return Data.activeSum;
        }

        private int wildAndBomb_Rate;
        private int wildAndHorizontal_Rate;
        private int wildAndVertical_Rate;
        public int GetWildAndPropSum(ElementType type)
        {
            int rate = 0;
            switch (type)
            {
                case ElementType.Prop_Horizontal:
                    rate = wildAndHorizontal_Rate;
                    break;

                case ElementType.Prop_Vertical:
                    rate = wildAndVertical_Rate;
                    break;

                case ElementType.Prop_Bomb:
                    rate = wildAndBomb_Rate;
                    break;

            }

            if (rate == 0)
            {
                LogUtil.LogError("该道具没有和Wild 道具组合");
                return 0;
            }

            int allSum = GetActiveSum();
            int val = (int)Math.Round(allSum / (rate / 100.0));

            return val;

        }
        
        
        #endregion


        #region 外置道具

        public ExternalProp SelectExternalProp => externalProp_Module.SelectExternalProp;

        public int GetExternalProp_AddSocre()
        {
            return externalProp_Module.GetExternalProp_AddSocre();
        }




        #endregion


        #region 玩家的操作 点击元素 拖动元素 
        public void ClickElementItem(ElementItem elementItem)
        {
            if (!Enabled_PlayerCtr) return;
            Dispatch(GameMsg.Player_ClickElement, elementItem.Data);
        }

        public void SwipeItemToItem(ElementItem startItem, ElementItem endItem)
        {
            if (!Enabled_PlayerCtr) return;

            if (startItem != null && endItem != null && startItem != endItem)
            {
                //拖动一个元素到另一个元素
                Dispatch(GameMsg.Player_SwipeElementToElement, startItem.Data, endItem.Data);
            }


        }

        public void SwipeElementItem(ElementItem startItem, Vector2 dir)
        {
            if (!Enabled_PlayerCtr) return;

            Dispatch(GameMsg.Player_SwipeElement, startItem.Data, dir);
        }

        public void ClickExternalPropItem(ExternalProp type)
        {
            if (!Enabled_PlayerCtr) return;



            Dispatch(GameMsg.Player_ClickExternalPropItem, type);


        }














        #endregion


        #region Model Tool
        public (Vector2 L_D, Vector2 L_U, Vector2 R_D, Vector2 R_U) GetBoardBoundsToScreenPoint()
        {
            if (visualEffectsModule == null) return (Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);

            return visualEffectsModule.GetBoardBoundsToScreenPoint();
        }

        #endregion
    }
}


