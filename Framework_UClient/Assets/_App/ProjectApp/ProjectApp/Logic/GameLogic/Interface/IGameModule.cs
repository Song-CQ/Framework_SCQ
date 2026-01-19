using FutureCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public interface IGameModule 
    {
        EliminateGameCore Core { get; }
        EliminateGameData Data { get; }

        Dispatcher<uint> Dispatcher { get; }


        void FillCore(EliminateGameCore _core);

        void AddListener();
        void RemoveListener();
        /// <summary>
        /// 初始化棋盘
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        void InitializeBoard(int w,int h);
        /// <summary>
        /// 生成初始棋盘数据
        /// </summary>
        void GenerateInitialElements();
        void Disposed();
    }
}
