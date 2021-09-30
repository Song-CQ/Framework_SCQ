/****************************************************
    文件：UIManager.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/2/23 18:43
    功能：UI窗口管理器
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using FutureCore;
using UnityEngine;
using XL.Common;

namespace XL.UI
{
    public class UIManager : MonoSingleton<UIManager>
    {

        private Dictionary<string, UIWindow> uiWindowsDic;
       
        public override void Init()
        {
            uiWindowsDic = new Dictionary<string, UIWindow>();
        
            UIWindow[] uiWindows=FindObjectsOfType<UIWindow>();
            foreach (var item in uiWindows)
            {
                item.SetVisibleWnd(false);
                uiWindowsDic.Add(item.GetType().Name, item);
            }
        }

        public T GetUIWindow<T>() where T : UIWindow
        {
            UIWindow wnd = null;
            
            if (uiWindowsDic.TryGetValue(typeof(T).Name, out wnd))
            {
                return wnd as T;
            }
            return null;

        }

        public bool Add(UIWindow window)
        {
            if (uiWindowsDic.ContainsKey(window.GetType().Name))
                return false;            
            uiWindowsDic.Add(window.GetType().Name, window);
            return true;
        }

        public bool Clear<T>()
        {
            if (!uiWindowsDic.ContainsKey(typeof(T).Name))
                return false;
            uiWindowsDic.Remove(typeof(T).Name);
            return true;
        }

       
        

    }
}
