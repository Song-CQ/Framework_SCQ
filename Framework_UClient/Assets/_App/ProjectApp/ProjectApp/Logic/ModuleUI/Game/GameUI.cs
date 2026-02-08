/****************************************************
    文件: GameUI.cs
    作者: Clear
    日期: 2023/11/23 20:49:21
    类型: MVC_AutoCread
    功能: GameUI界面
*****************************************************/
using ConsoleE;
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectApp
{
    public class GameUI : BaseUI
    {
        #region 控件常量
        //框架自动创建请勿在此处修改内容(会被覆盖掉)
        private const string ui_PropList_Key = "ui_PropList";

        #endregion
        private GameUICtrl uiCtrl;
        private GameModel model;
        private UGUIEntity u_Entity;

        private UI_List ui_PropList;

        private EliminateGameCore core;

        public GameUI(GameUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.GameUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "Game";
            uiInfo.assetName = "Game_Plane";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = false;
            uiInfo.isNeedCloseAnim = false;
            uiInfo.isNeedUIMask = false;
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.GameModel) as GameModel;
        }

        protected override void OnClose()
        {
        }

        private class PropData
        {
            public ExternalProp type;
            public int Sum;

        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;

            ui_PropList = GetComponent<UI_List>(ui_PropList_Key);
            ui_PropList.updateItemData = UpdataItemData;

            List<ItemData> datas = new List<ItemData>();
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Undo });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Vertical });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Horizontal });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Wild });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Hammer });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.AddScore });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Swipe });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.AllRandom });

            ui_PropList.SetData(datas);

            core = GameTool.GameCore;


        }


        private void UpdataItemData(BaseUIList_Item item, ItemData data)
        {
            ExternalPropUIList_Item externalPropItem = item as ExternalPropUIList_Item;
            externalPropItem.SetClickCallback(OnClickPropItem);
      

        }

        private void OnClickPropItem(BaseUIList_Item item, ItemData data)
        {
           
            ExternalProp type = (ExternalProp)data.IntData;
            core.ClickExternalPropItem(type);



        }

        protected override void OnOpenBefore(object args)
        {
            
        }

        protected override void OnOpen(object args)
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnDisplay(object args)
        {
        }
        #endregion

        #region 消息
        protected override void AddListener()
        {
            //modelDispatcher.AddListener(ModelMsg.XXX, OnXXX);
        }
        protected override void RemoveListener()
        {
            //modelDispatcher.RemoveListener(ModelMsg.XXX, OnXXX);
        }
        #endregion

    }
}