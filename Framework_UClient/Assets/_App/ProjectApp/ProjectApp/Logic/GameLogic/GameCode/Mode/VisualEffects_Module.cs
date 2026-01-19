using ConsoleE;
using FutureCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class VisualEffects_Module:IGameModule
    {
        private ElementItem[,] elementItems;

        private Vector3 startVector3;

        #region 对象池
        private ElementItem OnNewElement()
        {
            GameObject go = GameTool.InstantiateElementPrefab();
            ElementItem element = new ElementItem(go.transform);
            return element;
        }

        private void OnRelease(ElementItem element)
        {
            element.Transform.SetActive(false);
            element.Transform.SetParent(elementsPoolTrf);
        }

        private void OnGetElement(ElementItem element)
        {
            element.Transform.SetActive(true);
            element.Transform.SetParent(itemsTrf);
        }
        private ObjectPool<ElementItem> elementsPool;

        private Transform elementsPoolTrf;

        private Transform itemsTrf;
        #endregion

        #region 流程

        public Dispatcher<uint> Dispatcher => Core.Dispatcher;
        public EliminateGameData Data => Core.Data;

        public EliminateGameCore Core { get; private set; }
        public void FillCore(EliminateGameCore eliminateGameCore)
        {
            Core = eliminateGameCore;

            elementsPool = new ObjectPool<ElementItem>(OnNewElement, OnGetElement, OnRelease);
            elementsPoolTrf = new GameObject("elementsPoolTrf").transform;
            elementsPoolTrf.SetParent(Core.transform);
            elementsPoolTrf.localPosition = Vector3.zero;
            
            startVector3 = Core.startVector3;
        }

        public void InitializeBoard(int w, int h)
        {
            elementItems = new ElementItem[w, h];




        }

        public void GenerateInitialElements()
        {

        }

        public void AddListener()
        {
            Dispatcher.AddFinallyListener(GameMsg.DeselectElement, OnDeselectElement);
            Dispatcher.AddFinallyListener(GameMsg.SwapElements, OnSwapElements);
            Dispatcher.AddFinallyListener(GameMsg.ClearElements, OnClearElements);
            Dispatcher.AddFinallyListener(GameMsg.GenerateElements, OnGenerateElements);
            Dispatcher.AddFinallyListener(GameMsg.SelectElement,OnSelectElement);

        }

        public void RemoveListener()
        {
            Dispatcher.RemoveFinallyListener(GameMsg.SelectElement, OnSelectElement);
            Dispatcher.RemoveFinallyListener(GameMsg.DeselectElement, OnDeselectElement);
            Dispatcher.RemoveFinallyListener(GameMsg.SwapElements, OnSwapElements);
            Dispatcher.RemoveFinallyListener(GameMsg.ClearElements, OnClearElements);
            Dispatcher.RemoveFinallyListener(GameMsg.GenerateElements, OnGenerateElements);

        }

        #endregion

        /// <summary>
        /// 创建元素
        /// </summary>
        private void CreadElement(int x, int y, ElementType type)
        {
            //ElementData data = new ElementData();
            //data.X = x; 
            //data.Y = y; 
            //data.Type = type;

            //board[x, y] = data;

            ElementItem element = GetElement(data);

            Vector3 position = startVector3 + new Vector3(x, y, 0);
            element.Pos = position;

        }


        private ElementItem GetElement(ElementData data)
        {
            ElementItem element = elementsPool.Get();

            element.Init(data);
            return element;
        }

        // ========== 事件处理方法（方法签名，无实现代码） ==========

        /// <summary>
        /// 处理元素选择事件
        /// </summary>
        /// <param name="data">事件数据，通常包含被选择的元素信息</param>
        private void OnSelectElement(object data)
        {
            // TODO: 实现元素选择逻辑
            // 1. 获取被选择的元素
            // 2. 高亮显示选中状态
            // 3. 更新选择状态
            // 4. 播放选择音效
        }

        /// <summary>
        /// 处理元素取消选择事件
        /// </summary>
        /// <param name="data">事件数据，通常包含被取消选择的元素信息</param>
        private void OnDeselectElement(object data)
        {
            // TODO: 实现元素取消选择逻辑
            // 1. 移除元素的高亮状态
            // 2. 重置选择状态
            // 3. 清理相关缓存
        }

        /// <summary>
        /// 处理元素交换事件
        /// </summary>
        /// <param name="data">事件数据，包含交换的两个元素信息</param>
        private void OnSwapElements(object data)
        {
            // TODO: 实现元素交换逻辑
            // 1. 解析交换的元素数据
            // 2. 执行交换动画
            // 3. 验证交换结果
            // 4. 检查是否形成匹配
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





        public void Disposed()
        {

        }

       
    }
}
