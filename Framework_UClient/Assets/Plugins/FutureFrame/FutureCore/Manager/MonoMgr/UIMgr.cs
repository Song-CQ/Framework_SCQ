/****************************************************
    文件：UIMgr.cs
	作者：Clear
    日期：2022/1/15 17:52:12
    类型: 框架核心脚本(请勿修改)
	功能：UI管理器
*****************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{

    public static class UIMgrConst
    {
        public static bool IsEnableOpenUIAnim = true;
        public static bool IsEnableCloseUIAnim = true;
        public static Vector2 OpenUIAnimEffectScale = new Vector2(0.65f, 0.65f);
        public static float UIAnimEffectTime = 0.3f;
        public static float ClickDownAnimEffectScale = 0.9f;
    }

    public class UIMgr : BaseMonoMgr<UIMgr>
    {
        /// <summary>
        /// UI驱动
        /// </summary>
        private BaseUIDriver uiDriver;       
        /// <summary>
        /// 当前打开的UI界面
        /// </summary>
        private Dictionary<UILayerType, List<BaseUI>> openDynamicUI = new Dictionary<UILayerType, List<BaseUI>>();
        /// <summary>
        /// Updata的界面
        /// </summary>
        private List<BaseUI> tickUpdateUILst = new List<BaseUI>();


        public void RegisterUIDriver(BaseUIDriver _uIDriver)
        {
            uiDriver = _uIDriver;
            uiDriver.Register();
        }

        public void RegisterDefaultFont(string Font)
        {
            uiDriver.RegisterDefaultFont(Font);
        }

        public override void Init()
        {
            base.Init();           
            InItUIMgr();
        }

        private void InItUIMgr()
        {
            LogUtil.Log("[UIMgr]InitUIMgr");
            GameObject uiRootParent = new GameObject(AppObjConst.UIGoName);
            AppObjConst.UIGo = uiRootParent;
            AppObjConst.UIGo.layer = LayerMaskConst.UI;
            AppObjConst.UIGo.SetParent(AppObjConst.FutureFrameGo);
            AppObjConst.UIGo.transform.position = new Vector3(CameraConst.UICameraPos.x, CameraConst.UICameraPos.y, 0);
            ///驱动初始化
            uiDriver.Init();
            
           
        }

        #region Process UI

        public void Internal_OpenUI(BaseUI ui, object args)
        {
            if (!IsStartUp) return;
            uiDriver.LoadUI(ui, args,OpenUIProcess);

        }

        private void OpenUIProcess(BaseUI ui, object args)
        {
            AddExistDynamic(ui);

            ui.Process_Bind();
            ui.Process_OpenBefore(args);
            ui.Process_Open(args);

            if (ui.uiInfo.isTickUpdate&&!tickUpdateUILst.Contains(ui))
            {
                tickUpdateUILst.Add(ui);
            }
            if (UIMgrConst.IsEnableOpenUIAnim && ui.uiInfo.isNeedOpenAnim)
            {
                ui.OpenUIAnim(ui.Process_OpenUIAnimEnd);
            }
        }

     

        public void Internal_CloseUI(BaseUI ui)
        {
            if (RemoveExistDynamic(ui))
            {             
                if (ui.uiInfo.isTickUpdate)
                {
                    tickUpdateUILst.Remove(ui);
                }
                ui.Process_Close();
                if (UIMgrConst.IsEnableCloseUIAnim && ui.uiInfo.isNeedCloseAnim)
                {
                    ui.CloseUIAnim(() => uiDriver.DestroyUI(ui));
                }
                else
                {
                    uiDriver.DestroyUI(ui);
                }
            }


        }

        public void Internal_HideUI(BaseUI baseUI)
        {
            baseUI.Process_Hide();
            if (UIMgrConst.IsEnableCloseUIAnim && baseUI.uiInfo.isNeedCloseAnim)
            {
                baseUI.CloseUIAnim(baseUI.Process_CloseUIAnimEnd);
            }

        }

        public void Internal_DisplayUI(BaseUI baseUI, object args)
        {
            baseUI.Process_Display(args);
            if (UIMgrConst.IsEnableCloseUIAnim && baseUI.uiInfo.isNeedCloseAnim)
            {
                baseUI.OpenUIAnim(baseUI.Process_OpenUIAnimEnd);
            }

        }
        #endregion

        #region private
        private void AddExistDynamic(BaseUI baseUI)
        {
            UILayerType uILayerType = baseUI.uiInfo.layerType;
            List<BaseUI> uiList;
            if (!openDynamicUI.TryGetValue(uILayerType, out uiList))
            {
                uiList = new List<BaseUI>();
                openDynamicUI.Add(uILayerType, uiList);
            }
            uiList.Add(baseUI);
        }

        private bool RemoveExistDynamic(BaseUI baseUI)
        {
            UILayerType uILayerType = baseUI.uiInfo.layerType;
            List<BaseUI> uiList;
            if (!openDynamicUI.TryGetValue(uILayerType, out uiList))
            {
                return false;
            }
            uiList.Remove(baseUI);
            return true;
        }



        #endregion


        private void Update()
        {
            if (!IsStartUp) return;
            if (tickUpdateUILst.Count <= 0) return;
            foreach (BaseUI ui in tickUpdateUILst)
            {
                if (ui == null) continue;
                if (ui.isClose) continue;
                if (!ui.isOpen) continue;
                if (!ui.isVisible) continue;
                if (!ui.uiInfo.isTickUpdate) continue;

                ui.OnUpdate();
            }
        }



        public override void Dispose()
        {
            base.Dispose();
            ObjectPoolStatic<UIInfo>.Dispose();
            uiDriver.Dispose();
        }

       
    }
}