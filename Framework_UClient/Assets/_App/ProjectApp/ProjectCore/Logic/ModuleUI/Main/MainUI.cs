/****************************************************
    文件:MainUI.cs
    作者:Clear
    日期:2022/1/29 23:8:15
    类型:MVC_AutoCread
    功能:MainUI界面
*****************************************************/
using FutureCore;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using ProjectApp.Data;
//using UI.G000_Main;

namespace ProjectApp
{
    public class MainUI : BaseUI
    {
        #region 控件常量
        //框架自动创建请勿在此处修改内容
        private const string ui_OnClick_Key = "ui_OnClick";
        private const string ui_InputFieldScor_Key = "ui_InputFieldScor";
        private const string ui_InputFieldLv_Key = "ui_InputFieldLv";

        #endregion
        
        private MainUICtrl uiCtrl;
        private MainModel model;
        private UGUIEntity u_Entity;
        public static int GameRandomSeed = 13213;
        public static int GameLV = 50000;
        private TMP_InputField inputFieldLv;
        private TMP_InputField inputScoreField;

        public MainUI(MainUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.MainUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "Main";
            uiInfo.assetName = "Main_Plane";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = true;
            uiInfo.isNeedCloseAnim = false;
            uiInfo.isNeedUIMask = true;
           
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.MainModel) as MainModel;
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;
        }

        protected override void OnOpenBefore(object args)
        {
            u_Entity.GetComponent<Button>(ui_OnClick_Key).onClick.AddListener(OnClick);
            inputFieldLv = u_Entity.GetComponent<TMP_InputField>(ui_InputFieldLv_Key);
            inputScoreField = u_Entity.GetComponent<TMP_InputField>(ui_InputFieldScor_Key);
        }

        private void OnClick()
        {
            Close();
            GameLV = int.Parse(inputFieldLv.text);
            GameRandomSeed  = int.Parse(inputScoreField.text);

            var lv = LevelVOModel.Instance.GetVO(GameLV);
            if (lv != null)
            {
                GameManager.Instance.EnterGame();
            }
            else
            {
                inputFieldLv.text = "关卡:"+GameLV+"不存在";
            }
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