using FutureCore;
using ProjectApp;
using System;
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
        private List<ElementData> selectElementDataList = new List<ElementData>();




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
            Dispatcher.AddListener(GameMsg.Player_ClickExternalPropItem, OnPlayer_ClickExternalProp);
            Dispatcher.AddListener(GameMsg.CostExternalProp, OnConsumeExternalProp);

            
            Dispatcher.AddListener(GameMsg.Player_ClickElement, OnPlayer_ClickElement);

        }

        public void RemoveListener()
        {
            Dispatcher.RemoveListener(GameMsg.Player_ClickExternalPropItem, OnPlayer_ClickExternalProp);
            ///消耗
            Dispatcher.RemoveListener(GameMsg.CostExternalProp, OnConsumeExternalProp);

            Dispatcher.RemoveListener(GameMsg.Player_ClickElement, OnPlayer_ClickElement);
        }
        
        /// 消耗道具
        private void OnConsumeExternalProp(object obj)
        {
            object[] objects = obj as object[];
            ExternalProp propType = (ExternalProp)objects[0];
            List<Vector2Int> list = objects[1] as List<Vector2Int>;


            
            //消耗
            ConsumeExternalProp(propType);
        }

        /// <summary>
        /// 消耗外置道具
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ConsumeExternalProp(ExternalProp type)
        {



        }

        

        /// <summary>
        /// 点击外置道具
        /// </summary>
        /// <param name="o"></param>
        void OnPlayer_ClickExternalProp(object obj)
        {
            ExternalProp type = (ExternalProp)obj;

            SelectExternalProp = type;
            
            /// 如果是点击后立即触发的
            if (IsActivateExternalProp(type))
            {
                ActivateExternalProp(type);

            }

        }

        
        private void OnPlayer_ClickElement(object obj)
        {
            if(SelectExternalProp == ExternalProp.None) return;

            ElementData data = (ElementData)(obj);


            selectElementDataList.Add(data);

            if (IsActivateExternalProp(SelectExternalProp))
            {
                ActivateExternalProp(SelectExternalProp);
            }

        }
        
        private void Rest_SelectExternalProp()
        {
            SelectExternalProp = ExternalProp.None;

            selectElementDataList.Clear();

        }



        /// <summary>
        /// 该道具是否可以触发
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsActivateExternalProp(ExternalProp type,bool isClick = false)
        {
            bool isFade = false;
            switch (type)
            {
                case ExternalProp.Undo:
                case ExternalProp.AllRandom:
                case ExternalProp.AddScore:
                    {
                        isFade = true;
                        break;
                    }
                case ExternalProp.Hammer:
                case ExternalProp.Horizontal:
                case ExternalProp.Vertical:
                case ExternalProp.Wild:
                    {
                        if(selectElementDataList.Count == 1)
                        {
                            isFade = true;
                        }
                        break;
                    }
                case ExternalProp.Swipe:
                    {
                        if(selectElementDataList.Count == 2)
                        {
                            isFade = true;
                        }
                        break;
                    }
            }

            return isFade;
        }

        
        //激活外围道具

        private void ActivateExternalProp(ExternalProp propType)
        {
            List<Vector3Int> vector3List = null;
            if(selectElementDataList.Count!=0)
            {
                vector3List = ListPool<Vector3Int>.Get();
                foreach (var item in selectElementDataList)
                {
                    vector3List.Add(new Vector3Int(item.X,item.Y));
                }
            }

            Rest_SelectExternalProp();
            Core.Dispatch(GameMsg.UseExternalProp,propType,vector3List);
        }
        









        public void Dispose()
        {
            selectElementDataList.Clear();
            selectElementDataList = null;

        }


    }
}
