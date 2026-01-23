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
            element.Transform.SetActive(false);
            element.Transform.SetParent(elementsPoolTrf);
            element.Release();
        }

        private void OnGetElement(ElementItem element)
        {
            element.Transform.SetActive(true);
            element.Transform.SetParent(elementItemsTrf);
        }
        private FutureCore.ObjectPool<ElementItem> elementsPool;

        private Transform elementsPoolTrf;

        private Transform elementItemsTrf;

        #endregion

        #region 画面显示流程

        private class VisuaProcess
        {
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
            public float duration;
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
        private Queue<VisuaProcess> visuaProcessesQueue;
        private VisuaProcess currVisuaProcess;
        private void AddVisuaProcess(VisuaProcess process)
        {
            visuaProcessesQueue.Enqueue(process);
        }
        private VisuaProcess NextProcess()
        {
            if (visuaProcessesQueue.Count > 0)
            {
                var process = visuaProcessesQueue.Dequeue();
                process.Execute();
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
                    //回收 当前流程 
                }
            }


        }
        #endregion

        #region 流程

        public Dispatcher<uint> Dispatcher => Core.Dispatcher;
        public EliminateGameData Data => Core.Data;

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

            visuaProcessesQueue = new Queue<VisuaProcess>();

        }

        public void AddListener()
        {
            Dispatcher.AddFinallyListener(GameMsg.DeselectElement, OnDeselectElement);
            Dispatcher.AddFinallyListener(GameMsg.SwapElements, OnSwapElements);
            Dispatcher.AddFinallyListener(GameMsg.ClearElements, OnClearElements);
            Dispatcher.AddFinallyListener(GameMsg.GenerateElements, OnGenerateElements);
            Dispatcher.AddFinallyListener(GameMsg.SelectElement, OnSelectElement);

        }

        public void RemoveListener()
        {
            Dispatcher.RemoveFinallyListener(GameMsg.SelectElement, OnSelectElement);
            Dispatcher.RemoveFinallyListener(GameMsg.DeselectElement, OnDeselectElement);
            Dispatcher.RemoveFinallyListener(GameMsg.SwapElements, OnSwapElements);
            Dispatcher.RemoveFinallyListener(GameMsg.ClearElements, OnClearElements);
            Dispatcher.RemoveFinallyListener(GameMsg.GenerateElements, OnGenerateElements);

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

            foreach (var item in elementItems)
            {
                elementsPool.Release(item);
            }
            elementItems = null;
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

            while (visuaProcessesQueue.Count > 0)
            {
                var process = visuaProcessesQueue.Dequeue();
                process.Release();
            }
            visuaProcessesQueue = null;
            VisuaProcess.ClearPool();

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
            Vector3 position = startVector3 + new Vector3(X, Y, 0);
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
            if (data.X >= Data.BoardWidth || data.Y >= Data.BoardHeight) return null;

            return elementItems[data.X, data.Y];

        }
        #endregion



        // ========== 事件处理方法（方法签名，无实现代码） ==========

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
            ElementItem item1 = FindElementItem(elementDatas[0]);
            ElementItem item2 = FindElementItem(elementDatas[1]);

            Vector3 pot1 = item1.Pos;
            Vector3 pot2 = item2.Pos;
            // 2. 创建动画流程

            var process = VisuaProcess.Get();
            process.SetLinkExecute((p) =>
            {
                Core.Enabled_PlayerCtr = false;
                p.duration = AnimationSys.PlaySwapAin(item1, item2);
            });

            process.SetLinkFinish((p) =>
            {
                Core.Enabled_PlayerCtr = true;
            });

            AddVisuaProcess(process);


        }



        /// <summary>
        /// 处理元素清除事件
        /// </summary>
        /// <param name="data">事件数据，包含需要清除的元素位置列表</param>
        private void OnClearElements(object data)
        {
            // TODO: 实现元素清除逻辑
            // 1. 获取需要清除的元素列表
            // 2. 播放清除动画
            // 3. 计算清除得分
            // 4. 从游戏板移除元素
        }

        /// <summary>
        /// 处理元素生成事件
        /// </summary>
        /// <param name="data">事件数据，包含需要生成元素的空缺位置</param>
        private void OnGenerateElements(object data)
        {
            // TODO: 实现元素生成逻辑
            // 1. 获取空缺位置列表
            // 2. 生成新的随机元素
            // 3. 播放生成动画
            // 4. 填充游戏板空缺
        }








    }
}
