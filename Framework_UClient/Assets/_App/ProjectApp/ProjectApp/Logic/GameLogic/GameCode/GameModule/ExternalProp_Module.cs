using FutureCore;
using ProjectApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectApp
{
    public enum ExternalProp
    {
        None = 0,
        Hammer = 10000,
        Swipe,
        Horizontal,
        Vertical,
        AllRandom,
        Undo,
        Wild,
        AddScore,

    }
    public class ExternalProp_Module : IGameModule
    {
        public Dispatcher<uint> Dispatcher => Core.Dispatcher;
        public ElementGameData Data => Core.Data;

        public EliminateGameCore Core { get; private set; }

        public ExternalProp SelectExternalProp { get; set; } = ExternalProp.None;
        private List<ElementData> selectElementDataList = new List<ElementData>();
        private List<ElementData> dataList = new List<ElementData>();

        private ExternalProp_PlayerData externalProp_PlayerData;

        public void FillCore(EliminateGameCore _core)
        {
            Core = _core;
            externalProp_PlayerData = PlayerDataMgr.Instance.GetData<ExternalProp_PlayerData>();


        }


        public void GenerateInitialElements()
        {

        }

        public void InitializeBoard(int w, int h)
        {

        }
        public void AddListener()
        {
            Dispatcher.AddPriorityListener(GameMsg.CostExternalProp, OnConsumeExternalProp);
            Dispatcher.AddPriorityListener(GameMsg.CancelExternalProp, OnCancelExternalProp);
            
            Dispatcher.AddListener(GameMsg.Player_ClickExternalPropItem, OnPlayer_ClickExternalProp);
            Dispatcher.AddListener(GameMsg.Player_ClickElement, OnPlayer_ClickElement);


        }

        

        public void RemoveListener()
        {
            ///消耗
            Dispatcher.RemovePriorityListener(GameMsg.CostExternalProp, OnConsumeExternalProp);
            Dispatcher.RemovePriorityListener(GameMsg.CancelExternalProp, OnCancelExternalProp);

            Dispatcher.RemoveListener(GameMsg.Player_ClickExternalPropItem, OnPlayer_ClickExternalProp);
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
            if (externalProp_PlayerData.allExternalProp.ContainsKey(type))
            {
                int sum = externalProp_PlayerData.allExternalProp[type];
                if (sum > 0)
                {
                    sum = sum - 1;
                    externalProp_PlayerData.allExternalProp[type] = sum;
                }
            }

        }

        /// <summary>
        /// 取消使用盘外道具
        /// </summary>
        /// <param name="obj"></param>
        private void OnCancelExternalProp(object obj)
        {
            Rest_SelectExternalProp();
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
            else
            {
                //打开使用道具界面
                UICtrlDispatcher.Instance.Dispatch(UICtrlMsg.UsePropUI_Open);
            }

        }

        
        private void OnPlayer_ClickElement(object obj)
        {
            if(SelectExternalProp == ExternalProp.None) return;

            ElementData data = (ElementData)(obj);


            selectElementDataList.Add(data);
            Dispatcher.Dispatch(GameMsg.SelectElement, data);

            if (IsActivateExternalProp(SelectExternalProp))
            {
                ActivateExternalProp(SelectExternalProp);
            }

        }
        
        private void Rest_SelectExternalProp()
        {
            SelectExternalProp = ExternalProp.None;
            foreach (var item in selectElementDataList)
            {
                Dispatcher.Dispatch(GameMsg.DeselectElement, item);
            }

            selectElementDataList.Clear();

            UICtrlDispatcher.Instance.Dispatch(UICtrlMsg.UsePropUI_Close);

        }



        /// <summary>
        /// 该道具是否可以触发
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsActivateExternalProp(ExternalProp type)
        {
            dataList.Clear();
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
                case ExternalProp.Horizontal:
                case ExternalProp.Vertical:
                    {
                        if (selectElementDataList.Count == 1)
                        {
                            isFade = true;
                            dataList.Add(selectElementDataList[0]);
                        }
                        break;
                    }
                case ExternalProp.Hammer:
                case ExternalProp.Wild:
                    {
                        foreach (var item in selectElementDataList)
                        {
                            if (ElementTool.CheckType_CanMatches(item.Type))
                            {
                                dataList.Add(item);
                                isFade = true;
                                break;
                            }
                        }
                        break;
                    }
                    
                case ExternalProp.Swipe:
                    {
                        foreach (var item in selectElementDataList)
                        {
                            if (ElementTool.CheckType_CanMatches(item.Type)||ElementTool.CheckType_IsProp(item.Type))
                            {
                                dataList.Add(item);
                            }

                            if (dataList.Count >= 2)
                            {
                                isFade = true;
                                break;
                            }
                        }      
                        break;
                    }
            }

            return isFade;
        }

        
        //激活外围道具

        private void ActivateExternalProp(ExternalProp propType)
        {
            List<Vector2Int> vector3List = null;
            if(dataList.Count!=0)
            {
                vector3List = ListPool<Vector2Int>.Get();
                foreach (var item in dataList)
                {
                    vector3List.Add(new Vector2Int(item.X,item.Y));
                }
            }

            Rest_SelectExternalProp();
            Core.Dispatch(GameMsg.UseExternalProp,propType,vector3List);
            if(vector3List!=null)
            ListPool<Vector2Int>.Release(vector3List);
        }









        public void Dispose()
        {
            selectElementDataList.Clear();
            selectElementDataList = null;

        }

        
    }
}
