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
using XL.Common;
using UnityEngine.UI;

namespace XL.UI
{
   
    public class UIWindow : MonoBehaviour
    {
        
        //事件监听器对象池
        private Dictionary<string, UIEventListener> uiEventListenerDic;
       
        private void Awake()
        {    
            uiEventListenerDic = new Dictionary<string, UIEventListener>();
        }

        public virtual void Init()
        {

        }

        public virtual void Clear()
        {

        }

        /// <summary>
        /// 设置窗口的显隐
        /// </summary>
        /// <param name="state">显隐状态</param>
        /// <param name="delay">延长时间</param>
        /// <param name="cb">显隐方式</param>
        public void SetVisibleWnd(bool state,float delay=0,Action<bool> cb=null)
        {
            if (cb != null)
            {
                cb(state);
            }
            else
            {
                gameObject.SetActive(state);
            }
          
            if (state)
            {
                Init();
            }
            else
            {
                Clear();
            }
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
