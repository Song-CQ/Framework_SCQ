/****************************************************
    文件：BaseUIDriver.cs
	作者：Clear
    日期：2022/1/15 18:6:37
    类型: 框架核心脚本(请勿修改)
	功能：基础UI驱动
*****************************************************/
using System;
using UnityEngine;

namespace FutureCore
{
    public abstract class BaseUIDriver
    {
        /// <summary>
        /// 默认字体
        /// </summary>
        protected string uiDefaultFontName;

        public abstract void Register();
        public abstract void Init();

        public abstract void StartUp();

        public abstract void Dispose();

        public abstract void InitUILayer();

        public abstract Window GetWindow(UILayerType uILayerType);
        public abstract void LoadUI(BaseUI baseUI, object args, Action<BaseUI, object> openUIProcess);
        public abstract void DestroyUI(BaseUI ui);
        public virtual void RegisterDefaultFont(string font)
        {
            uiDefaultFontName = font;
        }


    }
    public abstract class Window
    {
        public UILayerType layerType;
        public abstract void Dispose();
        
    }
}
    