using FutureCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectApp
{
    public class VisualEffects_Module : IGameModule
    {
        public ElementItem[,] elementItems;

        private Vector3 startVector3;


        public Raycast3D_System RaycastSys { get; private set; }
        public ElementAni_System AnimationSys { get; private set; }

        private bool isInit;

        #region 对象池

        private FutureCore.ObjectPool<ElementItem> elementsPool;

        private Transform elementsPoolTrf;

        private Transform elementItemsTrf;

        private Transform connectionTrf;


        private ElementItem OnNewElement()
        {
            GameObject go = GameTool.InstantiateElementPrefab();
            ElementItem element = new ElementItem(go.transform);
            return element;
        }

        private void OnRelease(ElementItem element)
        {
            RaycastSys.UnregisterEvent_OnClick(element);
            element.Transform.SetParent(elementsPoolTrf);
            element.Release();
        }

        private void OnGetElement(ElementItem element)
        {
            element.SetActive(true);
            element.Transform.SetParent(elementItemsTrf);
        }


        #endregion

        #region 画面显示流程

        private class VisuaProcess
        {
            public const float CONST_AddTime = 0.02f;
            private static FutureCore.ObjectPool<VisuaProcess> objectPool = new FutureCore.ObjectPool<VisuaProcess>();
            public static VisuaProcess Get()
            {
                return objectPool.Get();
            }
            public static void ClearPool()
            {
                objectPool.Clear();
            }
            public uint id;
            public float Duration
            {
                get => duration;
                set
                {
                    duration = value;
                }
            }
            private float duration = 0.01f;
            private event Action<VisuaProcess> _executeCB;
            private event Action<VisuaProcess> _finishCB;

            public bool isRun = false;
            public bool isFinish = false;

            public void SetLinkExecute(Action<VisuaProcess> cb)
            {
                _executeCB += cb;
            }
            public void SetLinkFinish(Action<VisuaProcess> cb)
            {
                _finishCB += cb;
            }

            public void Execute()
            {
                _executeCB?.Invoke(this);
                isRun = true;
                Debug.Log("流程开始：" + FutureCore.TimerUtil.GetGameTime());
            }

            public void Run()
            {
                if (!isRun) return;

                duration -= UnityEngine.Time.deltaTime;

                if (duration + CONST_AddTime <= 0)
                {
                    Finish();
                    Debug.Log("流程完成：" + FutureCore.TimerUtil.GetGameTime());
                }
            }

            public void Finish()
            {
                _finishCB?.Invoke(this);
                isRun = false;
                isFinish = true;
            }

            public void Release()
            {
                id = 0;
                isRun = false;
                isFinish = false;
                _finishCB = null;
                _executeCB = null;

                objectPool.Release(this);
            }


        }
        private Queue<VisuaProcess> visuaProcessQueue;
        private Dictionary<uint, VisuaProcess> visuaProcessDic;
        private VisuaProcess currVisuaProcess;

        private void InitVisuaProcess()
        {
            visuaProcessQueue = new Queue<VisuaProcess>();
            visuaProcessDic = new Dictionary<uint, VisuaProcess>();
        }
        private VisuaProcess EnqueueVisuaProcess(VisuaProcess process)
        {
            visuaProcessQueue.Enqueue(process);
            if (process.id != 0)
            {
                if (visuaProcessDic.ContainsKey(process.id))
                {
                    visuaProcessDic[process.id].Release();
                }
                visuaProcessDic[process.id] = process;
            }
            return process;
        }

        private VisuaProcess GetProcessToEnqueue(uint id = 0)
        {
            var process = VisuaProcess.Get();
            if (id == 0) return EnqueueVisuaProcess(process);

            if (visuaProcessDic.ContainsKey(id))
            {
                return visuaProcessDic[id];
            }
            else
            {
                process.id = id;
                return EnqueueVisuaProcess(process);
            }

        }
        private VisuaProcess NextProcess()
        {
            if (visuaProcessQueue.Count > 0)
            {
                var process = visuaProcessQueue.Dequeue();
                process.Execute();
                return process;
            }
            return null;
        }

        private void RunProcess()
        {

            if (currVisuaProcess == null)//取出
            {
                currVisuaProcess = NextProcess();
            }

            if (currVisuaProcess != null)// 在下一帧运行
            {
                currVisuaProcess.Run();
                // Debug.Log("运行"+currVisuaProcess.Duration);
                if (currVisuaProcess.isFinish)
                {
                    if (visuaProcessDic.ContainsKey(currVisuaProcess.id))
                    {
                        visuaProcessDic.Remove(currVisuaProcess.id);
                    }
                    currVisuaProcess.Release();
                    currVisuaProcess = null;
                    //回收 当前流程 
                }
            }


        }
        #endregion

        #region 流程

        public Dispatcher<uint> Dispatcher => Core.Dispatcher;
        public ElementGameData Data => Core.Data;

        public EliminateGameCore Core { get; private set; }
        public void FillCore(EliminateGameCore eliminateGameCore)
        {
            Core = eliminateGameCore;

            elementsPool = new ObjectPool<ElementItem>(OnNewElement, OnGetElement, OnRelease);
            elementsPoolTrf = new GameObject("ElementsPool").transform;
            elementsPoolTrf.SetParent(Core.transform);
            elementsPoolTrf.localPosition = Vector3.zero;

            elementItemsTrf = new GameObject("ElementItems").transform;
            elementItemsTrf.SetParent(Core.transform);
            elementItemsTrf.localPosition = Core.startVector3;

            connectionTrf = new GameObject("ConnectionTrf").transform;
            connectionTrf.SetParent(Core.transform);
            connectionTrf.localPosition = Vector3.zero;

            startVector3 = Core.startVector3;

            InitVisuaProcess();

            InitSys();

            isInit = true;

        }



        private void InitSys()
        {
            RaycastSys = new Raycast3D_System();
            RaycastSys.Init();

            RaycastSys.mainCamera = Camera.main;
            RaycastSys.maxDistance = 1000;
            RaycastSys.queryTriggerInteraction = QueryTriggerInteraction.Ignore;
            RaycastSys.ClearCheckAllLayers();
            RaycastSys.AddCheckLayerMask("GameLayer");


            AnimationSys = new ElementAni_System();
            AnimationSys.Init();



        }

        public void AddListener()
        {
            Dispatcher.AddFinallyListener(GameMsg.DeselectElement, OnDeselectElement);
            Dispatcher.AddFinallyListener(GameMsg.SwapElements, OnSwapElements);
            Dispatcher.AddFinallyListener(GameMsg.ClearElements, OnClearElements);
            Dispatcher.AddFinallyListener(GameMsg.ElementsFall, OnElementsFall);
            Dispatcher.AddFinallyListener(GameMsg.GenerateElements, OnGenerateElements);
            Dispatcher.AddFinallyListener(GameMsg.SelectElement, OnSelectElement);
            Dispatcher.AddFinallyListener(GameMsg.RestAllElements, OnRestAllElements);
            Dispatcher.AddFinallyListener(GameMsg.ChangeElementType, OnChangeElementType);
            Dispatcher.AddFinallyListener(GameMsg.ActivateProp, OnActivateProp);
            Dispatcher.AddFinallyListener(GameMsg.ActivateTwoProp, OnActivateTwoProp);
            Dispatcher.AddFinallyListener(GameMsg.CostExternalProp, OnCostExternalProp);
            Dispatcher.AddFinallyListener(GameMsg.ScoreUpdated, OnScoreUpdated);
            Dispatcher.AddFinallyListener(GameMsg.GameWin, OnGameWin);

            InputMgr.OnPinchZoom += OnPinchZoom;

        }

       

        public void RemoveListener()
        {
            Dispatcher.RemoveFinallyListener(GameMsg.SelectElement, OnSelectElement);
            Dispatcher.RemoveFinallyListener(GameMsg.DeselectElement, OnDeselectElement);
            Dispatcher.RemoveFinallyListener(GameMsg.SwapElements, OnSwapElements);
            Dispatcher.RemoveFinallyListener(GameMsg.ClearElements, OnClearElements);
            Dispatcher.RemoveFinallyListener(GameMsg.ElementsFall, OnElementsFall);
            Dispatcher.RemoveFinallyListener(GameMsg.GenerateElements, OnGenerateElements);
            Dispatcher.RemoveFinallyListener(GameMsg.RestAllElements, OnRestAllElements);
            Dispatcher.RemoveFinallyListener(GameMsg.ChangeElementType, OnChangeElementType);
            Dispatcher.RemoveFinallyListener(GameMsg.ActivateProp, OnActivateProp);
            Dispatcher.RemoveFinallyListener(GameMsg.ActivateTwoProp, OnActivateTwoProp);
            Dispatcher.RemoveFinallyListener(GameMsg.CostExternalProp, OnCostExternalProp);
            Dispatcher.RemoveFinallyListener(GameMsg.ScoreUpdated, OnScoreUpdated);
            Dispatcher.RemoveFinallyListener(GameMsg.GameWin, OnGameWin);


            InputMgr.OnPinchZoom -= OnPinchZoom;



        }

        

        public void InitializeBoard(int w, int h)
        {
            elementItems = new ElementItem[w, h];

            RaycastSys.Start();
            AnimationSys.Start();
        }

        public void GenerateInitialElements()
        {
            for (int x = 0; x < Data.BoardWidth; x++)
            {
                for (int y = 0; y < Data.BoardHeight; y++)
                {
                    ElementData data = Data.boardData[x, y];
                    ElementItem element = CreadElemenItem(Data.boardData[x, y]);

                    //设置位置
                    element.Pos = GameTool.GetPosition(data);

                    elementItems[x, y] = element;

                }
            }

            LoadAllConnectionGo();


        }


        private void LoadAllConnectionGo()
        {


            foreach (var item in Data.GetAllConnection())
            {
                Vector2Int start = item.Value.Item1;
                Vector2Int end = item.Value.Item2;

                var dir = GameTool.GetDirection(start, end);
                Vector3 addVector = new Vector3(dir.x, dir.y) * 5f;
                Vector3 pot = startVector3 + GameTool.GetPosition(start.x, start.y) + addVector;
                SpriteRenderer go = GameTool.InstantiateConnectionPrefab().GetComponent<SpriteRenderer>();
                pot.z = 1.5f;
                go.name = (start + "-" + end);
                go.transform.SetParent(connectionTrf);
                go.transform.localPosition = pot;
                go.transform.localScale = Vector3.one * 4f;
                go.flipX = start.y > end.y;



            }

        }

        public void Dispose()
        {
            RemoveListener();

            Core = null;
            startVector3 = Vector3.zero;

            elementItems = null;
            elementsPool.ReleaseAll();
            foreach (var item in elementsPool.GetAll())
            {
                item.Dispose();
            }
            elementsPool.Dispose();
            elementsPool = null;
            GameObject.Destroy(elementsPoolTrf.gameObject);
            GameObject.Destroy(elementItemsTrf.gameObject);
            GameObject.Destroy(connectionTrf.gameObject);
            elementsPoolTrf = null;
            elementItemsTrf = null;
            connectionTrf = null;

            RaycastSys.Shutdown();
            RaycastSys.Dispose();
            RaycastSys = null;

            AnimationSys.Shutdown();
            AnimationSys.Dispose();
            AnimationSys = null;

            while (visuaProcessQueue.Count > 0)
            {
                var process = visuaProcessQueue.Dequeue();
                process.Release();
            }
            visuaProcessQueue = null;
            VisuaProcess.ClearPool();

            ListPool<ElementItem>.Clear();

            CameraMgr.Instance.mainCamera.orthographicSize = GameTool.DefOrthographicSize;


        }

        public void Update()
        {
            if (!isInit) return;

            RaycastSys.Run();

            AnimationSys.Run();

            RunProcess();

            UpdateOrthographicSize();

        }

        #endregion

        #region  ElementItemTool

        private Dictionary<Vector2Int, ElementItem> creadElementItemList = new Dictionary<Vector2Int, ElementItem>();


        private void Enqueue_To_CreadList(ElementItem item)
        {
            Vector2Int key = new Vector2Int(item.Data.X, item.Data.Y);
            if (creadElementItemList.ContainsKey(key))
            {
                Debug.LogError("当前的位置已经有了一个ElementItem：回收原来的ElementItem");
                elementsPool.Release(creadElementItemList[key]);
                creadElementItemList[key] = item;
            }
            else
            {
                creadElementItemList[key] = item;
            }


        }
        private ElementItem Dequeue_To_CreadList(ElementData data)
        {
            Vector2Int key = new Vector2Int(data.X, data.Y);
            if (creadElementItemList.ContainsKey(key))
            {
                ElementItem elementItem = creadElementItemList[key];
                creadElementItemList.Remove(key);
                return elementItem;
            }
            return null;
        }

        /// <summary>
        /// 创建元素
        /// </summary>
        private ElementItem CreadElemenItem(ElementData data)
        {
            ElementItem element = elementsPool.Get();
            element.Init(data);
            RaycastSys.RegisterEvent_OnClick(element);
            return element;
        }



        /// <summary>
        /// 查找棋盘对应的元素
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ElementItem FindElementItem(ElementData data)
        {
            return FindElementItem(data.X, data.Y);
        }
        /// <summary>
        /// 设置item 位置
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="data"></param>
        private void SetElementItemPot(ElementItem item, int x, int y)
        {
            if (x >= Data.BoardWidth || y >= Data.BoardHeight) return;
            //这个是错误的 这修改的其实是返回属性的副本
            //item.Data.SetPot(x, y);
            ElementData data = item.Data;
            data.SetPot(x, y);
            item.SetData(data);
            elementItems[x, y] = item;
        }



        private ElementItem FindElementItem(int x, int y)
        {
            if (x >= Data.BoardWidth || y >= Data.BoardHeight) return null;

            return elementItems[x, y];

        }


        private List<ElementItem> FindElementItem(List<ElementData> pots, ref List<ElementItem> elementItemList)
        {
            if (elementItemList == null) elementItemList = ListPool<ElementItem>.Get();

            foreach (var item in pots)
            {
                elementItemList.Add(FindElementItem(item.X, item.Y));
            }
            return elementItemList;
        }

        #endregion


        #region 事件处理方法
        //========== 事件处理方法 ==========



        /// <summary>
        /// 处理元素选择事件
        /// </summary>
        /// <param name="data">事件数据，通常包含被选择的元素信息</param>
        private void OnSelectElement(object data)
        {
            // TODO: 实现元素选择逻辑
            ElementData elementData = (ElementData)data;
            // 1. 获取被选择的元素
            ElementItem item = FindElementItem(elementData);
            // 2. 高亮显示选中状态
            if (item != null)
            {
                // 3. 更新选择状态
                item.SetSelect(true);
                // 4. 播放选择音效
            }

        }

        /// <summary>
        /// 处理元素取消选择事件
        /// </summary>
        /// <param name="data">事件数据，通常包含被取消选择的元素信息</param>
        private void OnDeselectElement(object data)
        {
            // TODO: 实现元素取消选择逻辑
            // TODO: 实现元素选择逻辑
            ElementData elementData = (ElementData)data;
            // 1. 获取被选择的元素
            ElementItem item = FindElementItem(elementData);
            // 2. 重置选择状态
            if (item != null)
            {
                // 3. 更新选择状态
                item.SetSelect(false);
                // 4. 播放选择音效

            }

        }

        /// <summary>
        /// 处理元素交换事件
        /// </summary>
        /// <param name="data">事件数据，包含交换的两个元素信息</param>
        private void OnSwapElements(object data)
        {
            // TODO: 实现元素交换逻辑
            // 1. 解析交换的元素数据
            List<ElementData> elementDatas = data as List<ElementData>;
            ElementData itemData1 = elementDatas[0];
            ElementData itemData2 = elementDatas[1];

            ElementItem item1 = FindElementItem(itemData1);
            ElementItem item2 = FindElementItem(itemData2);

            //设置新位置数据
            SetElementItemPot(item1, itemData2.X, itemData2.Y);
            SetElementItemPot(item2, itemData1.X, itemData1.Y);

            // 2. 创建动画流程

            Vector3 item1Pot = GameTool.GetPosition(itemData1);
            Vector3 item2Pot = GameTool.GetPosition(itemData2);

            var process = VisuaProcess.Get();
            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                p.Duration = AnimationSys.PlayAin_SwapElement(item1, item2, item1Pot, item2Pot);
                Debug.Log("当前触发" + UnityEngine.Time.time + "次序事件：" + p.Duration);
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;
            });

            EnqueueVisuaProcess(process);


        }

        /// <summary>
        /// 处理元素清除事件
        /// </summary>
        /// <param name="data">事件数据，包含需要清除的元素位置列表</param>
        private void OnClearElements(object data)
        {
            // TODO: 实现元素清除逻辑
            // 1. 获取需要清除的元素列表
            List<ElementData> matches = data as List<ElementData>;

            List<ElementItem> elementItemList = ListPool<ElementItem>.Get();
            elementItemList = FindElementItem(matches, ref elementItemList);

            // 设置item 新值
            foreach (var item in elementItemList)
            {
                //设置下落标记
                item.SetSpecial();
            }

            // 2. 播放清除动画
            var process = VisuaProcess.Get();
            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                float time = AnimationSys.PlayAin_ClearElements(elementItemList);

                p.Duration = time;
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;
                foreach (var item in elementItemList)
                {
                    elementsPool.Release(item);
                }
                ListPool<ElementItem>.Release(elementItemList);
            });

            EnqueueVisuaProcess(process);


        }

        /// <summary>
        /// 处理元素生成事件
        /// </summary>
        /// <param name="data">事件数据，包含需要生成元素的空缺位置</param>
        private void OnGenerateElements(object data)
        {
            // TODO: 实现元素生成逻辑
            // 1. 获取要生成的列表
            List<ElementData> creadDatas = data as List<ElementData>;
            List<ElementItem> elementItemList = ListPool<ElementItem>.Get();

            foreach (var _data in creadDatas)
            {
                var item = CreadElemenItem(_data);
                item.Pos = GameTool.GetPosition(item.Data);
                item.SetActive(false);

                Enqueue_To_CreadList(item);
                elementItemList.Add(item);
            }

            var process = VisuaProcess.Get();

            process.SetLinkFinish((p) =>
            {

                foreach (var item in elementItemList)
                {
                    item.SetActive(true);

                }
                ListPool<ElementItem>.Release(elementItemList);
            });

            EnqueueVisuaProcess(process);


        }

        /// <summary>
        /// 处理元素下落事件
        /// </summary>
        /// <param name="data">事件数据，包含需要生成元素的空缺位置</param>
        private void OnElementsFall(object data)
        {
            object[] datas = data as object[];
            //要下落的元素
            List<ElementData> souList = datas[0] as List<ElementData>;
            //下落元素的目标
            List<ElementData> tarList = datas[1] as List<ElementData>;
            //先找到下落元素


            List<ElementItem> elementItemList = ListPool<ElementItem>.Get();
            List<Vector3> formPotList = ListPool<Vector3>.Get();
            List<Vector3> tarPotList = ListPool<Vector3>.Get();

            for (int i = 0; i < souList.Count; i++)
            {
                ElementData sourData = souList[i];
                int tarX = tarList[i].X;
                int tarY = tarList[i].Y;
                //先去创新列表找
                ElementItem sourItem = Dequeue_To_CreadList(sourData);
                //如果新创建的列表没有 从棋盘列表找
                if (sourItem == null)
                {
                    sourItem = FindElementItem(sourData);
                }
                elementItemList.Add(sourItem);

                Vector3 formPot = GameTool.GetPosition(sourItem.Data);
                formPotList.Add(formPot);

                Vector3 toPot = GameTool.GetPosition(tarX, tarY);
                tarPotList.Add(toPot);


                //设置item 新位置
                //设置新位置数据
                SetElementItemPot(sourItem, tarX, tarY);

            }

            var process = VisuaProcess.Get();
            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                float time = AnimationSys.PlayAin_FallElements(elementItemList, formPotList, tarPotList);

                p.Duration = time;
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;

                ListPool<ElementItem>.Release(elementItemList);
                ListPool<Vector3>.Release(formPotList);
                ListPool<Vector3>.Release(tarPotList);
            });

            EnqueueVisuaProcess(process);

        }

        /// <summary>
        /// 处理改变当前元素类型的事件
        /// </summary>
        private void OnRestAllElements(object obj)
        {

            List<ElementItem> elementItemList = ListPool<ElementItem>.Get();
            List<Vector3> potList = ListPool<Vector3>.Get();

            for (int x = 0; x < Data.BoardWidth; x++)
            {

                for (int y = 0; y < Data.BoardHeight; y++)
                {
                    elementItems[x, y].SetData(Data.boardData[x, y]);
                    elementItemList.Add(elementItems[x, y]);

                    potList.Add(GameTool.GetPosition(x, y));

                }

            }

            //elementItemList.Clear();
            //elementItemList.Add(elementItems[0, 0]);


            var process = GetProcessToEnqueue();
            process.Duration = 2f;
            process.SetLinkExecute((p) =>
            {

                Core.Enabled_PlayerCtr = false;
                for (int x = 0; x < Data.BoardWidth; x++)
                {

                    for (int y = 0; y < Data.BoardHeight; y++)
                    {
                        elementItems[x, y].RefreshView();
                    }
                }

                float time = AnimationSys.PlayAin_ElasticShakeElements(elementItemList, potList);

            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;

                ListPool<ElementItem>.Release(elementItemList);
                ListPool<Vector3>.Release(potList);
            });


        }

        /// <summary>
        /// 处理特殊元素切换当前显示类型
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnChangeElementType(object obj)
        {
            object[] datas = obj as object[];
            //要下落的元素
            ElementData data = (ElementData)datas[0];
            bool isR = (bool)datas[1];

            ElementItem item = FindElementItem(data.X, data.Y);

            if (isR)
            {
                item.SwitchToNext();
            }
            else
            {
                item.SwitchToPrevious();
            }
            elementItems[data.X, data.Y].SetData(data);



        }

        /// <summary>
        /// 处理触发单个道具事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnActivateProp(object obj)
        {
            object[] datas = obj as object[];
            //要激活的道具
            uint indexId = (uint)datas[0];
            ElementData data = (ElementData)datas[1];
            List<Vector2Int> matches = datas[2] as List<Vector2Int>;
            List<ElementItem> props = datas[3] as List<ElementItem>;


            List<ElementItem> elementItemList = ListPool<ElementItem>.Get();
            List<Vector3> potList = ListPool<Vector3>.Get();

            foreach (var matche in matches)
            {
                ElementItem _item = FindElementItem(matche.x, matche.y);
                elementItemList.Add(_item);
                potList.Add(GameTool.GetPosition(matche.x, matche.y));

            }

            ElementItem item = FindElementItem(data.X, data.Y);

            GetPropAction(indexId, data.Type, item, elementItemList, potList, out Action<VisuaProcess> executeCB, out Action<VisuaProcess> finishCB);

        }

        /// <summary>
        /// 处理触发组合道具事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnActivateTwoProp(object obj)
        {
            object[] datas = obj as object[];
            //要激活的道具
            ElementData formData = (ElementData)datas[0];
            ElementData toData = (ElementData)datas[1];
            List<Vector2Int> matches = datas[2] as List<Vector2Int>;
            List<ElementData> props = datas[3] as List<ElementData>;

            List<ElementItem> elementItemList = ListPool<ElementItem>.Get();
            foreach (var matche in matches)
            {
                ElementItem _item = FindElementItem(matche.x, matche.y);
                elementItemList.Add(_item);
            }
            List<ElementItem> propItemList = ListPool<ElementItem>.Get();
            foreach (var item in props)
            {
                ElementItem _item = FindElementItem(item.X, item.Y);
                propItemList.Add(_item);
            }

            ElementItem formItem = FindElementItem(formData);
            ElementItem toItem = FindElementItem(toData);

            GetTwoPropAction(formItem, toItem, elementItemList, propItemList);



        }

        /// <summary>
        /// 处理成功使用盘外道具
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnCostExternalProp(object obj)
        {
            object[] datas = obj as object[];
            ExternalProp Type = (ExternalProp)datas[0];
            List<Vector2Int> list = datas[1] as List<Vector2Int>;

            switch (Type)
            {
                case ExternalProp.None:
                    break;
                case ExternalProp.Hammer:
                    break;
                case ExternalProp.Swipe:
                    {

                       
                        
                    }
                    break;
                case ExternalProp.Horizontal:
                    break;
                case ExternalProp.Vertical:
                    break;
                case ExternalProp.AllRandom:
                    break;
                case ExternalProp.Undo:
                    break;
                case ExternalProp.Wild:
                    break;
                case ExternalProp.AddScore:
                    break;
                default:
                    break;
            }

        }

        #endregion

        #region 道具
        private float GetPropAction(uint indexId, ElementType type, ElementItem item, List<ElementItem> elementItemList, List<Vector3> potList, out Action<VisuaProcess> executeCB, out Action<VisuaProcess> finishCB)
        {
            float time = 0;
            executeCB = null;
            finishCB = null;

            //将激活的道具设置为空
            item.SetSpecial();


            var process = GetProcessToEnqueue(indexId);

            switch (type)
            {
                case ElementType.Prop_Horizontal:
                    {
                        time = 1.5f;
                        executeCB = (p) =>
                        {
                            AnimationSys.PlayAin_ElasticShakeElement(item, item.Pos);
                            //GameTool.PlayTestEffect(item.Transform.position);
                            AnimationSys.PlayAin_ElasticShakeElements(elementItemList, potList, 0.5f);
                        };
                    }
                    break;
                case ElementType.Prop_Vertical:
                    time = 1.5f;
                    executeCB = (p) =>
                    {
                        Debug.Log("道具开始"+TimerUtil.GetGameTime());
                        AnimationSys.PlayAin_ElasticShakeElement(item, item.Pos);
                        
                        AnimationSys.PlayAin_ElasticShakeElements(elementItemList, potList, 0.5f);
                    };
                    break;
                case ElementType.Prop_Bomb:
                    time = 1.5f;
                    executeCB = (p) =>
                    {
                        AnimationSys.PlayAin_ElasticShakeElement(item, item.Pos);
                        //GameTool.PlayTestEffect(item.Transform.position);
                        AnimationSys.PlayAin_ElasticShakeElements(elementItemList, potList, 0.5f);
                    };

                    break;
                case ElementType.Prop_Wild:
                    time = 1.5f;
                    executeCB = (p) =>
                    {
                        Core.Enabled_PlayerCtr = false;

                        AnimationSys.PlayAin_ElasticShakeElement(item, item.Pos);
                        //GameTool.PlayTestEffect(item.Transform.position);
                        AnimationSys.PlayAin_ElasticShakeElements(elementItemList, potList,0.5f);
                    };

                    break;
            }


            if (finishCB == null)
            {
                finishCB = (p) =>
                {
                    GameTool.PlayTestEffect(item.Transform.position);
                    elementsPool.Release(item);

                    Debug.Log("道具结束"+TimerUtil.GetGameTime());

                    Core.Enabled_PlayerCtr = true;
                    ListPool<ElementItem>.Release(elementItemList);
                    ListPool<Vector3>.Release(potList);

                };
            }

            process.Duration = time > process.Duration ? time : process.Duration;

            process.SetLinkExecute(executeCB);
            process.SetLinkFinish(finishCB);

            return time;

        }


        private float GetTwoPropAction(ElementItem formItem, ElementItem toItem, List<ElementItem> elementItemList, List<ElementItem> propElementList)
        {
            ElementData formData = formItem.Data;
            ElementData toData = toItem.Data;

            formItem.SetSpecial();
            toItem.SetSpecial();
            foreach (var item in elementItemList)
            {
                item.SetSpecial();
            }

            if (toData.Type == ElementType.Prop_Vertical || toData.Type == ElementType.Prop_Horizontal)
            {
                if ((formData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Horizontal) && formData.Type != toData.Type)
                {
                    //横竖
                    return ActivateProp_V_H(formItem, toItem, elementItemList, propElementList);

                }

            }

            if (toData.Type == ElementType.Prop_Bomb || formData.Type == ElementType.Prop_Bomb)
            {
                if (toData.Type == ElementType.Prop_Bomb && formData.Type == ElementType.Prop_Bomb)
                {
                    //双炸弹
                    return ActivateProp_2Bomb(formItem, toItem, elementItemList, propElementList);

                }

                if (toData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Vertical)
                {
                    //炸弹加竖

                    return ActivateProp_Bomb_V(formItem, toItem, elementItemList, propElementList);

                }

                if (toData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Vertical)
                {
                    //炸弹加横

                    return ActivateProp_Bomb_H(formItem, toItem, elementItemList, propElementList);

                }

            }

            if (toData.Type == ElementType.Prop_Wild || formData.Type == ElementType.Prop_Wild)
            {
                if (toData.Type == ElementType.Prop_Bomb || formData.Type == ElementType.Prop_Bomb)
                {
                    //wild加炸弹
                    return ActivateProp_Wild_XX(ElementType.Prop_Bomb, formItem, toItem, elementItemList, propElementList);

                }

                if (toData.Type == ElementType.Prop_Vertical || formData.Type == ElementType.Prop_Vertical)
                {
                    //wild加Vertical

                    return ActivateProp_Wild_XX(ElementType.Prop_Vertical, formItem, toItem, elementItemList, propElementList);

                }

                if (toData.Type == ElementType.Prop_Horizontal || formData.Type == ElementType.Prop_Horizontal)
                {
                    //wild加Horizontal
                    return ActivateProp_Wild_XX(ElementType.Prop_Horizontal, formItem, toItem, elementItemList, propElementList);
                }
            }

            return 0;

        }

        private float ActivateProp_Wild_XX(ElementType type, ElementItem formItem, ElementItem toItem, List<ElementItem> elementItemList, List<ElementItem> propElementList)
        {
            AddFormMoveTo(formItem, toItem);

            List<ElementType> elementTypes = ListPool<ElementType>.Get();
            foreach (var item in propElementList)
            {
                elementTypes.Add(type);
            }

            List<Vector3> potList = ListPool<Vector3>.Get();
            GameTool.GetPositionToList(propElementList, ref potList);


            var process = GetProcessToEnqueue();

            float time = 3;

            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                for (int i = 0; i < propElementList.Count; i++)
                {
                    var item = propElementList[i];
                    item.SetType(elementTypes[i]);
                    item.RefreshView();
                }
                AnimationSys.PlayAin_ElasticShakeElements(propElementList, potList);
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;
                ListPool<ElementItem>.Release(elementItemList);
                ListPool<ElementItem>.Release(propElementList);
                ListPool<ElementType>.Release(elementTypes);
                ListPool<Vector3>.Release(potList);

            });

            process.Duration = time;
            return time;
        }

        private float ActivateProp_Bomb_H(ElementItem formItem, ElementItem toItem, List<ElementItem> elementItemList, List<ElementItem> propElementList)
        {
            AddFormMoveTo(formItem, toItem);


            var process = GetProcessToEnqueue();

            float time = 1;

            List<Vector3> potList = ListPool<Vector3>.Get();
            GameTool.GetPositionToList(elementItemList, ref potList);

            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                AnimationSys.PlayAin_ElasticShakeElements(elementItemList, potList);
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;
                ListPool<ElementItem>.Release(elementItemList);
                ListPool<ElementItem>.Release(propElementList);
                ListPool<Vector3>.Release(potList);

            });

            process.Duration = time;
            return time;
        }

        private float ActivateProp_Bomb_V(ElementItem formItem, ElementItem toItem, List<ElementItem> elementItemList, List<ElementItem> propElementList)
        {
            AddFormMoveTo(formItem, toItem);

            List<Vector3> potList = ListPool<Vector3>.Get();
            GameTool.GetPositionToList(elementItemList, ref potList);


            var process = GetProcessToEnqueue();

            float time = 1;

            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                AnimationSys.PlayAin_ElasticShakeElements(elementItemList, potList);
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;
                ListPool<ElementItem>.Release(elementItemList);
                ListPool<ElementItem>.Release(propElementList);
                ListPool<Vector3>.Release(potList);

            });

            process.Duration = time;
            return time;
        }

        private float ActivateProp_2Bomb(ElementItem formItem, ElementItem toItem, List<ElementItem> elementItemList, List<ElementItem> propElementList)
        {
            AddFormMoveTo(formItem, toItem);

            List<Vector3> potList = ListPool<Vector3>.Get();
            GameTool.GetPositionToList(elementItemList, ref potList);


            var process = GetProcessToEnqueue();

            float time = 1;

            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                AnimationSys.PlayAin_ElasticShakeElements(elementItemList, potList);
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;
                ListPool<ElementItem>.Release(elementItemList);
                ListPool<ElementItem>.Release(propElementList);
                ListPool<Vector3>.Release(potList);
            });

            process.Duration = time;
            return time;
        }

        private float ActivateProp_V_H(ElementItem formItem, ElementItem toItem, List<ElementItem> elementItemList, List<ElementItem> propElementList)
        {
            //移动
            AddFormMoveTo(formItem, toItem);

            List<Vector3> potList = ListPool<Vector3>.Get();
            GameTool.GetPositionToList(elementItemList, ref potList);



            var process = GetProcessToEnqueue();

            float time = 1;

            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                AnimationSys.PlayAin_ElasticShakeElements(elementItemList, potList);
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;
                ListPool<ElementItem>.Release(elementItemList);
                ListPool<ElementItem>.Release(propElementList);
                ListPool<Vector3>.Release(potList);

            });

            process.Duration = time;


            return time;
        }

        private void AddFormMoveTo(ElementItem formItem, ElementItem toItem)
        {
            formItem.SetSpecial();
            toItem.SetSpecial();

            var process = GetProcessToEnqueue();
            Vector3 formPot = formItem.Pos;
            Vector3 tarPot = toItem.Pos;
            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                AnimationSys.PlayAin_MovePot(formItem, formPot, tarPot);
            });

            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = true;
                GameTool.PlayTestEffect(tarPot);

                elementsPool.Release(formItem);
                elementsPool.Release(toItem);

            });
            process.Duration = 0.3f;

        }

        #endregion


        #region 分数和结算

        private void OnScoreUpdated(object obj)
        {

            var process = GetProcessToEnqueue();
            process.SetLinkFinish((p) =>
            {   
                UICtrlDispatcher.Instance.Dispatch(GameMsg.ScoreUpdated,obj);
            });
        }

        private void OnGameWin(object obj)
        {         
            var process = GetProcessToEnqueue();
            process.SetLinkFinish((p) =>
            {   
                UICtrlDispatcher.Instance.Dispatch(GameMsg.GameWin);
            });
        }

        #endregion

        #region 棋盘的缩放

        private float orthographicSize = 120;
        private float orthographicSizeSpeed = 1;
        private void OnPinchZoom(float delta)
        {
            if(!Core.Enabled_PlayerCtr)return;
            CameraMgr.Instance.mainCamera.orthographicSize = GameTool.DefOrthographicSize;
            orthographicSize = orthographicSize + delta*orthographicSizeSpeed;
            orthographicSize = Mathf.Clamp(orthographicSize,GameTool.MinOrthographicSize,GameTool.MaxOrthographicSize);
        }

        private void UpdateOrthographicSize()
        {
            
            CameraMgr.Instance.mainCamera.orthographicSize = orthographicSize;
        }
        #endregion

    }
}
