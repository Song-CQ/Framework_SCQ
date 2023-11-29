/****************************************************
    文件: EffectEntity.cs
    作者: Clear
    日期: 2023/11/28 17:54:23
    类型: 框架核心脚本(请勿修改)
    功能: 特效实体
*****************************************************/
using System;
using UnityEngine;

namespace FutureCore
{
    public class EffectEntity : MonoBehaviour
    {
        public ParticleSystem main_ParticleSystem;
        public event Action Event_OnParticleSystemStopped;

        public void OnParticleSystemStopped()
        {
            Event_OnParticleSystemStopped?.Invoke();
        }
    }
}