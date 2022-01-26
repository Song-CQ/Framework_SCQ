/****************************************************
    文件：VectorConst.cs
	作者：Clear
    日期：2022/1/25 19:56:40
    类型: 框架核心脚本(请勿修改)
	功能：Nothing
*****************************************************/
using UnityEngine;

namespace FutureCore
{
    public static class VectorConst
    {
        public static Vector3 Zero = Vector3.zero;
        public static Vector3 One = Vector3.one;
        public static Vector3 PPUOne = One * AppConst.PixelsPerUnit;
        public static Vector3 Half = new Vector3(0.5f, 0.5f, 0.5f);
        public static Vector3 XMirror = new Vector3(-1f, 1f, 1f);
    }
}