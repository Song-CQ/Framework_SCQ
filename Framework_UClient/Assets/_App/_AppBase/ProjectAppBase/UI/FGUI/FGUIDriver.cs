/****************************************************
    文件：FGUIDriver.cs
	作者：Clear
    日期：2022/1/15 18:9:24
    类型: 框架基础脚本(请勿修改)
	功能：Fgui UI驱动
*****************************************************/
using FairyGUI;
using FutureCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    public class FGUIDriver : BaseUIDriver
    {
        private Vector2 uiCenterPos;
        private Dictionary<UILayerType, Window> uiLayerWindowDict = new Dictionary<UILayerType, Window>();
        private List<string> commonPackageList = new List<string>();

        private Queue<GGraph> uiMaskCacheQueue = new Queue<GGraph>();
        /// <summary>
        /// 是否自动设置按钮中心点
        /// </summary>
        private bool IsSetButtonPivotCenter  = true;
        

        public override void Register()
        {          
            UIRegister_FGUI.AutoRegisterCommonBinder(); 
            UIRegister_FGUI.AutoRegisterCommonPackages(ref commonPackageList);          
            UIRegister_FGUI.AutoRegisterBinder();
        }
       
        public override void Init()
        {
            InitUILayer();
            InitFguiConfig();
            InitFguiSettings();
            InitFguiCommonPackages();       
        }

        public override void StartUp()
        {
          
        }

        /// <summary>
        /// Fgui基础设置
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void InitFguiConfig()
        {
            /// 设置FGUI的分支
            UIPackage.branch = null;

            /// 设置FGUI的配置
            AppObjConst.UIGo.AddComponent<UIConfig>();
            // UI默认字体
            UIConfig.defaultFont = uiDefaultFontName;
            // UI字体八方向描边效果
            UIConfig.enhancedTextOutlineEffect = true;
            // 关闭Window点击自动排序功能
            UIConfig.bringWindowToFrontOnClick = false;
            // 设置动态窗口的背景颜色
            UIConfig.modalLayerColor = new Color(0f, 0f, 0f, (255f / 2f) / 255f);
            // 设置按钮音效大小
            UIConfig.buttonSoundVolumeScale = 1;

        }

        private void InitFguiSettings()
        {
            DisplayObject.CreateUICacheRoot(AppObjConst.UICacheGo.transform);

            Stage.Instantiate();
            Stage.inst.gameObject.transform.SetParent(AppObjConst.UIGo.transform);
            Stage.inst.gameObject.transform.localPosition = VectorConst.Zero;
            ///设置ui的分辨率
            Vector2Int uiResolution = AppConst.UIResolution;
            GRoot.inst.SetContentScaleFactor(uiResolution.x, uiResolution.y,
                UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
            uiCenterPos = new Vector2(GRoot.inst.width / 2f, GRoot.inst.height / 2f);

        }

        private void InitFguiCommonPackages()
        {
            UIPackage.RemoveAllPackages();
            foreach (var PackageName in commonPackageList)
            {
                AddUIPackage(PackageName);
            }

        }
        public override void InitUILayer()
        {
            for (int i = 0; i < UILayerConst.AllUILayer.Length; i++)
            {
                string name = UILayerConst.AllUILayer[i];
                Window uiLayerWindow = new Window();
                uiLayerWindow.fairyBatching = false;
                uiLayerWindow.name = name;
                uiLayerWindow.displayObject.name = name;
                uiLayerWindow.gameObjectName = uiLayerWindow.name;
                uiLayerWindow.sortingOrder = i * 100;
                uiLayerWindow.Show();
                uiLayerWindow.fairyBatching = false;
                uiLayerWindowDict.Add((UILayerType)i, uiLayerWindow);
            }
        }
        private void AddUIPackage(string packageName)
        {
            if (UIPackage.GetByName(packageName) == null)
            {
                ResMgr.Instance.AddFguiPackage(packageName, GetPackageUIPath(packageName));
                
            }
        }

        private string GetPackageUIPath(string packageName)
        {
            return string.Format("FGUI/{0}", packageName);
        }    

        public override void LoadUI(BaseUI baseUI, object args, Action<BaseUI, object> openUIProcess)
        {

            AddUIPackage(baseUI.uiInfo.packageName);
            CreateUI(baseUI, args);

            openUIProcess?.Invoke(baseUI,args);
        }

        private void CreateUI(BaseUI ui, object args)
        {
            if (string.IsNullOrEmpty(ui.uiName))
            {
                LogUtil.LogErrorFormat("[UIMgr]Create {0} {1}/{2} UI Name Is Null", ui.GetType(), ui.uiInfo.packageName,
                    ui.uiInfo.assetName);
                return;
            }
            GObject gUI = UIPackage.CreateObject(ui.uiInfo.packageName, ui.uiInfo.assetName);
            FGUIEntity uiEntity = new FGUIEntity(gUI.asCom);
            ui.uiEntity = uiEntity;
            // 启用深度自动调整合批
            uiEntity.UI.fairyBatching = true;
            uiEntity.UI.SetSize(GRoot.inst.width, GRoot.inst.height, false);

            SetButtonClickDownEffect(uiEntity.UI);

            if (ui.uiInfo.isNeedUIMask)
            {
                GGraph mask = CreateUIMask(ui.uiInfo.uiMaskCustomColor);
                uiEntity.UIMask = mask;
                uiEntity.UI.AddChildAt(mask,0);
            }
            ui.currUILayer = ui.uiInfo.layerType;
            uiLayerWindowDict[ui.currUILayer].AddChild(uiEntity.UI);

        }
        public override void DestroyUI(BaseUI ui)
        {
            FGUIEntity fGUIEntity = ui.uiEntity as FGUIEntity;
            if (ui.uiInfo.isNeedUIMask)
            {
                ReleaseUIMaskToPool(fGUIEntity);
            }
            DisposeUI(ui.currUILayer, fGUIEntity);
            ui.Process_Destroy();
        }

        private void DisposeUI(UILayerType layerType, FGUIEntity fGUIEntity)
        {
            uiLayerWindowDict[layerType].RemoveChild(fGUIEntity.UI);
            fGUIEntity.Dispose();
        }

        private void ReleaseUIMaskToPool(FGUIEntity fGUIEntity)
        {
            if (fGUIEntity.UIMask == null) return;
            fGUIEntity.UIMask.onClick.Clear();
            fGUIEntity.UIMask.visible = false;
            uiMaskCacheQueue.Enqueue(fGUIEntity.UIMask);
            fGUIEntity.UIMask = null;
        }

        private GGraph CreateUIMask(Color uiMaskCustomColor)
        {
            GGraph uiMask = null;
            if (uiMaskCacheQueue.Count > 0)
            {
                uiMask = uiMaskCacheQueue.Dequeue();
                uiMask.color = uiMaskCustomColor;
                uiMask.alpha = uiMaskCustomColor.a;
                uiMask.visible = true;
            }
            else
            {
                uiMask = new GGraph();
                uiMask.gameObjectName = "UIMask";
                uiMask.name = uiMask.gameObjectName;

                uiMask.SetPivot(0.5f, 0.5f, true);
                uiMask.SetXY(uiCenterPos.x, uiCenterPos.y);
                uiMask.DrawRect(5000, 5000, 0, Color.black, uiMaskCustomColor);
            }
            return uiMask;
        }
        private void SetButtonClickDownEffect(GComponent gComponent)
        {
            foreach (var item in gComponent.GetChildren())
            {
                if (item==null)
                {
                    continue;
                }
                GButton gButton = item.asButton;
                if (gButton!=null && gButton.mode == ButtonMode.Common)
                {
                    if (IsSetButtonPivotCenter)
                    {
                        gButton.SetPivot(0.5f, 0.5f, false);
                    }                 
                    gButton.SetClickDownEffect(UIMgrConst.ClickDownAnimEffectScale);
                }

            }
        }


        public override void Dispose()
        {
            commonPackageList.Clear();
            foreach (var item in uiMaskCacheQueue)
            {
                item.Dispose();
            }
            uiMaskCacheQueue.Clear();
            foreach (var item in uiLayerWindowDict)
            {
                item.Value.Dispose();
            }
            uiLayerWindowDict.Clear();
        }

       
    }
}