/****************************************************
    文件: UGUIDriver.cs
    作者: Clear
    日期: 2023/4/23 14:58:37
    类型: 框架核心脚本(请勿修改)
    功能: UGUI驱动
*****************************************************/
using FutureCore;
using ProjectApp.UGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectApp
{
    public class UGUIDriver : BaseUIDriver
    {
        
        private Transform UIRoot;
        private Canvas canvas;
        private CanvasScaler canvasScaler;
        private GraphicRaycaster graphicRaycaster;

        private Dictionary<UILayerType, Window> uiLayerWindowDict = new Dictionary<UILayerType, Window>();

        public override void Init()
        {
            InitUIRoot();
            InitUILayer();
            InitMaskPool();
        }

        

        private void InitUIRoot()
        {
            UIRoot = new GameObject("UIRoot").transform;
          
            UIRoot.gameObject.SetParent(AppObjConst.UIGo);
            UIRoot.localPosition = Vector3.zero;
            UIRoot.gameObject.layer = LayerMaskConst.UI;

            canvas = UIRoot.gameObject.AddComponent<Canvas>();
            canvasScaler = UIRoot.gameObject.AddComponent<CanvasScaler>();
            graphicRaycaster = UIRoot.gameObject.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = AppConst.UIResolution;

        }

        public override void InitUILayer()
        {
            

            for (int i = 0; i < UILayerConst.AllUILayer.Length; i++)
            {
                string name = UILayerConst.AllUILayer[i];
                GameObject go = new GameObject(name);
                go.layer = LayerMaskConst.UI;
                go.SetParent(UIRoot);
                go.transform.localPosition = new Vector3(i*10,0,0);
                Window window = new Window() {
                    layerType = (UILayerType)i,
                    trf = go.transform
                };


            }
        }

        private void InitMaskPool()
        {
            UIMgr.Instance.ui_GObjectsPool.SetCallBack_onNew(UIConst.UGUI_Mask,(obj) => {
                obj.gameObject.AddComponent<UIEventListener>();
            
            });
        }

        public override void Register()
        {
            //todo 加载通用资源
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
            GameObject uiObj = GameObject.Instantiate(res.content.GetAsset<GameObject>(_ui.uiInfo.assetName));
            if (uiObj != null)
            {
                UGUIEntity uiEntity = new UGUIEntity(uiObj);
                _ui.uiEntity = uiEntity;

                if (_ui.uiInfo.isNeedUIMask)
                {
                    Transform mask = GetUIMask(_ui.uiInfo.uiMaskCustomColor);
                    uiEntity.UIMask = mask.GetComponent<UIEventListener>();
                    mask.SetParent(uiEntity.Transform);
                    mask.SetSiblingIndex(0);
                }
                _ui.currUILayer = _ui.uiInfo.layerType;
                uiLayerWindowDict[_ui.currUILayer].AddChild(uiEntity);
                uiEntity.Transform.localPosition = Vector3.zero;
            }
        }


        private Transform GetUIMask(Color uiMaskCustomColor)
        {
            GameObject mask = UIMgr.Instance.ui_GObjectsPool.Get(UIConst.UGUI_Mask);
            mask.GetComponent<Image>().color = uiMaskCustomColor;
            return mask.transform;
        }

        private string GetUIPath(UIInfo uIInfo)
        {
            return uIInfo.packageName+"/"+uIInfo.assetName;
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

        private class Window
        {
            public UILayerType layerType;
            public Transform trf;

            public Dictionary<string, UGUIEntity> childDic = new Dictionary<string, UGUIEntity>();

            public void AddChild(UGUIEntity entity)
            {
                entity.Transform.SetParent(trf);
   
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

            public void Dispose()
            {
                trf = null;
                childDic.Clear();
            }
        }
    }
}
