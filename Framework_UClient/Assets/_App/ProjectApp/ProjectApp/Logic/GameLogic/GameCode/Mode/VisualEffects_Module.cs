using Codice.Client.Common;
using ConsoleE;
using FutureCore;
using ILRuntime.Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
        private ElementItem OnNewElement()
        {
            GameObject go = GameTool.InstantiateElementPrefab();
            ElementItem element = new ElementItem(go.transform);
            return element;
        }

        private void OnRelease(ElementItem element)
        {
            RaycastSys.RegisterEvent_OnClick(element);
            element.Transform.SetParent(elementsPoolTrf);
            element.SetActive(false);
            element.Release();
        }

        private void OnGetElement(ElementItem element)
        {
            element.SetActive(true);
            element.Transform.SetParent(elementItemsTrf);
        }
        private FutureCore.ObjectPool<ElementItem> elementsPool;

        private Transform elementsPoolTrf;

        private Transform elementItemsTrf;

        #endregion

        #region 画面显示流程

        private class VisuaProcess
        {
            public const float CONST_AddTime = 0.01f;
            private static FutureCore.ObjectPool<VisuaProcess> objectPool = new FutureCore.ObjectPool<VisuaProcess>();
            public static VisuaProcess Get()
            {
                return objectPool.Get();
            }
            public static void ClearPool()
            {
                objectPool.Clear();
            }
            public string type;
            public float Duration
            {
                get => duration; 
                set
                {
                    duration = value + CONST_AddTime;
                }
            }
            private float duration = 0.01f;
            private Action<VisuaProcess> _executeCB;
            private Action<VisuaProcess> _finishCB;

            public bool isRun = false;
            public bool isFinish = false;

            public void SetLinkExecute(Action<VisuaProcess> cb)
            {
                _executeCB = cb;
            }
            public void SetLinkFinish(Action<VisuaProcess> cb)
            {
                _finishCB = cb;
            }

            public void Execute()
            {
                _executeCB?.Invoke(this);
                isRun = true;
            }

            public void Run()
            {
                if (!isRun) return;

                duration -= Time.deltaTime;

                if (duration <= 0)
                {
                    Finish();
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
                isRun = false;
                isFinish = false;
                _finishCB = null;
                _executeCB = null;

                objectPool.Release(this);
            }


        }
        private Queue<VisuaProcess> visuaProcessQueue;
        private VisuaProcess currVisuaProcess;
        private VisuaProcess EnqueueVisuaProcess(VisuaProcess process)
        {
            visuaProcessQueue.Enqueue(process);
            return process;
        }

        private VisuaProcess GetProcessToEnqueue()
        {
            return EnqueueVisuaProcess(VisuaProcess.Get());
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
                if (currVisuaProcess.isFinish)
                {
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
            elementItemsTrf.localPosition = Vector3.zero;

            startVector3 = Core.startVector3;


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
            RaycastSys.AddCheckLayerMask("ElementItem");


            AnimationSys = new ElementAni_System();
            AnimationSys.Init();

            visuaProcessQueue = new Queue<VisuaProcess>();

        }

        public void AddListener()
        {
            Dispatcher.AddFinallyListener(GameMsg.DeselectElement, OnDeselectElement);
            Dispatcher.AddFinallyListener(GameMsg.SwapElements, OnSwapElements);
            Dispatcher.AddFinallyListener(GameMsg.ClearElements, OnClearElements);
            Dispatcher.AddFinallyListener(GameMsg.ElementsFall, OnElementsFall);
            Dispatcher.AddFinallyListener(GameMsg.GenerateElements, OnGenerateElements);
            Dispatcher.AddFinallyListener(GameMsg.SelectElement, OnSelectElement);
            Dispatcher.AddFinallyListener(GameMsg.RestAllElements,OnRestAllElements);

        }

        public void RemoveListener()
        {
            Dispatcher.RemoveFinallyListener(GameMsg.SelectElement, OnSelectElement);
            Dispatcher.RemoveFinallyListener(GameMsg.DeselectElement, OnDeselectElement);
            Dispatcher.RemoveFinallyListener(GameMsg.SwapElements, OnSwapElements);
            Dispatcher.RemoveFinallyListener(GameMsg.ClearElements, OnClearElements);
            Dispatcher.RemoveFinallyListener(GameMsg.ElementsFall, OnElementsFall);
            Dispatcher.RemoveFinallyListener(GameMsg.GenerateElements, OnGenerateElements);
            Dispatcher.RemoveFinallyListener(GameMsg.RestAllElements,OnRestAllElements);

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
                    element.Pos = GetPosition(data);

                    elementItems[x, y] = element;

                }
            }


        }

        public void Dispose()
        {
            RemoveListener();

            Core = null;
            startVector3 = Vector3.zero;

            elementItems = null;
            elementsPool.ReleaseAll();
            elementsPool = null;
            GameObject.Destroy(elementsPoolTrf.gameObject);
            GameObject.Destroy(elementItemsTrf.gameObject);
            elementsPoolTrf = null;
            elementItemsTrf = null;

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

        }

        public void Update()
        {
            if (!isInit) return;
            if (Core.Enabled_PlayerCtr)
            {
                RaycastSys.Run();
            }

            AnimationSys.Run();

            RunProcess();
        }


        #endregion

        #region  ElementItemTool

        private Dictionary<Vector2Int,ElementItem> creadElementItemList = new Dictionary<Vector2Int, ElementItem>();

        private void Enqueue_To_CreadList(ElementItem item)
        {
            Vector2Int key = new Vector2Int(item.Data.X,item.Data.Y);
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

        private Vector3 GetPosition(int X, int Y)
        {
            Vector3 position = startVector3 + new Vector3(X, Y, Y*0.1f);
            return position;
        }
        private Vector3 GetPosition(ElementData data)
        {
            return GetPosition(data.X, data.Y); ;
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
        private void SetElementItemPot(ElementItem item, int x,int y)
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

            Vector3 pot1 = item1.Pos;
            Vector3 pot2 = item2.Pos;

            var process = VisuaProcess.Get();
            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                p.Duration = AnimationSys.PlayAin_SwapElement(item1, item2);
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
                var _data = item.Data;
                _data.SetType(ElementType.Fixed_Special);
                item.SetData(_data);
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
                var item  =  CreadElemenItem(_data);
                item.Pos = GetPosition(item.Data);
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
                
                Vector3 pot = GetPosition(tarX, tarY);
                tarPotList.Add(pot);

                //设置item 新位置
                //设置新位置数据
                SetElementItemPot(sourItem, tarX, tarY);
     
            }

            var process = VisuaProcess.Get();
            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                float time = AnimationSys.PlayAin_FallElements(elementItemList, tarPotList);

                p.Duration = time;
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;

                ListPool<ElementItem>.Release(elementItemList);
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
            for (int x = 0; x < Data.BoardWidth; x++)
            {

                for (int y = 0; y < Data.BoardHeight; y++)
                {          
                    elementItems[x, y].SetData(Data.boardData[x, y]);
                    elementItemList.Add(elementItems[x, y]);
                }
            }


            var process = GetProcessToEnqueue();
            process.Duration = 3f;
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
                
                float time = AnimationSys.PlayAin_ElasticShakeElements(elementItemList);

               
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;

                ListPool<ElementItem>.Release(elementItemList);

            });


        }
        #endregion





       
    }
}
