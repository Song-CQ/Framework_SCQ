/****************************************************
    文件: EffectData.cs
    作者: Clear
    日期: 2023/11/28 16:9:29
    类型: 框架核心脚本(请勿修改)
    功能: 特效数据
*****************************************************/
using UnityEngine;

namespace FutureCore
{
    public enum StopType
    {
        /// <summary>
        /// 默认 依赖于EffectData的存活时间
        /// </summary>
        Default,
        /// <summary>
        /// 依赖于主特效播放结束后 如果没有主特效 默认为Default
        /// </summary>
        ParticleSystemStopped_ToMain = 1,
    }
    public class EffectData
    {
        /// <summary>
        /// 特效名
        /// </summary>
        public string effectName;
        /// <summary>
        /// 特效路径
        /// </summary>
        public string effectPath;
        /// <summary>
        /// 特效结束类型
        /// </summary>
        public StopType stopType;
        /// <summary>
        /// 存活时间（-1为循环）
        /// </summary>
        public float lifeTime = 1;



    }
}