using FlyingWormConsole3;
using FutureCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ProjectApp
{
    public class GameInitial_Module : IGameModule
    {
        #region 斜对角生成
        const int MAX_ATTEMPTS_PER_CONNECTION = 100;

        private int linkBoardPotSum = 8;


        // 随机选择斜对角方向
        private static Vector2Int[] diagonalDirections = new Vector2Int[]
        {
            new Vector2Int(1, 1),   // 右下
            new Vector2Int(-1, 1),  // 左下
            new Vector2Int(1, -1),  // 右上
            new Vector2Int(-1, -1)  // 左上
        };
        #endregion

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

            Data.linkBoardPotLength = linkBoardPotSum;
        }
        void IGameModule.GenerateInitialElements()
        {
            RandomLinkPot(linkBoardPotSum);


            for (int x = 0; x < Data.boardSize.x; x++)
            {
                for (int y = 0; y < Data.boardSize.y; y++)
                {
                    ElementData data = Core.GetRandomElementData();
                    data.SetPot(x, y);

                    Data.boardData[x, y] = data;
                }
            }

            // 检查并消除初始匹配
            CheckInitialMatches();


        }

        private void RandomLinkPot(int sum)
        {

            // 使用HashSet<long>替代HashSet<string>
            HashSet<long> _connectionSet = new HashSet<long>();
            int attempts = 0;

            for (int i = 0; i < sum; i++)
            {
                bool validConnectionFound = false;
                attempts = 0;

                while (!validConnectionFound && attempts < MAX_ATTEMPTS_PER_CONNECTION)
                {
                    attempts++;

                    // 随机选择起点
                    int startX = GameTool.RandomToInt(0, Data.boardSize.x);
                    int startY = GameTool.RandomToInt(0, Data.boardSize.y);

                    Vector2Int direction = diagonalDirections[GameTool.RandomToInt(0, diagonalDirections.Length)];
                    Vector2Int start = new Vector2Int(startX, startY);
                    Vector2Int end = new Vector2Int(startX + direction.x, startY + direction.y);

                    // 检查终点是否在棋盘范围内
                    if (end.x >= 0 && end.x < Data.boardSize.x &&
                        end.y >= 0 && end.y < Data.boardSize.y)
                    {
                        validConnectionFound = Data.AddConnection(start, end);
                    }
                }

                // 如果找不到有效连接，使用默认连接
                if (!validConnectionFound)
                {
                    // 使用第一个可用的斜对角连接
                    for (int x = 0; x < Data.boardSize.x && !validConnectionFound; x++)
                    {
                        for (int y = 0; y < Data.boardSize.y && !validConnectionFound; y++)
                        {
                            Vector2Int start = new Vector2Int(x, y);
                            Vector2Int end = new Vector2Int(x + 1, y + 1);

                            if (end.x < Data.boardSize.x && end.y < Data.boardSize.y)
                            {
                                validConnectionFound = Data.AddConnection(start, end);

                            }
                        }
                    }
                }
            }



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
                        elementData.SetPot(pot.x, pot.y);

                        Data.SetElementData(elementData);
                    }
                }

                attempts++;
            }
            ListPool<Vector2Int>.Release(initialMatches);
        }

    }
}
