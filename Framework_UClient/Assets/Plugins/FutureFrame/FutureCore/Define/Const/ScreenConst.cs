/****************************************************************************
* ScriptType: 主框架
* 请勿修改!!!
****************************************************************************/

using UnityEngine;

namespace FutureCore
{
    /// <summary>
    /// 屏幕常量
    /// 竖版游戏UI固定宽度
    /// 横版游戏UI固定高度
    /// </summary>
    public static class ScreenConst
    {
        /// <summary>
        /// 标准分辨率
        /// </summary>
        public static Vector2 StandardResolution = new Vector2(AppConst.StandardResolution.x, AppConst.StandardResolution.y);
        /// <summary>
        /// 标准分辨率宽
        /// </summary>
        public static float StandardWidth { get { return AppConst.StandardResolution.x; } }
        /// <summary>
        /// 标准分辨率高
        /// </summary>
        public static float StandardHeight { get { return AppConst.StandardResolution.y; } }

        /// <summary>
        /// 原分辨率
        /// </summary>
        public static Vector2 RawResolution = new Vector2(Screen.width, Screen.height);
        /// <summary>
        /// 当前分辨率
        /// </summary>
        public static Vector2 CurrResolution = new Vector2(Screen.width, Screen.height);

        /// <summary>
        /// 高度固定适配 1280H
        /// </summary>
        public const int MatchHeight = 1;
        /// <summary>
        /// 宽度固定适配 720W
        /// </summary>
        public const int MatchWidth = 0;

        /// <summary>
        /// 正交大小 1280H
        /// </summary>
        public static float OrthographicSize_1280H = StandardHeight / 2 / AppConst.PixelsPerUnit;

        /// <summary>
        /// 基准高宽比
        /// 16:9 = 1.7778
        /// 9:16 = 0.5625
        /// </summary>
        public static float StandardAspectRatio = StandardHeight / StandardWidth;
        /// <summary>
        /// 当前高宽比
        /// </summary>
        public static float CurrAspectRatio = (float)Screen.height / Screen.width;

        /// <summary>
        /// 获取当前分辨率信息
        /// </summary>
        public static string CurrResolutionInfo = string.Format("{0}x{1}", CurrResolution.x, CurrResolution.y);

        /// <summary>
        /// 适配宽度计算
        /// </summary>
        public static float AdaptiveWidthCompute = Screen.height * StandardAspectRatio;
        /// <summary>
        /// 适配高度计算
        /// </summary>
        public static float AdaptiveHeightCompute = Screen.width * StandardAspectRatio;

        /// <summary>
        /// 适配宽度比例
        /// </summary>
        public static float AdaptiveWidthRatio = Screen.width / AdaptiveWidthCompute;
        /// <summary>
        /// 适配高度比例
        /// </summary>
        public static float AdaptiveHeightRatio = Screen.height / AdaptiveHeightCompute;

        /// <summary>
        /// 宽度比例
        /// </summary>
        public static float WidthRatio = Screen.width / StandardWidth;
        /// <summary>
        /// 高度比例
        /// </summary>
        public static float HeightRatio = Screen.height / StandardHeight;
    }
}