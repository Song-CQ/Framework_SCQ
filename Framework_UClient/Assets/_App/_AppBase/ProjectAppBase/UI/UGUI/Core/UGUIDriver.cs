/****************************************************
    �ļ�: UGUIDriver.cs
    ����: Clear
    ����: 2023/4/23 14:58:37
    ����: ��ܺ��Ľű�(�����޸�)
    ����: UGUI����

    UIGUI���������淶:
    1:������·�������� /UGUI/XXX(������߰���)_UIPack/������_Wnd

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
            canvas = new GameObject("UIRoot").AddComponent<Canvas>();
            UIRoot = canvas.transform;
            UIRoot.gameObject.SetParent(AppObjConst.UIGo);
            UIRoot.localPosition = Vector3.zero;
            UIRoot.gameObject.layer = LayerMaskConst.UI;
           
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
                string name = UILayerConst.AllUILayer[i];//��ʵÿ��UIlayer��Ӧ���Ǹ�Canvas������������
                RectTransform rtrf = new GameObject(name).AddComponent<RectTransform>();
                rtrf.SetParent(UIRoot);
                rtrf.gameObject.layer = LayerMaskConst.UI;
                rtrf.pivot = new Vector2(0.5f,0.5f);
                rtrf.anchorMax = Vector2.one;
                rtrf.anchorMin = Vector2.zero;
                rtrf.sizeDelta = Vector2.zero;

                rtrf.localPosition = new Vector3(0,0, i * 1);
                rtrf.localScale = Vector3.one;
                Window window = new Window() {
                    layerType = (UILayerType)i,
                    r_trf = rtrf
                };

                uiLayerWindowDict[window.layerType] = window;
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
            //todo ����ͨ����Դ
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
            //todo ������Կ��Ƕ������ϲ�����
            //��Դ��
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
                uiEntity.Transform.pivot = new Vector2(0.5f, 0.5f);
                uiEntity.Transform.anchorMax = Vector2.one;
                uiEntity.Transform.anchorMin = Vector2.zero;

                if (_ui.uiInfo.isNeedUIMask)
                {
                    RectTransform mask = GetUIMask(_ui.uiInfo.uiMaskCustomColor);
                    uiEntity.UIMask = mask.GetComponent<UIEventListener>();
                    mask.SetParent(uiEntity.Transform);
                    mask.SetSiblingIndex(0);
                    mask.sizeDelta = Vector2.zero;
                }
                _ui.currUILayer = _ui.uiInfo.layerType;
                
                uiLayerWindowDict[_ui.currUILayer].AddChild(uiEntity);
                //���س�����ƥ�䵱ǰ�㼶������
                uiEntity.Transform.sizeDelta = Vector2.zero;
                uiEntity.Transform.localPosition = Vector3.zero;
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
            return uIInfo.packageName+"_UIPack/"+uIInfo.assetName+"_Wnd";
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
            public RectTransform r_trf;

            public Dictionary<string, UGUIEntity> childDic = new Dictionary<string, UGUIEntity>();

            public void AddChild(UGUIEntity entity)
            {
                entity.Transform.SetParent(r_trf);//�����˸��ڵ��Ҫ���� sizeDelta
               
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
                r_trf = null;
                childDic.Clear();
            }
        }
    }
}