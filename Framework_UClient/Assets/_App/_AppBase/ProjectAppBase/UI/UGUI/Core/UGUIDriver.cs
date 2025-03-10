/****************************************************
    文件: UGUIDriver.cs
    作者: Clear
    日期: 2023/4/23 14:58:37
    类型: 框架核心脚本(请勿修改)
    功能: UGUI驱动

    UIGUI界面命名规范:
    1:界面存放路径必须是 /UGUI/XXX(界面或者包名)_UIPack/界面名_Wnd

*****************************************************/
using FutureCore;
using ProjectApp.UGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ProjectApp
{
    public class UGUIDriver : BaseUIDriver
    {

        private Transform UIRoot;
        private Canvas canvas;
        private CanvasScaler canvasScaler;
        private GraphicRaycaster graphicRaycaster;

        private Dictionary<UILayerType, UGUI_Window> uiLayerWindowDict = new Dictionary<UILayerType, UGUI_Window>();

        public override void Init()
        {
            InitUIRoot();
            InitUILayer();
            InitMaskPool();
        }



        private void InitUIRoot()
        {
            canvas = new GameObject("UIRoot").AddComponent<Canvas>();
            
            UIRoot = canvas.transform;
            UIRoot.gameObject.SetParent(AppObjConst.UIGo);
            UIRoot.localPosition = Vector3.zero;
            UIRoot.gameObject.layer = LayerMaskConst.UI;

            canvasScaler = UIRoot.gameObject.AddComponent<CanvasScaler>();
            graphicRaycaster = UIRoot.gameObject.AddComponent<GraphicRaycaster>();
            

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = AppConst.UIResolution;

        }

        public override void InitUILayer()
        {
            for (int i = 0; i < UILayerConst.AllUILayer.Length; i++)
            {
                string name = UILayerConst.AllUILayer[i];//其实每个UIlayer都应该是个Canvas，但是先这样 
                RectTransform rtrf = new GameObject(name).AddComponent<RectTransform>();               
                       
                rtrf.SetParent(UIRoot);
                rtrf.gameObject.layer = LayerMaskConst.UI;
                rtrf.pivot = new Vector2(0.5f, 0.5f);
                rtrf.anchorMax = Vector2.one;
                rtrf.anchorMin = Vector2.zero;
                rtrf.sizeDelta = Vector2.zero;

                rtrf.localPosition = new Vector3(0, 0, i * 1);
                rtrf.localScale = Vector3.one;

                SortingGroup _sortingGroup = rtrf.gameObject.AddComponent<SortingGroup>();
                _sortingGroup.sortingLayerID = SortingLayer.NameToID("UI");
                _sortingGroup.sortingOrder = i * 100;

                UGUI_Window window = new UGUI_Window() {
                    layerType = (UILayerType)i,
                    r_trf = rtrf,
                    sortingGroup = _sortingGroup
                };

                uiLayerWindowDict[window.layerType] = window;
            }
        }

        private void InitMaskPool()
        {
            UIMgr.Instance.ui_GObjectsPool.SetCallBack_onNew(UIConst.UGUI_Mask, (obj) => {
                (obj as GameObject).AddComponent<UIEventListener>();
            });
        }

        public override void Register()
        {
            //todo 加载通用资源
        }

        
        public override void StartUp()
        {
            //设置数据,在startUp流程调用，避免 CameraMgr还未初始化
            canvas.worldCamera = CameraMgr.Instance.uiCamera;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.planeDistance = 10;
            canvas.overrideSorting = true;
            canvas.sortingLayerID = SortingLayer.NameToID("UI");
            canvas.sortingOrder = 0;

        }
        private class OpenUIProcess  
        {
            public object args;

            public BaseUI baseUI;

            public Action<BaseUI, object> openUIProcess;

            public void Run()
            {
                openUIProcess?.Invoke(baseUI, args);
                Release();
            }

            private void Release()
            {
                args = null;
                baseUI = null;
                openUIProcess = null;
                ObjectPoolStatic<OpenUIProcess>.Release(this);
            }
        }

        public override void LoadUI(BaseUI baseUI, object args, Action<BaseUI, object> openUIProcess)
        {
            //todo 后面可以考虑多个界面合并加载
            //资源包
            string path = GetUIPath(baseUI.uiInfo);
            OpenUIProcess _openUIProcess = ObjectPoolStatic<OpenUIProcess>.Get();
            _openUIProcess.args = args;
            _openUIProcess.baseUI = baseUI;
            _openUIProcess.openUIProcess = openUIProcess;
            ResMgr.Instance.LoadUI(path, OnloadUICallBack, _openUIProcess);

        }

        private void OnloadUICallBack(ResLoadInfo res, object param)
        {
            OpenUIProcess _openUIProcess = param as OpenUIProcess;
            CreateUI(res, _openUIProcess.baseUI);

            _openUIProcess.Run();

        }

        private void CreateUI(ResLoadInfo res, BaseUI _ui)
        {
            var obj = res.content.GetAsset<GameObject>(_ui.uiInfo.assetName);
            if (obj==null)
            {
                return;
            }
            GameObject uiObj = GameObject.Instantiate(obj);
            if (uiObj != null)
            {
                UGUIEntity uiEntity = new UGUIEntity(uiObj);
                _ui.uiEntity = uiEntity;
                uiEntity.Transform.pivot = new Vector2(0.5f, 0.5f);
                uiEntity.Transform.anchorMax = Vector2.one;
                uiEntity.Transform.anchorMin = Vector2.zero;

                if (_ui.uiInfo.isNeedUIMask)
                {
                    RectTransform mask = GetUIMask(_ui.uiInfo.uiMaskCustomColor);
                    uiEntity.UIMask = mask.GetComponent<UIEventListener>();
                    mask.SetParent(uiEntity.Transform);
                    mask.SetSiblingIndex(0);
                    mask.anchorMax = Vector2.one;
                    mask.anchorMin = Vector2.zero;
                    mask.sizeDelta = Vector2.zero;
                    mask.localPosition = Vector3.zero;
                    mask.localScale = Vector2.one;
                }
                _ui.currUILayer = _ui.uiInfo.layerType;
                
                uiLayerWindowDict[_ui.currUILayer].AddChild(uiEntity);
                //加载出来后匹配当前层级的适配
                uiEntity.Transform.sizeDelta = Vector2.zero;
                uiEntity.Transform.localPosition = Vector3.zero;
                uiEntity.Transform.localScale = Vector3.one;
            }
        }


        private RectTransform GetUIMask(Color uiMaskCustomColor)
        {
            GameObject mask = UIMgr.Instance.ui_GObjectsPool.Get(UIConst.UGUI_Mask);
            mask.GetComponent<Image>().color = uiMaskCustomColor;
            return mask.transform as RectTransform;
        }

        private string GetUIPath(UIInfo uIInfo)
        {
            return uIInfo.packageName+"_UIPack/"+uIInfo.assetName;
        }

        public override Window GetWindow(UILayerType uILayerType)
        {
            if (uiLayerWindowDict.ContainsKey(uILayerType))
            {
                return uiLayerWindowDict[uILayerType];
            }
            return null;
        }
        public override void DestroyUI(BaseUI ui)
        {
            UGUIEntity entity = ui.uiEntity as UGUIEntity;
            if (ui.uiInfo.isNeedUIMask)
            {
                UIMgr.Instance.ui_GObjectsPool.Release(UIConst.UGUI_Mask, entity.UIMask.gameObject);
            }

            DisposeUI(ui.currUILayer, entity);
            ui.Process_Destroy();
        }

        private void DisposeUI(UILayerType currUILayer, UGUIEntity entity)
        {
            uiLayerWindowDict[currUILayer].RemoveChild(entity);
            entity.Dispose();
        }

        public override void Dispose()
        {

            foreach (var item in uiLayerWindowDict)
            {
                item.Value.Dispose();
            }
            uiLayerWindowDict.Clear();
            uiLayerWindowDict = null;
        }

        public class UGUI_Window : Window
        {
            public UILayerType layerType;
            public RectTransform r_trf;
            public Canvas canvas;
            public SortingGroup sortingGroup;

            public Dictionary<string, UGUIEntity> childDic = new Dictionary<string, UGUIEntity>();

            public void AddChild(UGUIEntity entity)
            {
                entity.Transform.SetParent(r_trf);//更换了父节点后要设置 sizeDelta
                entity.SortingGroup.sortingOrder = sortingGroup.sortingOrder;
                childDic[entity.Name] = entity;
            }

            public void RemoveChild(UGUIEntity entity)
            {
                if (childDic.ContainsKey(entity.Name))
                {
                    entity.Transform.SetParent(null);
                }
                childDic.Remove(entity.Name);
            }

            public override void Dispose()
            {
                r_trf = null;
                childDic.Clear();
            }
        }
    }
}
