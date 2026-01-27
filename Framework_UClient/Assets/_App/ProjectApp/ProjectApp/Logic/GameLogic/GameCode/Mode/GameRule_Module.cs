using FutureCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectApp
{
    public class GameRule_Module : IGameModule
    {
        private Vector2Int SelectedElement
        {
            get => Data.selectedElement;
            set => Data.selectedElement = value;
        }
        private ElementData[,] BoardData => Data.boardData;

        private Vector2Int boardSize;
        private bool[,] _visited;
        public GameRule_Module() { }

        #region 临时数据 
        private List<Vector2Int> temp_AllMatchesList = new List<Vector2Int>();
        #endregion

        #region 流程
        public Dispatcher<uint> Dispatcher => Core.Dispatcher;
        public ElementGameData Data => Core.Data;

        public EliminateGameCore Core { get; private set; }

        public void FillCore(EliminateGameCore _core)
        {
            Core = _core;
        }

        public void InitializeBoard(int w, int h)
        {
            boardSize = new Vector2Int(w, h);
            _visited = new bool[boardSize.x, boardSize.y];

        }



        public void AddListener()
        {
            //最先运行
            Dispatcher.AddPriorityListener(GameMsg.ClickElement, OnClickElement_test);
            Dispatcher.AddPriorityListener(GameMsg.SwipeElement, OnSwipe_Element);

        }



        public void RemoveListener()
        {

            Dispatcher.RemovePriorityListener(GameMsg.ClickElement, OnClickElement_test);
            Dispatcher.RemovePriorityListener(GameMsg.SwipeElement, OnSwipe_Element);
        }

        public void GenerateInitialElements()
        {

        }
        public void Dispose()
        {
            RemoveListener();


            Core = null;
            _visited = null;
            temp_AllMatchesList = null;
            boardSize = Vector2Int.zero;

            ListPool<ElementData>.Clear();

        }

        #endregion


        private const string lockStr = "loack";
        void OnClickElement_test(object o)
        {
            OnClick_Element(o);
            return;
            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    Task task = Task.Run(() =>
                    {
                        Thread currentThread = Thread.CurrentThread;
                        Debug.Log("当前线程" + currentThread.ManagedThreadId + ("  " + SelectedElement.x + "--" + SelectedElement.y));
                        OnClick_Element(o);
                    }
                     , cts.Token);
                    task.Wait(cts.Token);  // 同步等待，会抛出异常
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("OnElementClicked 执行超时，可能死循环");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    Debug.LogError($"OnElementClicked 执行出错: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"OnElementClicked 执行出错: {ex.Message}");
            }

        }



        #region  核心代码

        /// <summary>
        /// 点击元素
        /// </summary>
        /// <param name="o"></param>
        void OnClick_Element(object o)
        {

            ElementData element = (ElementData)(o);
            int x = element.X;
            int y = element.Y;

            if (SelectedElement.x < 0 || SelectedElement.y < 0)
            {
                // 第一次点击，选中元素
                SelectElement(x, y);
            }
            else
            {
                int select_X = SelectedElement.x;
                int select_Y = SelectedElement.y;
                // 清除选中状态
                DeselectElement(SelectedElement.x, SelectedElement.y);

                // 第二次点击，判断是否相邻
                if (IsAdjacent(select_X, select_Y, x, y) || Data.HasConnection(select_X, select_Y, x, y))
                {

                    Player_SwapElement(select_X, select_Y, x, y);

                }


            }

        }

        /// <summary>
        /// 拖动元素 到另一个元素上
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        void OnSwipe_Element(object obj)
        {
            object[] datas = obj as object[];

            ElementData data1 = (ElementData)datas[0];
            ElementData data2 = (ElementData)datas[1];

            if (IsAdjacent(data1.X, data1.Y, data2.X, data2.Y)
                || Data.HasConnection(data1.X, data1.Y, data2.X, data2.Y))
            {
                Player_SwapElement(data1.X, data1.Y, data2.X, data2.Y);
            }

        }


        /// <summary>
        /// 选中元素
        /// </summary>
        void SelectElement(int x, int y)
        {
            var elementData = BoardData[x, y];
            if (!ElementTypeTool.CheckType_ClickEvent(elementData.Type))
            {
                //不可点击
                return;
            }

            SelectedElement = new Vector2Int(x, y);
            Dispatcher.Dispatch(GameMsg.SelectElement, elementData);

        }
        /// <summary>
        /// 取消元素
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void DeselectElement(int x, int y)
        {
            var elementData = BoardData[x, y];
            SelectedElement = new Vector2Int(-1, -1);
            Dispatcher.Dispatch(GameMsg.DeselectElement, elementData);
        }

        #region 一次操作
        private void Player_SwapElement(int select_X, int select_Y, int x, int y)
        {
            //记录快照
            Data.TakeMemorySnapshotBoardData();

            // 交换元素
            SwapElements(select_X, select_Y, x, y);

            // 检查匹配
            List<Vector2Int> matches = FutureCore.ListPool<Vector2Int>.Get();
            matches = CheckMatchesAfterSwap(select_X, select_Y, x, y, ref matches);

            if (matches.Count > 0)
            {
                // 有匹配，进行消除
                ProcessMatches(matches);
                // 创建新元素 并补位
                FillEmptySpaces();
            }
            else
            {
                // 无匹配，交换回来
                SwapElements(select_X, select_Y, x, y);
            }
            //使用完回收List
            FutureCore.ListPool<Vector2Int>.Release(matches);


        }

        public void Player_RananAllElement()
        {
            //记录快照
            Data.TakeMemorySnapshotBoardData();


            for (int x = 0; x < Data.boardSize.x; x++)
            {
                for (int y = 0; y < Data.boardSize.y; y++)
                {
                    ElementData data = Core.GetRandomElementData();

                    SetBoardData(x, y, data);

                }
            }

            Dispatcher.Dispatch(GameMsg.RestAllElements);

            CheckAllMatches();
        }

        #endregion



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

            ElementData tempData1 = BoardData[x1, y1];
            ElementData tempData2 = BoardData[x2, y2];
            Debug.Log(string.Format("交换元素{0}  和 {1}", tempData1.ToString(), tempData2.ToString()));

            // 实际交换棋盘数据
            // 设置这个坐标位置 的数据
            SetBoardData(x1, y1, tempData2);
            SetBoardData(x2, y2, tempData1);

            // 通知其他模块 交换数据
            List<ElementData> elementDatas = FutureCore.ListPool<ElementData>.Get();
            elementDatas.Add(tempData1);
            elementDatas.Add(tempData2);
            Dispatcher.Dispatch(GameMsg.SwapElements, elementDatas);
            FutureCore.ListPool<ElementData>.Release(elementDatas);

        }

        /// <summary>
        /// 检查交换后的匹配
        /// </summary>
        private List<Vector2Int> CheckMatchesAfterSwap(int x1, int y1, int x2, int y2, ref List<Vector2Int> matches)
        {
            if (matches == null) matches = new List<Vector2Int>();
            var visited = GetVisited();
            // 检查交换的两个位置及其相关行列
            FindMatchesAt(x1, y1, visited, ref matches);
            FindMatchesAt(x2, y2, visited, ref matches);

            Debug.Log(string.Format("交换元素后消除的元素数量{0}", matches.Count));
            foreach (var item in matches)
            {
                Debug.Log(string.Format("分别是{0}", item.ToString()));
            }


            return matches;
        }


        /// <summary>
        /// 查找指定位置的匹配
        /// </summary>
        private List<Vector2Int> FindMatchesAt(int x, int y, bool[,] visited, ref List<Vector2Int> matches)
        {
            // 边界检查
            if (!IsPositionValid(x, y))
                return matches;

            ElementType type = BoardData[x, y].Type;
            if (!ElementTypeTool.CheckType_CanMatches(type))
                return matches;

            // 使用栈内存避免堆分配
            Span<Vector2Int> tempMatches = stackalloc Vector2Int[boardSize.x * boardSize.y];
            int matchCount = 0;

            //查找指定位置的垂直匹配
            FindHorizontalMatchesAt(x, y, 4, ref tempMatches, ref matchCount);
            //查找指定位置的垂直匹配
            FindVerticalMatchesAt(x, y, 4, ref tempMatches, ref matchCount);

            Span<Vector2Int> validMatches = tempMatches.Slice(0, matchCount);
            foreach (var item in validMatches)
            {
                if (!visited[item.x, item.y])
                {
                    //未加入过
                    matches.Add(item);
                    visited[item.x, item.y] = true;
                }

            }
            return matches;
        }


        #region 有错误的匹配方法
        /// <summary>
        /// 递归收集所有相邻的匹配（支持T形、L形等复杂匹配）
        /// </summary>
        private void CollectMatchesRecursive(int x, int y, ElementType type, Span<Vector2Int> matches, ref int matchCount)
        {
            // 检查是否已访问或类型不匹配
            if (!IsPositionValid(x, y) ||
                BoardData[x, y].Type != type ||
                ContainsPosition(matches, matchCount, new Vector2Int(x, y)))
                return;

            // 添加到匹配列表
            matches[matchCount++] = new Vector2Int(x, y);

            // 检查四个方向
            CheckAndCollect(x + 1, y, type, matches, ref matchCount); // 右
            CheckAndCollect(x - 1, y, type, matches, ref matchCount); // 左
            CheckAndCollect(x, y + 1, type, matches, ref matchCount); // 上
            CheckAndCollect(x, y - 1, type, matches, ref matchCount); // 下
        }

        private void CheckAndCollect(int x, int y, ElementType type, Span<Vector2Int> matches, ref int matchCount)
        {
            if (IsPositionValid(x, y) && BoardData[x, y].Type == type)
            {
                CollectMatchesRecursive(x, y, type, matches, ref matchCount);
            }
        }

        /// <summary>
        /// 检查位置是否已在匹配列表中
        /// </summary>
        private bool ContainsPosition(Span<Vector2Int> matches, int count, Vector2Int position)
        {
            for (int i = 0; i < count; i++)
            {
                if (matches[i] == position)
                    return true;
            }
            return false;
        }




        /// <summary>
        /// 查找匹配组（使用BFS）
        /// </summary>
        private List<Vector2Int> FindMatchGroup(int startX, int startY, bool[,] visited)
        {
            var matches = new List<Vector2Int>();
            var type = BoardData[startX, startY].Type;

            // 使用队列进行BFS
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(new Vector2Int(startX, startY));

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();

                if (visited[pos.x, pos.y]) continue;
                if (BoardData[pos.x, pos.y].Type != type) continue;

                visited[pos.x, pos.y] = true;
                matches.Add(pos);

                // 检查四个方向
                CheckAndEnqueue(pos.x + 1, pos.y, type, visited, queue);
                CheckAndEnqueue(pos.x - 1, pos.y, type, visited, queue);
                CheckAndEnqueue(pos.x, pos.y + 1, type, visited, queue);
                CheckAndEnqueue(pos.x, pos.y - 1, type, visited, queue);
            }

            return matches;
        }

        private void CheckAndEnqueue(int x, int y, ElementType type, bool[,] visited, Queue<Vector2Int> queue)
        {
            if (IsPositionValid(x, y) && !visited[x, y] && BoardData[x, y].Type == type)
            {
                queue.Enqueue(new Vector2Int(x, y));
            }
        }
        #endregion

        /// <summary>
        /// 查找指定位置的水平匹配
        /// </summary>
        private bool FindHorizontalMatchesAt(int x, int y, int minMatchCount, ref Span<Vector2Int> finalMatches, ref int finalMatchesCont)
        {
            if (!IsPositionValid(x, y))
                return false;

            ElementType type = ElementTypeTool.GetTypeToElementData(BoardData[x, y]);
            if (type == ElementType.Fixed_Special || type == ElementType.Fixed_None)
                return false;

            // 使用栈分配数组
            Span<Vector2Int> tempMatches = stackalloc Vector2Int[boardSize.x];
            int matchCount = 0;

            // 起始位置
            tempMatches[matchCount++] = new Vector2Int(x, y);

            // 向左检查
            for (int i = x - 1; i >= 0; i--)
            {
                ElementType tarType = ElementTypeTool.GetTypeToElementData(BoardData[i, y]);

                if (tarType == type)
                    tempMatches[matchCount++] = new Vector2Int(i, y);
                else
                    break;
            }

            // 向右检查
            for (int i = x + 1; i < boardSize.x; i++)
            {
                ElementType tarType = ElementTypeTool.GetTypeToElementData(BoardData[i, y]);
                if (tarType == type)
                    tempMatches[matchCount++] = new Vector2Int(i, y);
                else
                    break;
            }

            // 如果至少有3个匹配，创建最终列表
            if (matchCount >= minMatchCount)
            {
                foreach (var item in tempMatches.Slice(0, matchCount))
                {
                    finalMatches[finalMatchesCont++] = (item);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 查找指定位置的垂直匹配（单独方法，供特殊需求使用）
        /// </summary>
        private bool FindVerticalMatchesAt(int x, int y, int minMatchCount, ref Span<Vector2Int> finalMatches, ref int finalMatchesCont)
        {
            if (!IsPositionValid(x, y))
                return false;

            ElementType type = ElementTypeTool.GetTypeToElementData(BoardData[x, y]);
            if (type == ElementType.Fixed_Special || type == ElementType.Fixed_None)
                return false;

            // 使用栈分配数组
            Span<Vector2Int> tempMatches = stackalloc Vector2Int[boardSize.y];
            int matchCount = 0;

            // 起始位置
            tempMatches[matchCount++] = new Vector2Int(x, y);

            // 向下检查
            for (int j = y - 1; j >= 0; j--)
            {
                ElementType tarType = ElementTypeTool.GetTypeToElementData(BoardData[x, j]);
                if (tarType == type)
                    tempMatches[matchCount++] = new Vector2Int(x, j);
                else
                    break;
            }

            // 向上检查
            for (int j = y + 1; j < boardSize.y; j++)
            {
                ElementType tarType = ElementTypeTool.GetTypeToElementData(BoardData[x, j]);
                if (tarType == type)
                    tempMatches[matchCount++] = new Vector2Int(x, j);
                else
                    break;
            }

            // 如果至少有3个匹配，创建最终列表
            if (matchCount >= minMatchCount)
            {
                foreach (var item in tempMatches.Slice(0, matchCount))
                {
                    finalMatches[finalMatchesCont++] = (item);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 位置有效性检查
        /// </summary>
        private bool IsPositionValid(int x, int y)
        {
            return x >= 0 && x < boardSize.x &&
                   y >= 0 && y < boardSize.y;
        }

        /// <summary>
        /// 位置有效性检查（Vector2Int版本）
        /// </summary>
        private bool IsPositionValid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < boardSize.x &&
                   pos.y >= 0 && pos.y < boardSize.y;
        }


        /// <summary>
        /// 填充空位
        /// </summary>
        void FillEmptySpaces()
        {
            //要下落的元素
            List<ElementData> souList = ListPool<ElementData>.Get();
            //下落元素的目标
            List<ElementData> tarList = ListPool<ElementData>.Get();
            //新创建的元素位置
            List<ElementData> creadList = ListPool<ElementData>.Get();


            for (int x = 0; x < boardSize.x; x++)
            {
                int creadCont = 0;
                for (int y = 0; y < boardSize.y; y++)
                {
                    if (BoardData[x, y].Type == ElementType.Fixed_Special) // 空位标记
                    {
                        //要填充的位置
                        ElementData tar = BoardData[x, y];
                        //要填充的 源位置
                        ElementData sour = default;


                        //向上寻找到最近的不是空的物体
                        int temp_y = y + 1;
                        bool isCreate = false;
                        while (temp_y < boardSize.y)
                        {
                            ElementType type = BoardData[x, temp_y].Type;
                            //是空的 
                            if (ElementTypeTool.CheckType_UpEmpty(BoardData[x, temp_y].Type))
                            {
                                temp_y++;
                                continue;
                            }
                            //可下落
                            if (ElementTypeTool.CheckType_FillEmpty(type))
                            {
                                sour = BoardData[x, temp_y];
                                //下落了 将自身设置为空的
                                BoardData[x, temp_y].SetType(ElementType.Fixed_Special);
                            }
                            else
                            {
                                //碰到不可下落的元素 要创建新元素
                                isCreate = true;
                            }
                            break;
                        }


                        //最近的空
                        if (temp_y >= boardSize.y || isCreate)
                        {
                            // 生成新元素
                            sour = Core.GetRandomElementData();
                            //因为 是新创建的 所以可能 在高于棋盘的位置  
                            sour.SetPot(x, temp_y + creadCont);

                            creadCont++;
                            creadList.Add(sour);

                        }

                        souList.Add(sour);
                        tarList.Add(tar);

                        //设置当前的类型
                        SetBoardData(x, y, sour);
                    }
                }
            }

            Debug.LogError("要创建的元素数量" + creadList.Count);
            foreach (var item in creadList)
            {
                Debug.Log(item.ToString());
            }

            if (creadList.Count > 0)
            {
                //创建元素
                Dispatcher.Dispatch(GameMsg.GenerateElements, creadList);
            }
            ListPool<ElementData>.Release(creadList);

            Debug.LogError("要下落的元素数量" + souList.Count);
            for (int i = 0; i < souList.Count; i++)
            {
                ElementData item = souList[i];
                Debug.Log("下落元素" + item.ToString() + "目标位置" + tarList[i].ToString());
            }

            if (souList.Count > 0 && tarList.Count > 0)
            {
                // 下落元素
                Core.Dispatch(GameMsg.ElementsFall, souList, tarList);
            }
            ListPool<ElementData>.Release(souList);
            ListPool<ElementData>.Release(tarList);


            // 检查新的匹配

            CheckAllMatches();
        }

        private void SetBoardData(int x, int y, ElementData sour)
        {
            sour.SetPot(x, y);
            BoardData[x, y] = sour;
        }


        /// <summary>
        /// 检查所有匹配
        /// </summary>
        void CheckAllMatches()
        {
            // 使用对象池获取列表，避免GC分配
            var allMatches = FindAllMatches();

            if (allMatches.Count > 0)
            {
                //消除
                ProcessMatches(allMatches);
                //补位
                FillEmptySpaces();
            }

            allMatches.Clear();

        }

        /// <summary>
        /// 查找所有匹配
        /// </summary>
        public List<Vector2Int> FindAllMatches(List<Vector2Int> allMatches = null)
        {
            if (allMatches == null) allMatches = temp_AllMatchesList;

            allMatches.Clear();

            var visited = GetVisited();
            // 优化2：一次性收集所有匹配位置
            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    // 跳过已检查位置和空位
                    if (!ElementTypeTool.CheckType_CanMatches(BoardData[x, y].Type) || visited[x, y])
                    {
                        continue;
                    }

                    FindMatchesAt(x, y, visited, ref allMatches);
                }
            }

            return allMatches;
        }

        private bool[,] GetVisited()
        {
            if (_visited == null)
            {
                _visited = new bool[boardSize.x, boardSize.y];
            }
            else
            {
                System.Array.Clear(_visited, 0, _visited.Length);
            }

            return _visited;
        }

        /// <summary>
        /// 处理匹配消除
        /// </summary>
        void ProcessMatches(List<Vector2Int> matches)
        {

            // 四消规则：需要4个或更多相同元素
            //if (horizontalMatches.Count >= 4)
            //{
            //    matches.AddRange(horizontalMatches);

            //    // 生成道具（根据消除数量）
            //    if (horizontalMatches.Count == 5)
            //    {
            //        GeneratePropAt(x, y, PropType.Horizontal);
            //    }
            //    else if (horizontalMatches.Count == 6)
            //    {
            //        GeneratePropAt(x, y, PropType.Bomb);
            //    }
            //    else if (horizontalMatches.Count >= 7)
            //    {
            //        GeneratePropAt(x, y, PropType.Wild);
            //    }
            //}

            //if (verticalMatches.Count >= 4)
            //{
            //    matches.AddRange(verticalMatches);

            //    // 生成道具
            //    if (verticalMatches.Count == 5)
            //    {
            //        GeneratePropAt(x, y, PropType.Vertical);
            //    }
            //    else if (verticalMatches.Count == 6)
            //    {
            //        GeneratePropAt(x, y, PropType.Bomb);
            //    }
            //    else if (verticalMatches.Count >= 7)
            //    {
            //        GeneratePropAt(x, y, PropType.Wild);
            //    }
            //}




            List<ElementData> allMatches = ListPool<ElementData>.Get();

            // 消除元素
            foreach (Vector2Int match in matches)
            {
                Debug.Log("消除：" + Data.boardData[match.x, match.y].ToString());
                if (BoardData[match.x, match.y].Type != ElementType.Fixed_Special) //如果不是不存在
                {
                    allMatches.Add(BoardData[match.x, match.y]);
                    BoardData[match.x, match.y].SetType(ElementType.Fixed_Special); // 临时标记为空
                }
            }

            Dispatcher.Dispatch(GameMsg.ClearElements, allMatches);
            ListPool<ElementData>.Release(allMatches);

            // 计算分数
            int matchCount = matches.Count;
            int scoreToAdd = CalculateScore(matchCount);
            AddScore(scoreToAdd);


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
            int oldSocre = Data.currentScore;
            Data.currentScore += score;
            Debug.Log($"当前分数: {Data.currentScore}");

            Core.Dispatch(GameMsg.ScoreUpdated, oldSocre, Data.currentScore);
            // 检查是否达到目标
            if (Data.currentScore >= Data.targetScore)
            {
                Dispatcher.Dispatch(GameMsg.GameWin);
            }
        }

        #endregion

        #region 道具系统





        /*

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
            board[x, y] = ElementType.Fixed_Special;

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
                    board[i, y] = ElementType.Fixed_Special;
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
                    board[x, j] = ElementType.Fixed_Special;
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
                        board[i, j] = ElementType.Fixed_Special;
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
                        board[i, j] = ElementType.Fixed_Special;
                    }
                }
            }
        }

        */
        #endregion




    }
}
