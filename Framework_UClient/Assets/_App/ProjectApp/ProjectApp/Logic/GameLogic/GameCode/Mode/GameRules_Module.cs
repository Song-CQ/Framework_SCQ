using FutureCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class GameRules_Module : IGameModule
    {
        private EliminateGameCore core;
        private EliminateGameData data;
        private Vector2Int SelectedElement
        {
            get => data.selectedElement;
            set => data.selectedElement = value;
        }
        private ElementData[,] BoardData => data.boardData;

        private Vector2Int boardSize;
        private bool[,] _visited;

        public Dispatcher<uint> Dispatcher => core.Dispatcher;
        public GameRules_Module() { }
        public void FillCore(EliminateGameCore _core)
        {
            core = _core;
            data = core.Data;

            AddListener();
        }

        public void InitializeBoard(int w, int h)
        {
            boardSize = new Vector2Int(w, h);
        }



        public void AddListener()
        {
            Dispatcher.AddListener(GameMsg.ClickElement, OnElementClicked);

        }

        public void RemoveListener()
        {
            Dispatcher.RemoveListener(GameMsg.ClickElement, OnElementClicked);
        }

        /// <summary>
        /// 点击元素
        /// </summary>
        /// <param name="o"></param>
        void OnElementClicked(object o)
        {
            ElementItem element = (ElementItem)o;
            int x = element.Data.X;
            int y = element.Data.Y;

            if (SelectedElement.x < 0 || SelectedElement.y < 0)
            {
                // 第一次点击，选中元素
                SelectElement(x, y);
            }
            else
            {
                // 第二次点击，判断是否相邻
                if (IsAdjacent(SelectedElement.x, SelectedElement.y, x, y))
                {
                    // 交换元素
                    SwapElements(SelectedElement.x, SelectedElement.y, x, y);

                    // 检查匹配
                    List<Vector2Int> matches = FutureCore.ListPool<Vector2Int>.Get();
                    matches = CheckMatchesAfterSwap(SelectedElement.x, SelectedElement.y, x, y, ref matches);

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
                        SwapElements(SelectedElement.x, SelectedElement.y, x, y);
                    }
                    //使用完回收List
                    FutureCore.ListPool<Vector2Int>.Release(matches);

                }

                // 清除选中状态
                DeselectElement(SelectedElement.x, SelectedElement.y);

            }
        }


        /// <summary>
        /// 选中元素
        /// </summary>
        void SelectElement(int x, int y)
        {
            var elementData = BoardData[x, y];
            if ((int)elementData.Type >= 1000)
            {
                //不可点击
                return;
            }

            SelectedElement = new Vector2Int(x, y);
            Dispatcher.Dispatch(GameMsg.SelectElement, elementData);

        }

        private void DeselectElement(int x, int y)
        {
            var elementData = BoardData[x, y];
            SelectedElement = new Vector2Int(-1, -1);
            Dispatcher.Dispatch(GameMsg.DeselectElement, elementData);
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
            ElementData tempData1 = BoardData[x1, y1];
            ElementData tempData2 = BoardData[x2, y2];

            BoardData[x1, y1] = tempData2;
            BoardData[x2, y2] = tempData1;

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
            FindMatchesAt(x1, y1, visited,ref matches);
            FindMatchesAt(x2, y2, visited,ref matches);

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

            foreach (var item in tempMatches)
            {
                if(!visited[item.x,item.y])
                {
                    //未加入过
                    matches.Add(item);
                    visited[item.x,item.y] = true;
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
        /// 查找所有匹配（使用Span的高性能版本）
        /// </summary>
        public List<Vector2Int> FindAllMatches()
        {
            var allMatches = new List<Vector2Int>();

            // 使用bool数组记录已访问位置
            bool[,] visited = new bool[boardSize.x, boardSize.y];

            for (int y = 0; y < boardSize.y; y++)
            {
                for (int x = 0; x < boardSize.x; x++)
                {
                    if (!visited[x, y] &&
                        BoardData[x, y].Type != ElementType.None &&
                        BoardData[x, y].Type != ElementType.Special)
                    {
                        // 查找这个位置的匹配组
                        var matches = FindMatchGroup(x, y, visited);
                        if (matches.Count >= 3)
                        {
                            allMatches.AddRange(matches);
                        }
                    }
                }
            }

            return allMatches;
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

            ElementType type = BoardData[x, y].Type;
            if (type == ElementType.Special || type == ElementType.None)
                return false;

            // 使用栈分配数组
            Span<Vector2Int> tempMatches = stackalloc Vector2Int[boardSize.x];
            int matchCount = 0;

            // 起始位置
            tempMatches[matchCount++] = new Vector2Int(x, y);

            // 向左检查
            for (int i = x - 1; i >= 0; i--)
            {

                if (BoardData[i, y].Type == type)
                    tempMatches[matchCount++] = new Vector2Int(i, y);
                else
                    break;
            }

            // 向右检查
            for (int i = x + 1; i < boardSize.x; i++)
            {
                if (BoardData[i, y].Type == type)
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

            ElementType type = BoardData[x, y].Type;
            if (type == ElementType.Special || type == ElementType.None)
                return false;

            // 使用栈分配数组
            Span<Vector2Int> tempMatches = stackalloc Vector2Int[boardSize.y];
            int matchCount = 0;

            // 起始位置
            tempMatches[matchCount++] = new Vector2Int(x, y);

            // 向下检查
            for (int j = y - 1; j >= 0; j--)
            {
                if (BoardData[x, j].Type == type)
                    tempMatches[matchCount++] = new Vector2Int(x, j);
                else
                    break;
            }

            // 向上检查
            for (int j = y + 1; j < boardSize.y; j++)
            {
                if (BoardData[x, j].Type == type)
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
            List<ElementData> souList = ListPool<ElementData>.Get();
            List<ElementData> tarList = ListPool<ElementData>.Get();
            List<ElementData> creadList = ListPool<ElementData>.Get();


            for (int x = 0; x < boardSize.x; x++)
            {
                int creadCont = 0;
                for (int y = 0; y < boardSize.y; y++)
                {
                    if (BoardData[x, y].Type == ElementType.Special) // 空位标记
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
                                continue;
                            }
                            //可下落
                            if (ElementTypeTool.CheckType_FillEmpty(type))
                            {
                                sour = BoardData[x, temp_y];
                                //下落了 将自身设置为空的
                                BoardData[x, temp_y].SetType(ElementType.Special);
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
                            ElementType newType = core.GetRandomElementType();
                            //因为 是新创建的 所以可能 在高于棋盘的位置  
                            sour = new ElementData(x, temp_y + creadCont, newType);
                            creadCont++;
                            creadList.Add(sour);

                        }

                        souList.Add(sour);
                        tarList.Add(tar);

                        //设置当前的类型
                        BoardData[x, y].SetType(sour.Type);

                    }
                }
            }

            if (creadList.Count > 0)
            {
                //创建元素
                Dispatcher.Dispatch(GameMsg.GenerateElements, creadList);
            }

            if (souList.Count > 0 && tarList.Count > 0)
            {
                // 下落元素
                core.Dispatch(GameMsg.ElementsFall, souList, tarList);
            }


            // 检查新的匹配
            CheckAllMatches();
        }



        /// <summary>
        /// 检查所有匹配
        /// </summary>
        void CheckAllMatches()
        {
            // 使用对象池获取列表，避免GC分配
            var allMatches = ListPool<Vector2Int>.Get();
            
            var visited = GetVisited();
            // 优化2：一次性收集所有匹配位置
            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    // 跳过已检查位置和空位
                    if(ElementTypeTool.CheckType_CanMatches(BoardData[x,y].Type) || visited[x,y])

                    FindMatchesAt(x, y, visited,ref allMatches);
                }
            }

            if (allMatches.Count > 0)
            {
                //消除
                ProcessMatches(allMatches);
                //补位
                FillEmptySpaces();
            }


            ListPool<Vector2Int>.Release(allMatches);

        }

        private bool[,] GetVisited()
        {
            if(_visited ==null)
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




            // 消除元素
            foreach (Vector2Int match in matches)
            {
                if (BoardData[match.x, match.y].Type != ElementType.None) //如果不是不存在
                {
                    BoardData[match.x, match.y].SetType(ElementType.Special); // 临时标记为空
                }
            }

            Dispatcher.Dispatch(GameMsg.ClearElements, matches);


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
            int oldSocre = data.currentScore;
            data.currentScore += score;
            Debug.Log($"当前分数: {data.currentScore}");

            core.Dispatch(GameMsg.ScoreUpdated, oldSocre, data.currentScore);
            // 检查是否达到目标
            if (data.currentScore >= data.targetScore)
            {
                Dispatcher.Dispatch(GameMsg.GameWin);
            }
        }

        public void Disposed()
        {


        }


    }
}
