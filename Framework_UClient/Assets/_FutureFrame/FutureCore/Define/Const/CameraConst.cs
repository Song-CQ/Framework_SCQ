/****************************************************
    文件：CameraConst.cs
	作者：Clear
    日期：2022/1/16 15:34:16
    类型: 框架核心脚本(请勿修改)
	功能：摄像机常量
*****************************************************/

using UnityEngine;

namespace FutureCore
{
    public static class CameraConst
    {
        public const int MainDepth = 0;
        public const TransparencySortMode MainCameraSortMode = TransparencySortMode.Orthographic;

        public static int MainCameraOrthographicSize = 10;
        public static float MainCameraFarClipPlane = 60;
        public const int MainCameraPosValue = 0;
        public const int MainCameraZPos = 0;

        public const int UICameraDepth = 10;
        public static float UICameraFarClipPlane = 100;
        public const int UICameraPosValue = 10000;

        public static Vector3 MainCameraPos = new Vector3(MainCameraPosValue, MainCameraPosValue, MainCameraZPos);
        public static Vector3 UICameraPos = new Vector3(UICameraPosValue, UICameraPosValue, MainCameraZPos);

    }
}