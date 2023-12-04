/****************************************************
    文件: UGUIEntity.cs
    作者: Clear
    日期: 2023/4/23 14:58:37
    类型: 框架核心脚本(请勿修改)
    功能: UGUI实体
*****************************************************/
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FutureCore;
using ProjectApp.UGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectApp
{
    public class UGUIEntity : UIEntity
    {
        private Dictionary<string, Component> componentPool = new Dictionary<string, Component>();
        private Dictionary<string, GameObject> gameObjectPool = new Dictionary<string, GameObject>();


        public GameObject UI { get; private set; }
        public RectTransform Transform { get; private set; }
        public SortingGroup SortingGroup { get; private set; }
        public bool Visible { get; private set; }

        public UIEventListener UIMask;

        private TweenerCore<Vector3, Vector3, VectorOptions> openUI_tweener;
        private TweenerCore<Vector3, Vector3, VectorOptions> colseUI_tweener;

        public UGUIEntity(GameObject ui)
        {
            UI = ui;
            Transform = ui.GetComponent<RectTransform>();


            SortingGroup = ui.GetComponent<SortingGroup>();
            if (SortingGroup == null) SortingGroup = ui.gameObject.AddComponent<SortingGroup>();

            SortingGroup.sortingLayerID = SortingLayer.NameToID("UI");
            Visible = true;
            Name = ui.name;
            ui.gameObject.SetActive(true);
        }
        /// <summary>
        /// 获取对应路径名字的组件
        /// </summary>
        /// <typeparam name="T">组件名</typeparam>
        /// <param name="namePath">路径名</param>
        /// <param name="isAdd">是否要添加</param>
        /// <param name="isCance">是否缓存</param>
        /// <returns></returns>
        public T GetComponent<T>(string namePath, bool isAdd = false, bool isCance = false) where T : Component
        {
            if (Transform == null) return null;
            Component _component = null;
            if (!componentPool.TryGetValue(namePath, out _component))
            {
                Transform trf = Transform.FindTransformByName(namePath);
                if (trf)
                {
                    _component = trf.GetComponent<T>();
                    if (!_component && isAdd)
                    {
                        _component = trf.gameObject.AddComponent<T>();
                    }
                }
                else return null;
                if (isCance)
                {
                    componentPool.Add(namePath, _component);
                }
            }
            return _component as T;
        }

        public RectTransform GetTransform(string namePath, bool isCance = false)
        {
            return GetComponent<RectTransform>(namePath, false, isCance);
        }

        public GameObject GetGameObject(string namePath, bool isCance = false)
        {
            if (Transform == null) return null;
            GameObject _go = null;
            if (!gameObjectPool.TryGetValue(namePath, out _go))
            {
                Transform trf = Transform.FindTransformByName(namePath);
                if (trf)
                {
                    _go = trf.gameObject;
                }
                if (isCance)
                {
                    gameObjectPool.Add(namePath, _go);
                }
            }
            return _go;
        }

        protected override void SetName(string value)
        {
            base.SetName(value);
            Transform.name = value;
        }

        public override void SetVisible(bool State)
        {
            if (UI == null) return;
            if (State != Visible)
            {
                UI.SetActive(State);
                Visible = State;
            }
        }

        public override void OpenUIAnim(Action onComplete)
        {
            if (UI == null) return;

            Transform.localScale = new Vector2(UIMgrConst.OpenUIAnimEffectScale.x, UIMgrConst.OpenUIAnimEffectScale.y);
            openUI_tweener = Transform.DOScale(VectorConst.One, UIMgrConst.UIAnimEffectTime).SetEase(Ease.InOutBack);
            onComplete += RestOpenUI_tweener;
            openUI_tweener.onComplete = new TweenCallback(onComplete);

        }

        public override void CloseUIAnim(Action onComplete)
        {
            if (UI == null) return;

            Transform.localScale = new Vector2(UIMgrConst.OpenUIAnimEffectScale.x, UIMgrConst.OpenUIAnimEffectScale.y);
            colseUI_tweener = Transform.DOScale(UIMgrConst.OpenUIAnimEffectScale, UIMgrConst.UIAnimEffectTime).SetEase(Ease.InBack);
            onComplete += RestColseUI_tweener;
            colseUI_tweener.onComplete = new TweenCallback(onComplete);
        }

        private void RestOpenUI_tweener()
        {
            openUI_tweener = null;
        }
        private void RestColseUI_tweener()
        {
            openUI_tweener = null;
        }

        public override void Dispose()
        {
            componentPool.Clear();
            componentPool = null;
            gameObjectPool.Clear();
            gameObjectPool = null;

            UIMask = null;
            UI.Destroy();
            UI = null;
            Transform = null;
            Visible = false;
            SortingGroup = null;
            base.Dispose();
        }

    }

}