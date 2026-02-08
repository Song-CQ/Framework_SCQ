using FutureCore;
using ProjectApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectApp
{
    public class ExternalProp_Module : IGameModule
    {
        public Dispatcher<uint> Dispatcher => Core.Dispatcher;
        public ElementGameData Data => Core.Data;

        public EliminateGameCore Core { get; private set; }

        public ExternalProp SelectExternalProp { get; set; } = ExternalProp.None;




        public void FillCore(EliminateGameCore _core)
        {
            Core = _core;


        }


        public void GenerateInitialElements()
        {

        }

        public void InitializeBoard(int w, int h)
        {

        }
        public void AddListener()
        {

        }
        public void RemoveListener()
        {

        }
        /// <summary>
        /// 消耗外置道具
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ConsumeExternalProp(ExternalProp type)
        {



        }























        public void Dispose()
        {

        }


    }
}
