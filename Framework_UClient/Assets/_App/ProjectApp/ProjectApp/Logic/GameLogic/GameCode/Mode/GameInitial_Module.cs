using FlyingWormConsole3;
using FutureCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class GameInitial_Module : IGameModule
    {

        #region 流程
        public Dispatcher<uint> Dispatcher => Core.Dispatcher;
        public ElementGameData Data => Core.Data;

        public EliminateGameCore Core { get; private set; }

        void IGameModule.FillCore(EliminateGameCore _core)
        {
            Core = _core;
        }

        void IGameModule.AddListener()
        {
        }
        public void RemoveListener()
        {

        }
        /// <summary>
        /// 初始化棋盘
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        void IGameModule.InitializeBoard(int w, int h)
        {
            Data.boardData = new ElementData[w, h];
            Data.boardSize = new Vector2Int(w, h);
        }
        void IGameModule.GenerateInitialElements()
        {
            for (int x = 0; x < Data.boardSize.x; x++)
            {
                for (int y = 0; y < Data.boardSize.y; y++)
                {
                    ElementData data = Core.GetRandomElementData();
                    data.SetPot(x,y);

                    Data.boardData[x,y] = data;

                }
            }

            // 检查并消除初始匹配
            CheckInitialMatches();


        }

        void IGameModule.Dispose()
        {
            RemoveListener();
            Core = null;
        }


        #endregion


        /// <summary>
        /// 检查初始匹配（避免开局就有匹配）
        /// </summary>
        void CheckInitialMatches()
        {
            bool hasMatches = true;
            int attempts = 0;
            List<Vector2Int> initialMatches = ListPool<Vector2Int>.Get();
            while (hasMatches && attempts < 10)
            {
                hasMatches = false;
                initialMatches = Core.FindAllMatches(initialMatches);

                if (initialMatches.Count > 0)
                {
                    hasMatches = true;
                    // 重新生成有匹配的位置
                    foreach (Vector2Int pot in initialMatches)
                    {
                        ElementData elementData = Core.GetRandomElementData();
                        elementData.SetPot(pot.x,pot.y);

                        Data.SetElementData(elementData);
                    }
                }

                attempts++;
            }
            ListPool<Vector2Int>.Release(initialMatches);
        }

    }
}
