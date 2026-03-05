/****************************************************
    文件：UIManager.cs
    作者：相柳
    邮箱: Song-CQ@Outlook.com
    日期：2020/2/23 18:43
    功能：UI窗口管理器
*****************************************************/
using FutureCore;
using System.Collections.Generic;

namespace ProjectApp.UGUI
{
    public class UIManager : SingletonMono<UIManager>
    {

        private Dictionary<string, UIWindow> uiWindowsDic;

        public override void Init()
        {

            uiWindowsDic = new Dictionary<string, UIWindow>();

            UIWindow[] uiWindows = FindObjectsOfType<UIWindow>();
            foreach (var item in uiWindows)
            {
                if (!item.IsStart)
                    item.StartWnd();
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
            if (!window.IsStart)
                window.StartWnd();
            window.SetVisibleWnd(false);
            //window.SetCanvasGroup(0);
            uiWindowsDic.Add(window.GetType().Name, window);
            return true;
        }

        public bool Clear<T>()
        {
            if (!uiWindowsDic.TryGetValue(typeof(T).Name, out UIWindow uIWindow))
                return false;
            uIWindow.Clear();
            uiWindowsDic.Remove(typeof(T).Name);
            return true;
        }

        public void ClearAll()
        {
            if (uiWindowsDic != null)
            {
                foreach (var item in uiWindowsDic)
                {
                    item.Value.Clear();
                }
                uiWindowsDic.Clear();
            }
        }



    }
}
