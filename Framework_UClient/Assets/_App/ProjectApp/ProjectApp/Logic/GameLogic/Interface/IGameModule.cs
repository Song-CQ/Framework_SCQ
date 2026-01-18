using FutureCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public interface IGameModule 
    {
        Dispatcher<uint> Dispatcher { get; }
        void FillCore(EliminateGameCore _core);

        void InitializeBoard(int w,int h);
        void AddListener();
        void RemoveListener();
        void Disposed();
    }
}
