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
        private const string ui_PropItem_Key = "ui_PropItem";

        #endregion

        private UsePropUICtrl uiCtrl;
        private UsePropModel model;
        private UGUIEntity u_Entity;

        private GuideMask ui_mask;
        private RectTransform ui_maskRtf;
        private RectTransform ui_HoleImage;


        private ExternalPropUIList_Item item;



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
            ui_maskRtf = ui_mask.GetComponent<RectTransform>();
            ui_HoleImage = GetComponent<RectTransform>(ui_HoleImage_Key);
            item = GetComponent<ExternalPropUIList_Item>(ui_PropItem_Key);

        }

        private void Ui_mask_PointerClick_Event(UnityEngine.EventSystems.PointerEventData eventData)
        {
            GameTool.GameCore.Dispatch(GameMsg.CancelExternalProp);
        }


        public void SetViewMask()
        {
            (Vector2 L_D, Vector2 L_U, Vector2 R_D, Vector2 R_U) = GameTool.GameCore.GetBoardBoundsToScreenPoint();

            LogUtil.Log("1 :" + L_D.ToString() + L_U.ToString() + R_D.ToString() + R_U.ToString());

            // 直接使用世界坐标转换（绕开局部坐标的锚点问题）
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                ui_maskRtf,
                L_D,
                CameraMgr.Instance.uiCamera,
                out Vector3 worldPos_L_D
            );

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                ui_maskRtf,
                R_U,
                CameraMgr.Instance.uiCamera,
                out Vector3 worldPos_R_U
            );

            LogUtil.Log("2 :" + worldPos_L_D + worldPos_R_U);

            // 将世界坐标转换为相对于父物体的局部坐标
            Vector2 localPos_L_D = ui_maskRtf.parent.InverseTransformPoint(worldPos_L_D);
            Vector2 localPos_R_U = ui_maskRtf.parent.InverseTransformPoint(worldPos_R_U);




            ui_HoleImage.anchorMin = new Vector2(0.5f, 0.5f);
            ui_HoleImage.anchorMax = new Vector2(0.5f, 0.5f);
            ui_HoleImage.pivot = new Vector2(0, 0); // 设置轴心点为左下角

            ui_HoleImage.anchoredPosition = localPos_L_D;

            float uiWidth = Mathf.Abs(localPos_R_U.x - localPos_L_D.x);
            float uiHeight = Mathf.Abs(localPos_R_U.y - localPos_L_D.y);
            ui_HoleImage.sizeDelta = new Vector2(uiWidth, uiHeight);

            LogUtil.Log($"最终位置: {localPos_L_D}, 大小: {ui_HoleImage.sizeDelta}");


        }

        protected override void OnOpenBefore(object args)
        {
            ExternalPropUIList_Item nextItem = args as ExternalPropUIList_Item;
            item.Initialize(0,nextItem.propData);
            item.CloseSumAndBtn();
            item.transform.position = nextItem.transform.position;
            SetViewMask();

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