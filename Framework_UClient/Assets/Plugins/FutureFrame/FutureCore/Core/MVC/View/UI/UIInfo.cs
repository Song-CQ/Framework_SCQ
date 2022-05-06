/****************************************************
    文件：UIInfo.cs
	作者：Clear
    日期：2022/1/25 11:2:16
    类型: 框架核心脚本(请勿修改)
	功能：UI信息类
*****************************************************/
using System;
using UnityEngine;

namespace FutureCore
{
    public static class UIInfoConst
    {
        public static Color DefaultUIMaskColor = new Color(0, 0, 0, 0.85f);
    }
    public class UIInfo
    {
        public string packageName = null;
        public string assetName = null;

        public UILayerType layerType = UILayerType.Normal;

        public uint openUIMsgId = 0;
        public uint closeUIMsgId = 0;

        /// <summary>
        /// 是否需要Updade函数
        /// </summary>
        public bool isTickUpdate = false;
        /// <summary>
        /// 是否需要默认遮罩
        /// </summary>
        public bool isNeedUIMask = false;
        // 是否需要UI打开动画
        public bool isNeedOpenAnim = false;
        // 是否需要UI关闭动画
        public bool isNeedCloseAnim = false;
        // UI底部遮罩自定义颜色
        public Color uiMaskCustomColor = UIInfoConst.DefaultUIMaskColor;

        public void Reset()
        {
            packageName = null;
            assetName = null;           
            openUIMsgId = 0;
            closeUIMsgId = 0;
            layerType = UILayerType.Normal;
            isNeedOpenAnim = false;
            isNeedCloseAnim = false;
            isTickUpdate = false;
        }
    }

    public enum UILayerType : int
    {
        None = -1,

        Background = 0,
        Bottom = 1,

        Normal = 2,
        Top = 3,

        FullScreen = 4,
        Popup = 5,

        Highest = 6,
        Animation = 7,
        Tips = 8,

        Loading = 9,
        System = 10,
    }


}