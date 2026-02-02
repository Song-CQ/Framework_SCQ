using Codice.CM.Common;
using FutureCore;
using ILRuntime.CLR.TypeSystem;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
            Dispatcher.AddPriorityListener(GameMsg.Player_ClickElement, OnClickElement_test);
            Dispatcher.AddPriorityListener(GameMsg.Player_SwipeElement, OnPlayer_SwipeElement);
            Dispatcher.AddPriorityListener(GameMsg.Player_SwipeElementToElement, OnPlayer_SwipeElementToElement);

        }



        public void RemoveListener()
        {

            Dispatcher.RemovePriorityListener(GameMsg.Player_ClickElement, OnClickElement_test);
            Dispatcher.RemovePriorityListener(GameMsg.Player_SwipeElement, OnPlayer_SwipeElement);
            Dispatcher.RemovePriorityListener(GameMsg.Player_SwipeElementToElement, OnPlayer_SwipeElementToElement);
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
            ElementData data = (ElementData)(o);
            int x = data.X;
            int y = data.Y;

            //获取当前棋盘上对应位置的元素 不能信任传过来的数据
            data = Data.boardData[x, y];

            if (ElementTypeTool.CheckType_IsProp(data.Type))
            {
                //触发道具
                Player_ActivateProp(data);
                return;
            }



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
        void OnPlayer_SwipeElementToElement(object obj)
        {
            object[] datas = obj as object[];

            ElementData data1 = (ElementData)datas[0];
            ElementData data2 = (ElementData)datas[1];

            if (IsAdjacent(data1.X, data1.Y, data2.X, data2.Y)
                || Data.HasConnection(data1.X, data1.Y, data2.X, data2.Y))
            {

                if (ElementTypeTool.CheckType_IsProp(data1.Type)&&ElementTypeTool.CheckType_IsProp(data2.Type))
                {
                    Player_ActivateTwoProp(data1.X,data1.Y,data2.X,data2.Y);
                }
                else
                {

                    Player_SwapElement(data1.X, data1.Y, data2.X, data2.Y);
                }



            }


        }

        /// <summary>
        /// 在一个元素上 拖动 
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        void OnPlayer_SwipeElement(object obj)
        {
            object[] datas = obj as object[];

            ElementData data = (ElementData)datas[0];
            Vector2 dir = (Vector2)datas[1];

            if (data.Type == ElementType.Item_Change)
            {
                Player_ChangeElementType(data.X, data.Y, dir.x > 0);
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

            // 交换元素
            SwapElements(select_X, select_Y, x, y);

            // 检查匹配
            List<Vector2Int> matches = FutureCore.ListPool<Vector2Int>.Get();
            matches = CheckMatchesAfterSwap(select_X, select_Y, x, y, ref matches);

            if (matches.Count > 0)
            {
                //记录快照
                Data.TakeMemorySnapshotBoardData();

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

        public void Player_ChangeElementType(int x, int y, bool isRith)
        {
            ElementData data = BoardData[x, y];
            if (data.Type == ElementType.Item_Change)
            {
                if (isRith)
                {
                    int temp = data.data1;
                    data.data1 = data.data2;
                    data.data2 = data.data3;
                    data.data3 = temp;
                }
                else
                {
                    int temp = data.data1;
                    data.data1 = data.data3;
                    data.data3 = data.data2;
                    data.data2 = temp;
                }


                BoardData[x, y] = data;


                Core.Dispatch(GameMsg.ChangeElementType, data, isRith);


            }
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

            Debug.Log("要创建的元素数量" + creadList.Count);
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

            Debug.Log("要下落的元素数量" + souList.Count);
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
                    BoardData[match.x, match.y].SetSpecial(); // 标记为空
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


        private void Player_ActivateTwoProp(int form_x,int form_y, int to_x,int to_y)
        {
            ElementData formData = Data.boardData[form_x,form_y];
            ElementData toData = Data.boardData[to_x,to_y];

            if (!ElementTypeTool.CheckType_IsProp(formData.Type) || !ElementTypeTool.CheckType_IsProp(toData.Type)) return;
            
            List<Vector2Int> tempMatches = ListPool<Vector2Int>.Get();
            List<ElementData> currProps = ListPool<ElementData>.Get();
            
            //激活两个道具
            ActivatePropTwo(formData, toData, ref tempMatches, ref currProps);

            Core.Dispatch(GameMsg.ActivateTwoProp, formData, toData, tempMatches, currProps);
            if (tempMatches.Count > 0)
            {             
                ProcessMatches(tempMatches);
            }

            //道具触发的道具 激活
            ActivatePropList(currProps);

            ListPool<ElementData>.Release(currProps);

            //补位
            FillEmptySpaces();


        }


        /// <summary>
        /// 一次使用棋盘道具的操作
        /// </summary>
        /// <param name="data"></param>
        public void Player_ActivateProp(ElementData data)
        {
            if (!ElementTypeTool.CheckType_IsProp(data.Type)) return;

            Data.TakeMemorySnapshotBoardData();


            List<ElementData> currProps = ListPool<ElementData>.Get();
            currProps.Add(data);

            ActivatePropList(currProps);

            ListPool<ElementData>.Release(currProps);

            //补位
            FillEmptySpaces();

        }

        private void ActivatePropList(List<ElementData> currProps)
        {
            while (currProps.Count > 0)
            {
                List<Vector2Int> tempMatches = ListPool<Vector2Int>.Get();
                List<ElementData> tempProps = ListPool<ElementData>.Get();

                //用来判断当前是否在同一组
                uint index = GameTool.GetNextIndex();

                for (int i = 0; i < currProps.Count; i++)
                {
                    List<Vector2Int> matches = ListPool<Vector2Int>.Get();
                    List<ElementData> oneProp = ListPool<ElementData>.Get();

                    var propData = currProps[i];

                    Data.boardData[propData.X, propData.Y].SetSpecial();

                    ActivateProp(propData, ref matches, ref oneProp);
                    
                    Core.Dispatch(GameMsg.ActivateProp, index, propData, matches, oneProp);

                    tempMatches.AddRange(matches);
                    tempProps.AddRange(oneProp);
                    ListPool<Vector2Int>.Release(matches);
                    ListPool<ElementData>.Release(oneProp);
                }

                if (tempMatches.Count > 0)
                {
                    ProcessMatches(tempMatches);
                }

                //本次循环完成
                currProps.Clear();
                if (tempProps.Count > 0)
                {
                    //本次触发了道具 继续触发道具
                    currProps.AddRange(tempProps);
                }

                ListPool<Vector2Int>.Release(tempMatches);
                ListPool<ElementData>.Release(tempProps);

            }
        }
        private void ActivateProp(ElementData data, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
            ElementType type = data.Type;

            switch (type)
            {
                case ElementType.Prop_Horizontal:
                    ActivateProp_Horizontal(data.X, data.Y, ref matches, ref dataProp);
                    break;
                case ElementType.Prop_Vertical:

                    ActivateProp_Vertical(data.X, data.Y, ref matches, ref dataProp);
                    break;
                case ElementType.Prop_Bomb:
                    ActivateProp_Bomb(data.X, data.Y, 2, ref matches, ref dataProp);
                    break;
                case ElementType.Prop_Wild:
                    ActivateProp_Wild(data.X, data.Y, ElementType.Fixed_Special, ref matches, ref dataProp);
                    break;

                default:
                    Debug.Log(data.ToString() + "并不是道具");
                    break;
            }
        }

        private void ActivatePropTwo(ElementData formData, ElementData toData, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
          
            if (toData.Type == ElementType.Prop_Vertical || toData.Type == ElementType.Prop_Horizontal)
            {
                if ((toData.Type == ElementType.Prop_Vertical || toData.Type == ElementType.Prop_Horizontal) && formData.Type != toData.Type)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //横竖
                    Data.boardData[formData.X, formData.Y].SetSpecial();
                    Data.boardData[toData.X, toData.Y].SetSpecial();

                    ActivateProp_Horizontal(toData.X, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Vertical(toData.X, toData.Y, ref matches, ref dataProp);

                }

            }

            if (toData.Type == ElementType.Prop_Bomb || formData.Type == ElementType.Prop_Bomb)
            {
                if (toData.Type == ElementType.Prop_Bomb && formData.Type == ElementType.Prop_Bomb)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //双炸弹
                    Data.boardData[formData.X, formData.Y].SetSpecial();
                    Data.boardData[toData.X, toData.Y].SetSpecial();

                    ActivateProp_Bomb(toData.X, toData.Y, 3, ref matches, ref dataProp);
                }


                if (toData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Vertical)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //炸弹加竖
                    Data.boardData[formData.X, formData.Y].SetSpecial();
                    Data.boardData[toData.X, toData.Y].SetSpecial();
                    ActivateProp_Vertical(toData.X, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Vertical(toData.X + 1, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Vertical(toData.X - 1, toData.Y, ref matches, ref dataProp);

                }

                if (toData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Vertical)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //炸弹加横
                    Data.boardData[formData.X, formData.Y].SetSpecial();
                    Data.boardData[toData.X, toData.Y].SetSpecial();
                    ActivateProp_Horizontal(toData.X, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Horizontal(toData.X + 1, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Horizontal(toData.X - 1, toData.Y, ref matches, ref dataProp);

                }

            }

            if (toData.Type == ElementType.Prop_Wild || formData.Type == ElementType.Prop_Wild)
            {
                if (toData.Type == ElementType.Prop_Bomb || formData.Type == ElementType.Prop_Bomb)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //wild加炸弹
                    Data.boardData[formData.X, formData.Y].SetSpecial();
                    Data.boardData[toData.X, toData.Y].SetSpecial();
                    ActivateProp_WildAndProp(toData.X, toData.Y, ElementType.Prop_Bomb,3,ref matches, ref dataProp);
                }

                if (toData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Vertical)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //wild加Vertical
                    Data.boardData[formData.X, formData.Y].SetSpecial();
                    Data.boardData[toData.X, toData.Y].SetSpecial();
                    ActivateProp_WildAndProp(toData.X, toData.Y, ElementType.Prop_Vertical,3, ref matches, ref dataProp);
                }

                if (toData.Type == ElementType.Prop_Horizontal || formData.Type == ElementType.Prop_Horizontal)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //wild加Horizontal
                    Data.boardData[formData.X, formData.Y].SetSpecial();
                    Data.boardData[toData.X, toData.Y].SetSpecial();
                    ActivateProp_WildAndProp(toData.X, toData.Y, ElementType.Prop_Horizontal,3, ref matches, ref dataProp);
                }
            }

        }



        private void ActivateProp_Horizontal(int X, int Y, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
            if (!IsPositionValid(X, Y)) return;

            int L_Pot = X - 1;
            int R_Pot = X + 1;
            bool isL = true;

            for (int x = 0; x < Data.BoardWidth - 1; x++)
            {
                int potX = 0;
                if (isL)
                {
                    if (L_Pot >= 0)
                    {
                        potX = L_Pot;
                        L_Pot--;

                    }
                    else
                    {
                        potX = R_Pot;
                        R_Pot++;
                    }
                }
                else
                {
                    if (R_Pot < Data.BoardWidth)
                    {
                        potX = R_Pot;
                        R_Pot++;
                    }
                    else
                    {
                        potX = L_Pot;
                        L_Pot--;
                    }

                }
                var tempData = Data.boardData[potX, Y];
                if (ElementTypeTool.CheckType_CanMatches(tempData.Type))
                {
                    matches.Add(new Vector2Int(potX, Y));
                }
                else
                {
                    dataProp.Add(tempData);
                }

                isL = !isL;

            }


        }

        private void ActivateProp_Vertical(int X, int Y, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
            if (!IsPositionValid(X, Y)) return;


            int U_Pot = Y + 1;
            int D_Pot = Y - 1;
            bool isD = true;

            for (int x = 0; x < Data.BoardHeight - 1; x++)
            {
                int potY = 0;
                if (isD)
                {
                    if (D_Pot >= 0)
                    {
                        potY = D_Pot;
                        D_Pot--;

                    }
                    else
                    {
                        potY = U_Pot;
                        U_Pot++;
                    }
                }
                else
                {
                    if (U_Pot < Data.BoardHeight)
                    {
                        potY = U_Pot;
                        U_Pot++;
                    }
                    else
                    {
                        potY = D_Pot;
                        D_Pot--;
                    }

                }
                //Debug.Log(potY);
                var tempData = Data.boardData[X, potY];
                if (ElementTypeTool.CheckType_CanMatches(tempData.Type))
                {
                    matches.Add(new Vector2Int(X, potY));
                }
                else
                {
                    dataProp.Add(tempData);
                }

                isD = !isD;

            }

        }


        private void ActivateProp_Bomb(int X, int Y, int bombRadius, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
            if (!IsPositionValid(X, Y)) return;

            Vector2Int bombGridPosition = new Vector2Int(X, Y);

            int height = Data.BoardHeight;
            int width = Data.BoardWidth;

            // 检查以炸弹为中心的圆形区域
            for (int x = -bombRadius; x <= bombRadius; x++)
            {
                for (int y = -bombRadius; y <= bombRadius; y++)
                {
                    // 计算实际网格位置
                    int targetX = bombGridPosition.x + x;
                    int targetY = bombGridPosition.y + y;

                    // 检查是否在网格范围内
                    if (targetX >= 0 && targetX < width &&
                        targetY >= 0 && targetY < height)
                    {
                        // 使用圆形范围（距离平方判断）
                        int distanceSquared = x * x + y * y;
                        if (distanceSquared <= bombRadius * bombRadius)
                        {
                            if (targetX == bombGridPosition.X && targetX == bombGridPosition.Y) continue;

                            // 检查该位置是否有可消除的方块
                            var tempData = Data.boardData[targetX, targetY];

                            if (ElementTypeTool.CheckType_CanMatches(tempData.Type))
                            {
                                matches.Add(new Vector2Int(targetX, targetY));
                            }
                            else
                            {
                                dataProp.Add(tempData);
                            }
                        }
                    }
                }
            }

        }

        private void ActivateProp_Wild(int X, int Y, ElementType matches_type, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
            if (!IsPositionValid(X, Y)) return;

            if (matches_type == ElementType.Fixed_Special)
            {
                matches_type = GameTool.GetRandomBaseElementType();
            }


            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    ElementType type = ElementTypeTool.GetTypeToElementData(BoardData[x, y]);
                    if (matches_type == type)
                    {
                        matches.Add(new Vector2Int(x, y));
                    }

                }
            }

        }



        private void ActivateProp_WildAndProp(int X, int Y, ElementType elementType,int rananSum, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
            if (!IsPositionValid(X, Y)) return;

            List<ElementData> datas = ListPool<ElementData>.Get();
            foreach (var item in Data.boardData)
            {
                if (ElementTypeTool.CheckType_CanMatches(item.Type))
                {
                    datas.Add(item);
                }
            }

            for (int i = 0; i < rananSum; i++)
            {
                int index = GameTool.RandomToInt(0, datas.Count);
                ElementData data = datas[index];
                data.SetSpecial();
                data.Type = elementType;
                Data.boardData[data.X,data.Y] = data;
                
                datas.RemoveAt(index);
                dataProp.Add(data);
            }

            ListPool<ElementData>.Release(datas);
        }


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






        */
        #endregion




    }
}
