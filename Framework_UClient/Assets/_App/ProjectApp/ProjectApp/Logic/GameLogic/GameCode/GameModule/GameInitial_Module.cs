
using FutureCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class GameInitial_Module : IGameModule
    {
        #region б�Խ�����
        const int MAX_ATTEMPTS_PER_CONNECTION = 100;

        private int linkBoardPotSum = 8;


        // ���ѡ��б�ԽǷ���
        private static Vector2Int[] diagonalDirections = new Vector2Int[]
        {
            new Vector2Int(1, 1),   // ����
            new Vector2Int(-1, 1),  // ����
            new Vector2Int(1, -1),  // ����
            new Vector2Int(-1, -1)  // ����
        };
        #endregion

        #region ����
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
        /// ��ʼ������
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        void IGameModule.InitializeBoard(int w, int h)
        {
            Data.boardData = new ElementData[w, h];
            Data.boardSize = new Vector2Int(w, h);

            Data.linkBoardPotLength = linkBoardPotSum;

            Core.GetActiveSum(true);
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

            // ��鲢������ʼƥ��
            CheckInitialMatches();


            Data.boardData[6, 5].SetType( ElementType.Prop_Horizontal);
            Data.boardData[5, 5].SetType( ElementType.Prop_Vertical);


        }

        private void RandomLinkPot(int sum)
        {

            // ʹ��HashSet<long>���HashSet<string>
            HashSet<long> _connectionSet = new HashSet<long>();
            int attempts = 0;

            for (int i = 0; i < sum; i++)
            {
                bool validConnectionFound = false;
                attempts = 0;

                while (!validConnectionFound && attempts < MAX_ATTEMPTS_PER_CONNECTION)
                {
                    attempts++;

                    // ���ѡ�����
                    int startX = GameTool.RandomToInt(0, Data.boardSize.x);
                    int startY = GameTool.RandomToInt(0, Data.boardSize.y);

                    Vector2Int direction = diagonalDirections[GameTool.RandomToInt(0, diagonalDirections.Length)];
                    Vector2Int start = new Vector2Int(startX, startY);
                    Vector2Int end = new Vector2Int(startX + direction.x, startY + direction.y);

                    // ����յ��Ƿ������̷�Χ��
                    if (end.x >= 0 && end.x < Data.boardSize.x &&
                        end.y >= 0 && end.y < Data.boardSize.y)
                    {
                        validConnectionFound = Data.AddConnection(start, end);
                    }
                }

                // ����Ҳ�����Ч���ӣ�ʹ��Ĭ������
                if (!validConnectionFound)
                {
                    // ʹ�õ�һ�����õ�б�Խ�����
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
        /// ����ʼƥ�䣨���⿪�־���ƥ�䣩
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
                    // ����������ƥ���λ��
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
