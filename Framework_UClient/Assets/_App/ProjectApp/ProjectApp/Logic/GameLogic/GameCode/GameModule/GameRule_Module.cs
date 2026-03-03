
using FutureCore;
using ProjectApp.Data;
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

        #region ��ʱ���� 
        private List<Vector2Int> temp_AllMatchesList = new List<Vector2Int>();
        #endregion

        #region ����
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
            //��������
            Dispatcher.AddPriorityListener(GameMsg.Player_ClickElement, OnPlayer_ClickElement_test);
            Dispatcher.AddPriorityListener(GameMsg.Player_SwipeElement, OnPlayer_SwipeElement);
            Dispatcher.AddPriorityListener(GameMsg.Player_SwipeElementToElement, OnPlayer_SwipeElementToElement);

            Dispatcher.AddPriorityListener(GameMsg.UseExternalProp, OnUseExternalProp);



        }



        public void RemoveListener()
        {

            Dispatcher.RemovePriorityListener(GameMsg.Player_ClickElement, OnPlayer_ClickElement_test);
            Dispatcher.RemovePriorityListener(GameMsg.Player_SwipeElement, OnPlayer_SwipeElement);
            Dispatcher.RemovePriorityListener(GameMsg.Player_SwipeElementToElement, OnPlayer_SwipeElementToElement);

            Dispatcher.RemovePriorityListener(GameMsg.UseExternalProp, OnUseExternalProp);


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


        private const string lockStr = "lock";
        void OnPlayer_ClickElement_test(object o)
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
                        Debug.Log("��ǰ�߳�" + currentThread.ManagedThreadId + ("  " + SelectedElement.x + "--" + SelectedElement.y));
                        OnClick_Element(o);
                    }
                     , cts.Token);
                    task.Wait(cts.Token);  // ͬ���ȴ������׳��쳣
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("OnElementClicked ִ�г�ʱ��������ѭ��");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    Debug.LogError($"OnElementClicked ִ�г���: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"OnElementClicked ִ�г���: {ex.Message}");
            }

        }


        #region  ����


        /// <summary>
        /// ���Ԫ��
        /// </summary>
        /// <param name="o"></param>
        void OnClick_Element(object o)
        {
            ElementData data = (ElementData)(o);
            int x = data.X;
            int y = data.Y;

            //��ȡ��ǰ�����϶�Ӧλ�õ�Ԫ�� �������δ�����������
            data = Data.boardData[x, y];


            if (Core.SelectExternalProp != ExternalProp.None)
            {
                //��ǰ�����ڼ���ĵ���

                return;
            }

            if (ElementTool.CheckType_IsProp(data.Type) && !Core.IsClickProp)
            {
                //��������
                Player_ActivateProp(data);
                return;
            }



            if (SelectedElement.x < 0 || SelectedElement.y < 0)
            {
                // ��һ�ε����ѡ��Ԫ��
                SelectElement(x, y);
            }
            else
            {
                int select_X = SelectedElement.x;
                int select_Y = SelectedElement.y;
                // ���ѡ��״̬
                DeselectElement(SelectedElement.x, SelectedElement.y);

                // �ڶ��ε�����ж��Ƿ�����
                if (IsAdjacent(select_X, select_Y, x, y) || Data.HasConnection(select_X, select_Y, x, y))
                {
                    if (Core.IsClickProp)
                    {
                        Player_ActivateTwoProp(select_X, select_Y, x, y);
                        return;
                    }

                    Player_SwapElement(select_X, select_Y, x, y);
                }


            }

        }

        /// <summary>
        /// �϶�Ԫ�� ����һ��Ԫ����
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



                if (ElementTool.CheckType_IsProp(data1.Type) && ElementTool.CheckType_IsProp(data2.Type))
                {
                    Player_ActivateTwoProp(data1.X, data1.Y, data2.X, data2.Y);
                }
                else
                {

                    Player_SwapElement(data1.X, data1.Y, data2.X, data2.Y);
                }




            }


        }

        /// <summary>
        /// ��һ��Ԫ���� �϶� 
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        void OnPlayer_SwipeElement(object obj)
        {
            object[] datas = obj as object[];

            ElementData data = (ElementData)datas[0];
            Vector2 dir = (Vector2)datas[1];

            if (data.Type == ElementType.Item_Special)
            {
                Player_ChangeElementType(data.X, data.Y, dir.x > 0);
            }
        }



        #endregion


        #region  ���Ĵ���

        /// <summary>
        /// ѡ��Ԫ��
        /// </summary>
        void SelectElement(int x, int y)
        {
            var elementData = BoardData[x, y];
            if (!ElementTool.CheckType_ClickEvent(elementData.Type))
            {
                //���ɵ��
                return;
            }

            SelectedElement = new Vector2Int(x, y);
            Dispatcher.Dispatch(GameMsg.SelectElement, elementData);

        }
        /// <summary>
        /// ȡ��Ԫ��
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void DeselectElement(int x, int y)
        {
            var elementData = BoardData[x, y];
            SelectedElement = new Vector2Int(-1, -1);
            Dispatcher.Dispatch(GameMsg.DeselectElement, elementData);
        }

        #region һ�β���
        private void Player_SwapElement(int select_X, int select_Y, int x, int y)
        {
            //��¼����
            Data.TakeMemorySnapshotBoardData();

            // ����Ԫ��
            SwapElements(select_X, select_Y, x, y);

            // ���ƥ��
            List<Vector2Int> matches = FutureCore.ListPool<Vector2Int>.Get();
            matches = CheckMatchesAfterSwap(select_X, select_Y, x, y, ref matches);

            if (matches.Count > 0)
            {
                // ��ƥ�䣬��������
                ProcessMatches(matches);
                // ������Ԫ�� ����λ
                FillEmptySpaces();
            }
            else
            {
                if (Core.IsBackSwap)
                {

                    // ��ƥ�䣬��������
                    SwapElements(select_X, select_Y, x, y);
                    //���������� ɾ����һ�ؼ�¼�Ŀ���
                    Data.DelLastMemorySnapshotBoardData();

                }
            }
            //ʹ�������List
            FutureCore.ListPool<Vector2Int>.Release(matches);


        }

        public void Player_RananAllElement()
        {
            //��¼����
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

        public bool Player_UndoElement()
        {
            bool isCan = Data.CanUndo();

            if (!isCan) return false;

            Data.UndoStepBoardData();

            Dispatcher.Dispatch(GameMsg.RestAllElements);

            return true;
        }

        public void Player_ChangeElementType(int x, int y, bool isRith)
        {
            ElementData data = BoardData[x, y];
            if (data.Type == ElementType.Item_Special)
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



                List<Vector2Int> matches = FutureCore.ListPool<Vector2Int>.Get();
                var visited = GetVisited();
                FindMatchesAt(data.X, data.Y, visited, ref matches);
                if (matches.Count > 0)
                {
                    //��¼����
                    Data.TakeMemorySnapshotBoardData();

                    // ��ƥ�䣬��������
                    ProcessMatches(matches);
                    // ������Ԫ�� ����λ
                    FillEmptySpaces();
                }

            }
        }

        #endregion



        /// <summary>
        /// �ж�����Ԫ���Ƿ�����
        /// </summary>
        bool IsAdjacent(int x1, int y1, int x2, int y2)
        {
            int dx = Mathf.Abs(x1 - x2);
            int dy = Mathf.Abs(y1 - y2);
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        /// <summary>
        /// ��������Ԫ��
        /// </summary>
        void SwapElements(int x1, int y1, int x2, int y2)
        {

            ElementData tempData1 = BoardData[x1, y1];
            ElementData tempData2 = BoardData[x2, y2];
            Debug.Log(string.Format("����Ԫ��{0}  �� {1}", tempData1.ToString(), tempData2.ToString()));

            // ʵ�ʽ�����������
            // �����������λ�� ������
            SetBoardData(x1, y1, tempData2);
            SetBoardData(x2, y2, tempData1);

            // ֪ͨ����ģ�� ��������
            List<ElementData> elementDatas = FutureCore.ListPool<ElementData>.Get();
            elementDatas.Add(tempData1);
            elementDatas.Add(tempData2);
            Dispatcher.Dispatch(GameMsg.SwapElements, elementDatas);
            FutureCore.ListPool<ElementData>.Release(elementDatas);

        }

        /// <summary>
        /// ��齻�����ƥ��
        /// </summary>
        private List<Vector2Int> CheckMatchesAfterSwap(int x1, int y1, int x2, int y2, ref List<Vector2Int> matches)
        {
            if (matches == null) matches = new List<Vector2Int>();
            var visited = GetVisited();
            // ��齻��������λ�ü����������
            FindMatchesAt(x1, y1, visited, ref matches);
            FindMatchesAt(x2, y2, visited, ref matches);

            Debug.Log(string.Format("����Ԫ�غ�������Ԫ������{0}", matches.Count));
            foreach (var item in matches)
            {
                Debug.Log(string.Format("�ֱ���{0}", item.ToString()));
            }


            return matches;
        }


        /// <summary>
        /// ����ָ��λ�õ�ƥ��
        /// </summary>
        private List<Vector2Int> FindMatchesAt(int x, int y, bool[,] visited, ref List<Vector2Int> matches)
        {
            // �߽���
            if (!IsPositionValid(x, y))
                return matches;

            ElementType type = BoardData[x, y].Type;
            if (!ElementTool.CheckType_CanMatches(type))
                return matches;

            // ʹ��ջ�ڴ����ѷ���
            Span<Vector2Int> tempMatches = stackalloc Vector2Int[boardSize.x * boardSize.y];
            int matchCount = 0;

            //����ָ��λ�õĴ�ֱƥ��
            FindHorizontalMatchesAt(x, y, 4, ref tempMatches, ref matchCount);
            //����ָ��λ�õĴ�ֱƥ��
            FindVerticalMatchesAt(x, y, 4, ref tempMatches, ref matchCount);

            Span<Vector2Int> validMatches = tempMatches.Slice(0, matchCount);
            foreach (var item in validMatches)
            {
                if (!visited[item.x, item.y])
                {
                    //δ�����
                    matches.Add(item);
                    visited[item.x, item.y] = true;
                }

            }
            return matches;
        }


        #region �д����ƥ�䷽��
        /// <summary>
        /// �ݹ��ռ��������ڵ�ƥ�䣨֧��T�Ρ�L�εȸ���ƥ�䣩
        /// </summary>
        private void CollectMatchesRecursive(int x, int y, ElementType type, Span<Vector2Int> matches, ref int matchCount)
        {
            // ����Ƿ��ѷ��ʻ����Ͳ�ƥ��
            if (!IsPositionValid(x, y) ||
                BoardData[x, y].Type != type ||
                ContainsPosition(matches, matchCount, new Vector2Int(x, y)))
                return;

            // ���ӵ�ƥ���б�
            matches[matchCount++] = new Vector2Int(x, y);

            // ����ĸ�����
            CheckAndCollect(x + 1, y, type, matches, ref matchCount); // ��
            CheckAndCollect(x - 1, y, type, matches, ref matchCount); // ��
            CheckAndCollect(x, y + 1, type, matches, ref matchCount); // ��
            CheckAndCollect(x, y - 1, type, matches, ref matchCount); // ��
        }

        private void CheckAndCollect(int x, int y, ElementType type, Span<Vector2Int> matches, ref int matchCount)
        {
            if (IsPositionValid(x, y) && BoardData[x, y].Type == type)
            {
                CollectMatchesRecursive(x, y, type, matches, ref matchCount);
            }
        }

        /// <summary>
        /// ���λ���Ƿ�����ƥ���б���
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
        /// ����ƥ���飨ʹ��BFS��
        /// </summary>
        private List<Vector2Int> FindMatchGroup(int startX, int startY, bool[,] visited)
        {
            var matches = new List<Vector2Int>();
            var type = BoardData[startX, startY].Type;

            // ʹ�ö��н���BFS
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(new Vector2Int(startX, startY));

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();

                if (visited[pos.x, pos.y]) continue;
                if (BoardData[pos.x, pos.y].Type != type) continue;

                visited[pos.x, pos.y] = true;
                matches.Add(pos);

                // ����ĸ�����
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
        /// ����ָ��λ�õ�ˮƽƥ��
        /// </summary>
        private bool FindHorizontalMatchesAt(int x, int y, int minMatchCount, ref Span<Vector2Int> finalMatches, ref int finalMatchesCont)
        {
            if (!IsPositionValid(x, y))
                return false;

            ElementType type = ElementTool.GetTypeToElementData(BoardData[x, y]);
            if (type == ElementType.Fixed_Empty || type == ElementType.Fixed_None)
                return false;

            // ʹ��ջ��������
            Span<Vector2Int> tempMatches = stackalloc Vector2Int[boardSize.x];
            int matchCount = 0;

            // ��ʼλ��
            tempMatches[matchCount++] = new Vector2Int(x, y);

            // ������
            for (int i = x - 1; i >= 0; i--)
            {
                ElementType tarType = ElementTool.GetTypeToElementData(BoardData[i, y]);

                if (tarType == type)
                    tempMatches[matchCount++] = new Vector2Int(i, y);
                else
                    break;
            }

            // ���Ҽ��
            for (int i = x + 1; i < boardSize.x; i++)
            {
                ElementType tarType = ElementTool.GetTypeToElementData(BoardData[i, y]);
                if (tarType == type)
                    tempMatches[matchCount++] = new Vector2Int(i, y);
                else
                    break;
            }

            // ���������3��ƥ�䣬���������б�
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
        /// ����ָ��λ�õĴ�ֱƥ�䣨��������������������ʹ�ã�
        /// </summary>
        private bool FindVerticalMatchesAt(int x, int y, int minMatchCount, ref Span<Vector2Int> finalMatches, ref int finalMatchesCont)
        {
            if (!IsPositionValid(x, y))
                return false;

            ElementType type = ElementTool.GetTypeToElementData(BoardData[x, y]);
            if (type == ElementType.Fixed_Empty || type == ElementType.Fixed_None)
                return false;

            // ʹ��ջ��������
            Span<Vector2Int> tempMatches = stackalloc Vector2Int[boardSize.y];
            int matchCount = 0;

            // ��ʼλ��
            tempMatches[matchCount++] = new Vector2Int(x, y);

            // ���¼��
            for (int j = y - 1; j >= 0; j--)
            {
                ElementType tarType = ElementTool.GetTypeToElementData(BoardData[x, j]);
                if (tarType == type)
                    tempMatches[matchCount++] = new Vector2Int(x, j);
                else
                    break;
            }

            // ���ϼ��
            for (int j = y + 1; j < boardSize.y; j++)
            {
                ElementType tarType = ElementTool.GetTypeToElementData(BoardData[x, j]);
                if (tarType == type)
                    tempMatches[matchCount++] = new Vector2Int(x, j);
                else
                    break;
            }

            // ���������3��ƥ�䣬���������б�
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
        /// λ����Ч�Լ��
        /// </summary>
        private bool IsPositionValid(int x, int y)
        {
            return x >= 0 && x < boardSize.x &&
                   y >= 0 && y < boardSize.y;
        }

        /// <summary>
        /// λ����Ч�Լ�飨Vector2Int�汾��
        /// </summary>
        private bool IsPositionValid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < boardSize.x &&
                   pos.y >= 0 && pos.y < boardSize.y;
        }


        /// <summary>
        /// ����λ
        /// </summary>
        void FillEmptySpaces()
        {
            if (!Core.IsFill) return;

            //Ҫ�����Ԫ��
            List<ElementData> souList = ListPool<ElementData>.Get();
            //����Ԫ�ص�Ŀ��
            List<ElementData> tarList = ListPool<ElementData>.Get();
            //�´�����Ԫ��λ��
            List<ElementData> creadList = ListPool<ElementData>.Get();


            for (int x = 0; x < boardSize.x; x++)
            {
                int creadCont = 0;
                for (int y = 0; y < boardSize.y; y++)
                {
                    if (BoardData[x, y].Type == ElementType.Fixed_Empty) // ��λ���
                    {
                        //Ҫ����λ��
                        ElementData tar = BoardData[x, y];
                        //Ҫ���� Դλ��
                        ElementData sour = default;


                        //����Ѱ�ҵ�����Ĳ��ǿյ�����
                        int temp_y = y + 1;
                        bool isCreate = false;
                        while (temp_y < boardSize.y)
                        {
                            ElementType type = BoardData[x, temp_y].Type;
                            //�ǿյ� 
                            if (ElementTool.CheckType_UpEmpty(BoardData[x, temp_y].Type))
                            {
                                temp_y++;
                                continue;
                            }
                            //������
                            if (ElementTool.CheckType_FillEmpty(type))
                            {
                                sour = BoardData[x, temp_y];
                                //������ ����������Ϊ�յ�
                                BoardData[x, temp_y].SetType(ElementType.Fixed_Empty);
                            }
                            else
                            {
                                //�������������Ԫ�� Ҫ������Ԫ��
                                isCreate = true;
                            }
                            break;
                        }


                        //����Ŀ�
                        if (temp_y >= boardSize.y || isCreate)
                        {
                            // ������Ԫ��
                            sour = Core.GetRandomElementData();
                            //��Ϊ ���´����� ���Կ��� �ڸ������̵�λ��  
                            sour.SetPot(x, temp_y + creadCont);

                            creadCont++;
                            creadList.Add(sour);

                        }

                        souList.Add(sour);
                        tarList.Add(tar);

                        //���õ�ǰ������
                        SetBoardData(x, y, sour);
                    }
                }
            }

            Debug.Log("Ҫ������Ԫ������" + creadList.Count);
            //foreach (var item in creadList)
            //{
            //    Debug.Log(item.ToString());
            //}

            if (creadList.Count > 0)
            {
                //����Ԫ��
                Dispatcher.Dispatch(GameMsg.GenerateElements, creadList);
            }
            ListPool<ElementData>.Release(creadList);

            Debug.Log("Ҫ�����Ԫ������" + souList.Count);
            for (int i = 0; i < souList.Count; i++)
            {
                ElementData item = souList[i];
                //Debug.Log("����Ԫ��" + item.ToString() + "Ŀ��λ��" + tarList[i].ToString());
            }

            if (souList.Count > 0 && tarList.Count > 0)
            {
                // ����Ԫ��
                Core.Dispatch(GameMsg.ElementsFall, souList, tarList);
            }
            ListPool<ElementData>.Release(souList);
            ListPool<ElementData>.Release(tarList);


            // ����µ�ƥ��

            CheckAllMatches();
        }

        private void SetBoardData(int x, int y, ElementData sour)
        {
            sour.SetPot(x, y);
            BoardData[x, y] = sour;
        }


        /// <summary>
        /// �������ƥ��
        /// </summary>
        void CheckAllMatches()
        {
            if (!Core.isCheckAllMatches) return;

            // ʹ�ö���ػ�ȡ�б�������GC����
            var allMatches = FindAllMatches();

            if (allMatches.Count > 0)
            {
                //����
                ProcessMatches(allMatches);
                //��λ
                FillEmptySpaces();
            }

            allMatches.Clear();

        }

        /// <summary>
        /// ��������ƥ��
        /// </summary>
        public List<Vector2Int> FindAllMatches(List<Vector2Int> allMatches = null)
        {
            if (allMatches == null) allMatches = temp_AllMatchesList;

            allMatches.Clear();

            var visited = GetVisited();
            // �Ż�2��һ�����ռ�����ƥ��λ��
            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    // �����Ѽ��λ�úͿ�λ
                    if (!ElementTool.CheckType_CanMatches(BoardData[x, y].Type) || visited[x, y])
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
        /// ����ƥ������
        /// </summary>
        void ProcessMatches(List<Vector2Int> matches)
        {

            // ����������Ҫ4���������ͬԪ��
            //if (horizontalMatches.Count >= 4)
            //{
            //    matches.AddRange(horizontalMatches);

            //    // ���ɵ��ߣ���������������
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

            //    // ���ɵ���
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

            // ����Ԫ��
            foreach (Vector2Int match in matches)
            {
                //Debug.Log("������" + Data.boardData[match.x, match.y].ToString());
                if (BoardData[match.x, match.y].Type != ElementType.Fixed_Empty) //������ǲ�����
                {
                    allMatches.Add(BoardData[match.x, match.y]);
                    BoardData[match.x, match.y].SetEmpty(); // ���Ϊ��
                }
            }

            Dispatcher.Dispatch(GameMsg.ClearElements, allMatches);
            ListPool<ElementData>.Release(allMatches);

            // �������
            int matchCount = matches.Count;
            int scoreToAdd = CalculateScore(matchCount);
            AddScore(scoreToAdd);


        }

        /// <summary>
        /// �������
        /// </summary>
        int CalculateScore(int matchCount)
        {
            switch (matchCount)
            {
                case 4: return GeneralStaticVO.Instance.MatchScore_4;  // ����������
                case 5: return GeneralStaticVO.Instance.MatchScore_5;  // �����÷�
                case 6: return GeneralStaticVO.Instance.MatchScore_6;  // �����÷�
                case 7: return GeneralStaticVO.Instance.MatchScore_7;  // �����÷�
                default:  // ��������
                    {
                        if(matchCount< 7)
                        {
                            return GeneralStaticVO.Instance.MatchScore_1 * matchCount;
                        }else
                        {
                            return GeneralStaticVO.Instance.MatchScore_7 + GeneralStaticVO.Instance.MatchScore_1 * (matchCount-7);
                        }
                    }
            }
        }

        /// <summary>
        /// ���ӷ���
        /// </summary>
        void AddScore(int score)
        {
            int oldSocre = Data.currentScore;
            Data.currentScore += score;
            Debug.Log($"��ǰ����: {Data.currentScore}");

            Core.Dispatch(GameMsg.ScoreUpdated, oldSocre, Data.currentScore);
            // ����Ƿ�ﵽĿ��
            if (Data.currentScore >= Data.targetScore)
            {
                Core.GameWin();
            }
        }

        #endregion

        #region ����ϵͳ


        private void Player_ActivateTwoProp(int form_x, int form_y, int to_x, int to_y)
        {
            ElementData formData = Data.boardData[form_x, form_y];
            ElementData toData = Data.boardData[to_x, to_y];

            if (!ElementTool.CheckType_IsProp(formData.Type) || !ElementTool.CheckType_IsProp(toData.Type)) return;

            List<Vector2Int> tempMatches = ListPool<Vector2Int>.Get();
            List<ElementData> currProps = ListPool<ElementData>.Get();

            //������������
            ActivatePropTwo(formData, toData, ref tempMatches, ref currProps);

            Core.Dispatch(GameMsg.ActivateTwoProp, formData, toData, tempMatches, currProps);
            if (tempMatches.Count > 0)
            {
                ProcessMatches(tempMatches);
            }

            //���ߴ����ĵ��� ����
            ActivatePropList(currProps);

            ListPool<ElementData>.Release(currProps);

            //��λ
            FillEmptySpaces();


        }


        /// <summary>
        /// һ��ʹ�����̵��ߵĲ���
        /// </summary>
        /// <param name="data"></param>
        public void Player_ActivateProp(ElementData data)
        {
            if (!ElementTool.CheckType_IsProp(data.Type)) return;

            Data.TakeMemorySnapshotBoardData();


            List<ElementData> currProps = ListPool<ElementData>.Get();
            currProps.Add(data);

            ActivatePropList(currProps);

            ListPool<ElementData>.Release(currProps);

            //��λ
            FillEmptySpaces();

        }

        private void ActivatePropList(List<ElementData> currProps)
        {
            while (currProps.Count > 0)
            {
                List<Vector2Int> tempMatches = ListPool<Vector2Int>.Get();
                List<ElementData> tempProps = ListPool<ElementData>.Get();

                //�����жϵ�ǰ�Ƿ���ͬһ��
                uint index = GameTool.GetNextIndex();

                for (int i = 0; i < currProps.Count; i++)
                {
                    List<Vector2Int> matches = ListPool<Vector2Int>.Get();
                    List<ElementData> oneProp = ListPool<ElementData>.Get();

                    var propData = currProps[i];

                    Data.boardData[propData.X, propData.Y].SetEmpty();

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

                //����ѭ�����
                currProps.Clear();
                if (tempProps.Count > 0)
                {
                    //���δ����˵��� ������������
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
                    ActivateProp_Wild(data.X, data.Y, ElementType.Fixed_Empty, ref matches, ref dataProp);
                    break;

                default:
                    Debug.Log(data.ToString() + "�����ǵ���");
                    break;
            }
        }

        private void ActivatePropTwo(ElementData formData, ElementData toData, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {

            if (toData.Type == ElementType.Prop_Vertical || toData.Type == ElementType.Prop_Horizontal)
            {
                if (formData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Horizontal)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //����
                    Data.boardData[formData.X, formData.Y].SetEmpty();
                    Data.boardData[toData.X, toData.Y].SetEmpty();

                    ActivateProp_Horizontal(toData.X, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Vertical(toData.X, toData.Y, ref matches, ref dataProp);
                    Debug.LogWarning("����");

                }

            }

            if (toData.Type == ElementType.Prop_Bomb || formData.Type == ElementType.Prop_Bomb)
            {
                if (toData.Type == ElementType.Prop_Bomb && formData.Type == ElementType.Prop_Bomb)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //˫ը��
                    Data.boardData[formData.X, formData.Y].SetEmpty();
                    Data.boardData[toData.X, toData.Y].SetEmpty();

                    ActivateProp_Bomb(toData.X, toData.Y, 3, ref matches, ref dataProp);
                    Debug.LogWarning("˫ը��");
                }


                if (toData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Vertical)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //ը������
                    Data.boardData[formData.X, formData.Y].SetEmpty();
                    Data.boardData[toData.X, toData.Y].SetEmpty();
                    ActivateProp_Vertical(toData.X, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Vertical(toData.X + 1, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Vertical(toData.X - 1, toData.Y, ref matches, ref dataProp);
                    Debug.LogWarning("ը������");
                }

                if (toData.Type == ElementType.Prop_Horizontal || formData.Type == ElementType.Prop_Horizontal)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //ը���Ӻ�
                    Data.boardData[formData.X, formData.Y].SetEmpty();
                    Data.boardData[toData.X, toData.Y].SetEmpty();
                    ActivateProp_Horizontal(toData.X, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Horizontal(toData.X + 1, toData.Y, ref matches, ref dataProp);
                    ActivateProp_Horizontal(toData.X - 1, toData.Y, ref matches, ref dataProp);
                    Debug.LogWarning("ը���Ӻ�");
                }

            }

            if (toData.Type == ElementType.Prop_Wild || formData.Type == ElementType.Prop_Wild)
            {
                if (toData.Type == ElementType.Prop_Bomb || formData.Type == ElementType.Prop_Bomb)
                {
                    Data.TakeMemorySnapshotBoardData();
                    Debug.LogWarning("wild��ը��");
                    Data.boardData[formData.X, formData.Y].SetEmpty();
                    Data.boardData[toData.X, toData.Y].SetEmpty();
                    ActivateProp_WildAndProp(toData.X, toData.Y, ElementType.Prop_Bomb, 3, ref matches, ref dataProp);
                }

                if (toData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Vertical)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //wild��Vertical
                    Debug.LogWarning("wild��Vertical");
                    Data.boardData[formData.X, formData.Y].SetEmpty();
                    Data.boardData[toData.X, toData.Y].SetEmpty();
                    ActivateProp_WildAndProp(toData.X, toData.Y, ElementType.Prop_Vertical, 3, ref matches, ref dataProp);
                }

                if (toData.Type == ElementType.Prop_Horizontal || formData.Type == ElementType.Prop_Horizontal)
                {
                    Data.TakeMemorySnapshotBoardData();
                    //wild��Horizontal
                    Debug.LogWarning("wild��Horizontal");
                    Data.boardData[formData.X, formData.Y].SetEmpty();
                    Data.boardData[toData.X, toData.Y].SetEmpty();
                    ActivateProp_WildAndProp(toData.X, toData.Y, ElementType.Prop_Horizontal, 3, ref matches, ref dataProp);
                }
            }

        }



        private void ActivateProp_Horizontal(int X, int Y, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
            if (!IsPositionValid(X, Y)) return;

            int L_Pot = X;
            int R_Pot = X + 1;
            bool isL = true;

            for (int x = 0; x < Data.BoardWidth; x++)
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
                if (ElementTool.CheckType_CanMatches(tempData.Type))
                {
                    matches.Add(new Vector2Int(potX, Y));
                }
                else if (ElementTool.CheckType_IsProp(tempData.Type))
                {
                    dataProp.Add(tempData);
                }

                isL = !isL;

            }


        }

        private void ActivateProp_Vertical(int X, int Y, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
            if (!IsPositionValid(X, Y)) return;


            int U_Pot = Y;
            int D_Pot = Y - 1;
            bool isD = true;

            for (int x = 0; x < Data.BoardHeight; x++)
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
                if (ElementTool.CheckType_CanMatches(tempData.Type))
                {
                    matches.Add(new Vector2Int(X, potY));
                }
                else if (ElementTool.CheckType_IsProp(tempData.Type))
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

            // �����ը��Ϊ���ĵ�Բ������
            for (int x = -bombRadius; x <= bombRadius; x++)
            {
                for (int y = -bombRadius; y <= bombRadius; y++)
                {
                    // ����ʵ������λ��
                    int targetX = bombGridPosition.x + x;
                    int targetY = bombGridPosition.y + y;

                    // ����Ƿ�������Χ��
                    if (targetX >= 0 && targetX < width &&
                        targetY >= 0 && targetY < height)
                    {
                        // ʹ��Բ�η�Χ������ƽ���жϣ�
                        int distanceSquared = x * x + y * y;
                        if (distanceSquared <= bombRadius * bombRadius)
                        {

                            // ����λ���Ƿ��п������ķ���
                            var tempData = Data.boardData[targetX, targetY];

                            if (ElementTool.CheckType_CanMatches(tempData.Type))
                            {
                                matches.Add(new Vector2Int(targetX, targetY));
                            }
                            else if (ElementTool.CheckType_IsProp(tempData.Type))
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

            if (matches_type == ElementType.Fixed_Empty)
            {
                matches_type = GameTool.GetRandomBaseElementType();
            }


            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    ElementType type = ElementTool.GetTypeToElementData(BoardData[x, y]);
                    if (matches_type == type)
                    {
                        matches.Add(new Vector2Int(x, y));
                    }

                }
            }

        }



        private void ActivateProp_WildAndProp(int X, int Y, ElementType elementType, int rananSum, ref List<Vector2Int> matches, ref List<ElementData> dataProp)
        {
            if (!IsPositionValid(X, Y)) return;

            List<ElementData> datas = ListPool<ElementData>.Get();
            foreach (var item in Data.boardData)
            {
                if (ElementTool.CheckType_CanMatches(item.Type))
                {
                    datas.Add(item);
                }
            }

            for (int i = 0; i < rananSum; i++)
            {
                int index = GameTool.RandomToInt(0, datas.Count);
                ElementData data = datas[index];
                data.SetEmpty();
                data.Type = elementType;
                Data.boardData[data.X, data.Y] = data;

                datas.RemoveAt(index);
                dataProp.Add(data);
            }

            ListPool<ElementData>.Release(datas);
        }


        private void OnUseExternalProp(object obj)
        {
            object[] objects = obj as object[];
            ExternalProp propType = (ExternalProp)objects[0];
            List<Vector2Int> list = objects[1] as List<Vector2Int>;


            UseExternalProp(propType, list);
        }

        private bool UseExternalProp(ExternalProp propType, List<Vector2Int> list)
        {

            bool isSu = false;

            switch (propType)
            {
                case ExternalProp.Hammer:
                    {
                        Vector2Int pot = list[0];
                        if (!IsPositionValid(pot)) return false;
                        Data.TakeMemorySnapshotBoardData();
                        // �ɹ�ʹ�õ��� ֪ͨ����
                        Core.Dispatch(GameMsg.CostExternalProp, propType, list);
                        // ����Ч��
                        ProcessMatches(list);

                        FillEmptySpaces();
                        isSu = true;
                    }
                    break;
                case ExternalProp.Swipe:
                    {
                        Vector2Int pot1 = list[0];
                        Vector2Int pot2 = list[1];

                        if (!IsPositionValid(pot1) && !IsPositionValid(pot2)) return false;

                        if (IsAdjacent(pot1.x, pot1.y, pot2.x, pot2.y) || Data.HasConnection(pot1.x, pot1.y, pot2.x, pot2.y))
                        {
                            Data.TakeMemorySnapshotBoardData();
                            // �ɹ�ʹ�õ��� ֪ͨ����
                            Core.Dispatch(GameMsg.CostExternalProp, propType, list);

                            // ����Ԫ��
                            SwapElements(pot1.x, pot1.y, pot2.x, pot2.y);

                            // ���ƥ��
                            List<Vector2Int> matches = FutureCore.ListPool<Vector2Int>.Get();
                            matches = CheckMatchesAfterSwap(pot1.x, pot1.y, pot2.x, pot2.y, ref matches);

                            if (matches.Count > 0)
                            {
                                // ��ƥ�䣬��������
                                ProcessMatches(matches);
                                // ������Ԫ�� ����λ
                                FillEmptySpaces();
                            }
                            //ʹ�������List
                            FutureCore.ListPool<Vector2Int>.Release(matches);

                            isSu = true;
                        }


                    }
                    break;
                case ExternalProp.Horizontal:
                    {
                        Vector2Int pot = list[0];
                        if (!IsPositionValid(pot)) return false;
                        Data.TakeMemorySnapshotBoardData();
                        // �ɹ�ʹ�õ��� ֪ͨ����
                        Core.Dispatch(GameMsg.CostExternalProp, propType, list);
                        // ����Ч��
                        List<ElementData> currProps = ListPool<ElementData>.Get();
                        currProps.Add(new ElementData(ElementType.Prop_Horizontal).SetPot(pot.x, pot.y));

                        ActivatePropList(currProps);

                        ListPool<ElementData>.Release(currProps);

                        //��λ
                        FillEmptySpaces();

                        isSu = true;

                    }
                    break;
                case ExternalProp.Vertical:
                    {
                        Vector2Int pot = list[0];
                        if (!IsPositionValid(pot)) return false;
                        Data.TakeMemorySnapshotBoardData();
                        // �ɹ�ʹ�õ��� ֪ͨ����
                        Core.Dispatch(GameMsg.CostExternalProp, propType, list);
                        // ����Ч��
                        List<ElementData> currProps = ListPool<ElementData>.Get();
                        currProps.Add(new ElementData(ElementType.Prop_Vertical).SetPot(pot.x, pot.y));

                        ActivatePropList(currProps);

                        ListPool<ElementData>.Release(currProps);

                        //��λ
                        FillEmptySpaces();

                        isSu = true;

                    }
                    break;
                case ExternalProp.AllRandom:
                    {
                        // �ɹ�ʹ�õ��� ֪ͨ����
                        Core.Dispatch(GameMsg.CostExternalProp, propType, list);
                        Player_RananAllElement();
                        isSu = true;

                    }
                    break;
                case ExternalProp.Undo:
                    {
                        // �ɹ�ʹ�õ��� ����
                        bool isCan = Data.CanUndo();
                        if (isCan)
                        {
                            Core.Dispatch(GameMsg.CostExternalProp, propType, list);
                            Player_UndoElement();
                            isCan = true;
                        }
                    }
                    break;
                case ExternalProp.Wild:
                    {
                        Vector2Int pot = list[0];
                        if (!IsPositionValid(pot)) return false;

                        ElementType type = BoardData[pot.x, pot.y].Type;


                        // �ɹ�ʹ�õ��� ħ����
                        bool isCan = Data.CanUndo();
                        if (isCan)
                        {
                            Data.TakeMemorySnapshotBoardData();
                            Core.Dispatch(GameMsg.CostExternalProp, propType, list);
                            List<Vector2Int> matches = FutureCore.ListPool<Vector2Int>.Get();
                            List<ElementData> propList = FutureCore.ListPool<ElementData>.Get();
                            ActivateProp_Wild(pot.x, pot.y, type, ref matches, ref propList);

                            if (matches.Count > 0)
                            {
                                // ��ƥ�䣬��������
                                ProcessMatches(matches);
                                // ������Ԫ�� ����λ
                                FillEmptySpaces();
                            }

                            FutureCore.ListPool<Vector2Int>.Release(matches);
                            FutureCore.ListPool<ElementData>.Release(propList);

                            isCan = true;
                        }
                    }
                    break;
                case ExternalProp.AddScore:
                    {
                        Core.Dispatch(GameMsg.CostExternalProp, propType, list);
                        AddScore(Core.GetExternalProp_AddSocre());
                        isSu = true;
                    }
                    break;

            }

            return isSu;






        }

        /*

        /// <summary>
        /// ���ɵ���
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
                // ��Ԫ��λ�����ɵ���
                Destroy(elementObjects[x, y]);
                GameObject prop = Instantiate(propPrefab, new Vector3(x, y, 0), Quaternion.identity);
                elementObjects[x, y] = prop;

                // ���ӵ��߽ű�
                GameProp propScript = prop.AddComponent<GameProp>();
                propScript.Initialize(propType, x, y);
                propScript.OnPropClicked += OnPropClicked;
            }
        }






        */
        #endregion




    }
}
