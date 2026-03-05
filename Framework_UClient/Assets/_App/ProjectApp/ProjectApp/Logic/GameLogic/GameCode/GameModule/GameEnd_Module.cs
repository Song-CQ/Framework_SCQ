
using FutureCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class GameEnd_Module : IGameModule
    {
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
            Dispatcher.AddListener(GameMsg.GameWin,OnGameWin);
          
        }

        private void OnGameWin(object obj)
        {
            Core.Enabled_PlayerCtr = false;

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

        }
       

       public void GenerateInitialElements()
        {
            
        }
        



        void IGameModule.Dispose()
        {
            RemoveListener();
            Core = null;
        }

        


        #endregion




    }
}
