/****************************************************
    文件: UsePropUI.cs
    作者: Clear
    日期: 2026/2/13 1:1:48
    类型: MVC_AutoCread
    功能: UsePropUI界面
*****************************************************/
using FutureCore;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectApp
{
    public class UsePropUI : BaseUI
    {
        #region 控件常量
        //框架自动创建请勿在此处修改内容
        private const string ui_mask_Key = "ui_mask";
        private const string ui_HoleImage_Key = "ui_HoleImage";
        private const string ui_TipsText_Key = "ui_TipsText";

        #endregion
        
		private UsePropUICtrl uiCtrl;
        private UsePropModel model;
        private UGUIEntity u_Entity;

        private GuideMask ui_mask;
        private RectTransform ui_HoleImage;

        private RectTransform canvasRect;



        public UsePropUI(UsePropUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.UsePropUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "UseProp";
            uiInfo.assetName = "UseProp_Plane";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedUIMask = false;
        }


        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.UsePropModel) as UsePropModel;
        }

        protected override void OnClose()
        {
        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;

            ui_mask = GetComponent<GuideMask>(ui_mask_Key);
            ui_mask.ClickMask = Ui_mask_PointerClick_Event;
            ui_HoleImage = GetComponent<RectTransform>(ui_HoleImage_Key);
            canvasRect = u_Entity.GetCanvas().GetComponent<RectTransform>();
            
        }

        private void Ui_mask_PointerClick_Event(UnityEngine.EventSystems.PointerEventData eventData)
        {
            GameTool.GameCore.Dispatch(GameMsg.CancelExternalProp);
        }


        public void SetViewMask()
        {
            (Vector2 L_D, Vector2 L_U, Vector2 R_D, Vector2 R_U) = GameTool.GameCore.GetMapToViewPot();



            // 2. 屏幕坐标转UI局部坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                L_D,  // 这里应该是屏幕坐标，不是世界坐标
                CameraMgr.Instance.uiCamera,
                out Vector2 uiPos_L_D
            );

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                R_U,
                CameraMgr.Instance.uiCamera,
                out Vector2 uiPos_R_U
            );


            

            ui_HoleImage.anchorMin = new Vector2(0, 0);
            ui_HoleImage.anchorMax = new Vector2(0, 0);
            ui_HoleImage.pivot = new Vector2(0, 0); // 设置轴心点为左下角

            // 计算尺寸
            float uiWidth = Mathf.Abs(uiPos_R_U.x - uiPos_L_D.x);
            float uiHeight = Mathf.Abs(uiPos_R_U.y - uiPos_L_D.y);

            ui_HoleImage.anchoredPosition = uiPos_L_D;

            // 设置大小
            ui_HoleImage.sizeDelta = new Vector2(uiWidth, uiHeight);


        }

        protected override void OnOpenBefore(object args)
        {
        }

        protected override void OnOpen(object args)
        {
            SetViewMask();
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