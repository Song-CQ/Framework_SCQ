using FutureCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public interface IGameModule 
    {
        EliminateGameCore Core { get; }
        ElementGameData Data { get; }

        Dispatcher<uint> Dispatcher { get; }


        void FillCore(EliminateGameCore _core);

        void AddListener();
        void RemoveListener();
        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        void InitializeBoard(int w,int h);
        /// <summary>
        /// ���ɳ�ʼ��������
        /// </summary>
        void GenerateInitialElements();
        void Dispose();
    }
}
