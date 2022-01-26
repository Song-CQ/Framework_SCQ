/****************************************************
    文件：UIWindow.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/2/23 18:23
    功能：UI窗口父类
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XL.Common;

namespace XL.UI
{


    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        //据Unity官方文档表示，在控制UI显隐上使用阿尔法通道归零
        //比设置物体的stable性能要更好，但是没有说明为什么
        private CanvasGroup canvasGroup;

        //事件监听器对象池
        private Dictionary<string, UIEventListener> uiEventListenerDic;
        //设置窗口显现的协程
        private IEnumerator setVisibleIEtor = null;
        protected Action<CanvasGroup, bool> CutWndStateAction;
        public bool WindowState { get; private set; } = true;
        public bool IsStart { get; protected set; } = false;
        protected Animation OpenWndAni;
        [Header("BaseWnd")]
        [SerializeField]
        protected bool IsPlayOpenWndAni = false;
        [SerializeField]
        protected bool IsPlayCloseWndAni = false;


        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            uiEventListenerDic = new Dictionary<string, UIEventListener>();
            if (IsPlayOpenWndAni)
            {
                OpenWndAni = GetComponent<Animation>();
            }
        }

        public virtual void SetCanvasGroup(float val)
        {
            if (canvasGroup)
            {
                canvasGroup.alpha = val;
            }
        }

        public virtual void StartWnd()
        {
            if (OpenWndAni)
            {
                OpenWndAni = GetComponent<Animation>();
            }
            CurrPlanePin.Id = -1;
            IsStart = true;
            InitUIEvent();
        }


        public virtual void RefreshUI() { }
        public virtual void Init() { }

        protected virtual void InitUIEvent() { }
        public virtual void Clear()
        {
            WindowState = false;
            foreach (var item in PlanePinDic)
            {
                item.Value.CutPlaneCb?.Invoke(false);
            }
            if (IsPlayCloseWndAni)
            {

                if (OpenWndAni.isPlaying)
                {
                    OpenWndAni.Stop();
                }
                OpenWndAni[OpenWndAni.clip.name].speed = -1;
                OpenWndAni[OpenWndAni.clip.name].time = OpenWndAni[OpenWndAni.clip.name].length;
                OpenWndAni.Play();
            }
        }


        /// <summary>
        /// 设置窗口的显隐
        /// </summary>
        /// <param name="state">显隐状态</param>
        /// <param name="delay">延长时间</param>
        /// <param name="cb">显隐方式</param>
        public void SetVisibleWnd(bool state, float delay = 0)
        {

            if (WindowState == state) return;
            WindowState = state;
            if (CutWndStateAction != null)
            {
                CutWndStateAction(canvasGroup, state);
            }
            else
            {
                if (canvasGroup)
                {
                    SetCanvasGroup(state);
                    //gameObject.SetActive(state);                                       
                }
            }
            if (state)
            {
                if (IsPlayOpenWndAni && OpenWndAni)
                {
                    OpenWndAni[OpenWndAni.clip.name].speed = 1;
                    OpenWndAni[OpenWndAni.clip.name].time = 0;
                    OpenWndAni.Play();
                }
                Init();

            }
            else
            {
                Clear();
            }

        }

        protected void SetCanvasGroup(bool state)
        {
            SetCanvasGroup(state ? 1 : 0);
            canvasGroup.blocksRaycasts = state;
        }

        /// <summary>
        /// 获取UI元素的事件监听组件
        /// </summary>
        /// <param name="name">要获取的UI元素的名字</param>
        /// <returns></returns>
        public UIEventListener GetUIEventListener(string name)
        {
            UIEventListener eventListener = null;
            if (!uiEventListenerDic.TryGetValue(name, out eventListener))
            {
                Transform tf = transform.FindTransformByName(name);
                eventListener = UIEventListener.GetEventListener(tf);
            }
            return eventListener;

        }
        #region  CutPlanePin

        protected struct PlanePin
        {
            public string Name;
            public int LastId;
            public int Id;
            public Action<bool> CutPlaneCb;

            public PlanePin(int _id, int _LastId = -1, string _name = null, Action<bool> _CutPlane = null)
            {
                Name = _name;
                Id = _id;
                LastId = _LastId;
                CutPlaneCb = _CutPlane;
            }
            public PlanePin(object _id = null, object _LastId = null, string _name = null, Action<bool> _CutPlane = null)
            {
                Name = _name;
                if (_id != null)
                {
                    Id = (int)_id;
                }
                else
                {
                    Id = -1;
                }
                if (_LastId != null)
                {
                    LastId = (int)_LastId;
                }
                else
                {
                    LastId = -1;
                }
                CutPlaneCb = _CutPlane;
            }

        }
        protected Dictionary<int, PlanePin> PlanePinDic = new Dictionary<int, PlanePin>();

        protected void AddPlanePin(PlanePin planePin)
        {
            PlanePinDic.Add(planePin.Id, planePin);
        }
        protected PlanePin CurrPlanePin;
        protected void CutPlanePin(PlanePin planePin)
        {
            if (CurrPlanePin.Id != -1)
            {
                CurrPlanePin.CutPlaneCb?.Invoke(false);
            }
            planePin.CutPlaneCb?.Invoke(true);
            CurrPlanePin = planePin;
        }
        protected void CutPlanePin(int planePinId)
        {
            if (PlanePinDic.TryGetValue(planePinId, out PlanePin planePin))
            {
                CutPlanePin(planePin);
            }
        }
        protected void CutPlanePin(object planePinId)
        {
            if (PlanePinDic.TryGetValue((int)planePinId, out PlanePin planePin))
            {
                CutPlanePin(planePin);
            }
        }
        #endregion


        #region Tool Functions

        protected void SetActive(GameObject go, bool state = true)
        {
            go.SetActive(state);
        }
        protected void SetActive(Transform trans, bool state = true)
        {
            trans.gameObject.SetActive(state);
        }
        protected void SetActive(RectTransform rectTrans, bool state = true)
        {
            rectTrans.gameObject.SetActive(state);
        }
        protected void SetActive(Image img, bool state = true)
        {
            img.transform.gameObject.SetActive(state);
        }
        protected void SetActive(Text txt, bool state = true)
        {
            txt.transform.gameObject.SetActive(state);
        }

        protected void SetText(Text txt, string context = "")
        {
            txt.text = context;
        }
        protected void SetText(Transform trans, int num = 0)
        {
            SetText(trans.GetComponent<Text>(), num);
        }

        protected void SetText(Text txt, int num = 0)
        {
            SetText(txt, num.ToString());
        }
        protected void SetText(Text txt, float num = 0)
        {
            SetText(txt, num.ToString());
        }

        protected void SetText(Transform trans, string context = "")
        {
            SetText(trans.GetComponent<Text>(), context);
        }

        protected Transform FindObj(string name, Transform pa = null)
        {
            Transform obj = null;
            if (pa != null)
            {
                obj = pa.Find(name);
            }
            else
            {
                obj = transform.Find(name);
            }
            return obj;
        }

        #endregion
    }
}
